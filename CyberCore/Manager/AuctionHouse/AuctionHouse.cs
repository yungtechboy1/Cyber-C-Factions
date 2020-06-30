using System;
using System.Collections.Generic;
using System.Numerics;
using CyberCore.Utils;
using fNbt;
using MiNET;
using MiNET.BlockEntities;
using MiNET.Blocks;
using MiNET.Entities;
using MiNET.Items;
using MiNET.Utils;
using OpenAPI.Player;
using SharpAvi.Codecs;

namespace CyberCore.Manager.AuctionHouse
{
    public class AuctionHouse
    {
        protected readonly String name;

        protected readonly String title;

        //    public static HashMap<int, Item> slots = new HashMap<>();
        protected readonly List<Player> viewers = new List<Player>();
        public bool ConfirmPurchase = false;
        public int ConfirmPurchaseSlot = 0;
        public Inventory I;
        public AuctionFactory AF = null;
        public int size = 12;
        protected int maxStackSize = 64; //Default
        Player holder;
        Vector3 BA;
        CyberCoreMain CCM;
        BlockEntity blockEntity2 = null;
        BlockEntity blockEntity = null;

        public CurrentPageEnum getCurrentPage()
        {
            return CurrentPage;
        }

        public void setCurrentPage(CurrentPageEnum currentPage)
        {
            CurrentPage = currentPage;
        }

        CurrentPageEnum CurrentPage;
        private int Page = 1;
        public int _inventoryId = 7000000;
        public object _cache = new object();

        private int GetInventoryId()
        {
            lock (_cache)
            {
                _inventoryId++;
                if (_inventoryId == 0x78)
                    _inventoryId++;
                if (_inventoryId == 0x79)
                    _inventoryId++;

                return _inventoryId;
            }
        }

        public ChestBlockEntity getChestBlockEntity()
        {
            return new ChestBlockEntity();
        }

        public AuctionHouse(Player Holder, CyberCoreMain ccm, Vector3 ba, int page = 1)
        {
//        super(Holder, InventoryType.DOUBLE_CHEST, CyberCoreMain.getInstance().SF.getPageHash(page), 9 * 6);//54??
            // super(Holder, InventoryType.DOUBLE_CHEST, ccm.AF.getPageHash(page), 9 * 6);//54??
            I = new Inventory(GetInventoryId(), getChestBlockEntity(), 54, new NbtList());
            //TODO SHOULD SIZE BE 54!?!?
            holder = Holder;
            this.size = 9 * 6;

            CCM = ccm;
            AF = CCM.AF;
//        addItem(SF.getPage(Page));
            Page = page;

            BA = ba;

            this.title = "Auction House Page" + page;

            this.name = title;
            System.Console.WriteLine("Creating AuctionHouse Class");
//        if (CyberCoreMain.getInstance().SF.getPageHash(page) == null) CyberCoreMain.Log.Error("Was LOG ||"+"NUUUUUUUUUUU");
//        setContents(CyberCoreMain.getInstance().SF.getPageHash(page));
        }

        public void GoToSellerPage()
        {
            clearAll();
            setPage(1);
            setContents(AF.getPageHash(getPage(), getHolder().getName()), true);
            ReloadInv();
            // sendContents(getHolder());
            SendAllSlots(getHolder());
        }

        public void ReloadCurrentPage()
        {
            switch (CurrentPage)
            {
                case CurrentPageEnum.ItemPage:
                    clearAll();
                    setPage(getPage());
                    break;
                case CurrentPageEnum.PlayerSellingPage:
                    setPagePlayerSelling(getPage());
                    break;
            }
        }

        public void ClearConfirmPurchase()
        {
            ConfirmPurchase = false;
            ConfirmPurchaseSlot = -1;
        }

        public void DisplayCatagories()
        {
            clearAll();
            for (int i = 0; i < 5 * 9; i++)
            {
                StainedGlass itm = new StainedGlass();
                itm.Color = "Gray";
                Item bi = new ItemBlock(itm);
                bi.setCustomName(ChatColors.Gray + "FEATURE CURRENTLY DISABLED!");
                setItem(i, bi, true);
            }

            ReloadInv();
            SendAllSlots(getHolder());
        }

