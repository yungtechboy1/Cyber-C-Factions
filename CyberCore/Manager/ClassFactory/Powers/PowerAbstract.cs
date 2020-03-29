using System;
using CyberCore.CustomEnums;
using CyberCore.Utils;
using CyberCore.Utils.Cooldowns;
using MiNET;
using MiNET.Effects;
using MiNET.Items;
using OpenAPI.Events;
using OpenAPI.Events.Entity;

namespace CyberCore.Manager.ClassFactory.Powers
{
    public class PowerAbstract
    {
        public static readonly String PowerHotBarNBTTag = "PowerHotBar";

        public BaseClass PlayerClass = null;

        //    public int TickUpdate = -1;
        public CoolDown Cooldown = null;
        public bool TakePowerOnFail = false;
        public bool PlayerToggleable = true;
        public bool CanSendCanNotRunMessage = true;
        public PowerSettings PS = null;
        public PowerType MainPowerType = PowerType.Regular;
        public PowerType SecondaryPowerType = PowerType.None;
        protected int DeActivatedTick = -1;

        LockedSlot LS = LockedSlot.NA;

        //    private LevelingType LT;
        private bool Active = false;

        //    private int PowerSuccessChance = 0;
        private int _lasttick = -1;
        private double PowerSourceCost = 0;
        private int DurationTick = -1;
        private bool Enabled = false;
        private bool AbilityActive = false;
        private ClassLevelingManagerStage SLM;
        private ClassLevelingManagerXPLevel XLM;

        public PowerAbstract(AdvancedPowerEnum ape)
        {
            if (ape == null) ape = new AdvancedPowerEnum(getType());
            if (!ape.isValid())
            {
                CyberCoreMain.Log.Error("Was LOG ||"+"Error! APE is not valid!");
//            if(this instanceof StagePowerAbstract){
                try
                {
                    if (this instanceof StagePowerAbstract) {
                        CyberCoreMain.Log.Error("Was LOG ||"+"IS STAGE ABSTRACT!!!!!");
                        ape = new AdvancedPowerEnum(ape.getPowerEnum(), StageEnum.STAGE_1);
                    } else ape = new AdvancedPowerEnum(ape.getPowerEnum(), 0);
                }
                catch (Exception e)
                {
                    CyberCoreMain.Log.Error("Was LOG ||"+"Error!eeeeeeeeeeeeeeeeeeee");
                    e.printStackTrace();
                    return;
                }

//            }else return;
            }

            if (ape.isStage())
            {
                loadLevelManager(new ClassLevelingManagerStage(ape.getStageEnum()));
            }
            else
            {
                loadLevelManager(new ClassLevelingManagerXPLevel(ape.getXP()));
            }
        }

        @Deprecated

        public PowerAbstract(BaseClass b, AdvancedPowerEnum ape, PowerSettings ps)
        {
            if (!ape.isValid())
            {
                CyberCoreMain.Log.Error("Was LOG ||"+"Error! APE is not valid!22222");
                return;
            }

            PlayerClass = b;
            if (ape.isStage())
            {
                loadLevelManager(new ClassLevelingManagerStage(ape.getStageEnum()));
            }
            else
            {
                loadLevelManager(new ClassLevelingManagerXPLevel(ape.getXP()));
            }

            if (ps != null)
            {
                setPowerSettings(ps);
            }
            else
                b.getPlayer().getServer().getLogger()
                    .warning("POWER ABSTRACT ERROR! NO POWER SOURCE ASSIGNED FOR " + getName());

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
                CyberCoreMain.Log.Error("Was LOG ||"+"Error! APE is not valid!");
//            if(this instanceof StagePowerAbstract){
                try
                {
                    if (this instanceof StagePowerAbstract) {
                        CyberCoreMain.Log.Error("Was LOG ||"+"IS STAGE ABSTRACT!!!!!");
                        ape = new AdvancedPowerEnum(ape.getPowerEnum(), StageEnum.STAGE_1);
                    } else ape = new AdvancedPowerEnum(ape.getPowerEnum(), 0);
                }
                catch (Exception e)
                {
                    CyberCoreMain.Log.Error("Was LOG ||"+"Error!eeeeeeeeeeeeeeeeeeee");
                    e.printStackTrace();
                    return;
                }

//            }else return;
            }

