using System;
using System.Collections.Generic;
using CyberCore.CustomEnums;
using CyberCore.Manager.ClassFactory.Powers;
using CyberCore.Manager.Forms;
using MiNET;
using MiNET.UI;
using MiNET.Utils;
using Newtonsoft.Json;

namespace CyberCore.Manager.ClassFactory.Window
{
    public class MainClassSettingsWindowActivePowers :CyberFormCustom
    {
        [JsonIgnore] BaseClass _BC;
        
        public MainClassSettingsWindowActivePowers(String title, BaseClass bd) : base(MainForm.Main_Class_Settings_Window_Active_Powers)
        {
            Title = title;
            _BC = bd;
            ExecuteAction = onRun;
            inti();
        }

        public MainClassSettingsWindowActivePowers(BaseClass bd): base(MainForm.Main_Class_Settings_Window_Active_Powers)
        {
            Title = _BC.getDisplayName() + " Passive Power InternalPlayerSettings";
            _BC = bd;
            inti();
        }
        
        private void inti() {
                String ppn = ChatColors.Green + " [Currently Active]";
                String ppnn = ChatColors.Red + " [Inactive]";
            foreach (var pd in _BC.getClassSettings().getPowerDataList())
            {
                // Console.WriteLine("YEAHHHHH >>> " + pd + " || " + pd.getNeedsLockedSlot() + "||" + pd.getPowerID() + "||" + pd.getPowerID().Name);
                if (check(pd)) continue;//Can not Enable LockedSlot Powers here
                Console.WriteLine("\\/\\/\\/\\/ NExt!!");
                bool e = pd.getEnabled();
                PowerEnum pe = pd.getPowerID();
                PowerAbstract p = pd.getPA();//_BC.getPossiblePower(pe, false);
                Console.WriteLine("NOW PPPPPP >>> " + p + " || ");//+p.getDispalyName()+"||"+p.getName());
                String pn;
                if (p == null) pn = "UNKNOWN?!?" + e;
                else pn = p.getDispalyName();
                if (e)
                    pn += ppn;
                else
                    pn += ppnn;
                addElement(new Toggle(){Text = pn + ChatColors.Gray,Value = e});
            }
//        addButton(new ElementButton("<< Back"));
        }
        
        private bool check(PowerData pd) {
            return (pd.getLS().Slot != LockedSlot.NA.Slot || pd.getNeedsLockedSlot());//Can not Enable LockedSlot Powers here
        }

        /**
     * Return True only if a Response has been executed
     *
     * @param p CorePlayer
     * @return bool
     */
        public void onRun(Player p, CustomForm f) {
            int key = 0;
            foreach (var pd in _BC.getClassSettings().getPowerDataList())
            {
                if (check(pd)) continue;//Skip like above
                var a = (Toggle)Content[key];
                bool on = a.Value;
                bool b = _BC.getClassSettings().getEnabledPowers().Contains(pd.getPowerID());
                if (on && !b) {
                    _BC.enablePower(pd.getAPE());
//                _BC.getPlayer().sendMessage(TextFormat.GREEN+"POWER > "+pd.getPowerID().name()+" has been activated!");
                } else if (!on && b) {
                    _BC.disablePower(pd.getAPE());
                }
                key++;
            }
            p.SendForm(_BC.getSettingsWindow());
            // return key > 0;
        }
        
    }
}