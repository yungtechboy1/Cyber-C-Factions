﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Numerics;
using System.Runtime.InteropServices.WindowsRuntime;
using MiNET;
using MiNET.BlockEntities;
using MiNET.Blocks;
using MiNET.Effects;
using MiNET.Entities;
using MiNET.Entities.World;
using MiNET.Items;
using MiNET.Net;
using MiNET.Sounds;
using MiNET.UI;
using MiNET.Utils;
using MiNET.Worlds;
// using MySqlX.XDevAPI;
using OpenAPI;
using OpenAPI.Events.Entity;
using OpenAPI.Events.Player;
using OpenAPI.Player;

namespace CyberCore
{
    public class CorePlayer : OpenPlayer
    {
        private static bool CooldownLock;
        public static readonly float DEFAULT_SPEED = 0.1F;
        private readonly string Cooldown_Class = "Class";
        private readonly string Cooldown_DTP = "DelayTP";
        private readonly string Cooldown_Faction = "Faction";
        private readonly string Cooldown_EPD = "EPD";
        private readonly string Cooldown_EPD_Valid = "EPDValid";
        private readonly string Scoreboard_Class = "ScoreBoard";
        public int banned = 0;

        private readonly int BaseSwingSpeed = 7; //Was 10 but felt too slow for good PVP

     
        public bool BuffsChanced = false;


        private int ClassCheck = -1;
        public CombatData Combat;
        private PlayerLocation CTLastPos;
        public int CustomExtraHP;
        public float CustomMovementSpeed = 0.1f;
        public int deaths;
        public bool DebuffsChanced = false;

        //    public String faction_id = null;
        public Dictionary<string, object> extraData = new Dictionary<string, object>();
        public string Faction;

        private int FactionCheck = -1;

        // public string FactionInvite;
        // public int FactionInviteTimeout = -1;
        public int fixcoins = 0;

        // private bool isInTeleportingProcess = false;

        // private bool isTeleporting;

        private Form nw;
        private Item ItemBeingEnchanted;
        private bool ItemBeingEnchantedLock;
        public int kills;
        private Vector3 lastBreakVector31;
        public string LastMessageSentTo = null;
        public int MaxHomes = 5;

        /**
     * @
     */
        public int money = 0;

        public bool MuteMessage = false;
        

        // private CorePlayer TargetTeleporting;
        // private PlayerLocation TargetTeleportingLoc;
        // private int TeleportTick;
        public string TPR;

        public int TPRTimeout;

        private long uct;
        private bool uw;
        private long WFTP_TPTick = -1;

        public int WFTP_CancelDistance = 2;
        public bool WFTP_Effects;
        private PlayerLocation WFTP_ToPlayerLocation;
        private PlayerLocation WFTP_StartPos;

        public CorePlayer(MiNetServer server, IPEndPoint endPoint, OpenApi api) : base(server, endPoint, api)
        {
            
        }
        //
        // private void InvChange(Player player, Inventory inventory, byte slot, Item itemStack)
        // {
        //
        // }
        //

        // public override void HandleMcpeContainerClose(McpeContainerClose message)
        // {
        //     base.HandleMcpeContainerClose(message);
        //     
        //     _openInventory = 
        // }
        //
        // openinv



        // public new void OpenInventory(BlockCoordinates inventoryCoord)
        // {
        //     SendMessage("THIS CHU2222222222222222222222222222222222NK WILL NOW BE SAVED!!!");
        //     
        // }
        public void CyberOpenInventory(BlockCoordinates inventoryCoord)
        {
         OpenInventory(inventoryCoord);
         Inventory inventory = Level.InventoryManager.GetInventory(inventoryCoord);
         if (!inventory.Observers.Contains(this))
         {
             return;
         }
        
         var c = Level.GetChunk(inventoryCoord);
         c.IsDirty = true;
         c.NeedSave = true;
         SendMessage("THIS CHUNK WILL NOW BE SAVED!!!");
        }


        public bool ShowHTP { get; set; }
       
        public override void HandleMcpeInventoryTransaction(McpeInventoryTransaction message)
        {                        Console.WriteLine("CALLLLLLLLL3333333333LLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLL");

            switch (message.transaction)
            {
                case InventoryMismatchTransaction transaction:
                    Console.WriteLine("IMT");
                    break;
                case ItemReleaseTransaction transaction:
                    Console.WriteLine("IRT");
                    break;
                case ItemUseOnEntityTransaction transaction:
                    Console.WriteLine("IUOET");
                    break;
                case ItemUseTransaction transaction:
                    Console.WriteLine("IUT");
                    break;
                case NormalTransaction transaction:
                    // if (CustomInvOpen != CustomInvType.NA)
                    // {
                        Console.WriteLine("NT");
                        HandleTransactionRecords2(transaction.TransactionRecords);
                        return;
                    // }
                    // else break;
            }

            // Console.WriteLine("THIS TRANSACTION WAS A "+message.transaction);
            base.HandleMcpeInventoryTransaction(message);
        }
        //0 Player Inv
        //10 Chest
        //124 Cursor
        
        public enum TransactionType
        {
            NA = -1,
            PlayerInv = 0,
            Chest = 10,
            Cursor = 124
        }

        protected override void HandleNormalTransaction(NormalTransaction transaction)
        {
            Console.WriteLine("TTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTT CCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCC");
            base.HandleNormalTransaction(transaction);
        }

        public  void HandleTransactionRecords2(List<TransactionRecord> transactionRecords)
        {
            Item obj1 = (Item) null;
            Item obj2 = (Item) null;
            TransactionType from = TransactionType.NA;
            TransactionType to = TransactionType.NA;
            
            ItemEntity itemEntity = (ItemEntity) null;
            foreach (TransactionRecord transactionRecord1 in transactionRecords)
            {
                Item oldItem = transactionRecord1.OldItem;
                Item itemStack = transactionRecord1.NewItem;
                int slot1 = transactionRecord1.Slot;
                switch (transactionRecord1)
                {
                    case ContainerTransactionRecord transactionRecord:
                        int inventoryId = transactionRecord.InventoryId;
                        Console.WriteLine(inventoryId+"<<<<<");
                        // if (inventoryId == 10)
                        // {
                        //     if (CustomInvOpen == CustomInvType.Shop)
                        //     {
                        //         ClearCursor();
                        //         ShopInv?.MakeSelection(slot1,this);
                        //         
                        //     }
                        // }else if (inventoryId == 124)
                        // {
                        //     Inventory.UiInventory.Cursor = new ItemAir();
                        //     SendPlayerInventory();
                        //     ClearCursor();
                        // }
                        break;
                }
            }
        }

        public void ClearCursor()
        {
            Inventory.UiInventory/*?*/.Cursor = new ItemAir();
            SendPlayerInventory();
            // var a = new ItemStacks();
            // a.Add(new ItemAir());
            // McpeInventoryContent inventoryContent1 = Packet<McpeInventoryContent>.CreateObject(1L);
            // inventoryContent1.inventoryId = 124;
            // inventoryContent1.input = a;
            // this.SendPacket((Packet) inventoryContent1);
            // McpeInventoryContent inventoryContent2 = Packet<McpeInventoryContent>.CreateObject(1L);
            // inventoryContent2.inventoryId = 119;
            // inventoryContent2.input = a;
            // this.SendPacket((Packet) inventoryContent1);
            // McpeInventoryContent inventoryContent3 = Packet<McpeInventoryContent>.CreateObject(1L);
            // inventoryContent3.inventoryId = 123;
            // inventoryContent3.input = a;
            // this.SendPacket((Packet) inventoryContent1);
            McpeInventorySlot mis = Packet<McpeInventorySlot>.CreateObject(1l);
            mis.item = new ItemAir();
            mis.slot = 0;
            mis.inventoryId = 124;
            SendPacket(mis);
            
            // McpeInventorySlot mis2 = Packet<McpeInventorySlot>.CreateObject(1l);
            // mis2.item = new ItemAir();
            // mis2.slot = 0;
            // mis2.inventoryId = 119;
            // SendPacket(mis2);
            //
            // McpeInventorySlot mis3 = Packet<McpeInventorySlot>.CreateObject(1l);
            // mis3.item = new ItemAir();
            // mis3.slot = 0;
            // mis3.inventoryId = 123;
            // SendPacket(mis3);
            // Inventory.CursorInventory
        }

