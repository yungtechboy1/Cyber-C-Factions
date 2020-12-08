using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using CyberCore.Custom.Events;
using CyberCore.CustomEnums;
using CyberCore.Manager.ClassFactory.Powers;
using CyberCore.Manager.ClassFactory.Window;
using CyberCore.Manager.Forms;
using CyberCore.Manager.TypeFactory.Powers;
using CyberCore.Utils;
using CyberCore.Utils.Cooldowns;
using log4net.Util.TypeConverters;
using MiNET;
using MiNET.Blocks;
using MiNET.Items;
using MiNET.Utils;
using OpenAPI.Events;
using OpenAPI.Events.Block;
using OpenAPI.Events.Entity;
using OpenAPI.Events.Player;

namespace CyberCore.Manager.ClassFactory
{
    public abstract class BaseClass
    {
        public List<CoolDown> COOLDOWNS = new List<CoolDown>();
        public bool Prime = false;
        public int PrimeKey = 0;
        public int SwingTime = 20;

        public List<PowerEnum> ActiveClassPowerList = new List<PowerEnum>();

        //    public List<PowerEnum> DefaultPowers = new List<>();
        public Dictionary<PowerEnum, PowerAbstract> PossibleClassPowerList = new Dictionary<PowerEnum, PowerAbstract>();
        public List<AdvancedPowerEnum> DefaultPowers1 = new List<AdvancedPowerEnum>();
        public Dictionary<PowerEnum, PowerAbstract> USEABLEClassPowersList = new Dictionary<PowerEnum, PowerAbstract>();
        public Dictionary<LockedSlot, PowerAbstract> HotBarPowers = new Dictionary<LockedSlot, PowerAbstract>();
        public CyberCoreMain CCM;
        protected int MainID = 0;

        Dictionary<int, int> Herbal = new Dictionary<int, int>()
        {
            {new Grass().Id, 10},
            {new Vine().Id, 10},
            {new Pumpkin().Id, 20},
            {new MelonBlock().Id, 20},
            {new Cocoa().Id, 30},
            {new Reeds().Id, 30},
            {new Cactus().Id, 30},
            {new Carrots().Id, 50},
            {new Wheat().Id, 50},
            {115, 50} //Neather Wart
            ,
            {38, 100}, //FLOWER
            {39, 150}, //BROWN_MUSHROOM
            {99, 150}, //BROWN_MUSHROOM_BLOCK
            {40, 150}, //Red_MUSHROOM
            {100, 150} //Red_MUSHROOM_BLOCK
        };

        Dictionary<int, int> Excavation = new Dictionary<int, int>()
        {
            {new Grass().Id, 40},
            {
                110, 40
            }, //MYCELIUM
            {
                3, 40
            }, //DIRT
            {
                13, 40
            }, //GRAVEL
            {
                12, 40
            }, //SAND
            {
                88, 40
            }, //SOUL_SAND
            {
                82, 40
            } //CLAY_BLOCK
        };

        public CorePlayer P;

        //    private ClassType TYPE;
        private int LVL = 0;

        private int XP = 0;

        // private Ability ActiveAbility;
        private Dictionary<BuffType, Buff> Buffs = new Dictionary<BuffType, Buff>();
        private Dictionary<BuffType, DeBuff> DeBuffs = new Dictionary<BuffType, DeBuff>();
        // internal List<LockedSlot> LockedSlots = new List<LockedSlot>();
        private double PowerSourceCount = 0;
        private ClassSettingsObj ClassSettings = null; //new ClassSettingsObj(this);


        public virtual void AddClassPowers()
        {
        }


        //Get all the Powers that the player has Learned
        //Next Filter By the Class Currently Choosen
        //Then Add all aplicable Powers
        public BaseClass(CyberCoreMain main, CorePlayer player, Dictionary<String, Object> data = null)
        {
            CCM = main;

//        MainID = mid;
            P = player;
//        TYPE = rank;
//        LVL = XPToLevel(XP);
            ClassSettings = new ClassSettingsObj(this);
            SetBuffs();
            // startSetPowers();


            if (data != null)
            {
                if (data.ContainsKey("COOLDONWS"))
                {
                    Dictionary<String, Object> css = (Dictionary<string, object>) data["COOLDONWS"];
                    if (css == null)
                    {
                        CyberCoreMain.Log.Error("Was LOG ||" + "ERROROORR COOLDOWNS NOT IN CORRECT FOPRMT");
                    }
                    else
                    {
                        foreach (var a in css)
                        {
                            var key = a.Key;
                            var value = a.Value;
                            CyberCoreMain.Log.Error("Was LOG ||" + value + " <<<<<<<<<<<<<<<<<<<<<< ");
                            AddCooldown(key, (int) value);
                        }

                        ;
                    }
                }

                if (data.ContainsKey("XP"))
                {
                    int xpi = (int) data["XP"];
                    addXP(xpi);
                }

                if (data.ContainsKey("PowerSourceCount"))
                {
                    int psc = (int) data["PowerSourceCount"];
                    addPowerSourceCount(psc);
                }

                if (data.ContainsKey("CS"))
                {
//                int psc = data.getInt("PowerSourceCount", 0);
                    ClassSettings = new ClassSettingsObj(this, ((Dictionary<String, Object>) data["CS"]));
                }
                else
                {
                    CyberCoreMain.Log.Error("Was LOG ||" + "Error! No ClassSetting Found!!!");
                }
            }
            else
            {
                ClassSettings = new ClassSettingsObj(this);
            }

            try
            {
                AddClassPowers();
                Console.WriteLine($"Adding Powers to : {getName()} => {PossibleClassPowerList.Count}");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Oh they dont have an Add Class Virtural metod for Player Class: {getName()}");
            }

            LearnDefaultPowers();
            SetBuffs();
            ActivatePossiblePower();
            startSetPowers();
        }

