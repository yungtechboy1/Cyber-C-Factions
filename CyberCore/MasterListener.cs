using System;
using System.Collections.Generic;
using System.Numerics;
using System.Reflection.Metadata.Ecma335;
using CyberCore.Manager.Factions;
using CyberCore.Manager.Factions.Windows;
using CyberCore.Utils;
using MiNET;
using MiNET.Blocks;
using MiNET.Entities;
using MiNET.Items;
using MiNET.Net;
using MiNET.Utils;
using OpenAPI.Events;
using OpenAPI.Events.Block;
using OpenAPI.Events.Entity;
using OpenAPI.Events.Player;

namespace CyberCore
{
    public class MasterListener : IEventHandler
    {
        public static CyberCoreMain plugin = CyberCoreMain.GetInstance();

        public static void joinEvent(object o, PlayerEventArgs eventArgs)
        {
            Player p = (Player) o;
            if (p == null)
            {
                CyberCoreMain.Log.Error("Error With Join Event! Object is not player");
                return;
            }

            bool isnew = CyberUtils.hasExtraPlayerData(p);
            var epd = p.GetExtraPlayerData();
            if (epd == null)
            {
                CyberCoreMain.Log.Error($"Extra Player Data for {p.getName()} is NULL?!?!?!");
                return;
            }

            epd.PlayerDetailedInfo.onLogin(p);
            p.SendForm(new HTP_0_Window()); //TODO
            p.Level.BroadcastMessage(ChatColors.Aqua + "Welcome " + p.getName() +
                                     " to the community!!! They have logged in for the 1st time!");


            String Msg = CyberUtils.colorize((String) plugin.MasterConfig.GetProperty("Join-Message",
                "{0} has joined the server. Default message"));
            String fm = (Msg.Replace("{player}", p.getName()));
            p.Level.BroadcastMessage(fm);
            p.SendTitle(CyberUtils.colorize("&l&bCyberTech")+"\n"+CyberUtils.colorize("&l&2Welcome!"),TitleType.Title, 30, 30, 10);
            
//        _plugin.initiatePlayer(p);
            plugin.ServerSQL.LoadPlayer(p);
            String rank = plugin.RF.getPlayerRank(p).getDisplayName();
            p.SendMessage(CyberUtils.colorize("&2You Have Joined with the Rank: " + rank));
            if (rank != null && rank.equalsIgnoreCase("op"))
            {
                p.PermissionLevel = PermissionLevel.Operator;
            }
            else
            {
                p.PermissionLevel = PermissionLevel.Visitor;
            }

//        Scoreboard s = ScoreboardAPI.createScoreboard();
//        ScoreboardDisplay sd = s.addDisplay(DisplaySlot.SIDEBAR,"Dummy","TESTTTT");
//            sd.addLine("TEST LINE 0",0);
//            sd.addLine("TEST LINE 1",1);
//            sd.addLine("YOUR NAME"+p.getDisplayName(),2);
//            ScoreboardAPI.setScoreboard(p,s);
        }

