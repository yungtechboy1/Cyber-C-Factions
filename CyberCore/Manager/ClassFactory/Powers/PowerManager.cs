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

        public PowerManager(CyberCoreMain CCM) {
            this.CCM = CCM;
//        addPowerToList();


        }

        public static void addPowerToList(PowerEnum pe, Type c) {
            if (PowerList.ContainsKey(pe)) CyberCoreMain.Log.Info("WAS LOG"+"WARNING!!! Overwriting a Power with the Key > " + pe);
            PowerList[pe] = c;
        }

        public static PowerAbstract getPowerfromAPE(AdvancedPowerEnum pe) {
            return getPowerfromAPE(pe,null);
        }
        public static PowerAbstract getPowerfromAPE(AdvancedPowerEnum pe, BaseType b) {
            Type cpa = PowerList.get(pe.getPowerEnum());
            if (cpa == null) {
                CyberCoreMain.Log.Info("WAS LOG"+"NONE IN POWER LIST!!!" + pe);
                return null;
            }
            if (cpa.getSuperclass().isAssignableFrom(StagePowerAbstract.class)) {
                //Stage
                Constructor c;
                if (b == null) {
                    CyberCoreMain.Log.Info("WAS LOG"+"BASECLASS IS NULL!!!!!!!!!" + cpa.getName());
                    try {
                        c = cpa.getConstructor(AdvancedPowerEnum.class);
                        return (PowerAbstract) c.newInstance(pe);
                    } catch (Exception e) {
                        e.printStackTrace();
                        CyberCoreMain.Log.Info("WAS LOG"+"ERRORROOORROR C2  +++++=====  NUUULLLLLL");
                        return null;
                    }
                } else {
                    try {
                        c = cpa.getConstructor(BaseType.class, AdvancedPowerEnum.class);
                        return (PowerAbstract) c.newInstance(b, pe);
                    } catch (Exception e) {
                        CyberCoreMain.Log.Info("WAS LOG"+"ERRORROOORROR C  +++++=====  NUUULLLLLL");
                        return null;
                    }
                }
//        }else if(cpa.isAssignableFrom()){
            } else {
                CyberCoreMain.Log.Info("WAS LOG"+"ERROR! " + cpa.getName() + "|| " + cpa);
            }
//        return cpa;
            return null;
        }
    }
    }