        private void ActivatePossiblePower()
        {
            foreach (var a in getClassSettings().getLearnedPowers())
            {
            }
        }

        /***
         * This Constructor is used for only obtaining basic information!
         * This Constructor can never be used for a Player or to get any dynamic data.
         */
        public BaseClass(CyberCoreMain main)
        {
            CCM = main;
            ClassSettings = new ClassSettingsObj(this);
            AddClassPowers();
            SetBuffs();
//        MainID = mid;
//        P = player;
//        TYPE = rank;
//        LVL = XPToLevel(XP);
//        startbuffs();
//        startSetPowers();
        }

        public static int XPToLevel(int xp)
        {
            int lvl = 0;
            while (xp >= calculateRequireExperience(lvl))
            {
                xp = xp - calculateRequireExperience(lvl);
                lvl++;
            }

            return lvl;
        }

        public static int XPRemainder(int xp)
        {
            int lvl = 0;
            while (xp >= calculateRequireExperience(lvl))
            {
                xp = xp - calculateRequireExperience(lvl);
                lvl++;
            }

            return xp;
        }

        public static int XPToGetToLevel(int level)
        {
            int xp = 0;
            for (; level > 0;)
            {
                xp += calculateRequireExperience(level);
                --level;
            }

            return xp;
        }

        public static int calculateRequireExperience(int level)
        {
            if (level >= 30)
            {
                return 112 + (level - 30) * 9 * 100;
            }
            else if (level >= 15)
            {
                return 37 + (level - 15) * 5 * 100;
            }
            else
            {
                return 7 + level * 2 * 100;
            }
        }

        public void LearnDefaultPowers()
        {
            foreach (PowerEnum pe in GetDefaultPowers())
            {
                if (!getClassSettings().isPowerLearned(pe))
                {
                    CyberCoreMain.Log.Error("Was LOG ||" + "SEND LEARN TO " + pe);
                    getClassSettings().learnNewPower(pe, true);
                }
            }
        }

        private void startSetPowers(bool activateall = false)
        {
            Console.WriteLine($"Starting to set Powers!2222333344445 > {PossibleClassPowerList.Count}");
            foreach (KeyValuePair<PowerEnum, PowerAbstract> a in PossibleClassPowerList)
            {
                var k = a.Key;
                var v = a.Value;
                Console.WriteLine($"Checking Power: {k.Name}");
                if (v.isDefaultPower || v.Requirement.Pass(this) || activateall)
                {
                    //Activate;
                    Console.WriteLine($"Activating Power: {k.Name}");
                    v.StartLoading(this);
                }
            }

//        CCM.PowerManagerr.getPossiblePowers(getClassSettings().getLearnedPowers());
//        SetPowers();
        }

        public List<LockedSlot> getLockedSlots()
        {
            return HotBarPowers.Keys.ToList();
        }

        public void onLeaveClass()
        {
            // if (this is PowerHotBarInt)
            // {
            //     if (getLockedSlots().Count > 0)
            //     {
            //         foreach (LockedSlot ls in getLockedSlots()) getPlayer().Inventory.clear(ls.getSlot());
            //         LockedSlots.Clear();
            //     }
            // }
        }

        public PlayerInventory getPlayerInventory()
        {
            PlayerInventory pi = new PlayerInventory(getPlayer());

            List<Item> s = getPlayer().Inventory.Slots;
            if (getLockedSlots().Count > 0)
                foreach (LockedSlot ls in getLockedSlots())
                    s[ls.getSlot()] = new ItemAir();
            pi.setContents(s);
            return pi;
        }

        public abstract PrimalPowerType getPowerSourceType();

        public double getPowerSourceCount()
        {
            return PowerSourceCount;
        }

        public void addPowerSourceCount()
        {
            addPowerSourceCount(1);
        }

        public void addPowerSourceCount(double a)
        {
            if (PowerSourceCount + a > getMaxPowerSourceCount())
            {
                double d = getMaxPowerSourceCount() - a;
                if (d < 0) PowerSourceCount += d;
            }
            else
            {
                PowerSourceCount += Math.Abs(a);
            }
        }

        public virtual List<PowerEnum> GetDefaultPowers()
        {
            return new List<PowerEnum>();
        }

        public String getColor()
        {
            return ChatColors.Gray;
        }

        public bool takePowerSourceCount(double a)
        {
            if (a > PowerSourceCount) return false;
            PowerSourceCount -= a;
            return true;
        }

