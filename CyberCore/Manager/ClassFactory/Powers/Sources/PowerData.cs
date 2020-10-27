using System;
using CyberCore.CustomEnums;

namespace CyberCore.Manager.ClassFactory.Powers
{
    public class PowerData
    {
        PowerAbstract PA = null;
        AdvancedPowerEnum APE = null;
        bool Enabled = false;
        bool NeedsLockedSlot = false;
        LockedSlot LS = LockedSlot.NA;

        public AdvancedPowerEnum getAPE() {
            return APE;
        }
//    public PowerData(AdvancedPowerEnum powerID, bool enabled) {
//        PowerID = powerID;
//        Enabled = enabled;
//    }

        public PowerData(PowerAbstract PA) {
            this.PA = PA;
            APE = PA.getAPE();
            if(PA.getPowerSettings() != null){
                NeedsLockedSlot = PA.getPowerSettings().isHotbar;
            }
        }
//
//    public PowerData(PowerEnum powerID, bool enabled, bool nls) {
//        PowerID = powerID;
//        Enabled = enabled;
//        NeedsLockedSlot = nls;
//    }
//
//    public PowerData(PowerEnum powerID, bool enabled, bool nls, LockedSlot LS, PowerAbstract pa) {
//        PowerID = powerID;
//        NeedsLockedSlot = nls;
//        Enabled = enabled;
//        PA = pa;
//        this.LS = LS;
//    }

        public PowerAbstract getPA(){
            return PA;
        }

        public bool getNeedsLockedSlot() {
            return NeedsLockedSlot;
        }

        public void setNeedsLockedSlot(bool needsLockedSlot) {
            NeedsLockedSlot = needsLockedSlot;
        }

        public PowerEnum getPowerID() {
            return APE.getPowerEnum();
        }

//    public void setPowerID(PowerEnum powerID) {
//        PowerID = powerID;
//    }

        public bool getEnabled() {
            return Enabled;
        }

        public void setEnabled(bool enabled) {
            Enabled = enabled;
        }

        public LockedSlot getLS() {
            return LS;
        }

        public void setLS(LockedSlot l) {
            Console.WriteLine("LOCKEDSLOT22222222 SET FOR "+this.GetType().Name+" || "+l);
            NeedsLockedSlot = (l.Slot != LockedSlot.NA.Slot);
            if(l.Slot != LockedSlot.NA.Slot && getPA() != null)getPA().setLS(l);
            LS = l;
        }
    }
}