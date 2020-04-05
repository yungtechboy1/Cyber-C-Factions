using CyberCore.Manager.Crate;
using CyberCore.Manager.Crate.Form;
using CyberCore.Manager.Factions.Windows;
using CyberCore.Manager.Forms;
using CyberCore.Utils;
using MiNET;
using MiNET.Blocks;
using MiNET.Items;
using MiNET.Net;
using MiNET.Utils;
using OpenAPI.Events;
using OpenAPI.Events.Block;
using OpenAPI.Events.Player;
using static CyberCore.Manager.ClassFactory.Powers.PowerHotBarInt;

namespace CyberCore
{
    public class MasterListener : IEventHandler
    {
        public static CyberCoreMain plugin = CyberCoreMain.GetInstance();

        [EventHandler]
        public void OnPlayerJoin(PlayerJoinEvent e)
        {
            Player p = e.Player;
            if (p == null)
            {
                CyberCoreMain.Log.Error("Error With Join Event! Object is not player");
                return;
            }

            var isnew = CyberUtils.hasExtraPlayerData(p);
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


            var Msg = CyberUtils.colorize(plugin.MasterConfig.GetProperty("Join-Message",
                "{0} has joined the server. Default message"));
            var fm = Msg.Replace("{player}", p.getName());
            p.Level.BroadcastMessage(fm);
            p.SendTitle(CyberUtils.colorize("&l&bCyberTech") + "\n" + CyberUtils.colorize("&l&2Welcome!"),
                TitleType.Title, 30, 30, 10);

//        _plugin.initiatePlayer(p);
            plugin.ServerSQL.LoadPlayer((CorePlayer) p);
            var rank = plugin.RF.getPlayerRank(p).getDisplayName();
            p.SendMessage(CyberUtils.colorize("&2You Have Joined with the Rank: " + rank));
            if (rank != null && rank.equalsIgnoreCase("op"))
                p.PermissionLevel = PermissionLevel.Operator;
            else
                p.PermissionLevel = PermissionLevel.Visitor;

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
            var n = e.Player.getName();
            var b = e.Player.Level.GetBlock(e.Coordinates);
            if (b.Id == new Chest().Id)
            {
                var x = CyberCoreMain.GetInstance().CrateMain.isCrate(b);
                if (x != null)
                {
                    e.SetCancelled(true);
                    if (plugin.CrateMain.PrimedPlayer.Remove(n))
                    {
                        if (plugin.CrateMain.SetKeyPrimedPlayer.Remove(n))
                        {
                            var cd = x.CD;
                            var hand = e.Player.Inventory.GetItemInHand();
                            var ki = plugin.CrateMain.getKeyIDFromKey(hand);
                            if (ki != null) cd.KeyItems.Add(ki);
                            x.CD = cd;
                            plugin.CrateMain.CrateChests[x.Location] = x;
                            var s = new CrateConfirmWindow(MainForm.Crate_Confirm_Key_Assign,
                                "Crate - Keep on Adding KEY ASSSING? ");
//                        s.on
                            e.Player.SendForm(s);
                        }
                        else if (plugin.CrateMain.RemoveCrate.Remove(n))
                        {
                            e.SetCancelled(false);
                        }
                        else if (plugin.CrateMain.ViewCrateItems.Remove(n))
                        {
                            e.Player.showFormWindow(new AdminCrateViewItemsWindow(x));
                        }
                        else if (plugin.CrateMain.SetCrateItemPrimedPlayer.Remove(n))
                        {
                            var cd = x.CD;
                            var hand = e.Player.Inventory.GetItemInHand();
                            cd.PossibleItems.Add(new ItemChanceData(hand, 100, hand.Count));
                            var s = new CrateConfirmWindow(MainForm.Crate_Confirm_Add,
                                "Crate - Keep on Adding? ");
                            e.Player.showFormWindow(s);
                        }
                        else
                        {
                            plugin.CrateMain.addCrate((CorePlayer) e.Player, b.Coordinates);
                            var s = new CrateConfirmWindow(MainForm.Crate_Confirm_ADD_Crate,
                                "Crate - Keep on Adding Crates? ");
//                        s.on
                            e.Player.showFormWindow(s);
                        }
                    }
                    else
                    {
                        //Check Key
                        var hand = e.Player.Inventory.GetItemInHand();
                        e.SetCancelled(true);
                        if (!CrateMain.isItemKey(hand))
                        {
                            e.Player.SendMessage("Error! Item is not a valid Crate Key!");
                            return;
                        }

                        if (x.checkKey(hand))
                        {
                            //Valid Key & Take it
                            var pi = e.Player.Inventory;
                            var i = pi.GetItemInHand();
                            i.Count--;
                            if (i.Count == 0) i = new ItemAir();
                            pi.setItemInHand(i);
                            CyberCoreMain.GetInstance().CrateMain.showCrate(b.Coordinates, e.Player);
                            CyberCoreMain.GetInstance().CrateMain.rollCrate(b.Coordinates, e.Player);
                        }
                        else
                        {
                            e.Player.SendMessage("Error! Key was invalid!");
                        }
                    }
                }
            }
        }

