using System;
using System.Collections.Generic;
using CyberCore.CustomEnums;
using CyberCore.Manager.ClassFactory;
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

        public static PowerAbstract getPowerfromAPE(AdvancedPowerEnum pe, BaseClass b)
        {
            Type cpa = PowerList[pe.getPowerEnum()];
            if (cpa == null)
            {
                CyberCoreMain.Log.Info("WAS LOG" + "NONE IN POWER LIST!!!" + pe);
                return null;
            }
            
                CyberCoreMain.Log.Info("WAS LOG" + "ERROR! " + cpa.Name + "|| " + cpa);
            

//        return cpa;
            return null;
        }
    }
}