        public double getMaxPowerSourceCount()
        {
            return Math.Abs(Math.Pow(57 * (getLVL() + 1), 2)) / Math.Sqrt(Math.Pow(20 * (getLVL() + 1), 3)) +
                   (getLVL() + 1) * 10;
        }

        public void tickPowerSource(int tick)
        {
            addPowerSourceCount(); //From Server Every 20 Secs
            double t = Math.Abs(Math.Pow(27 * (getLVL() + 1), 2));
            double b = Math.Sqrt(Math.Pow(18 * (getLVL() + 1), 3));
            int f = (int) ((t / b) * .2);

            addPowerSourceCount(Math.Abs(f));
            //TODO
            //ISSUE
            //Maybe TIck player power here too??
        }

        public List<CoolDown> getCOOLDOWNS()
        {
            return COOLDOWNS;
        }

        private void SetBuffs(bool clear = true)
        {
            if (clear)
            {
                Buffs.Clear();
                DeBuffs.Clear();
            }

            AddStartingBuffs();
            if (P != null) registerAllBuffsToCorePlayer(P);
        }

        public ClassTeir getTeir()
        {
            int d = (int) (getLVL() / 10);
            return (ClassTeir) Enum.Parse(typeof(ClassTeir), "Class" + d);
        }

        public abstract ClassType getTYPE();

        public abstract void SetPowers();

        public int getMainID()
        {
            return (int) getTYPE();
        }

        public abstract void AddStartingBuffs();

        private void registerAllBuffsToCorePlayer(CorePlayer cp)
        {
//        for (Buff bingetBuffs().values()) {
            foreach (Buff b in getBuffs().Values)
            {
                cp.addBuffFromClass(b);
            }

            foreach (DeBuff b in getDeBuffs().Values)
            {
                cp.addDeBuffFromClass(b);
            }

            cp.initAllClassBuffs();
        }

        private void recheckAllBuffs(long tick)
        {
            //No Need to Keep resending :/
//        for (Buff bingetBuffs().values()) {
//            getPlayer().addBuffFromClass(b);
//        }
//        for (DeBuff bingetDeBuffs().values()) {
//            getPlayer().addDeBuffFromClass(b);
//        }
            getPlayer().initAllClassBuffs();
        }

        public String getDisplayName()
        {
            return getColor() + getName();
        }

        public Dictionary<BuffType, Buff> addBuff(Buff o)
        {
            Buffs[o.getBt()] = o;
            return getBuffs();
        }

        public Dictionary<BuffType, Buff> removeBuffs(Buff o)
        {
            Buffs.Remove(o.getBt());
            return getBuffs();
        }

        public Dictionary<BuffType, Buff> getBuffs()
        {
            return new Dictionary<BuffType, Buff>(Buffs);
        }

        public Buff getBuff(BuffType o)
        {
            return Buffs[o];
        }

        public Dictionary<BuffType, DeBuff> removeDeBuff(DeBuff o)
        {
            DeBuffs.Remove(o.getBt());
            return getDeBuffs();
        }

        public Dictionary<BuffType, DeBuff> addDeBuff(DeBuff o)
        {
            DeBuffs[o.getBt()] = o;
            return getDeBuffs();
        }

        public Dictionary<BuffType, DeBuff> getDeBuffs()
        {
            return new Dictionary<BuffType, DeBuff>(DeBuffs);
        }

        public DeBuff getDeBuff(BuffType o)
        {
            return DeBuffs[o];
        }

        public float getDamageBuff()
        {
            return 1f;
        }

        public float getArmorBuff()
        {
            return 1f;
        }

        public int getExtraHealth()
        {
            return 0;
        }

        public float getMovementBuff()
        {
            return 0;
        }

        public List<PowerEnum> getEnabledPowersList()
        {
            return getClassSettings().getEnabledPowers();
        }
//
//    public void addDefaultPower(PowerEnum power) {
//        DefaultPowers.add(power);
////        getClassSettings().getClassDefaultPowers().add(power);
//    }

        public List<PowerAbstract> getActivePowers()
        {
            List<PowerAbstract> pp = new List<PowerAbstract>();
            foreach (PowerEnum pe in getEnabledPowersList())
            {
                pp.Add(getPossiblePower(pe));
            }

            return pp;
        }

        public PowerAbstract getPossiblePower(PowerEnum key, bool active)
        {
//        if(active)return ActivePowers.get(key);
            return USEABLEClassPowersList[key];
        }

        public PowerAbstract getPossiblePower(PowerEnum key)
        {
            return getPossiblePower(key, true);
        }

        public abstract Object RunPower(PowerEnum powerid, params Object[] args);

        public void deactivatePower(PowerEnum pe)
        {
            PowerAbstract p = getPossiblePower(pe, false);
            if (p == null)
            {
                getPlayer().SendMessage("Error DeActivating " + pe.Name);
            }

            p.setActive(false);

//        getClassSettings().delActiveGPDLPower(pe);
            onPowerDeActivate(p); //callback
            delActivePower(p);
            getPlayer().SendMessage(ChatColors.Red + "POWER > " + p.getDispalyName() + " has been DEactivated!");
        }

//
//    public void enablePower(PowerData pe) {
//        CyberCoreMain.Log.Error("Was LOG ||"+"Attempting to activate "+pe.getPowerID());
//        PowerAbstract p = pe.getPA();
//        if (p == null) {
//            getPlayer().SendMessage("E:221S: Error attempting to Activating " + pe.getPowerID());
//            return;
//        }
//        PossiblePowerList.put(pe.getPowerID(),p);
//        p.enablePower();
//        onPowerEnabled(p);//callback
////        addActivePower(p);
//        getPlayer().SendMessage(ChatColors.Green + "POWER > " + p.getDispalyName() + " has been activated!");
//    }

