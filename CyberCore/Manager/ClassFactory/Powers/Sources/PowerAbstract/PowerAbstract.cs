using System;
using System.Collections.Generic;
using CyberCore.Custom.Events;
using CyberCore.CustomEnums;
using CyberCore.Manager.ClassFactory.Window;
using CyberCore.Utils;
using CyberCore.Utils.Cooldowns;
using fNbt;
using MiNET.Blocks;
using MiNET.Effects;
using MiNET.Items;
using MiNET.Utils;
using static CyberCore.CustomEnums.HotbarStatus;

namespace CyberCore.Manager.ClassFactory.Powers
{
    public abstract class PowerAbstract
    {
        public static readonly string PowerHotBarNBTTag = "PowerHotBar";

        //    private int PowerSuccessChance = 0;
        private long _lasttick = -1;
        private bool AbilityActive;

        //    private LevelingType LT;
        private bool Active;
        public bool CanSendCanNotRunMessage = true;

        //    public int TickUpdate = -1;
        public CoolDown Cooldown;
        protected long DeActivatedTick = -1;
        private long DurationTick = -1;
        private bool Enabled;
        public bool isDefaultPower = false;

        private LockedSlot LS = LockedSlot.NA;
        public PowerType MainPowerType = PowerType.Regular;

        public BaseClass PlayerClass;
        public bool PlayerToggleable = true;
        private double PowerSourceCost;
        public PowerSettings PS;
        public PowerType SecondaryPowerType = PowerType.None;
        // private ClassLevelingManagerStage SLM;
        public bool TakePowerOnFail = false;
        // private ClassLevelingManagerXPLevel XLM;
        // public ClassLevelingManager XPManager;
        public ClassLevelingManager XPManager;

        public PowerAbstract(AdvancedPowerEnum ape, ClassLevelingManager xpm = null)
        {
            XPManager = xpm ?? getDefaultClassLevelingManager();
            if (ape == null) ape = getAPE();
            if (!ape.isValid())
            {
                CyberCoreMain.Log.Error("Was LOG ||" + "Error! 2APE is not valid!");
//            if(this instanceof StagePowerAbstract){
                try
                {
                    if (GetType() == typeof(StagePowerAbstract))
                    {
                        CyberCoreMain.Log.Error("Was LOG ||" + "IS STAGE ABSTRACT!!!!!");
                        ape = new AdvancedPowerEnum(ape.getPowerEnum(), StageEnum.STAGE_1);
                    }
                    else
                    {
                        ape = new AdvancedPowerEnum(ape.getPowerEnum(), 0);
                    }
                }
                catch (Exception e)
                {
                    CyberCoreMain.Log.Error("Was LOG ||" + "Error!eeeeeeeeeeeeeeeeeeee", e);
                    return;
                }

//            }else return;
            }

            if (ape.isStage())
                loadLevelManager(new ClassLevelingManagerStage(ape.getStageEnum()));
            else
                loadLevelManager(new ClassLevelingManagerXPLevel(ape.getXP()));
        }

        protected abstract ClassLevelingManager getDefaultClassLevelingManager();


        public PowerAbstract(BaseClass b, AdvancedPowerEnum ape, PowerSettings ps)
        {
            if (!ape.isValid())
            {
                CyberCoreMain.Log.Error("Was LOG ||" + "Error! APE is not valid!22222");
                return;
            }

            PlayerClass = b;
            if (ape.isStage())
                loadLevelManager(new ClassLevelingManagerStage(ape.getStageEnum()));
            else
                loadLevelManager(new ClassLevelingManagerXPLevel(ape.getXP()));

            if (ps != null)
                setPowerSettings(ps);
            else
                CyberCoreMain.Log.Warn("POWER ABSTRACT ERROR! NO POWER SOURCE ASSIGNED FOR " + getName());

//        Level = lvl;
//        Level = b.getLVL();
            initStages();
            initAfterCreation();
        }