        public void GoToNextPage()
        {
            setPage(getPage() + 1);
        }

        public void GoToPrevPage()
        {
            setPage(getPage() - 1);
        }

        public void setPagePlayerSelling()
        {
            setPagePlayerSelling(1);
        }

        public void setPagePlayerSelling(int page)
        {
            Page = page;
            CurrentPage = CurrentPageEnum.PlayerSellingPage;
            clearAll();
            setContents(AF.getPageHash(getPage(), getHolder().getName()));
            ReloadInv();
            SendAllSlots(getHolder());
        }

        public int getPage()
        {
            return Page;
        }

        public void setPage(int page)
        {
            if (1 > page) page = 1;
            Page = page;
            CurrentPage = CurrentPageEnum.ItemPage;
            clearAll();
            addItem(AF.getPage(getPage()));
            ReloadInv();
            SendAllSlots(getHolder());
        }


        // public void onOpen(Player who) {
        //     // super.onOpen(who);
        //     ReloadInv();
        //     ContainerOpenPacket containerOpenPacket = new ContainerOpenPacket();
        //     containerOpenPacket.windowId = who.getWindowId(this);
        //     containerOpenPacket.type = this.getType().getNetworkType();
        //     BlockEnderChest chest = null;//who.getViewingEnderChest();
        //     containerOpenPacket.x = who.getFloorX();
        //     containerOpenPacket.y = who.getFloorY() - 2;
        //     containerOpenPacket.z = who.getFloorZ();
        //
        //
        //     who.dataPacket(containerOpenPacket);
        //     this.sendContents(who);
        //
        //
        // }


        public void onClose(Player who)
        {
        }


        // public void onSlotChange(int index, Item before, bool send) {
        //     super.onSlotChange(index, before, send);
        // }

        public void setSize(int size)
        {
            this.size = size;
        }


        public Dictionary<int, Item> getContents()
        {
            Dictionary<int, Item> contents = new Dictionary<int, Item>();

            for (int i = 0; i < this.size; ++i)
            {
                contents[i] = inv.Slots[i];
            }

            return contents;
        }


        public void setContents(Dictionary<int, Item> items)
        {
//        super.setContents(items);
//        if (holder != null) SendAllSlots((Player) holder);
            setContents(items, true);
        }

        public int getSize()
        {
            return inv.Slots.Count;
        }

        public void ReloadInv()
        {
            AHStaticItems si = new AHStaticItems(Page);
            int k = 9;
            setItem(getSize() - k--, si.Redglass, true);
            setItem(getSize() - k--, si.Paper, true);
            setItem(getSize() - k--, si.Grayglass, true);
            setItem(getSize() - k--, si.Diamond, true);
            setItem(getSize() - k--, si.Netherstar, true);
            setItem(getSize() - k--, si.Chest, true);
            setItem(getSize() - k--, si.Grayglass, true);
            setItem(getSize() - k--, si.Dictionary, true);
            setItem(getSize() - k, si.Greenglass, true);
//        sendContents((Player) holder);
        }

        public void ConfirmItemPurchase(int slot)
        {
            clearAll();
            AuctionItemData aid = AF.getAIDFromPage(Page, slot);
            SetupPageToConfirmSingleItem(aid);
            ReloadInv();
            ConfirmPurchase = true;
            ConfirmPurchaseSlot = slot;

//        sendContents((Player) holder);
        }

