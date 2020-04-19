using System.Collections.Generic;
using CyberCore.Manager.Forms;
using LibNoise.Combiner;
using MiNET;
using MiNET.UI;

namespace CyberCore.Manager.Factions.Windows
{
    public class FactionPromoteDemoteWindow : CyberFormSimple
    {
        /**
         * Promote  = True / Demote = false 
         */
        public FactionPromoteDemoteWindow(bool b, CorePlayer sender) : base(MainForm.Faction_Promote_Demote, $"Choose A Player to ")
        {
            var t = b ? "Promote" : "Demote";
            Title += t;
            Content = $"Please select a member in your faction to {t}";
            var f = sender.getFaction();
            foreach (var v in f.PlayerRanks)
            {
                var k = v.Key;
                var r = v.Value.toEnum();
                addButton($"{t} {k} From {r}",delegate(Player player, SimpleForm form)
                {
                    if (b)
                    {
                        f.PromotePlayer(k);
                    }
                    else
                    {
                        f.DemotePlayer(k);
                    }

                });

            }
        }
    }
}