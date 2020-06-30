using System;
using System.Collections.Generic;
using System.Numerics;
using CyberCore.Manager.Factions.Windows;
using CyberCore.Utils;
using CyberCore.Utils.Data;
using fNbt;
using MiNET;
using MiNET.BlockEntities;
using MiNET.Blocks;
using MiNET.Entities;
using MiNET.Items;
using MiNET.Utils;

namespace CyberCore.Manager.Shop
{
    public class ShopInv
    {
        protected String name;
        protected String title;
        //    public static HashMap<Integer, Item> slots = new HashMap<>();
        protected  List<Player> viewers = new List<Player>();
        public bool AdminMode = false;
        public bool ConfirmPurchase = false;
        public int ConfirmPurchaseSlot = 0;
        public ShopFactory SF = null;
        /**
     * Maybe use later... Probablly wont work well with /ah I think...
     *
     * @param aid
     */
        public ShopMysqlData MultiConfirmData = null;
        public CyberCoreMain CCM;
        public Inventory I;
        public CurrentPageEnum CurrentPage;
        public ShopCategory Catg = ShopCategory.NA;
        protected int maxStackSize = 64;
        CorePlayer holder;
        Vector3 BA;
        BlockEntity blockEntity2 = null;
        BlockEntity blockEntity = null;
        bool SetupPageToFinalConfirmItemSell = false;
        private int Page = 1;
        public ShopInv(CorePlayer corePlayer, CyberCoreMain ccm, Vector3 toVector3, int pg = 1)
        {
            CCM = ccm;
            holder = corePlayer;
            BA = BA;
            I = new Inventory(GetInventoryId(), getChestBlockEntity(), 54, new NbtList());
        }
        public ChestBlockEntity getChestBlockEntity()
        {
            return new ChestBlockEntity();
        }
        
        
        public object _cache = new object();
        
        public int _inventoryId = 8500000;
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
        
        
        public ShopCategory getCatg() {
        return Catg;
    }

    public void setCatg(ShopCategory catg) {
        Catg = catg;
    }

    public CurrentPageEnum getCurrentPage() {
        return CurrentPage;
    }

    public void setCurrentPage(CurrentPageEnum currentPage) {
        CurrentPage = currentPage;
    }

    public void GoToSellerPage() {
        clearAll();
        setPage(1);
        setContents(SF.getPageHash(getPage(), getCatg(), AdminMode), true);
        ReloadInv();
        sendContents(getHolder());
        SendAllSlots(getHolder());
    }

    public void ReloadCurrentPage() {
        if(CurrentPage == null)CurrentPage = CurrentPageEnum.ItemPage;
        switch (CurrentPage) {
            case ItemPage:
                clearAll();
                setPage(getPage());
                break;
            case PlayerSellingPage:
                setPagePlayerSelling(getPage());
                break;


        }
    }

    public void ClearConfirmPurchase() {

        ConfirmPurchase = false;
        ConfirmPurchaseSlot = -1;
    }

    public void DisplayCatagories() {
        clearAll();
        setCurrentPage(CurrentPageEnum.Catagories);
        ShopStaticItems si = new ShopStaticItems(getPage());
        for (int i = 0; i < 5; i++) {
            for (int ii = 0; ii < 9; ii++) {
                Item bi = null;
                int slot = (i * 9) + ii;
                if (i == 0 || i == 4 || ii == 8 || ii == 7 || ii == 0 || ii == 1) {
//                    bi = new ItemBlock(new BlockGlassPaneStained(0));
                    bi = new ItemBlock(new Bedrock());
                    bi.setCustomName(" ");
                } else {
                    if (i == 1) {
                        if (ii == 2) {
                            bi = (Item) si.Spawner.Clone();
                        } else if (ii == 3) {
                            bi = (Item) si.FoodCatagoty.Clone();
                        } else if (ii == 4) {
                            bi = (Item) si.WeaponsCatagory.Clone();
                        } else if (ii == 5) {
                            bi = (Item) si.BuildingCatagory.Clone();
                        } else if (ii == 6) {
                            bi = (Item) si.RaidingCatagory.Clone();
                        }
                    }
                }
                if (bi == null) {
                    bi = new ItemBlock(new Bedrock());
                    bi.setCustomName(ChatColors.Gray + "FEATURE CURRENTLY DISABLED!");
                }
                setItem(slot, bi, true);
            }
        }
        ReloadInv();
        SendAllSlots(getHolder());
    }

