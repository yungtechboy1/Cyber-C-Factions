using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using CyberCore.Manager.AuctionHouse;
using CyberCore.Manager.Factions.Windows;
using CyberCore.Utils;
using CyberCore.Utils.Cooldowns;
using fNbt;
using log4net.Core;
using MiNET;
using MiNET.Blocks;
using MiNET.Items;
using MiNET.Net;
using MiNET.Plugins;
using MiNET.Utils;

namespace CyberCore.Manager.Shop
{
    public class ShopFactory
    {
        // private SqlManager SQL;
        CyberCoreMain CCM;
        private List<ShopMysqlData> ShopCache = null;
        private CoolDown ShopCacheReset = null;

        public ShopFactory(CyberCoreMain CCM)
        {
            this.CCM = CCM;
//            InternalPlayerSettings = new Config(new File(CCM.getDataFolder(), "Auctions.yml"), Config.YAML);
            // SQL = new SqlManager(CCM, "shop");
            // LoadAllItems();
        }

//         public void LoadAllItems()
//         {
//             try
//             {
//                 List<Dictionary<String, Object>> data = SQL.executeSelect("SELECT * FROM `AuctionHouse`");
//                 if (data == null)
//                 {
//                     CyberCoreMain.Log.Error("Error Loading Auctions from Sqlite!");
//                     return;
//                 }
//                 else
//                 {
//                     CyberCoreMain.Log.Info("Loading " + data.Count + " Auction Items!");
//                 }
//
//                 foreach (Dictionary<String, Object> v in data)
//                 {
// //                Console.WriteLine(v+" << "+v.getClass().getName());
//                     AuctionItemData a = new AuctionItemData(v);
//                 }
//             }
//             catch (Exception e)
//             {
//                 Console.WriteLine("EE21122332112` +>" + e);
//             }
//         }

        public void AddItemForSale(AuctionItemData aid) {
            // Save(aid);
        }
        // public AuctionItemData Save(AuctionItemData data)
        // {
        //     try
        //     {
        //         if (data.masterid != -1)
        //             SQL.Insert($"DELETE FROM `AuctionHouse` WHERE `master_id` == '{data.masterid}'");
        //         String fnt = "";
        //         if (data.item.hasCompoundTag()) fnt = data.item.ExtraData.NBTToString();
        //         SQL.Insert(
        //             "INSERT INTO `AuctionHouse` VALUES (null," + data.item.Id + "," + data.item.Metadata + "," +
        //             data.item.Count + ",'" + fnt + "'," + data.Cost + ",'" + data.Soldby + "','" + data.Soldbyn +
        //             "',false)");
        //
        //         CyberCoreMain.Log.Info("AH saved for " + data.toString());
        //         // ExecuteQuerySQLite("SELECT * FROM `AuctionHouse` ");
        //     }
        //     catch (Exception e)
        //     {
        //         Console.WriteLine(e);
        //     }
        //
        //     return data;
        // }
        //
        
    //      public List<ShopMysqlData> GetAllItems() {
    //     return GetAllItems(false);
    // }
//     public List<ShopMysqlData> GetAllItems(bool admin) {
//         if (ShopCache != null) {
//             if (ShopCacheReset != null) {
//                 if (ShopCacheReset.isValid()) {
//                     Console.WriteLine("Using Cache!!!");
//                     return ShopCache;
//                 } else {
//                     ShopCache = null;
//                     ShopCacheReset = null;
//                 }
//             }
//         }
//         List<ShopMysqlData> l = new List<ShopMysqlData>();
//         try {
//             var rs = SQL.executeSelect("SELECT * FROM `Shop` WHERE "+ (!admin ? "`enabled` = 1 ":"")+"ORDER BY `Itemid` ASC");
//             if (rs != null) {
//                 try {
//                     foreach (Dictionary<string, object> d in rs)
//                     {
//                         ShopMysqlData aid = new ShopMysqlData(d);
// //                        Console.WriteLine(">>>!!!+" + aid);
//                         l.Add(aid);
//                     }
//                 } catch (Exception ex) {
//                     CyberCoreMain.Log.Error("Error Loading Shop Items3!\n\n\n\n\n"+ex);
//                     return null;
//                 }
//                  CyberCoreMain.Log.Info("Loaded " + l.Count + " Items for AH1");
//                 List<ShopMysqlData> t = new List<ShopMysqlData>(l);
//                 if (t != null) {
//                     ShopCache = t;
//                     ShopCacheReset = new CoolDown("Shop", 0, 15);
//                     //Set Cache
//                 }
//                 return l;
//             }
//         } catch (Exception e) {
//              CyberCoreMain.Log.Error("SSSSHHHHH ERRRORRROOROORORR", e);
//         }
//         return l;
//     }


