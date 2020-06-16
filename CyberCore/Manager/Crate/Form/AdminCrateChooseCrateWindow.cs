using System;
using System.Linq;
using System.Numerics;
using CyberCore.Manager.Forms;
using MiNET;
using MiNET.Blocks;
using MiNET.UI;
using MiNET.Utils;
using Newtonsoft.Json;
using Button = MiNET.UI.Button;

namespace CyberCore.Manager.Crate.Form
{
    public class AdminCrateChooseCrateWindow : CyberFormSimple
    {
        [JsonIgnore] private Vector3 _V;
        [JsonIgnore] private CrateMain _C;

        public AdminCrateChooseCrateWindow(Vector3 v, CrateMain c) : base(MainForm.Crate_Admin_ChooseCrate, "Choose Crate")
        {
            _C = c;
            _V = v;
            foreach (CrateData cd in _C.getCrateMap().Values){
                addButton(new Button()
                {
                    Text = cd.Name,
                    ExecuteAction = onRun
                } );
            }
        }

        public void onRun(Player p, SimpleForm simpleForm)
        {
            if (ClickedID == -1)
            {
                CyberCoreMain.Log.Error("Could not RUn AdminChooseCrate because No Clicked ID");
                return;
            }

            int k = ClickedID;
            if (k > _C.getCrateMap().Count)
            {
                p.SendMessage("Invalid Selection!");
                return;
            }

            CrateData cd = (CrateData) _C.getCrateMap().Values.ToArray()[k];
            if (cd == null)
            {
                Console.WriteLine("Errrororororo!!!!!");
                return;
            }
_C.CreateCrate(_V,p.Level,cd);
            // _C.CrateChests[_V] = new CrateObject(new PlayerLocation(_V)+new Vector3(.5f,.5f,.5f), p.Level, cd);
            return;
        }
    }
}