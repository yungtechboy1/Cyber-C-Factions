using System;
using System.Collections.Generic;
using System.Net;
using System.Numerics;
using CyberCore.CustomEnums;
using CyberCore.Manager.AuctionHouse;
using CyberCore.Manager.ClassFactory;
using CyberCore.Manager.Factions;
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
using MiNET.Items;
using MiNET.Sounds;
using MySqlX.XDevAPI;
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
        private readonly string Scoreboard_Class = "ScoreBoard";
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

        // private BaseClass PlayerClass = null;
        private int ClassCheck = -1;
        public CombatData Combat;
        private Vector3 CTLastPos;
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
        public string FactionInvite;
        public int FactionInviteTimeout = -1;
        public int fixcoins = 0;

        public PlayerFactionSettings fsettings = new PlayerFactionSettings();

        // public List<Enchantment> MasterEnchantigList = null;
        public List<Faction.HomeData> HD = new List<Faction.HomeData>();
        public CoreSettings InternalPlayerSettings = new CoreSettings();
        private bool isInTeleportingProcess = false;

        private bool isTeleporting;

        // private FormWindow nw;
        private Item ItemBeingEnchanted;
        private bool ItemBeingEnchantedLock;
        public int kills;
        private Vector3 lastBreakVector31;
        public string LastMessageSentTo = null;
        public MainForm LastSentFormType = MainForm.NULL;
        public SubMenu LastSentSubMenu = SubMenu.NULL;
        public int MaxHomes = 5;

        /**
     * @@deprecated
     */
        public int money = 0;

        public bool MuteMessage = false;
        private RankList rank = RankList.PERM_GUEST;
        private PlayerSettingsData SettingsData;
        public ShopInv Shop = null;
        public SpawnerShop SpawnerShop = null;
        private CoolDown SwingCooldown = new CoolDown();
        private CorePlayer TargetTeleporting;
        private Vector3 TargetTeleportingLoc;
        private int TeleportTick;
        public string TPR;

        public int TPRTimeout;

        // public Scoreboard PlayerScoreBoard = ScoreboardAPI.createScoreboard();
        protected Dictionary<BuffType, float> lastdata = null;
        private long uct;
        private bool uw;
        private int WaitingForTP = -1;

        public int WaitingForTPCD = 2;
        public bool WaitingForTPEffects;
        private Vector3 WaitingForTPPos;
        private Vector3 WaitingForTPStartPos;

        public CorePlayer(MiNetServer server, IPEndPoint endPoint, OpenApi api) : base(server, endPoint, api)
        {
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
                foreach (var VARIABLE in COLLECTION)
                {
                    
                }
                data.forEach((key, value)-> {
                    switch (key)
                    {
                        case Movement:
                            setMovementSpeed(DEFAULT_SPEED * value, true);
                            break;
                        case NULL:
                            break;
                        case Health:
                            CustomExtraHP = value + 0;
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
                });
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
            return super.getHealth();
        }

        

        public int getMaxHealth()
        {
            return 20 + (this.hasEffect(Effect.HEALTH_BOOST)
                ? 4 * (this.getEffect(Effect.HEALTH_BOOST).getAmplifier() + 1)
                : 0) + CustomExtraHP;
        }

        public void addBuffFromClass(Buff b)
        {
            addBuffFromClass(b, false);
        }

        public void addBuffFromClass(Buff b, bool force)
        {
            if (b == null) return;
            if (!Bufflist.ContainsKey(BuffOrigin.Class) || Bufflist.get(BuffOrigin.Class) == null)
            {
                var hash = new Dictionary<BuffType, Buff>();
                hash.put(b.getBt(), b);
                Bufflist.put(BuffOrigin.Class, hash);
            }
            else
            {
                Dictionary<BuffType, Buff> bm = Bufflist.get(BuffOrigin.Class);
                if (bm.ContainsKey(b.getBt()) && !force) return; //Cant write it something is here
                bm.put(b.getBt(), b);
                Bufflist.put(BuffOrigin.Class, bm);
            }
        }

        public void addDeBuffFromClass(DeBuff b)
        {
            addDeBuffFromClass(b, false);
        }

        public void addDeBuffFromClass(DeBuff b, bool force)
        {
            if (b == null) return;
            if (!DeBufflist.ContainsKey(BuffOrigin.Class) || DeBufflist.get(BuffOrigin.Class) == null)
            {
                var hash = new Dictionary<BuffType, DeBuff>();
                hash.put(b.getBt(), b);
                DeBufflist.put(BuffOrigin.Class, hash);
            }
            else
            {
                Dictionary<BuffType, DeBuff> bm = DeBufflist.get(BuffOrigin.Class);
                if (bm.ContainsKey(b.getBt()) && !force) return; //Cant write it something is here
                bm.put(b.getBt(), b);
                DeBufflist.put(BuffOrigin.Class, bm);
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
////        System.out.println("Sending >> "+packet);
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
            UpdateAttributesPacket pk = new UpdateAttributesPacket();
            pk.entityId = this.getId();
            pk.entries = new Attribute[]
            {
                Attribute.getAttribute(Attribute.MAX_HEALTH).setMaxValue(getMaxHealth())
                    .setValue(health > 0 ? health < getMaxHealth() ? health : getMaxHealth() : 0),
                Attribute.getAttribute(Attribute.MAX_HUNGER).setValue(getFoodData().getLevel()),
                Attribute.getAttribute(Attribute.MOVEMENT_SPEED).setValue(getMovementSpeed()),
                Attribute.getAttribute(Attribute.EXPERIENCE_LEVEL).setValue(this.getExperienceLevel()),
                Attribute.getAttribute(Attribute.EXPERIENCE)
                    .setValue((float) this.getExperience() / calculateRequireExperience(this.getExperienceLevel()))
            };
            this.dataPacket(pk);
        }

        public bool isInTeleportingProcess()
        {
            return isInTeleportingProcess;
        }

        public void setInTeleportingProcess(bool inTeleportingProcess)
        {
            isInTeleportingProcess = inTeleportingProcess;
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

//    @Deprecated
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
                getInventory().addItem(i);
                clearItemBeingEnchanted();
                removeItemBeingEnchantedLock();
            }
        }

        public PlayerSettingsData GetData()
        {
            if (getSettingsData() == null) CreateDefaultSettingsData(this);
            return getSettingsData();
        }

        

        public void close(TextContainer message, string reason, bool notify)
        {
            System.out.println("CCCCCCCC1");
            CyberCoreMain.getInstance().ServerSQL.UnLoadPlayer(this);
            System.out.println("CCCCCCCC2");
            super.close(message, reason, notify);
        }

        public void CreateDefaultSettingsData(CorePlayer p)
        {
            var a = new PlayerSettingsData(p);
            setSettingsData(a);
        }

        

        public int addWindow(Inventory inventory, int forceId, bool isPermanent)
        {
            if (this.windows.ContainsKey(inventory)) return this.windows.get(inventory);
            int cnt;
            if (forceId == null)
                this.windowCnt = cnt = Math.max(4, ++this.windowCnt % 99);
            else
                cnt = forceId;
            this.windows.put(inventory, cnt);

            if (isPermanent) this.permanentWindows.add(cnt);

            if (inventory.open(this)) return cnt;

            this.removeWindow(inventory);
            sendMessage("ERROR!!!!!!! I FUCKKKKED UUUUPPP");
            return -1;
        }

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

        

        public float getMovementSpeed()
        {
            return super.getMovementSpeed();
        }

        public void TakeMoney(double price)
        {
            if (price <= 0) return;
            var ped = GetData();
            ped.takeCash(price);
        }

        public void AddMoney(double price)
        {
            if (price <= 0) return;
            var ped = GetData();
            ped.addCash(price);
        }

        public double getMoney()
        {
            return GetData().getCash();
        }

        public void SetRank(RankList r)
        {
            SetRank(r.getRank());
        }

        public void SetRank(Rank r)
        {
            rank = r;
        }

        public Rank GetRank()
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

        public FormWindow getNewWindow()
        {
            return nw;
        }

        public void setNewWindow(FormWindow nw)
        {
            this.nw = nw;
        }

        public void clearNewWindow()
        {
            this.nw = null;
        }

        

        public void fall(float fallDistance)
        {
            if (!uw) super.fall(fallDistance);
        }

        public void enterCombat()
        {
            if (Combat == null) sendMessage(TextFormat.YELLOW + "You are now in combat!");
            Combat = new CombatData(getServer().getTick());
        }

        public readonly bool isinCombat()
        {
            return checkCombat();
        }

        public bool checkCombat()
        {
            if (Combat == null) return false;
            if (Combat.getTick() < getServer().getTick())
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
            Buff b = getClassBuffList().get(SwingSpeed);
            DeBuff db = getClassDeBuffList().get(SwingSpeed);
            if (b == null) b = new Buff(NULL, 1);
            if (db == null) db = new DeBuff(NULL, 1);
            return (int) Math.floor(getBaseSwingSpeed() * b.getAmount() / db.getAmount());
        }

        public bool attack(CustomEntityDamageEvent source)
        {
            getServer().getPluginManager().callEvent(source);
            if (source.isCancelled()) return false;
            PlayerTakeDamageEvent e = new PlayerTakeDamageEvent(source);
            getServer().getPluginManager().callEvent(e);
            setHealth(getHealth() - e.getFinalDamage());
            return true;
        }

        

        public bool attack(EntityDamageEvent source)
        {
            List<EntityDamageEvent.DamageCause> da = new List<>();
            da.add(EntityDamageEvent.DamageCause.FIRE);
            da.add(EntityDamageEvent.DamageCause.FIRE_TICK);
            da.add(EntityDamageEvent.DamageCause.LAVA);
            if (uw && source.getCause() == EntityDamageEvent.DamageCause.FALL) return false;
            if (da.contains(source.getCause()))
            {
                var defender = (Player) source.getEntity();
                if (defender == null) return super.attack(source); //Defender not player
                Item cp = defender.getInventory().getChestplate();
                if (cp == null) return super.attack(source); //No Chestplate on
                EntityDamageEvent.DamageCause cause = source.getCause();
                //Check if defender has BurnShield
                BurnShield bs =
                    (BurnShield) CustomEnchantment.getEnchantFromIDFromItem(cp, CustomEnchantment.BURNSHILED);
                if (bs == null) return super.attack(source);
                int bsl = bs.getLevel();
                switch (cause)
                {
                    case FIRE_TICK:
                        if (bsl >= 1) return false;
                    case FIRE:
                        if (bsl >= 2) return false;
                    case LAVA:
                        if (bsl >= 3) return false;
                }
            }

            if (attackTime > 0) sendMessage(TextFormat.YELLOW + "YOU STILL HAVE SWING COOLDONW!!!!!!");

            if (super.attack(source))
            {
                enterCombat();
//            attackTime = getAttackTime();
                return true;
            }

            return false;
        }

        

        protected void doFirstSpawn()
        {
            this.spawned = true;
            this.setEnableClientCommand(true);
            this.getAdventureSettings().update();
            this.sendPotionEffects(this);
            this.sendData(this);
            this.inventory.sendContents(this);
            this.inventory.sendArmorContents(this);
            SetTimePacket setTimePacket = new SetTimePacket();
            setTimePacket.time = this.level.getTime();
            this.dataPacket(setTimePacket);
            Vector3 pos = this.level.getSafeSpawn(this);
            PlayerRespawnEvent respawnEvent = new PlayerRespawnEvent(this, pos, true);
            this.server.getPluginManager().callEvent(respawnEvent);
            pos = respawnEvent.getRespawnVector3();
            if (getHealth() <= 0.0F)
            {
                pos = this.getSpawn();
                RespawnPacket respawnPacket = new RespawnPacket();
                respawnPacket.x = (float) pos.x;
                respawnPacket.y = (float) pos.y;
                respawnPacket.z = (float) pos.z;
                this.dataPacket(respawnPacket);
            }

            this.sendPlayStatus(3);
            var playerJoinEvent = new PlayerJoinEvent(this,
                new TranslationContainer(TextFormat.YELLOW + "%multiplayer.player.joined",
                    new string[] {this.getDisplayName()}));
            this.server.getPluginManager().callEvent(playerJoinEvent);
            if (playerJoinEvent.getJoinMessage().toString().trim().length() > 0)
                this.server.broadcastMessage(playerJoinEvent.getJoinMessage());

            this.noDamageTicks = 60;

            CyberCoreMain.getInstance().CraftingManager.rebuildPacket();
            dataPacket(CustomCraftingManager.packet);
//        this.getServer().sendRecipeList(this);
            if (this.gamemode == 3)
            {
                InventoryContentPacket inventoryContentPacket = new InventoryContentPacket();
                inventoryContentPacket.inventoryId = 121;
                this.dataPacket(inventoryContentPacket);
            }
            else
            {
                this.inventory.sendCreativeContents();
            }

            Iterator var13 = this.usedChunks.keySet().iterator();

            while (var13.hasNext())
            {
                long index = (Long) var13.next();
                int chunkX = Level.getHashX(index);
                int chunkZ = Level.getHashZ(index);
                Iterator var10 = this.level.getChunkEntities(chunkX, chunkZ).values().iterator();

                while (var10.hasNext())
                {
                    var entity = (Entity) var10.next();
                    if (this != entity && !entity.closed && entity.isAlive()) entity.spawnTo(this);
                }
            }

            int experience = this.getExperience();
            if (experience != 0) this.sendExperience(experience);

            int level = this.getExperienceLevel();
            if (level != 0) this.sendExperienceLevel(this.getExperienceLevel());

            this.teleport(pos, null);
            if (!this.isSpectator()) this.spawnToAll();

            if (this.level.isRaining() || this.level.isThundering()) this.getLevel().sendWeather(this);

            this.getLevel().sendWeather(this);
            PlayerFood food = getFoodData();
            if (food.getLevel() != food.getMaxLevel()) food.sendFoodLevel();


            //        player.dataPacket(CraftingManager.packet);
//        CyberCoreMain.getInstance().CraftingManager.rebuildPacket();
//        dataPacket(CustomRecipeCraftingManager.packet);
        }

        public bool CheckGround()
        {
            AxisAlignedBB bb = this.boundingBox.clone();
            bb.setMinY(bb.getMinY() - 0.75);

            this.onGround = this.level.getCollisionBlocks(bb).length > 0;
            this.isCollided = this.onGround;
            return onGround;
        }

        

        public int showFormWindow(FormWindow window, int id)
        {
            ModalFormRequestPacket packet = new ModalFormRequestPacket();
            packet.formId = id;
            packet.data = window.getJSONData();
            this.formWindows.put(packet.formId, window);
            this.dataPacket(packet);
            if (window instanceof CyberForm) {
                FormType.MainForm ft = ((CyberForm) window)._FT;
                if (ft != null && ft != FormType.MainForm.NULL) LastSentFormType = ft;
            }
            return id;
        }

        

        public void handleDataPacket(DataPacket packet)
        {
//        System.out.println("DP >>>> " + packet + "||" + packet.pid());
            if (!connected) return;

            try

            (Timing timing = Timings.getReceiveDataPacketTiming(packet)) {
                DataPacketReceiveEvent ev = new DataPacketReceiveEvent(this, packet);
                this.server.getPluginManager().callEvent(ev);
                if (ev.isCancelled()) return;

                if (packet.pid() == ProtocolInfo.BATCH_PACKET)
                {
                    this.server.getNetwork().processBatch((BatchPacket) packet, this);
                    return;
                }

                switch (packet.pid())
                {
                    case ProtocolInfo.MODAL_FORM_RESPONSE_PACKET:
                        if (!this.spawned || !this.isAlive()) break;

                        ModalFormResponsePacket modalFormPacket = (ModalFormResponsePacket) packet;

                        if (formWindows.ContainsKey(modalFormPacket.formId))
                        {
                            FormWindow window = formWindows.remove(modalFormPacket.formId);
                            if (window instanceof CyberForm)
                            ((CyberForm) window).setResponse(modalFormPacket.data.trim(), this);
                            else
                            window.setResponse(modalFormPacket.data.trim());

                            PlayerFormRespondedEvent event = new PlayerFormRespondedEvent(this, modalFormPacket.formId,
                                window);
                            getServer().getPluginManager().callEvent(event);
                            return;
                        }

//                    return;
                        break;
                    case ProtocolInfo.MOB_EQUIPMENT_PACKET:
                        if (!this.spawned || !this.isAlive()) break;

                        MobEquipmentPacket mobEquipmentPacket = (MobEquipmentPacket) packet;

                        //TODO Make into a Custom Event!
                        if (getPlayerClass() != null)
                        {
                            for (PowerAbstract p :
                            getPlayerClass().getActivePowers()) {
                                if (p.getLS().getSlot() == mobEquipmentPacket.hotbarSlot)
                                {
                                    p.initPowerRun();
                                    getInventory().setHeldItemIndex(getInventory().getHeldItemIndex(), true);
                                }
                            }
                        }

                        break;
                    case INVENTORY_TRANSACTION_PACKET:
//                    break;
                        if (this.isSpectator())
                        {
                            this.sendAllInventories();
                            break;
                        }

                        try
                        {
//                        InventoryTransactionPacket transactionPacket = (InventoryTransactionPacket) packet;
//                        System.out.println("BBBBBBBBBBBBBBBBBBBBBBBBB!!!!!!!!1");b
                            CustomInventoryTransactionPacket transactionPacket2 =
                                (CustomInventoryTransactionPacket) packet;

                            if (transactionPacket2 == null)
                                System.out.
                            println("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");

//                        System.out.println("BBBBBBBBBBBBBBBBBBBBBBBBB!!!!!!!!2");
                            List<InventoryAction> actions = new List<>();
                            for (CustomNetworkInventoryAction na :
                            transactionPacket2.actions) {
//                            System.out.println("zACTIONz z-1+++");
//                            CustomNetworkInventoryAction networkInventoryAction = new CustomNetworkInventoryAction(na);
                                CustomNetworkInventoryAction networkInventoryAction = na;
                                InventoryAction a = networkInventoryAction.createInventoryAction(this);
                                InventoryAction aa = na.createInventoryAction(this);

//                            System.out.println("zACTIONz xxxx>"+new CustomNetworkInventoryAction(na));
                                System.out.println("zACTIONz xxxx>" + na);
//                            System.out.println("zACTIONz z");Cybers
//                            System.out.println("zACTIONz z > "+a);
//                            System.out.println("zACTIONz z > "+a.getClass().getName());
                                if (a instanceof SlotChangeAction && aa instanceof SlotChangeAction) {
                                    SlotChangeAction sca = (SlotChangeAction) a;
                                    SlotChangeAction scaa = (SlotChangeAction) aa;
//                                System.out.println("GGGGGGGGGGGGG"+scaa.getSlot());
//                                System.out.println("GGGGGGGGGGGGG"+sca.getSlot());
                                }
//                            System.out.println("zACTIONz 5.1.1 > "+a.getTargetItem());
//                            System.out.println("zACTIONz 5.1.2 > "+a.getSourceItem());

                                if (a == null)
                                {
                                    this.getServer().getLogger()
                                        .debug("Unmatched inventory action from " + this.getName() + ": " +
                                               networkInventoryAction);
                                    this.sendAllInventories();
                                    break packetswitch;
                                }

                                System.out.println("Adding Network Action to Regualr Action!!!!!!!!");
                                actions.add(a);
                            }

//                        System.out.println("BBBBBBBBBBBBBBBBBBBBBBBBB!!!!!!!!3");
                            if (transactionPacket2.isCraftingPart)
                            {
                                System.out.println("BBBBBBBBBBBBBBBBBBBBBBBBB!!!!!!!!4 IS CRAFTING");
                                if (this.cct == null)
                                {
                                    System.out.println("BBBBBBBBBBBBBBBBBBBBBBBBB!!!!!!!!5 WILL CREATE NEW CT");
                                    this.cct = new CustomCraftingTransaction(this, actions);
                                }
                                else
                                {
                                    for (InventoryAction action :
                                    actions) {
                                        System.out.println("BBBBBBBBBBBBBBBBBBBBBBBBB!!!!!!!!6 ADDING TO EXISTING CTT");
                                        this.cct.addAction(action);
                                    }
                                }

                                if (this.cct.getPrimaryOutput() != null)
                                {
                                    //we get the actions for this in several packets, so we can't execute it until we get the result

                                    System.out.println("BBBBBBBBBBBBBBBBBBBBBBBBB!!!!!!!!7 CALLING EXECUTE");
                                    if (!this.cct.execute()) server.getLogger().error("ERROR NO EXECITE!");
                                    this.cct = null;
                                }

                                return;
                            }

                            if (this.cct != null)
                            {
                                this.server.getLogger()
                                    .debug(
                                        "Got unexpected normal inventory action with incomplete crafting transaction from " +
                                        this.getName() + ", refusing to execute crafting");
                                this.cct = null;
                            }


                            //                        System.out.println("AAAAAAAAAAAAAAAAAAAAAAAAA");
//                        System.out.println("AAAAAAAAAAAAAAAAAAAAAAAAA2"+transactionPacket2.transactionType);
                            switch (transactionPacket2.transactionType)
                            {
                                case InventoryTransactionPacket.TYPE_NORMAL:
//                                System.out.println("ZZZZZZZZZZZZZZZZZZ");
                                    CustomInventoryTransaction transaction =
                                        new CustomInventoryTransaction(this, actions);

//                                System.out.println("ZZZZZZZZZZZZZZZZZZ1"+transaction);
//                                System.out.println("ZZZZZZZZZZZZZZZZZZ2"+transaction.getInventories());
//                                System.out.println("ZZZZZZZZZZZZZZZZZZ3"+transaction.canExecute());
                                    if (!transaction.execute())
                                    {
                                        this.server.getLogger()
                                            .debug("Failed to execute inventory transaction from " + this.getName() +
                                                   " with actions: " + Arrays.toString(transactionPacket2.actions));
                                        break packetswitch; //oops!
                                    }

                                    //TODO: fix achievement for getting iron from furnace

                                    break packetswitch;
                                case InventoryTransactionPacket.TYPE_MISMATCH:
                                    if (transactionPacket2.actions.length > 0)
                                    {
                                        this.server.getLogger()
                                            .debug("Expected 0 actions for mismatch, got " +
                                                   transactionPacket2.actions.length + ", " +
                                                   Arrays.toString(transactionPacket2.actions));
                                        this.server.getLogger()
                                            .error("Expected 0 actions for mismatch, got " +
                                                   transactionPacket2.actions.length + ", " +
                                                   Arrays.toString(transactionPacket2.actions));
                                        getCursorInventory().clearAll();
                                    }
                                    else
                                    {
                                        this.server.getLogger().error("Expected 0 actions for mismatch, got NUN");
                                    }

                                    this.sendAllInventories();

                                    break packetswitch;
                                case InventoryTransactionPacket.TYPE_USE_ITEM:
                                    UseItemData useItemData = (UseItemData) transactionPacket2.transactionData;

                                    BlockVector3 blockVector = useItemData.blockPos;
                                    BlockFace face = useItemData.face;

                                    int type = useItemData.actionType;
                                    Item item;
                                    switch (type)
                                    {
                                        case InventoryTransactionPacket.USE_ITEM_ACTION_CLICK_BLOCK:
                                            this.setDataFlag(DATA_FLAGS, DATA_FLAG_ACTION, false);

                                            System.out.println("wwwwwwwwwwwwww > 11111111111111111");
                                            if (this.canInteract(blockVector.add(0.5, 0.5, 0.5),
                                                this.isCreative() ? 13 : 7))
                                            {
                                                System.out.println("wwwwwwwwwwwwww > 22222222222");
                                                if (this.isCreative())
                                                {
                                                    Item i = inventory.getItemInHand();
                                                    if (this.level.useItemOn(blockVector.asVector3(), i, face,
                                                        useItemData.clickPos.x, useItemData.clickPos.y,
                                                        useItemData.clickPos.z, this) != null)
                                                    {
                                                        System.out.println("wwwwwwwwwwwwww > GOOD");
                                                        break packetswitch;
                                                    }
                                                }
                                                else if (inventory.getItemInHand().equals(useItemData.itemInHand))
                                                {
                                                    Item i = inventory.getItemInHand();
                                                    System.out.println(
                                                        "wwwwwwwwwwwwww > GOOD " + i + "||" + i.getClass());
                                                    Item oldItem = i.clone();
                                                    if (i instanceof ItemBlock)
                                                    System.out.println("YYYYYYYYYYYYYYYYEEEEEEE");
                                                    //TODO: Implement adventure mode checks
                                                    if ((i = this.level.useItemOn(blockVector.asVector3(), i, face,
                                                        useItemData.clickPos.x, useItemData.clickPos.y,
                                                        useItemData.clickPos.z, this)) != null)
                                                    {
                                                        System.out.println("wwwwwwwwwwwwww > GOOD2");
                                                        if (!i.equals(oldItem) || i.getCount() != oldItem.getCount())
                                                        {
                                                            System.out.println("wwwwwwwwwwwwww > GOOD3");
                                                            inventory.setItemInHand(i);
                                                            inventory.sendHeldItem(this.getViewers().values());
                                                        }
                                                    }
                                                    else
                                                    {
                                                        if (i instanceof ItemBlock)
                                                        System.out.println("YYYYYYYYYYYYYYYYEEEEEE222222222222E");
                                                        System.out.println(
                                                            "ERRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRR" + i);
                                                    }
                                                }

                                                break packetswitch;
                                            }

                                            inventory.sendHeldItem(this);

                                            if (blockVector.distanceSquared(this) > 10000)
                                            {
                                                break packetswitch;
                                            }

                                            Block target = this.level.getBlock(blockVector.asVector3());
                                            Block block = target.getSide(face);

                                            this.level.sendBlocks(new Player[] {this}, new[] {target, block},
                                                UpdateBlockPacket.FLAG_ALL_PRIORITY);

                                            if (target instanceof BlockDoor) {
                                            BlockDoor door = (BlockDoor) target;

                                            Block part;

                                            if ((door.getDamage() & 0x08) > 0)
                                            {
                                                //up
                                                part = target.down();

                                                if (part.getId() == target.getId())
                                                {
                                                    target = part;

                                                    this.level.sendBlocks(new Player[] {this}, new[] {target},
                                                        UpdateBlockPacket.FLAG_ALL_PRIORITY);
                                                }
                                            }
                                        }
                                            break packetswitch;
                                        case InventoryTransactionPacket.USE_ITEM_ACTION_BREAK_BLOCK:
                                            if (!this.spawned || !this.isAlive())
                                            {
                                                break packetswitch;
                                            }

                                            this.resetCraftingGridType();

                                            Item i = this.getInventory().getItemInHand();

                                            Item oldItem = i.clone();

                                            if (this.canInteract(blockVector.add(0.5, 0.5, 0.5),
                                                    this.isCreative() ? 13 : 7) &&
                                                (i = this.level.useBreakOn(blockVector.asVector3(), face, i, this,
                                                    true)) != null)
                                            {
                                                if (this.isSurvival())
                                                {
                                                    getFoodData().updateFoodExpLevel(0.025);
                                                    if (!i.equals(oldItem) || i.getCount() != oldItem.getCount())
                                                    {
                                                        inventory.setItemInHand(i);
                                                        inventory.sendHeldItem(this.getViewers().values());
                                                    }
                                                }

                                                break packetswitch;
                                            }

                                            inventory.sendContents(this);
                                            target = this.level.getBlock(blockVector.asVector3());
                                            BlockEntity blockEntity =
                                                this.level.getBlockEntity(blockVector.asVector3());

                                            this.level.sendBlocks(new Player[] {this}, new[] {target},
                                                UpdateBlockPacket.FLAG_ALL_PRIORITY);

                                            inventory.sendHeldItem(this);

                                            if (blockEntity instanceof BlockEntitySpawnable) {
                                            ((BlockEntitySpawnable) blockEntity).spawnTo(this);
                                        }

                                            break packetswitch;
                                        case InventoryTransactionPacket.USE_ITEM_ACTION_CLICK_AIR:
                                            Vector3 directionVector = this.getDirectionVector();

                                            if (this.isCreative())
                                            {
                                                item = this.inventory.getItemInHand();
                                            }
                                            else if (!this.inventory.getItemInHand().equals(useItemData.itemInHand))
                                            {
                                                this.inventory.sendHeldItem(this);
                                                break packetswitch;
                                            }
                                            else
                                            {
                                                item = this.inventory.getItemInHand();
                                            }

                                            var interactEvent = new PlayerInteractEvent(this, item, directionVector,
                                                face, PlayerInteractEvent.Action.RIGHT_CLICK_AIR);

                                            this.server.getPluginManager().callEvent(interactEvent);

                                            if (interactEvent.isCancelled())
                                            {
                                                this.inventory.sendHeldItem(this);
                                                break packetswitch;
                                            }

                                            if (item.onClickAir(this, directionVector) && this.isSurvival())
                                                this.inventory.setItemInHand(item);

                                            this.setDataFlag(DATA_FLAGS, DATA_FLAG_ACTION, true);
                                            this.startAction = this.server.getTick();

                                            break packetswitch;
                                    }

                                    break;
                                case InventoryTransactionPacket.TYPE_USE_ITEM_ON_ENTITY:
                                    UseItemOnEntityData useItemOnEntityData =
                                        (UseItemOnEntityData) transactionPacket2.transactionData;

                                    Entity target = this.level.getEntity(useItemOnEntityData.entityRuntimeId);
                                    if (target == null) return;

                                    type = useItemOnEntityData.actionType;

                                    if (!useItemOnEntityData.itemInHand.equalsExact(this.inventory.getItemInHand()))
                                        this.inventory.sendHeldItem(this);

                                    item = this.inventory.getItemInHand();

                                    switch (type)
                                    {
                                        case InventoryTransactionPacket.USE_ITEM_ON_ENTITY_ACTION_INTERACT:
                                            PlayerInteractEntityEvent playerInteractEntityEvent =
                                                new PlayerInteractEntityEvent(this, target, item);
                                            if (this.isSpectator()) playerInteractEntityEvent.setCancelled();
                                            getServer().getPluginManager().callEvent(playerInteractEntityEvent);

                                            if (playerInteractEntityEvent.isCancelled()) break;

                                            if (target.onInteract(this, item) && this.isSurvival())
                                            {
                                                if (item.isTool())
                                                {
                                                    if (item.useOn(target) &&
                                                        item.getDamage() >= item.getMaxDurability())
                                                        item = new ItemBlock(new BlockAir());
                                                }
                                                else
                                                {
                                                    if (item.count > 1)
                                                        item.count--;
                                                    else
                                                        item = new ItemBlock(new BlockAir());
                                                }

                                                this.inventory.setItemInHand(item);
                                            }

                                            break;
                                        case InventoryTransactionPacket.USE_ITEM_ON_ENTITY_ACTION_ATTACK:
                                            if (SwingCooldown.isValid())
                                            {
                                                sendTip(TextFormat.GRAY + "Class: Swing Cooldown");
//                                            sendTitle(TextFormat.GRAY + "Class: Swing Cooldown");
                                                break;
                                            }

                                            float itemDamage = item.getAttackDamage();

                                            for (Enchantment enchantment :
                                            item.getEnchantments()) {
                                            itemDamage += enchantment.getDamageBonus(target);
                                        }

                                            Map<EntityDamageEvent.DamageModifier, float> damage =
                                                new EnumMap<>(EntityDamageEvent.DamageModifier.class);
                                            damage.put(EntityDamageEvent.DamageModifier.BASE, itemDamage);

                                            if (!this.canInteract(target, isCreative() ? 8 : 5))
                                                break;
                                            else if (target
                                            instanceof Player) {
                                            if ((((Player) target).getGamemode() & 0x01) > 0)
                                                break;
                                            if (!this.server.getPropertyBoolean("pvp") ||
                                                this.server.getDifficulty() == 0) break;
                                        }

                                            //TODO maybe custom???
                                            //Call Custom 1st then Default

                                            CustomEntityDamageByEntityEvent centityDamageByEntityEvent =
                                                new CustomEntityDamageByEntityEvent(this, target,
                                                    CustomEntityDamageEvent.CustomDamageCause.ENTITY_ATTACK,
                                                    itemDamage);
                                            BaseClass bc = getPlayerClass();
                                            if (bc != null) bc.HandelEvent(centityDamageByEntityEvent);
                                            getServer().getPluginManager().callEvent(centityDamageByEntityEvent);
                                            if (centityDamageByEntityEvent.isCancelled()) break;
                                            EntityDamageByEntityEvent entityDamageByEntityEvent =
                                                new EntityDamageByEntityEvent(this, target,
                                                    EntityDamageEvent.DamageCause.ENTITY_ATTACK, damage);
                                            if (this.isSpectator()) entityDamageByEntityEvent.setCancelled();
                                            if ((target instanceof Player) && !this.level.getGameRules()
                                                .getBoolean(GameRule.PVP)) {
                                            entityDamageByEntityEvent.setCancelled();
                                        }

                                            if (!target.attack(entityDamageByEntityEvent))
                                            {
                                                if (item.isTool() && this.isSurvival())
                                                    this.inventory.sendContents(this);
                                                break;
                                            }

                                            //When you Successfully Attack Someone
                                            enterCombat();
                                            SwingCooldown = new CoolDown().setTimeTick(getAttackTime());

                                            for (Enchantment enchantment :
                                            item.getEnchantments()) {
                                            enchantment.doPostAttack(this, target);
                                        }

                                            if (item.isTool() && this.isSurvival())
                                            {
                                                if (item.useOn(target) && item.getDamage() >= item.getMaxDurability())
                                                    this.inventory.setItemInHand(new ItemBlock(new BlockAir()));
                                                else
                                                    this.inventory.setItemInHand(item);
                                            }

                                            return;
                                    }

                                    break;
                                case InventoryTransactionPacket.TYPE_RELEASE_ITEM:
                                    if (this.isSpectator())
                                    {
                                        this.sendAllInventories();
                                        break packetswitch;
                                    }

                                    ReleaseItemData releaseItemData =
                                        (ReleaseItemData) transactionPacket2.transactionData;

                                    try
                                    {
                                        type = releaseItemData.actionType;
                                        switch (type)
                                        {
                                            case InventoryTransactionPacket.RELEASE_ITEM_ACTION_RELEASE:
                                                if (this.isUsingItem())
                                                {
                                                    item = this.inventory.getItemInHand();
                                                    if (item.onReleaseUsing(this)) this.inventory.setItemInHand(item);
                                                }
                                                else
                                                {
                                                    this.inventory.sendContents(this);
                                                }

                                                return;
                                            case InventoryTransactionPacket.RELEASE_ITEM_ACTION_CONSUME:
                                                Item itemInHand = this.inventory.getItemInHand();
                                                PlayerItemConsumeEvent consumeEvent =
                                                    new PlayerItemConsumeEvent(this, itemInHand);

                                                if (itemInHand.getId() == Item.POTION)
                                                {
                                                    this.server.getPluginManager().callEvent(consumeEvent);
                                                    if (consumeEvent.isCancelled())
                                                    {
                                                        this.inventory.sendContents(this);
                                                        break;
                                                    }

                                                    Potion potion = Potion.getPotion(itemInHand.getDamage())
                                                        .setSplash(false);

                                                    if (this.getGamemode() == SURVIVAL)
                                                    {
                                                        --itemInHand.count;
                                                        this.inventory.setItemInHand(itemInHand);
                                                        this.inventory.addItem(new ItemGlassBottle());
                                                    }

                                                    if (potion != null) potion.applyPotion(this);
                                                }
                                                else if (itemInHand.getId() == Item.BUCKET &&
                                                         itemInHand.getDamage() == 1)
                                                {
                                                    //milk
                                                    this.server.getPluginManager().callEvent(consumeEvent);
                                                    if (consumeEvent.isCancelled())
                                                    {
                                                        this.inventory.sendContents(this);
                                                        break;
                                                    }

                                                    EntityEventPacket eventPacket = new EntityEventPacket();
                                                    eventPacket.eid = this.getId();
                                                    eventPacket.event = EntityEventPacket.USE_ITEM;
                                                    this.dataPacket(eventPacket);
                                                    Server.broadcastPacket(this.getViewers().values(), eventPacket);

                                                    if (this.isSurvival())
                                                    {
                                                        itemInHand.count--;
                                                        this.inventory.setItemInHand(itemInHand);
                                                        this.inventory.addItem(new ItemBucket());
                                                    }

                                                    this.removeAllEffects();
                                                }
                                                else
                                                {
                                                    this.server.getPluginManager().callEvent(consumeEvent);
                                                    if (consumeEvent.isCancelled())
                                                    {
                                                        this.inventory.sendContents(this);
                                                        break;
                                                    }

                                                    Food food = Food.getByRelative(itemInHand);
                                                    if (food != null && food.eatenBy(this)) --itemInHand.count;
                                                    this.inventory.setItemInHand(itemInHand);
                                                }

                                                return;
                                        }
                                    }

                                    readonlyly {
                                    this.setUsingItem(false);
                                }
                                    break;
                                default:
                                    this.inventory.sendContents(this);
                                    break;
                            }
                        }
                        catch (Exception e)
                        {
                            Server.getInstance().getLogger().error("EEEEE123>>>>", e);
                        }

                        return;
                    //Do not Pass GO!
                    case ProtocolInfo.MOVE_PLAYER_PACKET:
                        super.handleDataPacket(packet);
                        if (this.teleportVector3 != null) break;
                        Item boots = getInventory().getBoots();
                        if (boots == null) break;
                        Spring se = (Spring) CustomEnchantment.getEnchantFromIDFromItem(boots,
                            (short) CustomEnchantment.SPRING);
                        Climber ce =
                            (Climber) CustomEnchantment.getEnchantFromIDFromItem(boots,
                                (short) CustomEnchantment.CLIMBER);
                        if (se == null && ce == null) break;
                        var nm = new Vector3();
                        if (se != null)
                        {
                            MovePlayerPacket movePlayerPacket = (MovePlayerPacket) packet;
                            var newPos = new Vector3(movePlayerPacket.x, movePlayerPacket.y - this.getEyeHeight(),
                                movePlayerPacket.z);


                            Vector3 dif = newPos.subtract(this);
                            CheckGround();
                            if (dif.getY() > 0 && !uw)
                            {
                                //No Lvl_4 Boost!
                                if (0 < dif.x && dif.x > DEFAULT_SPEED) dif.x = DEFAULT_SPEED;
                                if (0 > dif.x && dif.x < -1 * DEFAULT_SPEED) dif.x = -1 * DEFAULT_SPEED;
                                if (0 > dif.z && dif.z > DEFAULT_SPEED) dif.z = DEFAULT_SPEED;
                                if (0 > dif.z && dif.z < -1 * DEFAULT_SPEED) dif.z = -1 * DEFAULT_SPEED;

                                sendMessage(dif.getY() + "!");

                                nm.add(dif.x * .25, DEFAULT_SPEED * se.GetLevelEffect(), dif.z * .25);
                                resetFallDistance();
                                uct = lastUpdate + 20;
                                uw = true;
//                        upp++;
//                        if (upp > 3) uw = true;
                            }
                            else if (onGround && uct > lastUpdate)
                            {
//                        if (upp > 0) upp--;
                                uw = false;
                            }

                            if (uw) inAirTicks = 0;
                        }
                        else if (ce != null)
                        {
                            MovePlayerPacket movePlayerPacket = (MovePlayerPacket) packet;
                            var newPos = new Vector3(movePlayerPacket.x, movePlayerPacket.y - this.getEyeHeight(),
                                movePlayerPacket.z);


                            Vector3 dif = newPos.subtract(this);
                            CheckGround();
                            if (dif.getY() > 0 && !uw)
                            {
                                //No Lvl_4 Boost!
                                if (0 < dif.x && dif.x > DEFAULT_SPEED) dif.x = DEFAULT_SPEED;
                                if (0 > dif.x && dif.x < -1 * DEFAULT_SPEED) dif.x = -1 * DEFAULT_SPEED;
                                if (0 > dif.z && dif.z > DEFAULT_SPEED) dif.z = DEFAULT_SPEED;
                                if (0 > dif.z && dif.z < -1 * DEFAULT_SPEED) dif.z = -1 * DEFAULT_SPEED;

                                sendMessage(dif.getY() + "!");

                                nm.add(dif.x, DEFAULT_SPEED * ce.GetLevelEffect(), dif.z);
                                resetFallDistance();
                                uct = lastUpdate + 20;
                                uw = true;
//                        upp++;
//                        if (upp > 3) uw = true;
                            }
                            else if (onGround && uct > lastUpdate)
                            {
//                        if (upp > 0) upp--;
                                uw = false;
                            }

                            if (uw) inAirTicks = 0;
                        }

//                    if (upp == 0) uw = false;
                        if (nm != null && !(nm.x != 0 && nm.y != 0 && nm.z != 0)) addMotion(nm.x, nm.y, nm.z);

                        break;
                    case ProtocolInfo.PLAYER_ACTION_PACKET:
                        PlayerActionPacket playerActionPacket = (PlayerActionPacket) packet;
                        if (!this.spawned || !this.isAlive() &&
                            playerActionPacket.action != PlayerActionPacket.ACTION_RESPAWN &&
                            playerActionPacket.action != PlayerActionPacket.ACTION_DIMENSION_CHANGE_REQUEST) return;

                        playerActionPacket.entityId = this.id;
                        var pos = new Vector3(playerActionPacket.x, playerActionPacket.y, playerActionPacket.z);
                        BlockFace face = BlockFace.fromIndex(playerActionPacket.face);

                        actionswitch:
                        switch (playerActionPacket.action)
                        {
                            case PlayerActionPacket.ACTION_START_BREAK:
                                long currentBreak = System.currentTimeMillis();
                                BlockVector3 currentBreakVector3 = new BlockVector3(playerActionPacket.x,
                                    playerActionPacket.y, playerActionPacket.z);
                                // HACK: Client spams multiple left clicks so we need to skip them.
                                if (lastBreakVector31.equals(currentBreakVector3) &&
                                    currentBreak - this.lastBreak < 10 || pos.distanceSquared(this) > 100) return;
                                Block target = this.level.getBlock(pos);
                                var playerInteractEvent = new PlayerInteractEvent(this, this.inventory.getItemInHand(),
                                    target, face,
                                    target.getId() == 0
                                        ? PlayerInteractEvent.Action.LEFT_CLICK_AIR
                                        : PlayerInteractEvent.Action.LEFT_CLICK_BLOCK);
                                this.getServer().getPluginManager().callEvent(playerInteractEvent);
                                if (playerInteractEvent.isCancelled())
                                {
                                    this.inventory.sendHeldItem(this);
                                    return;
                                }

                                switch (target.getId())
                                {
                                    case Block.NOTEBLOCK:
                                        ((BlockNoteblock) target).emitSound();
                                        break actionswitch;
                                    case Block.DRAGON_EGG:
                                        ((BlockDragonEgg) target).teleport();
                                        break actionswitch;
                                }

                                Block block = target.getSide(face);
                                if (block.getId() == Block.FIRE)
                                {
                                    //Chance of getting caught on fire!
                                    RandomChanceOfFire(77);
                                    this.level.setBlock(block, new BlockAir(), true);
                                    return;
                                }

                                if (!this.isCreative())
                                {
                                    //improved this to take stuff like swimming, ladders, enchanted tools into account, fix wrong tool break time calculations for bad tools (pmmp/PocketMine-MP#211)
                                    //Done by lmlstarqaq
                                    double breakTime =
                                        Math.ceil(target.getBreakTime(this.inventory.getItemInHand(), this) * 20);
                                    if (PlayerClass != null)
                                    {
                                        var obreaktime = breakTime;
                                        if (PlayerClass instanceof MineLifeClass &&
                                            PlayerClass.TryRunPower(PowerEnum.MineLife)) {
                                            object nbt = PlayerClass.RunPower(PowerEnum.MineLife,
                                                this.inventory.getItemInHand(), target, breakTime);
                                            if (nbt != null)
                                            {
                                                var nd = (double) nbt;
                                                if (nd > 0)
                                                {
                                                    var dec = obreaktime - nd;
                                                    var dp = dec / obreaktime * 100d;
                                                    breakTime = nd;
                                                    sendMessage("Break Time Redueced by " + dp);
                                                }
                                            }
                                        }
                                    }

                                    if (breakTime > 0)
                                    {
                                        LevelEventPacket pk = new LevelEventPacket();
                                        pk.evid = LevelEventPacket.EVENT_BLOCK_START_BREAK;
                                        pk.x = (float) pos.x;
                                        pk.y = (float) pos.y;
                                        pk.z = (float) pos.z;
                                        pk.data = (int) (65535 / breakTime);
                                        this.getLevel().addChunkPacket(pos.getFloorX() >> 4, pos.getFloorZ() >> 4, pk);
                                    }
                                }

                                this.breakingBlock = target;
                                this.lastBreak = currentBreak;
                                lastBreakVector31 = currentBreakVector3;
                                break;
                            case PlayerActionPacket.ACTION_JUMP:
                                sendMessage("JUMMMPPPPP!!!" + getDirection() + "|" + getMotion());
//                            if (PlayerClass != null) PlayerClass.HandelEvent(new PlayerJumpEvent(this));
                                getServer().getPluginManager().callEvent(new PlayerJumpEvent(this));
//                            addMovement(0,2.5,0,0,0,0);
//                            switch (getDirection()) {
//                                case NORTH:
//                                    addMotion(motionX * 2, 3, 0);
//                                    break;
//                                case EAST:
//                                    addMotion(0, 3, motionZ * 2);
//                                    break;
//                                case WEST:
//                                    addMotion(motionX * -2, 3, 0);
//                                    break;
//                                case SOUTH:
//                                    addMotion(0, 3, motionZ * -2);
//                                    break;
//
//                            }
                                break;
                        }
                }
            }
            if (packet.pid() == INVENTORY_TRANSACTION_PACKET) return;
            super.handleDataPacket(packet);
        }

        public void RandomChanceOfFire(int max)
        {
            NukkitRandom nr = new NukkitRandom(entityCount * max);
            int f = nr.nextRange(0, 100);
            if (f < max) setOnFire(nr.nextRange(1, 4));
        }

        public void ReloadScoreBoard()
        {
            Scoreboard s = PlayerScoreBoard;
            if (s != null) s.hideFor(this);
            s = ScoreboardAPI.createScoreboard();
            ScoreboardDisplay sd = s.addDisplay(DisplaySlot.SIDEBAR, "PlayerInfo",
                TextFormat.AQUA + "~~| UnlimitedPE Factions |~~");
            var k = 0;
            if (!InternalPlayerSettings.isHudPosOff())
            {
                sd.addLine(TextFormat.AQUA + "Positon:", k++);
                sd.addLine(
                    "    " + TextFormat.GOLD + "X: " + TextFormat.GREEN + getFloorX() + TextFormat.GOLD + " Y: " +
                    TextFormat.GREEN + getFloorY() + TextFormat.GOLD + " Z: " + TextFormat.GREEN + getFloorZ(), k++);
            }

            if (!InternalPlayerSettings.isHudFactionOff() && getFaction() != null)
            {
                sd.addLine(TextFormat.GRAY + "Faction : " + TextFormat.AQUA + getFaction().GetDisplayName(), k++);
                sd.addLine(
                    "    " + TextFormat.AQUA + "XP" + TextFormat.GRAY + " | " + TextFormat.GREEN +
                    getFaction().GetXP() + TextFormat.AQUA + " / " + TextFormat.GOLD +
                    getFaction().calculateRequireExperience() + TextFormat.GRAY + " | " + TextFormat.GREEN + "Level: " +
                    TextFormat.YELLOW + getFaction().GetLevel(), k++);
            }

            if (!InternalPlayerSettings.isHudClassOff())
            {
//            TODO
                BaseClass bc = getPlayerClass();
                if (bc != null)
                {
                    List<string> t = bc.FormatHudText();
                    if (t != null && t.size() != 0)
                    {
                        sd.addLine("Class:", k++);
                        for (string ss :
                        t) sd.addLine("    " + ss, k++);
                    }
                    else
                    {
                        sd.addLine("Class: None", k++);
                    }
                }
            }


//        if(!InternalPlayerSettings.isHudClassOff())sd.addLine(TextFormat.GOLD+"X: "+TextFormat.GREEN+getX()+TextFormat.GOLD+"Y: "+TextFormat.GREEN+getY()+TextFormat.GOLD+"Z: "+TextFormat.GREEN+getZ(),k++);
            sd.addLine(TextFormat.YELLOW + "Money : " + TextFormat.AQUA + "$" + getMoney(), k++);
            sd.addLine(TextFormat.YELLOW + "Hunger : " + TextFormat.AQUA + getFoodData().getLevel(), k++);
            sd.addLine(TextFormat.YELLOW + "HP : " + TextFormat.AQUA + getHealth() + " / " + getMaxHealth(), k++);
            sd.addLine(TextFormat.GRAY + "Class & Power COOLDOWNS:", k++);
            sd.addEntity(this, k++);
//        sd.addLine("TEST LINE 1",1);
//        sd.addLine("YOUR NAME"+p.getDisplayName(),2);
//        }else{
//            ScoreboardDisplay sd = s.addDisplay();
//            sd.addEntity(this,10);
//
//        }
            PlayerScoreBoard = s;
            ScoreboardAPI.setScoreboard(this, s);
        }

        private void AddCoolDown(string key, int secs)
        {
            CDL.put(key, new CoolDown(key, secs * 20));
        }

        private CoolDown GetCooldown(string key)
        {
            return GetCooldown(key, false);
        }

        private CoolDown GetCooldown(string key, bool checkvalid)
        {
            if (!CDL.ContainsKey(key)) return null;
//        CyberCoreMain.getInstance().getLogger().info(" VALID"+key);
            CoolDown cd = CDL.get(key);
            if (cd == null) return null;
//        CyberCoreMain.getInstance().getLogger().info("CVALID"+!cd.isValidTick()+" | "+cd.Time+"|"+Server.getInstance().getTick());
            if (checkvalid && !cd.isValid())
            {
//            CyberCoreMain.getInstance().getLogger().info(" EXPIRED "+key);
                CDL.remove(key);
                return null;
            }

//        CyberCoreMain.getInstance().getLogger().info(" GOOD "+key);
            return cd;
//
//
//        for (CoolDown c : (List<CoolDown>) CDL.clone()) {
//            CyberCoreMain.getInstance().getLogger().info("CHECK KEY"+key +" == "+ c.Key);
//            if (c.Key.equalsIgnoreCase(key)) {
//                CyberCoreMain.getInstance().getLogger().info("CHECK VALID"+checkvalid +" == "+ !c.isValidTick());
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
            sendMessage(TextFormat.GREEN + "You are now out of Combat!");
        }

        

        public bool onUpdate(int currentTick)
        {
            //Check for Faction!
            if (this.spawned)
            {
                if (currentTick % 5 == 0) //Only allows 4 Ticks per Sec
                    if (!CooldownLock && isAlive() && spawned)
                    {
                        CooldownLock = true;
                        if (Combat != null)
                            if (Combat.getTick() < currentTick) //No Long in combat
                                leaveCombat();

                        //            CyberCoreMain.getInstance().getLogger().info("RUNNNING "+CDL.size());
                        var fc = GetCooldown(Cooldown_Faction, true);
                        if (fc == null)
                        {
//                    CyberCoreMain.getInstance().getLogger().info("RUNNNING FACTION CHECK IN CP" + CDL.size());
                            AddCoolDown(Cooldown_Faction, 60); //3 mins
                            if (Faction == null)
                            {
                                Faction f = CyberCoreMain.getInstance().FM.FFactory.IsPlayerInFaction(this);
                                if (f == null)
                                    Faction = null;
                                else
                                    Faction = f.GetName();
                            }

                            //Check to See if Faction Invite Expired
                            if (FactionInvite != null && FactionInviteTimeout > 0)
                            {
                                int t = CyberCoreMain.getInstance().GetIntTime();
                                if (t < FactionInviteTimeout)
                                {
                                    Faction fac = CyberCoreMain.getInstance().FM.FFactory.getFaction(FactionInvite);
                                    fac.BroadcastMessage(TextFormat.YELLOW + getName() +
                                                         " has declined your faction invite");
                                    ClearFactionInvite(true);
                                }
                            }
                        }

                        //Delay Teleport
                        var tc = GetCooldown(Cooldown_DTP, true);
                        if (tc == null)
                        {
//                    CyberCoreMain.getInstance().getLogger().info("RUNNNING CLASS CHECK IN CP" + CDL.size()+"||"+ getPlayerClass());
                            AddCoolDown(Cooldown_DTP, 1);
                            if (isWaitingForTeleport())
                            {
                                teleport(WaitingForTPPos);
                                clearWaitingForTP();
                            }
                        }
                        //Class Check

                        //FIX HERE
                        //TODO FIX HERE
                        var cc = GetCooldown(Cooldown_Class, true);
                        if (cc == null)
                        {
//                    CyberCoreMain.getInstance().getLogger().info("RUNNNING CLASS CHECK IN CP" + CDL.size()+"||"+ getPlayerClass());
                            AddCoolDown(Cooldown_Class, 5);
                            BaseClass bc = getPlayerClass();
                            if (bc != null) bc.onUpdate(currentTick);
                            initAllClassBuffs();
                        }

                        var sc = GetCooldown(Scoreboard_Class, true);
                        if (sc == null)
                        {
//                    CyberCoreMain.getInstance().getLogger().info("RUNNNING CLASS CHECK IN CP" + CDL.size()+"||"+ getPlayerClass());
                            AddCoolDown(Scoreboard_Class, 3);
                            ReloadScoreBoard();

                            //REMOVE SHOP/AH/SpawnShop GUI Items
                            var k = 0;
                            for (Item i :
                            getInventory().getContents().values()) {
                                if (i.hasCompoundTag())
                                {
                                    if (i.getNamedTag().contains(ShopInv.StaticItems.KeyName)) getInventory().remove(i);
                                    if (i.getNamedTag().contains("AHITEM")) getInventory().remove(i);
                                    if (i.getNamedTag().contains(net.yungtechboy1.CyberCore.Factory.Shop.Spawner
                                        .SpawnerShop.StaticItems.KeyName))
                                        getInventory().remove(i);
//                                k++;
                                }
                            }
                        }

                        CooldownLock = false;
                    }


                //Check to see if Player as medic or Restoration
                PlayerFood pf = getFoodData();
                if (TPR != null && TPRTimeout != 0 && TPRTimeout < currentTick && !isInTeleportingProcess)
                {
                    TPRTimeout = 0;
                    CorePlayer cp = CyberCoreMain.getInstance().getCorePlayer(TPR);
                    if (cp != null) cp.sendPopup(TextFormat.YELLOW + "Teleport request expired");
                    sendPopup(TextFormat.YELLOW + "Teleport request expired");
                    TPR = null;
                }

                if (isInTeleportingProcess)
                {
                    sendPopup(TeleportTick + "|" + isTeleporting);
                    if (TeleportTick != 0 && isTeleporting)
                    {
                        if (CTLastPos == null)
                        {
                            CTLastPos = getVector3();
                        }
                        else
                        {
                            sendMessage(CTLastPos.distance(getVector3()) + "");
                            if (CTLastPos.distance(getVector3()) > 3) isTeleporting = false;
                            //            CTLastPos = getVector3();
                        }

                        if (TeleportTick <= currentTick && isTeleporting)
                        {
                            System.out.println("AAAAAA");
                            if (isTeleporting)
                            {
                                removeAllEffects(); //TODO use `removeEffect` to only remove that Effect
                                if (TargetTeleporting != null && !TargetTeleporting.isAlive())
                                {
                                    sendMessage("Error! Player Not found!!");
                                    isInTeleportingProcess = false;
                                    TeleportTick = 0;
                                    CTLastPos = null;
                                    return super.onUpdate(currentTick);
                                }

                                if (TargetTeleporting == null && TargetTeleportingLoc == null)
                                {
                                    sendMessage("Error! No Teleport data found!!!");
                                    isInTeleportingProcess = false;
                                    TeleportTick = 0;
                                    CTLastPos = null;
                                    return super.onUpdate(currentTick);
                                }

                                if (TargetTeleportingLoc != null)
                                {
                                    getLevel().addSound(getVector3(), Sound.MOB_ENDERMEN_PORTAL);
                                    teleport(TargetTeleportingLoc);
                                    TargetTeleportingLoc = null;
                                    TargetTeleporting = null;
                                }
                                else
                                {
                                    getLevel().addSound(getVector3(), Sound.MOB_ENDERMEN_PORTAL);
                                    teleport(TargetTeleporting);
                                    TargetTeleportingLoc = null;
                                    TargetTeleporting = null;
                                }

                                isInTeleportingProcess = false;
                                TeleportTick = 0;
                                CTLastPos = null;
                            }
                        }
                        else if (isInTeleportingProcess && !isTeleporting)
                        {
                            removeAllEffects();
                            sendMessage("Error! you moved too much!");
                            isInTeleportingProcess = false;
                            TeleportTick = 0;
                            CTLastPos = null;
                            TargetTeleportingLoc = null;
                            TargetTeleporting = null;
                        }
                    }
                }
            }

            if (!this.loggedIn) return false;

            var tickDiff = currentTick - this.lastUpdate;

            if (tickDiff <= 0) return true;

            this.messageCounter = 2;

            this.lastUpdate = currentTick;

            if (this.fishing != null)
                if (this.distance(fishing) > 80)
                    this.stopFishing(false);

            if (!this.isAlive() && this.spawned)
            {
                ++this.deadTicks;
                if (this.deadTicks >= 10) this.despawnFromAll();
                return true;
            }

            if (this.spawned)
            {
                this.processMovement(tickDiff);

                this.entityBaseTick(tickDiff);

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

                if (this.isOnFire() && this.lastUpdate % 10 == 0)
                {
                    if (this.isCreative() && !this.isInsideOfFire())
                        this.extinguish();
                    else if (this.getLevel().isRaining())
                        if (this.getLevel().canBlockSeeSky(this))
                            this.extinguish();
                }

                if (!this.isSpectator() && this.speed != null)
                {
                    if (this.onGround)
                    {
                        if (this.inAirTicks != 0) this.startAirTicks = 5;
                        this.inAirTicks = 0;
                        this.highestVector3 = this.y;
                    }
                    else
                    {
                        if (this.checkMovement && !this.isGliding() && !server.getAllowFlight() &&
                            !this.getAdventureSettings().get(AdventureSettings.Type.ALLOW_FLIGHT) &&
                            this.inAirTicks > 20 && !this.isSleeping() && !this.isImmobile() && !this.isSwimming() &&
                            this.riding == null)
                        {
                            var expectedVelocity = -this.getGravity() / (double) this.getDrag() - -this.getGravity() /
                                (double) this.getDrag() *
                                Math.exp(-(double) this.getDrag() *
                                         (double) (this.inAirTicks - this.startAirTicks));
                            var diff = (this.speed.y - expectedVelocity) * (this.speed.y - expectedVelocity);

                            int block = level.getBlock(this).getId();
                            var ignore = block == Block.LADDER || block == Block.VINES || block == Block.COBWEB;

                            if (!this.hasEffect(Effect.JUMP) && diff > 0.6 && expectedVelocity < this.speed.y &&
                                !ignore)
                            {
                                if (this.inAirTicks < 100)
                                    this.setMotion(new Vector3(0, expectedVelocity, 0));
                                else if (this.kick(PlayerKickEvent.Reason.FLYING_DISABLED,
                                    "Flying is not enabled on this server")) return false;
                            }

                            if (ignore) this.resetFallDistance();
                        }

                        if (this.y > highestVector3) this.highestVector3 = this.y;

                        if (this.isGliding()) this.resetFallDistance();

                        ++this.inAirTicks;
                    }

                    if (this.isSurvival() || this.isAdventure())
                        if (getFoodData() != null)
                            getFoodData().update(tickDiff);
                }
            }

            this.checkTeleportVector3();
            this.checkInteractNearby();

            if (this.spawned && this.dummyBossBars.size() > 0 && currentTick % 100 == 0)
                this.dummyBossBars.values().forEach(DummyBossBar::updateBossEntityVector3);


            return true;
        }

        public void ReloadMasterEnchatingList()
        {
            MasterEnchantigList = null;
        }

        public List<Enchantment> GetStoredEnchants()
        {
            return MasterEnchantigList;
        }

        public List<Enchantment> GetStoredEnchants(CustomEnchantment.Tier tier, int i, Item item)
        {
            if (MasterEnchantigList == null) MasterEnchantigList = CustomEnchantment.GetRandomEnchant(tier, 3, item);
            return MasterEnchantigList;
        }

        public void LoadHomes(List<Faction.HomeData> hd)
        {
            HD.addAll(hd);
        }

        public bool CheckHomeKey(string key)
        {
            for (Manager.Factions.Faction.HomeData h :
            HD) {
                if (h.getName().equalsIgnoreCase(key)) return true;
            }
            return false;
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
            for (Manager.Factions.Faction.HomeData h :
            HD) {
                if (h.getName().equalsIgnoreCase(key))
                {
                    Vector3 v3 = h.toVector3();
                    if (instant) teleport(v3);
                    else
                        StartTeleport(h.toVector3(getLevel()), 7);
                }
            }
        }

        public bool CanAddHome()
        {
            return HD.size() < MaxHomes;
        }

        public void DelHome(string name)
        {
            var k = 0;
            var kk = 0;
            for (Manager.Factions.Faction.HomeData h :
            HD) {
                k++;
                if (h.getName().equalsIgnoreCase(name))
                {
                    kk = 1;
                    break;
                }
            }
            if (kk == 1) HD.remove(k);
        }

        public void AddHome(string name)
        {
            Vector3 v = getVector3();
            HD.add(new Faction.HomeData(name, this));
        }

        public void AddHome(Faction.HomeData homeData)
        {
            HD.add(homeData);
        }

        public PlayerSettingsData getSettingsData()
        {
            return SettingsData;
        }

        public void setSettingsData(PlayerSettingsData settingsData)
        {
            SettingsData = settingsData;
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

        public void StartTeleport(Vector3 v3, Player pl, int delay)
        {
            BeginTeleportEffects(new Location(v3.x, v3.y, v3.z, pl.getLevel()), delay);
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
            BeginTeleportEffects(corePlayer.getLocation(), delay);
        }

        private void BeginTeleportEffects(Vector3 pos, int delay)
        {
            Effect e1 = Effect.getEffect(9);
            Effect e2 = Effect.getEffect(2);
            e1.setAmplifier(2);
            e2.setAmplifier(2);
            e1.setDuration(20 * 600);
            e2.setDuration(20 * 600);
            addEffect(e1);
            addEffect(e2);
            isTeleporting = true;
            isInTeleportingProcess = true;
            TeleportTick = getServer().getTick() + 20 * delay;
            TargetTeleporting = null;
            TargetTeleportingLoc = pos;
        }

        public void ClearFactionInvite()
        {
            ClearFactionInvite(false);
        }


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

        public void ClearFactionInvite(bool fromfac)
        {
            if (fromfac)
            {
                Faction f = CyberCoreMain.getInstance().FM.FFactory.getFaction(FactionInvite);
                if (f != null)
                    f.DelInvite(getName().toLowerCase());
                else
                    getServer().getLogger().error("ERROR! Faction from CorePlayer invite not found! E3333");
            }

            FactionInvite = null;
            FactionInviteTimeout = -1;
        }

        

        public void completeLoginSequence()
        {
            PlayerLoginEvent ev;
            this.server.getPluginManager().callEvent(ev = new PlayerLoginEvent(this, "Plugin reason"));
            if (ev.isCancelled())
            {
                this.close(this.getLeaveMessage(), ev.getKickMessage());
                return;
            }

            StartGamePacket startGamePacket = new StartGamePacket();
            startGamePacket.entityUniqueId = this.id;
            startGamePacket.entityRuntimeId = this.id;
            startGamePacket.playerGamemode = this.gamemode;
            startGamePacket.x = (float) this.x;
            startGamePacket.y = (float) this.y;
            startGamePacket.z = (float) this.z;
            startGamePacket.yaw = (float) this.yaw;
            startGamePacket.pitch = (float) this.pitch;
            startGamePacket.seed = -1;
            startGamePacket.dimension = (byte) (this.level.getDimension() & 0xff);
            startGamePacket.worldGamemode = this.gamemode;
            startGamePacket.difficulty = this.server.getDifficulty();
            startGamePacket.spawnX = (int) this.x;
            startGamePacket.spawnY = (int) this.y;
            startGamePacket.spawnZ = (int) this.z;
            startGamePacket.hasAchievementsDisabled = true;
            startGamePacket.dayCycleStopTime = -1;
            startGamePacket.eduMode = false;
            startGamePacket.hasEduFeaturesEnabled = true;
            startGamePacket.rainLevel = 0;
            startGamePacket.lightningLevel = 0;
            startGamePacket.commandsEnabled = this.isEnableClientCommand();
            startGamePacket.gameRules = getLevel().getGameRules();
            startGamePacket.levelId = "";
            startGamePacket.worldName = this.getServer().getNetwork().getName();
            startGamePacket.generator = 1; //0 old, 1 infinite, 2 flat
            this.dataPacket(startGamePacket);

            this.dataPacket(new AvailableEntityIdentifiersPacket());

            this.loggedIn = true;

            this.level.sendTime(this);

            //todo cHANGE
            this.setMovementSpeed(DEFAULT_SPEED);
            sendAttributes();
            this.setNameTagVisible(true);
            this.setNameTagAlwaysVisible(true);
            this.setCanClimb(true);

            this.server.getLogger().info(this.getServer().getLanguage().translateString("nukkit.player.logIn",
                TextFormat.AQUA + this.username + TextFormat.WHITE,
                this.ip,
                string.valueOf(this.port),
                string.valueOf(this.id),
                this.level.getName(),
                string.valueOf(NukkitMath.round(this.x, 4)),
                string.valueOf(NukkitMath.round(this.y, 4)),
                string.valueOf(NukkitMath.round(this.z, 4))));

            if (this.isOp() || this.hasPermission("nukkit.textcolor")) this.setRemoveFormat(false);

            this.server.addOnlinePlayer(this);
            this.server.onPlayerCompleteLoginSequence(this);
        }

        

        public void heal(EntityRegainHealthEvent source)
        {
            this.server.getPluginManager().callEvent(source);
            if (source.isCancelled()) return;
            if (getHealth() >= 20) return;
            this.setHealth(getHealth() + source.getAmount());
        }

        public void sendHealth()
        {
            //TODO: Remove it in future! This a hack to solve the client-side absorption bug! WFT Mojang (Half a yellow heart cannot be shown, we can test it in local gaming)
            Attribute attr = Attribute.getAttribute(Attribute.MAX_HEALTH)
                .setMaxValue(this.getAbsorption() % 2 != 0 ? getMaxHealth() + 1 : getMaxHealth())
                .setValue(health > 0 ? health < getMaxHealth() ? health : getMaxHealth() : 0);

//        System.out.println("PRINGING A LINE HEEERRRRRRRRRR");
            if (this.spawned)
            {
//        System.out.println("PRINGING A LINE SEEEEEEEEEEEEEEEEEEEEE");
                UpdateAttributesPacket pk = new UpdateAttributesPacket();
                pk.entries = new[] {attr};
                pk.entityId = this.id;
                this.dataPacket(pk);
            }
        }

        

        public PlayerFood getFoodData()
        {
            return foodData;
        }

        

        protected void processLogin()
        {
            super.processLogin();
            PlayerFood pf = getFoodData();
            foodData = new CustomPlayerFood(this, pf.getLevel(), pf.getFoodSaturationLevel());
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
            return f.GetDisplayName();
        }

        public Faction getFaction()
        {
            if (Faction == null) return null;
            return CyberCoreMain.getInstance().FM.FFactory.getFaction(Faction);
        }

        public bool isWaitingForTeleport()
        {
            if (!(WaitingForTP == -1) && WaitingForTPPos != null)
            {
                if (WaitingForTPStartPos != null &&
                    WaitingForTPStartPos.distance(WaitingForTPPos.asVector3f().asVector3()) > WaitingForTPCD)
                {
                    sendMessage("Error! You moved too much so your teleport was canceled");
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
            WaitingForTP = -1;
            WaitingForTPPos = null;
        }

        public bool delayTeleport(int i, Vector3 pos, bool giveeffects)
        {
            return delayTeleport(i, pos, giveeffects, 2);
        }

        public bool delayTeleport(int i, Vector3 pos)
        {
            return delayTeleport(i, pos, true, 2);
        }

        public bool delayTeleport(int i, Vector3 pos, bool giveeffects, int canceltpdistance)
        {
            if (isWaitingForTeleport())
            {
                sendMessage(TextFormat.RED + "Error! You are already waiting for a teleport!");
                return false;
            }

            WaitingForTP = i;
            WaitingForTPStartPos = getVector3().clone();
            WaitingForTPPos = pos.clone();
            WaitingForTPEffects = giveeffects;
            WaitingForTPCD = canceltpdistance;
            if (WaitingForTPEffects)
            {
                addEffect(Effect.getEffect(Effect.NAUSEA).setDuration(20 * 5));
                addEffect(Effect.getEffect(Effect.SLOWNESS).setDuration(20 * 5));
            }

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
//        this.forceMovement = this.teleportVector3 = this.getVector3();
//
//        ResourcePacksInfoPacket infoPacket = new ResourcePacksInfoPacket();
//        infoPacket.resourcePackEntries = this.server.getResourcePackManager().getResourceStack();
//        infoPacket.mustAccept = this.server.getForceResources();
//        this.dataPacket(infoPacket);
//    }

        public class CombatData
        {
            public readonly int CombatTime = 20 * 7; // 7 Secs
            public int Tick = -1;

            public CombatData(int tick)
            {
                Tick = tick;
            }

            public int getTick(int a)
            {
                return Tick + a;
            }

            public int getTick()
            {
                return getTick(CombatTime);
            }
        }
    }