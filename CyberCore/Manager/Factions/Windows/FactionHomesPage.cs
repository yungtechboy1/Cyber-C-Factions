using System.Collections.Generic;
using CyberCore.Manager.Forms;
using MiNET;
using MiNET.UI;
using MiNET.Utils;
using Newtonsoft.Json;

namespace CyberCore.Manager.Factions.Windows
{
    public class FactionHomesPage : CyberFormSimple
    {
        [JsonIgnore]
        public CorePlayer P;
        public FactionHomesPage(CorePlayer p) : base(MainForm.Faction_Home_Page, "-")
        {
            var f = p.getFaction();
            Title = f.getDisplayName() + "'s Homes and Ally Homes";
            var hl = f.GetHome().Values;
            foreach (var h in hl)
            {
                addButton($"{h.toDisplayString()}", delegate(Player player, SimpleForm form)
                {
                    var ff = (FactionHomesPage) form;
                    var cp = (CorePlayer) player;
                    cp.delayTeleport(h.getVector3());
                });
            }

            foreach (var a in f.GetAllies())
            {
                var ff = FactionsMain.GetInstance().FFactory.getFaction(a);
                if (a != null && ff.getPermSettings().AllowAlliesToTPToHomes)
                {
                    addButton($"View {ff.getDisplayName()}'s {ChatFormatting.Reset}Homes", delegate(Player player, SimpleForm form)
                    {
                        player.SendForm(new FactionAllyHomesPage((CorePlayer) player,ff));
                    });
                        
                }
            }
        }
    }
}