        [EventHandler]
        public void BlockBreakEvent(BlockBreakEvent e)
        {
            if (e.Block.Id == new Chest().Id)
                foreach (var v in plugin.CrateMain.CrateChests.Keys)
                    if (2 > e.Block.Coordinates.DistanceTo(v))
                    {
                        e.SetCancelled(true);
                        ((Player) e.Source)?.SendMessage("error! CHEST BLOCKING!");
                    }
        }


        [EventHandler]
        public void PlayerDropItemEvent(PlayerItemDropEvent eeee)
        {
            var cp = (CorePlayer) eeee.Player;

            if (eeee.DroppedItem.hasCompoundTag())
                if (eeee.DroppedItem.getNamedTag().Contains(getPowerHotBarItemNamedTagKey))
                    eeee.SetCancelled(true);
        }

        // [EventHandler]
        // public void InventoryClickEvent(InventoryClickEvent eeee)
        // {
        //     CyberCoreMain.Log.Error("Was LOG ||" + "CLICKKKKK EVENTTTTTTTTTTTT >>>> " + eeee);
        //     Player p = eeee.getPlayer();
        //     CorePlayer cp = (CorePlayer) p;
        //     if (cp.getPlayerClass() == null) return;
        //     for (PowerAbstract pp :
        //     cp.getPlayerClass().getActivePowers()) {
        //         if (pp.getPowerSettings().isHotbar())
        //         {
        //             eeee = (InventoryClickEvent) pp.handelEvent(event);
        //         }
        //     }
        //     if (event.isCancelled()) CyberCoreMain.Log.Error(
        //         "Was LOG ||" + "CANNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNN");
        // }