    public void GoToNextPage() {
        setPage(getPage() + 1);
    }

    public void GoToPrevPage() {
        setPage(getPage() - 1);
    }

    public void setPagePlayerSelling() {
        setPagePlayerSelling(1);
    }

    public void setPagePlayerSelling(Integer page) {
        Page = page;
        CurrentPage = CurrentPageEnum.PlayerSellingPage;
        clearAll();
        setContents(SF.getPageHash(getPage(), getCatg(), AdminMode));
        ReloadInv();
        SendAllSlots(getHolder());
    }

    public int getPage() {
        return Page;
    }

    public void setPage(int page) {
        if (1 > page) page = 1;
        Page = page;
        CurrentPage = CurrentPageEnum.ItemPage;
        clearAll();
        addItem(SF.getPage(getPage(), getCatg(), AdminMode));
        ReloadInv();
        SendAllSlots(getHolder());
    }

    @Override
    public void onOpen(Player who) {
        super.onOpen(who);
        ReloadInv();
        ContainerOpenPacket containerOpenPacket = new ContainerOpenPacket();
        containerOpenPacket.windowId = who.getWindowId(this);
        containerOpenPacket.type = this.getType().getNetworkType();
        BlockEnderChest chest = null;//who.getViewingEnderChest();
        containerOpenPacket.x = who.getFloorX();
        containerOpenPacket.y = who.getFloorY() - 2;
        containerOpenPacket.z = who.getFloorZ();


        who.dataPacket(containerOpenPacket);
        this.sendContents(who);


    }

    @Override
    public void onClose(Player who) {

    }

    @Override
    public void onSlotChange(int index, Item before, boolean send) {
        super.onSlotChange(index, before, send);
    }

    public void setSize(int size) {
        this.size = size;
    }

    @Override
    public Map<Integer, Item> getContents() {

        Map<Integer, Item> contents = new HashMap<>();

        for (int i = 0; i < this.getSize(); ++i) {
            contents.put(i, this.getItem(i));
        }

        return contents;
    }

    @Override
    public void setContents(Map<Integer, Item> items) {
//        super.setContents(items);
//        if (holder != null) SendAllSlots((Player) holder);
        setContents(items, true);
    }

    public void ReloadInv() {
        AHStaticItems si = new AHStaticItems(Page);
        int k = 9;
        setItem(AuctionHouse.AuctionHouse.MainPageItemRef.LastPage, si.Redglass);
//        setItem( k--, si.Paper);
//        setItem( k--, si.Grayglass);
        if(AdminMode){
            Item i = si.Deny.clone();
            i.setCustomName(TextFormat.GOLD+""+TextFormat.BOLD+"Disable Admin Mode");
            setItem(AuctionHouse.AuctionHouse.MainPageItemRef.ToggleAdmin, i);
        }else{
            Item i = si.Greenglass.clone();
            i.setCustomName(TextFormat.GOLD+""+TextFormat.BOLD+"Enable Admin Mode");
            setItem(AuctionHouse.AuctionHouse.MainPageItemRef.ToggleAdmin, i);
        }
        setItem(AuctionHouse.AuctionHouse.MainPageItemRef.Reload, si.Netherstar);
        setItem(AuctionHouse.AuctionHouse.MainPageItemRef.Catagories, si.CatagoryChest);
//        setItem( k--, si.Grayglass);
//        setItem( k--, si.Map);
        setItem(AuctionHouse.AuctionHouse.MainPageItemRef.NextPage, si.Greenglass);
//        sendContents((Player) holder);
    }

