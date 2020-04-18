using System.Collections.Generic;
using CyberCore.Manager.Forms;
using MiNET;
using MiNET.UI;

namespace CyberCore.Manager.Factions.Windows
{
    public class FactionAllyHomesPage : CyberFormSimple
    {
        public FactionAllyHomesPage(CorePlayer p, Faction f) : base(MainForm.Faction_Home_Page, "-")
        {
            Title = f.getDisplayName() + "'s Homes and Ally Homes";
            var hl = f.GetHome().Values;
            addButton("Go Back >",
                delegate(Player player, SimpleForm form)
                {
                    player.SendForm(new FactionHomesPage((CorePlayer) player));
                });
            foreach (var h in hl)
                addButton($"{h.toDisplayString()}", delegate(Player player, SimpleForm form)
                {
                    var ff = (FactionHomesPage) form;
                    var cp = (CorePlayer) player;
                    cp.delayTeleport(h.getVector3());
                });
        }
    }
}