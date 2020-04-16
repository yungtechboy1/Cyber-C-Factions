using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using CyberCore.Manager.Factions.Windows;
using CyberCore.Manager.Forms;
using CyberCore.Manager.Rank;
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
        private readonly FactionFactory Manager;

        private List<string> bannednames = new List<string>
        {
            "wilderness",
            "safezone",
            "peace"
        };

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
                CyberCoreMain.Log.Error(p.GetRank().Name + " <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<< WORKZ");
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

        [Command(Name = "f accept", Aliases = new[]
        {
            "f deny",
            "f invites"
        }, Description = "View/Accept/Deny Faction Invites (Must not currently be in a faction)")]
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
                Sender.SendMessage(FactionErrorString.Error_No_Permission.getMsg() +
                                   $" You must be {fac.getPermSettings().AllowedToInvite.Id.ToString()}");
                return;
            }

            Sender.showFormWindow(new FactionInviteChooseRank(Sender, invited));
        }

        private bool IsValid(string value)
        {
            return Regex.IsMatch(value, @"^[a-zA-Z0-9]*$");
        }

        [FactionCommand]
        [Command(Name = "f balance", Description = "Check faction Money Balance")]
        public string fbalance(CorePlayer p, string fac = null)
        {
            Faction f;
            if (string.IsNullOrEmpty(fac))
            {
                f = p.getFaction();
            }
            else
            {
                f = FactionFactory.GetInstance().getFaction(fac);
                if (f == null)
                {
                    var fn = FactionFactory.GetInstance().factionPartialName(fac);
                    if (fn == null) return ChatColors.Red + " Error no faction found with the name " + fac;

                    f = FactionFactory.GetInstance().getFaction(fn);
                }
            }

            var money = f.GetMoney();
            return FactionsMain.NAME + ChatColors.Green + "Your Faction has " + ChatColors.Aqua + money;
        }

        [FactionCommand]
        [Command(Name = "f chat", Description = "Send Chat to Faction")]
        public void fchat(CorePlayer p)
        {
            p.showFormWindow(new FactionChatChoose());
        }

        [FactionCommand]
        [Command(Name = "f chat", Description = "Send Chat to Faction")]
        public void fchat(CorePlayer p, string[] msgs)
        {
            var fac = p.getFaction();
            var msg = string.Join(" ", msgs);
            fac.AddFactionChatMessage(msg, p);
        }

        [FactionCommand]
        [Command(Name = "f allychat", Description = "Send Chat to Faction")]
        public void fallychat(CorePlayer p, string[] msgs)
        {
            var fac = p.getFaction();
            var msg = string.Join(" ", msgs);
            fac.AddAllyChatMessage(msg, p);
        }

        [FactionCommand]
        [Command(Name = "f claim", Description = "Claim Faction Land")]
        public void fclaim(CorePlayer p, int Radius = 1)
        {
            var fac = p.getFaction();
            var r = fac.getPlayerRank(p);
            if (r.toEnum() != FactionRankEnum.None)
                if (!r.hasPerm(fac.getPermSettings().getAllowedToClaim()))
                {
                    p.SendMessage("Error! You don't have permission to Claim Plots!");
                    return;
                }

            if (Radius > 1)
            {
                var rr = Radius * Radius;
                var money = 5000 * rr;
                var power = rr;
                if (fac.getSettings().getMoney() > money)
                {
                    p.SendMessage(FactionsMain.NAME + ChatColors.Red + "Your Faction does not have $" + money +
                                  " in your faction account!");
                    return;
                }

                if (fac.getSettings().getPower() < power)
                {
                    p.SendMessage(FactionsMain.NAME + ChatColors.Red + "Your Faction does not have enough power!");
                    return;
                }

                for (var x = -1 * Radius; x < Radius; x++)
                for (var z = -1 * Radius; z < Radius; z++)
                {
                    var xx = ((int) p.KnownPosition.X >> 4) + x;
                    var zz = ((int) p.KnownPosition.Z >> 4) + z;
                    ClaimLand(p, xx, zz);
                }
            }
            else
            {
                var x = (int) p.KnownPosition.X >> 4;
                var z = (int) p.KnownPosition.Z >> 4;
                //amount = (100) * Main.prefs["PlotPrice"];
                var money = 5000;
                var power = 1;
                if (fac.GetMoney() < money)
                {
                    p.SendMessage(FactionsMain.NAME + ChatColors.Red + "Your Faction does not have $" + money +
                                  " in your faction account!");
                    return;
                }

                if (fac.GetPower() < power)
                {
                    p.SendMessage(FactionsMain.NAME + ChatColors.Red + "Your Faction does not have " + power +
                                  " PowerAbstract!");
                    return;
                }

                var f = Manager.checkPlot(x, z);
                if (f != null)
                {
                    p.SendMessage(FactionsMain.NAME + ChatColors.Red + "That land is already Claimed by" + f.getName() +
                                  "'s Faction!!");
                    return;
                }

                p.SendMessage(FactionsMain.NAME + ChatColors.Green +
                              "Purchase Successful! $5000 and 1 PowerAbstract Withdrawn To Purchase This Chunk!");
                fac.TakeMoney(money);
                fac.AddPlots(x, z, p);
                fac.TakePower(power);
                fac.AddPlots(x, z, p);
//            Main.FFactory.PlotsList.put(x + "|" + z, fac.getName());
            }
        }

        private void ClaimLand(CorePlayer Sender, int x, int z)
        {
            var fac = Sender.getFaction();
            var money = 5000;
            var power = 1;
            if (fac.GetMoney() < money)
            {
                Sender.SendMessage(FactionsMain.NAME + ChatColors.Red + "Your Faction does not have $" + money +
                                   " in your faction account to claim Chunk at X:" + x + " Z:" + z + "!");
                return;
            }

            if (fac.GetPower() < power)
            {
                Sender.SendMessage(FactionsMain.NAME + ChatColors.Red + "Your Faction does not have " + power +
                                   " PowerAbstract to claim Chunk at X:" + x + " Z:" + z + "!");
                return;
            }

            if (Manager.PM.isPlotClaimed(x, z))
            {
                Sender.SendMessage(FactionsMain.NAME + ChatColors.Red + "That Chunk at X:" + x + " Z:" + z +
                                   " is already Claimed by" + Manager.PM.getFactionFromPlot(x, z) + "'s Faction!!");
                return;
            }

            Sender.SendMessage(FactionsMain.NAME + ChatColors.Green +
                               "Purchase Successful! $5000 and 1 PowerAbstract Withdrawn To Purchase Chunk at X:" + x +
                               " Z:" + z + "!");
            fac.TakeMoney(money);
            fac.AddPlots(x, z, Sender);
            fac.TakePower(power);
            fac.AddPlots(x, z, Sender);
//        Main.FFactory.PlotsList.put(x + "|" + z, fac.getName());
        }

        [Command(Name = "f enemy", Description = "Deposit Money into Faction")]
        [FactionCommand]
        public void fenemy(CorePlayer Sender, string targetfaction)
        {
            var tf = targetfaction;
            var target = Manager.getFaction(targetfaction);
            if (target == null)
            {
                //Partial Name Search
                var l = Manager.factionPartialNameList(tf);
                if (l.Count == 0)
                {
                    Sender.SendMessage(FactionErrorString.Error_UnableToFindFaction.getMsg());
                    Sender.SendMessage(ChatColors.Red + "Error the faction containing '" + tf +
                                       "' could not be found!");
                }
                else if (l.Count == 1)
                {
                    tf = l[0];
                    Sender.getFaction().AddEnemy(Manager.getFaction(tf), Sender);
                }
                else
                {
                    Sender.showFormWindow(new FactionEnemyWindow(l, Sender));
                }
            }
        }

        [Command(Name = "f deposit", Description = "Deposit Money into Faction")]
        [FactionCommand]
        public void fdeposit(CorePlayer Sender, int money)
        {
            var fac = Sender.getFaction();
            if (money == null || money == 0)
            {
                Sender.SendMessage(FactionsMain.NAME + ChatColors.Gray + "Usage /f deposit <amount>");
                return;
            }

            if (!Sender.MakeTransaction(money))
            {
                Sender.SendMessage(FactionsMain.NAME + ChatColors.Red + "You don't have " + money + " Money!");
                return;
            }

            fac.getSettings().addMoney(money);
            Sender.SendMessage(FactionsMain.NAME + ChatColors.Green + "$" + money + " Money Added to your Faction!");
            fac.BroadcastMessage(FactionsMain.NAME + ChatColors.Green + Sender.getName() + " has deposited $" + money +
                                 " Money to the faction account!");
        }

        [Command(Name = "f demote", Description = "Demote Player in Faction")]
        [FactionCommand]
        public void fdemote(CorePlayer Sender, Target player)
        {
            var fac = Sender.getFaction();
            var pp = player.getPlayer();
            if (pp == null)
            {
                Sender.SendMessage(FactionsMain.NAME + ChatColors.Red + "Player Is Not Online!");
                ;
                return;
            }

            var ppn = pp.getName();
            if (!Manager.Main.isInFaction(ppn))
            {
                Sender.SendMessage(FactionsMain.NAME + ChatColors.Red + "Target Player Not In Your Faction!");
                return;
            }

            var r = fac.getPlayerRank((Player) Sender);
            var fr = fac.getPermSettings().getAllowedToPromote();
            if (r.hasPerm(fr)) fac.DemotePlayer(pp);
        }

        [Command(Name = "f delete", Description = "Delete Faction")]
        [FactionCommand]
        public void fdelete(CorePlayer Sender)
        {
            var fac = Sender.getFaction();
            if (fac.GetLeader().equalsIgnoreCase(Sender.getName()))
                Sender.showFormWindow(new FactionConfirmDelete());
            else
                Sender.SendMessage(FactionsMain.NAME + ChatColors.Red + "You are not the leader!");
        }

        [Command(Name = "f admin", Description = "Admin Command")]
        [ServerRankAttr(RankEnum.JrServerAdmin)]
        public void fadmin(CorePlayer p)
        {
            p.SendForm(new FactionAdminPage1());
        }

        //F ally CyberTech
        [FactionCommand]
        [Command(Name = "f ally", Description = "")]
        public void fally(CorePlayer p, string facname)
        {
            var fac = p.getFaction();
            var r = fac.getPlayerRank(p);
            if (!fac.getPermSettings().getAllowedToAcceptAlly().hasPerm(r))
            {
                p.SendMessage(
                    ChatColors.Red + "Error, you do not have permission to send ally requests or accept them!");
                return;
            }

            if (!fac.getName().equalsIgnoreCase(facname))
            {
                p.SendMessage(
                    ChatColors.Red + "Error, you can not ally your own faction!");
                return;
            }

            p.SendForm(new FactionAllyWindow(facname));

//             Faction target = Manager.getFaction(facname);
//             if (target == null) {
//                 target = Manager.getFaction(Manager.factionPartialName(Args[1]));
//                 if (target == null) {
//                     Sender.SendMessage(ChatColors.Red + "Error the faction containing '" + Args[1] + "' could not be found!");
//                     return;
//                 }
//             }
//
// //        target.AddAllyRequest(fac);
//             int to = Main.GetIntTime() + 60 * 60 * 3;
//             target.AddAllyRequest(fac, Sender, to);//3 Day Time out
        }

        [FactionCommand]
        [Command(Name = "f leave", Description = "Leave the faction that you are currently in")]
        public void FLeave(CorePlayer Sender)
        {
            var fac = getFactionFromPlayer(Sender);
            if (fac == null) return;
            if (!fac.GetLeader().equalsIgnoreCase(Sender.getName()))
                Sender.showFormWindow(new FactionLeaveConfirmWindow(fac));
            else
                Sender.SendMessage(FactionsMain.NAME +
                                   "You are the leader of the faction... Please Do `/f del` if you wish to leave or pass leadership on to someone else!");
        }

        private Faction getFactionFromPlayer(CorePlayer p)
        {
            var fac = p.getFaction();
            if (fac == null) p.SendMessage(ChatColors.Red + "Error! You must be in a faction to use this Command!");

            return fac;
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