    public void ConfirmItemPurchase(int slot, boolean admin) {
        clearAll();
        ShopMysqlData aid = SF.getItemFrom(Page, slot, admin);
//        close(getHolder());
//        SetupFormToConfirmItem(aid);
        SetupPageToConfirmMultiItem(aid);
        ReloadInv();
        ConfirmPurchase = true;
        ConfirmPurchaseSlot = slot;

        sendContents((Player) holder);


    }

    public void SetupFormToConfirmItem(ShopMysqlData aid) {
        getHolder().removeWindow(this);
        getHolder().showFormWindow(new ShopChooseBuySell(aid, (CorePlayer) getHolder()));
    }

    public void SetupPageToConfirmMultiItem(ShopMysqlData aid) {
        CurrentPage = CurrentPageEnum.PlayerSellingPage;
        AHStaticItems si = new AHStaticItems(Page);
        CorePlayer cp = (CorePlayer) getHolder();
        Item item = aid.getItem();
        MultiConfirmData = aid;
        Collection<Item> ai = cp.getInventory().all(aid.getItem(true)).values();
        int ic = 0;
        for (Item iii : ai) {
            ic += iii.getCount();
        }
        for (int i = 0; i < 5; i++) {
            for (int ii = 0; ii < 9; ii++) {
                int key = (i * 9) + ii;
                Item add = null;
                if (i == 0) {
                    if (ii <= 3) {
                        add = si.ChestSell.clone();
                        add.setLore("You have " + ic + " available for sale!", "and can sell for a total of $" + (ic * aid.getSellPrice()), "" + key);
                        setItem(key, add, true);
                    } else if (ii == 4) {
                        Item g = si.Gold.clone();
                        g.setCustomName(TextFormat.GOLD + " Your money: " + cp.getMoney());
                        setItem(key, g, true);
                    } else {
                        add = si.ChestBuy.clone();
                        int mb = (int) Math.floor(cp.getMoney() / aid.getPrice());
                        add.setLore("You can buy " + mb + " " + item.getName() + "(s)", "" + key);
                        setItem(key, add, true);
                    }
                    continue;
                }
                add = null;
                switch (ii) {
                    case 0:
//                    case 1:
//                    case 2:
//                    case 3:
                        if (ic >= 64) add = si.RmvX64.clone();
                        else add = si.RmvX64N.clone();
                        setItem(key, add, true);
                        break;
                    case 1:
                        if (ic >= 32) add = si.RmvX32.clone();
                        else add = si.RmvX32N.clone();
                        setItem(key, add, true);
                        break;
                    case 2:
                        if (ic >= 10) add = si.RmvX10.clone();
                        else add = si.RmvX10N.clone();
                        setItem(key, add, true);
                        break;
                    case 3:
                        if (ic >= 1) add = si.RmvX1.clone();
                        else add = si.RmvX1N.clone();
                        setItem(key, add, true);
                        break;
                    case 4:
                        if (i == 2) add = item.clone();
                        else {
                            add = Item.get(160).clone();
                            add.setCustomName("--------");
                        }
                        setItem(key, add, true);
                        break;
                    case 5:
                        if (cp.getMoney() >= aid.getPrice()) add = si.AddX1.clone();
                        else add = si.AddX1N.clone();
                        setItem(key, add, true);
                        break;
                    case 6:
                        if (cp.getMoney() >= aid.getPrice() * 10) add = si.AddX10.clone();
                        else add = si.AddX10N.clone();
                        setItem(key, add, true);
                        break;
                    case 7:
                        if (cp.getMoney() >= aid.getPrice() * 32) add = si.AddX32.clone();
                        else add = si.AddX32N.clone();
                        setItem(key, add, true);
                        break;
                    case 8:
                        if (cp.getMoney() >= aid.getPrice() * 64) add = si.AddX64.clone();
                        else add = si.AddX64N.clone();
                        setItem(key, add, true);
                        break;
                }


//                if (ii < 4) {
//                    //RED
//                    setItem(key, deny.clone(), true);
//                } else if (ii == 4) {
//                    //White or Item
//                    if (i == 2) {
//                        //@TODO Get ITem
//                        setItem(key, item, true);
//                    } else {
//                        setItem(key, Item.get(160), true);
//                    }
//                } else {
//                    //GREEN
//                    setItem(key, confrim.clone(), true);
//                }
            }
        }
    }

