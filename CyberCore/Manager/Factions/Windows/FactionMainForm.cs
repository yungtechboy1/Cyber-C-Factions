using System.Collections.Generic;
using CyberCore.Manager.Factions.Data;
using CyberCore.Manager.Forms;
using LibNoise.Combiner;
using MiNET;
using MiNET.UI;
using Newtonsoft.Json;
using OpenAPI.Events.Player;

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
            var perm = f.getPlayerRank(p);
                // if(perm.hasPerm(f.getPermSettings().AllowedToEditSettings))addButton("Change MOTD",delegate(Player player, SimpleForm form) {
                //     player.SendForm(new FactionSettingsWindow(f));
                //  });
                // if (perm.hasPerm(f.getPermSettings().AllowedToPromote)) addButton("Change Leader",
                //     delegate(Player player, SimpleForm form)
                //     {
                //         player.SendForm(new FactionChangeLeaderWindow());
                //     }
                //
                //     );
                if(perm.hasPerm(f.getPermSettings().AllowedToPromote))addButton("Promote Member",delegate(Player player, SimpleForm form) {
                player.SendForm(new FactionPromoteDemoteWindow(true,(CorePlayer) player));
                  });
                if(perm.hasPerm(f.getPermSettings().AllowedToPromote))addButton("Demote Member",delegate(Player player, SimpleForm form) {
                    player.SendForm(new FactionPromoteDemoteWindow(false,(CorePlayer) player));
                });
                if(perm.hasPerm(f.getPermSettings().AllowedToKick))addButton("Kick Member",delegate(Player player, SimpleForm form) {
                    player.SendForm(new FactionKickListWindow((CorePlayer) player));
                });
                if(perm.hasPerm(f.getPermSettings().AllowedToViewInbox))addButton("Faction Inbox",delegate(Player player, SimpleForm form) {
                    player.SendForm(new FactionInboxWindow((CorePlayer) player));
                });
                if(perm.hasPerm(f.getPermSettings().AllowedToEditSettings))addButton("Faction Settings",delegate(Player player, SimpleForm form) {
                    player.SendForm(new FactionSettingsWindow(f));
                });
                addButton("Faction Shop?");
    if(perm.hasPerm(FactionRank.Leader))addButton("Set New Leader");
            }
        }

        public FactionMainForm(MainForm ttype, MainForm attype, List<Button> bl, string desc = "") : base(ttype, attype, bl, desc)
        {
        }
    }
}