        public void setMovementSpeed(float speed, bool send)
        {
            MovementSpeed = speed;
            if (IsSpawned && send) SendUpdateAttributes();
        }

        public void setMaxHealth(int maxHealth)
        {
            HealthManager.MaxHealth = maxHealth;
        }

      

        public float getHealth()
        {
            return HealthManager.Health;
        }


      
        public void sendAttributes()
        {
            SendUpdateAttributes();
        }


        public bool IsItemBeingEnchanted()
        {
            return getItemBeingEnchanted() != null;
        }

        public void clearItemBeingEnchanted()
        {
            setItemBeingEnchanted(null);
        }

        public Item getItemBeingEnchanted()
        {
            return ItemBeingEnchanted;
        }

        public void setItemBeingEnchanted(Item itemBeingEnchanted)
        {
            if (IsItemBeingEnchanted())
            {
            }

            ItemBeingEnchanted = itemBeingEnchanted;
        }

        public bool isItemBeingEnchantedLock()
        {
            return ItemBeingEnchantedLock;
        }

        public void setItemBeingEnchantedLock(bool itemBeingEnchantedLock)
        {
            ItemBeingEnchantedLock = itemBeingEnchantedLock;
        }

//    
//    public PlayerEconData GetEconData() {
//        return new PlayerEconData(GetData());
//    }

        public void removeItemBeingEnchantedLock()
        {
            setItemBeingEnchantedLock(false);
        }

        public void setItemBeingEnchantedLock()
        {
            setItemBeingEnchantedLock(true);
        }

        public void ReturnItemBeingEnchanted()
        {
            if (IsItemBeingEnchanted() && !isItemBeingEnchantedLock())
            {
                var i = getItemBeingEnchanted();
                Inventory.AddItem(i, true);
                clearItemBeingEnchanted();
                removeItemBeingEnchantedLock();
            }
        }




        // public int addWindow(Inventory inventory, int forceId, bool isPermanent)
        // {
        //     if (this.windows.ContainsKey(inventory)) return this.windows.get(inventory);
        //     int cnt;
        //     if (forceId == null)
        //         this.windowCnt = cnt = Math.max(4, ++this.windowCnt % 99);
        //     else
        //         cnt = forceId;
        //     this.windows.put(inventory, cnt);
        //
        //     if (isPermanent) this.permanentWindows.add(cnt);
        //
        //     if (inventory.open(this)) return cnt;
        //
        //     this.removeWindow(inventory);
        //     SendMessage("ERROR!!!!!!! I FUCKKKKED UUUUPPP");
        //     return -1;
        // }
        // public float getMovementSpeed()
        // {
        //     return super.getMovementSpeed();
        // }
        
        public int addDeath()
        {
            return deaths += 1;
        }

        public int addDeaths(int amount)
        {
            return deaths += amount;
        }

        public int addKill()
        {
            return kills += 1;
        }

        public int addKills(int amount)
        {
            return kills += amount;
        }

        public double calculateKD()
        {
            return kills / deaths;
        }

        public bool hasNewWindow()
        {
            return nw != null;
        }

        public Form getNewWindow()
        {
            return nw;
        }

        public void setNewWindow(Form nw)
        {
            this.nw = nw;
        }

        public void clearNewWindow()
        {
            this.nw = null;
        }


        //
        // public void fall(float fallDistance)
        // {
        //     if (!uw) super.fall(fallDistance);
        // }



        protected override bool AcceptPlayerMove(McpeMovePlayer message, bool isOnGround, bool isFlyingHorizontally)
        {
            // if (ShowHTP)
            // {
            //     ShowHTP = false;
            //     SendForm(new HTP_0_Window());
            // }
            return base.AcceptPlayerMove(message, isOnGround, isFlyingHorizontally);
        }

  
       

        public int getBaseSwingSpeed()
        {
            return BaseSwingSpeed;
        }

      