        public void enablePower(AdvancedPowerEnum pe, LockedSlot ls = new LockedSlot())
        {
            CyberCoreMain.Log.Error("Was LOG ||" + "Attempting to activate222 " + pe);

            PowerAbstract p = PowerManager.getPowerfromAPE(pe, this);
            if (p == null)
            {
                getPlayer().SendMessage("E:221S: Error attempting to Activating " + pe.getPowerID());
                return;
            }

            if (p.IsHotbarPower())
            {
                if (ls.Equals(LockedSlot.NA))
                {
                    CyberCoreMain.Log.Error("Was LOG ||" + "Error! Did not set Locked Slot!");
                    getPlayer().SendMessage("E:2ww21S: Error attempting to Activating " + pe.getPowerID());
                    return;
                }
                else
                {
                    p.setLS(ls);
                }
            }

            PossibleClassPowerList[pe.getPowerID()] = p;
            p.enablePower();
            onPowerEnabled(p); //callback

//        addActivePower(p);
            getPlayer().SendMessage(ChatColors.Green + "POWER > " + p.getDispalyName() + " has been activated!");
        }
//    public void enablePower(PowerEnum pe) {
//        CyberCoreMain.Log.Error("Was LOG ||"+"Attempting to activate222 "+pe);
//        PowerAbstract p = PowerManager.getPowerfromAPE(pe,this);
//        if (p == null) {
//            getPlayer().SendMessage("E:221S: Error attempting to Activating " + pe);
//            return;
//        }
////        if(ls != null && ls != LockedSlot.NA)p.setLS(ls);
//        p.enablePower();
//        onPowerEnabled(p);//callback
//        addActivePower(p);
//        getPlayer().SendMessage(ChatColors.Green + "POWER2 > " + p.getDispalyName() + " has been activated!");
//    }

        private void onPowerEnabled(PowerAbstract p)
        {
        }

        public void activatePower(PowerEnum pe)
        {
            PowerAbstract p = getPossiblePower(pe, false);
            if (p == null)
            {
                getPlayer().SendMessage("Error Activating " + pe.Name);
                return;
            }

            p.setActive();
            onPowerActivate(p); //callback

//        addActivePower(p);
            getPlayer().SendMessage(ChatColors.Green + "POWER > " + p.getDispalyName() + " has been activated!");
        }

        public void onPowerDeActivate(PowerAbstract p)
        {
        }

        public void onPowerActivate(PowerAbstract p)
        {
        }

        private void addActivePower(PowerAbstract p)
        {
            addActivePower(p.getType());
            AddPossiblePower(p);
        }

        private void addActivePower(PowerEnum p)
        {
            if (!getClassSettings().getEnabledPowers().Contains(p)) getClassSettings().addActivePower(p);
        }

        private void delActivePower(PowerAbstract p)
        {
            delActivePower(p.getType());
        }


//        PowerAbstract p = ActivePowers.get(powerid);
//        if(p == null || args.length != 3 ){
//            CCM.getLogger().error("No PowerAbstract found or Incorrect Args For MineLife E334221");
//            return -1;
//        }
//        if(powerid == 1 && p instanceof MineLifePower){
//            MineLifePower mlp = (MineLifePower) p;
//            return mlp.GetBreakTime((Item)args[0],(Block)args[1],(double)args[2]);
//        }
//        return (double)args[2];
//    }

        private void delActivePower(PowerEnum p)
        {
            if (getClassSettings().getEnabledPowers().Contains(p)) getClassSettings().delActivePower(p);
            if (getClassSettings().getPreferedSlot7().Equals(p)) getClassSettings().clearSlot7();
            if (getClassSettings().getPreferedSlot8().Equals(p)) getClassSettings().clearSlot8();
            if (getClassSettings().getPreferedSlot9().Equals(p)) getClassSettings().clearSlot9();
        }