        /**
     * Maybe use later... Probablly wont work well with /ah I think...
     *
     * @param aid
     * 
     */
        public void SetupPageToConfirmMultiItem(AuctionItemData aid)
        {
            AHStaticItems si = new AHStaticItems(Page);
            Item item = aid.MakePretty();
            Item confrim = si.Confirm;
            Item deny = si.Deny;
            for (int i = 0; i < 5; i++)
            {
                for (int ii = 0; ii < 9; ii++)
                {
                    int key = (i * 9) + ii;
                    Item add = null;
                    switch (ii)
                    {
                        case 0:
                        case 1:
                        case 2:
                        case 3:
                            add = (Item) si.Deny.Clone();
                            setItem(key, add, true);
                            break;
//                    case 1:
//                        if (item.count >= 32) add = si.RmvX32.Clone();
//                        else add = si.Deny.Clone();
//                        setItem(key,add,true);
//                        break;
//                    case 2:
//                        if (item.count >= 10) add = si.RmvX10.Clone();
//                        else add = si.Deny.Clone();
//                        setItem(key,add,true);
//                        break;
//                    case 3:
//                        if (item.count >= 1) add = si.RmvX1.Clone();
//                        else add = si.Deny.Clone();
//                        setItem(key,add,true);
//                        break;
                        case 4:
                            if (i == 2) add = (Item) item.Clone();
                            else add = new ItemBlock(new StainedGlassPane());
                            setItem(key, add, true);
                            break;
                        case 5:
                            if (item.Count >= 1) add = (Item) si.AddX1.Clone();
                            else add = (Item) si.Deny.Clone();
                            setItem(key, add, true);
                            break;
                    }


                    if (ii < 4)
                    {
                        //RED
                        setItem(key, (Item) deny.Clone(), true);
                    }
                    else if (ii == 4)
                    {
                        //White or Item
                        if (i == 2)
                        {
                            //@TODO Get ITem
                            setItem(key, item, true);
                        }
                        else
                        {
                            setItem(key, new ItemBlock(new StainedGlassPane()), true);
                        }
                    }
                    else
                    {
                        //Green
                        setItem(key, (Item) si.Confirm.Clone(), true);
                    }
                }
            }
        }

        public void setContents(Dictionary<int, Item> items, bool send)
        {
            CyberCoreMain.Log.Info("AHHHH>>>>>>> SETTINNGG CCCOONNNTTTZ " + items.Count);
            for (int i = 0; i < this.size - 1; ++i)
            {
//            CyberCoreMain.Log.Error("Was LOG ||"+"SETTING ITEM IN KEY " + i + " VVVVVVVV " + items.get(i).getClass().getName());
                if (!items.ContainsKey(i))
                {
                    if (inv.isEmpty(i))
                    {
                        // this.clear(i);
                    }
                }
                else if (!this.setItem(i, items[i], send))
                {
                    // this.clear(i);
                }
            }


            ReloadInv();
        }


        public bool setItem(int index, Item item, bool send)
        {
            item = (Item) item.Clone();
//    CyberCoreMain.Log.Error("Was LOG ||"+"INNNNEEDDDDEDEE >> "+index);
//    CyberCoreMain.Log.Error("Was LOG ||"+"INNNNEEDDDDEDEE >> "+item.getClass().getName());
//        CyberCoreMain.Log.Error("Was LOG ||"+"INNNNEEDDDDEDEE >> "+item.getCount());
//        CyberCoreMain.Log.Error("Was LOG ||"+"INNNNEEDDDDEDEE >> "+item.getId());
            if (index >= 0 && index < getSize())
            {
                if (item.Id != 0 && item.Count > 0)
                {
                    Player holder = getHolder();
                    // if (holder instanceof Entity && !send) {
                    //     EntityInventoryChangeEvent ev =
                    //         new EntityInventoryChangeEvent((Entity) holder, this.getItem(index), item, index);
                    //     Server.getInstance().getPluginManager().callEvent(ev);
                    //     if (ev.isCancelled())
                    //     {
                    //         sendSlot(index, getHolder());
                    //         return false;
                    //     }
                    //
                    //     item = ev.getNewItem();
                    // }

                    // if (holder instanceof BlockEntity) {
                    //     ((BlockEntity) holder).setDirty();
                    // }
//
                    Item old = inv.Slots[index];
                    inv.SetInventorySlot(index, (Item) item.Clone());
//            CyberCoreMain.Log.Error("Was LOG ||"+"AAAAAAAAAAAAAAAAAAAA >> "+index);
//            CyberCoreMain.Log.Error("Was LOG ||"+"AAAAAAAAAAAAAAAAAAAA >> "+old);
//            CyberCoreMain.Log.Error("Was LOG ||"+"AAAAAAAAAAAAAAAAAAAA >> "+send);
//            this.onSlotChange(index, old, send);
                    // if (getHolder() != null) sendSlot(index, getHolder());
                    return true;

//            return super.setItem(index,item,send);
                }
                else
                {
                    // return this.clear(index);
                }
            }
            else
            {
                return false;
            }

            return true;
        }