            PlayerClass = b;
            if (ape.isStage())
            {
                loadLevelManager(new ClassLevelingManagerStage(ape.getStageEnum()));
            }
            else
            {
                loadLevelManager(new ClassLevelingManagerXPLevel(ape.getXP()));
            }

            if (getPowerSettings() == null)
                b.getPlayer().getServer().getLogger()
                    .warning("POWER ABSTRACT ERROR! NO POWER SOURCE ASSIGNED FOR " + getName());
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
                b.getPlayer().getServer().getLogger()
                    .warning("POWER ABSTRACT ERROR! NO POWER SOURCE ASSIGNED FOR " + getName());
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
//            b.getPlayer().getServer().getLogger().warning("POWER ABSTRACT ERROR! NO POWER SOURCE ASSIGNED FOR " + getName());
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
                b.getPlayer().getServer().getLogger()
                    .warning("POWER ABSTRACT ERROR! NO POWER SOURCE ASSIGNED FOR " + getName());
            initStages();
            initAfterCreation();
        }

        public int getTickUpdate()
        {
            if (isAbility()) return 20;
            if (isHotbar()) return 20 * 5;
            return -1;
        }

//    public PowerAbstract(BaseClass b, StageEnum stageEnum, PowerSettings ps) {
//        PlayerClass = b;
//        loadLevelManager(new ClassLevelingManagerStage(stageEnum));
//        if (ps != null) {
//            setPowerSettings(ps);
//        } else
//            b.getPlayer().getServer().getLogger().warning("POWER ABSTRACT ERROR! NO POWER SOURCE ASSIGNED FOR " + getName());
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
            if (Cooldown == null || !Cooldown.isValid())
            {
                return null;
            }

            return Cooldown;
        }

        public ArrayList<Class> getAllowedClasses()
        {
            return new ArrayList<>();
        }

        public ClassLevelingManagerStage getStageLevelManager()
        {
            return SLM;
        }

        public void setSLM(ClassLevelingManagerStage SLM)
        {
            this.SLM = SLM;
        }

        public ClassLevelingManagerXPLevel getXLM()
        {
            return XLM;
        }

        public void setXLM(ClassLevelingManagerXPLevel XLM)
        {
            this.XLM = XLM;
        }


        public PowerSettings getPowerSettings()
        {
            if (PS == null)
                getPlayer().getServer().getLogger()
                    .error("POWER ABSTRACT ERROR! NO POWER SOURCE ASSIGNED FOR " + getName());
            return PS;
        }

        public void setPowerSettings(PowerSettings ps)
        {
            PS = ps;
        }

        protected void setPowerSettings(bool ability, bool effect, bool hotbar, bool passive)
        {
            if (getPowerSettings() == null) PS = new PowerSettings();
            getPowerSettings().setAbility(ability);
            getPowerSettings().setEffect(effect);
            getPowerSettings().setHotbar(hotbar);
            getPowerSettings().setPassive(passive);
        }

        public final void activate()
        {
            if (isAbilityActive()) return;
            ActivateAbility();
            onAbilityActivate();
//        onActivate();
        }

        private void onAbilityActivate()
        {
        }

        public int getDeActivatedTick()
        {
            return DeActivatedTick;
        }

        public final void setDeActivatedTick(int deActivatedTick)
        {
            DeActivatedTick = deActivatedTick;
        }

        public void loadLevelManager(ClassLevelingManager lm)
        {
            if (lm == null) return;
            if (lm instanceof ClassLevelingManagerStage) SLM = (ClassLevelingManagerStage) lm;
            if (lm instanceof ClassLevelingManagerXPLevel) XLM = (ClassLevelingManagerXPLevel) lm;
        }

        public int getDurationTick()
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
//    @Deprecated
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
                    CyberCoreMain.Log.Error("Was LOG ||"+"====> CAN NOT ACTIVATE POWER NO POWER SETTINGS!!!");
                    return;
                }

                if (getPowerSettings().isHotbar() && isLSNull())
                {
                    CyberCoreMain.Log.Error("Was LOG ||"+"====> CAN NOT ACTIVATE POWER NO HOT BAR SLOT IN SETTINGS!!!");
                    return;
                }

                DeActivatedTick = Server.getInstance().getTick() + getRunTimeTick();
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
            if (getLS() != ls) setLS(ls);
            if (!hasPowerSettings())
            {
                CyberCoreMain.Log.Error("Was LOG ||"+"====> CAN NOT ENAVBLE POWER NO POWER SETTINGS!!!");
                return;
            }

            if (getPowerSettings().isHotbar() && isLSNull())
            {
                CyberCoreMain.Log.Error("Was LOG ||"+"====> CAN NOT ENAVBLE POWER NO HOT BAR SLOT IN SETTINGS!!!" + getClass().getName());
                return;
            }

            setEnabled(true);
            PlayerClass.getClassSettings().delLearnedPowerAndLearnIfNotEqual(getAPE());