        public PowerAbstract(BaseClass b, AdvancedPowerEnum ape)
        {
            if (ape == null) ape = new AdvancedPowerEnum(getType());
            if (!ape.isValid())
            {
                CyberCoreMain.Log.Error("Was LOG ||" + "Error! APE is not valid!");
//            if(this instanceof StagePowerAbstract){
                try
                {
                    if (GetType() == typeof(StagePowerAbstract))
                    {
                        CyberCoreMain.Log.Error("Was LOG ||" + "IS STAGE ABSTRACT!!!!!");
                        ape = new AdvancedPowerEnum(ape.getPowerEnum(), StageEnum.STAGE_1);
                    }
                    else
                    {
                        ape = new AdvancedPowerEnum(ape.getPowerEnum(), 0);
                    }
                }
                catch (Exception e)
                {
                    CyberCoreMain.Log.Error("Was LOG ||" + "Error!eeeeeeeeeeeeeeeeeeee", e);
                    ;
                    return;
                }

//            }else return;
            }

            PlayerClass = b;
            if (ape.isStage())
                loadLevelManager(new ClassLevelingManagerStage(ape.getStageEnum()));
            else
                loadLevelManager(new ClassLevelingManagerXPLevel(ape.getXP()));

            if (getPowerSettings() == null)
                CyberCoreMain.Log.Warn("POWER ABSTRACT ERROR! NO POWER SOURCE ASSIGNED FOR " + getName());
//        Level = lvl;
//        Level = b.getLVL();
            initStages();
            initAfterCreation();
        }

//    public PowerAbstract(BaseClass b, ClassLevelingManager lt, int psc) {
//        this(b, lt, null, psc);
//    }
//
//    public PowerAbstract(BaseClass b, ClassLevelingManager lt, int psc, double cost) {
//        this(b, lt, null, psc, cost);
//    }
//
//    public PowerAbstract(BaseClass b, ClassLevelingManager lt, PowerSettings ps, int psc) {
//        this(b, lt, ps, psc, 5);
//    }

        public PowerAbstract(BaseClass b)
        {
            PlayerClass = b;
            loadLevelManager(new ClassLevelingManagerXPLevel());
            if (getPowerSettings() == null)
                CyberCoreMain.Log.Warn("POWER ABSTRACT ERROR! NO POWER SOURCE ASSIGNED FOR " + getName());
            initStages();
            initAfterCreation();
        }

//    public PowerAbstract(BaseClass b, Integer xp, PowerSettings ps, int psc, double cost) {
////        PowerSuccessChance = psc;
//        PlayerClass = b;
//        loadLevelManager(new ClassLevelingManagerXPLevel(xp));
//        if (ps != null) {
//            setPowerSettings(ps);
//        } else
//            CyberCoreMain.Log..warning("POWER ABSTRACT ERROR! NO POWER SOURCE ASSIGNED FOR " + getName());
////        Level = lvl;
////        Level = b.getLVL();
//        initStages();
//        initAfterCreation();
//        PowerSourceCost = cost;
//    }

        public PowerAbstract(BaseClass b, StageEnum stageEnum)
        {
            PlayerClass = b;
            loadLevelManager(new ClassLevelingManagerStage(stageEnum));
            if (getPowerSettings() == null)
                CyberCoreMain.Log.Warn("POWER ABSTRACT ERROR! NO POWER SOURCE ASSIGNED FOR " + getName());
            initStages();
            initAfterCreation();
        }

        public bool Loaded { get; set; }
        public Reqs Requirement { get; set; }

        public class Reqs
        {
            public Reqs()
            {
                
            }

            public bool Pass(BaseClass baseClass)
            {
                throw new NotImplementedException();
            }
        }
        
        public int getTickUpdate()
        {
            if (isAbility()) return 20;
            if (isHotbarPower()) return 20 * 5;
            return -1;
        }

//    public PowerAbstract(BaseClass b, StageEnum stageEnum, PowerSettings ps) {
//        PlayerClass = b;
//        loadLevelManager(new ClassLevelingManagerStage(stageEnum));
//        if (ps != null) {
//            setPowerSettings(ps);
//        } else
//            CyberCoreMain.Log..warning("POWER ABSTRACT ERROR! NO POWER SOURCE ASSIGNED FOR " + getName());
////        Level = lvl;
////        Level = b.getLVL();
//        initStages();
//        initAfterCreation();
//    }

//    public PowerAbstract(BaseClass b, AdvancedPowerEnum advancedPowerEnum, int i) {
//        CyberCoreMain.Log.Error("Was LOG ||"+"NOT IMPLEMENTEDDDDDD");
//    }

