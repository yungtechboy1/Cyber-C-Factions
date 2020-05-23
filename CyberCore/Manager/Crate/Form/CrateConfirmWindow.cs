using System.Collections.Generic;
using CyberCore.Manager.Forms;
using CyberCore.Utils;
using MiNET;
using MiNET.UI;

namespace CyberCore.Manager.Crate.Form
{
    public class CrateConfirmWindow : CyberFormSimple
    {
        public CrateConfirmWindow(MainForm ttype, List<Button> bl, string desc = "") : base(ttype, bl, desc)
        {
        }

        public CrateConfirmWindow(MainForm ttype, string desc = "") : base(ttype, desc)
        {
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

        public CrateConfirmWindow(MainForm ttype, MainForm attype, List<Button> bl, string desc = "") : base(ttype, attype, bl, desc)
        {
        }

        public void trueRun(Player p, SimpleForm simpleForm)
        {
            switch (FT)
            {
                case MainForm.Crate_Confirm_Add:
                    CyberCoreMain.GetInstance().CrateMain.SetCrateItemPrimedPlayer.Add(p.getName());
                    CyberCoreMain.GetInstance().CrateMain.PrimedPlayer.Add(p.getName());
                    break;
                case MainForm.Crate_Confirm_Key_Assign:
                    CyberCoreMain.GetInstance().CrateMain.PrimedPlayer.Add(p.getName());
                    CyberCoreMain.GetInstance().CrateMain.SetKeyPrimedPlayer.Add(p.getName());
                    
                    break;
                case MainForm.Crate_Confirm_ADD_Crate:
                    CyberCoreMain.GetInstance().CrateMain.PrimedPlayer.Add(p.getName());
                    CyberCoreMain.GetInstance().CrateMain.SetKeyPrimedPlayer.Add(p.getName());
                    break;
                    
            }
        }
    }
}