    //@Todo
//    public List<Item> getSoldItems() {
//        List<Item> is = new List<>();
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
//                    is.add(i);
//                }
//            } catch (Exception ex) {
//                ex.printStackTrace();
//                 CyberCoreMain.Log.info("ERror loading Items!");
//                return null;
//            }
//             CyberCoreMain.Log.info("Loaded " + is.size() + " Items for AH");
//            items = is;
//        }
//        return null;
//    }

    // public List<Item> getListOfItems(bool admin) {
    //     List<Item> il = new List<Item>();
    //     foreach (ShopMysqlData ahd in GetAllItems(admin)) {
    //         il.Add(ahd.getItem());
    //     }
    //     return il;
    // }


//    public Item getItem(int page, int slot) {
//        int stop = page * 45;
//        int start = stop - 45;
//        int key = start + slot;
//        return items.get(key);
//    }

    //
    // public Dictionary<int, Item> getPageHash(int page, ShopCategory catg, bool AdminMode) {
    //     Dictionary<int, Item> list = new Dictionary<int, Item>();
    //     int k = 0;
    //     foreach (Item i in getPageItems(page, catg, AdminMode))
    //     {
    //         list[k] = i;
    //         k++;
    //     }
    //     return list;
    //
    // }
    //
    //
    // public List<ShopMysqlData> GetAllItemsDataLimit(int start, int stop, bool admin) {
    //     List<ShopMysqlData> il = new List<ShopMysqlData>();
    //     List<ShopMysqlData> a = GetAllItems(admin);
    //     for (int i = start; i < stop; i++) {
    //         if (i >= a.Count) break;
    //         ShopMysqlData smd = a[i];
    //         if (smd != null) il.Add(smd);
    //     }
    //     return il;
    // }
    //
    // public List<Item> GetAllItemsLimit(int start, int stop, ShopCategory catg, bool adminMode) {
    //     List<Item> il = new List<Item>();
    //     List<ShopMysqlData> a = GetAllItems(adminMode);
    //     a = filterShopDataByCategory(a,catg);
    //     for (int i = start; i < stop; i++) {
    //         if (i >= a.Count) break;
    //         ShopMysqlData smd = a[i];
    //         if (smd != null) {
    //             il.Add(smd.getItem());
    //         }
    //     }
    //     return il;
    // }
    //
    // private List<ShopMysqlData> filterShopDataByCategory(List<ShopMysqlData> a, ShopCategory catg) {
    //     List<ShopMysqlData> sd = new List<ShopMysqlData>();
    //     foreach(ShopMysqlData d in a){
    //         if(d.getCategory().Count == 0 && catg != ShopCategory.NA)continue;
    //         if(catg != ShopCategory.NA &&!d.getCategory().Contains(catg))continue;
    //         sd.Add(d);
    //     }
    //     return sd;
    // }
//
//    public List<ShopMysqlData> GetAllItemsLimitData(int start, int stop) {
//        List<ShopMysqlData> il = new List<>();
//        for (ShopMysqlData ahd : GetAllItemsDataLimit(start, stop)) {
//            il.add(ahd);
//        }
//        return il;
//    }
    //
    // public Item getItemFromPage(int page, int slot, ShopCategory catg, bool AdminMode) {
    //     if (slot > 45) {
    //          CyberCoreMain.Log.Error("ERROR! Slot out of range! E443 Slot:" + slot);
    //         return null;
    //     }
    //     List<Item> list = getPageItems(page, catg, AdminMode);
    //     if (slot > list.Count) {
    //          CyberCoreMain.Log.Error("ERROR! Selected Slot out of List Range! E33342 SLOT:" + slot + " OF " + list.Count);
    //         return null;
    //     }
    //     Item s = list[slot];
    //     if (s.Id == 0) return null;
    //     return s;
    // }


//     public List<Item> getPageItems(int page, ShopCategory catg, bool adminMode) {
//         int stop = page * 45;
//         int start = stop - 45;
//         List<Item> list2 = GetAllItemsLimit(start, stop,catg,adminMode);
//         if (45 > list2.Count) {
//             List<Item> a = new List<Item>();
//             for (int i = 0; i < 45; i++) {
// //                list2.iterator().n
//                 if (list2.Count > i && list2[i] != null) {
// //                    Console.WriteLine("ADDING ACTUAL ITEM || " + list2.get(i).getId());
//                     a.Add(list2[i]);
//                 } else {
//                     a.Add(new ItemBlock(new Air()));
// //                    Console.WriteLine("ADDING AIR ||");
//                 }
//             }
//
//             return a;
//         } else {
//             return list2;
//         }
//     }

//     public List<ShopMysqlData> getPageData(int page, bool admin) {
//         int stop = page * 45;
//         int start = stop - 45;
//         List<ShopMysqlData> list2 = GetAllItemsDataLimit(start, stop,admin);
//         if (45 > list2.Count) {
//             List<ShopMysqlData> a = new List<ShopMysqlData>();
//             for (int i = 0; i < 45; i++) {
// //                list2.iterator().n
//                 if (list2.Count > i && list2[i] != null) {
//                     a.Add(list2[i]);
//                 } else {
//                     a.Add(null);
// //                    Console.WriteLine("ADDING AIR ||");
//                 }
//             }
//
//             return a;
//         } else {
//             return list2;
//         }
//     }