//        PlayerClass.getClassSettings().delLearnedPower(getType());
//        PlayerClass.getClassSettings().learnNewPower(getAPE(),true);
            if (!PlayerClass.getClassSettings().getEnabledPowers().contains(getType()))
                PlayerClass.getClassSettings().getEnabledPowers().add(getType());
            PlayerClass.getClassSettings().setPreferedSlot(getLS(), getType());
            onEnable();
        }

        public AdvancedPowerEnum getAPE()
        {
            if (getStageLevelManager() != null)
            {
                try
                {
                    return new AdvancedPowerEnum(getType(), getStage());
                }
                catch (Exception e)
                {
                    CyberCoreMain.Log.Error("Was LOG ||"+"ERRORR ###@@@ " + this + " || " + getClass().getName());
                    e.printStackTrace();
                    return null;
                }
            }
            else return new AdvancedPowerEnum(getType(), getXLM().getXP());
        }

        public void onEnable()
        {
            if (hasPowerSettings())
            {
                PowerSettings ps = getPowerSettings();
                if (ps.isHotbar())
                {
                    if (getLS() == LockedSlot.NA)
                    {
                        CyberCoreMain.Log.Error("Was LOG ||"+"Error! No Valid locked slot set!");
                        getPlayer().sendMessage("Error! No Valid locked slot set!");
                    }
                    else
                    {
                        sendHotbarItemToLS(HotbarStatus.Success);
                    }
                }
            }
        }

        private void sendHotbarItemToLS(HotbarStatus hbs)
        {
            Item i = getHotbarItem(hbs);
            PlayerInventory pi = getPlayer().getInventory();
            Item ii = pi.getItem(getLS().getSlot()).clone();
            pi.clear(getLS().getSlot());
            if (ii == null || ii.hasCompoundTag() && ii.getNamedTag().contains(PowerHotBarNBTTag)) ii = null;
            CyberCoreMain.Log.Error("Was LOG ||"+">>>>>>>>>>>>>Data::: " + getLS() + "|||||" + i);
            try
            {
                if (i == null)
                {
                    CyberCoreMain.Log.Error("Was LOG ||"+"Error! WTF ITEM IS NELL");
                    return;
                }

                if (getLS() == LockedSlot.NA)
                {
                    CyberCoreMain.Log.Error("Was LOG ||"+"Error! LS IS NA aaaaaaaaaaaaaaaNELL");
                    return;
                }

                pi.setItem(getLS().getSlot(), i);
                if (ii != null && !ii.isNull()) pi.addItem(ii);
            }
            catch (Exception e)
            {
                CyberCoreMain.Log.Error("Was LOG ||"+"Ahhhhhh Fuck!");
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
                case Cooldown:
                default:
                    i = getHotbarItem_Cooldown();
                    break;
            }

            if (!i.hasCompoundTag()) i.setCompoundTag(new CompoundTag());
            i.getNamedTag().putBoolean(PowerHotBarNBTTag, true);
            return i;
        }

        public Item getHotbarItem_Success()
        {
            Item i = Item.get(Item.EMERALD);
            i.setCustomName(getDispalyName() + " Power");
            i.setLore(TextFormat.GREEN + "Ready for Use!",
                TextFormat.GRAY + "Cooldown: " + getCooldownTimeSecs() + " Secs");
            return i;
        }

        public Item getHotbarItem_Fail()
        {
            Item i = Item.get(Item.REDSTONE_DUST);
            i.setCustomName(getDispalyName() + " Power");
            i.setLore(TextFormat.RED + "Power Failed Please Wait!",
                TextFormat.GRAY + "Cooldown: " + getCooldownTimeSecs() + " Secs");
            return i;
        }

        public Item getHotbarItem_Cooldown()
        {
            Item i = Item.get(Item.GLOWSTONE_DUST);
            i.setCustomName(getDispalyName() + " Power");
            i.setLore(TextFormat.YELLOW + "Power is in Cooldown!",
                TextFormat.RED + "Cooldown Time Left: " +
                ((getCooldown().getTick() - getPlayer().getServer().getTick()) / 20) + " Secs",
                TextFormat.GRAY + "Cooldown: " + getCooldownTimeSecs() + " Secs");
            return i;
        }

        public bool hasPowerSettings()
        {
            return getPowerSettings() != null;
        }

        public bool isLSNull()
        {
            LockedSlot ls = getLS();
            if (ls == null) return true;
            return ls == LockedSlot.NA;
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
            if (SLM != null)
            {
                return SLM.getStage();
            }
            else if (XLM != null)
            {
                return formatLeveltoStage(XLM.getLevel());
            }

            return StageEnum.NA;
        }

        public StageEnum formatLeveltoStage(int lvl)
        {
            return StageEnum.getStageFromInt(1 + (lvl / 20));
        }

        public void onActivate()
        {
        }

        public LockedSlot getLS()
        {
            return LS;
        }

        public void setLS(LockedSlot LS)
        {
            CyberCoreMain.Log.Error("Was LOG ||"+"LOCKEDSLOT SET FOR " + this.getClass().getName());
            this.LS = LS;
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
            NukkitRandom nr = new NukkitRandom();
            int l = PlayerClass.getLVL();
            double f = ((-Math.sin(l / 90) * 13 + Math.sin(-50 + (l / 80))));
            return (int) Math.round(f * 100);
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
        public Event handelEvent(Event event) {
            if (event instanceof PlayerTakeDamageEvent)
            return PlayerTakeDamageEvent((PlayerTakeDamageEvent) event);
            if (event instanceof EntityDamageEvent)
            return EntityDamageEvent((EntityDamageEvent) event);
            if (event instanceof CustomEntityDamageByEntityEvent)
            return CustomEntityDamageByEntityEvent((CustomEntityDamageByEntityEvent) event);
            if (event instanceof PlayerJumpEvent)
            return PlayerJumpEvent((PlayerJumpEvent) event);
            if (event instanceof InventoryTransactionEvent)
            return InventoryTransactionEvent((InventoryTransactionEvent) event);
            if (event instanceof InventoryClickEvent)
            return InventoryClickEvent((InventoryClickEvent) event);
            if (event instanceof EntityInventoryChangeEvent) {
                return EntityInventoryChangeEvent((EntityInventoryChangeEvent) event);
            }
            return event;
        }

        public EntityDamageEvent EntityDamageEvent(EntityDamageEvent e)
        {
            return e;
        }

        public InventoryClickEvent InventoryClickEvent(InventoryClickEvent e)
        {
            return e;
        }

        public InventoryTransactionEvent InventoryTransactionEvent(InventoryTransactionEvent e)
        {
            return e;
        }

        public EntityInventoryChangeEvent EntityInventoryChangeEvent(EntityInventoryChangeEvent e)
        {
            return e;
        }

        public PlayerJumpEvent PlayerJumpEvent(PlayerJumpEvent e)
        {
            return e;
        }

        /**
     * ALWAYS RETURN THE EVENT
     *
     * @param e
     * @return e Event
     */
        public abstract CustomEntityDamageByEntityEvent CustomEntityDamageByEntityEvent(
            CustomEntityDamageByEntityEvent e);

        /**
     * Time in Secs
     *
     * @return int Time in secs
     */
        protected int getCooldownTimeSecs()
        {
            return 60 * 3; //3 Mins
        }

        public final String getSafeName()
        {
            return getName().replaceAll(" ", "_");
        }

        public final int getCooldownTimeTick()
        {
            return getCooldownTimeSecs() * 20;
        }

        public void initStages()
        {
        }

//    public void importConfig(ConfigSection cs) {
//        if (LM != null) LM.importConfig(cs);
//    }
//
//    public ConfigSection exportConfig() {
//        ConfigSection c = new ConfigSection();
//        if (LM != null) c.put("LM", LM.exportConfig());
//        return c;
//    }

        public final void handleTick(int tick)
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
            if (SLM != null)
            {
                try
                {
                    return new AdvancedStagePowerEnum(getType(), getStage());
                }
                catch (Exception e)
                {
                    e.printStackTrace();
                    return null;
                }
            }
            else if (XLM != null)
            {
                return new AdvancedXPPowerEnum(getType(), XLM.getXP());
            }
            else
            {
                return new AdvancedPowerEnum(getType());
            }
        }

        public abstract PowerEnum getType();
