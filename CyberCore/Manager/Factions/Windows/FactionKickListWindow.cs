using System.Collections.Generic;
using CyberCore.Manager.Forms;
using CyberCore.Utils;
using MiNET;
using MiNET.UI;
using Newtonsoft.Json;

namespace CyberCore.Manager.Factions.Windows
{
    public class FactionKickListWindow : CyberFormSimple
    {
        [JsonIgnore]
        public CorePlayer P;
        public FactionKickListWindow(CorePlayer p) : base(MainForm.Faction_Kick_List, "CyberFactions | Faction Kick Page")
        {
            var f = p.getFaction();
            Content =
                "Please select a player to kick. If you do not see a player then they are a higher rank and you can not kick them.";
            
            var l = f.PlayerRanks;
            foreach (var a in l)
            {
                var name = a.Key;
                var rank = a.Value;
                if(f.getPlayerRank(p).hasPerm(rank))addButton($"Kick {name}",delegate(Player player, SimpleForm form)
                {
                    var f = (FactionKickListWindow) form;
                    var p = f.P;
                    var fac = p.getFaction();
                    fac.KickOfflineorOnlinePlayer(f.Buttons[f.ClickedID].Text.Replace("Kick ",""));
                });
            }
        }
    }
}