        //No need RN
//    @EventHandler(priority = EventPriority.HIGHEST)
//    public void InventoryTransactionEvent(InventoryTransactionEvent eeee) {
//        Player p = eeee.getTransaction().getSource();
//        CorePlayer cp = (CorePlayer) p;
//        if (cp.getPlayerClass() == null) return;
//        for (PowerAbstract pp : cp.getPlayerClass().getActivePowers()) {
//            if (pp.getPowerSettings().isHotbar()) {
//                pp.handelEvent(event);
//            }
//        }
//    }

//         @EventHandler(ignoreCancelled = true)
//
//         public void TEE(InventoryTransactionEvent eeee)
//         {
// //        eeee.SetCancelled();
//             CyberCoreMain.Log.Error("Was LOG ||" + "CALLLLzzzzzzzzAAAAASSDDb");
// //        eeee.getTransaction().
//             InventoryTransaction transaction = eeee.getTransaction();
//             Set<InventoryAction> traa = transaction.getActions();
//             for (InventoryAction t :
//             traa) {
//                 if (t instanceof SlotChangeAction) {
//                     SlotChangeAction sca = (SlotChangeAction) t;
//                     Inventory inv = sca.Inventory;
//                     if (inv instanceof PlayerInventory) {
//                         if (sca.getSourceItem().hasCompoundTag() &&
//                             sca.getSourceItem().getNamedTag().contains(ShopInv.StaticItems.KeyName))
//                             eeee.SetCancelled();
//                     } else if (inv instanceof PlayerCursorInventory) {
//                         if (sca.getSourceItem().hasCompoundTag() &&
//                             sca.getSourceItem().getNamedTag().contains(ShopInv.StaticItems.KeyName))
//                             eeee.SetCancelled();
//                     }
//                 }
//             }
//         }

//         @EventHandler(ignoreCancelled = true)
//
//         public void TE(InventoryTransactionEvent eeee)
//         {
// //        eeee.SetCancelled();
//             CyberCoreMain.Log.Error("Was LOG ||" + "CALLLL");
// //        eeee.getTransaction().
//             InventoryTransaction transaction = eeee.getTransaction();
//             Set<InventoryAction> traa = transaction.getActions();
//             boolean s = true;
//             for (Inventory i :
//             transaction.getInventories()) {
//                 if (i instanceof ShopInv) s = false;
//                 if (i instanceof AuctionHouse) s = false;
//                 if (i instanceof SpawnerShop) s = false;
//             }
//             if (s) return;
//             for (InventoryAction t :
//             traa) {
//                 CyberCoreMain.Log.Error("Was LOG ||" + "CALLLL TTTTTTTTTTTTTTTTTTT" + t.getClass().getName());
//                 if (t instanceof SlotChangeAction) {
// //                CyberCoreMain.Log.Error("Was LOG ||"+"CALLLL SLOTCCCCCCCC");
//                     SlotChangeAction sca = (SlotChangeAction) t;
// //                sca.Inventory
//
//                     Inventory inv = sca.Inventory;
// //                CyberCoreMain.Log.Error("Was LOG ||"+"CHECK INNNNNVVVVVVV " + inv.getClass().getName());
// //                if (inv.isEmpty()) return;
//
// //                CyberCoreMain.Log.Error("Was LOG ||"+"NEEEEEEE" + inv.getClass().getTypeName());
//                     if (inv instanceof PlayerInventory) {
//                         CyberCoreMain.Log.Error("Was LOG ||" + "CHECK INNNNNVVVVVVV " + sca);
// //                eeee.SetCancelled();
//                     } else if (inv instanceof PlayerCursorInventory) {
//                         eeee.SetCancelled();
//                         transaction.getSource().getCursorInventory().clearAll();
//                         transaction.getSource().sendAllInventories();
//                         CyberCoreMain.Log.Error("Was LOG ||" + "+++++>" + transaction.getSource().getCursorInventory());
//                         CyberCoreMain.Log.Error("Was LOG ||" + "+++++>" +
//                                                 transaction.getSource().getCursorInventory().slots);
//                     }
//                     if (inv instanceof ShopInv) {
//                         ShopInvMainHandle(inv, sca, eeee);
//                     }
//                     if (inv instanceof AuctionHouse) {
//                         AuctionHouse ah = (AuctionHouse) inv;
// //                    if(!ah.Init)return;
//                         CyberCoreMain.Log.Error("Was LOG ||" + sca.getSlot() + " || " + ah.getHolder().getName() +
//                                                 " || " +
//                                                 ah.getHolder().getClass().getName());
//                         CorePlayer ccpp = (CorePlayer) ah.getHolder();
//                         int slot = sca.getSlot();
// //                    eeee.SetCancelled();
// //                    eeee.SetCancelled();
//                         if (slot < 5 * 9)
//                         {
//                             CyberCoreMain.Log.Error("Was LOG ||" + "TOP INV");
//                             //TODO CONFIRM AND SHOW ITEM
//                             if (!ah.ConfirmPurchase)
//                             {
//                                 ah.ConfirmItemPurchase(slot);
//                                 CyberCoreMain.Log.Error("Was LOG ||" + "SSSSSSSSSSSSCPPPPPPPP");
// //                        ccpp.AH.ConfirmItemPurchase(slot);
//                             }
//                             else
//                             {
//                                 Item si = ah.getContents().get(slot);
//                                 if (si != null)
//                                 {
//                                     if (ah.getCurrentPage() == Confirm_Purchase_Not_Enough_Money)
//                                     {
//                                         ah.setPage(1);
//                                         ah.ClearConfirmPurchase();
//                                         //Back Home
//                                         break;
//                                     }
//                                     else
//                                     {
//                                         CyberCoreMain.Log.Error("Was LOG ||" + "CPPPPPPPP");
//
//                                         if (si.getId() == BlockID.EMERALD_BLOCK)
//                                         {
//                                             CyberCoreMain.Log.Error("Was LOG ||" + "CONFIRM PURCHASE!!!!!!!");
//                                             ah.AF.PurchaseItem((CorePlayer) ah.getHolder(), ah.getPage(),
//                                                 ah.ConfirmPurchaseSlot);
//                                             break;
//                                         }
//                                         else if (si.getId() == BlockID.RedSTONE_BLOCK)
//                                         {
//                                             CyberCoreMain.Log.Error("Was LOG ||" + "DENCLINE PURCHASE!!!!!!!!");
//                                             ah.setPage(1);
//                                             ah.ClearConfirmPurchase();
//                                             break;
//                                         }
//                                         else
//                                         {
//                                             ah.setPage(1);
//                                             CyberCoreMain.Log.Error("Was LOG ||" + "UNKNOWNMNNN!!!!!!!!");
//                                             ah.ClearConfirmPurchase();
//                                             break;
//                                         }
//                                     }
//                                 }
//                             }
//                         }
//                         else
//                         {
//                             switch (slot)
//                             {
//                                 case AuctionHouse.MainPageItemRef.LastPage:
//                                     ah.GoToPrevPage();
//                                     break;
//                                 case AuctionHouse.MainPageItemRef.NextPage:
//                                     ah.GoToNextPage();
//                                     break;
//                                 case AuctionHouse.MainPageItemRef.Search:
//                                     break;
//                                 case AuctionHouse.MainPageItemRef.Reload:
//                                     ah.ReloadCurrentPage();
//                                     break;
//                                 case AuctionHouse.MainPageItemRef.Catagories:
//                                     ah.DisplayCatagories();
//                                     break;
//                                 case AuctionHouse.MainPageItemRef.PlayerSelling:
//                                     ah.GoToSellerPage();
//                                     eeee.SetCancelled(false);
//                                     break;
//                             }
//                         }
//                     }
//                     if (inv instanceof SpawnerShop) {
//                         SpawnerShop ah = (SpawnerShop) inv;
// //                    if(!ah.Init)return;
//                         CyberCoreMain.Log.Error("Was LOG ||" + sca.getSlot() + " || " + ah.getHolder().getName() +
//                                                 " || " +
//                                                 ah.getHolder().getClass().getName());
//                         CorePlayer ccpp = (CorePlayer) ah.getHolder();
//                         int slot = sca.getSlot();
// //                    eeee.SetCancelled();
// //                    eeee.SetCancelled();
//                         if (slot < 5 * 9)
//                         {
//                             CyberCoreMain.Log.Error("Was LOG ||" + "TOP INV");
//                             //TODO CONFIRM AND SHOW ITEM
//                             if (!ah.ConfirmPurchase)
//                             {
//                                 ah.ConfirmItemPurchase(slot);
// //                        ccpp.AH.ConfirmItemPurchase(slot);
//                             }
//                             else
//                             {
//                                 if (ah.CurrentPage == SpawnerShop.CurrentPageEnum.PlayerSellingPage)
//                                 {
//                                     int sx = slot % 9;
//                                     int sy = (int) Math.floor(slot / 9);
//                                     Item is  = ah.getItem(slot);
//                                     boolean isi = false;
//                                     int isc = is.getCount();
//                                     if (is != null && is.getId() != 0) {
//                                         if (is.getId() == Item.IRON_BLOCK) isi = true;
//                                         CyberCoreMain.Log.Error(
//                                             "Was LOG ||" + "Selected Slot SX:" + sx + " | SY:" + sy);
//                                         if (sy != 0 && sy != 5 && sx != 4 && !isi)
//                                         {
//                                             if (sx < 4)
//                                             {
//                                                 //Cancel
//                                                 ah.setPage(1);
//                                             }
//                                             else
//                                             {
//                                                 //Buy
//                                                 ah.SetupPageToFinalConfirmItem(ah.MultiConfirmData, isc, false);
//                                             }
//                                         }
//                                     }
//                                     eeee.SetCancelled();
//                                     return;
//                                 }
//                                 else
//                                 {
//                                     Item si = ah.getContents().get(slot);
//                                     if (si != null)
//                                     {
//                                         if (ah.getCurrentPage() ==
//                                             SpawnerShop.CurrentPageEnum.Confirm_Purchase_Not_Enough_Money)
//                                         {
//                                             ah.setPage(1);
//                                             ah.ClearConfirmPurchase();
//                                             //Back Home
//                                             break;
//                                         }
//                                         else
//                                         {
//                                             CyberCoreMain.Log.Error("Was LOG ||" + "CPPPPPPPP");
//
//                                             if (si.getId() == BlockID.EMERALD_BLOCK)
//                                             {
//                                                 CyberCoreMain.Log.Error("Was LOG ||" + "CONFIRM PURCHASE!!!!!!!");
//                                                 ah.SSF.PurchaseItem((CorePlayer) ah.getHolder(), ah.getPage(),
//                                                     ah.ConfirmPurchaseSlot, si.getCount());
//                                                 break;
//                                             }
//                                             else if (si.getId() == BlockID.RedSTONE_BLOCK)
//                                             {
//                                                 CyberCoreMain.Log.Error("Was LOG ||" + "DENCLINE PURCHASE!!!!!!!!");
//                                                 ah.setPage(1);
//                                                 ah.ClearConfirmPurchase();
//                                                 break;
//                                             }
//                                             else
//                                             {
//                                                 ah.setPage(1);
//                                                 CyberCoreMain.Log.Error("Was LOG ||" + "UNKNOWNMNNN!!!!!!!!");
//                                                 ah.ClearConfirmPurchase();
//                                                 break;
//                                             }
//                                         }
//                                     }
//                                 }
//                             }
//                         }
//                         else
//                         {
//                             switch (slot)
//                             {
//                                 case SpawnerShop.MainPageItemRef.LastPage:
//                                     ah.GoToPrevPage();
//                                     break;
//                                 case SpawnerShop.MainPageItemRef.NextPage:
//                                     ah.GoToNextPage();
//                                     break;
//                                 case SpawnerShop.MainPageItemRef.Search:
//                                     break;
//                                 case SpawnerShop.MainPageItemRef.Reload:
//                                     ah.ReloadCurrentPage();
//                                     break;
//                                 case SpawnerShop.MainPageItemRef.Catagories:
//                                     ah.DisplayCatagories();
//                                     break;
//                                 case SpawnerShop.MainPageItemRef.ToggleAdmin:
//                                     ah.AdminMode = !ah.AdminMode;
//                                     eeee.SetCancelled(false);
//                                     ah.ReloadCurrentPage();
//                                     break;
//                             }
//                         }
//                     }
//                 }
//             }
//
//             eeee.SetCancelled();
//             eeee.getTransaction().getSource().sendAllInventories();
//         }

//         private void ShopInvMainHandle(Inventory inv, SlotChangeAction sca, InventoryTransactionEvent eeee)
//         {
//             eeee.SetCancelled();
//             CyberCoreMain.Log.Error("Was LOG ||" + "CHECK INNNNNVVV222222VVVV " + sca);
//             ShopInv ah = (ShopInv) inv;
// //                    if(!ah.Init)return;
//             CyberCoreMain.Log.Error("Was LOG ||" + sca.getSlot() + " || " + ah.getHolder().getName() + " || " +
//                                     ah.getHolder().getClass().getName());
//             CorePlayer ccpp = (CorePlayer) ah.getHolder();
//             int slot = sca.getSlot();
//             int sx = slot % 9;
//             int sy = (int) Math.floor(slot / 9);
// //                    eeee.SetCancelled();
//             if (slot < 5 * 9)
//             {
//                 CyberCoreMain.Log.Error("Was LOG ||" + "TOP INV");
//                 //TODO CONFIRM AND SHOW ITEM
//                 if (!ah.ConfirmPurchase)
//                 {
//                     Item is  = ah.getItem(slot);
//                     if (ah.CurrentPage == ShopInv.CurrentPageEnum.Catagories)
//                     {
//                         ShopCatagoreyHandler(slot, ah);
//                     }
//                     else
//                     {
//                         if (!ah.AdminMode)
//                             ah.ConfirmItemPurchase(slot, ah.AdminMode);
//                         else
//                         {
//                             if (ah.CurrentPage == AdminItemEdit)
//                             {
//                                 AdminItemEditHandle(ah, inv, sca, eeee, slot);
//                             }
//                             else
//                                 ah.AdminModeItem(slot, ah.AdminMode);
//                         }
//                     }
//
// //                        ccpp.AH.ConfirmItemPurchase(slot);
//                 }
//                 else
//                 {
//                     Item is  = ah.getItem(slot);
//                     if (ah.CurrentPage == ShopInv.CurrentPageEnum.Catagories)
//                     {
//                         if (slot == 11)
//                         {
//                             ah.CCM.SpawnShop.OpenShop((CorePlayer) ah.getHolder(), 1);
//                         }
//                         else
//                         {
//                             ah.setPage(1);
//                         }
//                     }
//                     else if (ah.CurrentPage == ShopInv.CurrentPageEnum.PlayerSellingPage)
//                     {
//                         boolean isi = false;
//                         int isc = is.getCount();
//                         if (is != null && is.getId() != 0) {
//                             if (is.getId() == Item.IRON_BLOCK) isi = true;
//                             CyberCoreMain.Log.Error("Was LOG ||" + "Selected Slot SX:" + sx + " | SY:" + sy);
//                             if (sy != 0 && sy != 5 && sx != 4 && !isi)
//                             {
//                                 if (sx < 4)
//                                 {
//                                     //Sell
//                                     ah.SetupPageToFinalConfirmItem(ah.MultiConfirmData, isc, true);
//                                 }
//                                 else
//                                 {
//                                     //Buy
//                                     ah.SetupPageToFinalConfirmItem(ah.MultiConfirmData, isc, false);
//                                 }
//                             }
//                         }
//                         return;
//                     }
//                     else
//                     {
//                         Item si = ah.getContents().get(slot);
//                         if (si != null)
//                         {
//                             if (ah.getCurrentPage() == ShopInv.CurrentPageEnum.Confirm_Purchase_Not_Enough_Money)
//                             {
//                                 ah.setPage(1);
//                                 ah.ClearConfirmPurchase();
//                                 //Back Home
//                                 return;
//                             }
//                             else
//                             {
//                                 CyberCoreMain.Log.Error("Was LOG ||" + "CPPPPPPPP");
//
//                                 if (si.getId() == BlockID.EMERALD_BLOCK)
//                                 {
//                                     CyberCoreMain.Log.Error("Was LOG ||" + "CONFIRM PURCHASE!!!!!!!");
//                                     ah.SF.PurchaseItem((CorePlayer) ah.getHolder(), ah.getPage(),
//                                         ah.ConfirmPurchaseSlot, si.getCount(), ah.AdminMode);
//                                     return;
//                                 }
//                                 else if (si.getId() == BlockID.RedSTONE_BLOCK)
//                                 {
//                                     CyberCoreMain.Log.Error("Was LOG ||" + "DENCLINE PURCHASE!!!!!!!!");
//                                     ah.setPage(1);
//                                     ah.ClearConfirmPurchase();
//                                     return;
//                                 }
//                                 else
//                                 {
//                                     ah.setPage(1);
//                                     CyberCoreMain.Log.Error("Was LOG ||" + "UNKNOWNMNNN!!!!!!!!");
//                                     ah.ClearConfirmPurchase();
//                                     return;
//                                 }
//                             }
//                         }
//                     }
//                 }
//             }
//             else
//             {
//                 switch (slot)
//                 {
//                     case ShopInv.MainPageItemRef.LastPage:
//                         ah.GoToPrevPage();
//                         break;
//                     case ShopInv.MainPageItemRef.NextPage:
//                         ah.GoToNextPage();
//                         break;
//                     case ShopInv.MainPageItemRef.Search:
//                         break;
//                     case ShopInv.MainPageItemRef.Reload:
//                         ah.ReloadCurrentPage();
//                         break;
//                     case ShopInv.MainPageItemRef.Catagories:
//                         ah.DisplayCatagories();
//                         break;
//                     case ShopInv.MainPageItemRef.ToggleAdmin:
//                         ah.AdminMode = !ah.AdminMode;
//                         eeee.SetCancelled(false);
//                         ah.ReloadCurrentPage();
//                         break;
//                 }
//             }
//
//             eeee.SetCancelled();
//         }
//
//         private void
//             AdminItemEditHandle(ShopInv ah, Inventory inv, SlotChangeAction sca, InventoryTransactionEvent eeee, int
//                 slot)
//         {
//             int sx = slot % 9;
//             int sy = (int) Math.floor(slot / 9);
//             Item i = sca.Inventory.getItem(sca.getSlot());
//             if (sx == 3 && sy == 3)
//             {
//                 if (i.hasCompoundTag() && i.getNamedTag().contains("ShopID"))
//                 {
//                     int s = i.getNamedTag().getInt("ShopID");
//                     ShopMysqlData sd = ah.CCM.Shop.getItemFrom(s);
//                     if (sd == null)
//                     {
//                         CyberCoreMain.Log.Error("Was LOG ||" + "Error!!!!! WTF!!!1221aas222e2aaqqqwd  ass");
//                     }
//                     else
//                     {
//                         //New Edit Window
//                     }
//                 }
//             }
//         }

