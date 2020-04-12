using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using CyberCore.Manager.Factions.Windows;
using CyberCore.Manager.Forms;
using CyberCore.Utils;
using MiNET;
using MiNET.Plugins;
using MiNET.Plugins.Attributes;
using MiNET.UI;
using MiNET.Utils;
using OpenAPI.Player;

namespace CyberCore.Manager.Factions
{
    public class FactionCommands
    {
        private List<string> bannednames = new List<string>
        {
            "wilderness",
            "safezone",
            "peace"
        };

        private readonly FactionFactory Manager;

        public FactionCommands(FactionFactory manager)
        {
            Manager = manager;
        }
        //GOOD TO KNOW
        //typeof(OpenPlayer).IsAssignableFrom(parameter.ParameterType)

        [Command(Name = "f create", Description = "Create a new faction")]
        // [FactionPermission(FactionRankEnum.None)]
        public void FCreate(CorePlayer p)
        {
            if (p is CorePlayer)
            {
                CyberCoreMain.Log.Error("YOYOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOO");
                CyberCoreMain.Log.Error("YOYOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOO");
                CyberCoreMain.Log.Error("YOYOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOO");
                CyberCoreMain.Log.Error(((CorePlayer) p).GetRank().Name + " <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<< WORKZ");
                CyberCoreMain.Log.Error("YOYOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOO");
                CyberCoreMain.Log.Error("YOYOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOO");
            }
            else
            {
                CyberCoreMain.Log.Error(
                    "11111111111111111111111111111111111111111111111111YOYOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOO");
            }

            p.SendForm(new FactionCreate0());
        }

        [Command(Name = "f invite2", Description = "Create a new faction")]
        // [FactionPermission(FactionRankEnum.None)]
        public void FInvite2(OpenPlayer Sender, Target invited)
        {
            var m = "";
            Sender.SendMessage(m);
            CyberCoreMain.Log.Error("==========================================");
            CyberCoreMain.Log.Error(m);
            CyberCoreMain.Log.Error(invited);
            // CyberCoreMain.Log.Error(invited.Entities.Length);
            CyberCoreMain.Log.Error(invited.Players.Length);
            CyberCoreMain.Log.Error(invited.Rules.Length);
            CyberCoreMain.Log.Error(invited.Selector.Length);
            CyberCoreMain.Log.Error("==========================================");
        }

        [Command(Name = "f accept",Aliases =  new String[]{
            "f deny",
            "f invites"
        },Description = "View/Accept/Deny Faction Invites (Must not currently be in a faction)")]
        public void OpenFactionInvites(CorePlayer Sender)
        {
            if (Sender.getFaction() != null)
            {
                var fac = Manager.getPlayerFaction(Sender);
            }
            Sender.showFormWindow(new FactionInvitesWindow(Sender));
        }
        
        // [Command]
                    // public string CmdTarget(Target t)
                    // {
                    //     return $"{t}";
                    // }
                    // [Command]
                    // public string TestTest2(OpenPlayer p, OpenPlayer pp)
                    // {
                    //     return $"{p} | {pp}";
                    // }
                    [Command]
                    public string TestTest3(Player p, Player pp)
                    {
                        return $"{p} | {pp}";
                    }
                    [Command]
                    public string TestTest4(CorePlayer p, Player pp)
                    {
                        return $"{p.Username} | {pp.Username}";
                    }
                    // [Command]
                    // public string TestTest(CorePlayer p, CorePlayer pp)
                    // {
                    //     return $"{p} | {pp}";
                    // }
        
        [Command(Name = "f invite", Description = "Create a new faction")]
        public void FInvite(CorePlayer Sender, Target invite)
        {
            // var Sender = (CorePlayer) Sende;
            var invited = (CorePlayer) invite.getPlayer();
            if (invited == null)
            {
                Sender.SendMessage(FactionErrorString.Error_CMD_Invite_UnableToFindPlayer.getMsg() +
                                   "!@ SUPER ERROR!!!@@@");
                return;
            }

            if (null != Manager.getPlayerFaction(invited))
            {
                //TODO Allow Setting to ignore Faction messages
                Sender.SendMessage(FactionErrorString.Error_CMD_Invite_PlayerInFaction.getMsg());
                return;
            }
            

            var fac = Sender.getFaction();
            if (fac == null)
            {
                Sender.SendMessage(FactionErrorString.Error_NotInFaction.getMsg());
                return;
            }

            if (!Sender.getFactionRank().hasPerm(fac.getPermSettings().AllowedToInvite))
            {
                Sender.SendMessage(FactionErrorString.Error_No_Permission.getMsg()+$" You must be {fac.getPermSettings().AllowedToInvite.Id.ToString()}");
                return;
            }
            Sender.showFormWindow(new FactionInviteChooseRank(Sender, invited));
        }

        private bool IsValid(string value)
        {
            return Regex.IsMatch(value, @"^[a-zA-Z0-9]*$");
        }

