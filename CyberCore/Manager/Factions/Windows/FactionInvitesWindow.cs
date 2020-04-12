using System;
using System.Collections.Generic;
using CyberCore.Manager.Factions.Data;
using CyberCore.Manager.Forms;
using MiNET;
using MiNET.UI;
using Newtonsoft.Json;

namespace CyberCore.Manager.Factions.Windows
{
    public class FactionInvitesWindow : CyberFormSimple
    {
        public FactionInvitesWindow(MainForm ttype, List<Button> bl, string desc = "") : base(ttype, bl, desc)
        {
        }

        [JsonIgnore] public List<String> Options = new List<string>();

        public FactionInvitesWindow(CorePlayer p, String factionname = null) : base(MainForm.Faction_View_Invites,
            "Select a faction name below to Accept/Deny faction invites")
        {
            if (factionname != null)
            {
                var r = new List<FactionInviteData>();
                foreach (var fid in p.EPD.FactionInviteData)
                {
                    if (!fid.isValid())
                    {
                        r.Add(fid);
                        continue;
                    }

                    Options.Add(fid.getFaction());
                    addButton("Faction Invite from : " + fid.getFaction(),
                        delegate(Player player, SimpleForm form)
                        {
                            var a = (FactionInvitesWindow) form;
                            var aa = a.ClickedID;
                            player.SendForm(new FactionInvitesWindow((CorePlayer) player, a.Options[aa]));
                        });
                }
                
                addButton("Deny All Faction Invites ",
                    delegate(Player player, SimpleForm form)
                    {
                        var p = (CorePlayer) player;
                        p.EPD.clearFactionInvites();
                    });

                foreach (var rr in r)
                {
                    p.EPD.FactionInviteData.Remove(rr);
                }
            }
        }

        public FactionInvitesWindow(MainForm ttype, MainForm attype, List<Button> bl, string desc =
            "") : base(ttype, attype, bl, desc)
        {
        }
    }
}