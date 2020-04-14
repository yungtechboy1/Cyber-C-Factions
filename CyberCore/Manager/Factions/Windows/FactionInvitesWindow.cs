using System;
using System.Collections.Generic;
using CyberCore.Manager.Factions.Data;
using CyberCore.Manager.Forms;
using CyberCore.Utils;
using MiNET;
using MiNET.UI;
using MiNET.Utils;
using Newtonsoft.Json;

namespace CyberCore.Manager.Factions.Windows
{
    public class FactionInvitesWindow : CyberFormSimple
    {
        public FactionInvitesWindow(MainForm ttype, List<Button> bl, string desc = "") : base(ttype, bl, desc)
        {
        }

        [JsonIgnore] public List<String> Options = new List<string>();
        [JsonIgnore] public String FN;

        public FactionInvitesWindow(CorePlayer p, String factionname = null) : base(MainForm.Faction_View_Invites,
            "Faction Invite Manager ("+p.EPD.FactionInviteData.Count+")")
        {
            FN = factionname;
            Content = "Select a faction name below to Accept/Deny faction invites";
            if (factionname == null)
            {
                var r = new List<FactionInviteData>();
                foreach (var fid in p.EPD.FactionInviteData)
                {
                    if (!fid.isValid())
                    {
                        addButton(ChatColors.Gray+"[TIMEDOUT]"+ChatColors.Yellow+fid.getFaction()+" | ");
                        addButton(ChatColors.Gray+"[TIMEDOUT]"+ChatColors.Yellow+fid.getTimeStamp()+" |||| "+CyberUtils.getLongTime());
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
                    p.EPD.DeleteFactionInvite(rr);
                }
            }
            else
            {
                FactionInviteData fid = p.EPD.getFactionInviteDataFromFaction(factionname);
                if(fid != null)
                {
                    Content = ChatColors.Green+$"The Faction {CyberCoreMain.GetInstance().FM.FFactory.getFaction(fid.getFaction()).getDisplayName()} has invited you to join their faction!\n" +
                              $"If you choose to accept, you will enter the faction with Rank {ChatColors.Aqua}{fid.FacRank}";
                    addButton("Accept Invite", delegate(Player player, SimpleForm form)
                    {
                        var a = (FactionInvitesWindow)form;
                        var f = a.FN;
                        var fac = CyberCoreMain.GetInstance().FM.FFactory.getFaction(f);
                        fac.AcceptInvite((CorePlayer) player);
                    });
                    addButton("Deny Invite");
                    addButton("< Go Back", delegate(Player player, SimpleForm form) { 
                    player.SendForm(new FactionInvitesWindow((CorePlayer)player));
                     });
                    //Options
                    //Accept
                    //Deny
                }
            }
        }

        public FactionInvitesWindow(MainForm ttype, MainForm attype, List<Button> bl, string desc =
            "") : base(ttype, attype, bl, desc)
        {
        }
    }
}