        [EventHandler(EventPriority.Highest)]
        public void InteractEvent(PlayerInteractEvent e)
        {
            String n = e.Player.getName();
            Item b = e.Item;
            if (b.Id == new Chest().Id)
            {
                CrateObject x = CyberCoreMain.GetInstance().CrateMain.isCrate(b);
                if (x != null)
                {
                    e.setCancelled();
                    if (plugin.CrateMain.PrimedPlayer.remove(n))
                    {
                        if (plugin.CrateMain.SetKeyPrimedPlayer.remove(n))
                        {
                            CrateData cd = x.CD;
                            Item hand = e.getPlayer().getInventory().getItemInHand();
                            String ki = plugin.CrateMain.getKeyIDFromKey(hand);
                            if (ki != null) cd.KeyItems.add(ki);
                            x.CD = cd;
                            plugin.CrateMain.CrateChests.put(x.Location.asBlockVector3().asVector3(), x);
                            CrateConfirmWindow s = new CrateConfirmWindow(FormType.MainForm.Crate_Confirm_Key_Assign,
                                "Crate - Keep on Adding KEY ASSSING? ");
                            s.addButton(new ElementButton("Keep adding Key Items"));
                            s.addButton(new ElementButton("Stop Adding"));
//                        s.on
                            e.getPlayer().showFormWindow(s);
                        }
                        else if (plugin.CrateMain.RemoveCrate.remove(n))
                        {
                            e.setCancelled(false);
                            return;
                        }
                        else if (plugin.CrateMain.ViewCrateItems.remove(n))
                        {
                            e.getPlayer().showFormWindow(new AdminCrateViewItemsWindow(x));
                        }
                        else if (plugin.CrateMain.SetCrateItemPrimedPlayer.remove(n))
                        {
                            CrateData cd = x.CD;
                            Item hand = e.getPlayer().getInventory().getItemInHand();
                            cd.PossibleItems.add(new ItemChanceData(hand, 100, hand.getCount()));
                            CrateConfirmWindow s = new CrateConfirmWindow(FormType.MainForm.Crate_Confirm_Add,
                                "Crate - Keep on Adding? ");
                            s.addButton(new ElementButton("Keep adding items"));
                            s.addButton(new ElementButton("Stop Adding"));
//                        s.on
                            e.getPlayer().showFormWindow(s);
                        }
                        else
                        {
                            plugin.CrateMain.addCrate((CorePlayer) e.getPlayer(), b);
                            CrateConfirmWindow s = new CrateConfirmWindow(FormType.MainForm.Crate_Confirm_ADD_Crate,
                                "Crate - Keep on Adding Crates? ");
                            s.addButton(new ElementButton("Keep adding Crates"));
                            s.addButton(new ElementButton("Stop Adding"));
//                        s.on
                            e.getPlayer().showFormWindow(s);
                        }
                    }
                    else
                    {
                        //Check Key
                        Item hand = e.getPlayer().getInventory().getItemInHand();
                        e.setCancelled();
                        if (!CrateMain.isItemKey(hand))
                        {
                            e.getPlayer().sendMessage("Error! Item is not a valid Crate Key!");
                            return;
                        }

                        if (x.checkKey(hand))
                        {
                            //Valid Key & Take it
                            PlayerInventory pi = e.getPlayer().getInventory();
                            Item i = pi.getItemInHand();
                            i.count--;
                            if (i.count == 0) i = Item.get(0);
                            pi.setItemInHand(i);
                            CyberCoreMain.getInstance().CrateMain.showCrate(b, e.getPlayer());
                            CyberCoreMain.getInstance().CrateMain.rollCrate(b, e.getPlayer());
                        }
                        else
                            e.getPlayer().sendMessage("Error! Key was invalid!");
                    }
                }
            }
        }

        @EventHandler()

        public void BlockBreakEvent(BlockBreakEvent e)
        {
            if (e.getBlock().getId() == Block.CHEST)
            {
                for (Vector3 v :
                plugin.CrateMain.CrateChests.keySet()) {
                    if (2 > e.getBlock().getLocation().asVector3f().asVector3().distance(v))
                    {
                        e.setCancelled();
                        e.getPlayer().sendMessage("error! CHEST BLOCKING!");
                    }
                }
            }
        }

        @EventHandler(priority = EventPriority.HIGHEST)
        public void PlayerDropItemEvent(PlayerDropItemEvent event) {
            CorePlayer cp = (CorePlayer) event.getPlayer();

            if (event.getItem().hasCompoundTag()) {
                if (event.getItem().getNamedTag().contains(getPowerHotBarItemNamedTagKey)) {
                    event.setCancelled();
                }
            }
        }

        @EventHandler(priority = EventPriority.HIGHEST)
        public void InventoryClickEvent(InventoryClickEvent event) {
            CyberCoreMain.Log.Error("Was LOG ||"+"CLICKKKKK EVENTTTTTTTTTTTT >>>> " + event);
            Player p = event.getPlayer();
            CorePlayer cp = (CorePlayer) p;
            if (cp.getPlayerClass() == null) return;
            for (PowerAbstract pp :
            cp.getPlayerClass().getActivePowers()) {
                if (pp.getPowerSettings().isHotbar())
                {
                    event = (InventoryClickEvent) pp.handelEvent(event);
                }
            }
            if (event.isCancelled()) CyberCoreMain.Log.Error("Was LOG ||"+"CANNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNN");
        }

