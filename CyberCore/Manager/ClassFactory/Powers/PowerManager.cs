using System;
using System.Collections.Generic;
using CyberCore.CustomEnums;
using CyberCore.Manager.ClassFactory.Powers;

namespace CyberCore.Manager.TypeFactory.Powers
{
    public class PowerManager
    {
        public static Dictionary<PowerEnum, Type> PowerList = new Dictionary<PowerEnum, Type>();

        CyberCoreMain CCM;

        public PowerManager(CyberCoreMain CCM)
        {
            this.CCM = CCM;
//        addPowerToList();
        }

        public static void addPowerToList(PowerEnum pe, Type c)
        {
            if (PowerList.ContainsKey(pe))
                CyberCoreMain.Log.Info("WAS LOG" + "WARNING!!! Overwriting a Power with the Key > " + pe);
            PowerList[pe] = c;
        }

        public static PowerAbstract getPowerfromAPE(AdvancedPowerEnum pe)
        {
            return getPowerfromAPE(pe, null);
        }

        public static PowerAbstract getPowerfromAPE(AdvancedPowerEnum pe, Type b)
        {
            Type cpa = PowerList[pe.getPowerEnum()];
            if (cpa == null)
            {
                CyberCoreMain.Log.Info("WAS LOG" + "NONE IN POWER LIST!!!" + pe);
                return null;
            }

            if (cpa.BaseType == typeof(StagePowerAbstract))
            {
                if (b == null)
                {
                    CyberCoreMain.Log.Info("WAS LOG" + "BASECLASS IS NULL!!!!!!!!!" + cpa.Name);
                    //Stage
                    StagePowerAbstract o = (StagePowerAbstract) Activator.CreateInstance(cpa, pe);


                    if (o == null)
                    {
                        CyberCoreMain.Log.Error("WAS LOG" + "ERRORROOORROR C2  +++++=====  NUUULLLLLL");
                        return null;
                    }

                    return o;
                }
                else
                {
                    CyberCoreMain.Log.Info("WAS LOG" + "BASECLASS IS NULL!!!!!!!!!" + cpa.Name);
                    //Stage
                    var o = (StagePowerAbstract) Activator.CreateInstance(cpa, b, pe);
                    if (o == null)
                    {
                        CyberCoreMain.Log.Error("WAS LOG" + "ERRORROOORROR44444444 C2  +++++=====  NUUULLLLLL");
                        return null;
                    }

                    return o;
                }

//        }else if(cpa.isAssignableFrom()){
            }
            else
            {
                CyberCoreMain.Log.Info("WAS LOG" + "ERROR! " + cpa.Name + "|| " + cpa);
            }

//        return cpa;
            return null;
        }
    }
}