        // public void sendContents(Player player)
        // {
        //     ArrayList<Player> al = new ArrayList<>();
        //     al.add(player);
        //     this.sendContents(al.toArray(new Player[1]));
        // }

//    public bool setItem(int index, Item item, bool send) {
//        item = item.Clone();
//        if (index >= 0 && index < this.size) {
//            if (item.getId() != 0 && item.getCount() > 0) {
//                InventoryHolder holder = this.getHolder();
//                if (holder instanceof Entity && !send) {
//                    EntityInventoryChangeEvent ev = new EntityInventoryChangeEvent((Entity) holder, this.getItem(index), item, index);
//                    Server.getInstance().getPluginManager().callEvent(ev);
//                    if (ev.isCancelled()) {
//                        this.sendSlot(index, (Collection) this.getViewers());
//                        return false;
//                    }
//
//                    item = ev.getNewItem();
//                }
//
//                if (holder instanceof BlockEntity) {
//                    ((BlockEntity) holder).setDirty();
//                }
//
//                Item old = this.getItem(index);
//                CyberCoreMain.Log.Error("Was LOG ||"+"SEEEEETTTTT >> "+index);
//                CyberCoreMain.Log.Error("Was LOG ||"+"SEEEEETTTTT >> "+item.getClass().getName());
//                CyberCoreMain.Log.Error("Was LOG ||"+"SEEEEETTTTT >> "+item.Clone().getClass().getName());
//                CyberCoreMain.Log.Error("Was LOG ||"+"SEEEEETTTTT >> "+slots.getClass().getName());
//                slots.put((int) index, item.Clone());
//                this.onSlotChange(index, old, send);
//                return true;
//            } else {
//                slots.put(index, new ItemBlock(new BlockAir(), 0, 0));
//                this.onSlotChange(index, this.getItem(index), send);
//                CyberCoreMain.Log.Error("Was LOG ||"+"MAN CLLEEAARRR");
////                return this.clear(index);
//                return true;
//            }
//        } else {
//            return false;
//        }
//    }

        public void SendAllSlots(Player p)
        {
            for (int i = 0; i < getSize(); i++)
            {
                // sendSlot(i, p);
            }
        }

        public void SetupPageNotEnoughMoney(AuctionItemData aid)
        {
            // CorePlayer cp = (CorePlayer) getHolder();
            // StaticItems si = new StaticItems(Page);
            // Item item = aid.MakePretty();
            // Item deny = si.Deny.Clone();
            // Item deny2 = si.Deny.Clone();
            // CurrentPage = Confirm_Purchase_Not_Enough_Money;
            // deny.setCustomName(ChatColors.Red + "Not Enough Money!");
            // for (int i = 0; i < 5; i++)
            // {
            //     for (int ii = 0; ii < 9; ii++)
            //     {
            //         int key = (i * 9) + ii;
            //         if (ii != 4)
            //         {
            //             //RED
            //             setItem(key, deny.Clone(), true);
            //         }
            //         else
            //         {
            //             //White or Item
            //             if (i == 2)
            //             {
            //                 //@TODO Get ITem
            //                 setItem(key, item, true);
            //             }
            //             else if (i == 0)
            //             {
            //                 Item g = si.Gold.Clone();
            //                 g.setCustomName(ChatColors.GOLD + " Your money: " + cp.getMoney());
            //                 setItem(key, g, true);
            //             }
            //             else
            //             {
            //                 Item r = Item.get(160, 14);
            //                 r.setCustomName(ChatColors.Red + "Not Enough Money \n" + ChatColors.Yellow +
            //                                 " Your Balance : " + cp.getMoney() + "\n" + ChatColors.AQUA +
            //                                 "Item Cost : " + aid.getCost());
            //                 setItem(key, r, true);
            //             }
            //         }
            //     }
            // }
        }

