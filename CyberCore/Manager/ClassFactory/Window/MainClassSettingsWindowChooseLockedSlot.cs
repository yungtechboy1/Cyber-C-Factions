using System;
using System.Collections.Generic;
using CyberCore.CustomEnums;
using CyberCore.Manager.ClassFactory.Powers;
using CyberCore.Manager.Forms;
using MiNET;
using MiNET.UI;
using Newtonsoft.Json;

namespace CyberCore.Manager.ClassFactory.Window
{
    public class MainClassSettingsWindowChooseLockedSlot : CyberFormCustom
    {
        [JsonIgnore] BaseClass _BC;
        [JsonIgnore] LockedSlot _LS;

        public MainClassSettingsWindowChooseLockedSlot(String title, BaseClass _BC, LockedSlot ls) : base(
            MainForm.Main_Class_Settings_Window_Active_Powers)
        {
            Title = title;
            this._BC = _BC;
            _LS = ls;
            inti();
        }

        public MainClassSettingsWindowChooseLockedSlot(BaseClass _BC, LockedSlot ls) : base(MainForm
            .Main_Class_Settings_Window_Active_Powers)
        {
            Title = _BC.getDisplayName() + " Power Slot InternalPlayerSettings";
            this._BC = _BC;
            _LS = ls;
            inti();
        }

        private void inti()
        {
            ExecuteAction = onRun;
            int d = 0;
            int k = 0;
            List<String> l = new List<String>();
            l.Add("N/A");
            foreach (var pa in _BC.getLearnedPowersAbstract())
            {
                k++;
                PowerEnum pe = pa.getType();
                if (!pa.isHotbarPower())
                {
                    Console.WriteLine(pe + " NO LOCKED SLOT!!!");
                    continue; //Can not Enable NOT LockedSlot Powers here
                }

//            boolean e = pa.isEnabled();
                if (Equals(pe, _BC.getClassSettings().getPreferedSlot(_LS))) d = k;
                String pn = pa.getDispalyName();
                l.Add(pn);
            }
//        for (PowerData pd : _BC.getClassSettings().getPowerDataList()) {
//            k++;
//            if (!pd.getNeedsLockedSlot()) {
//                Console.WriteLine(pd.getPowerID()+" NO LOCKED SLOT!!!");
//                continue;//Can not Enable NOT LockedSlot Powers here
//            }
//            boolean e = pd.getEnabled();
//            PowerEnum pe = pd.getPowerID();
//            if (pe == _BC.getClassSettings().getPreferedSlot(_LS)) d = k;
//            PowerAbstract p = pd.getPA();
//            String pn = p.getDispalyName();
//            l.add(pn);
//        }
            addElement(new Dropdown()
            {
                Text = "Choose Which Power Will be In Slot " + _LS.getSlot(),
                Options = l,
                Value = d
            });
//        addButton(new ElementButton("<< Back"));
        }

        /**
     * Return True only if a Response has been executed
     *
     * @param p CorePlayer
     * @return boolean
         */

        public void onRun(Player pp, CustomForm f)
        {
            CorePlayer p = (CorePlayer) pp;
            
            var a = (Dropdown)Content[0];
            var k = a.Value;
            if (k == 0)
            {
                _BC.deactivatePower(_BC.getClassSettings().getPreferedSlot(_LS));
            }
            else
            {
                var kk = 0;

                foreach (var pa in _BC.getLearnedPowersAbstract())
                {
                    kk++;
                    if (!pa.isHotbarPower())
                    {
                        Console.WriteLine(pa.getType() + " NO22222 LOCKED SLOT!!!");
                        continue; //Can not Enable NOT LockedSlot Powers here
                    }

                    if (kk == k)
                    {
                        _BC.enablePower(pa.getAPE(), _LS);
                        return;
                    }
                }
                p.SendMessage(" EROORRRRR!!!! E332242");
                Console.WriteLine("ERROR! Could not Enable Power");

//            for (PowerData pd : _BC.getClassSettings().getPowerDataList()) {
//
//                if (!pd.getNeedsLockedSlot()) continue;//Can not Enable NOT LockedSlot Powers here
//                kk++;
//                if (kk == k) {
//                    _BC.enablePower(pd);
////                    if(!pd.getEnabled()){
////                        p.sendMessage("Attempting to set active slot and Class!");
////                        _BC.enablePower(pd);
////                    }
//                }
//            }
            }


            p.SendForm(_BC.getSettingsWindow());
            return;
        }
    }
}