    public void setContents(Map<Integer, Item> items, boolean send) {
        System.out.println("SETTINNGG CCCOONNNTTTZ " + items.size());
        for (int i = 0; i < this.size - 1; ++i) {

//            System.out.println("SETTING ITEM IN KEY " + i + " VVVVVVVV " + items.get(i).getClass().getName());
            if (!items.containsKey(i)) {
                if (this.slots.containsKey(i)) {
                    this.clear(i);
                }
            } else if (!this.setItem(i, (Item) ((Map) items).get(i), send)) {
                this.clear(i);
            }
        }


        ReloadInv();
    }

    @Override
    public boolean setItem(int index, Item item, boolean send) {
        item = item.clone();
//    System.out.println("INNNNEEDDDDEDEE >> "+index);
//    System.out.println("INNNNEEDDDDEDEE >> "+item.getClass().getName());
//        System.out.println("INNNNEEDDDDEDEE >> "+item.getCount());
//        System.out.println("INNNNEEDDDDEDEE >> "+item.getId());
        if (index >= 0 && index < getSize()) {
            if (item.getId() != 0 && item.getCount() > 0) {
                InventoryHolder holder = this.getHolder();
                if (holder instanceof Entity && !send) {
                    EntityInventoryChangeEvent ev = new EntityInventoryChangeEvent((Entity) holder, this.getItem(index), item, index);
                    Server.getInstance().getPluginManager().callEvent(ev);
                    if (ev.isCancelled()) {
                        this.sendSlot(index, this.getViewers());
                        return false;
                    }

                    item = ev.getNewItem();
                }

                if (holder instanceof BlockEntity) {
                    ((BlockEntity) holder).setDirty();
                }
//
                Item old = this.getItem(index);
                slots.put(index, item.clone());
//            System.out.println("AAAAAAAAAAAAAAAAAAAA >> "+index);
//            System.out.println("AAAAAAAAAAAAAAAAAAAA >> "+old);
//            System.out.println("AAAAAAAAAAAAAAAAAAAA >> "+send);
//            this.onSlotChange(index, old, send);
                if (getHolder() != null) sendSlot(index, getHolder());
                return true;

//            return super.setItem(index,item,send);
            } else {
                return this.clear(index);
            }
        } else {
            return false;
        }
    }

//    public boolean setItem(int index, Item item, boolean send) {
//        item = item.clone();
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
//                System.out.println("SEEEEETTTTT >> "+index);
//                System.out.println("SEEEEETTTTT >> "+item.getClass().getName());
//                System.out.println("SEEEEETTTTT >> "+item.clone().getClass().getName());
//                System.out.println("SEEEEETTTTT >> "+slots.getClass().getName());
//                slots.put((Integer) index, item.clone());
//                this.onSlotChange(index, old, send);
//                return true;
//            } else {
//                slots.put(index, new ItemBlock(new BlockAir(), 0, 0));
//                this.onSlotChange(index, this.getItem(index), send);
//                System.out.println("MAN CLLEEAARRR");
////                return this.clear(index);
//                return true;
//            }
//        } else {
//            return false;
//        }
//    }

    @Override
    public void sendContents(Player player) {
        ArrayList<Player> al = new ArrayList<>();
        al.add(player);
        this.sendContents(al.toArray(new Player[1]));
    }

    public void SendAllSlots(Player p) {
        ArrayList<Player> al = new ArrayList<>();
        al.add(p);
        for (int i = 0; i < getSize(); i++) {
            sendSlot(i, p);
        }
    }