        public void AddPossiblePower(PowerAbstract power)
        {
            PowerSettings ps = power.getPowerSettings();
            if (ps == null)
            {
                CyberCoreMain.Log.Error("CAN NOT ADD POWER " + power.getName() + "! No PowerSetting Set!");
                getPlayer().SendMessage(ChatColors.Red + "Error > Plugin > Power > " + power.getName() +
                                        " | Error activating power! No Power Setting Set!!!!");
                return;
            }

            if (!getLearnedPowersPE().Contains(power.getType()))
            {
                getPlayer().SendMessage("Error! Could not Add Possible Power " + power.getDispalyName() +
                                        " because you have not learned that power yet!");
                CyberCoreMain.Log.Error("Was LOG ||" + "ERror! " + getPlayer().getName() + " has not learned " +
                                        power.getDispalyName() + " Yet!");
                return;
            }

            if (ClassSettings.getEnabledPowers().Count == 0 ||
                !ClassSettings.getEnabledPowers().Contains(power.getType()))
            {
                getPlayer().SendMessage("Error! Could not Add non enabled Power to Possible Power " +
                                        power.getDispalyName() +
                                        " because you have not learned that power yet!");
                CyberCoreMain.Log.Error("Was LOG ||" + "ERror! " + getPlayer().getName() + " has not learned " +
                                        power.getDispalyName() + " Yet!");
                return;
            }

//        if (power instanceof PowerHotBarInt) {
//            LockedSlots.add(power.getLS());
//            if( getClassSettings().getPreferedSlot7() == power.getType()){
//
//            }
//        }
//        if(ClassSettings.getLearnedPowers().contains(power.getType())){
//Add to Power List to Pick From!
            PossibleClassPowerList[power.getType()] = power;
//Power is Learned
//Power Active
            if (ps.isHotbar)
            {
                if (ClassSettings.getPreferedSlot7().Equals(power.getType())) power.setLS(LockedSlot.SLOT_7);
                if (ClassSettings.getPreferedSlot8().Equals(power.getType())) power.setLS(LockedSlot.SLOT_8);
                if (ClassSettings.getPreferedSlot9().Equals(power.getType())) power.setLS(LockedSlot.SLOT_9);
            }

//            power.enablePower();
        }

        private List<PowerEnum> getLearnedPowersPE()
        {
            List<PowerEnum> p = new List<PowerEnum>();
            foreach (AdvancedPowerEnum pe in getLearnedPowers()) p.Add(pe.getPowerEnum());
            return p;
        }

        public ClassSettingsObj getClassSettings()
        {
//        if(ClassSettings == null)
            return ClassSettings;
        }

        public bool TryRunPower(PowerEnum powerid)
        {
            PowerAbstract p = getPossiblePower(powerid);
            if (p == null) return false;
            return p.CanRun(false);
        }

        public void CmdRunPower(PowerEnum powerid)
        {
            RunPower(powerid);
        }

        public void RunPower(PowerEnum powerid)
        {
            PowerAbstract p = getPossiblePower(powerid);
            if (p == null) return;
            p.usePower(getPlayer());
        }

        public List<PowerAbstract> PossiblePowers()
        {
            List<PowerAbstract> a = new List<PowerAbstract>();
            return a;
        }

        public abstract String getName();

        public Dictionary<String, Object> export()
        {
            var a = new Dictionary<String, Object>()
            {
                {"COOLDOWNS", getCOOLDOWNStoConfig()},
                {"PowerSourceCount", PowerSourceCount},

                {"XP", getXP()},
                {"TYPE", (int) getTYPE()},
            };
            if (getClassSettings() != null) a["CS"] = getClassSettings().export();
            else a["CS"] = "{}";
            return a;
        }

        public Dictionary<String, Object> getCOOLDOWNStoConfig()
        {
            Dictionary<String, Object> conf = new Dictionary<String, Object>();
            foreach (CoolDown c in getCOOLDOWNS())
            {
                CyberCoreMain.Log.Error("Was LOG ||" + c.toString() + "|||| AND " + c.isValid());
                if (c.isValid()) conf[c.getKey()] = c.getTime();
            }

            return conf;
        }

        public CorePlayer getPlayer()
        {
            return P;
        }

        // public void setActiveAbility(Ability activeAbility)
        // {
        //     ActiveAbility = activeAbility;
        // }

        public void addXP(int xp)
        {
            XP += xp;
            LVL = XPToLevel(XP);
        }

        public void takeXP(int xp)
        {
            XP -= xp;
            LVL = XPToLevel(XP);
        }

        public int getXP()
        {
            LVL = XPToLevel(XP);
            return XP;
        }

        public int getLVL()
        {
            return XPToLevel(getXP());
        }

        public bool isPrime()
        {
            return Prime;
        }

        public void setPrime(bool prime)
        {
            Prime = prime;
        }

        public void setPrime(int key)
        {
            setPrime(true);
            PrimeKey = key;
//        Ability a = PossibleAbillity().get(key);
//        a.PrimeEvent();
        }

        public void AddCooldown(String perk, long value)
        {
            COOLDOWNS.Add(new CoolDown(perk, value));
        }

        public void RemoveCooldown(String perk)
        {
            if (!HasCooldown(perk)) return;

            CoolDown cr = null;
            foreach (CoolDown c in COOLDOWNS)
            {
                if (c.getKey().equalsIgnoreCase(perk))
                {
                    cr = c;
                    break;
                }
            }

            if (cr != null)
            {
                COOLDOWNS.Remove(cr);
            }
            else
            {
                CyberCoreMain.Log.Error("Error! No cooldown to remove!");
            }
        }

        public void ReduceCooldown(String perk, int value)
        {
            if (!HasCooldown(perk)) return;

            CoolDown cr = null;
            foreach (CoolDown c in COOLDOWNS)
            {
                if (c.getKey().equalsIgnoreCase(perk))
                {
                    cr = c;
                    break;
                }
            }

            if (cr != null)
            {
                COOLDOWNS.Remove(cr);
                AddCooldown(perk, cr.getTime() - CyberUtils.getLongTime() - value);
            }
            else
            {
                CyberCoreMain.Log.Error("Error! No cooldown to reduce!");
            }
        }