        public int attackTime = 0;

//         public void handleDataPacket(DataPacket packet)
//         {
// //        CyberCoreMain.Log.Error("Was LOG ||"+"DP >>>> " + packet + "||" + packet.pid());
//             if (!connected) return;
//
//             try
//
//             (Timing timing = Timings.getReceiveDataPacketTiming(packet)) {
//                 DataPacketReceiveEvent ev = new DataPacketReceiveEvent(this, packet);
//                 this.server.getPluginManager().callEvent(ev);
//                 if (ev.isCancelled()) return;
//
//                 if (packet.pid() == ProtocolInfo.BATCH_PACKET)
//                 {
//                     this.server.getNetwork().processBatch((BatchPacket) packet, this);
//                     return;
//                 }
//
//                 switch (packet.pid())
//                 {
//                     case ProtocolInfo.MODAL_FORM_RESPONSE_PACKET:
//                         if (!this.spawned || !this.isAlive()) break;
//
//                         ModalFormResponsePacket modalFormPacket = (ModalFormResponsePacket) packet;
//
//                         if (formWindows.ContainsKey(modalFormPacket.formId))
//                         {
//                             Form window = formWindows.remove(modalFormPacket.formId);
//                             if (window instanceof CyberForm)
//                             ((CyberForm) window).setResponse(modalFormPacket.data.trim(), this);
//                             else
//                             window.setResponse(modalFormPacket.data.trim());
//
//                             PlayerFormRespondedEvent event = new PlayerFormRespondedEvent(this, modalFormPacket.formId,
//                                 window);
//                             getServer().getPluginManager().callEvent(event);
//                             return;
//                         }
//
// //                    return;
//                         break;
//                     case ProtocolInfo.MOB_EQUIPMENT_PACKET:
//                         if (!this.spawned || !this.isAlive()) break;
//
//                         MobEquipmentPacket mobEquipmentPacket = (MobEquipmentPacket) packet;
//
//                         //TODO Make into a Custom Event!
//                         if (getPlayerClass() != null)
//                         {
//                             for (PowerAbstract p :
//                             getPlayerClass().getActivePowers()) {
//                                 if (p.getLS().getSlot() == mobEquipmentPacket.hotbarSlot)
//                                 {
//                                     p.initPowerRun();
//                                     getInventory().setHeldItemIndex(getInventory().getHeldItemIndex(), true);
//                                 }
//                             }
//                         }
//
//                         break;
//                     case INVENTORY_TRANSACTION_PACKET:
// //                    break;
//                         if (this.isSpectator())
//                         {
//                             this.sendAllInventories();
//                             break;
//                         }
//
//                         try
//                         {
// //                        InventoryTransactionPacket transactionPacket = (InventoryTransactionPacket) packet;
// //                        CyberCoreMain.Log.Error("Was LOG ||"+"BBBBBBBBBBBBBBBBBBBBBBBBB!!!!!!!!1");b
//                             CustomInventoryTransactionPacket transactionPacket2 =
//                                 (CustomInventoryTransactionPacket) packet;
//
//                             if (transactionPacket2 == null)
//                                 System.out.
//                             println("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
//
// //                        CyberCoreMain.Log.Error("Was LOG ||"+"BBBBBBBBBBBBBBBBBBBBBBBBB!!!!!!!!2");
//                             List<InventoryAction> actions = new List<>();
//                             for (CustomNetworkInventoryAction na :
//                             transactionPacket2.actions) {
// //                            CyberCoreMain.Log.Error("Was LOG ||"+"zACTIONz z-1+++");
// //                            CustomNetworkInventoryAction networkInventoryAction = new CustomNetworkInventoryAction(na);
//                                 CustomNetworkInventoryAction networkInventoryAction = na;
//                                 InventoryAction a = networkInventoryAction.createInventoryAction(this);
//                                 InventoryAction aa = na.createInventoryAction(this);
//
// //                            CyberCoreMain.Log.Error("Was LOG ||"+"zACTIONz xxxx>"+new CustomNetworkInventoryAction(na));
//                                 CyberCoreMain.Log.Error("Was LOG ||"+"zACTIONz xxxx>" + na);
// //                            CyberCoreMain.Log.Error("Was LOG ||"+"zACTIONz z");Cybers
// //                            CyberCoreMain.Log.Error("Was LOG ||"+"zACTIONz z > "+a);
// //                            CyberCoreMain.Log.Error("Was LOG ||"+"zACTIONz z > "+a.getClass().getName());
//                                 if (a instanceof SlotChangeAction && aa instanceof SlotChangeAction) {
//                                     SlotChangeAction sca = (SlotChangeAction) a;
//                                     SlotChangeAction scaa = (SlotChangeAction) aa;
// //                                CyberCoreMain.Log.Error("Was LOG ||"+"GGGGGGGGGGGGG"+scaa.getSlot());
// //                                CyberCoreMain.Log.Error("Was LOG ||"+"GGGGGGGGGGGGG"+sca.getSlot());
//                                 }
// //                            CyberCoreMain.Log.Error("Was LOG ||"+"zACTIONz 5.1.1 > "+a.getTargetItem());
// //                            CyberCoreMain.Log.Error("Was LOG ||"+"zACTIONz 5.1.2 > "+a.getSourceItem());
//
//                                 if (a == null)
//                                 {
//                                     this.getServer().getLogger()
//                                         .debug("Unmatched inventory action from " + this.getName() + ": " +
//                                                networkInventoryAction);
//                                     this.sendAllInventories();
//                                     break packetswitch;
//                                 }
//
//                                 CyberCoreMain.Log.Error("Was LOG ||"+"Adding Network Action to Regualr Action!!!!!!!!");
//                                 actions.add(a);
//                             }
//
// //                        CyberCoreMain.Log.Error("Was LOG ||"+"BBBBBBBBBBBBBBBBBBBBBBBBB!!!!!!!!3");
//                             if (transactionPacket2.isCraftingPart)
//                             {
//                                 CyberCoreMain.Log.Error("Was LOG ||"+"BBBBBBBBBBBBBBBBBBBBBBBBB!!!!!!!!4 IS CRAFTING");
//                                 if (this.cct == null)
//                                 {
//                                     CyberCoreMain.Log.Error("Was LOG ||"+"BBBBBBBBBBBBBBBBBBBBBBBBB!!!!!!!!5 WILL CREATE NEW CT");
//                                     this.cct = new CustomCraftingTransaction(this, actions);
//                                 }
//                                 else
//                                 {
//                                     for (InventoryAction action :
//                                     actions) {
//                                         CyberCoreMain.Log.Error("Was LOG ||"+"BBBBBBBBBBBBBBBBBBBBBBBBB!!!!!!!!6 ADDING TO EXISTING CTT");
//                                         this.cct.addAction(action);
//                                     }
//                                 }
//
//                                 if (this.cct.getPrimaryOutput() != null)
//                                 {
//                                     //we get the actions for this in several packets, so we can't execute it until we get the result
//
//                                     CyberCoreMain.Log.Error("Was LOG ||"+"BBBBBBBBBBBBBBBBBBBBBBBBB!!!!!!!!7 CALLING EXECUTE");
//                                     if (!this.cct.execute()) server.getLogger().error("ERROR NO EXECITE!");
//                                     this.cct = null;
//                                 }
//
//                                 return;
//                             }
//
//                             if (this.cct != null)
//                             {
//                                 this.server.getLogger()
//                                     .debug(
//                                         "Got unexpected normal inventory action with incomplete crafting transaction from " +
//                                         this.getName() + ", refusing to execute crafting");
//                                 this.cct = null;
//                             }
//
//
//                             //                        CyberCoreMain.Log.Error("Was LOG ||"+"AAAAAAAAAAAAAAAAAAAAAAAAA");
// //                        CyberCoreMain.Log.Error("Was LOG ||"+"AAAAAAAAAAAAAAAAAAAAAAAAA2"+transactionPacket2.transactionType);
//                             switch (transactionPacket2.transactionType)
//                             {
//                                 case InventoryTransactionPacket.TYPE_NORMAL:
// //                                CyberCoreMain.Log.Error("Was LOG ||"+"ZZZZZZZZZZZZZZZZZZ");
//                                     CustomInventoryTransaction transaction =
//                                         new CustomInventoryTransaction(this, actions);
//
// //                                CyberCoreMain.Log.Error("Was LOG ||"+"ZZZZZZZZZZZZZZZZZZ1"+transaction);
// //                                CyberCoreMain.Log.Error("Was LOG ||"+"ZZZZZZZZZZZZZZZZZZ2"+transaction.getInventories());
// //                                CyberCoreMain.Log.Error("Was LOG ||"+"ZZZZZZZZZZZZZZZZZZ3"+transaction.canExecute());
//                                     if (!transaction.execute())
//                                     {
//                                         this.server.getLogger()
//                                             .debug("Failed to execute inventory transaction from " + this.getName() +
//                                                    " with actions: " + Arrays.toString(transactionPacket2.actions));
//                                         break packetswitch; //oops!
//                                     }
//
//                                     //TODO: fix achievement for getting iron from furnace
//
//                                     break packetswitch;
//                                 case InventoryTransactionPacket.TYPE_MISMATCH:
//                                     if (transactionPacket2.actions.length > 0)
//                                     {
//                                         this.server.getLogger()
//                                             .debug("Expected 0 actions for mismatch, got " +
//                                                    transactionPacket2.actions.length + ", " +
//                                                    Arrays.toString(transactionPacket2.actions));
//                                         this.server.getLogger()
//                                             .error("Expected 0 actions for mismatch, got " +
//                                                    transactionPacket2.actions.length + ", " +
//                                                    Arrays.toString(transactionPacket2.actions));
//                                         getCursorInventory().clearAll();
//                                     }
//                                     else
//                                     {
//                                         this.server.getLogger().error("Expected 0 actions for mismatch, got NUN");
//                                     }
//
//                                     this.sendAllInventories();
//
//                                     break packetswitch;
//                                 case InventoryTransactionPacket.TYPE_USE_ITEM:
//                                     UseItemData useItemData = (UseItemData) transactionPacket2.transactionData;
//
//                                     BlockVector3 blockVector = useItemData.blockPos;
//                                     BlockFace face = useItemData.face;
//
//                                     int type = useItemData.actionType;
//                                     Item item;
//                                     switch (type)
//                                     {
//                                         case InventoryTransactionPacket.USE_ITEM_ACTION_CLICK_BLOCK:
//                                             this.setDataFlag(DATA_FLAGS, DATA_FLAG_ACTION, false);
//
//                                             CyberCoreMain.Log.Error("Was LOG ||"+"wwwwwwwwwwwwww > 11111111111111111");
//                                             if (this.canInteract(blockVector.add(0.5, 0.5, 0.5),
//                                                 this.isCreative() ? 13 : 7))
//                                             {
//                                                 CyberCoreMain.Log.Error("Was LOG ||"+"wwwwwwwwwwwwww > 22222222222");
//                                                 if (this.isCreative())
//                                                 {
//                                                     Item i = inventory.getItemInHand();
//                                                     if (this.level.useItemOn(blockVector.asVector3(), i, face,
//                                                         useItemData.clickPos.x, useItemData.clickPos.y,
//                                                         useItemData.clickPos.z, this) != null)
//                                                     {
//                                                         CyberCoreMain.Log.Error("Was LOG ||"+"wwwwwwwwwwwwww > GOOD");
//                                                         break packetswitch;
//                                                     }
//                                                 }
//                                                 else if (inventory.getItemInHand().equals(useItemData.itemInHand))
//                                                 {
//                                                     Item i = inventory.getItemInHand();
//                                                     CyberCoreMain.Log.Error("Was LOG ||"+
//                                                         "wwwwwwwwwwwwww > GOOD " + i + "||" + i.getClass());
//                                                     Item oldItem = i.clone();
//                                                     if (i instanceof ItemBlock)
//                                                     CyberCoreMain.Log.Error("Was LOG ||"+"YYYYYYYYYYYYYYYYEEEEEEE");
//                                                     //TODO: Implement adventure mode checks
//                                                     if ((i = this.level.useItemOn(blockVector.asVector3(), i, face,
//                                                         useItemData.clickPos.x, useItemData.clickPos.y,
//                                                         useItemData.clickPos.z, this)) != null)
//                                                     {
//                                                         CyberCoreMain.Log.Error("Was LOG ||"+"wwwwwwwwwwwwww > GOOD2");
//                                                         if (!i.equals(oldItem) || i.getCount() != oldItem.getCount())
//                                                         {
//                                                             CyberCoreMain.Log.Error("Was LOG ||"+"wwwwwwwwwwwwww > GOOD3");
//                                                             inventory.setItemInHand(i);
//                                                             inventory.sendHeldItem(this.getViewers().values());
//                                                         }
//                                                     }
//                                                     else
//                                                     {
//                                                         if (i instanceof ItemBlock)
//                                                         CyberCoreMain.Log.Error("Was LOG ||"+"YYYYYYYYYYYYYYYYEEEEEE222222222222E");
//                                                         CyberCoreMain.Log.Error("Was LOG ||"+
//                                                             "ERRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRR" + i);
//                                                     }
//                                                 }
//
//                                                 break packetswitch;
//                                             }
//
//                                             inventory.sendHeldItem(this);
//
//                                             if (blockVector.distanceSquared(this) > 10000)
//                                             {
//                                                 break packetswitch;
//                                             }
//
//                                             Block target = this.level.getBlock(blockVector.asVector3());
//                                             Block block = target.getSide(face);
//
//                                             this.level.sendBlocks(new Player[] {this}, new[] {target, block},
//                                                 UpdateBlockPacket.FLAG_ALL_PRIORITY);
//
//                                             if (target instanceof BlockDoor) {
//                                             BlockDoor door = (BlockDoor) target;
//
//                                             Block part;
//
//                                             if ((door.getDamage() & 0x08) > 0)
//                                             {
//                                                 //up
//                                                 part = target.down();
//
//                                                 if (part.getId() == target.getId())
//                                                 {
//                                                     target = part;
//
//                                                     this.level.sendBlocks(new Player[] {this}, new[] {target},
//                                                         UpdateBlockPacket.FLAG_ALL_PRIORITY);
//                                                 }
//                                             }
//                                         }
//                                             break packetswitch;
//                                         case InventoryTransactionPacket.USE_ITEM_ACTION_BREAK_BLOCK:
//                                             if (!this.spawned || !this.isAlive())
//                                             {
//                                                 break packetswitch;
//                                             }
//
//                                             this.resetCraftingGridType();
//
//                                             Item i = this.getInventory().getItemInHand();
//
//                                             Item oldItem = i.clone();
//
//                                             if (this.canInteract(blockVector.add(0.5, 0.5, 0.5),
//                                                     this.isCreative() ? 13 : 7) &&
//                                                 (i = this.level.useBreakOn(blockVector.asVector3(), face, i, this,
//                                                     true)) != null)
//                                             {
//                                                 if (this.isSurvival())
//                                                 {
//                                                     getFoodData().updateFoodExpLevel(0.025);
//                                                     if (!i.equals(oldItem) || i.getCount() != oldItem.getCount())
//                                                     {
//                                                         inventory.setItemInHand(i);
//                                                         inventory.sendHeldItem(this.getViewers().values());
//                                                     }
//                                                 }
//
//                                                 break packetswitch;
//                                             }
//
//                                             inventory.sendContents(this);
//                                             target = this.level.getBlock(blockVector.asVector3());
//                                             BlockEntity blockEntity =
//                                                 this.level.getBlockEntity(blockVector.asVector3());
//
//                                             this.level.sendBlocks(new Player[] {this}, new[] {target},
//                                                 UpdateBlockPacket.FLAG_ALL_PRIORITY);
//
//                                             inventory.sendHeldItem(this);
//
//                                             if (blockEntity instanceof BlockEntitySpawnable) {
//                                             ((BlockEntitySpawnable) blockEntity).spawnTo(this);
//                                         }
//
//                                             break packetswitch;
//                                         case InventoryTransactionPacket.USE_ITEM_ACTION_CLICK_AIR:
//                                             Vector3 directionVector = this.getDirectionVector();
//
//                                             if (this.isCreative())
//                                             {
//                                                 item = this.inventory.getItemInHand();
//                                             }
//                                             else if (!this.inventory.getItemInHand().equals(useItemData.itemInHand))
//                                             {
//                                                 this.inventory.sendHeldItem(this);
//                                                 break packetswitch;
//                                             }
//                                             else
//                                             {
//                                                 item = this.inventory.getItemInHand();
//                                             }
//
//                                             var interactEvent = new PlayerInteractEvent(this, item, directionVector,
//                                                 face, PlayerInteractEvent.Action.RIGHT_CLICK_AIR);
//
//                                             this.server.getPluginManager().callEvent(interactEvent);
//
//                                             if (interactEvent.isCancelled())
//                                             {
//                                                 this.inventory.sendHeldItem(this);
//                                                 break packetswitch;
//                                             }
//
//                                             if (item.onClickAir(this, directionVector) && this.isSurvival())
//                                                 this.inventory.setItemInHand(item);
//
//                                             this.setDataFlag(DATA_FLAGS, DATA_FLAG_ACTION, true);
//                                             this.startAction = this.server.getTick();
//
//                                             break packetswitch;
//                                     }
//
//                                     break;
//                                 case InventoryTransactionPacket.TYPE_USE_ITEM_ON_ENTITY:
//                                     UseItemOnEntityData useItemOnEntityData =
//                                         (UseItemOnEntityData) transactionPacket2.transactionData;
//
//                                     Entity target = this.level.getEntity(useItemOnEntityData.entityRuntimeId);
//                                     if (target == null) return;
//
//                                     type = useItemOnEntityData.actionType;
//
//                                     if (!useItemOnEntityData.itemInHand.equalsExact(this.inventory.getItemInHand()))
//                                         this.inventory.sendHeldItem(this);
//
//                                     item = this.inventory.getItemInHand();
//
//                                     switch (type)
//                                     {
//                                         case InventoryTransactionPacket.USE_ITEM_ON_ENTITY_ACTION_INTERACT:
//                                             PlayerInteractEntityEvent playerInteractEntityEvent =
//                                                 new PlayerInteractEntityEvent(this, target, item);
//                                             if (this.isSpectator()) playerInteractEntityEvent.setCancelled();
//                                             getServer().getPluginManager().callEvent(playerInteractEntityEvent);
//
//                                             if (playerInteractEntityEvent.isCancelled()) break;
//
//                                             if (target.onInteract(this, item) && this.isSurvival())
//                                             {
//                                                 if (item.isTool())
//                                                 {
//                                                     if (item.useOn(target) &&
//                                                         item.getDamage() >= item.getMaxDurability())
//                                                         item = new ItemBlock(new BlockAir());
//                                                 }
//                                                 else
//                                                 {
//                                                     if (item.count > 1)
//                                                         item.count--;
//                                                     else
//                                                         item = new ItemBlock(new BlockAir());
//                                                 }
//
//                                                 this.inventory.setItemInHand(item);
//                                             }
//
//                                             break;
//                                         case InventoryTransactionPacket.USE_ITEM_ON_ENTITY_ACTION_ATTACK:
//                                             if (SwingCooldown.isValid())
//                                             {
//                                                 sendTip(ChatColors.Gray + "Class: Swing Cooldown");
// //                                            sendTitle(ChatColors.Gray + "Class: Swing Cooldown");
//                                                 break;
//                                             }
//
//                                             float itemDamage = item.getAttackDamage();
//
//                                             for (Enchantment enchantment :
//                                             item.getEnchantments()) {
//                                             itemDamage += enchantment.getDamageBonus(target);
//                                         }
//
//                                             Map<EntityDamageEvent.DamageModifier, float> damage =
//                                                 new EnumMap<>(EntityDamageEvent.DamageModifier.class);
//                                             damage.put(EntityDamageEvent.DamageModifier.BASE, itemDamage);
//
//                                             if (!this.canInteract(target, isCreative() ? 8 : 5))
//                                                 break;
//                                             else if (target
//                                             instanceof Player) {
//                                             if ((((Player) target).getGamemode() & 0x01) > 0)
//                                                 break;
//                                             if (!this.server.getPropertyBoolean("pvp") ||
//                                                 this.server.getDifficulty() == 0) break;
//                                         }
//
//                                             //TODO maybe custom???
//                                             //Call Custom 1st then Default
//
//                                             CustomEntityDamageByEntityEvent centityDamageByEntityEvent =
//                                                 new CustomEntityDamageByEntityEvent(this, target,
//                                                     CustomEntityDamageEvent.CustomDamageCause.ENTITY_ATTACK,
//                                                     itemDamage);
//                                             BaseClass bc = getPlayerClass();
//                                             if (bc != null) bc.HandelEvent(centityDamageByEntityEvent);
//                                             getServer().getPluginManager().callEvent(centityDamageByEntityEvent);
//                                             if (centityDamageByEntityEvent.isCancelled()) break;
//                                             EntityDamageByEntityEvent entityDamageByEntityEvent =
//                                                 new EntityDamageByEntityEvent(this, target,
//                                                     EntityDamageEvent.DamageCause.ENTITY_ATTACK, damage);
//                                             if (this.isSpectator()) entityDamageByEntityEvent.setCancelled();
//                                             if ((target instanceof Player) && !this.level.getGameRules()
//                                                 .getBoolean(GameRule.PVP)) {
//                                             entityDamageByEntityEvent.setCancelled();
//                                         }
//
//                                             if (!target.attack(entityDamageByEntityEvent))
//                                             {
//                                                 if (item.isTool() && this.isSurvival())
//                                                     this.inventory.sendContents(this);
//                                                 break;
//                                             }
//
//                                             //When you Successfully Attack Someone
//                                             enterCombat();
//                                             SwingCooldown = new CoolDown().setTimeTick(getAttackTime());
//
//                                             for (Enchantment enchantment :
//                                             item.getEnchantments()) {
//                                             enchantment.doPostAttack(this, target);
//                                         }
//
//                                             if (item.isTool() && this.isSurvival())
//                                             {
//                                                 if (item.useOn(target) && item.getDamage() >= item.getMaxDurability())
//                                                     this.inventory.setItemInHand(new ItemBlock(new BlockAir()));
//                                                 else
//                                                     this.inventory.setItemInHand(item);
//                                             }
//
//                                             return;
//                                     }
//
//                                     break;
//                                 case InventoryTransactionPacket.TYPE_RELEASE_ITEM:
//                                     if (this.isSpectator())
//                                     {
//                                         this.sendAllInventories();
//                                         break packetswitch;
//                                     }
//
//                                     ReleaseItemData releaseItemData =
//                                         (ReleaseItemData) transactionPacket2.transactionData;
//
//                                     try
//                                     {
//                                         type = releaseItemData.actionType;
//                                         switch (type)
//                                         {
//                                             case InventoryTransactionPacket.RELEASE_ITEM_ACTION_RELEASE:
//                                                 if (this.isUsingItem())
//                                                 {
//                                                     item = this.inventory.getItemInHand();
//                                                     if (item.onReleaseUsing(this)) this.inventory.setItemInHand(item);
//                                                 }
//                                                 else
//                                                 {
//                                                     this.inventory.sendContents(this);
//                                                 }
//
//                                                 return;
//                                             case InventoryTransactionPacket.RELEASE_ITEM_ACTION_CONSUME:
//                                                 Item itemInHand = this.inventory.getItemInHand();
//                                                 PlayerItemConsumeEvent consumeEvent =
//                                                     new PlayerItemConsumeEvent(this, itemInHand);
//
//                                                 if (itemInHand.getId() == Item.POTION)
//                                                 {
//                                                     this.server.getPluginManager().callEvent(consumeEvent);
//                                                     if (consumeEvent.isCancelled())
//                                                     {
//                                                         this.inventory.sendContents(this);
//                                                         break;
//                                                     }
//
//                                                     Potion potion = Potion.getPotion(itemInHand.getDamage())
//                                                         .setSplash(false);
//
//                                                     if (this.getGamemode() == SURVIVAL)
//                                                     {
//                                                         --itemInHand.count;
//                                                         this.inventory.setItemInHand(itemInHand);
//                                                         this.inventory.addItem(new ItemGlassBottle());
//                                                     }
//
//                                                     if (potion != null) potion.applyPotion(this);
//                                                 }
//                                                 else if (itemInHand.getId() == Item.BUCKET &&
//                                                          itemInHand.getDamage() == 1)
//                                                 {
//                                                     //milk
//                                                     this.server.getPluginManager().callEvent(consumeEvent);
//                                                     if (consumeEvent.isCancelled())
//                                                     {
//                                                         this.inventory.sendContents(this);
//                                                         break;
//                                                     }
//
//                                                     EntityEventPacket eventPacket = new EntityEventPacket();
//                                                     eventPacket.eid = this.getId();
//                                                     eventPacket.event = EntityEventPacket.USE_ITEM;
//                                                     this.dataPacket(eventPacket);
//                                                     Server.broadcastPacket(this.getViewers().values(), eventPacket);
//
//                                                     if (this.isSurvival())
//                                                     {
//                                                         itemInHand.count--;
//                                                         this.inventory.setItemInHand(itemInHand);
//                                                         this.inventory.addItem(new ItemBucket());
//                                                     }
//
//                                                     this.removeAllEffects();
//                                                 }
//                                                 else
//                                                 {
//                                                     this.server.getPluginManager().callEvent(consumeEvent);
//                                                     if (consumeEvent.isCancelled())
//                                                     {
//                                                         this.inventory.sendContents(this);
//                                                         break;
//                                                     }
//
//                                                     Food food = Food.getByRelative(itemInHand);
//                                                     if (food != null && food.eatenBy(this)) --itemInHand.count;
//                                                     this.inventory.setItemInHand(itemInHand);
//                                                 }
//
//                                                 return;
//                                         }
//                                     }
//
//                                     readonlyly {
//                                     this.setUsingItem(false);
//                                 }
//                                     break;
//                                 default:
//                                     this.inventory.sendContents(this);
//                                     break;
//                             }
//                         }
//                         catch (Exception e)
//                         {
//                             Server.GetInstance().getLogger().error("EEEEE123>>>>", e);
//                         }
//
//                         return;
//                     //Do not Pass GO!
//                     case ProtocolInfo.MOVE_PLAYER_PACKET:
//                         super.handleDataPacket(packet);
//                         if (this.TeleportVector3 != null) break;
//                         Item boots = getInventory().getBoots();
//                         if (boots == null) break;
//                         Spring se = (Spring) CustomEnchantment.getEnchantFromIDFromItem(boots,
//                             (short) CustomEnchantment.SPRING);
//                         Climber ce =
//                             (Climber) CustomEnchantment.getEnchantFromIDFromItem(boots,
//                                 (short) CustomEnchantment.CLIMBER);
//                         if (se == null && ce == null) break;
//                         var nm = new Vector3();
//                         if (se != null)
//                         {
//                             MovePlayerPacket movePlayerPacket = (MovePlayerPacket) packet;
//                             var newPos = new Vector3(movePlayerPacket.x, movePlayerPacket.y - this.getEyeHeight(),
//                                 movePlayerPacket.z);
//
//
//                             Vector3 dif = newPos.subtract(this);
//                             CheckGround();
//                             if (dif.getY() > 0 && !uw)
//                             {
//                                 //No Lvl_4 Boost!
//                                 if (0 < dif.x && dif.x > DEFAULT_SPEED) dif.x = DEFAULT_SPEED;
//                                 if (0 > dif.x && dif.x < -1 * DEFAULT_SPEED) dif.x = -1 * DEFAULT_SPEED;
//                                 if (0 > dif.z && dif.z > DEFAULT_SPEED) dif.z = DEFAULT_SPEED;
//                                 if (0 > dif.z && dif.z < -1 * DEFAULT_SPEED) dif.z = -1 * DEFAULT_SPEED;
//
//                                 SendMessage(dif.getY() + "!");
//
//                                 nm.add(dif.x * .25, DEFAULT_SPEED * se.GetLevelEffect(), dif.z * .25);
//                                 resetFallDistance();
//                                 uct = lastUpdate + 20;
//                                 uw = true;
// //                        upp++;
// //                        if (upp > 3) uw = true;
//                             }
//                             else if (onGround && uct > lastUpdate)
//                             {
// //                        if (upp > 0) upp--;
//                                 uw = false;
//                             }
//
//                             if (uw) inAirTicks = 0;
//                         }
//                         else if (ce != null)
//                         {
//                             MovePlayerPacket movePlayerPacket = (MovePlayerPacket) packet;
//                             var newPos = new Vector3(movePlayerPacket.x, movePlayerPacket.y - this.getEyeHeight(),
//                                 movePlayerPacket.z);
//
//
//                             Vector3 dif = newPos.subtract(this);
//                             CheckGround();
//                             if (dif.getY() > 0 && !uw)
//                             {
//                                 //No Lvl_4 Boost!
//                                 if (0 < dif.x && dif.x > DEFAULT_SPEED) dif.x = DEFAULT_SPEED;
//                                 if (0 > dif.x && dif.x < -1 * DEFAULT_SPEED) dif.x = -1 * DEFAULT_SPEED;
//                                 if (0 > dif.z && dif.z > DEFAULT_SPEED) dif.z = DEFAULT_SPEED;
//                                 if (0 > dif.z && dif.z < -1 * DEFAULT_SPEED) dif.z = -1 * DEFAULT_SPEED;
//
//                                 SendMessage(dif.getY() + "!");
//
//                                 nm.add(dif.x, DEFAULT_SPEED * ce.GetLevelEffect(), dif.z);
//                                 resetFallDistance();
//                                 uct = lastUpdate + 20;
//                                 uw = true;
// //                        upp++;
// //                        if (upp > 3) uw = true;
//                             }
//                             else if (onGround && uct > lastUpdate)
//                             {
// //                        if (upp > 0) upp--;
//                                 uw = false;
//                             }
//
//                             if (uw) inAirTicks = 0;
//                         }
//
// //                    if (upp == 0) uw = false;
//                         if (nm != null && !(nm.x != 0 && nm.y != 0 && nm.z != 0)) addMotion(nm.x, nm.y, nm.z);
//
//                         break;
//                     case ProtocolInfo.PLAYER_ACTION_PACKET:
//                         PlayerActionPacket playerActionPacket = (PlayerActionPacket) packet;
//                         if (!this.spawned || !this.isAlive() &&
//                             playerActionPacket.action != PlayerActionPacket.ACTION_RESPAWN &&
//                             playerActionPacket.action != PlayerActionPacket.ACTION_DIMENSION_CHANGE_REQUEST) return;
//
//                         playerActionPacket.entityId = this.id;
//                         var pos = new Vector3(playerActionPacket.x, playerActionPacket.y, playerActionPacket.z);
//                         BlockFace face = BlockFace.fromIndex(playerActionPacket.face);
//
//                         actionswitch:
//                         switch (playerActionPacket.action)
//                         {
//                             case PlayerActionPacket.ACTION_START_BREAK:
//                                 long currentBreak = System.currentTimeMillis();
//                                 BlockVector3 currentBreakVector3 = new BlockVector3(playerActionPacket.x,
//                                     playerActionPacket.y, playerActionPacket.z);
//                                 // HACK: Client spams multiple left clicks so we need to skip them.
//                                 if (lastBreakVector31.equals(currentBreakVector3) &&
//                                     currentBreak - this.lastBreak < 10 || pos.distanceSquared(this) > 100) return;
//                                 Block target = this.level.getBlock(pos);
//                                 var playerInteractEvent = new PlayerInteractEvent(this, this.inventory.getItemInHand(),
//                                     target, face,
//                                     target.getId() == 0
//                                         ? PlayerInteractEvent.Action.LEFT_CLICK_AIR
//                                         : PlayerInteractEvent.Action.LEFT_CLICK_BLOCK);
//                                 this.getServer().getPluginManager().callEvent(playerInteractEvent);
//                                 if (playerInteractEvent.isCancelled())
//                                 {
//                                     this.inventory.sendHeldItem(this);
//                                     return;
//                                 }
//
//                                 switch (target.getId())
//                                 {
//                                     case Block.NOTEBLOCK:
//                                         ((BlockNoteblock) target).emitSound();
//                                         break actionswitch;
//                                     case Block.DRAGON_EGG:
//                                         ((BlockDragonEgg) target).Teleport();
//                                         break actionswitch;
//                                 }
//
//                                 Block block = target.getSide(face);
//                                 if (block.getId() == Block.FIRE)
//                                 {
//                                     //Chance of getting caught on fire!
//                                     RandomChanceOfFire(77);
//                                     this.level.setBlock(block, new BlockAir(), true);
//                                     return;
//                                 }
//
//                                 if (!this.isCreative())
//                                 {
//                                     //improved this to take stuff like swimming, ladders, enchanted tools into account, fix wrong tool break time calculations for bad tools (pmmp/PocketMine-MP#211)
//                                     //Done by lmlstarqaq
//                                     double breakTime =
//                                         Math.ceil(target.getBreakTime(this.inventory.getItemInHand(), this) * 20);
//                                     if (PlayerClass != null)
//                                     {
//                                         var obreaktime = breakTime;
//                                         if (PlayerClass instanceof MineLifeClass &&
//                                             PlayerClass.TryRunPower(PowerEnum.MineLife)) {
//                                             object nbt = PlayerClass.RunPower(PowerEnum.MineLife,
//                                                 this.inventory.getItemInHand(), target, breakTime);
//                                             if (nbt != null)
//                                             {
//                                                 var nd = (double) nbt;
//                                                 if (nd > 0)
//                                                 {
//                                                     var dec = obreaktime - nd;
//                                                     var dp = dec / obreaktime * 100d;
//                                                     breakTime = nd;
//                                                     SendMessage("Break Time Redueced by " + dp);
//                                                 }
//                                             }
//                                         }
//                                     }
//
//                                     if (breakTime > 0)
//                                     {
//                                         LevelEventPacket pk = new LevelEventPacket();
//                                         pk.evid = LevelEventPacket.EVENT_BLOCK_START_BREAK;
//                                         pk.x = (float) pos.x;
//                                         pk.y = (float) pos.y;
//                                         pk.z = (float) pos.z;
//                                         pk.data = (int) (65535 / breakTime);
//                                         this.getLevel().addChunkPacket(pos.getFloorX() >> 4, pos.getFloorZ() >> 4, pk);
//                                     }
//                                 }
//
//                                 this.breakingBlock = target;
//                                 this.lastBreak = currentBreak;
//                                 lastBreakVector31 = currentBreakVector3;
//                                 break;
//                             case PlayerActionPacket.ACTION_JUMP:
//                                 SendMessage("JUMMMPPPPP!!!" + getDirection() + "|" + getMotion());
// //                            if (PlayerClass != null) PlayerClass.HandelEvent(new PlayerJumpEvent(this));
//                                 getServer().getPluginManager().callEvent(new PlayerJumpEvent(this));
// //                            addMovement(0,2.5,0,0,0,0);
// //                            switch (getDirection()) {
// //                                case NORTH:
// //                                    addMotion(motionX * 2, 3, 0);
// //                                    break;
// //                                case EAST:
// //                                    addMotion(0, 3, motionZ * 2);
// //                                    break;
// //                                case WEST:
// //                                    addMotion(motionX * -2, 3, 0);
// //                                    break;
// //                                case SOUTH:
// //                                    addMotion(0, 3, motionZ * -2);
// //                                    break;
// //
// //                            }
//                                 break;
//                         }
//                 }
//             }
//             if (packet.pid() == INVENTORY_TRANSACTION_PACKET) return;
//             super.handleDataPacket(packet);
//         }

