using System.Collections.Generic;
using CyberCore.Manager.Forms;
using CyberCore.Utils;
using MiNET;
using MiNET.UI;
using Newtonsoft.Json;

namespace CyberCore.Manager.Crate.Form
{
    public class CrateConfirmWindow : CyberFormSimple
    {
        public CrateConfirmWindow(MainForm ttype, List<Button> bl, string desc = "") : base(ttype, bl, desc)
        {
        }

        // [JsonIgnore] private CrateMain.CrateAction CA;
        public CrateConfirmWindow(MainForm ttype, /*CrateMain.CrateAction ca,*/string desc = "" ) : base(ttype, desc)
        {
            // CA = ca;
            Buttons.Add(new Button()
            {
                Text = "Keep Adding",
                ExecuteAction = trueRun
            });
            Buttons.Add(new Button()
            {
                Text = "Stop Adding",
                // ExecuteAction = trueRun
            });
        }

        public void trueRun(Player p, SimpleForm simpleForm)
        {
            switch (FT)
            {
                case MainForm.Crate_Confirm_Add:
                    CyberCoreMain.GetInstance().CrateMain.addPrimedPlayer(p.getName().ToLower(),CrateMain.CrateAction.AddItemToCrate);
                    break;
                case MainForm.Crate_Confirm_Key_Assign:
                    CyberCoreMain.GetInstance().CrateMain.addPrimedPlayer(p.getName().ToLower(),CrateMain.CrateAction.AddKeyToCrate);
                    break;
                case MainForm.Crate_Confirm_ADD_Crate:
                    CyberCoreMain.GetInstance().CrateMain.addPrimedPlayer(p.getName().ToLower(),CrateMain.CrateAction.AddCrate);
                    break;
                    
            }
        }
    }
}