    public void OpenShop(CorePlayer p, int pg) {
        
        var n = new NbtCompound("");
        n.Add(new NbtString("CustomName", "SHOTTP!"));
        SpawnFakeBlockAndEntity(p, n);
        
        PlayerLocation a = new PlayerLocation();
        PlayerLocation aa = new PlayerLocation();
        a.X = aa.X = p.KnownPosition.X;
        a.Y = aa.Y = p.KnownPosition.Y - 2;
        a.Z = aa.Z = p.KnownPosition.Z;
        aa.Z += 1;
        
        
        // ShopInv b = new ShopInv(p, CCM, p.KnownPosition.ToVector3(), pg);
        var b = new NewShopInv();
        // CyberCoreMain.getInstance().getLogger().info(b.getContents().values().size() + " < SIZZEEE" + b.getSize());
        // CyberCoreMain.getInstance().getServer().getScheduler().scheduleDelayedTask(new OpenShop(p, b), 5);
//        b.open()
        // AuctionHouse b = new AuctionHouse(p, CCM, p.KnownPosition.ToVector3(), pg);
        // CyberCoreMain.Log.Info(b.getContents().Values.Count + " < SIZZEEE" + b.size);
        //TODO !IMPORTANT
        p.SetOpenInventory(b);
        // p.Shop = b;
        
        var containerOpen = McpeContainerOpen.CreateObject();
        containerOpen.windowId = 10;
        containerOpen.type = 0;
        containerOpen.coordinates = (BlockCoordinates) a;
        containerOpen.runtimeEntityId = -1;
        p.SendPacket(containerOpen);

        Console.WriteLine("SLOT===============================================");
        foreach (var s in b.Slots)
        {
            if (s == null || s.Id == 0) continue;
            Console.WriteLine(s.Id + "||| "+s.Metadata);
        }
        Console.WriteLine("SLOT===============================================");
        McpeInventoryContent containerSetContent = McpeInventoryContent.CreateObject();
        containerSetContent.inventoryId = 10;
        containerSetContent.input = b.Slots;
        p.SendPacket(containerSetContent);
    }

    public void SpawnFakeBlockAndEntity(Player to, NbtCompound data) {

        BlockCoordinates m = SpawnBlock(to);
        SpawnBlockEntity(m, data,to);

    }