        public void RandomChanceOfFire(int max)
        {
            Random nr = new Random(max);
            int f = nr.Next(0, 100);
            if (f < max) setOnFire(nr.Next(1, 4) * 20);
        }

        public void setOnFire(int ticks)
        {
            HealthManager.Ignite(ticks);
        }

//         public void ReloadScoreBoard()
//         {
//             Scoreboard s = PlayerScoreBoard;
//             if (s != null) s.hideFor(this);
//             s = ScoreboardAPI.createScoreboard();
//             ScoreboardDisplay sd = s.addDisplay(DisplaySlot.SIDEBAR, "PlayerInfo",
//                 ChatColors.AQUA + "~~| UnlimitedPE Factions |~~");
//             var k = 0;
//             if (!InternalPlayerSettings.isHudPosOff())
//             {
//                 sd.addLine(ChatColors.AQUA + "Positon:", k++);
//                 sd.addLine(
//                     "    " + ChatColors.GOLD + "X: " + ChatColors.Green + getFloorX() + ChatColors.GOLD + " Y: " +
//                     ChatColors.Green + getFloorY() + ChatColors.GOLD + " Z: " + ChatColors.Green + getFloorZ(), k++);
//             }
//
//             if (!InternalPlayerSettings.isHudFactionOff() && getFaction() != null)
//             {
//                 sd.addLine(ChatColors.Gray + "Faction : " + ChatColors.AQUA + getFaction().GetDisplayName(), k++);
//                 sd.addLine(
//                     "    " + ChatColors.AQUA + "XP" + ChatColors.Gray + " | " + ChatColors.Green +
//                     getFaction().GetXP() + ChatColors.AQUA + " / " + ChatColors.GOLD +
//                     getFaction().calculateRequireExperience() + ChatColors.Gray + " | " + ChatColors.Green + "Level: " +
//                     ChatColors.Yellow + getFaction().GetLevel(), k++);
//             }
//
//             if (!InternalPlayerSettings.isHudClassOff())
//             {
// //            TODO
//                 BaseClass bc = getPlayerClass();
//                 if (bc != null)
//                 {
//                     List<string> t = bc.FormatHudText();
//                     if (t != null && t.size() != 0)
//                     {
//                         sd.addLine("Class:", k++);
//                         for (string ss :
//                         t) sd.addLine("    " + ss, k++);
//                     }
//                     else
//                     {
//                         sd.addLine("Class: None", k++);
//                     }
//                 }
//             }
//
//
// //        if(!InternalPlayerSettings.isHudClassOff())sd.addLine(ChatColors.GOLD+"X: "+ChatColors.Green+getX()+ChatColors.GOLD+"Y: "+ChatColors.Green+getY()+ChatColors.GOLD+"Z: "+ChatColors.Green+getZ(),k++);
//             sd.addLine(ChatColors.Yellow + "Money : " + ChatColors.AQUA + "$" + getMoney(), k++);
//             sd.addLine(ChatColors.Yellow + "Hunger : " + ChatColors.AQUA + getFoodData().getLevel(), k++);
//             sd.addLine(ChatColors.Yellow + "HP : " + ChatColors.AQUA + getHealth() + " / " + getMaxHealth(), k++);
//             sd.addLine(ChatColors.Gray + "Class & Power COOLDOWNS:", k++);
//             sd.addEntity(this, k++);
// //        sd.addLine("TEST LINE 1",1);
// //        sd.addLine("YOUR NAME"+p.getDisplayName(),2);
// //        }else{
// //            ScoreboardDisplay sd = s.addDisplay();
// //            sd.addEntity(this,10);
// //
// //        }
//             PlayerScoreBoard = s;
//             ScoreboardAPI.setScoreboard(this, s);
//         }

      
        public void leaveCombat()
        {
            Combat = null;
            SendMessage(ChatColors.Green + "You are now out of Combat!");
        }


