using System;
using CyberCore.CustomEnums;
using CyberCore.Manager.TypeFactory.Powers;
using CyberCore.Utils;

namespace CyberCore.Manager.ClassFactory.Powers
{
    public class AdvancedPowerEnum
    {
        PowerEnum PwrEnum;
        int XP = -1;


        public AdvancedPowerEnum(PowerEnum pwrEnum, int xp = 0)
        {
            this.PwrEnum = pwrEnum;
            this.XP = xp;
        }


        //0    1            2  3
        //APE|DragonJumper|XP|100
        public static AdvancedPowerEnum fromString(String s)
        {
            String[] ss = s.Split("|");
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
                return new AdvancedPowerEnum(pe, int.Parse(ss[3]));

            CyberCoreMain.Log.Error("Was LOG ||" + "Error! Why did this go all the way!?!?!? E110393");
            return null;
        }

        public PowerEnum getPowerEnum()
        {
            return PwrEnum;
        }

        public int getXP()
        {
            return XP;
        }


        public bool isValid()
        {
            if (XP == -1)
                return false;
            return true;
        }

        public String toString()
        {
            String s = "APE|" + getPowerEnum() + "|XP|" + getXP();
            ;
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
            return "XP =" + getXP();
        }

        public String getLore1()
        {
            return getPowerEnum().Name;
        }

        public String getLore2()
        {
            return "XP =" + getXP();
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

//    public Dictionary<String,Object> toConfig() {
//        Dictionary<String,Object> c = new Dictionary<String,Object>();
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