        public BlockCoordinates SpawnBlock(Player to)
        {
            Chest b = new Chest();
            //0 Right
            //1 Right
            //2 Left
            //3 Right?
            //4 Down
            //5 Up
            b.FacingDirection = 5;
            PlayerLocation a = new PlayerLocation();
            PlayerLocation aa = new PlayerLocation();
            a.X = aa.X = to.KnownPosition.X;
            a.Y = aa.Y = to.KnownPosition.Y - 2;
            a.Z = aa.Z = to.KnownPosition.Z;
            aa.Z += 1;
            BlockCoordinates l1 = new BlockCoordinates(a);
            BlockCoordinates l2 = new BlockCoordinates(aa);
            var message = McpeUpdateBlock.CreateObject();
            message.blockRuntimeId = (uint) b.GetRuntimeId();
            message.coordinates = l1;
            message.blockPriority = (int) (McpeUpdateBlock.Flags.AllPriority);
            var message2 = McpeUpdateBlock.CreateObject();
            message2.blockRuntimeId = (uint) b.GetRuntimeId();
            message2.coordinates = l2;
            message2.blockPriority = (int) (McpeUpdateBlock.Flags.AllPriority);
            to.SendPacket(message);
            to.SendPacket(message2);
            return l1;
        }

        public void SpawnBlockEntity(BlockCoordinates bc, NbtCompound data, Player to)
        {
            BlockCoordinates a = new BlockCoordinates(bc);
            BlockCoordinates aa = new BlockCoordinates(bc);
            // a.X = aa.X = (int) to.KnownPosition.X;
            // a.Y = aa.Y = (int) (to.KnownPosition.Y - 2);
            // a.Z = aa.Z = (int) to.KnownPosition.Z;
            aa.Z += 1;
            data.Add(new NbtInt("pairx",(int) aa.X));
            data.Add(new NbtInt("pairz",(int) aa.Z));
            
            var nbt = new Nbt
            {
                NbtFile = new NbtFile
                {
                    BigEndian = false,
                    UseVarInt = true,
                    RootTag = data
                }
            };
            Console.WriteLine("FIRST NBT1111111111111111");
            var nbt2 = new Nbt
            {
                NbtFile = new NbtFile
                {
                    BigEndian = false,
                    UseVarInt = true,
                    RootTag = new NbtCompound("")
                    {
                        new NbtInt("pairx", (int) a.X),
                        new NbtInt("pairz", (int) a.Z),
                    }
                }
            };
            BlockCoordinates l1 = new BlockCoordinates(a);
            BlockCoordinates l2 = new BlockCoordinates(aa);
            McpeBlockEntityData bedp = McpeBlockEntityData.CreateObject();
            McpeBlockEntityData bedp2 = McpeBlockEntityData.CreateObject();
            bedp.coordinates = (BlockCoordinates) a;
            bedp.namedtag = nbt;
            bedp2.coordinates = (BlockCoordinates) aa;
            bedp2.namedtag = nbt2;
            //
            // Console.WriteLine($"BEPD : {a}");
            // Console.WriteLine($"BEPD2 : {aa}");
            
            to.SendPacket(bedp);
            to.SendPacket(bedp2);
        }

//    @EventHandler(ignoreCancelled = true)
//    public void TTE(InventoryClickEvent event) {
//
//        Console.WriteLine("++++++++++++++++++++++++++++++++++++++++");
//        Console.WriteLine("++++++++++++++++++++++++++++++++++++++++");
//        Console.WriteLine(event.getSlot());
//        Console.WriteLine(event.getInventory().getClass().getName());
//
//        Console.WriteLine("CALLLLCLLIICCCKKK");
//                Console.WriteLine("CALLLL SLOTCCCCCCCC");
//                int slott = event.getSlot();
//
////                sca.getInventory()
//
//                Inventory inv = event.getInventory();
//                Console.WriteLine("CHECK INNNNNVVVVVVV " + inv.getClass().getName());
////                if (inv.isEmpty()) return;
//
//                Console.WriteLine("NEEEEEEE" + inv.getClass().getTypeName());
//                if (inv instanceof PlayerInventory) {
//
//                }
//                if (inv instanceof AuctionHouse) {
//
//                    AuctionHouse ah = (AuctionHouse) inv;
////                    if(!ah.Init)return;
//                    Console.WriteLine(slott + " || " + ah.getHolder().getName() + " || " + ah.getHolder().getClass().getName());
//                    CorePlayer ccpp = (CorePlayer) ah.getHolder();
//                    int slot = slott;
////                    event.setCancelled();
//                    if (slot < 5 * 9) {
//                        Console.WriteLine("TOP INV");
//                        //TODO CONFIRM AND SHOW ITEM
//                        if (!ah.ConfirmPurchase) {
//                            ah.ConfirmItemPurchase(slot);
//                            event.setCancelled();
//                            Console.WriteLine("SSSSSSSSSSSSCPPPPPPPP");
////                        ccpp.AH.ConfirmItemPurchase(slot);
//                        } else {
//                            Console.WriteLine("CPPPPPPPP");
//                            Item si = ah.getContents().get(slot);
//                            if (si != null) {
//                                if (si.getId() == BlockID.EMERALD_BLOCK) {
//                                    Console.WriteLine("CONFIRM PURCHASE!!!!!!!");
////                                    ah.SF.PurchaseItem((CorePlayer) ah.getHolder(), Page, slot);
//                                } else if (si.getId() == BlockID.REDSTONE_BLOCK) {
//                                    Console.WriteLine("DENCLINE PURCHASE!!!!!!!!");
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

//    @EventHandler(ignoreCancelled = true)
//    public void T(InventoryClickEvent e){
//
//                Console.WriteLine("CHECK INNNNNVVVVVVV1111");
//
//                Inventory inv = e.getInventory();
//                Console.WriteLine("CHECK INNNNNVVVVVVV " + inv.getClass().getName());
////                if (inv.isEmpty()) return;
//
////                Console.WriteLine("NEEEEEEE" + inv.getClass().getTypeName());
//                if (inv instanceof PlayerInventory) {
//
//                }else if (inv instanceof PlayerCursorInventory) {
//                    PlayerCursorInventory pci = (PlayerCursorInventory)inv;
//                    e.setCancelled();
//
//                }else if (inv instanceof ShopInv) {
//
//                    ShopInv ah = (ShopInv) inv;
////                    if(!ah.Init)return;
//                    Console.WriteLine(e.getSlot() + " || " + ah.getHolder().getName() + " || " + ah.getHolder().getClass().getName());
//                    CorePlayer ccpp = (CorePlayer) ah.getHolder();
//                    int slot = e.getSlot();
//                    int sx = slot % 9;
//                    int sy = (int) Math.floor(slot / 9);
////                    event.setCancelled();
//                    e.setCancelled();
//                    if (slot < 5 * 9) {
//                        Console.WriteLine("TOP INV");
//                        //TODO CONFIRM AND SHOW ITEM
//                        if (!ah.ConfirmPurchase) {
//                            Item is = ah.getItem(slot);
//                            if (ah.CurrentPage == ShopInv.CurrentPageEnum.Catagories) {
//                                if (slot == 11) {
//                                    ah.CCM.SpawnShop.OpenShop((CorePlayer) ah.getHolder(), 1);
//                                } else {
//                                    ah.setPage(1);
//                                }
//                            } else {
//                                ah.ConfirmItemPurchase(slot);
//                            }
////                        ccpp.AH.ConfirmItemPurchase(slot);
//                        } else {
//                            Item is = ah.getItem(slot);
//                            if (ah.CurrentPage == ShopInv.CurrentPageEnum.Catagories) {
//                                if (slot == 11) {
//                                    ah.CCM.SpawnShop.OpenShop((CorePlayer) ah.getHolder(), 1);
//                                } else {
//                                    ah.setPage(1);
//                                }
//                            } else if (ah.CurrentPage == ShopInv.CurrentPageEnum.PlayerSellingPage) {
//                                bool isi = false;
//                                int isc = is.getCount();
//                                if (is != null && is.getId() != 0) {
//                                    if (is.getId() == Item.IRON_BLOCK) isi = true;
//                                    Console.WriteLine("Selected Slot SX:" + sx + " | SY:" + sy);
//                                    if (sy != 0 && sy != 5 && sx != 4 && !isi) {
//                                        if (sx < 4) {
//                                            //Sell
//                                            ah.SetupPageToFinalConfirmItem(ah.MultiConfirmData, isc, true);
//
//                                        } else {
//                                            //Buy
//                                            ah.SetupPageToFinalConfirmItem(ah.MultiConfirmData, isc, false);
//                                        }
//                                    }
//                                }
//                                e.setCancelled();
//                                return;
//                            } else {
//                                Item si = ah.getContents().get(slot);
//                                if (si != null) {
//                                    if (ah.getCurrentPage() == ShopInv.CurrentPageEnum.Confirm_Purchase_Not_Enough_Money) {
//                                        ah.setPage(1);
//                                        ah.ClearConfirmPurchase();
//                                        //Back Home
//                                    } else {
//                                        if (si.getId() == BlockID.EMERALD_BLOCK) {
//                                            Console.WriteLine("CONFIRM PURCHASE!!!!!!!");
//                                            ah.SF.PurchaseItem((CorePlayer) ah.getHolder(), ah.getPage(), ah.ConfirmPurchaseSlot, si.getCount());
//                                        } else if (si.getId() == BlockID.REDSTONE_BLOCK) {
//                                            Console.WriteLine("DENCLINE PURCHASE!!!!!!!!");
//                                            ah.setPage(1);
//                                            ah.ClearConfirmPurchase();
//                                        } else {
//                                            ah.setPage(1);
//                                            Console.WriteLine("UNKNOWNMNNN!!!!!!!!");
//                                            ah.ClearConfirmPurchase();
//                                        }
//                                    }
//                                }
//                            }
//                        }
//                    } else {
//                        switch (slot) {
//                            case ShopInv.MainPageItemRef.LastPage:
//                                ah.GoToPrevPage();
//                                break;
//                            case ShopInv.MainPageItemRef.NextPage:
//                                ah.GoToNextPage();
//                                break;
//                            case ShopInv.MainPageItemRef.Search:
//                                break;
//                            case ShopInv.MainPageItemRef.Reload:
//                                ah.ReloadCurrentPage();
//                                break;
//                            case ShopInv.MainPageItemRef.Catagories:
//                                ah.DisplayCatagories();
//                                break;
//                            case ShopInv.MainPageItemRef.ToggleAdmin:
//                                ah.AdminMode = !ah.AdminMode;
//                                e.setCancelled(false);
//                                ah.ReloadCurrentPage();
//                                break;
//
//                        }
//                    }
//                }
//            }

