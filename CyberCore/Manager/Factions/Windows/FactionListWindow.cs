using System;
using System.Collections.Generic;
using CyberCore.Manager.Forms;
using MiNET;
using MiNET.UI;
using MiNET.Utils;

namespace CyberCore.Manager.Factions.Windows
{
    public class FactionListWindow : CyberFormSimple
    {
        public FactionListWindow(MainForm ttype, List<Button> bl, string title = "") : base(ttype, bl, title)
        {
        }

        public FactionListWindow(String fac = null, int page = 1) : base(MainForm.Faction_List_Window,
            "Please Choose a faction to get more info about")
        {
            Content = $"Choose a faction to learn more info about that faction!";
            if (fac == null)
            {
                int to = page * 10;
                int from = to - 10;
                // 5 -> 0 ||| 10 -> 5
                int x = 0;
                bool hasextraa = false;

                foreach (var e in FactionFactory.GetInstance().LocalFactionCache)
                {
                    String a = "";
                    Faction f = e.Value;
                    a = "";
                    // 0 < 5 && 0 >= 0
                    //   YES     NO
                    //0
                    //1 2 3 4 5
                    //0 < 10 && 0 >= 5
                    if (!(x < to && x >= from))
                    {
                        x++;
                        continue;
                    }

                    if (x > to)
                    {
                        hasextraa = true;
                        break;
                    }

                    x++;
                    //Privacy
                    a += ChatColors.DarkGreen + "" + x + " > ";
                    if (f.getSettings().getPrivacy() == 1)
                    {
                        a += ChatColors.Red + "[P] ";
                    }
                    else
                    {
                        a += ChatColors.Green + "[O] ";
                    }

                    a += f.getDisplayName();
                    addButton(a,
                        delegate(Player player, SimpleForm form)
                        {
                            player.SendForm(new FactionInfoWindow(f, page, fac));
                        });
                } 
                if(hasextraa)
                addButton("Next Page >",delegate(Player player, SimpleForm form) {
                    player.SendForm(new FactionListWindow(fac,page+1));
                  });
                if(page >= 2)addButton("Previous Page <",delegate(Player player, SimpleForm form) {
                    player.SendForm(new FactionListWindow(fac,page-1));
                  }); 
            }
            else
            {
                int to = page * 10;
                int from = to - 10;
                // 5 -> 0 ||| 10 -> 5
                int x = 0;
                bool hasextra = false;

                foreach (var e in FactionFactory.GetInstance().factionPartialNameList(fac))
                {
                    String a = "";
                    Faction f = FactionFactory.GetInstance().getFaction(e);
                    if (f == null) continue;
                    // 0 < 5 && 0 >= 0
                    //   YES     NO
                    //0
                    //1 2 3 4 5
                    //0 < 10 && 0 >= 5
                    if (!(x < to && x >= from))
                    {
                        x++;
                        continue;
                    }

                    if (x > to)
                    {
                        hasextra = true;
                        break;
                    }

                    x++;
                    //Privacy
                    a += ChatColors.DarkGreen + "" + x + " > ";
                    if (f.getSettings().getPrivacy() == 1)
                    {
                        a += ChatColors.Red + "[P] ";
                    }
                    else
                    {
                        a += ChatColors.Green + "[O] ";
                    }

                    a += f.getDisplayName();
                    addButton(a,
                        delegate(Player player, SimpleForm form)
                        {
                            player.SendForm(new FactionInfoWindow(f, page, fac));
                        });
                } 
                if(hasextra)addButton("Next Page >",delegate(Player player, SimpleForm form) {
                    player.SendForm(new FactionListWindow(fac,page+1));
                });
                if(page >= 2)addButton("Previous Page <",delegate(Player player, SimpleForm form) {
                    player.SendForm(new FactionListWindow(fac,page-1));
                });
            }
        }

        public FactionListWindow(MainForm ttype, MainForm attype, List<Button> bl, string desc = "") : base(ttype,
            attype, bl, desc)
        {
        }
    }
}