        //No need RN
//    @EventHandler(priority = EventPriority.HIGHEST)
//    public void InventoryTransactionEvent(InventoryTransactionEvent event) {
//        Player p = event.getTransaction().getSource();
//        CorePlayer cp = (CorePlayer) p;
//        if (cp.getPlayerClass() == null) return;
//        for (PowerAbstract pp : cp.getPlayerClass().getActivePowers()) {
//            if (pp.getPowerSettings().isHotbar()) {
//                pp.handelEvent(event);
//            }
//        }
//    }

        @EventHandler(ignoreCancelled = true)
        public void TEE(InventoryTransactionEvent event) {
//        event.setCancelled();
            CyberCoreMain.Log.Error("Was LOG ||"+"CALLLLzzzzzzzzAAAAASSDDb");
//        event.getTransaction().
            InventoryTransaction transaction = event.getTransaction();
            Set<InventoryAction> traa = transaction.getActions();
            for (InventoryAction t :
            traa) {
                if (t instanceof SlotChangeAction) {
                    SlotChangeAction sca = (SlotChangeAction) t;
                    Inventory inv = sca.getInventory();
                    if (inv instanceof PlayerInventory) {
                        if (sca.getSourceItem().hasCompoundTag() &&
                            sca.getSourceItem().getNamedTag().contains(ShopInv.StaticItems.KeyName))
                            event.setCancelled();
                    } else if (inv instanceof PlayerCursorInventory) {
                        if (sca.getSourceItem().hasCompoundTag() &&
                            sca.getSourceItem().getNamedTag().contains(ShopInv.StaticItems.KeyName))
                            event.setCancelled();
                    }
                }
            }
        }