        public CoolDown getCooldown()
        {
            if (Cooldown == null || !Cooldown.isValid()) return null;

            return Cooldown;
        }

        public List<Type> getAllowedClasses()
        {
            return new List<Type>();
        }


        public PowerSettings getPowerSettings()
        {
            if (PS == null)
                CyberCoreMain.Log.Error("POWER ABSTRACT ERROR! NO POWER SOURCE ASSIGNED FOR " + getName());
            return PS;
        }

        public void setPowerSettings(PowerSettings ps)
        {
            PS = ps;
        }

        protected void setPowerSettings(bool ability, bool effect, bool hotbar, bool passive)
        {
            if (getPowerSettings() == null) PS = new PowerSettings();
            getPowerSettings().isAbility = ability;
            getPowerSettings().isEffect = effect;
            getPowerSettings().isHotbar = hotbar;
            getPowerSettings().isPassive = passive;
        }

        public void activate()
        {
            if (isAbilityActive()) return;
            ActivateAbility();
            onAbilityActivate();
//        onActivate();
        }

        public abstract void onAbilityActivate();

        public long getDeActivatedTick()
        {
            return DeActivatedTick;
        }

        public void setDeActivatedTick(int deActivatedTick)
        {
            DeActivatedTick = deActivatedTick;
        }

        public void loadLevelManager(ClassLevelingManager lm)
        {
            if (lm == null) return;
            XPManager = lm;
        }

        public long getDurationTick()
        {
            return DurationTick;
        }

        public void setDurationTick(int t)
        {
            DurationTick = t;
        }
//
//    public LevelingType getLevelingType() {
//        return LT;
//    }
//
//    
//    public LevelingType getLT() {
//        return LT;
//    }

        public bool isActive()
        {
            return Active;
        }

        public void setActive(bool active)
        {
            if (active)
            {
                if (!hasPowerSettings())
                {
                    CyberCoreMain.Log.Error("Was LOG ||" + "====> CAN NOT ACTIVATE POWER NO POWER SETTINGS!!!");
                    return;
                }

                if (getPowerSettings().isHotbar && isLSNull())
                {
                    CyberCoreMain.Log.Error(
                        "Was LOG ||" + "====> CAN NOT ACTIVATE POWER NO HOT BAR SLOT IN SETTINGS!!!");
                    return;
                }

                DeActivatedTick = CyberUtils.getTick() + getRunTimeTick();
            }

            Active = active;
        }

        public bool isEnabled()
        {
            return Enabled;
        }

        private void setEnabled(bool ee)
        {
            Enabled = ee;
        }

        public void enablePower()
        {
            enablePower(getLS());
        }

        public void enablePower(LockedSlot ls)
        {
            if (!getLS().Equals(ls)) setLS(ls);
            if (!hasPowerSettings())
            {
                CyberCoreMain.Log.Error("Was LOG ||" + "====> CAN NOT ENAVBLE POWER NO POWER SETTINGS!!!");
                return;
            }

            if (getPowerSettings().isHotbar && isLSNull())
            {
                CyberCoreMain.Log.Error("Was LOG ||" + "====> CAN NOT ENAVBLE POWER NO HOT BAR SLOT IN SETTINGS!!!" +
                                        GetType().Name);
                return;
            }

            setEnabled(true);
            PlayerClass.getClassSettings().delLearnedPowerAndLearnIfNotEqual(getAPE());
//        PlayerClass.getClassSettings().delLearnedPower(getType());
//        PlayerClass.getClassSettings().learnNewPower(getAPE(),true);
            if (!PlayerClass.getClassSettings().getEnabledPowers().Contains(getType()))
                PlayerClass.getClassSettings().getEnabledPowers().Add(getType());
            PlayerClass.getClassSettings().setPreferedSlot(getLS(), getType());
            onEnable();
        }