        public CoolDown GetCooldown(String key)
        {
            if (!HasCooldown(key)) return null;
            foreach (CoolDown c in COOLDOWNS)
            {
                if (c.getKey().equalsIgnoreCase(key))
                {
                    return c;
                }
            }

            return null;
        }

        public bool HasCooldown(String perk)
        {
            foreach (CoolDown c in (List<CoolDown>) COOLDOWNS)
            {
                if (c.getKey().equalsIgnoreCase(perk))
                {
                    if (!c.isValid())
                    {
                        COOLDOWNS.Remove(c);
                        return false;
                    }

                    return true;
                }
            }

            return false;
        }

        public Event PowerHandelEvent(Event e)
        {
            if (e == null)
            {
                CyberCoreMain.Log.Error(
                    "Was LOG ||" + "WTF NUUU222222222222222222UUUUUUUUUUUUUUUULLLLLLLLLLLLLLLLLLLLL");
                return null;
            }

//        Event ee = e;
            foreach (PowerAbstract p in getActivePowers())
            {
                //TODO
                // p.handelEvent(e);
            }

            return e;
        }

        // public Event PlayerTakeDamageEvent(PlayerTakeDamageEvent event) {
        //     return event;
        // }

//TODO
//         public Event HandelEvent(Event event) {
//             event = PowerHandelEvent(event);
//             if (event instanceof PlayerTakeDamageEvent) {
//                 event = PlayerTakeDamageEvent((PlayerTakeDamageEvent) event);
// //            if (ActiveAbility != null) event = ActiveAbility.CustomEntityDamageByEntityEvent((CustomEntityDamageByEntityEvent) event);
//                 return event;
//             } else if (event instanceof CustomEntityDamageByEntityEvent) {
//                 event = CustomEntityDamageByEntityEvent((CustomEntityDamageByEntityEvent) event);
// //            if (ActiveAbility != null) event = ActiveAbility.CustomEntityDamageByEntityEvent((CustomEntityDamageByEntityEvent) event);
//                 return event;
//             } else if (event instanceof BlockBreakEvent) {
//                 event = BlockBreakEvent((BlockBreakEvent) event);
//                 if (ActiveAbility != null) event = ActiveAbility.BlockBreakEvent((BlockBreakEvent) event);
//                 return event;
//             } else if (event instanceof PlayerToggleSprintEvent) {
//                 event = PlayerToggleSprintEvent((PlayerToggleSprintEvent) event);
//                 if (ActiveAbility != null) event = ActiveAbility.PlayerToggleSprintEvent((PlayerToggleSprintEvent) event
//                     );
//                 return event;
//             } else if (event instanceof PlayerInteractEvent) {
//                 event = PlayerInteractEvent((PlayerInteractEvent) event);
//                 if (ActiveAbility != null) event = ActiveAbility.PlayerInteractEvent((PlayerInteractEvent) event);
//                 return event;
//             } else if (event instanceof EntityRegainHealthEvent) {
//                 event = EntityRegainHealthEvent((EntityRegainHealthEvent) event);
//                 if (ActiveAbility != null) event = ActiveAbility.EntityRegainHealthEvent((EntityRegainHealthEvent) event
//                     );
//                 return event;
//             } else if (event instanceof BlockPlaceEvent) {
//                 event = BlockPlaceEvent((BlockPlaceEvent) event);
//                 if (ActiveAbility != null) event = ActiveAbility.BlockPlaceEvent((BlockPlaceEvent) event);
//                 return event;
//             } else if (event instanceof EntityDamageEvent) {
//                 event = EntityDamageEvent((EntityDamageEvent) event);
//                 if (ActiveAbility != null) event = ActiveAbility.EntityDamageEvent((EntityDamageEvent) event);
//                 return event;
//             } else if (event instanceof CraftItemEvent) {
//                 event = CraftItemEvent((CraftItemEvent) event);
//                 if (ActiveAbility != null) event = ActiveAbility.CraftItemEvent((CraftItemEvent) event);
//                 return event;
//             } else if (event instanceof PlayerJumpEvent) {
//                 event = PlayerJumpEvent((PlayerJumpEvent) event);
//                 if (ActiveAbility != null) event = ActiveAbility.PlayerJumpEvent((PlayerJumpEvent) event);
//                 return event;
//             } else if (event instanceof EntityInventoryChangeEvent) {
//                 event = EntityInventoryChangeEvent((EntityInventoryChangeEvent) event);
// //            if (ActiveAbility != null) event = ActiveAbility.PlayerJumpEvent((PlayerJumpEvent) event);
//                 return event;
//             }
//             return event;
//         }

        // public PlayerJumpEvent PlayerJumpEvent(PlayerJumpEvent event) {
        //     return event;
        // }
        //
        // public EntityInventoryChangeEvent EntityInventoryChangeEvent(EntityInventoryChangeEvent event) {
        //     return event;
        // }

//TODO Change to MainClassSettingsWindow return tyoe
        public CyberFormSimple getSettingsWindow()
        {
            return new MainClassSettingsWindow(this, MainForm.Class_Settings_Window, "InternalPlayerSettings Window",
                "");
        }