        protected override void OnPlayerJoining(PlayerEventArgs e)
        {
            base.OnPlayerJoining(e);
            Console.WriteLine("YDEWASDASDASDAS DAS DAS DADSs");
            base.OnPlayerJoining(e);
            // if (EPD == null) loadEPD();
        }


        // public List<Enchantment> GetStoredEnchants()
        // {
        //     return MasterEnchantigList;
        // }

        // public List<Enchantment> GetStoredEnchants(CustomEnchantment.Tier tier, int i, Item item)
        // {
        //     if (MasterEnchantigList == null) MasterEnchantigList = CustomEnchantment.GetRandomEnchant(tier, 3, item);
        //     return MasterEnchantigList;
        // }

        public void sendPopup(String msg, long duration = 40l)
        {
            var p = new Popup()
            {
                Duration = duration,
                MessageType = MessageType.Tip,
                Message = msg
            };
            AddPopup(p);
        }



        public override void HandleMcpeResourcePackClientResponse(McpeResourcePackClientResponse message)
        {
            Console.WriteLine("HEEETT=================================TTTT");
            if (message.responseStatus == 4)
            {
                Console.WriteLine("HEEETTTTTT222222222222");
                Console.WriteLine("HEEETTTTTT");
                Console.WriteLine("HEEETTTTTT");
                Console.WriteLine("HEEETTTTTT2222222");
                OpenServer.FastThreadPool.QueueUserWorkItem(() => { Start(null); });
                Console.WriteLine("HEEETTTTTT");
                return;
            }
            else
            {
                base.HandleMcpeResourcePackClientResponse(message);
            }
        }


