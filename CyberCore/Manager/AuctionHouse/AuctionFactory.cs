using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using CyberCore.Utils;
using fNbt;
using MiNET;
using MiNET.Blocks;
using MiNET.Items;
using MiNET.Net;
using MiNET.Utils;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace CyberCore.Manager.AuctionHouse
{
    public class AuctionFactory
    {
        // private final AHSqlite Sqlite;
        CyberCoreMain CCM;

        /**
     * InternalPlayerSettings:
     * Key: {
     * id:
     * meta:
     * count:
     * namedtag:
     * cost:
     * soldby:
     * }
     */
        AFSettings Settings;

        public readonly String Path = "./AF.js";


        List<AuctionItemData> items = new List<AuctionItemData>();

        public AuctionFactory(CyberCoreMain CCM)
        {
            this.CCM = CCM;
            loadFromJSONFile();
            // Sqlite = new AHSqlite(CCM);
        }

        public void loadFromJSONFile()
        {
            if (File.Exists(Path)) Settings = JsonConvert.DeserializeObject<AFSettings>(File.ReadAllText(Path));
        }


        public List<AuctionItemData> GetAllItems()
        {
            List<AuctionItemData> ils = new List<AuctionItemData>();


            MySqlDataReader q = CCM.SQL.Query("select * from `AuctionHouse` where `purchased` != 1");
            if (q != null)
            {
                while (q.Read())
                {
                    AuctionItemData aid = new AuctionItemData(q);
                    ils.Add(aid);
                }
            }

            CyberCoreMain.Log.Info("AF>>Loaded " + ils.Count + " Items for AH1");
            return ils;
        }


        public List<AuctionItemData> GetAllItemsLimit(int start, int stop, String seller)
        {
            List<AuctionItemData> ils = new List<AuctionItemData>();

            MySqlDataReader r =
                CCM.SQL.Query($"SELECT * FROM `AuctionHouse` WHERE `purchased` != true LIMIT {start},{stop}");
            ;
            if (seller != null)
                r = CCM.SQL.Query(
                    $"SELECT * FROM `AuctionHouse` WHERE `soldbyn` = '{seller}' AND `purchased` != true LIMIT {start},{stop}");

            while (r.Read())
            {
                AuctionItemData aid = new AuctionItemData(r);
                ils.Add(aid);
            }

            return ils;
        }

        //@Todo
//    public List<Item> getSoldItems() {
//        List<Item> ils = new List<>();
//        ResultSet rs = null;//ExecuteQuerySQLite("SELECT * FROM `auctions` WHERE `purchased` != 1");
//        if (rs != null) {
//            try {
//                while (rs.next()) {
//                    int id = rs.getInt("id");
//                    int item_id = rs.getInt("item-id");
//                    int item_meta = rs.getInt("item-meta");
//                    int item_count = rs.getInt("item-count");
//                    byte[] namedtag = rs.getString("namedtag").getBytes();
//                    int cost = rs.getInt("cost");
//                    String soldby = rs.getString("soldby");
//
//                    Item i = Item.get(item_id, item_meta, item_count);
//                    i.setCompoundTag(namedtag);
//
//                    CompoundTag tag = i.getNamedTag();
//                    if (tag == null) tag = new CompoundTag();
//
//                    if (!i.getCustomName().equals("") && tag.contains("display") && tag.get("display") instanceof CompoundTag)
//                        tag.getCompound("display").putString("Name2", i.getCustomName());
//
//                    if (tag.contains("display") && tag.get("display") instanceof CompoundTag) {
//                        tag.getCompound("display").putInt("keyid", id);
//                        tag.getCompound("display").putInt("cost", cost);
//                    } else {
//                        tag.putCompound("display", (new CompoundTag("display")).putInt("keyid", id).putInt("cost", cost));
//                    }
//                    i.setNamedTag(tag);
//
//
//                    String cn = i.getCustomName();
//
//                    if (cn.equalsIgnoreCase("")) cn = i.getName();
//
//                    cn += TextFormat.RESET + "\n" + TextFormat.AQUA +
//                            "-------------" + TextFormat.RESET + "\n" +
//                            TextFormat.GREEN + "$" + cost + TextFormat.RESET + "\n" +
//                            TextFormat.GOLD + "Sold By: " + soldby
//                    // + TextFormat.RESET + "\n" +TextFormat.BLACK+"{#"+id;
//                    ;
//
//                    i.setCustomName(cn);
//
//                    ils.add(i);
//                }
//            } catch (Exception ex) {
//                ex.printStackTrace();
//                CCM.getLogger().info("ERror loading Items!");
//                return null;
//            }
//            CCM.getLogger().info("Loaded " + ils.Count + " Items for AH");
//            items = ils;
//        }
//        return null;
//    }

        public List<Item> getListOfItems()
        {
            List<Item> il = new List<Item>();
            foreach (var ahd in GetAllItems())
            {
                il.Add(ahd.MakePretty());
            }

            return il;
        }

        public List<Item> getListOfItemsBetween(int start, int stop)
        {
            return getListOfItemsBetween(start, stop, null);
        }

        public List<AuctionItemData> getListOfAIDBetween(int start, int stop)
        {
            return getListOfAIDBetween(start, stop, null);
        }

        public List<AuctionItemData> getListOfAIDBetween(int start, int stop, String seller)
        {
            List<AuctionItemData> il = new List<AuctionItemData>();
//        if(GetAllItemsLimit(start, stop) == null)System.out.println("YEAAAAAAAAAA THISSSSS SSSHSHHHHIIIITTT NUUUULLLLLLIINNNNN~!!!!!!!!");
            foreach (var ahd in GetAllItemsLimit(start, stop, seller))
            {
                il.Add(ahd);
            }

            return il;
        }

        public List<Item> getListOfItemsBetween(int start, int stop, String seller)
        {
            List<Item> il = new List<Item>();
            foreach (var ahd in GetAllItemsLimit(start, stop, seller))
            {
                il.Add(ahd.MakePretty());
            }

            return il;
        }

//    public Item getItem(int page, int slot) {
//        int stop = page * 45;
//        int start = stop - 45;
//        int key = start + slot;
//        return items.get(key);
//    }


        public Dictionary<int, Item> getPageHash(int page, String seller = null)
        {
            Dictionary<int, Item> list = new Dictionary<int, Item>();
            int k = 0;
            if (seller == null)
            {
                foreach (var i in getPage(page))
                {
                    list[k] = i;
                    k++;
                }
            }
            else
            {
                foreach (var i in SetPagePlayerSelling(seller, page))
                {
                    list[k] = i;
                    k++;
                }
            }

            return list;
        }

        public List<Item> SetPagePlayerSelling(String seller, int page)
        {
            int stop = page * 45;
            int start = stop - 45;
//        System.out.println("START = " + start + ", STOP = " + stop + " Seller" + seller);
            List<Item> list2 = getListOfItemsBetween(start, stop, seller);
            if (45 > list2.Count)
            {
                List<Item> a = new List<Item>();
                for (int i = 0; i < 45; i++)
                {
//                list2.iterator().n
                    if (list2.Count > i && list2[i] != null)
                    {
//                    System.out.println("ADDING ACTUAL ITEM " + list2.get(i).getId());
                        a.Add(list2[i]);
                    }
                    else
                    {
                        a.Add(new ItemAir());
//                    System.out.println("ADDING AIR");
                    }
                }

                return a;
            }
            else
            {
                return list2;
            }
        }

        public Item getItemFromPage(int page, int slot)
        {
            if (slot > 45)
            {
                CyberCoreMain.Log.Error("ERROR! Slot out of range! E443 Slot:" + slot);
                return null;
            }

            List<Item> list = getPage(page);
            if (slot > list.Count)
            {
                CyberCoreMain.Log.Error("ERROR! Selected Slot out of List Range! E33342 SLOT:" + slot + " OF " +
                                        list.Count);
                return null;
            }

            Item s = list[slot];
            if (s.Id == new ItemAir().Id) return null;
            return s;
        }

        public AuctionItemData getAIDFromPage(int page, int slot)
        {
            if (slot > 45)
            {
                CyberCoreMain.Log.Error("ERROR! Slot out of range! E443 Slot:" + slot);
                return null;
            }

            List<AuctionItemData> list = getPageAID(page);
            if (slot > list.Count)
            {
                CyberCoreMain.Log.Error("ERROR! Selected Slot out of List Range! E33342 SLOT:" + slot + " OF " +
                                        list.Count);
                return null;
            }

            return list[slot];
        }

        public List<AuctionItemData> getPageAID(int page)
        {
            int stop = page * 45;
            int start = stop - 45;
//        System.out.println("START = " + start + ", STOP = " + stop);
            List<AuctionItemData> list2 = getListOfAIDBetween(start, stop);
            if (45 > list2.Count)
            {
                List<AuctionItemData> a = new List<AuctionItemData>();
                for (int i = 0; i < 45; i++)
                {
//                list2.iterator().n
                    if (list2.Count > i && list2[i] != null)
                    {
//                    System.out.println("ADDING ACTUAL ITEM " + list2.get(i).toString());
                        a.Add(list2[i]);
                    }
                    else
                    {
                        a.Add(null);
//                    System.out.println("ADDING AIR");
                    }
                }

                return a;
            }
            else
            {
                return list2;
            }
        }

        public List<Item> getPage(int page)
        {
            int stop = page * 45;
            int start = stop - 45;
//        System.out.println("START = " + start + ", STOP = " + stop);
            List<Item> list2 = getListOfItemsBetween(start, stop);
            if (45 > list2.Count)
            {
                List<Item> a = new List<Item>();
                for (int i = 0; i < 45; i++)
                {
//                list2.iterator().n
                    if (list2.Count > i && list2[i] != null)
                    {
//                    System.out.println("ADDING ACTUAL ITEM || " + list2.get(i).getId());
                        a.Add(list2[i]);
                    }
                    else
                    {
                        a.Add(new ItemAir());
//                    System.out.println("ADDING AIR ||");
                    }
                }

                return a;
            }
            else
            {
                return list2;
            }
//
//        List<Item> list = new List<>();
//
//        for (int a = start; a < list2.Count; a++) {
//            if (a >= stop) break;
//            Item newitem = list2.get(a).clone();
//            System.out.println(newitem.toString());
//            if (newitem == null) list.add(new ItemBlock(new BlockAir(), (int) null, 0));
//            else list.add(newitem);
//        }
//
//        return (Item[]) list.toArray();

            /*
            1 => 0 | 44
            2 => 45 | 89
             */
        }

        public void OpenAH(CorePlayer p, int pg)
        {
            var n = new NbtCompound();
            n.Add(new NbtString("CustomName", "Auction House!"));
            SpawnFakeBlockAndEntity(p, n);
            AuctionHouse b = new AuctionHouse(p, CCM, p.KnownPosition.ToVector3(), pg);
            CyberCoreMain.Log.Info(b.getContents().values().Count + " < SIZZEEE" + b.size);
            //TODO !IMPORTANT
            // CyberCoreMain.getInstance().getServer().getScheduler().scheduleDelayedTask(new OpenAH(p, b), 5);
//        b.open()
        }

        public void SpawnFakeBlockAndEntity(Player to, NbtCompound data)
        {
            SpawnBlock(to, new Chest());
            SpawnBlockEntity(to, data);
        }

        public void SpawnBlock(Player to, Block b)
        {
            PlayerLocation a = new PlayerLocation();
            PlayerLocation aa = new PlayerLocation();
            a.X = aa.X = to.KnownPosition.X;
            a.Y = aa.Y = to.KnownPosition.Y - 2;
            a.Z = aa.Z = to.KnownPosition.Z;
            aa.Z += 1;
            BlockCoordinates l1 = new BlockCoordinates(a);
            BlockCoordinates l2 = new BlockCoordinates(aa);
            var message = McpeUpdateBlock.CreateObject();
            message.blockRuntimeId = (uint) new Chest().GetRuntimeId();
            message.coordinates = l1;
            message.blockPriority = (int) (McpeUpdateBlock.Flags.AllPriority);
            var message2 = McpeUpdateBlock.CreateObject();
            message2.blockRuntimeId = (uint) new Chest().GetRuntimeId();
            message2.coordinates = l2;
            message2.blockPriority = (int) (McpeUpdateBlock.Flags.AllPriority);
            to.SendPacket(message);
            to.SendPacket(message2);
        }

        public void SpawnBlockEntity(Player to, NbtCompound data)
        {
            PlayerLocation a = new PlayerLocation();
            PlayerLocation aa = new PlayerLocation();
            a.X = aa.X = to.KnownPosition.X;
            a.Y = aa.Y = to.KnownPosition.Y - 2;
            a.Z = aa.Z = to.KnownPosition.Z;
            aa.Z += 1;
            var nbt = new Nbt
            {
                NbtFile = new NbtFile
                {
                    BigEndian = false,
                    UseVarInt = true,
                    RootTag =  data
                }
            };
            var nbt2 = new Nbt
            {
                NbtFile = new NbtFile
                {
                    BigEndian = false,
                    UseVarInt = true,
                    RootTag =  new NbtCompound()
                    {
                        new NbtInt("pairx",(int)a.X),
                        new NbtInt("pairxz",(int)a.Z),
                    }
                }
            };
            BlockCoordinates l1 = new BlockCoordinates(a);
            BlockCoordinates l2 = new BlockCoordinates(aa);
            McpeBlockEntityData bedp = McpeBlockEntityData.CreateObject();
            McpeBlockEntityData bedp2 = McpeBlockEntityData.CreateObject();
            bedp.coordinates = l1;
            bedp.namedtag = nbt;
            bedp2.coordinates = l2;
            bedp2.namedtag = nbt2;
            
            to.SendPacket(bedp);
            to.SendPacket(bedp2);
        }

//    @EventHandler(ignoreCancelled = true)
//    public void TTE(InventoryClickEvent event) {
//
//        System.out.println("++++++++++++++++++++++++++++++++++++++++");
//        System.out.println("++++++++++++++++++++++++++++++++++++++++");
//        System.out.println(event.getSlot());
//        System.out.println(event.getInventory().getClass().getName());
//
//        System.out.println("CALLLLCLLIICCCKKK");
//                System.out.println("CALLLL SLOTCCCCCCCC");
//                int slott = event.getSlot();
//
////                sca.getInventory()
//
//                Inventory inv = event.getInventory();
//                System.out.println("CHECK INNNNNVVVVVVV " + inv.getClass().getName());
////                if (inv.isEmpty()) return;
//
//                System.out.println("NEEEEEEE" + inv.getClass().getTypeName());
//                if (inv instanceof PlayerInventory) {
//
//                }
//                if (inv instanceof AuctionHouse) {
//
//                    AuctionHouse ah = (AuctionHouse) inv;
////                    if(!ah.Init)return;
//                    System.out.println(slott + " || " + ah.getHolder().getName() + " || " + ah.getHolder().getClass().getName());
//                    CorePlayer ccpp = (CorePlayer) ah.getHolder();
//                    int slot = slott;
////                    event.setCancelled();
//                    if (slot < 5 * 9) {
//                        System.out.println("TOP INV");
//                        //TODO CONFIRM AND SHOW ITEM
//                        if (!ah.ConfirmPurchase) {
//                            ah.ConfirmItemPurchase(slot);
//                            event.setCancelled();
//                            System.out.println("SSSSSSSSSSSSCPPPPPPPP");
////                        ccpp.AH.ConfirmItemPurchase(slot);
//                        } else {
//                            System.out.println("CPPPPPPPP");
//                            Item si = ah.getContents().get(slot);
//                            if (si != null) {
//                                if (si.getId() == BlockID.EMERALD_BLOCK) {
//                                    System.out.println("CONFIRM PURCHASE!!!!!!!");
////                                    ah.SF.PurchaseItem((CorePlayer) ah.getHolder(), Page, slot);
//                                } else if (si.getId() == BlockID.REDSTONE_BLOCK) {
//                                    System.out.println("DENCLINE PURCHASE!!!!!!!!");
//                                }
//                            }
//                        }
//                    } else {
//                        switch (slot) {
//                            case AuctionHouse.MainPageItemRef.LastPage:
//                                Page--;
//                                if (Page < 1) Page = 1;
//                                Server.getInstance().getPlayerExact(ah.getHolder().getName()).sendPopup("PAGE SET TO " + Page);
//                                ah.clearAll();
//                                ah.addItem(getPage(Page));
//                                ah.ReloadInv();
//                                ah.sendContents(ah.getHolder());
//
//                                break;
//                            case AuctionHouse.MainPageItemRef.NextPage:
//                                Server.getInstance().getPlayerExact(ah.getHolder().getName()).sendTip("PAGE SET TO " + Page);
//                                Page++;
//
//                                ah.clearAll();
//                                ah.addItem(getPage(Page));
//                                ah.ReloadInv();
//                                ah.sendContents(ah.getHolder());
//                                break;
//                            case AuctionHouse.MainPageItemRef.Search:
//                                break;
//                            case AuctionHouse.MainPageItemRef.Reload:
//                                ah.clearAll();
//                                ah.addItem(getPage(Page));
//                                ah.ReloadInv();
//                                ah.SendAllSlots(ah.getHolder());
//                                break;
//                            case AuctionHouse.MainPageItemRef.PlayerSelling:
//                                ah.setContents(getPageHash(Page, ah.getHolder().getName()), true);
//                                ah.sendContents((Player) ah.getHolder());
//                                ah.SendAllSlots((Player) ah.getHolder());
//                                event.setCancelled(false);
//                                break;
//
//                        }
//                    }
//                }
//            }

        //TODO MAke Pages with new API
//    @EventHandler(ignoreCancelled = true)
//    public void TE(InventoryTransactionEvent event) {
////        System.out.println("CALLLL");
//        InventoryTransaction transaction = event.getTransaction();
//        Set<InventoryAction> traa = transaction.getActions();
//        for (InventoryAction t : traa) {
////            System.out.println("CALLLL TTTTTTTTTTTTTTTTTTT" + t.getClass().getName());
//            if (t instanceof SlotChangeAction) {
////                System.out.println("CALLLL SLOTCCCCCCCC");
//                SlotChangeAction sca = (SlotChangeAction) t;
//
////                sca.getInventory()
//
//                Inventory inv = sca.getInventory();
//                System.out.println("CHECK INNNNNVVVVVV2222222222222V " + inv.getClass().getName());
////                if (inv.isEmpty()) return;
//
////                System.out.println("NEEEEEEE" + inv.getClass().getTypeName());
//                if (inv instanceof PlayerInventory) {
//
//                }else if(inv instanceof PlayerCursorInventory){
//                    event.setCancelled();
//                    transaction.getSource().sendAllInventories();
//                    System.out.println("+++++>"+transaction.getSource().getCursorInventory());
//                    System.out.println("+++++>"+transaction.getSource().getCursorInventory().slots);
//                }
//                if (inv instanceof AuctionHouse) {
//
//                    AuctionHouse ah = (AuctionHouse) inv;
////                    if(!ah.Init)return;
//                    System.out.println(sca.getSlot() + " || " + ah.getHolder().getName() + " || " + ah.getHolder().getClass().getName());
//                    CorePlayer ccpp = (CorePlayer) ah.getHolder();
//                    int slot = sca.getSlot();
////                    event.setCancelled();
//                    event.setCancelled();
//                    if (slot < 5 * 9) {
//                        System.out.println("TOP INV");
//                        //TODO CONFIRM AND SHOW ITEM
//                        if (!ah.ConfirmPurchase) {
//                            ah.ConfirmItemPurchase(slot);
//                            System.out.println("SSSSSSSSSSSSCPPPPPPPP");
////                        ccpp.AH.ConfirmItemPurchase(slot);
//                        } else {
//                            Item si = ah.getContents().get(slot);
//                            if (si != null) {
//                                if (ah.getCurrentPage() == Confirm_Purchase_Not_Enough_Money) {
//                                    ah.setPage(1);
//                                    ah.ClearConfirmPurchase();
//                                    //Back Home
//                                    break;
//                                } else {
//                                    System.out.println("CPPPPPPPP");
//
//                                    if (si.getId() == BlockID.EMERALD_BLOCK) {
//                                        System.out.println("CONFIRM PURCHASE!!!!!!!");
//                                        ah.SF.PurchaseItem((CorePlayer) ah.getHolder(), ah.getPage(), ah.ConfirmPurchaseSlot);
//                                        break;
//                                    } else if (si.getId() == BlockID.REDSTONE_BLOCK) {
//                                        System.out.println("DENCLINE PURCHASE!!!!!!!!");
//                                        ah.setPage(1);
//                                        ah.ClearConfirmPurchase();
//                                        break;
//                                    } else {
//                                        ah.setPage(1);
//                                        System.out.println("UNKNOWNMNNN!!!!!!!!");
//                                        ah.ClearConfirmPurchase();
//                                        break;
//                                    }
//                                }
//                            }
//                        }
//                    } else {
//                        switch (slot) {
//                            case AuctionHouse.MainPageItemRef.LastPage:
//                                ah.GoToPrevPage();
//                                break;
//                            case AuctionHouse.MainPageItemRef.NextPage:
//                                ah.GoToNextPage();
//                                break;
//                            case AuctionHouse.MainPageItemRef.Search:
//                                break;
//                            case AuctionHouse.MainPageItemRef.Reload:
//                                ah.ReloadCurrentPage();
//                                break;
//                            case AuctionHouse.MainPageItemRef.Catagories:
//                                ah.DisplayCatagories();
//                                break;
//                            case AuctionHouse.MainPageItemRef.PlayerSelling:
//                                ah.GoToSellerPage();
//                                event.setCancelled(false);
//                                break;
//
//                        }
//                    }
//                }
//            }
//        }
//    }

        public void PurchaseItem(CorePlayer holder, int page, int slot)
        {
            AuctionItemData aid = getAIDFromPage(page, slot);
            if (aid == null)
            {
                Console.WriteLine("ERROR IN SELECTION!!!!");
            }
            else if (aid.getCost() > holder.getMoney())
            {
                holder.AH.SetupPageNotEnoughMoney(aid);
                return;
            }

//        SetBought(aid.getMasterid());
            holder.TakeMoney(aid.getCost());
            holder.Inventory.AddItem(aid.getKeepItem(),true);
            holder.AH.ClearConfirmPurchase();
            holder.AH.setPage(1);
        }

        public void SetBought(int id)
        {
            String sql = $"UPDATE `AuctionHouse` SET `purchased` = '1' WHERE `id` = {id};";
            try
            {
                CCM.SQL.Insert(sql);
            }
            catch (Exception e)
            {
                CyberCoreMain.Log.Error("ERRORRR 555 ", e);
            }
        }

        public void ClaimMoney(int id)
        {
            String sql = $"UPDATE `AuctionHouse` SET `moneysent` = '1' WHERE `master_id` = {id}";
            CCM.SQL.Insert(sql);
            //ExecuteUpdateSQLite(sql);
        }

        public void additem(Item i, CorePlayer p, int cost)
        {
            AuctionItemData aid = new AuctionItemData(i, cost, p);
            AddItemForSale(aid);
        }
        
        
        public AuctionItemData AddItemForSale(AuctionItemData data) {
           
                if (data.getMasterid() != -1)
                    CCM.SQL.Insert($"DELETE FROM `AuctionHouse` WHERE `master_id` == ' {data.getMasterid()}'");
                String fnt = "";
                var a = new NbtFile();
                a.RootTag = data.getItem().ExtraData;
                // byte[] bytes = NBTCompressionSteamTool.NBTCompressedStreamTools.a(a);
        var aa = (new MemoryStream());
        a.SaveToStream(aa, NbtCompression.AutoDetect);
        var aaa = new StreamReader(aa).ReadToEnd();

        if (data.getItem().ExtraData.HasValue) fnt = aaa;
        CCM.SQL.Insert(
                    $"INSERT INTO `AuctionHouse` VALUES (null,{data.getItem().Id},{data.getItem().Metadata},{data.getItem().Count},@data,{data.Cost}, {data.Soldby},{data.Soldbyn} ,false)","@data",Encoding.ASCII.GetBytes(aaa));

                CyberCoreMain.Log.Info("AH saved for " + data.toString());
                // ExecuteQuerySQLite("SELECT * FROM `AuctionHouse` ");

            
            return data;
        }
        
    }

    public class AFSettings
    {
        public AFSettings()
        {
        }
    }
}