        public void activateAbility()
        {
//        if (HasCooldown(PrimeKey)) {
//            getPlayer().SendMessage("This Has a CoolDown!");
//            return;
//        } else if (PrimeKey <= PossibleAbillity().size() - 1) {
//            Ability a = PossibleAbillity().get(PrimeKey);
//            if (a != null && a.activate()) {
//                setActiveAbility(a);
//            }
//        }
        }

        // public void activateAbility(Vector3 pos)
        // {
        //     activateAbility(pos, 0);
        // }

        // public void activateAbility(int id)
        // {
        //     activateAbility(null, id);
        // }

        // public void activateAbility(Vector3 pos, int id)
        // {
        // }

        // public void disableAbility()
        // {
        //     ActiveAbility.deactivate();
        // }

        // public EntityDamageEvent EntityDamageEvent(EntityDamageEvent event) {
        //     return event;
        // }
        //
        // public PlayerToggleSprintEvent PlayerToggleSprintEvent(PlayerToggleSprintEvent event) {
        //     return event;
        // }
        //
        // public BlockPlaceEvent BlockPlaceEvent(BlockPlaceEvent event) {
        //     return event;
        // }
        //
        // public EntityRegainHealthEvent EntityRegainHealthEvent(EntityRegainHealthEvent event) {
        //     return event;
        // }
        //
        // public PlayerInteractEvent PlayerInteractEvent(PlayerInteractEvent event) {
        //     return event;
        // }
        //
        // public BlockBreakEvent BlockBreakEvent(BlockBreakEvent event) {
        //     return event;
        // }
        //
        // public CraftItemEvent CraftItemEvent(CraftItemEvent event) {
        //     return event;
        // }

//         public CustomEntityDamageByEntityEvent CustomEntityDamageByEntityEvent(CustomEntityDamageByEntityEvent event) {
// //        for (PowerAbstract pingetActivePowers()) p.CustomEntityDamageByEntityEvent(event);
//             float bd = event.getOriginalDamage();
//
//             Buff b = getBuff(BuffType.Damage);
//             if (event.getEntity() instanceof Player && getBuff(BuffType.DamageToPlayer) != null) {
//                 b = getBuff(BuffType.DamageToPlayer);
//             } else if (getBuff(BuffType.DamageToEntity) != null)
//             {
//                 b = getBuff(BuffType.DamageToEntity);
//             }
//
//             if (b != null)
//                 bd *= b.getAmount();
//                     event.
//             setDamage(bd);
//             return event;
//         }
//
//         public CustomEntityDamageEvent CustomEntityDamageEvent(CustomEntityDamageEvent event) {
//             float bd = event.getOriginalDamage();
//
//             Buff b = getBuff(BuffType.Damage);
//             if (b != null)
//                 bd *= b.getAmount();
//                     event.
//             setDamage(bd);
//             return event;
//         }

        public int XPRemainder()
        {
            return XPRemainder(getXP());
        }

        public void tickPowers(long tick)
        {
//        CyberCoreMain.Log.Error("Was LOG ||"+"Tring to TICKING POWER "+getActivePowers().size());
//        CyberCoreMain.Log.Error("Was LOG ||"+"Tring to TICKING POWER "+getActivePowers());
            foreach (PowerAbstract p in getActivePowers())
            {
//            CyberCoreMain.Log.Error("Was LOG ||"+"TICKING POWER " + p.getName());
                try
                {
                    //No Need to tick Disabled Or Non Ticking Powers
                    if (p.getTickUpdate() != -1 && p.isActive() || p.IsHotbarPower()) p.handleTick(tick);
                }
                catch (Exception e)
                {
                    CyberCoreMain.Log.Error("ERror in BC 33es: ", e);
                }
            }
        }

        public void onUpdate(long tick)
        {
//        CyberCoreMain.Log.Error("Was LOG ||"+"TICKING BASECLASS");
            tickPowers(tick);
            if (GetCooldown("CheckBuff") == null)
            {
                AddCooldown("CheckBuff", 15);
                recheckAllBuffs(tick);
            }

//        if(GetCooldown("Recheck") == null) {
//            AddCooldown("CheckBuff", 15);
//            recheckAllBuffs(tick);
//        }
        }

        public List<String> FormatHudText()
        {
            List<String> f = new List<String>();
            int lvl = XPToLevel(getXP());
            String pclass = getName();
            int pxp = XPRemainder(getXP());
            int pxpof = calculateRequireExperience(lvl + 1);
            int plvl = lvl;
            f.Add(ChatColors.Aqua + pclass);
            f.Add("" + ChatColors.Green + pxp + ChatColors.Aqua + " / " + ChatColors.Gold + pxpof);
            f.Add(ChatColors.Green + "Level: " + ChatColors.Yellow + plvl);
            f.Add(ChatColors.Aqua + getPowerSourceType() + " PowerAbstractin" + getPowerSourceCount() + " / "
                  + getMaxPowerSourceCount());
            return f;
        }

        public CyberFormSimple getHowToUseClassWindow()
        {
            return null;
        }