        public void StartTeleport(CorePlayer pl, int delay)
        {
            pl.BeginTeleportEffects(this, delay);
        }

        public void StartTeleport(CorePlayer pl)
        {
            StartTeleport(pl, 3);
        }

        public void StartTeleport(Vector3 pl, int delay)
        {
            BeginTeleportEffects(pl, delay);
        }

        public void StartTeleport(Vector3 pl)
        {
            StartTeleport(pl, 3);
        }

        private void BeginTeleportEffects(CorePlayer corePlayer)
        {
            BeginTeleportEffects(corePlayer, 3);
        }

        private void BeginTeleportEffects(CorePlayer corePlayer, int delay)
        {
            BeginTeleportEffects(corePlayer.KnownPosition, delay);
        }

        private void BeginTeleportEffects(Vector3 pos, int delay)
        {
            var e1 = new Nausea();
            var e2 = new Slowness();

            e1.Level = (2);
            e2.Level = (2);
            e1.Duration = (20 * 600);
            e2.Duration = (20 * 600);

            SetEffect(e1);
            SetEffect(e2);

            // isTeleporting = true;
            // TeleportTick = CyberUtils.getTick() + 20 * delay;
            // TargetTeleporting = null;
            // TargetTeleportingLoc = pos;
        }
        //
        // public void ClearFactionInvite()
        // {
        //     ClearFactionInvite(false);
        // }


//
//        int tickDiff = currentTick - this.lastUpdate;
//
//        if (tickDiff <= 0) {
//            return true;
//        }
//
//        this.messageCounter = 2;
//
//        this.lastUpdate = currentTick;
//
//        if (!this.isAlive() && this.spawned) {
//            ++this.deadTicks;
//            if (this.deadTicks >= 10) {
//                this.despawnFromAll();
//            }
//            return true;
//        }
//
//        if (this.spawned) {
//            this.processMovement(tickDiff);
//
//            this.entityBaseTick(tickDiff);
//
//            if (this.getServer().getDifficulty() == 0 && this.level.getGameRules().getBoolean(GameRule.NATURAL_REGENERATION)) {
//                if (this.getHealth() < this.getMaxHealth() && this.ticksLived % 20 == 0) {
//                    this.heal(1);
//                }
//
//                PlayerFood foodData = this.getFoodData();
//
//                if (foodData.getLevel() < 20 && this.ticksLived % 10 == 0) {
//                    foodData.addFoodLevel(1, 0);
//                }
//            }
//
//            if (this.isOnFire() && this.lastUpdate % 10 == 0) {
//                if (this.isCreative() && !this.isInsideOfFire()) {
//                    this.extinguish();
//                } else if (this.getLevel().isRaining()) {
//                    if (this.getLevel().canBlockSeeSky(this)) {
//                        this.extinguish();
//                    }
//                }
//            }
//
//            if (!this.isSpectator() && this.speed != null) {
//                if (this.onGround) {
//                    if (this.inAirTicks != 0) {
//                        this.startAirTicks = 5;
//                    }
//                    this.inAirTicks = 0;
//                    this.highestVector3 = this.y;
//                } else {
//                    if (!this.isGliding() && !server.getAllowFlight() && !this.getAdventureSettings().get(AdventureSettings.Type.ALLOW_FLIGHT) && this.inAirTicks > 10 && !this.isSleeping() && !this.isImmobile() && uw) {
//                        double expectedVelocity = (-this.getGravity()) / ((double) this.getDrag()) - ((-this.getGravity()) / ((double) this.getDrag())) * Math.exp(-((double) this.getDrag()) * ((double) (this.inAirTicks - this.startAirTicks)));
//                        double diff = (this.speed.y - expectedVelocity) * (this.speed.y - expectedVelocity);
//
//                        Block block = level.getBlock(this);
//                        bool onLadder = block.getId() == BlockID.LADDER;
//
//                        if (!this.hasEffect(Effect.JUMP) && diff > 0.6 && expectedVelocity < this.speed.y && !onLadder) {
//                            if (this.inAirTicks < 100) {
//                                //this.sendSettings();
//                                this.setMotion(new Vector3(0, expectedVelocity, 0));
//                            } else if (this.kick(PlayerKickEvent.Reason.FLYING_DISABLED, "Flying is not enabled on this server")) {
//                                return false;
//                            }
//                        }
//                        if (onLadder) {
//                            this.resetFallDistance();
//                        }
//                    }
//
//                    if (this.y > highestVector3) {
//                        this.highestVector3 = this.y;
//                    }
//
//                    if (this.isGliding()) this.resetFallDistance();
//
//                    ++this.inAirTicks;
//
//                }
//
//                if (this.isSurvival() || this.isAdventure()) {
//                    if (this.getFoodData() != null) this.getFoodData().update(tickDiff);
//                }
//            }
//        }
//
//        this.checkTeleportVector3();
//        this.checkInteractNearby();
//
//        if (this.spawned && this.dummyBossBars.size() > 0 && currentTick % 100 == 0) {
//            this.dummyBossBars.values().forEach(DummyBossBar::updateBossEntityVector3);
//        }
//
//        return true;
//    }