    //    TODO MAke Pages with new API
//    @EventHandler(ignoreCancelled = true)
//    public void TE(InventoryTransactionEvent event) {
//
////        event.setCancelled();
//        Console.WriteLine("CALLLL");
////        event.getTransaction().
//        InventoryTransaction transaction = event.getTransaction();
//        Set<InventoryAction> traa = transaction.getActions();
//        bool s = true;
//        for (Inventory i :transaction.getInventories()) {
//            if(i instanceof ShopInv)s  = false;
//        }
//        if(s)return;
//        for (InventoryAction t : traa) {
//            Console.WriteLine("CALLLL TTTTTTTTTTTTTTTTTTT" + t.getClass().getName());
//            if (t instanceof SlotChangeAction) {
////                Console.WriteLine("CALLLL SLOTCCCCCCCC");
//                SlotChangeAction sca = (SlotChangeAction) t;
////                sca.getInventory()
//
//                Inventory inv = sca.getInventory();
////                Console.WriteLine("CHECK INNNNNVVVVVVV " + inv.getClass().getName());
////                if (inv.isEmpty()) return;
//
////                Console.WriteLine("NEEEEEEE" + inv.getClass().getTypeName());
//                if (inv instanceof PlayerInventory) {
//                    Console.WriteLine("CHECK INNNNNVVVVVVV " + sca);
////                event.setCancelled();
//
//                } else if (inv instanceof PlayerCursorInventory) {
//                    event.setCancelled();
//                    transaction.getSource().getCursorInventory().clearAll();
//                    transaction.getSource().sendAllInventories();
//                    Console.WriteLine("+++++>" + transaction.getSource().getCursorInventory());
//                    Console.WriteLine("+++++>" + transaction.getSource().getCursorInventory().slots);
//                }
//                if (inv instanceof ShopInv) {
//
//                    Console.WriteLine("CHECK INNNNNVVV222222VVVV " + sca);
//                    ShopInv m = (ShopInv) inv;
////                    if(!ah.Init)return;
//                    Console.WriteLine(sca.getSlot() + " || " + ah.getHolder().getName() + " || " + ah.getHolder().getClass().getName());
//                    CorePlayer ccpp = (CorePlayer) ah.getHolder();
//                    int slot = sca.getSlot();
//                    int sx = slot % 9;
//                    int sy = (int) Math.floor(slot / 9);
////                    event.setCancelled();
//                    if (slot < 5 * 9) {
//                        Console.WriteLine("TOP INV");
//                        //TODO CONFIRM AND SHOW ITEM
//                        if (!ah.ConfirmPurchase) {
//                            Item is = ah.getItem(slot);
//                            if (ah.CurrentPage == ShopInv.CurrentPageEnum.Catagories) {
//                                if (slot == 11) {
//                                    ah.CCM.SpawnShop.OpenShop((CorePlayer) ah.getHolder(), 1);
//                                } else {
//                                    ah.setPage(1);
//                                }
//                            } else {
//                                ah.ConfirmItemPurchase(slot);
//                            }
////                        ccpp.AH.ConfirmItemPurchase(slot);
//                        } else {
//                            Item is = ah.getItem(slot);
//                            if (ah.CurrentPage == ShopInv.CurrentPageEnum.Catagories) {
//                                if (slot == 11) {
//                                    ah.CCM.SpawnShop.OpenShop((CorePlayer) ah.getHolder(), 1);
//                                } else {
//                                    ah.setPage(1);
//                                }
//                            } else if (ah.CurrentPage == ShopInv.CurrentPageEnum.PlayerSellingPage) {
//                                bool isi = false;
//                                int isc = is.getCount();
//                                if (is != null && is.getId() != 0) {
//                                    if (is.getId() == Item.IRON_BLOCK) isi = true;
//                                    Console.WriteLine("Selected Slot SX:" + sx + " | SY:" + sy);
//                                    if (sy != 0 && sy != 5 && sx != 4 && !isi) {
//                                        if (sx < 4) {
//                                            //Sell
//                                            ah.SetupPageToFinalConfirmItem(ah.MultiConfirmData, isc, true);
//
//                                        } else {
//                                            //Buy
//                                            ah.SetupPageToFinalConfirmItem(ah.MultiConfirmData, isc, false);
//                                        }
//                                    }
//                                }
////                                event.setCancelled();
//                                return;
//                            } else {
//                                Item si = ah.getContents().get(slot);
//                                if (si != null) {
//                                    if (ah.getCurrentPage() == ShopInv.CurrentPageEnum.Confirm_Purchase_Not_Enough_Money) {
//                                        ah.setPage(1);
//                                        ah.ClearConfirmPurchase();
//                                        //Back Home
//                                        break;
//                                    } else {
//                                        Console.WriteLine("CPPPPPPPP");
//
//                                        if (si.getId() == BlockID.EMERALD_BLOCK) {
//                                            Console.WriteLine("CONFIRM PURCHASE!!!!!!!");
//                                            ah.SF.PurchaseItem((CorePlayer) ah.getHolder(), ah.getPage(), ah.ConfirmPurchaseSlot, si.getCount());
//                                            break;
//                                        } else if (si.getId() == BlockID.REDSTONE_BLOCK) {
//                                            Console.WriteLine("DENCLINE PURCHASE!!!!!!!!");
//                                            ah.setPage(1);
//                                            ah.ClearConfirmPurchase();
//                                            break;
//                                        } else {
//                                            ah.setPage(1);
//                                            Console.WriteLine("UNKNOWNMNNN!!!!!!!!");
//                                            ah.ClearConfirmPurchase();
//                                            break;
//                                        }
//                                    }
//                                }
//                            }
//                        }
//                        event.setCancelled();
//                    } else {
//                        switch (slot) {
//                            case ShopInv.MainPageItemRef.LastPage:
//                                ah.GoToPrevPage();
//                                break;
//                            case ShopInv.MainPageItemRef.NextPage:
//                                ah.GoToNextPage();
//                                break;
//                            case ShopInv.MainPageItemRef.Search:
//                                break;
//                            case ShopInv.MainPageItemRef.Reload:
//                                ah.ReloadCurrentPage();
//                                break;
//                            case ShopInv.MainPageItemRef.Catagories:
//                                ah.DisplayCatagories();
//                                break;
//                            case ShopInv.MainPageItemRef.ToggleAdmin:
//                                ah.AdminMode = !ah.AdminMode;
//                                event.setCancelled(false);
//                                ah.ReloadCurrentPage();
//                                break;
//
//                        }
//                    }
//                }
//            }
//        }
//        event.getTransaction().getSource().sendAllInventories();
//    }