        public void SetupPageToConfirmSingleItem(AuctionItemData aid)
        {
            // CurrentPage = Confirm_Purchase;
            // CorePlayer cp = (CorePlayer) getHolder();
            // StaticItems si = new StaticItems(Page);
            // Item item = aid.MakePretty();
            // Item confrim = si.Confirm.Clone();
            // Item deny = si.Deny.Clone();
            // for (int i = 0; i < 5; i++)
            // {
            //     for (int ii = 0; ii < 9; ii++)
            //     {
            //         int key = (i * 9) + ii;
            //         if (ii < 4)
            //         {
            //             //RED
            //             setItem(key, deny.Clone(), true);
            //         }
            //         else if (ii == 4)
            //         {
            //             //White or Item
            //             if (i == 2)
            //             {
            //                 //@TODO Get ITem
            //                 setItem(key, item, true);
            //             }
            //             else if (i == 0)
            //             {
            //                 Item g = si.Gold.Clone();
            //                 g.setCustomName(ChatColors.GOLD + " Your money: " + cp.getMoney());
            //                 setItem(key, g, true);
            //             }
            //             else
            //             {
            //                 setItem(key, Item.get(160), true);
            //             }
            //         }
            //         else
            //         {
            //             //Green
            //             setItem(key, confrim.Clone(), true);
            //         }
            //     }
            // }
        }

        // public void setItem2(int index, Item item)
        // {
        //     setItem(index, item.Clone());
        //     this.onSlotChange(index, null);
        // }


        // public bool contains(Item item)
        // {
        //     int count = Math.max(1, item.getCount());
        //     bool checkDamage = item.hasMeta();
        //     bool checkTag = item.getCompoundTag() != null;
        //     for (Item i :
        //     this.getContents().values()) {
        //         if (item.equals(i, checkDamage, checkTag))
        //         {
        //             count -= i.getCount();
        //             if (count <= 0)
        //             {
        //                 return true;
        //             }
        //         }
        //     }
        //
        //     return false;
        // }


        // public Dictionary<int, Item> all(Item item)
        // {
        //     Dictionary<int, Item> slots = new HashMap<>();
        //     bool checkDamage = item.hasMeta();
        //     bool checkTag = item.getCompoundTag() != null;
        //     for (Dictionary.Entry < int, Item > entry : this.getContents().entrySet())
        //     {
        //         if (item.equals(entry.getValue(), checkDamage, checkTag))
        //         {
        //             slots.put(entry.getKey(), entry.getValue());
        //         }
        //     }
        //
        //     return slots;
        // }
        //
        //
        // public void remove(Item item)
        // {
        //     bool checkDamage = item.hasMeta();
        //     bool checkTag = item.getCompoundTag() != null;
        //     for (Dictionary.Entry < int, Item > entry : this.getContents().entrySet())
        //     {
        //         if (item.equals(entry.getValue(), checkDamage, checkTag))
        //         {
        //             this.clear(entry.getKey());
        //         }
        //     }
        // }

//    
//    public bool setItem(int index, Item item) {
//        item = item.Clone();
//        if (index < 0 || index >= this.size) {
//            return false;
//        } else if (item.getId() == 0 || item.getCount() <= 0) {
//            return this.clear(index);
//        }
//
//
//        Item old = this.getItem(index);
//        setItem(index, item.Clone());
//        this.onSlotChange(index, old);
//        //if (getItem(0).getId() == 0 || getItem(4).getId() == 0) setItem2(2, Item.get(Item.ANVIL));
//
//        return true;
//    }
//
//    
//    public bool setItem(int index, Item item, bool send) {
//        return false;
//    }


        // public int first(Item item)
        // {
        //     int count = Math.max(1, item.getCount());
        //     bool checkDamage = item.hasMeta();
        //     bool checkTag = item.getCompoundTag() != null;
        //     for (Dictionary.Entry < int, Item > entry : this.getContents().entrySet())
        //     {
        //         if (item.equals(entry.getValue(), checkDamage, checkTag) && entry.getValue().getCount() >= count)
        //         {
        //             return entry.getKey();
        //         }
        //     }
        //
        //     return -1;
        // }
        //
        //
        // public int first(Item item, bool exact)
        // {
        //     return 0;
        // }
        //
        //
        // public int firstEmpty(Item item)
        // {
        //     for (int i = 0; i < this.size; ++i)
        //     {
        //         if (this.getItem(i).getId() == Item.AIR)
        //         {
        //             return i;
        //         }
        //     }
        //
        //     return -1;
        // }


        public void decreaseCount(int slot)
        {
        }