        public HungerManager getFoodData()
        {
            return HungerManager;
        }


      
//        if (!this.server.isWhitelisted((this.getName()).toLowerCase())) {
//            this.kick(PlayerKickEvent.Reason.NOT_WHITELISTED, "Server is white-listed");
//
//            return;
//        } else if (this.isBanned()) {
//            this.kick(PlayerKickEvent.Reason.NAME_BANNED, "You are banned");
//            return;
//        } else if (this.server.getIPBans().isBanned(this.getAddress())) {
//            this.kick(PlayerKickEvent.Reason.IP_BANNED, "You are banned");
//            return;
//        }
//
//        if (this.hasPermission(Server.BROADCAST_CHANNEL_USERS)) {
//            this.server.getPluginManager().subscribeToPermission(Server.BROADCAST_CHANNEL_USERS, this);
//        }
//        if (this.hasPermission(Server.BROADCAST_CHANNEL_ADMINISTRATIVE)) {
//            this.server.getPluginManager().subscribeToPermission(Server.BROADCAST_CHANNEL_ADMINISTRATIVE, this);
//        }
//
//        for (Player p : new List<>(this.server.getOnlinePlayers().values())) {
//            if (p != this && p.getName() != null && p.getName().equalsIgnoreCase(this.getName())) {
//                if (!p.kick(PlayerKickEvent.Reason.NEW_CONNECTION, "logged in from another location")) {
//                    this.close(this.getLeaveMessage(), "Already connected");
//                    return;
//                }
//            } else if (p.loggedIn && this.getUniqueId().equals(p.getUniqueId())) {
//                if (!p.kick(PlayerKickEvent.Reason.NEW_CONNECTION, "logged in from another location")) {
//                    this.close(this.getLeaveMessage(), "Already connected");
//                    return;
//                }
//            }
//        }
//
//        CompoundTag nbt;
//        File legacyDataFile = new File(server.getDataPath() + "players/" + this.username.toLowerCase() + ".dat");
//        File dataFile = new File(server.getDataPath() + "players/" + this.uuid.toString() + ".dat");
//        if (legacyDataFile.exists() && !dataFile.exists()) {
//            nbt = this.server.getOfflinePlayerData(this.username);
//
//            if (!legacyDataFile.delete()) {
//                log.warn("Could not delete legacy player data for {}", this.username);
//            }
//        } else {
//            nbt = this.server.getOfflinePlayerData(this.uuid);
//        }
//
//        if (nbt == null) {
//            this.close(this.getLeaveMessage(), "Invalid data");
//            return;
//        }
//
//        if (loginChainData.isXboxAuthed() && server.getPropertyBoolean("xbox-auth") || !server.getPropertyBoolean("xbox-auth")) {
//            server.updateName(this.uuid, this.username);
//        }
//
//        this.playedBefore = (nbt.getLong("lastPlayed") - nbt.getLong("firstPlayed")) > 1;
//
//        bool alive = true;
//
//        nbt.putString("NameTag", this.username);
//
//        if (0 >= nbt.getShort("Health")) {
//            alive = false;
//        }
//
//        int exp = nbt.getInt("EXP");
//        int expLevel = nbt.getInt("expLevel");
//        this.setExperience(exp, expLevel);
//
//        this.gamemode = nbt.getInt("playerGameType") & 0x03;
//        if (this.server.getForceGamemode()) {
//            this.gamemode = this.server.getGamemode();
//            nbt.putInt("playerGameType", this.gamemode);
//        }
//
//        this.adventureSettings = new AdventureSettings(this)
//                .set(AdventureSettings.Type.WORLD_IMMUTABLE, isAdventure())
//                .set(AdventureSettings.Type.WORLD_BUILDER, !isAdventure())
//                .set(AdventureSettings.Type.AUTO_JUMP, true)
//                .set(AdventureSettings.Type.ALLOW_FLIGHT, isCreative())
//                .set(AdventureSettings.Type.NO_CLIP, isSpectator());
//
//        Level level;
//        if ((level = this.server.getLevelByName(nbt.getString("Level"))) == null || !alive) {
//            this.setLevel(this.server.getDefaultLevel());
//            nbt.putString("Level", this.level.getName());
//            nbt.getList("Pos", DoubleTag.class)
//                    .add(new DoubleTag("0", this.level.getSpawnLocation().x))
//                    .add(new DoubleTag("1", this.level.getSpawnLocation().y))
//                    .add(new DoubleTag("2", this.level.getSpawnLocation().z));
//        } else {
//            this.setLevel(level);
//        }
//
//        for (Tag achievement : nbt.getCompound("Achievements").getAllTags()) {
//            if (!(achievement instanceof ByteTag)) {
//                continue;
//            }
//
//            if (((ByteTag) achievement).getData() > 0) {
//                this.achievements.add(achievement.getName());
//            }
//        }
//
//        nbt.putLong("lastPlayed", System.currentTimeMillis() / 1000);
//
//        UUID uuid = getUniqueId();
//        nbt.putLong("UUIDLeast", uuid.getLeastSignificantBits());
//        nbt.putLong("UUIDMost", uuid.getMostSignificantBits());
//
//        if (this.server.getAutoSave()) {
//            this.server.saveOfflinePlayerData(this.uuid, nbt, true);
//        }
//
//        this.sendPlayStatus(PlayStatusPacket.LOGIN_SUCCESS);
//        this.server.onPlayerLogin(this);
//
//        ListTag<DoubleTag> posList = nbt.getList("Pos", DoubleTag.class);
//
//        super.init(this.level.getChunk((int) posList.get(0).data >> 4, (int) posList.get(2).data >> 4, true), nbt);
//
//        if (!this.namedTag.contains("foodLevel")) {
//            this.namedTag.putInt("foodLevel", 20);
//        }
//        int foodLevel = this.namedTag.getInt("foodLevel");
//        if (!this.namedTag.contains("FoodSaturationLevel")) {
//            this.namedTag.putfloat("FoodSaturationLevel", 20);
//        }
//        float foodSaturationLevel = this.namedTag.getfloat("foodSaturationLevel");
//        this.foodData = new PlayerFood(this, foodLevel, foodSaturationLevel);
//
//        if (this.isSpectator()) this.keepMovement = true;
//
//        this.forceMovement = this.TeleportVector3 = this.getVector3();
//
//        ResourcePacksInfoPacket infoPacket = new ResourcePacksInfoPacket();
//        infoPacket.resourcePackEntries = this.server.getResourcePackManager().getResourceStack();
//        infoPacket.mustAccept = this.server.getForceResources();
//        this.dataPacket(infoPacket);
//    }

        public class CombatData
        {
            public readonly int CombatTime = 20 * 7; // 7 Secs
            public long Tick = -1;

            public CombatData(long tick)
            {
                Tick = tick;
            }

            public long getTick(int a)
            {
                return Tick + a;
            }

            public long getTick()
            {
                return getTick(CombatTime);
            }
        }
    }
}