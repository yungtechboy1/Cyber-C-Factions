﻿using System;
using System.Collections.Generic;
using CyberCore.CustomEnums;
using CyberCore.Manager.ClassFactory.Powers;
using CyberCore.Manager.TypeFactory.Powers;
using CyberCore.Utils;
using LibNoise.Combiner;
using static CyberCore.CustomEnums.LockedSlot;

namespace CyberCore.Manager.ClassFactory
{
    public class ClassSettingsObj
    {
     
    //ALL Player Powers
    //Powers that cant be given automatically
    //Class Level up powers too!
    private List<AdvancedPowerEnum> LearnedPowers = new List<AdvancedPowerEnum>();
    //Powers given from Class
    private List<PowerEnum> ClassDefaultPowers = new List<PowerEnum>();
    //Only Powers that are Currently Active
    private List<PowerEnum> ActivatedPowers = new List<PowerEnum>();
    private PowerEnum PreferedSlot9 = PowerEnum.Unknown;
    private PowerEnum PreferedSlot8 = PowerEnum.Unknown;
    private PowerEnum PreferedSlot7 = PowerEnum.Unknown;
    private BaseClass BC;

    public ClassSettingsObj(BaseClass bc) {
        BC = bc;
    }

    public ClassSettingsObj(BaseClass bc, Dictionary<String,Object> c) {
        LearnedPowers = ListtoAPEList((List<String>)c["lp"]);
        ClassDefaultPowers = ListStringtoPE((List<String>) c["cdp"]);
        ActivatedPowers = ListStringtoPE((List<String>) c["ap"]);
        var a = new PowerEnum();
        PreferedSlot9 = PowerEnum.fromstr((String) c["ps9"],a);
        PreferedSlot8 = PowerEnum.fromstr((String) c["ps8"],a);
        PreferedSlot7 = PowerEnum.fromstr((String) c["ps7"],a);
        BC = bc;
    }

    public List<PowerEnum> getEnabledPowers() {
        return ActivatedPowers;
    }

    public List<String> getActivatedPowersJSON() {
        List<String> i = new List<String>();
        foreach (PowerEnum e in getEnabledPowers()) {
            i.Add(e.Name);
        }
        return i;
    }

    public List<String> ListPEtoString(List<PowerEnum> p) {
        List<String> i = new List<String>();
        foreach (PowerEnum e in p) {
            i.Add(e.Name);
        }
        return i;
    }


    public List<AdvancedPowerEnum> ListtoAPEList(List<String> p) {
        List<AdvancedPowerEnum> i = new List<AdvancedPowerEnum>();
        foreach (String e in p) {
            i.Add(AdvancedPowerEnum.fromString(e));
        }
        return i;
    }

    public List<PowerEnum> ListStringtoPE(List<String> p) {
        List<PowerEnum> i = new List<PowerEnum>();
        var a = new PowerEnum();
        foreach (String e in p) {
            i.Add(PowerEnum.fromstr(e, a));
        }
        return i;
    }

    public List<AdvancedPowerEnum> getLearnedPowers() {
        return LearnedPowers;
    }

    public List<PowerEnum> getClassDefaultPowers() {
        return ClassDefaultPowers;
    }

    public PowerEnum getPreferedSlot9() {
        return PreferedSlot9;
    }

    public void setPreferedSlot(LockedSlot ls,PowerEnum pe) {
        if (ls.Equals(SLOT_7))
        {
            setPreferedSlot7(pe);
            
        }else if (ls.Equals(SLOT_8))
        {
            setPreferedSlot8(pe);
        }else if (ls.Equals(SLOT_9))
        {
            setPreferedSlot9(pe);
        }
    }
    public void setPreferedSlot9(PowerEnum preferedSlot9) {
        PreferedSlot9 = preferedSlot9;
    }

    public PowerEnum getPreferedSlot8() {
        return PreferedSlot8;
    }

    public void setPreferedSlot8(PowerEnum preferedSlot8) {
        PreferedSlot8 = preferedSlot8;
    }

    public PowerEnum getPreferedSlot7() {
        return PreferedSlot7;
    }

    public void setPreferedSlot7(PowerEnum preferedSlot7) {
        PreferedSlot7 = preferedSlot7;
    }

    public PowerEnum getPreferedSlot(LockedSlot ls) {
        if (ls.Equals(SLOT_9)) return PreferedSlot9;
        if (ls.Equals(SLOT_8)) return PreferedSlot8;
        if (ls.Equals(SLOT_7)) return PreferedSlot7;
        return PowerEnum.Unknown;
    }

    public Dictionary<String,Object> export()
    {
        var a = new Dictionary<String, Object>();
            a.Add("lp", ListAPEtoStrings(getLearnedPowers()));
            a.Add("cdp", ListPEtoString(getClassDefaultPowers()));
            a.Add("ap", ListPEtoString(getEnabledPowers()));
            a.Add("ps9", getPreferedSlot9().Name);
            a.Add("ps8", getPreferedSlot8().Name);
            a.Add("ps7", getPreferedSlot7().Name);
            return a;
    }

    private List<String> ListAPEtoStrings(List<AdvancedPowerEnum> learnedPowers) {
        List<String> c = new List<String>();
        foreach (AdvancedPowerEnum ape in learnedPowers) {
            if (!ape.isValid()) continue;
            c.Add(ape.toString());
        }
        return c;
    }
//    private ConfigSection ListAPEtoConfigSection(List<AdvancedPowerEnum> learnedPowers) {
//        ConfigSection c = new ConfigSection();
//        for(AdvancedPowerEnum ape: learnedPowers){
//            if(!ape.isValid() || ape.toConfig().isEmpty())continue;
//        c.put(ape.getPowerEnum().name(),ape.toConfig());
//        }
//    }