    public void SetupPageNotEnoughMoney(ShopMysqlData aid) {
        CorePlayer cp = (CorePlayer) getHolder();
        AHStaticItems si = new AHStaticItems(Page);
        Item item = aid.getItem(true);
        Item deny = si.Deny.clone();
        Item deny2 = si.Deny.clone();
        CurrentPage = CurrentPageEnum.Confirm_Purchase_Not_Enough_Money;
        deny.setCustomName(TextFormat.RED + "Not Enough Money!");
        for (int i = 0; i < 5; i++) {
            for (int ii = 0; ii < 9; ii++) {
                int key = (i * 9) + ii;
                if (ii != 4) {
                    //RED
                    setItem(key, deny.clone(), true);
                } else {
                    //White or Item
                    if (i == 2) {
                        //@TODO Get ITem
                        setItem(key, item, true);
                    } else if (i == 0) {
                        Item g = si.Gold.clone();
                        g.setCustomName(TextFormat.GOLD + " Your money: " + cp.getMoney());
                        setItem(key, g, true);
                    } else {
                        Item r = Item.get(160, 14);
                        r.setCustomName(TextFormat.RED + "Not Enough Money \n" + TextFormat.YELLOW + " Your Balance : " + cp.getMoney() + "\n" + TextFormat.AQUA + "Item Cost : " + aid.getPrice());
                        setItem(key, r, true);
                    }
                }
            }
        }
    }

    public void SetupPageToFinalConfirmItem(ShopMysqlData aid) {
        SetupPageToFinalConfirmItem(aid, 1, false);
    }

    //    int SetupPageToFinalConfirmItemCount = 0;
    public void SetupPageToFinalConfirmItem(ShopMysqlData aid, int count, boolean sell) {
        CurrentPage = CurrentPageEnum.Confirm_Purchase_Final;
        SetupPageToFinalConfirmItemSell = sell;
//        SetupPageToFinalConfirmItemCount = count;
        CorePlayer cp = (CorePlayer) getHolder();
        AHStaticItems si = new AHStaticItems(Page);
        Item item = aid.getItem().clone();
        item.setCount(count);
        Item confrim = si.Confirm.clone();
        if (sell) confrim = si.ConfirmSell.clone();
        confrim.setCount(count);
        if (sell) {
            confrim.setCustomName("Sell " + aid.getPrettyString(count, !sell));
            confrim.setLore(TextFormat.AQUA + "Sell Price Per Item: " + TextFormat.GREEN + aid.getSellPrice(), TextFormat.AQUA + "Sell Quantity: " + TextFormat.GREEN + count, TextFormat.GRAY + "------------------------", TextFormat.AQUA + "Final Sale price: " + TextFormat.GREEN + aid.getPrice(count));
        } else {
            confrim.setCustomName("Buy " + aid.getPrettyString(count, !sell));
            confrim.setLore(TextFormat.AQUA + "Buy Price Per Item: " + TextFormat.GREEN + aid.getPrice(), TextFormat.AQUA + "Purchase Quantity: " + TextFormat.GREEN + count, TextFormat.GRAY + "------------------------", TextFormat.AQUA + "Final Purchase price: " + TextFormat.GREEN + aid.getPrice(count));
        }
        Item deny = si.Deny.clone();
        for (int i = 0; i < 5; i++) {
            for (int ii = 0; ii < 9; ii++) {
                int key = (i * 9) + ii;
                if (ii < 4) {
                    //RED
                    setItem(key, deny.clone(), true);
                } else if (ii == 4) {
                    //White or Item
                    if (i == 2) {
                        //@TODO Get ITem
                        setItem(key, item, true);

                    } else if (i == 0) {
                        Item g = si.Gold.clone();
                        g.setCustomName(TextFormat.GOLD + " Your money: " + cp.getMoney());
                        setItem(key, g, true);
                    } else {
                        setItem(key, Item.get(160), true);
                    }
                } else {
                    //GREEN
                    setItem(key, confrim.clone(), true);
                }
            }
        }
    }

    public void setItem2(int index, Item item) {
        setItem(index, item.clone());
        this.onSlotChange(index, null);
    }

