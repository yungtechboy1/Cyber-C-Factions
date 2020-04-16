using System;
using System.Collections.Generic;
using CyberCore.Manager.Forms;
using MiNET;
using MiNET.UI;
using Newtonsoft.Json;

namespace CyberCore.Manager.Factions.Windows
{
    public class FactionEnemyWindow : CyberFormSimple
    {
   

        [JsonIgnore]
        public List<String> LL;
        [JsonIgnore]
        public Faction JF;

        public FactionEnemyWindow(List<String> l, CorePlayer sender) : base(MainForm.Faction_Enemy_Choose, "CyberFactions | Add Enemy Faction")
        {
            JF = sender.getFaction();
            Content = "Mulitple factions were found with that name, please choose one.";
            addButton("====Cancel/Exit===");
            int k = 0;
            LL = l;
            foreach (String p in l) {
                k++;
                if (k > 20) break;
                addButton((p),delegate(Player player, SimpleForm form)
                {
                    var few = (FactionEnemyWindow) form;
                    int c = this.ClickedID;
                    var s = few.LL[c];
                    few.JF.AddEnemy(FactionFactory.GetInstance().getFaction(s),sender);
                });
            }
            
        }
    }
}