        // private void ShopCatagoreyHandler(int slot, ShopInv ah)
        // {
        //     if (slot == 11)
        //     {
        //         ah.CCM.SpawnShop.OpenShop((CorePlayer) ah.getHolder(), 1);
        //     }
        //     else if (slot == 12)
        //     {
        //         //Food
        //         ah.showFoodCategory();
        //     }
        //     else
        //     {
        //         ah.setPage(1);
        //     }
        // }


        // [EventHandler]
        //
        // public void TD(PlayerTakeDamageEvent eeee)
        // {
        //     Entity e = eeee.source.entity;
        //     if (e is CorePlayer) {
        //         CorePlayer cp = (CorePlayer) e;
        //         if (cp.getPlayerClass() != null)
        //         {
        //             // cp.getPlayerClass().HandelEvent(event);
        //         }
        //     }
        // }

        // @EventHandler(priority = EventPriority.HIGHEST)
        //
        // public void EICE(EntityInventoryChangeEvent eeee)
        // {
        //     if (event == null) {
        //         CyberCoreMain.Log.Error("Was LOG ||" + "WTF NUUUUUUUUUUUUUUUUUUULLLLLLLLLLLLLLLLLLLLL");
        //         return;
        //     }
        //     Entity e = eeee.getEntity();
        //     if (e instanceof CorePlayer) {
        //         CorePlayer cp = (CorePlayer) e;
        //         if (cp.getPlayerClass() != null)
        //         {
        //             cp.getPlayerClass().HandelEvent(event);
        //         }
        //     }
        // }

