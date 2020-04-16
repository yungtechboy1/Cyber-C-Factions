using System;
using System.Collections.Generic;
using CyberCore.Manager.Forms;
using MiNET;
using MiNET.UI;
using Newtonsoft.Json;

namespace CyberCore.Manager.Factions.Windows
{
    public class FactionAllyWindow :CyberFormSimple
    {
        [JsonIgnore]
        public String Fac;
        public FactionAllyWindow(String fac) : base(MainForm.Faction_Ally_Select, "Faction Ally | Select Faction To Send Ally Invite To")
        {
            Fac = fac;
            Content = $"Select a faction below to send a request to:\n The following factions match the name '{fac}'";
            var a = plugin.FM.FFactory.factionPartialNameList(fac);
            foreach (var aa in a)
            {
                addButton(aa,delegate(Player player, SimpleForm form) {
                    player.SendMessage(aa);
                  });
            }
        }

    }
}