        public AdvancedPowerEnum getAPE()
        {
            if (XPManager != null)
                try
                {
                    return new AdvancedXPPowerEnum(getType(), getXP());
                }
                catch (Exception e)
                {
                    CyberCoreMain.Log.Error("Was LOG ||" + "ERRORR ###@@@ " + this + " || " + GetType().Name, e);
                    return null;
                }

            return new AdvancedPowerEnum(getType(), XPManager.getXP());
        }

        private int getXP()
        {
            if (XPManager is ClassLevelingManagerXPLevel)
            {
                return XPManager.getXP();
            }
            return XPManager.getStage().Level;
        }

        public void onEnable()
        {
            if (hasPowerSettings())
            {
                var ps = getPowerSettings();
                if (ps.isHotbar)
                {
                    if (getLS().Equals(LockedSlot.NA))
                    {
                        CyberCoreMain.Log.Error("Was LOG ||" + "Error! No Valid locked slot set!");
                        getPlayer().SendMessage("Error! No Valid locked slot set!");
                    }
                    else
                    {
                        sendHotbarItemToLS(Success);
                    }
                }
            }
        }

        private void sendHotbarItemToLS(HotbarStatus hbs)
        {
            var i = getHotbarItem(hbs);
            var pi = getPlayer().Inventory;
            var ii = (Item) pi.Slots[getLS().getSlot()].Clone();
            pi.Slots[getLS().getSlot()] = new ItemAir();
            if (ii == null || ii.ExtraData != null && ii.ExtraData.Contains(PowerHotBarNBTTag)) ii = null;
            CyberCoreMain.Log.Error("Was LOG ||" + ">>>>>>>>>>>>>Data::: " + getLS() + "|||||" + i);
            try
            {
                if (i == null)
                {
                    CyberCoreMain.Log.Error("Was LOG ||" + "Error! WTF ITEM IS NELL");
                    return;
                }

                if (getLS().Equals(LockedSlot.NA))
                {
                    CyberCoreMain.Log.Error("Was LOG ||" + "Error! LS IS NA aaaaaaaaaaaaaaaNELL");
                    return;
                }

                pi.Slots[getLS().getSlot()] = i;
                if (ii != null && ii.Id != 0 && ii.Id != -1) pi.AddItem(ii, true);
            }
            catch (Exception e)
            {
                CyberCoreMain.Log.Error("Was LOG ||" + "Ahhhhhh Fuck!");
            }
        }

        public Item getHotbarItem(HotbarStatus hbs)
        {
            Item i = null;
            switch (hbs)
            {
                case Fail:
                    i = getHotbarItem_Fail();
                    break;
                case Success:
                    i = getHotbarItem_Success();
                    break;
                default:
                    i = getHotbarItem_Cooldown();
                    break;
            }

            if (!i.hasCompoundTag()) i.setCompoundTag(new NbtCompound());
            i.getNamedTag().putBoolean(PowerHotBarNBTTag, true);
            return i;
        }

        public Item getHotbarItem_Success()
        {
            Item i = new ItemApple();
            i.setCustomName(getDispalyName() + " Power");
            i.setLore(new List<string>
            {
                ChatColors.Green + "Ready for Use!",
                ChatColors.Gray + "Cooldown: " + getCooldownTimeSecs() + " Secs"
            }.ToArray());
            return i;
        }

        public Item getHotbarItem_Fail()
        {
            Item i = new ItemRedstone();
            i.setCustomName(getDispalyName() + " Power");
            i.setLore(new List<string>
            {
                ChatColors.Red + "Power Failed Please Wait!",
                ChatColors.Gray + "Cooldown: " + getCooldownTimeSecs() + " Secs"
            }.ToArray());
            return i;
        }

        public Item getHotbarItem_Cooldown()
        {
            Item i = new ItemBlock(new Glowstone());
            i.setCustomName(getDispalyName() + " Power");
            i.setLore(new List<string>
            {
                ChatColors.Yellow + "Power is in Cooldown!",
                ChatColors.Red + "Cooldown Time Left: " +
                (getCooldown().getTime() - CyberUtils.getLongTime() + " Secs",
                    ChatColors.Gray + "Cooldown: " + getCooldownTimeSecs() + " Secs")
            }.ToArray());
            return i;
        }