    @Override
    public boolean contains(Item item) {
        int count = Math.max(1, item.getCount());
        boolean checkDamage = item.hasMeta();
        boolean checkTag = item.getCompoundTag() != null;
        for (Item i : this.getContents().values()) {
            if (item.equals(FactionSettingsWindow.i, checkDamage, checkTag)) {
                count -= FactionSettingsWindow.i.getCount();
                if (count <= 0) {
                    return true;
                }
            }
        }

        return false;
    }

    @Override
    public Map<Integer, Item> all(Item item) {
        Map<Integer, Item> slots = new HashMap<>();
        boolean checkDamage = item.hasMeta();
        boolean checkTag = item.getCompoundTag() != null;
        for (Map.Entry<Integer, Item> entry : this.getContents().entrySet()) {
            if (item.equals(entry.getValue(), checkDamage, checkTag)) {
                slots.put(entry.getKey(), entry.getValue());
            }
        }

        return slots;
    }

    @Override
    public void remove(Item item) {
        boolean checkDamage = item.hasMeta();
        boolean checkTag = item.getCompoundTag() != null;
        for (Map.Entry<Integer, Item> entry : this.getContents().entrySet()) {
            if (item.equals(entry.getValue(), checkDamage, checkTag)) {
                this.clear(entry.getKey());
            }
        }
    }

//    @Override
//    public boolean setItem(int index, Item item) {
//        item = item.clone();
//        if (index < 0 || index >= this.size) {
//            return false;
//        } else if (item.getId() == 0 || item.getCount() <= 0) {
//            return this.clear(index);
//        }
//
//
//        Item old = this.getItem(index);
//        setItem(index, item.clone());
//        this.onSlotChange(index, old);
//        //if (getItem(0).getId() == 0 || getItem(4).getId() == 0) setItem2(2, Item.get(Item.ANVIL));
//
//        return true;
//    }
//
//    @Override
//    public boolean setItem(int index, Item item, boolean send) {
//        return false;
//    }

    @Override
    public int first(Item item) {
        int count = Math.max(1, item.getCount());
        boolean checkDamage = item.hasMeta();
        boolean checkTag = item.getCompoundTag() != null;
        for (Map.Entry<Integer, Item> entry : this.getContents().entrySet()) {
            if (item.equals(entry.getValue(), checkDamage, checkTag) && entry.getValue().getCount() >= count) {
                return entry.getKey();
            }
        }

        return -1;
    }

    @Override
    public int first(Item item, boolean exact) {
        return 0;
    }

    @Override
    public int firstEmpty(Item item) {
        for (int i = 0; i < this.size; ++i) {
            if (this.getItem(i).getId() == Item.AIR) {
                return i;
            }
        }

        return -1;
    }

    @Override
    public void decreaseCount(int slot) {

    }

    @Override
    public boolean clear(int index) {
        return this.clear(index, true);
    }

    @Override
    public boolean clear(int index, boolean send) {
//        System.out.println("AAAAAAAAAAAAAA" + index);
        if (this.slots.containsKey(index)) {
            Item item = new ItemBlock(new BlockAir(), null, 0);
            Item old = this.slots.get(index);
//            if (item.getId() != Item.AIR) {
//                setItem(index, item.clone());
//            } else {
            this.slots.remove(index);
//            }
//            this.onSlotChange(index, old, send);
        } else if (send) {
            Item item = new ItemBlock(new BlockAir(), null, 0);
            Item old = this.slots.get(index);
//            setItem(index, item.clone(),true);
//            this.onSlotChange(index, old, true);
            slots.remove(index);
        }
        if (getHolder() != null) sendSlot(index, getHolder());
//        System.out.println("CLEARRRR###" + index);
        return true;
    }

    @Override
    public void clearAll() {
        for (Integer index : this.getContents().keySet()) {
            this.clear(index);
        }
    }

    @Override
    public boolean isFull() {
        return false;
    }

    @Override
    public boolean isEmpty() {
        return false;
    }

    @Override
    public Set<Player> getViewers() {
        return viewers;
    }

    @Override
    public Player getHolder() {
        return (Player) this.holder;
    }

    @Override
    public boolean open(Player who) {
        this.onOpen(who);

        return true;
    }