    /**
     * Returns All learned powers as a PowerData[] Active or Not
     * Which contains the class it self, The Slot that it should be assigned to
     * @return
     */
    public PowerData[] getPowerDataList() {
        List<PowerData> pd = new List<PowerData>();
        foreach (AdvancedPowerEnum pe in getLearnedPowers()) {
//            System.out.println("GPDL >>>>>>>> "+pe.getPowerEnum());
            PowerAbstract ppa = PowerManager.getPowerfromAPE(pe,BC);
            if(ppa == null){
                System.out.println("EEEEEE1342342 234423334 !!!!!!!!!!!!!!!!!!!!!!!!");
                continue;
            }
            boolean a = false;
            if(ppa.getAllowedClasses() != null) {
                for (Class b : ppa.getAllowedClasses()) {
                    if (BC.getClass().isAssignableFrom(b)) {
                        a = true;
                        break;
                    }
                }
                if (!a) continue;
            }
//            PowerAbstract pa = BC.getPossiblePower(pe, false);
            PowerData p = new PowerData(ppa);
            if (getPreferedSlot7() == pe.getPowerEnum()) p.setLS(SLOT_7);
            if (getPreferedSlot8() == pe.getPowerEnum()) p.setLS(SLOT_8);
            if (getPreferedSlot9() == pe.getPowerEnum()) p.setLS(SLOT_9);
            if(getEnabledPowers().contains(ppa.getType()))p.setEnabled(true);
            pd.add(p);
        }
        return pd.toArray(new PowerData[0]);
    }

    public void addActivePower(PowerEnum pe) {
        if (!ActivatedPowers.contains(pe)) ActivatedPowers.add(pe);
    }

    public void delActivePower(PowerEnum pe) {
        ActivatedPowers.remove(pe);
    }

    public void clearSlot7() {
        PreferedSlot7 = PowerEnum.Unknown;
    }

    public void clearSlot8() {
        PreferedSlot8 = PowerEnum.Unknown;
    }

    public void clearSlot9() {
        PreferedSlot9 = PowerEnum.Unknown;
    }

    //Ran to Activate Powers from InternalPlayerSettings
    public void check() {
        for (PowerData pd : getPowerDataList()) {
            if(pd.getPA() == null){
                System.out.println("ERRROO3333333R!!! "+pd.getPowerID());
                return;
            }
            BC.addPossiblePower(pd.getPA());
//            BC.enablePower(pd);
//            PowerAbstract pa = pd.getPA();
//            if(pa != null){
//                pa.setEnabled();
//            }
        }
    }

    public void learnNewPower(PowerEnum pe) {
        learnNewPower(new AdvancedPowerEnum(pe));
    }
    public void learnNewPower(PowerEnum pe, boolean silentfail) {
        learnNewPower(new AdvancedPowerEnum(pe), silentfail);
    }
    public boolean canLearnNewPower(AdvancedPowerEnum a){
        for(AdvancedPowerEnum ape: getLearnedPowers()){
            if(ape.getPowerEnum() == a.getPowerEnum() && ape.isValid())return false;
        }
        return true;
    }
    public void learnNewPower(AdvancedPowerEnum advancedPowerEnum) {
        learnNewPower(advancedPowerEnum,false);
    }
    public void learnNewPower(AdvancedPowerEnum advancedPowerEnum, boolean silentfail) {
        if(!canLearnNewPower(advancedPowerEnum) && !silentfail)BC.getPlayer().sendMessage(TextFormat.YELLOW+"ClassSettings > WARNING > Can not re-learn"+advancedPowerEnum.getPowerEnum()+" Power has already been learned!");
        LearnedPowers.add(advancedPowerEnum);
        BC.getPlayer().sendMessage(TextFormat.GREEN+"ClassSettings > "+advancedPowerEnum.getPowerEnum()+" Power has now been learned!");
    }

    public void delLearnedPower(PowerEnum pe) {
        if(pe == null)return;
        int k = 0;
        System.out.println("LFFFF >>> "+pe);
        for(AdvancedPowerEnum ape: LearnedPowers){
            System.out.println("|||||"+ape);
            if(ape.getPowerEnum() == pe)break;
            k++;
        }
        if(k < LearnedPowers.size())LearnedPowers.remove(k);
    }

    public boolean isPowerLearned(AdvancedPowerEnum pe) {
        return isPowerLearned(pe.getPowerEnum());
    }
    public boolean isPowerLearned(PowerEnum pe) {
        for(AdvancedPowerEnum ape: LearnedPowers){
            if(ape.getPowerEnum() == pe)return true;
        }
        return false;
    }

    public void delLearnedPowerAndLearnIfNotEqual(AdvancedPowerEnum ape) {
        boolean rl = false;
        for(AdvancedPowerEnum aa: LearnedPowers){
            if(aa.sameType(ape)) {
                if(!aa.checkEquals(ape)) {
                    rl = true;
                }
               break;
            }
        }
        if(rl){
                delLearnedPower(ape);
                learnNewPower(ape, true);
        }
    }

    public void delLearnedPower(AdvancedPowerEnum ape) {
        delLearnedPower(ape.getPowerEnum());
    }
}
   
    }