        public bool hasPowerSettings()
        {
            return getPowerSettings() != null;
        }

        public bool isLSNull()
        {
            return getLS().Equals(LockedSlot.NA);
        }

        public int getRunTimeTick()
        {
            return getStage().getValue() * 20; //1-5 Secs
        }

        public void setActive()
        {
            setActive(true);
        }

        public StageEnum getStage()
        {
            if (XPManager is ClassLevelingManagerStage)
                return ((ClassLevelingManagerStage)XPManager).getStage();
            return StageEnum.NA;
        }

        public StageEnum formatLeveltoStage(int lvl)
        {
            return StageEnum.getStageFromInt(1 + lvl / 20);
        }

        public void onActivate()
        {
        }

        public LockedSlot getLS()
        {
            return LS;
        }

        public void setLS(LockedSlot ls)
        {
            CyberCoreMain.Log.Error("Was LOG ||" + "LOCKEDSLOT SET FOR " + GetType().Name);
            PlayerClass.HotBarPowers[ls] = this;
            LS = ls;
        }

        public double getPowerSourceCost()
        {
            return PowerSourceCost;
        }

        public void setPowerSourceCost(double powerSourceCost)
        {
            PowerSourceCost = powerSourceCost;
        }

        public int getPowerSuccessChance()
        {
            return 100;
        }

        public int getDefaultPowerSuccessChance()
        {
            var l = PlayerClass.getLVL();
            var f = -Math.Sin(l / 90) * 13 + Math.Sin(-50 + l / 80);
            return (int) (f * 100);
        }

        public void initAfterCreation()
        {
        }

        public CorePlayer getPlayer()
        {
            return PlayerClass.getPlayer();
        }

        public PlayerTakeDamageEvent PlayerTakeDamageEvent(PlayerTakeDamageEvent e)
        {
            return e;
        }

        //TODO IMPLEMENT
        // public Event handelEvent(Event event) {
        //     if (event instanceof PlayerTakeDamageEvent)
        //     return PlayerTakeDamageEvent((PlayerTakeDamageEvent) event);
        //     if (event instanceof EntityDamageEvent)
        //     return EntityDamageEvent((EntityDamageEvent) event);
        //     if (event instanceof CustomEntityDamageByEntityEvent)
        //     return CustomEntityDamageByEntityEvent((CustomEntityDamageByEntityEvent) event);
        //     if (event instanceof PlayerJumpEvent)
        //     return PlayerJumpEvent((PlayerJumpEvent) event);
        //     if (event instanceof InventoryTransactionEvent)
        //     return InventoryTransactionEvent((InventoryTransactionEvent) event);
        //     if (event instanceof InventoryClickEvent)
        //     return InventoryClickEvent((InventoryClickEvent) event);
        //     if (event instanceof EntityInventoryChangeEvent) {
        //         return EntityInventoryChangeEvent((EntityInventoryChangeEvent) event);
        //     }
        //     return event;
        // }

        //    public EntityDamageEvent EntityDamageEvent(EntityDamageEvent e)
        //    {
        //        return e;
        //    }
        //
        //    public InventoryClickEvent InventoryClickEvent(InventoryClickEvent e)
        //    {
        //        return e;
        //    }
        //
        //    public InventoryTransactionEvent InventoryTransactionEvent(InventoryTransactionEvent e)
        //    {
        //        return e;
        //    }
        //
        //    public EntityInventoryChangeEvent EntityInventoryChangeEvent(EntityInventoryChangeEvent e)
        //    {
        //        return e;
        //    }
        //
        //    public PlayerJumpEvent PlayerJumpEvent(PlayerJumpEvent e)
        //    {
        //        return e;
        //    }
        //
        //    /**
        // * ALWAYS RETURN THE EVENT
        // *
        // * @param e
        // * @return e Event
        // */
        //    public abstract CustomEntityDamageByEntityEvent CustomEntityDamageByEntityEvent(
        //        CustomEntityDamageByEntityEvent e);

