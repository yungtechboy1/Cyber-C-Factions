﻿using System.Collections.Generic;
using CyberCore.Manager.Factions.Data;
using CyberCore.Manager.Forms;
using LibNoise.Combiner;
using MiNET;
using MiNET.UI;
using Newtonsoft.Json;

namespace CyberCore.Manager.Factions.Windows
{
    public class FactionMainForm : CyberFormSimple
    {
        [JsonIgnore]
        public CorePlayer P;
        public FactionMainForm(CorePlayer p) : base(MainForm.Faction_Main_Window, "Faction Main Window")
        {
            P = p;
            var f = p.getFaction();

            var perm = f.getPlayerRank(p);
            if (f == null)
            {
                addButton("View Faction Invites",delegate(Player player, SimpleForm form) {
                    player.SendForm(new FactionInvitesWindow(p));
                  });
                addButton("Create Faction",delegate(Player player, SimpleForm form) {
                    player.SendForm(new FactionCreate0());
                  });
            }
            else
            {
                if(perm.hasPerm(f.getPermSettings().AllowedToEditSettings))addButton("Change MOTD");
                if(perm.hasPerm(f.getPermSettings().AllowedToPromote))addButton("Promote Member");
                if(perm.hasPerm(f.getPermSettings().AllowedToPromote))addButton("Demote Member");
                if(perm.hasPerm(f.getPermSettings().AllowedToKick))addButton("Kick Member");
                if(perm.hasPerm(f.getPermSettings().AllowedToViewInbox))addButton("Faction Inbox");
                if(perm.hasPerm(f.getPermSettings().AllowedToEditSettings))addButton("Faction Settings");
                addButton("Faction Shop?");
    if(perm.hasPerm(FactionRank.Leader))addButton("Set New Leader");
            }
        }

        public FactionMainForm(MainForm ttype, MainForm attype, List<Button> bl, string desc = "") : base(ttype, attype, bl, desc)
        {
        }
    }
}