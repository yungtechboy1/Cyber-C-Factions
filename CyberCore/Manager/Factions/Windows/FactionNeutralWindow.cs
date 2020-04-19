using System;
using CyberCore.Manager.Forms;
using MiNET;
using MiNET.UI;
using Newtonsoft.Json;

namespace CyberCore.Manager.Factions.Windows
{
    public class FactionNeutralWindow :CyberFormSimple
    {
        [JsonIgnore]
        public String Fac;
        public FactionNeutralWindow(String fac) : base(MainForm.Faction_Nuetral_Select, "Faction Neutral | Select Faction To re-set to Neutral")
        {
            Fac = fac;
            Content = $"Select a faction below to reset ally and enemy states back to neutral:\nThis will remove any existing Ally or Enemy setting for the selected faction! \n The following factions match the name '{fac}'";
            var a = plugin.FM.FFactory.factionPartialNameList(fac);
            foreach (var aa in a)
            {
                addButton(aa,delegate(Player player, SimpleForm form)
                {
                    var f = ((CorePlayer)player).getFaction();
                    f.resetNuetral(FactionFactory.GetInstance().getFaction(aa),player);
                });
            }
        }

    }
}