        /**
         * Time in Secs
         * 
         * @return int Time in secs
         */
        protected int getCooldownTimeSecs()
        {
            return 60 * 3; //3 Mins
        }

        public string getSafeName()
        {
            return getName().Replace(" ", "_");
        }

        public int getCooldownTimeTick()
        {
            return getCooldownTimeSecs() * 20;
        }

        public void initStages()
        {
        }

//    public void importConfig(Dictionary<String,Object> cs) {
//        if (LM != null) LM.importConfig(cs);
//    }
//
//    public Dictionary<String,Object> exportConfig() {
//        Dictionary<String,Object> c = new Dictionary<String,Object>();
//        if (LM != null) c.put("LM", LM.exportConfig());
//        return c;
//    }

        public void handleTick(long tick)
        {
//        CyberCoreMain.Log.Error("Was LOG ||"+"PowerAbstract Call TICK");
//        CyberCoreMain.Log.Error("Was LOG ||"+"PowerAbstract Call TICK 1");
            if (getTickUpdate() == -1) return;
            if (_lasttick + getTickUpdate() < tick)
            {
//            CyberCoreMain.Log.Error("Was LOG ||"+"PowerAbstract Called THE ACTUAL TICK");
                onTick(tick);
                _lasttick = tick;
            }
        }

        public AdvancedPowerEnum getAdvancedPowerEnum()
        {
            if (XPManager is ClassLevelingManagerStage)
                try
                {
                    var s = getStage();
                    if (s.Level == StageEnum.NA.Level)
                        return null;
                    return new AdvancedStagePowerEnum(getType(), s);
                }
                catch (Exception e)
                {
                    CyberCoreMain.Log.Error("PA>>> Err", e);
                    return null;
                }

            if (XPManager is ClassLevelingManagerXPLevel)
                return new AdvancedXPPowerEnum(getType(), XPManager.getXP());
            return new AdvancedPowerEnum(getType());
        }

        public abstract PowerEnum getType();
//    {
//        CyberCoreMain.Log.Error("ERROR GETTING TYPE FROM POWER!!!!!");
//        return PowerEnum.Unknown;
//    }

        //USE TO RUN
        public void initPowerRun(object[] args)
        {
            if (CanRun(false))
            {
                PlayerClass.takePowerSourceCount(PowerSourceCost);
                usePower(args);
                afterPowerRun(args);
            }
            else
            {
                if (Cooldown != null && Cooldown.isValid())
                    if (CanSendCanNotRunMessage)
                        sendCanNotRunMessage();
            }
        }

        public void initForcePowerRun(object[] args)
        {
            PlayerClass.takePowerSourceCount(PowerSourceCost);
            usePower(args);
            afterPowerRun(args);
        }

        public void sendCanNotRunMessage()
        {
            getPlayer().SendMessage(ChatColors.Red + "Error! PowerAbstract " + getDispalyName() + ChatColors.Red +
                                    " still has a " + ChatColors.LightPurple + Cooldown.toString() + ChatColors.Red +
                                    " Cooldown.");
        }

        public void afterPowerRun(object[] args)
        {
            addCooldown();
            getPlayer().SendMessage(getSuccessUsageMessage());
        }

        public Effect getEffect()
        {
            return new Poison();
        }

        public string getSuccessUsageMessage()
        {
            return ChatColors.Green + " > PowerAbstract " + getDispalyName() + ChatColors.Green +
                   " has been activated!";
        }

        public object usePower(params object[] args)
        {
            setActive();
            if (isAbility())
                activate();
            else
                setActive(false);

            return null;
        }