    @Override
    public void close(Player who) {
        this.onClose(who);
    }

    public void onSlotChange(int index, Item before) {

        switch (index) {
            case AuctionHouse.MainPageItemRef.Reload:
                SendPage(Page);
                break;
        }
        this.sendSlot(index, this.getViewers());
    }

    public void SendPage(int page) {

    }

    @Override
    public Item[] addItem(Item... slots) {
        if (slots.length > 5 * 9) {
            ArrayList<Item> I = new ArrayList<>();
            for (int i = 0; i < 5 * 9; i++) {
                I.add(slots[i]);
            }
            CyberCoreMain.getInstance().getLogger().error("ERROR TRIED TO ADD " + slots.length);
            return this.addItem(I.toArray(new Item[45]));
        }
        return super.addItem(slots);
    }

    @Override
    public void sendContents(Collection<Player> players) {
        this.sendContents(players.stream().toArray(Player[]::new));
    }

    @Override
    public void sendSlot(int index, Player player) {
        this.sendSlot(index, new Player[]{player});
    }

    @Override
    public InventoryType getType() {
        return InventoryType.DOUBLE_CHEST;
    }

    public void showFoodCategory() {

//        clearAll();
//        setCurrentPage(CurrentPageEnum.Food_Catagory);
        setCatg(ShopCategory.Food);
        setPage(1);

    }

    public void AdminModeItem(int slot, boolean admin) {
        clearAll();
        ShopMysqlData aid = SF.getItemFrom(Page, slot, admin);
//        close(getHolder());
//        SetupFormToConfirmItem(aid);
        SetupPageToAdminEdit(aid);
        ReloadInv();
        ConfirmPurchase = true;
        ConfirmPurchaseSlot = slot;

        sendContents((Player) holder);
    }

    private void SetupPageToAdminEdit(ShopMysqlData aid) {
        CurrentPage = CurrentPageEnum.AdminItemEdit;
        AHStaticItems si = new AHStaticItems(Page);
        CorePlayer cp = (CorePlayer) getHolder();
        Item item = aid.getItem();
        MultiConfirmData = aid;
        Collection<Item> ai = cp.getInventory().all(aid.getItem(true)).values();
        int ic = 0;
        for (Item iii : ai) {
            ic += iii.getCount();
        }
        Item bi = new ItemBlock(new BlockBedrock());
        bi.setCustomName(" ");
        for (int i = 0; i < 5; i++) {
            for (int ii = 0; ii < 9; ii++) {
                int key = (i * 9) + ii;
                Item add = null;
                if (i == 0 || i == 4 || ii ==0|| ii ==8) {
                    add = bi;
                }else if(i == 1 && ii == 4){
                    add = item.clone();
                } else if(i == 3 ){
                    if(ii == 3) {
                        Item a = Item.get(Item.PAPER);
                        a.setCustomName("Change Sell Item Price");
                        add = a;
                    }else if(ii ==4){
                        Item a = Item.get(Item.PAPER);
                        a.setCustomName("Change Buy Item Price");
                        add = a;
                    }else if(ii ==5){

                        Item a = si.Redglass.clone();
                        a.setCustomName("Enable Item");
                        if(aid.isEnabled()){
                            a = si.Greenglass.clone();
                            a.setCustomName("Disable Item");
                        }
                        add = a;
                    }else if(ii ==6){
                        Item a = si.CatagoryChest.clone();
                        a.setCustomName("Change Category");
                        add = a;
                    }else{
                        add = bi;
                    }
                } else{
                    add = bi;
                }
                setItem(key, add, true);

            }
        }
    }

//    @Override
//    public void sendContents(Player player) {
//        this.sendContents(new Player[]{player});
//    }


    public enum CurrentPageEnum {
        ItemPage,
        PlayerSellingPage,
        Expired,
        Confirm_Purchase,
        Confirm_Purchase_Not_Enough_Money, Confirm_Purchase_Final, Catagories, Food_Catagory, AdminItemEdit,

    }

    }
}