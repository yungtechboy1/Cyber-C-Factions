using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Numerics;
using System.Runtime.InteropServices.WindowsRuntime;
using CyberCore.Custom.Events;
using CyberCore.CustomEnums;
using CyberCore.Manager.AuctionHouse;
using CyberCore.Manager.ClassFactory;
using CyberCore.Manager.ClassFactory.Powers;
using CyberCore.Manager.Factions;
using CyberCore.Manager.Factions.Windows;
using CyberCore.Manager.Forms;
using CyberCore.Manager.Rank;
using CyberCore.Manager.Shop;
using CyberCore.Manager.Shop.Spawner;
using CyberCore.Utils;
using CyberCore.Utils.Cooldowns;
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
using static CyberCore.Manager.ClassFactory.BuffType;

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
        public ExtraPlayerData EPD = null;
        public AuctionHouse AH = null;
        public int banned = 0;

        private readonly int BaseSwingSpeed = 7; //Was 10 but felt too slow for good PVP

        // private CustomCraftingTransaction cct;
        private readonly Dictionary<BuffOrigin, Dictionary<BuffType, Buff>> Bufflist =
            new Dictionary<BuffOrigin, Dictionary<BuffType, Buff>>
            {
                {BuffOrigin.Class, new Dictionary<BuffType, Buff>()}
            };

        public bool BuffsChanced = false;

        private readonly Dictionary<string, CoolDown> CDL = new Dictionary<string, CoolDown>();

        private BaseClass PlayerClass = null;
        private int ClassCheck = -1;
        public CombatData Combat;
        private PlayerLocation CTLastPos;
        public int CustomExtraHP;
        public float CustomMovementSpeed = 0.1f;
        public int deaths;

        private readonly Dictionary<BuffOrigin, Dictionary<BuffType, DeBuff>> DeBufflist =
            new Dictionary<BuffOrigin, Dictionary<BuffType, DeBuff>>
            {
                {BuffOrigin.Class, new Dictionary<BuffType, DeBuff>()}
            };

        public bool DebuffsChanced = false;

        //    public String faction_id = null;
        public Dictionary<string, object> extraData = new Dictionary<string, object>();
        public string Faction;

        private int FactionCheck = -1;

        // public string FactionInvite;
        // public int FactionInviteTimeout = -1;
        public int fixcoins = 0;

        public PlayerFactionSettings fsettings = new PlayerFactionSettings();

        // public List<Enchantment> MasterEnchantigList = null;
        public List<Faction.HomeData> HD = new List<Faction.HomeData>();

        public CoreSettings InternalPlayerSettings = new CoreSettings();
        // private bool isInTeleportingProcess = false;

        // private bool isTeleporting;

        private Form nw;
        private Item ItemBeingEnchanted;
        private bool ItemBeingEnchantedLock;
        public int kills;
        private Vector3 lastBreakVector31;
        public string LastMessageSentTo = null;
        public MainForm LastSentFormType = MainForm.NULL;
        public SubMenu LastSentSubMenu = SubMenu.NULL;
        public int MaxHomes = 5;

        /**
     * @
     */
        public int money = 0;

        public bool MuteMessage = false;
        private Rank2 rank = RankList2.getInstance().getRankFromID(RankEnum.Guest);

        private PlayerSettingsData SettingsData;

        // public ShopInv Shop = null;
        public SpawnerShop SpawnerShop = null;

        private CoolDown SwingCooldown = new CoolDown();

        // private CorePlayer TargetTeleporting;
        // private PlayerLocation TargetTeleportingLoc;
        // private int TeleportTick;
        public string TPR;

        public int TPRTimeout;

        // public Scoreboard PlayerScoreBoard = ScoreboardAPI.createScoreboard();
        protected Dictionary<BuffType, float> lastdata = null;
        private long uct;
        private bool uw;
        private long WFTP_TPTick = -1;

        public int WFTP_CancelDistance = 2;
        public bool WFTP_Effects;
        private PlayerLocation WFTP_ToPlayerLocation;
        private PlayerLocation WFTP_StartPos;

        public CorePlayer(MiNetServer server, IPEndPoint endPoint, OpenApi api) : base(server, endPoint, api)
        {
            ItemStackInventoryManager = new CustomItmStkInvMgr(this);
        }
        //
        // private void InvChange(Player player, Inventory inventory, byte slot, Item itemStack)
        // {
        //
        // }
        //

        // public override void HandleMcpeContainerClose(McpeContainerClose message)
        // {
        //  Console.WriteLine("CLOSE WAS CALLED");
        //     base.HandleMcpeContainerClose(message);
        //     
        //     // _openInventory = 
        // }

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
        public CustomInvType CustomInvOpen { get; set; } = CustomInvType.NA;

        public NewShopInv ShopInv = null;

        public enum CustomInvType
        {
            NA,
            Shop,
            AH,
            SpawnerShop
        }

        public Dictionary<BuffOrigin, Dictionary<BuffType, Buff>> getBufflist()
        {
            return new Dictionary<BuffOrigin, Dictionary<BuffType, Buff>>(Bufflist);
        }

        public Dictionary<BuffOrigin, Dictionary<BuffType, DeBuff>> getDeBufflist()
        {
            return new Dictionary<BuffOrigin, Dictionary<BuffType, DeBuff>>(DeBufflist);
        }

        public Dictionary<BuffType, DeBuff> getClassDeBuffList()
        {
            if (!getBufflist().ContainsKey(BuffOrigin.Class)) return new Dictionary<BuffType, DeBuff>();
            return new Dictionary<BuffType, DeBuff>(getDeBufflist()[BuffOrigin.Class]);
        }

        //     public class DeathEventArgs
        //     {
        //         public CorePlayer Player { get;}
        //         public Level Level { get;}
        // // public EntityDamageEvent
        //         public DamageCause Cause {get;}
        //         
        //         
        //     }
        //     
        //     public event EventHandler<PlayerEventArgs> DamageEventHandler;
        //
        //     protected virtual void onDamage(PlayerEventArgs e)
        //     {
        //         DamageEventHandler?.Invoke(this, e);
        //     }
        //     public event EventHandler<PlayerEventArgs> MoveEventHandler;
        //
        //     protected virtual void onMove(PlayerEventArgs e)
        //     {
        //         MoveEventHandler?.Invoke(this, e);
        //     }

        public Dictionary<BuffType, Buff> getClassBuffList()
        {
            if (!getBufflist().ContainsKey(BuffOrigin.Class)) return new Dictionary<BuffType, Buff>();
            return new Dictionary<BuffType, Buff>(getBufflist()[BuffOrigin.Class]);
        }

        public Dictionary<BuffType, Buff> getTempBuff()
        {
            if (!getBufflist().ContainsKey(BuffOrigin.Temp)) return new Dictionary<BuffType, Buff>();
            return new Dictionary<BuffType, Buff>(getBufflist()[BuffOrigin.Temp]);
        }

        public override void HandleMcpeContainerClose(McpeContainerClose message)
        {
            if (CustomInvOpen != CustomInvType.NA)
            {
                CustomInvOpen = CustomInvType.NA;
                SendMessage("CUSTOM INV HAS BEEN CLOSED INTERNALLY");
                McpeContainerClose mcpeContainerClose = Packet<McpeContainerClose>.CreateObject(1L);
                mcpeContainerClose.windowId = 10;
                this.SendPacket((Packet) mcpeContainerClose);
                
                // McpeBlockEvent message1 = Packet<McpeBlockEvent>.CreateObject(1L);
                // message1.coordinates = openInventory.Coordinates;
                // message1.case1 = 1;
                // message1.case2 = 0;
                // this.Level.RelayBroadcast<McpeBlockEvent>(message1);
            }
            else
                base.HandleMcpeContainerClose(message);
        }


        public override void HandleMcpeInventoryTransaction(McpeInventoryTransaction message)
        {
            Console.WriteLine("CALLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLL");

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
                    if (CustomInvOpen != CustomInvType.NA)
                    {
                        Console.WriteLine("NT");
                        HandleTransactionRecords2(transaction.TransactionRecords);
                        return;
                    }
                    else break;
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

        public void HandleTransactionRecords2(List<TransactionRecord> transactionRecords)
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
                        Console.WriteLine(inventoryId + "<<<<<");
                        if (inventoryId == 10)
                        {
                            if (CustomInvOpen == CustomInvType.Shop)
                            {
                                ClearCursor();
                                ShopInv?.MakeSelection(slot1, this);
                            }
                        }
                        else if (inventoryId == 124)
                        {
                            Inventory.UiInventory.Cursor = new ItemAir();
                            SendPlayerInventory();
                            ClearCursor();
                        }

                        break;
                }
            }
        }

        public void ClearCursor()
        {
            Inventory.UiInventory /*?*/.Cursor = new ItemAir();
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

        //    private static String TempKey = "TEMPBuffs";
        public void addTemporaryBuff(Buff b)
        {
            if (Bufflist.ContainsKey(BuffOrigin.Temp))
            {
                var o = Bufflist[BuffOrigin.Temp];
                o[b.getBt()] = b;
                Bufflist[BuffOrigin.Temp] = o;
            }
            else
            {
                //TODO DECIDE SHOULD WE ALLOW THIS TO BE WRITEN OVER WHEN THEIR IS ALREADY A TEMP BUFF SET FOR A BUFF TYPE.... ie I CANT OVER WRITE MOVEMENT BUFF

                Bufflist[BuffOrigin.Temp] = new Dictionary<BuffType, Buff>
                {
                    {b.getBt(), b}
                };
            }
        }

        public void delTemporbparyBuff(BuffType b)
        {
            if (Bufflist.ContainsKey(BuffOrigin.Temp))
            {
                var o = Bufflist[BuffOrigin.Temp];
                o.Remove(b);
            }
        }

        //TODO
        public void clearClassBuffs()
        {
        }

        public void initAllBuffs()
        {
            initBuffs();
        }

        //Used by BaseClass to update players effects and buffs, Debuffs
        public void initAllClassBuffs()
        {
//        initClassDeBuffs();
//        initClassBuffs();
            initAllBuffs();
        }

        public void initClassDeBuffs()
        {
            //Class
            foreach (var b in getClassDeBuffList().Values)
                switch (b.getBt())
                {
                    case Movement:
                        CustomMovementSpeed = DEFAULT_SPEED / b.getAmount();
                        setMovementSpeed(CustomMovementSpeed, true);
                        break;
//                case SwingSpeed: NO NEED
                    //COPY
//                case SwingSpeed:
//                    CustomMovementSpeed= (DEFAULT_SPEED / b.getAmount());
//                    setMovementSpeed(CustomMovementSpeed,true);
//                    break;
                }
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

        public void initClassBuffs()
        {
            foreach (var b in getClassBuffList().Values)
            {
                switch (b.getBt())
                {
                    case Movement:
                        CustomMovementSpeed = DEFAULT_SPEED * b.getAmount();
                        setMovementSpeed(CustomMovementSpeed, true);
                        break;
                    case Health:
                        CustomExtraHP = (int) b.getAmount();
                        setMaxHealth(20 + CustomExtraHP);
                        sendAttributes();
                        break;
                }
            }
        }

        public void initBuffs()
        {
            Console.WriteLine("Runing INITBUFFS BUFFFFFFFFFFFFFF");
            Dictionary<BuffType, float> data = new Dictionary<BuffType, float>();
            //BUFFS
            List<Buff> ab = new List<Buff>(getClassBuffList().Values);
//        ab.addAll(getTempBuff().values());
            foreach (var b in getClassBuffList().Values)
            {
                data[b.getBt()] = b.getAmount();
            }

            foreach (var b in getClassDeBuffList().Values)
            {
                if (data.ContainsKey(b.getBt()))
                {
                    float f = data[b.getBt()];
                    data[b.getBt()] = f / b.getAmount();
                }
                else
                {
                    data[b.getBt()] = 1 / b.getAmount();
                }
            }

            //Temp Buffs  Everything!
            if (getTempBuff().Count > 0)
            {
                Console.WriteLine("HAS TEMPPPPPPPPP BUFFFFFFFFFFFFFF");
                foreach (var b in getTempBuff().Values)
                {
                    data[b.getBt()] = b.getAmount();
                }
            }

            if (!data.ContainsKey(Health)) CustomExtraHP = 0;
            if (!data.ContainsKey(Movement)) setMovementSpeed(DEFAULT_SPEED, true);
            if (!areequal(data, lastdata))
            {
                foreach (var v in data)
                {
                    var key = v.Key;
                    var value = v.Value;
                    switch (key)
                    {
                        case Movement:
                            setMovementSpeed(DEFAULT_SPEED * value, true);
                            break;
                        case NULL:
                            break;
                        case Health:
                            CustomExtraHP = (int) value + 0;
                            setMaxHealth(20 + CustomExtraHP);
                            sendAttributes();
                            break;
                        case Armor:
                            break;
                        case DamageFromPlayer:
                            break;
                        case DamageToPlayer:
                            break;
                        case DamageToEntity:
                            break;
                        case DamageFromEntity:
                            break;
                        case Damage:
                            break;
                        case SwingSpeed:
                            break;
                        case Reach:
                            break;
                        case Healing:
                            break;
                        case SuperFoodHeartRegin:
                            break;
                        case Magic:
                            break;
                        case Jump:
                            break;
                    }
                }

                ;
                lastdata = data;
            }
        }

        private bool areequal(Dictionary<BuffType, float> a, Dictionary<BuffType, float> b)
        {
            if (a == null || b == null || a.Count != b.Count) return false;
            foreach (var entry in a)
            {
                BuffType k = entry.Key;
                float v = entry.Value;
                if (!b.ContainsKey(k) || !v.Equals(b[k])) return false;
            }

            return true;
        }


        public float getHealth()
        {
            return HealthManager.Health;
        }


        public int getMaxHealth()
        {
            return 20 + (this.hasEffect(EffectType.HealthBoost)
                ? 4 * (this.getEffect(EffectType.HealthBoost).Level + 1)
                : 0) + CustomExtraHP;
        }

        public void addBuffFromClass(Buff b)
        {
            addBuffFromClass(b, false);
        }

        public void addBuffFromClass(Buff b, bool force)
        {
            if (b == null) return;
            if (!Bufflist.ContainsKey(BuffOrigin.Class) || Bufflist[BuffOrigin.Class] == null)
            {
                var hash = new Dictionary<BuffType, Buff>();
                hash[b.getBt()] = b;
                Bufflist[BuffOrigin.Class] = hash;
            }
            else
            {
                Dictionary<BuffType, Buff> bm = Bufflist[BuffOrigin.Class];
                if (bm.ContainsKey(b.getBt()) && !force) return; //Cant write it something is here
                bm[b.getBt()] = b;
                Bufflist[BuffOrigin.Class] = bm;
            }
        }

        public void addDeBuffFromClass(DeBuff b)
        {
            addDeBuffFromClass(b, false);
        }

        public void addDeBuffFromClass(DeBuff b, bool force)
        {
            if (b == null) return;
            if (!DeBufflist.ContainsKey(BuffOrigin.Class) || DeBufflist[BuffOrigin.Class] == null)
            {
                var hash = new Dictionary<BuffType, DeBuff>();
                hash[b.getBt()] = b;
                DeBufflist[BuffOrigin.Class] = hash;
            }
            else
            {
                Dictionary<BuffType, DeBuff> bm = DeBufflist[BuffOrigin.Class];
                if (bm.ContainsKey(b.getBt()) && !force) return; //Cant write it something is here
                bm[b.getBt()] = b;
                DeBufflist[BuffOrigin.Class] = bm;
            }
        }

        //for testing
//    @
//    public int dataPacket(DataPacket packet, bool needACK) {
//        if (!this.connected) {
//            return -1;
//        }
//
//        if(needACK)return super.dataPacket(packet,needACK);
//
////        CyberCoreMain.Log.Error("Was LOG ||"+"Sending >> "+packet);
//
//        try (Timing timing = Timings.getSendDataPacketTiming(packet)) {
//            DataPacketSendEvent ev = new DataPacketSendEvent(this, packet);
//            this.server.getPluginManager().callEvent(ev);
//            if (ev.isCancelled()) {
//                return -1;
//            }
//
//            int identifier = this.interfaz.putPacket(this, packet, needACK, false);
//
////            if (needACK && identifier != null) {
////                this.notifyACK();
////                this.needACK.put(identifier, Boolean.FALSE);
////                return identifier;
////            }
//        }
//        return 0;
//    }

        public void SetPlayerClass(BaseClass bc)
        {
            PlayerClass = bc;
            if (PlayerClass != null) PlayerClass.initBuffs();
        }

        public BaseClass getPlayerClass()
        {
            return PlayerClass;
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


        public override void Disconnect(string reason, bool sendDisconnect = true)
        {
            base.Disconnect(reason, sendDisconnect);
            //TODO Save Player Inv
            CyberCoreMain.GetInstance().ServerSQL.UnLoadPlayer(this);
        }

        public void CreateDefaultSettingsData(CorePlayer p)
        {
            var a = new PlayerSettingsData(p);
            setSettingsData(a);
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

        public bool canMakeTransaction(double price)
        {
            if (price > getMoney()) return false;
//        TakeMoney(price);
            return true;
        }

        public bool MakeTransaction(double price)
        {
            if (price > getMoney()) return false;
            TakeMoney(price);
            return true;
        }


        // public float getMovementSpeed()
        // {
        //     return super.getMovementSpeed();
        // }

        public bool TakeMoney(double price)
        {
            if (price <= 0) return false;
            return getPlayerSettingsData().takeCash(price);
        }

        public void AddMoney(double price)
        {
            if (price <= 0) return;
            getPlayerSettingsData().addCash(price);
        }

        public double getMoney()
        {
            return getPlayerSettingsData().getCash();
        }


        public void SetRank(RankEnum r)
        {
            rank = RankList2.getInstance().getRankFromID(r);
        }

        public void SetRank(Rank2 r)
        {
            rank = r;
        }

        public Rank2 GetRank()
        {
            return rank;
        }

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


        public void enterCombat()
        {
            if (Combat == null) SendMessage(ChatColors.Yellow + "You are now in combat!");
            Combat = new CombatData(CyberUtils.getTick());
        }

        protected override bool AcceptPlayerMove(McpeMovePlayer message, bool isOnGround, bool isFlyingHorizontally)
        {
            // if (ShowHTP)
            // {
            //     ShowHTP = false;
            //     SendForm(new HTP_0_Window());
            // }
            return base.AcceptPlayerMove(message, isOnGround, isFlyingHorizontally);
        }

        public bool isinCombat()
        {
            return checkCombat();
        }

        public bool checkCombat()
        {
            if (Combat == null) return false;
            if (Combat.getTick() < CyberUtils.getTick())
            {
                leaveCombat();
                return false;
            }

            return true;
        }

        public int getBaseSwingSpeed()
        {
            return BaseSwingSpeed;
        }

        public int getAttackTime()
        {
            Buff b = getClassBuffList()[SwingSpeed];
            DeBuff db = getClassDeBuffList()[SwingSpeed];
            if (b == null) b = new Buff(NULL, 1);
            if (db == null) db = new DeBuff(NULL, 1);
            return (int) (getBaseSwingSpeed() * b.getAmount() / db.getAmount());
        }

        public bool attack(CustomEntityDamageEvent source)
        {
            // getServer().getPluginManager().callEvent(source);
            // if (source.isCancelled()) return false;
            // PlayerTakeDamageEvent e = new PlayerTakeDamageEvent(source);
            // getServer().getPluginManager().callEvent(e);
            // setHealth(getHealth() - e.getFinalDamage());
            return true;
        }

        public int attackTime = 0;

        protected override void EntityAttack(ItemUseOnEntityTransaction transaction)
        {
            if (SwingCooldown.isValid())
            {
                return;
                SendMessage(ChatColors.Yellow + "YOU STILL HAVE SWING COOLDONW!!!!!!");
            }

            base.EntityAttack(transaction);
            enterCombat();
            SwingCooldown.Reset(getAttackTime());


            // List<DamageCause> da = new List<DamageCause>();
            // da.Add(DamageCause.Fire);
            // da.Add(DamageCause.FireTick);
            // da.Add(DamageCause.Lava);
            // Entity entity;
            // if (!Level.TryGetEntity(transaction.EntityId, out entity)) return;
            // if (entity is CorePlayer target)
            // {
            //     
            // }
            // if (uw && source.getCause() == EntityDamageEvent.DamageCause.FALL) return false;
            // if (da.contains(source.getCause()))
            // {
            //     var defender = (Player) source.getEntity();
            //     if (defender == null) return super.attack(source); //Defender not player
            //     Item cp = defender.getInventory().getChestplate();
            //     if (cp == null) return super.attack(source); //No Chestplate on
            //     EntityDamageEvent.DamageCause cause = source.getCause();
            //     /*//Check if defender has BurnShield
            //     BurnShield bs =
            //         (BurnShield) CustomEnchantment.getEnchantFromIDFromItem(cp, CustomEnchantment.BURNSHILED);
            //     if (bs == null) return super.attack(source);
            //     int bsl = bs.getLevel();
            //     switch (cause)
            //     {
            //         case FIRE_TICK:
            //             if (bsl >= 1) return false;
            //         case FIRE:
            //             if (bsl >= 2) return false;
            //         case LAVA:
            //             if (bsl >= 3) return false;
            //     }*/
            // }
        }


        public bool CheckGround()
        {
            var a = KnownPosition.Subtract(new PlayerLocation(0, 1, 0));
            var b = Level.GetBlock(a);
            if (b == null) return false;
            if (b.Id != 0)
            {
                if (IsOnGround) CyberCoreMain.Log.Info($"SYSTEM ON GROUND FOR PLAYER {Username} works!");
                return true;
            }

            return false;
        }


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

        private void AddCoolDown(string key, int secs)
        {
            CDL[key] = new CoolDown(key, secs);
        }

        private CoolDown GetCooldown(string key)
        {
            return GetCooldown(key, false);
        }

        private CoolDown GetCooldown(string key, bool checkvalid)
        {
            if (!CDL.ContainsKey(key)) return null;
//        CyberCoreMain.Log.info(" VALID"+key);
            CoolDown cd = CDL[key];
            if (cd == null) return null;
//        CyberCoreMain.Log.info("CVALID"+!cd.isValidTick()+" | "+cd.Time+"|"+Server.GetInstance().getTick());
            if (checkvalid && !cd.isValid())
            {
//            CyberCoreMain.Log.info(" EXPIRED "+key);
                CDL.Remove(key);
                return null;
            }

//        CyberCoreMain.Log.info(" GOOD "+key);
            return cd;
//
//
//        for (CoolDown c : (List<CoolDown>) CDL.clone()) {
//            CyberCoreMain.Log.info("CHECK KEY"+key +" == "+ c.Key);
//            if (c.Key.equalsIgnoreCase(key)) {
//                CyberCoreMain.Log.info("CHECK VALID"+checkvalid +" == "+ !c.isValidTick());
//                if (checkvalid && !c.isValidTick()) {//CT !> Set Time
//                    CDL.remove(c);
//                    return null;
//                }
//                return c;
//            }
//        }
//        return null;
        }

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

        public void loadEPD()
        {
            List<Dictionary<string, object>> a = CyberCoreMain.GetInstance().SQL
                .executeSelect($"SELECT * FROM `EPD` WHERE player = '{Username}'");
            if (a.Count != 0)
            {
                CyberCoreMain.Log.Info($" Loading Extra Player Data for {Username}");
                EPD = new ExtraPlayerData(this, a[0]);
                return;
            }

            EPD = new ExtraPlayerData(this);
            CyberCoreMain.Log.Info($" Extra Player Data NOTTTTTT FOUND DDDD for {Username}");
        }

        protected override void OnTicked(PlayerEventArgs e)
        {
            base.OnTicked(e);
            onUpdate(CyberUtils.getTick());
        }

        private long tt = 0;

        public void onUpdate(long currentTick)
        {
            tt++;
            if (tt < 20 * 10) return;
            if (tt > Int64.MaxValue - 1) tt = 0;

            if (ShowHTP)
            {
                ShowHTP = false;
                SendForm(new HTP_0_Window());
            }

            //Check for Faction!
            if (currentTick % 5 == 0) //Only allows 4 Ticks per Sec
                if (!CooldownLock)
                {
                    CooldownLock = true;
                    if (Combat != null)
                        if (Combat.getTick() < currentTick) //No Long in combat
                            leaveCombat();
                    // var epd = GetCooldown(Cooldown_EPD, true);
                    // if (epd == null)
                    // {
                    //     AddCoolDown(Cooldown_EPD,60*5);//5 Mins
                    //     if (EPD != null)
                    //     {
                    //         EPD.upload();
                    //     }
                    //     else
                    //     {
                    //         EPD = new ExtraPlayerData(this);
                    //     }
                    // }
                    // var epd1 = GetCooldown(Cooldown_EPD_Valid, true);
                    // if (epd1 == null)
                    // {
                    //     AddCoolDown(Cooldown_EPD_Valid,30);//5 Mins
                    //     if (EPD != null)
                    //     {
                    //         EPD.update();
                    //     }
                    //     else
                    //     {
                    //         EPD = new ExtraPlayerData(this);
                    //     }
                    // }
                    //            CyberCoreMain.Log.info("RUNNNING "+CDL.size());
                    var fc = GetCooldown(Cooldown_Faction, true);
                    if (fc == null)
                    {
//                    CyberCoreMain.Log.info("RUNNNING FACTION CHECK IN CP" + CDL.size());
                        AddCoolDown(Cooldown_Faction, 60); //3 mins
                        if (Faction == null)
                        {
                            Faction f = CyberCoreMain.GetInstance().FM.FFactory.getPlayerFaction(this);
                            if (f == null)
                                Faction = null;
                            else
                                Faction = f.getName();
                        }

                        //Check to See if Faction Invite Expired
                        EPD.update();
                    }

                    //Delay Teleport
                    var tc = GetCooldown(Cooldown_DTP, true);
                    if (tc == null)
                    {
//                    CyberCoreMain.Log.info("RUNNNING CLASS CHECK IN CP" + CDL.size()+"||"+ getPlayerClass());
                        AddCoolDown(Cooldown_DTP, 2);
                        if (isWaitingForTeleport())
                        {
                            if (isReadyToTeleport())
                            {
                                Teleport(WFTP_ToPlayerLocation.Safe(Level));
                                clearWaitingForTP();
                            }
                        }
                    }
                    //Class Check

                    //FIX HERE
                    //TODO FIX HERE
                    var cc = GetCooldown(Cooldown_Class, true);
                    if (cc == null)
                    {
//                    CyberCoreMain.Log.info("RUNNNING CLASS CHECK IN CP" + CDL.size()+"||"+ getPlayerClass());
                        AddCoolDown(Cooldown_Class, 5);
                        BaseClass bc = getPlayerClass();
                        if (bc != null) bc.onUpdate(currentTick);
                        initAllClassBuffs();
                    }

//                         var sc = GetCooldown(Scoreboard_Class, true);
//                         if (sc == null)
//                         {
// //                    CyberCoreMain.Log.info("RUNNNING CLASS CHECK IN CP" + CDL.size()+"||"+ getPlayerClass());
//                             AddCoolDown(Scoreboard_Class, 3);
//                             ReloadScoreBoard();
//
//                             //REMOVE SHOP/AH/SpawnShop GUI Items
//                             var k = 0;
//                             for (Item i :
//                             getInventory().getContents().values()) {
//                                 if (i.hasCompoundTag())
//                                 {
//                                     if (i.getNamedTag().contains(ShopInv.StaticItems.KeyName)) getInventory().remove(i);
//                                     if (i.getNamedTag().contains("AHITEM")) getInventory().remove(i);
//                                     if (i.getNamedTag().contains(net.yungtechboy1.CyberCore.Factory.Shop.Spawner
//                                         .SpawnerShop.StaticItems.KeyName))
//                                         getInventory().remove(i);
// //                                k++;
//                                 }
//                             }
//                         }

                    CooldownLock = false;
                }


            //Check to see if Player as medic or Restoration
            HungerManager pf = getFoodData();
            if (TPR != null && TPRTimeout != 0 && TPRTimeout < currentTick)
            {
                TPRTimeout = 0;
                CorePlayer cp = CyberCoreMain.GetInstance().getPlayer(TPR);
                if (cp != null)
                {
                    var p = new Popup()
                    {
                        Duration = 40L,
                        MessageType = MessageType.Tip,
                        Message = ChatColors.Yellow + "Teleport request expired"
                    };
                    AddPopup(p);
                    cp.AddPopup(p);
                }

                ;
                TPR = null;
            }

            // if (isInTeleportingProcess)
            // {
            //     sendPopup(TeleportTick + "|" + isTeleporting);
            //     if (TeleportTick != 0 && isTeleporting)
            //     {
            //         if (CTLastPos == null)
            //         {
            //             CTLastPos = (PlayerLocation) KnownPosition.Clone();
            //         }
            //         else
            //         {
            //             SendMessage(CTLastPos.DistanceTo(KnownPosition) + "");
            //             if (CTLastPos.DistanceTo(KnownPosition) > 3) isTeleporting = false;
            //             //            CTLastPos = getVector3();
            //         }
            //
            //         if (TeleportTick <= currentTick && isTeleporting)
            //         {
            //             CyberCoreMain.Log.Error("Was LOG ||" + "AAAAAA");
            //             if (isTeleporting)
            //             {
            //                 RemoveAllEffects(); //TODO use `removeEffect` to only remove that Effect
            //                 if (TargetTeleporting != null && TargetTeleporting.HealthManager.IsDead)
            //                 {
            //                     SendMessage("Error! Player Not found or Dead!!");
            //                     isInTeleportingProcess = false;
            //                     TeleportTick = 0;
            //                     CTLastPos = null;
            //                     return;
            //                 }
            //
            //                 if (TargetTeleporting == null && TargetTeleportingLoc == null)
            //                 {
            //                     SendMessage("Error! No Teleport data found!!!");
            //                     isInTeleportingProcess = false;
            //                     TeleportTick = 0;
            //                     CTLastPos = null;
            //                     return;
            //                 }
            //
            //                 if (TargetTeleportingLoc != null)
            //                 {
            //                     Level.MakeSound(new BlazeFireballSound(KnownPosition));
            //                     getLevel().addSound(getVector3(), Sound.MOB_ENDERMEN_PORTAL);
            //                     Teleport(TargetTeleportingLoc);
            //                     TargetTeleportingLoc = null;
            //                     TargetTeleporting = null;
            //                 }
            //                 else
            //                 {
            //                     getLevel().addSound(getVector3(), Sound.MOB_ENDERMEN_PORTAL);
            //                     Teleport(TargetTeleporting);
            //                     TargetTeleportingLoc = null;
            //                     TargetTeleporting = null;
            //                 }
            //
            //                 isInTeleportingProcess = false;
            //                 TeleportTick = 0;
            //                 CTLastPos = null;
            //             }
            //         }
            //         else if (isInTeleportingProcess && !isTeleporting)
            //         {
            //             removeAllEffects();
            //             SendMessage("Error! you moved too much!");
            //             isInTeleportingProcess = false;
            //             TeleportTick = 0;
            //             CTLastPos = null;
            //             TargetTeleportingLoc = null;
            //             TargetTeleporting = null;
            //         }
            //     }
            // }
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

        public void LoadHomes(List<Faction.HomeData> hd)
        {
            HD.AddRange(hd);
        }

        public bool CheckHomeKey(string key)
        {
            foreach (Faction.HomeData h in HD)
            {
                if (h.getName().equalsIgnoreCase(key)) return true;
            }

            return false;
        }

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

        public void TeleportToHome(string key)
        {
            TeleportToHome(key, false);
        }

        public void TeleportToHome(string key, int delay)
        {
            TeleportToHome(key, false, delay);
        }

        public void TeleportToHome(string key, bool instant)
        {
            TeleportToHome(key, instant, 3);
        }

        public void TeleportToHome(string key, bool instant, int delay)
        {
            foreach (Faction.HomeData h in HD)
            {
                if (h.getName().equalsIgnoreCase(key))
                {
                    PlayerLocation v3 = h.getPosition();
                    if (instant) Teleport(v3);
                    else
                        StartTeleport(h.getPosition(), 7);
                }
            }
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

        public void Start(object o)
        {
            CyberCoreMain.Log.Info("============================================== SUMMM NULLL");
            CyberCoreMain.Log.Info("============================================== SUMMM NULLL");
            CyberCoreMain.Log.Info("============================================== SUMMM NULLL");
            CyberCoreMain.Log.Info("============================================== SUMMM NULLL");
            CyberCoreMain.Log.Info("============================================== SUMMM NULLL");
            CyberCoreMain.Log.Info("============================================== SUMMM NULLL");
            CyberCoreMain.Log.Info("============================================== SUMMM NULLL");
            CyberCoreMain.Log.Info("============================================== SUMMM NULLL");
            if (!this.IsConnected || this.Level != null) CyberCoreMain.Log.Info("YOPOOOOOO SUMMM NULLL");
            CyberCoreMain.Log.Info("LVELEEEELLL" + CyberCoreMain.GetInstance().getAPI().LevelManager
                .GetLevel(this, Dimension.Overworld.ToString()));
            base.Start(o);
        }

        public bool CanAddHome()
        {
            return HD.Count < MaxHomes;
        }

        public void DelHome(string name)
        {
            var k = 0;
            var kk = 0;
            foreach (Faction.HomeData h in HD)
            {
                k++;
                if (h.getName().equalsIgnoreCase(name))
                {
                    kk = 1;
                    break;
                }
            }

            if (kk == 1) HD.RemoveAt(k);
        }

        public void AddHome(string name)
        {
            HD.Add(new Faction.HomeData(this, name));
        }

        public void AddHome(Faction.HomeData homeData)
        {
            HD.Add(homeData);
        }

        private PlayerSettingsData getSettingsData()
        {
            return SettingsData;
        }

        public void setSettingsData(PlayerSettingsData settingsData)
        {
            SettingsData = settingsData;
        }

        private PlayerSettingsData PlayerSettingsData = null;

        public void setPlayerSettingsData(PlayerSettingsData playerSettingsData)
        {
            PlayerSettingsData = playerSettingsData;
        }

        public PlayerSettingsData getPlayerSettingsData()
        {
            if (getSettingsData() == null) CreateDefaultSettingsData(this);
            return getSettingsData();
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


        public void tickPowerSource(int tick)
        {
            if (PlayerClass != null) PlayerClass.tickPowerSource(tick);
            //TODO
        }

        public string getFactionName()
        {
            var f = getFaction();
            if (f == null) return "No Faction";
            return f.getDisplayName();
        }

        public Faction getFaction()
        {
            if (Faction == null) return null;
            return CyberCoreMain.GetInstance().FM.FFactory.getFaction(Faction);
        }

        public bool isReadyToTeleport()
        {
            if (!(WFTP_TPTick == -1) && WFTP_ToPlayerLocation != null)
            {
                return CyberUtils.getTick() >= WFTP_TPTick;
            }

            return false;
        }

        public bool isWaitingForTeleport()
        {
            if (!(WFTP_TPTick == -1) && WFTP_ToPlayerLocation != null)
            {
                if (WFTP_StartPos != null &&
                    WFTP_StartPos.DistanceTo(KnownPosition) > WFTP_CancelDistance)
                {
                    SendMessage(ChatColors.Red + "Error! You moved too much so your Teleport request was canceled");
                    clearWaitingForTP();
                    return false;
                }

                return true;
            }

            clearWaitingForTP();
            return false;
        }

        public void clearWaitingForTP()
        {
            WFTP_TPTick = -1;
            WFTP_ToPlayerLocation = null;
        }

        public bool delayTeleport(Vector3 pos, int secs = 5, bool giveeffects = true, int canceltpdistance = 2)
        {
            if (isWaitingForTeleport())
            {
                SendMessage(ChatColors.Red + "Error! You are already waiting for a Teleport!");
                return false;
            }

            WFTP_TPTick = CyberUtils.getTick() + (secs * 20);
            WFTP_StartPos = (PlayerLocation) KnownPosition.Clone();
            WFTP_ToPlayerLocation = new PlayerLocation(pos);
            WFTP_Effects = giveeffects;
            WFTP_CancelDistance = canceltpdistance;
            if (WFTP_Effects)
            {
                SetEffect(new Nausea()
                {
                    Duration = 20 * (secs + 3)
                });
                SetEffect(new Slowness()
                {
                    Duration = 20 * (secs + 3)
                });
            }

            SendMessage(ChatColors.Yellow + $"Teleporting in {secs} Secs! Please stay still!");

            return true;
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

        public FactionRank getFactionRank()
        {
            var f = getFaction();
            if (f == null) return FactionRank.None;
            return f.getPlayerRank(this);
        }
    }
}