        @EventHandler(ignoreCancelled = true)
        public void TE(InventoryTransactionEvent event) {
//        event.setCancelled();
            CyberCoreMain.Log.Error("Was LOG ||"+"CALLLL");
//        event.getTransaction().
            InventoryTransaction transaction = event.getTransaction();
            Set<InventoryAction> traa = transaction.getActions();
            boolean s = true;
            for (Inventory i :
            transaction.getInventories()) {
                if (i instanceof ShopInv) s = false;
                if (i instanceof AuctionHouse) s = false;
                if (i instanceof SpawnerShop) s = false;
            }
            if (s) return;
            for (InventoryAction t :
            traa) {
                CyberCoreMain.Log.Error("Was LOG ||"+"CALLLL TTTTTTTTTTTTTTTTTTT" + t.getClass().getName());
                if (t instanceof SlotChangeAction) {
//                CyberCoreMain.Log.Error("Was LOG ||"+"CALLLL SLOTCCCCCCCC");
                    SlotChangeAction sca = (SlotChangeAction) t;
//                sca.getInventory()

                    Inventory inv = sca.getInventory();
//                CyberCoreMain.Log.Error("Was LOG ||"+"CHECK INNNNNVVVVVVV " + inv.getClass().getName());
//                if (inv.isEmpty()) return;

//                CyberCoreMain.Log.Error("Was LOG ||"+"NEEEEEEE" + inv.getClass().getTypeName());
                    if (inv instanceof PlayerInventory) {
                        CyberCoreMain.Log.Error("Was LOG ||"+"CHECK INNNNNVVVVVVV " + sca);
//                event.setCancelled();
                    } else if (inv instanceof PlayerCursorInventory) {
                        event.setCancelled();
                        transaction.getSource().getCursorInventory().clearAll();
                        transaction.getSource().sendAllInventories();
                        CyberCoreMain.Log.Error("Was LOG ||"+"+++++>" + transaction.getSource().getCursorInventory());
                        CyberCoreMain.Log.Error("Was LOG ||"+"+++++>" + transaction.getSource().getCursorInventory().slots);
                    }
                    if (inv instanceof ShopInv) {
                        ShopInvMainHandle(inv, sca,  event);
                    }
                    if (inv instanceof AuctionHouse) {
                        AuctionHouse ah = (AuctionHouse) inv;
//                    if(!ah.Init)return;
                        CyberCoreMain.Log.Error("Was LOG ||"+sca.getSlot() + " || " + ah.getHolder().getName() + " || " +
                                           ah.getHolder().getClass().getName());
                        CorePlayer ccpp = (CorePlayer) ah.getHolder();
                        int slot = sca.getSlot();
//                    event.setCancelled();
//                    event.setCancelled();
                        if (slot < 5 * 9)
                        {
                            CyberCoreMain.Log.Error("Was LOG ||"+"TOP INV");
                            //TODO CONFIRM AND SHOW ITEM
                            if (!ah.ConfirmPurchase)
                            {
                                ah.ConfirmItemPurchase(slot);
                                CyberCoreMain.Log.Error("Was LOG ||"+"SSSSSSSSSSSSCPPPPPPPP");
//                        ccpp.AH.ConfirmItemPurchase(slot);
                            }
                            else
                            {
                                Item si = ah.getContents().get(slot);
                                if (si != null)
                                {
                                    if (ah.getCurrentPage() == Confirm_Purchase_Not_Enough_Money)
                                    {
                                        ah.setPage(1);
                                        ah.ClearConfirmPurchase();
                                        //Back Home
                                        break;
                                    }
                                    else
                                    {
                                        CyberCoreMain.Log.Error("Was LOG ||"+"CPPPPPPPP");

                                        if (si.getId() == BlockID.EMERALD_BLOCK)
                                        {
                                            CyberCoreMain.Log.Error("Was LOG ||"+"CONFIRM PURCHASE!!!!!!!");
                                            ah.AF.PurchaseItem((CorePlayer) ah.getHolder(), ah.getPage(),
                                                ah.ConfirmPurchaseSlot);
                                            break;
                                        }
                                        else if (si.getId() == BlockID.RedSTONE_BLOCK)
                                        {
                                            CyberCoreMain.Log.Error("Was LOG ||"+"DENCLINE PURCHASE!!!!!!!!");
                                            ah.setPage(1);
                                            ah.ClearConfirmPurchase();
                                            break;
                                        }
                                        else
                                        {
                                            ah.setPage(1);
                                            CyberCoreMain.Log.Error("Was LOG ||"+"UNKNOWNMNNN!!!!!!!!");
                                            ah.ClearConfirmPurchase();
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            switch (slot)
                            {
                                case AuctionHouse.MainPageItemRef.LastPage:
                                    ah.GoToPrevPage();
                                    break;
                                case AuctionHouse.MainPageItemRef.NextPage:
                                    ah.GoToNextPage();
                                    break;
                                case AuctionHouse.MainPageItemRef.Search:
                                    break;
                                case AuctionHouse.MainPageItemRef.Reload:
                                    ah.ReloadCurrentPage();
                                    break;
                                case AuctionHouse.MainPageItemRef.Catagories:
                                    ah.DisplayCatagories();
                                    break;
                                case AuctionHouse.MainPageItemRef.PlayerSelling:
                                    ah.GoToSellerPage();
                                        event.setCancelled(false);
                                    break;
                            }
                        }
                    }
                    if (inv instanceof SpawnerShop) {
                        SpawnerShop ah = (SpawnerShop) inv;
//                    if(!ah.Init)return;
                        CyberCoreMain.Log.Error("Was LOG ||"+sca.getSlot() + " || " + ah.getHolder().getName() + " || " +
                                           ah.getHolder().getClass().getName());
                        CorePlayer ccpp = (CorePlayer) ah.getHolder();
                        int slot = sca.getSlot();
//                    event.setCancelled();
//                    event.setCancelled();
                        if (slot < 5 * 9)
                        {
                            CyberCoreMain.Log.Error("Was LOG ||"+"TOP INV");
                            //TODO CONFIRM AND SHOW ITEM
                            if (!ah.ConfirmPurchase)
                            {
                                ah.ConfirmItemPurchase(slot);
//                        ccpp.AH.ConfirmItemPurchase(slot);
                            }
                            else
                            {
                                if (ah.CurrentPage == SpawnerShop.CurrentPageEnum.PlayerSellingPage)
                                {
                                    int sx = slot % 9;
                                    int sy = (int) Math.floor(slot / 9);
                                    Item is  = ah.getItem(slot);
                                    boolean isi = false;
                                    int isc = is.getCount();
                                    if (is != null && is.getId() != 0) {
                                        if (is.getId() == Item.IRON_BLOCK) isi = true;
                                        CyberCoreMain.Log.Error("Was LOG ||"+"Selected Slot SX:" + sx + " | SY:" + sy);
                                        if (sy != 0 && sy != 5 && sx != 4 && !isi)
                                        {
                                            if (sx < 4)
                                            {
                                                //Cancel
                                                ah.setPage(1);
                                            }
                                            else
                                            {
                                                //Buy
                                                ah.SetupPageToFinalConfirmItem(ah.MultiConfirmData, isc, false);
                                            }
                                        }
                                    }
                                    event.setCancelled();
                                    return;
                                }
                                else
                                {
                                    Item si = ah.getContents().get(slot);
                                    if (si != null)
                                    {
                                        if (ah.getCurrentPage() ==
                                            SpawnerShop.CurrentPageEnum.Confirm_Purchase_Not_Enough_Money)
                                        {
                                            ah.setPage(1);
                                            ah.ClearConfirmPurchase();
                                            //Back Home
                                            break;
                                        }
                                        else
                                        {
                                            CyberCoreMain.Log.Error("Was LOG ||"+"CPPPPPPPP");

                                            if (si.getId() == BlockID.EMERALD_BLOCK)
                                            {
                                                CyberCoreMain.Log.Error("Was LOG ||"+"CONFIRM PURCHASE!!!!!!!");
                                                ah.SSF.PurchaseItem((CorePlayer) ah.getHolder(), ah.getPage(),
                                                    ah.ConfirmPurchaseSlot, si.getCount());
                                                break;
                                            }
                                            else if (si.getId() == BlockID.RedSTONE_BLOCK)
                                            {
                                                CyberCoreMain.Log.Error("Was LOG ||"+"DENCLINE PURCHASE!!!!!!!!");
                                                ah.setPage(1);
                                                ah.ClearConfirmPurchase();
                                                break;
                                            }
                                            else
                                            {
                                                ah.setPage(1);
                                                CyberCoreMain.Log.Error("Was LOG ||"+"UNKNOWNMNNN!!!!!!!!");
                                                ah.ClearConfirmPurchase();
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            switch (slot)
                            {
                                case SpawnerShop.MainPageItemRef.LastPage:
                                    ah.GoToPrevPage();
                                    break;
                                case SpawnerShop.MainPageItemRef.NextPage:
                                    ah.GoToNextPage();
                                    break;
                                case SpawnerShop.MainPageItemRef.Search:
                                    break;
                                case SpawnerShop.MainPageItemRef.Reload:
                                    ah.ReloadCurrentPage();
                                    break;
                                case SpawnerShop.MainPageItemRef.Catagories:
                                    ah.DisplayCatagories();
                                    break;
                                case SpawnerShop.MainPageItemRef.ToggleAdmin:
                                    ah.AdminMode = !ah.AdminMode;
                                        event.setCancelled(false);
                                    ah.ReloadCurrentPage();
                                    break;
                            }
                        }
                    }
                }
            }

            event.setCancelled();
                event.getTransaction().getSource().sendAllInventories();
        }

        private void ShopInvMainHandle(Inventory inv, SlotChangeAction sca, InventoryTransactionEvent event) {
            event.setCancelled();
            CyberCoreMain.Log.Error("Was LOG ||"+"CHECK INNNNNVVV222222VVVV " + sca);
            ShopInv ah = (ShopInv) inv;
//                    if(!ah.Init)return;
            CyberCoreMain.Log.Error("Was LOG ||"+sca.getSlot() + " || " + ah.getHolder().getName() + " || " +
                               ah.getHolder().getClass().getName());
            CorePlayer ccpp = (CorePlayer) ah.getHolder();
            int slot = sca.getSlot();
            int sx = slot % 9;
            int sy = (int) Math.floor(slot / 9);
//                    event.setCancelled();
            if (slot < 5 * 9)
            {
                CyberCoreMain.Log.Error("Was LOG ||"+"TOP INV");
                //TODO CONFIRM AND SHOW ITEM
                if (!ah.ConfirmPurchase)
                {
                    Item is  = ah.getItem(slot);
                    if (ah.CurrentPage == ShopInv.CurrentPageEnum.Catagories)
                    {
                        ShopCatagoreyHandler(slot, ah);
                    }
                    else
                    {
                        if (!ah.AdminMode)
                            ah.ConfirmItemPurchase(slot, ah.AdminMode);
                        else
                        {
                            if (ah.CurrentPage == AdminItemEdit)
                            {
                                AdminItemEditHandle(ah, inv, sca,  event,slot);
                            }
                            else
                                ah.AdminModeItem(slot, ah.AdminMode);
                        }
                    }

//                        ccpp.AH.ConfirmItemPurchase(slot);
                }
                else
                {
                    Item is  = ah.getItem(slot);
                    if (ah.CurrentPage == ShopInv.CurrentPageEnum.Catagories)
                    {
                        if (slot == 11)
                        {
                            ah.CCM.SpawnShop.OpenShop((CorePlayer) ah.getHolder(), 1);
                        }
                        else
                        {
                            ah.setPage(1);
                        }
                    }
                    else if (ah.CurrentPage == ShopInv.CurrentPageEnum.PlayerSellingPage)
                    {
                        boolean isi = false;
                        int isc = is.getCount();
                        if (is != null && is.getId() != 0) {
                            if (is.getId() == Item.IRON_BLOCK) isi = true;
                            CyberCoreMain.Log.Error("Was LOG ||"+"Selected Slot SX:" + sx + " | SY:" + sy);
                            if (sy != 0 && sy != 5 && sx != 4 && !isi)
                            {
                                if (sx < 4)
                                {
                                    //Sell
                                    ah.SetupPageToFinalConfirmItem(ah.MultiConfirmData, isc, true);
                                }
                                else
                                {
                                    //Buy
                                    ah.SetupPageToFinalConfirmItem(ah.MultiConfirmData, isc, false);
                                }
                            }
                        }
                        return;
                    }
                    else
                    {
                        Item si = ah.getContents().get(slot);
                        if (si != null)
                        {
                            if (ah.getCurrentPage() == ShopInv.CurrentPageEnum.Confirm_Purchase_Not_Enough_Money)
                            {
                                ah.setPage(1);
                                ah.ClearConfirmPurchase();
                                //Back Home
                                return;
                            }
                            else
                            {
                                CyberCoreMain.Log.Error("Was LOG ||"+"CPPPPPPPP");

                                if (si.getId() == BlockID.EMERALD_BLOCK)
                                {
                                    CyberCoreMain.Log.Error("Was LOG ||"+"CONFIRM PURCHASE!!!!!!!");
                                    ah.SF.PurchaseItem((CorePlayer) ah.getHolder(), ah.getPage(),
                                        ah.ConfirmPurchaseSlot, si.getCount(), ah.AdminMode);
                                    return;
                                }
                                else if (si.getId() == BlockID.RedSTONE_BLOCK)
                                {
                                    CyberCoreMain.Log.Error("Was LOG ||"+"DENCLINE PURCHASE!!!!!!!!");
                                    ah.setPage(1);
                                    ah.ClearConfirmPurchase();
                                    return;
                                }
                                else
                                {
                                    ah.setPage(1);
                                    CyberCoreMain.Log.Error("Was LOG ||"+"UNKNOWNMNNN!!!!!!!!");
                                    ah.ClearConfirmPurchase();
                                    return;
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                switch (slot)
                {
                    case ShopInv.MainPageItemRef.LastPage:
                        ah.GoToPrevPage();
                        break;
                    case ShopInv.MainPageItemRef.NextPage:
                        ah.GoToNextPage();
                        break;
                    case ShopInv.MainPageItemRef.Search:
                        break;
                    case ShopInv.MainPageItemRef.Reload:
                        ah.ReloadCurrentPage();
                        break;
                    case ShopInv.MainPageItemRef.Catagories:
                        ah.DisplayCatagories();
                        break;
                    case ShopInv.MainPageItemRef.ToggleAdmin:
                        ah.AdminMode = !ah.AdminMode;
                            event.setCancelled(false);
                        ah.ReloadCurrentPage();
                        break;
                }
            }
            event.setCancelled();
        }

        private void
            AdminItemEditHandle(ShopInv ah, Inventory inv, SlotChangeAction sca, InventoryTransactionEvent event, int
            slot) {
            int sx = slot % 9;
            int sy = (int) Math.floor(slot / 9);
            Item i = sca.getInventory().getItem(sca.getSlot());
            if (sx == 3 && sy == 3)
            {
                if (i.hasCompoundTag() && i.getNamedTag().contains("ShopID"))
                {
                    int s = i.getNamedTag().getInt("ShopID");
                    ShopMysqlData sd = ah.CCM.Shop.getItemFrom(s);
                    if (sd == null)
                    {
                        CyberCoreMain.Log.Error("Was LOG ||"+"Error!!!!! WTF!!!1221aas222e2aaqqqwd  ass");
                    }
                    else
                    {
                        //New Edit Window
                    }
                }
            }
        }

        private void ShopCatagoreyHandler(int slot, ShopInv ah)
        {
            if (slot == 11)
            {
                ah.CCM.SpawnShop.OpenShop((CorePlayer) ah.getHolder(), 1);
            }
            else if (slot == 12)
            {
                //Food
                ah.showFoodCategory();
            }
            else
            {
                ah.setPage(1);
            }
        }

        @EventHandler(priority = EventPriority.HIGHEST)
        public void spawnEvent(PlayerRespawnEvent event) {
        }

        @EventHandler(priority = EventPriority.HIGHEST)
        public void TD(PlayerTakeDamageEvent event) {
            Entity e = event.source.entity;
            if (e instanceof CorePlayer) {
                CorePlayer cp = (CorePlayer) e;
                if (cp.getPlayerClass() != null)
                {
                    cp.getPlayerClass().HandelEvent(event);
                }
            }
        }

        @EventHandler(priority = EventPriority.HIGHEST)
        public void EICE(EntityInventoryChangeEvent event) {
            if (event == null) {
                CyberCoreMain.Log.Error("Was LOG ||"+"WTF NUUUUUUUUUUUUUUUUUUULLLLLLLLLLLLLLLLLLLLL");
                return;
            }
            Entity e = event.getEntity();
            if (e instanceof CorePlayer) {
                CorePlayer cp = (CorePlayer) e;
                if (cp.getPlayerClass() != null)
                {
                    cp.getPlayerClass().HandelEvent(event);
                }
            }
        }

        @EventHandler(priority = EventPriority.HIGHEST)
        public void EntityDamageEvent(EntityDamageEvent event) {
            Entity e = event.getEntity();
            if (e instanceof CorePlayer) {
                CorePlayer cp = (CorePlayer) e;
                if (cp.getPlayerClass() != null)
                {
                    cp.getPlayerClass().HandelEvent(event);
                }
            }
        }


//    @EventHandler(priority = EventPriority.HIGHEST)
//    public void onCreation(PlayerCreationEvent event) {
//        event.setPlayerClass(CorePlayer.class);
//        event.setBaseClass(CorePlayer.class);
//    }


        @EventHandler(priority = EventPriority.HIGHEST)
        public void onCreation(PlayerCreationEvent event) {
            event.setPlayerClass(CorePlayer.class);
        }

        @EventHandler(priority = EventPriority.LOWEST)
        public void quitEvent(PlayerQuitEvent event) {
            String Msg = (String) plugin.MainConfig.get("Leave-Message");
                event.setQuitMessage(Msg.replace("{player}",  event.getPlayer().getName()));
            Player p = event.getPlayer();
            if (p instanceof CorePlayer) {
                plugin.ClassFactory.save((CorePlayer) p);
            }
        }


        //@TODO Check for BadWords!
        @EventHandler(priority = EventPriority.HIGHEST)
        public void onChatEvent(PlayerChatEvent event) {
            if (event.isCancelled()) return;
                //SHouldnt need thins @TODO^^
                event.setCancelled(true);
            if (plugin.MuteChat && (!event.getPlayer().hasPermission("CyberTech.CyberChat.op"))) {
                event.getPlayer().sendMessage(ChatColors.Yellow + "All Chat Is Muted! Try again later!");
                return;
            }
            if (plugin.isMuted(event.getPlayer())) {
                event.getPlayer().sendMessage(ChatColors.Yellow + "You are Muted! Try again later!");
                return;
            }
            String FinalChat = formatForChat(event.getPlayer(), event.getMessage());
            if (FinalChat == null) return;
            if (plugin.LM.containsKey(event.getPlayer().getName().toLowerCase()) && plugin.LM.get(event.getPlayer()
                .getName().toLowerCase()).equalsIgnoreCase(FinalChat)) {
                event.getPlayer().sendMessage(FinalChat);
                return;
            }
            plugin.LM.put(event.getPlayer().getName().toLowerCase(), FinalChat);
            plugin.checkSpam(event.getPlayer());
            Server.getInstance().getLogger().info(FinalChat);
            for (Map.Entry < UUID, Player > e : plugin.getServer().getOnlinePlayers().entrySet())
            {
                if (plugin.PlayerMuted.contains(e.getValue().getName().toLowerCase())) continue;
                e.getValue().sendMessage(FinalChat);
            }
        }

        public String formatForChat(Player player, String chat)
        {
            Dictionary<String, Object> badwords = new Dictionary<String, Object>()
            {
                {
                    put("fuck", "f***");
                    put("shit", "s***");
                    put("nigger", "kitty");
                    put("nigga", "boi");
                    put("bitch", "Sweetheart");
                    put("hoe", "tool");
                    put("ass", "butt");
                }
            };
            String faction;
            Faction pf = plugin.getPlayerFaction(player);
            if (pf != null)
            {
                faction = pf.getDisplayName();
                //FactionFormat = ChatColors.Gray+FactionFormat.replace("{value}",fp.getFaction().getTag())+ChatColors.WHITE;
            }
            else
            {
                faction = ChatColors.Gray + "[NF]" + ChatColors.WHITE;
            }

            //ANTI BADWORDS
            String chatb4 = chat;
            String chatafter = chat;
            for (String s :
            chat.split(" ")) {
                if (!badwords.containsKey(s.toLowerCase())) continue;
                chatafter = chatafter.replaceAll("(?i)" + s, badwords.get(s.toLowerCase()).toString());
            }
            /*
            Fucks up words like Class and BAss
            for(String b: badwords.keySet()){
                if(chatafter.toLowerCase().contains(b.toLowerCase())) {
                    chatafter = chatafter.replaceAll("(?i)" + b, badwords.get(b).toString());
                    chatafter = chatafter.replaceAll(b, badwords.get(b).toString());
                }
            }*/
            /*String chat2 = chat;
            chat2 = chat.replace(" ","");
             */
            //ANTI WORK AROUND BADWORDS
            //@TODO remove all spaces and use Regex to replace all Instaces of it

            return plugin.RF.getPlayerRank(player).getChat_format().format(faction,
                plugin.RF.getPlayerRank(player).getDisplayName(), player, chatafter);
/*
        put("Chat-Format", "{rank}{faction}{player-name} > {msg}");
        put("Faction-Format", "[{value}]");
        put("Rank-Format", "[{value}]");
        put("Join-Message", "");
        put("Leave-Message", "");*/
        }
    }