    //SetupPageToFinalConfirmItemSell
//     public void PurchaseItem(CorePlayer holder, int page, int slot, int count, bool admin) {
//         ShopMysqlData aid = getItemFrom(page, slot, admin);
//         if (aid == null) {
//             Console.WriteLine("ERROR IN SELECTION!!!!");
//         } else if (aid.getPrice(count) > holder.getMoney() && !holder.Shop.SetupPageToFinalConfirmItemSell) {
//             holder.Shop.SetupPageNotEnoughMoney(aid);
//             return;
//         }
// //        int c = holder.Shop.SetupPageToFinalConfirmItemCount;
//         if (holder.Shop.SetupPageToFinalConfirmItemSell) {
//             holder.AddMoney(aid.getSellPrice(count));
//             Item i = aid.getItem(true);
//             i.setCount(count);
//             holder.getInventory().removeItem(i);
//             holder.Shop.ClearConfirmPurchase();
//             holder.Shop.setPage(1);
//         } else {
//
// //        SetBought(aid.getMasterid());
//             holder.TakeMoney(aid.getPrice(count));
//             Item i = aid.getItem(true);
//             i.setCount(count);
//             holder.getInventory().addItem(i);
//             holder.Shop.ClearConfirmPurchase();
//             holder.Shop.setPage(1);
//         }
//     }
//
//     public void PurchaseItem(CorePlayer holder, ShopMysqlData aid, int count, bool buy) {
//        if (aid.getPrice(count) > holder.getMoney() && !holder.Shop.SetupPageToFinalConfirmItemSell) {
//             holder.Shop.SetupPageNotEnoughMoney(aid);
//             return;
//         }
// //        int c = holder.Shop.SetupPageToFinalConfirmItemCount;
//         if (!buy) {
//             holder.AddMoney(aid.getSellPrice(count));
//             Item i = aid.getItem(true);
//             i.setCount(count);
//             holder.getInventory().removeItem(i);
//             holder.Shop.ClearConfirmPurchase();
//             holder.Shop.setPage(1);
//         } else {
//
// //        SetBought(aid.getMasterid());
//             holder.TakeMoney(aid.getPrice(count));
//             Item i = aid.getItem(true);
//             i.setCount(count);
//             holder.getInventory().addItem(i);
//             holder.Shop.ClearConfirmPurchase();
//             holder.Shop.setPage(1);
//         }
//     }

//     public ShopMysqlData getItemFrom(int page, int slot, bool admin) {
//         List<ShopMysqlData> smd = getPageData(page, admin);
//         if (smd.size() < slot) return null;
//         return smd.get(slot);
//     }
//
//     public void additem(Item i, CorePlayer p, int cost) {
//         AuctionItemData aid = new AuctionItemData(i, cost, p);
//         SQL.AddItemForSale(aid);
//     }
//
//     public ShopMysqlData getItemFrom(int s) {
//         List<ShopMysqlData> is = new List<>();
//         try {
//             ResultSet rs = SQL.ExecuteQuerySQLite("SELECT * FROM `Shop` WHERE `ShopID` = "+ s+"ORDER BY `Itemid` ASC");
//             if (rs != null) {
//                 try {
//                     while (rs.next()) {
//                         ShopMysqlData aid = new ShopMysqlData(rs);
// //                        Console.WriteLine(">>>!!!+" + aid);
//                         return aid;
//                     }
//                 } catch (Exception ex) {
//                     ex.printStackTrace();
//                      CyberCoreMain.Log.info("Error loading Shop Items3!");
//                     return null;
//                 };
//             }
//         } catch (Exception e) {
//              CyberCoreMain.Log.error("SSSSHHHHH ERRRORRROOROORORR", e);
//         }
//         return null;
//     }
//         
        
        
        
        
    }
}