//    {
//        CyberCoreMain.Log.Error("ERROR GETTING TYPE FROM POWER!!!!!");
//        return PowerEnum.Unknown;
//    }

        //USE TO RUN
        public final void initPowerRun(Object...args)
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
                {
                    if (CanSendCanNotRunMessage) sendCanNotRunMessage();
                }
            }
        }

        public final void initForcePowerRun(Object...args)
        {
            PlayerClass.takePowerSourceCount(PowerSourceCost);
            usePower(args);
            afterPowerRun(args);
        }

        public void sendCanNotRunMessage()
        {
            getPlayer().sendMessage(TextFormat.RED + "Error! PowerAbstract " + getDispalyName() + TextFormat.RED +
                                    " still has a " + TextFormat.LIGHT_PURPLE + Cooldown.toString() + TextFormat.RED +
                                    " Cooldown.");
        }

        public void afterPowerRun(Object...args)
        {
            addCooldown();
            getPlayer().sendMessage(getSuccessUsageMessage());
        }

        public Effect getEffect()
        {
            return Effect.getEffect(Effect.FATAL_POISON);
        }

        public String getSuccessUsageMessage()
        {
            return TextFormat.GREEN + " > PowerAbstract " + getDispalyName() + TextFormat.GREEN +
                   " has been activated!";
        }

        public Object usePower(Object...args)
        {
            setActive();
            if (isAbility())
            {
                activate();
            }
            else
            {
                setActive(false);
            }

            return null;
        }

        public bool CanRun(bool force, Object...args)
        {
            if (!PlayerClass.CCM.isInSpawn(getPlayer()))
            {
                getPlayer().sendMessage(TextFormat.RED + "Error! can not use " + getDispalyName() + " while in spawn!");
                return false;
            }

            if (force) return true;
            if (PlayerClass.getPowerSourceCount() < PowerSourceCost)
            {
                getPlayer().sendMessage(TextFormat.RED + "Not enough " + PlayerClass.getPowerSourceType().name() +
                                        " Energy!");
                return false;
            }

            NukkitRandom nr = new NukkitRandom();
            int r = nr.nextRange(0, 100);
            if (r <= getPowerSuccessChance())
            {
                //Success
                if (Cooldown != null && Cooldown.isValid())
                {
                    CyberCoreMain.Log.Error("Was LOG ||"+"CD FAIL!!!");
                    return false;
                }
            }
            else
            {
                CyberCoreMain.Log.Error("Was LOG ||"+"PSC FAIL!!!" + r + "||" + getPowerSuccessChance());
                return false; //Fail
            }

            if (isAbility())
            {
                CyberCoreMain.Log.Error("Was LOG ||"+"ABILITY " + !isActive());
                return !isActive();
            }

            return true;
        }

        public bool isAbility()
        {
            return getPowerSettings().isAbility();
        }

        public bool isHotbar()
        {
            return getPowerSettings().isHotbar();
        }

        public HotbarStatus getCurrentHotbarStatus()
        {
            if (getCooldown() != null) return HotbarStatus.Cooldown;
            if (getPowerState() == PowerState.Fail) return HotbarStatus.Fail;
            return HotbarStatus.Success;
        }

        public PowerState getPowerState()
        {
            return PowerState.Idle;
        }

        public void onTick(int tick)
        {
            if (isHotbar())
            {
                if (!isLSNull())
                {
                    sendHotbarItemToLS(getCurrentHotbarStatus());
                }
            }

            if (isAbility())
            {
                //Only For Deactivation
//            CyberCoreMain.Log.Error("Was LOG ||"+"POWER TICKKKKKK2");
                if (isAbilityActive())
                {
                    CyberCoreMain.Log.Error("Was LOG ||"+"POWER TICKKKKKK3");
                    whileAbilityActive();
                    if (getDeActivatedTick() != -1 && tick >= getDeActivatedTick())
                    {
                        CyberCoreMain.Log.Error("Was LOG ||"+"POWER TICKKKKKK44444444444444444444444444444");
//                    setEnabled(false);
                        DeactivateAbility();
                        DeActivatedTick = -1;
                        onAbilityDeActivate();
                    }
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

        public final void ActivateAbility()
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
            Cooldown = new CoolDown(getType().name(), secs * 20);
            return Cooldown;
        }

        public abstract String getName();

        public String getDispalyName()
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
    }
}