        public CyberFormSimple getClassMerchantWindow()
        {
            // return new CyberFormSimpleClassMerchant(this);
            return null;
        }

        /**
 * After Creating the class from the class manager this method is ran for final checks
 */
        public void onCreate()
        {
            if (getClassSettings() != null) ClassSettings.check();
            else
                CyberCoreMain.Log.Error("Was LOG ||" +
                                        "ERRROROROOROROROOROROROROOasdasd asd asdas dasd111231232122333");
        }

        public void addButtons(MainClassSettingsWindow mainClassSettingsWindow)
        {
            foreach (var p in getActivePowers())
            {
                p.addButton(mainClassSettingsWindow);
            }
        }

        public List<AdvancedPowerEnum> getLearnedPowers()
        {
            return new List<AdvancedPowerEnum>(getClassSettings().getLearnedPowers());
        }

        public List<PowerAbstract> getLearnedPowersAbstract()
        {
            List<PowerAbstract> pa = new List<PowerAbstract>();
            foreach (AdvancedPowerEnum e in getClassSettings().getLearnedPowers())
            {
                pa.Add(PowerManager.getPowerfromAPE(e, this));
            }

            return pa;
        }

        public void disablePower(AdvancedPowerEnum ape)
        {
            getClassSettings().getEnabledPowers().Remove(ape.getPowerEnum());
            PossibleClassPowerList.Remove(ape.getPowerEnum());
        }


        public enum ClassTeir
        {
            Class1,
            Class2,
            Class3,
            Class4,
            Class5,
            Class6,
            Class7,
            Class8,
            Class9,
            Class10
        }

        public bool CanSwitchHotbar(int to, int from)
        {
            var tls = LockedSlot.FromInt(to+1);
            Console.WriteLine($"Switching from  {from} to {to}");
            if (HotBarPowers.Keys.Contains(tls))
            {
                //TO Slot is a LOCKED Slot!
                //A Hotbar Power might be tring to run 
                var p = HotBarPowers[tls];
                PowerHotBarInt pp = (PowerHotBarInt) p;
                pp.updateHotbar(p.getLS(), p.Cooldown, p);
            }


            //TODO CHECK FOR ACTIVE HOTBAR POWERS
            return true;
        }

        public string getConfirmWindowMessage()
        {
            String t =
                $"{ChatColors.Yellow}{ChatFormatting.Bold} Are you sure you want to change classes to {getDisplayName()} ?{ChatFormatting.Reset}\n" +
                $"===========================================\n" +
                $"Class Powers:\n";
            foreach (PowerAbstract cpa in USEABLEClassPowersList.Values)
            {
                t += " -> " + cpa.getDispalyName();
                if (cpa.isDefaultPower) t += " {Starting Power}";
                t += ChatFormatting.Reset + "\n";
            }

            t += "Buffs\n";
            foreach (KeyValuePair<BuffType, Buff> a in getBuffs())
            {
                t += " -> " + a.Key + " > + " + a.Value.getAmount() + "\n";
            }

            t += "DeBuffs\n";

            foreach (KeyValuePair<BuffType, DeBuff> a in getDeBuffs())
            {
                t += " -> " + a.Key + " > - " + a.Value.getAmount() + "\n";
            }

            return t;
        }

        public virtual bool CanSetPlayerClass(CorePlayer player)
        {
            return true;
        }

        // public abstract void PlayerAddedToClass(CorePlayer p);

        public BaseClass getsettablePlayerClass(CorePlayer corePlayer)
        {
            if (CanSetPlayerClass(corePlayer))
            {
                ActivateBuffs = true;
                corePlayer.SendMessage(
                    $"{ChatColors.Green}Success! Your class has now been set to {getDisplayName()}{ChatColors.Green}!");
                // P = corePlayer;
                // PlayerAddedToClass(P);
                return NewClassForPlayer(corePlayer);
            }
            else
                return null;
        }

        protected abstract BaseClass NewClassForPlayer(CorePlayer corePlayer);

        public bool ActivateBuffs { get; set; } = false;

        public LockedSlot ClaimFirstOpenPowerSlot()
        {
            Console.WriteLine("AAAAA");
            var aa = new List<LockedSlot>();
            foreach (var a in HotBarPowers)
            {
                Console.WriteLine("AA21222AAA");

                var v = a.Value;
                var k = a.Key;
                aa.Add(k);
                Console.WriteLine("AAAAccccccccA");
            }

            Console.WriteLine("AAAdddddddddddddddddddddddddddddddddAA");

            bool s7 = false;
            bool s8 = false;
            bool s9 = false;
            Console.WriteLine("AAAzzzzzzzazzzzzAA");

            if (aa.Contains(LockedSlot.SLOT_7)) s7 = true;
            if (aa.Contains(LockedSlot.SLOT_8)) s8 = true;
            if (aa.Contains(LockedSlot.SLOT_9)) s9 = true;
            Console.WriteLine("AAA555555555555555555555555AA");

            if (!s7) return LockedSlot.SLOT_7;
            if (!s8) return LockedSlot.SLOT_8;
            if (!s9) return LockedSlot.SLOT_9;
            return LockedSlot.NA;
        }
    }
}