        public bool CanRun(bool force, object[] args = null)
        {
            if (!PlayerClass.CCM.isInSpawn(getPlayer()))
            {
                getPlayer().SendMessage(ChatColors.Red + "Error! can not use " + getDispalyName() + " while in spawn!");
                return false;
            }

            if (force) return true;
            if (PlayerClass.getPowerSourceCount() < PowerSourceCost)
            {
                getPlayer().SendMessage(ChatColors.Red + "Not enough " + PlayerClass.getPowerSourceType() + " Energy!");
                return false;
            }

            var nr = new Random();
            var r = nr.Next(0, 100);
            if (r <= getPowerSuccessChance())
            {
                //Success
                if (Cooldown != null && Cooldown.isValid())
                {
                    CyberCoreMain.Log.Error("Was LOG ||" + "CD FAIL!!!");
                    return false;
                }
            }
            else
            {
                CyberCoreMain.Log.Error("Was LOG ||" + "PSC FAIL!!!" + r + "||" + getPowerSuccessChance());
                return false; //Fail
            }

            if (isAbility())
            {
                CyberCoreMain.Log.Error("Was LOG ||" + "ABILITY " + !isActive());
                return !isActive();
            }

            return true;
        }

        public bool isAbility()
        {
            return getPowerSettings().isAbility;
        }

        // public bool isHotbar()
        // {
        //     return getPowerSettings().isHotbar;
        // }

        public HotbarStatus getCurrentHotbarStatus()
        {
            if (getCooldown() != null) return HotbarStatus.Cooldown;
            if (getPowerState() == PowerState.Fail) return Fail;
            return Success;
        }

        public PowerState getPowerState()
        {
            return PowerState.Idle;
        }

        public void onTick(long tick)
        {
            if (isHotbarPower())
                if (!isLSNull())
                    sendHotbarItemToLS(getCurrentHotbarStatus());

            if (isAbility())
                //Only For Deactivation
//            CyberCoreMain.Log.Error("Was LOG ||"+"POWER TICKKKKKK2");
                if (isAbilityActive())
                {
                    CyberCoreMain.Log.Error("Was LOG ||" + "POWER TICKKKKKK3");
                    whileAbilityActive();
                    if (getDeActivatedTick() != -1 && tick >= getDeActivatedTick())
                    {
                        CyberCoreMain.Log.Error("Was LOG ||" + "POWER TICKKKKKK44444444444444444444444444444");
//                    setEnabled(false);
                        DeactivateAbility();
                        DeActivatedTick = -1;
                        onAbilityDeActivate();
                    }
                }
        }

        public bool isAbilityActive()
        {
            return AbilityActive;
        }

        public void DeactivateAbility()
        {
            AbilityActive = false;
            setActive(false);
        }

        public void ActivateAbility()
        {
            AbilityActive = true;
        }

        public void onAbilityDeActivate()
        {
        }

        public void whileAbilityActive()
        {
        }

        public CoolDown addCooldown()
        {
            return addCooldown(getCooldownTimeSecs());
        }

        public CoolDown addCooldown(int secs)
        {
            Cooldown = new CoolDown(getType().Name, secs * 20);
            return Cooldown;
        }

        public abstract string getName();

        public string getDispalyName()
        {
            return getName();
        }

        /**
         * Button Callback to add a Button to the Window!
         * 
         * @param mainClassSettingsWindow
         */
        public void addButton(MainClassSettingsWindow mainClassSettingsWindow)
        {
//        if()
        }

        public void importAPE(AdvancedPowerEnum pe)
        {
            if (pe.isStage())
            {
            }
        }

        public virtual bool PowerLoad()
        {
            if (isHotbarPower())
            {
                var l = PlayerClass.ClaimFirstOpenPowerSlot();
                if (l.Slot == LockedSlot.NA.Slot)
                {
                    //No Free Slots
                    PlayerClass.P.SendMessage($"{ChatColors.Yellow} Error! There was no free Hotbar Power Slot for " +
                                              getDispalyName());
                    return false;
                }

                setLS(l);
                var h = (PowerHotBarInt) this;
                h.initHotbar(PlayerClass.P);
                h.updateHotbar(getLS(),Cooldown,this);
            }

            return true;
        }

        public bool isHotbarPower()
        {
            return this is PowerHotBarInt;
        }

        public bool Load()
        {
            if (!PowerLoad()) return false;
                Loaded = true;
                PlayerClass.UsaeableClassPowersList.Add(getType(),this);
                return true;
        }

        public void UnLoad()
        {
            Loaded = false;
        }
    }
}