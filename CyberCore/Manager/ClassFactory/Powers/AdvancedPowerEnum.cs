using System;
using CyberCore.CustomEnums;
using CyberCore.Manager.TypeFactory.Powers;
using CyberCore.Utils;
using static CyberCore.CustomEnums.LevelingType;

namespace CyberCore.Manager.ClassFactory.Powers
{
    public class AdvancedPowerEnum
    {
        PowerEnum PE;
        StageEnum SE = StageEnum.NA;
        int XP = -1;
        LevelingType tt = None;

        public AdvancedPowerEnum(PowerEnum PE)
        {
            this.PE = PE;
        }

        public AdvancedPowerEnum(PowerEnum PE, int XP)
        {
            this.PE = PE;
            this.XP = XP;
            tt = XPLevel;
        }

        public AdvancedPowerEnum(PowerEnum PE, StageEnum SE)
        {
            if (SE.getValue() == StageEnum.NA.getValue())
                CyberCoreMain.Log.Error("APEEE>>>   Error! CAN NOT SEND NA!!!!");
//            throw new Exception("Error! Can not pass StageEnum.NA to AdvancedPowerEnum Constructor!");
            this.PE = PE;
            this.SE = SE;
            tt = Stage;
        }

        //0    1            2  3
        //APE|DragonJumper|XP|100
        public static AdvancedPowerEnum fromString(String s)
        {
            String[] ss = s.Split("\\|");
            if (ss.Length == 3)
            {
                //None, Invalid Save
                CyberCoreMain.Log.Error("Error! Invalid AdvancedPowerEnum Save from > " + s);
                return null;
            }

            PowerEnum pe = PowerEnum.fromstr(ss[1], new PowerEnum());
            if (pe.ID == PowerEnum.Unknown.ID)
            {
                CyberCoreMain.Log.Error("Error! Invalid PowerEnu Name from Save > " + s);
                return null;
            }

            if (ss[2].equalsIgnoreCase("xp"))
                return new AdvancedXPPowerEnum(pe, int.Parse(ss[3]));
            if (ss[2].equalsIgnoreCase("stage"))
                try
                {
                    return new AdvancedStagePowerEnum(pe,
                        StageEnum.getStageFromInt(int.Parse(ss[3])));
                }
                catch (Exception e)
                {
                    CyberCoreMain.Log.Error("APEEE>>>>>>>> ", e);
                }

            CyberCoreMain.Log.Error("Was LOG ||" + "Error! Why did this go all the way!?!?!? E110393");
            return null;
        }

        public void setSE(StageEnum SE)
        {
            this.SE = SE;
        }

        public bool isStage()
        {
            //Something so Simple low key caused 2 hours of work! >:(
            return getStageEnum().Level != StageEnum.NA.Level;
        }

        public PowerEnum getPowerEnum()
        {
            return PE;
        }

        public StageEnum getStageEnum()
        {
            return SE;
        }

        public int getXP()
        {
            return XP;
        }

        public LevelingType getTt()
        {
            return tt;
        }

        public bool isValid()
        {
            if (tt == None) return false;
            if (tt == XPLevel)
                if (XP == -1)
                    return false;
            if (tt == Stage) return SE.Level != StageEnum.NA.Level;
            return true;
        }

        public String toString()
        {
            String s = "APE|" + getPowerEnum() + "|";
            switch (tt)
            {
                case XPLevel:
                    s += "XP|" + getXP();
                    break;
                case Stage:
                    s += "Stage|" + getStageEnum().Level;
                    break;
                default:
                    s += "None";
                    break;
            }

            return s;
        }

        public bool checkEquals(AdvancedPowerEnum ape)
        {
            return toString().equalsIgnoreCase(ape.toString());
        }

        public bool sameType(AdvancedPowerEnum ape)
        {
            return getPowerEnum().ID == ape.getPowerEnum().ID;
        }

        public String getValue()
        {
            switch (tt)
            {
                case XPLevel:
                    return "XP =" + getXP();
                case Stage:
                    return "Stage = " + getStageEnum().Level;
                case None:
                default:
                    return "Unknown Data!";
            }
        }

        public String getLore1()
        {
            return getPowerEnum().Name;
        }

        public String getLore2()
        {
            switch (tt)
            {
                case XPLevel:
                    return "XP =" + getXP();
                case Stage:
                    return "Stage = " + getStageEnum().Level;
                case None:
                default:
                    return "------------";
            }
        }

        public String getLore3()
        {
            PowerAbstract ape = PowerManager.getPowerfromAPE(this);
            String s = "";
            if (ape == null)
            {
                s = "=====N/A=====";
            }
            else
            {
                if (ape.getAllowedClasses() == null || ape.getAllowedClasses().Count == 0)
                {
                    s = "== ANY CLASS ==";
                }
                else
                {
                    var c = CyberCoreMain.GetInstance();
                    s = "== Available Classes ==\n";
                    foreach (Type v in ape.getAllowedClasses())
                    {
                        try
                        {
                            BaseClass bc = (BaseClass) Activator.CreateInstance(v, c);
                            s += " - " + bc.getDisplayName() + "\n";
                        }
                        catch (Exception e)
                        {
                            CyberCoreMain.Log.Error($"APE: {getValue()} HAD ERROR CREATING CLASS >", e);
                            s += v.Name + "\n";
                        }
                    }
                }
            }

            return s;
        }

        public PowerEnum getPowerID()
        {
            return getPowerEnum();
        }

//    public ConfigSection toConfig() {
//        ConfigSection c = new ConfigSection();
//        if (tt == LevelingType.None) return c;
//        if (tt == LevelingType.XPLevel) {
//            if (XP == -1) return false;
//        }
//        if (tt == LevelingType.Stage) {
//            if (SE == StageEnum.NA) return false;
//        }
//    }
    }
}