        // public bool clear(int index)
        // {
        //     return this.clear(index, true);
        // }


//         public bool clear(int index, bool send)
//         {
// //        CyberCoreMain.Log.Error("Was LOG ||"+"AAAAAAAAAAAAAA" + index);
//             if (this.slots.containsKey(index))
//             {
//                 Item item = new ItemBlock(new BlockAir(), null, 0);
//                 Item old = this.slots.get(index);
// //            if (item.getId() != Item.AIR) {
// //                setItem(index, item.Clone());
// //            } else {
//                 this.slots.remove(index);
// //            }
// //            this.onSlotChange(index, old, send);
//             }
//             else if (send)
//             {
//                 Item item = new ItemBlock(new BlockAir(), null, 0);
//                 Item old = this.slots.get(index);
// //            setItem(index, item.Clone(),true);
// //            this.onSlotChange(index, old, true);
//                 slots.remove(index);
//             }
//
//             if (getHolder() != null) sendSlot(index, getHolder());
// //        CyberCoreMain.Log.Error("Was LOG ||"+"CLEARRRR###" + index);
//             return true;
//         }


        public void clearAll()
        {
            inv.Clear();
        }


        public bool isFull()
        {
            return false;
        }


        public bool isEmpty()
        {
            return false;
        }


        public List<Player> getViewers()
        {
            return viewers;
        }


        public Player getHolder()
        {
            return (Player) this.holder;
        }


        // public bool open(Player who) {
        //     this.onOpen(who);
        //
        //     return true;
        // }


        // public void close(Player who) {
        //     this.onClose(who);
        // }

        public void onSlotChange(int index, Item before)
        {
            if (index == MainPageItemRef.Reload)
            {
                SendPage(Page);
            }

            // this.sendSlot(index, getHolder());
        }

        public void SendPage(int page)
        {
        }

        private OpenPlayerInventory inv = new OpenPlayerInventory(null);

        public List<Item> addItem(List<Item> slots)
        {
            if (slots.Count > 5 * 9)
            {
                List<Item> I = new List<Item>();
                for (int i = 0; i < 5 * 9; i++)
                {
                    I.Add(slots[i]);
                }

                CyberCoreMain.Log.Error("ERROR TRIED TO ADD " + slots.Count);
                foreach (var i in I)
                {
                    inv.AddItem(i, true);
                }
            }
            else
            {
                foreach (var i in slots)
                {
                    inv.AddItem(i, true);
                }
            }

            return inv.GetSlots();
        }

        // public void sendContents(Collection<Player> players)
        // {
        //     this.sendContents(players.stream().toArray(Player[]::new));
        //     inv.SendSetSlot();
        // }


        // public void sendSlot(int index, Player player)
        // {
        //     this.sendSlot(index, new Player[] {player});
        // }


        // public InventoryType getType()
        // {
        //     return InventoryType.DOUBLE_CHEST;
        // }

//    
//    public void sendContents(Player player) {
//        this.sendContents(new Player[]{player});
//    }


        public enum CurrentPageEnum
        {
            ItemPage,
            PlayerSellingPage,
            Expired,
            Confirm_Purchase,
            Confirm_Purchase_Not_Enough_Money,
        }
//    public void sendSlot(int index, Player[] players) {
//        ContainerSetSlotPacket pk = new ContainerSetSlotPacket();
//        pk.slot = index;
//        pk.item = this.getItem(index).Clone();
//
//        for (Player player : players) {
//            int id = player.getWindowId(this);
//            if (id == -1) {
//                this.close(player);
//                continue;
//            }
//            pk.windowid = (byte) id;
//            player.dataPacket(pk);
//        }
//    }


        public class MainPageItemRef
        {
            public static readonly int Size = 6 * 9;
            public static readonly int LastPage = Size - 9;

            public static readonly int Search = Size - 8;

            //        public static readonly int NULL = Size - 7;
            public static readonly int PlayerSelling = Size - 6;
            public static readonly int Reload = Size - 5;

            public static readonly int Catagories = Size - 4;

            //        public static readonly int MULL = Size - 3;
            public static readonly int ListItem = Size - 2;
            public static readonly int NextPage = Size - 1;
        }
    }
}