        [Command(Name = "f info", Description = "Usage '/f info [faction]' | View your Faction Info or Others")]
        public void FInfo(CorePlayer Sender, string faction = null)
        {
            var cp = Sender;
            if (faction != null)
            {
                if (!IsValid(faction))
                {
                    Sender.SendMessage(ChatColors.Red + "Invalid Faction Name");
                    return;
                }

                var ffaction = Manager.getFaction(faction);
                if (ffaction == null)
                {
                    var lc = Manager.getFaction(Manager.factionPartialName(faction));
                    if (lc == null)
                    {
                        Sender.SendMessage(ChatColors.Red + $"Faction: '{faction}' does not exist");
                        return;
                    }

                    ffaction = lc;
                }

                var sb = new StringBuilder();
                sb.Append(formatString("Faction name", ffaction.getDisplayName())).Append("\n");
                sb.Append(formatString("Leader", ffaction.GetLeader())).Append("\n");
                sb.Append(formatString("# of Players", ffaction.getPlayerCount() + "")).Append("\n");
                sb.Append(formatString("Max # of Players", ffaction.GetMaxPlayers())).Append("\n");
                sb.Append(formatString("MOTD", ffaction.getSettings().getMOTD())).Append("\n");
                sb.Append(formatString("Desc", ffaction.getSettings().getDescription())).Append("\n");
                sb.Append(formatString("PowerAbstract", ffaction.GetPower())).Append("\n");
                sb.Append(formatString("Land Owned", ffaction.GetPlots().Count)).Append("\n");
                sb.Append(formatString("Money", ffaction.GetMoney())).Append("\n");
                sb.Append(formatString("XP", ffaction.getSettings().getXP())).Append("\n");
                sb.Append(formatString("Level", ffaction.getSettings().getLevel())).Append("\n");

                var f = new CyberFormSimple(MainForm.Faction_Info_Other);
                f.Title = "CyberFactions | " + ffaction.getDisplayName() + ChatFormatting.Reset + " Faction info";
                f.Content = sb.ToString();


                f.addButton("View All Players", CommingSoon);
                f.addButton("Invite to Ally", CommingSoon);
                f.addButton("Add to Enemy", CommingSoon);
                f.addButton("Message Faction", CommingSoon);
                f.addButton("Report Faction", CommingSoon);

                // CorePlayer cp = (CorePlayer) Sender;
                cp.showFormWindow(f);
                cp.LastSentFormType = MainForm.Faction_Info_Other;
            }
            else
            {
                var fac = Sender.getFaction();
                if (fac == null)
                {
                    if (Manager.getPlayerFaction(Sender) == null)
                    {
                        Sender.SendMessage(ChatColors.Red + "[CyberTech] You are not in a Faction2222!");
                    }
                    else
                    {
                        fac = Sender.getFaction();
                        if (fac == null)
                        {
                            Sender.SendMessage(ChatColors.Red + "[CyberTech] You are not in a Faction!");
                            return;
                        }
                    }
                }

                var sb = new StringBuilder();
                sb.Append(formatString("Faction name", fac.getDisplayName())).Append("\n");
                sb.Append(formatString("Leader", fac.GetLeader())).Append("\n");
                sb.Append(formatString("# of Players", fac.getPlayerCount() + "")).Append("\n");
                sb.Append(formatString("Max # of Players", fac.GetMaxPlayers())).Append("\n");
                sb.Append(formatString("MOTD", fac.getSettings().getMOTD())).Append("\n");
                sb.Append(formatString("Desc", fac.getSettings().getDescription())).Append("\n");
                sb.Append(formatString("PowerAbstract", fac.GetPower())).Append("\n");
                sb.Append(formatString("Land Owned", fac.GetPlots().Count)).Append("\n");
                sb.Append(formatString("Money", fac.GetMoney())).Append("\n");
                sb.Append(formatString("XP", fac.getSettings().getXP())).Append("\n");
                sb.Append(formatString("Level", fac.getSettings().getLevel())).Append("\n");


                var f = new CyberFormSimple(MainForm.Faction_Info_Other);
                f.Title = "CyberFactions | " + fac.getDisplayName() + ChatFormatting.Reset + " Faction info";
                f.Content = sb.ToString();


                f.addButton("View All Players", CommingSoon);
                f.addButton("View Allies", CommingSoon);
                f.addButton("View Enemies", CommingSoon);
                f.addButton("Open Inbox", CommingSoon);
                f.addButton("Report Faction", CommingSoon);

                cp.showFormWindow(f);
                cp.LastSentFormType = MainForm.Faction_Info_Self;
            }
        }

        private void CommingSoon(Player arg1, SimpleForm arg2)
        {
            arg1.SendMessage(ChatColors.Yellow + ChatFormatting.Bold + "Comming Soon!");
        }

        private string formatString(string keyname, object val)
        {
            return ChatColors.Yellow + keyname + ": " + ChatColors.Aqua + val;
        }
    }
}