        // @EventHandler(priority = EventPriority.HIGHEST)
        //
        // public void EntityDamageEvent(EntityDamageEvent eeee)
        // {
        //     Entity e = eeee.getEntity();
        //     if (e instanceof CorePlayer) {
        //         CorePlayer cp = (CorePlayer) e;
        //         if (cp.getPlayerClass() != null)
        //         {
        //             cp.getPlayerClass().HandelEvent(event);
        //         }
        //     }
        // }


//    @EventHandler(priority = EventPriority.HIGHEST)
//    public void onCreation(PlayerCreationEvent eeee) {
//        eeee.setPlayerClass(CorePlayer.class);
//        eeee.setBaseClass(CorePlayer.class);
//    }


        // @EventHandler(priority = EventPriority.LOWEST)
        //
        // public void quitEvent(PlayerQuitEvent eeee)
        // {
        //     String Msg = (String) plugin.MainConfig.get("Leave-Message");
        //     eeee.setQuitMessage(Msg.replace("{player}", eeee.getPlayer().getName()));
        //     Player p = eeee.getPlayer();
        //     if (p instanceof CorePlayer) {
        //         plugin.ClassFactory.save((CorePlayer) p);
        //     }
        // }


        //@TODO Check for BadWords!
        // [EventHandler]
        //        public void onChatEvent(PlayerChatEvent eeee)
        //        {
        //            if (event.isCancelled()) return;
        //            //SHouldnt need thins @TODO^^
        //            eeee.SetCancelled(true);
        //            if (plugin.MuteChat && (!event.getPlayer().hasPermission("CyberTech.CyberChat.op"))) {
        //                eeee.getPlayer().SendMessage(ChatColors.Yellow + "All Chat Is Muted! Try again later!");
        //                return;
        //            }
        //            if (plugin.isMuted(event.getPlayer())) {
        //                eeee.getPlayer().SendMessage(ChatColors.Yellow + "You are Muted! Try again later!");
        //                return;
        //            }
        //            String FinalChat = formatForChat(event.getPlayer(), eeee.getMessage());
        //            if (FinalChat == null) return;
        //            if (plugin.LM.containsKey(event.getPlayer().getName().toLowerCase()) && plugin.LM.get(event.getPlayer()
        //                .getName().toLowerCase()).equalsIgnoreCase(FinalChat)) {
        //                eeee.getPlayer().SendMessage(FinalChat);
        //                return;
        //            }
        //            plugin.LM.put(event.getPlayer().getName().toLowerCase(), FinalChat);
        //            plugin.checkSpam(event.getPlayer());
        //            Server.GetInstance().getLogger().info(FinalChat);
        //            for (Map.Entry < UUID, Player > e : plugin.getServer().getOnlinePlayers().entrySet())
        //            {
        //                if (plugin.PlayerMuted.contains(e.getValue().getName().toLowerCase())) continue;
        //                e.getValue().SendMessage(FinalChat);
        //            }
        //        }

//         public String formatForChat(Player player, String chat)
//         {
//             Dictionary<String, Object> badwords = new Dictionary<String, Object>()
//             {
//                 {
//                     put("fuck", "f***");
//                     put("shit", "s***");
//                     put("nigger", "kitty");
//                     put("nigga", "boi");
//                     put("bitch", "Sweetheart");
//                     put("hoe", "tool");
//                     put("ass", "butt");
//                 }
//             };
//             String faction;
//             Faction pf = plugin.getPlayerFaction(player);
//             if (pf != null)
//             {
//                 faction = pf.getDisplayName();
//                 //FactionFormat = ChatColors.Gray+FactionFormat.replace("{value}",fp.getFaction().getTag())+ChatColors.WHITE;
//             }
//             else
//             {
//                 faction = ChatColors.Gray + "[NF]" + ChatColors.WHITE;
//             }
//
//             //ANTI BADWORDS
//             String chatb4 = chat;
//             String chatafter = chat;
//             for (String s :
//             chat.split(" ")) {
//                 if (!badwords.containsKey(s.toLowerCase())) continue;
//                 chatafter = chatafter.replaceAll("(?i)" + s, badwords.get(s.toLowerCase()).toString());
//             }
//             /*
//             Fucks up words like Class and BAss
//             for(String b: badwords.keySet()){
//                 if(chatafter.toLowerCase().contains(b.toLowerCase())) {
//                     chatafter = chatafter.replaceAll("(?i)" + b, badwords.get(b).toString());
//                     chatafter = chatafter.replaceAll(b, badwords.get(b).toString());
//                 }
//             }*/
//             /*String chat2 = chat;
//             chat2 = chat.replace(" ","");
//              */
//             //ANTI WORK AROUND BADWORDS
//             //@TODO Remove all spaces and use Regex to replace all Instaces of it
//
//             return plugin.RF.getPlayerRank(player).getChat_format().format(faction,
//                 plugin.RF.getPlayerRank(player).getDisplayName(), player, chatafter);
// /*
//         put("Chat-Format", "{rank}{faction}{player-name} > {msg}");
//         put("Faction-Format", "[{value}]");
//         put("Rank-Format", "[{value}]");
//         put("Join-Message", "");
//         put("Leave-Message", "");*/
//         }
//         
    }
}