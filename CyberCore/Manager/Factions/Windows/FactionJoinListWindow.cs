using System.Collections.Generic;
using CyberCore.Manager.Forms;
using MiNET;
using MiNET.UI;
using MiNET.Utils;

namespace CyberCore.Manager.Factions.Windows
{
    public class FactionJoinListWindow : CyberFormSimple
    {
        public FactionJoinListWindow(MainForm ttype, List<Button> bl, string title = "") : base(ttype, bl, title)
        {
        }

        public FactionJoinListWindow(int page = 0) : base(MainForm.Faction_Join_List,
            "CyberFactions | Joining an Open Faction")
        {
            int i = 0;
            int skip = 20 * page;
            var l = FactionFactory.GetInstance().GetAllOpenFactions();
            foreach (var f in l)
            {
                if (skip > 0)
                {
                    skip--;
                    continue;
                }

                i++;
                if (i > 20) continue;
                addButton((f.getDisplayName()),
                    delegate(Player player, SimpleForm form) { player.SendForm(new FactionJoinConfirm(page, f)); });
            }
        }

        public FactionJoinListWindow(List<string> l) : base(MainForm.Faction_Join_List,
            "CyberFactions | Choose an Open Faction")
        {
            Content = "We found multiple factions beginning with that name. Please choose one below.";
            int i = 0;
            foreach (var f in l)
            {
                i++;
                if (i > 20) continue;
                var ff = FactionFactory.GetInstance().getFaction(f);
                if (ff.canJoin())
                    addButton(f,
                        delegate(Player player, SimpleForm form) { player.SendForm(new FactionJoinConfirm(-1, ff)); });
            }

            if (i == 0)
            {
                Content += $"\n\n" +
                           $"{ChatColors.Red} ERROR! Sorry but there are no factions matching that name that are available to join";
            }
        }

        public FactionJoinListWindow(MainForm ttype, MainForm attype, List<Button> bl, string desc = "") : base(ttype,
            attype, bl, desc)
        {
        }
    }
}