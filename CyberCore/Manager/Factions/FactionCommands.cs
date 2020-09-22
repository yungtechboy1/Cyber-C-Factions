using System.Collections.Generic;
using System.Text.RegularExpressions;
using CyberCore.Manager.Factions.Windows;
using CyberCore.Utils;
using MiNET;
using MiNET.Plugins;
using MiNET.Plugins.Attributes;
using MiNET.UI;
using MiNET.Utils;

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
        public void FCreate(CorePlayer p)
        {
            p.SendForm(new FactionCreate0());
        }

        [Command(Name = "f perm settings", Description = "View your faction's power")]
        [FactionPermission(FactionRankEnum.Recruit)]
        public void fpermsettings(CorePlayer p)
        {
            var fac = p.getFaction();
            var nr = fac.getPermSettings().getAllowedToEditSettings();
            var pr = fac.getPlayerRank(p);
            if (pr.hasPerm(nr))
                p.showFormWindow(new FactionPermSettingsWindow(fac));
            else
                p.SendMessage(FactionErrorString.Error_Settings_No_Permission.getMsg());
        }

        [Command(Name = "f settings", Description = "View your faction's power")]
        [FactionPermission(FactionRankEnum.Recruit)]
        public void fsettings(CorePlayer p)
        {
            var fac = p.getFaction();
            var nr = fac.getPermSettings().getAllowedToEditSettings();
            var pr = fac.getPlayerRank(p);
            if (pr.hasPerm(nr))
                p.showFormWindow(new FactionSettingsWindow(fac));
            else
                p.SendMessage(FactionErrorString.Error_Settings_No_Permission.getMsg());
        }

        [Command(Name = "f power", Description = "View your faction's power")]
        [FactionPermission(FactionRankEnum.Recruit)]
        public string fpower(CorePlayer p)
        {
            var f = p.getFaction();
            return FactionsMain.NAME + ChatColors.LightPurple + "Your Faction Has " + f.GetPower() + " Power!";
        }

        [Command(Name = "f neutral", Description = "Re-set faction relationship back to neutral")]
        [FactionPermission(FactionRankEnum.Recruit)]
        public void fneutral(CorePlayer p, string fac)
        {
            var f = p.getFaction();
            if (f.getPlayerRank(p).hasPerm(f.getPermSettings().AllowedToAcceptAlly))
                p.SendForm(new FactionNeutralWindow(fac));
            else
                p.SendMessage($"{ChatColors.Red} Error! You do not have permission to do this!");
        }

        [Command(Name = "f leave", Description = "Use the command to leave your current faction!")]
        [FactionPermission(FactionRankEnum.Recruit)]
        public void Fleave(CorePlayer Sender)
        {
            Sender.SendForm(new FactionLeaveConfirmWindow(Sender.getFaction()));
        }

        [Command(Name = "faction", Aliases = new[]
        {
            "fac"
        }, Description = "Faction Base Command")]
        public void F(CorePlayer Sender)
        {
            Sender.SendForm(new FactionMainForm(Sender));
        }

        [FactionPermission(FactionRankEnum.Recruit)]
        [Command(Name = "f invites", Aliases = new[]
        {
            "f deny",
            "f accept"
        }, Description = "View/Accept/Deny Faction Invites (Must not currently be in a faction)")]
        public void finvites(CorePlayer Sender)
        {
            var fac = Sender.getFaction();
            if (fac.getPlayerRank(Sender).hasPerm(fac.getPermSettings().AllowedToInvite))
                Sender.showFormWindow(new FactionInvitesWindow(Sender));
        }


        [Command(Name = "f leader",
            Description =
                "Transfer leadership of faction to another player in your faction (Target Player Must Be Online)")]
        [FactionPermission(FactionRankEnum.Leader)]
        public void fleader(CorePlayer p, Target t)
        {
            var currentleader = p;
            var newleader = t.getPlayer();
            var fac1 = currentleader.getFaction();
            var fac2 = newleader.getFaction();
            if (!fac1.getName().equalsIgnoreCase(fac2.getName()))
            {
                currentleader.SendMessage(ChatColors.Red + "Error! You MUST BE IN THE SAME FACTION!");
                return;
            }

            p.SendForm(new FactionChangeLeaderWindow(p, (CorePlayer) t.getPlayer()));
        }

        [Command(Name = "f join", Description = "View Open Factions and Join One")]
        public void fjoin(CorePlayer p, string faction = null)
        {
            if (faction == null)
            {
                p.SendForm(new FactionJoinListWindow());
            }
            else
            {
                var fl = Manager.factionPartialNameList(faction);
                if (fl.Count == 0)
                {
                    p.SendMessage($"{ChatColors.Red} Error! No factions found with the name '{faction}'");
                }
                else if (fl.Count == 1)
                {
                    var tf = Manager.getFaction(fl[0]);
                    if (!tf.canJoin())
                    {
                        p.SendMessage(
                            $"{ChatColors.Red} Error! The faction {tf.getDisplayName()}{ChatColors.Red} is a private faction or full! You can not join, you must be invited by someone within the faction!");
                        return;
                    }

                    p.SendForm(new FactionJoinConfirm(0, tf));
                }
                else
                {
                    p.SendForm(new FactionJoinListWindow(fl));
                }
            }
        }

        [Command(Name = "f invite", Description = "Create a new faction")]
        [FactionPermission(FactionRankEnum.Recruit)]
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

        [Command(Name = "f home", Description = "View All Faction Homes")]
        [FactionCommand]
        public void fhome(CorePlayer Sender, int page = 1)
        {
            Sender.SendForm(new FactionHomesPage(Sender));
        }


        [Command(Name = "f delhome", Description = "Set a Faction Home")]
        [FactionCommand]
        public void fdelhome(CorePlayer Sender, string name)
        {
            var f = Sender.getFaction();
            var a = f.GetHome();
            var v = a[name];
            if (v == null)
            {
                Sender.SendMessage($"{ChatColors.Red} Error! The home '{name}' Could not be found!");
                return;
            }

            var perm = f.getPermSettings().AllowedToSetHome;
            if (!f.getPlayerRank(Sender).hasPerm(perm))
            {
                Sender.SendMessage(
                    $"{ChatColors.Red} Error! You do not have permission to set homes for your faction!");
                return;
            }

            f.DelHome(v.HomeID);
        }


        [Command(Name = "f sethome", Description = "Set a Faction Home")]
        [FactionCommand]
        public void fsethome(CorePlayer Sender, string name)
        {
            var f = Sender.getFaction();
            var a = f.GetHome();
            var max = f.getSettings().getMaxHomes();
            if (a.Count >= max)
            {
                Sender.SendMessage(
                    $"{ChatColors.Red} Error! You faction has set the max amount of homes allowed already!");
                return;
            }

            var perm = f.getPermSettings().AllowedToSetHome;
            if (!f.getPlayerRank(Sender).hasPerm(perm))
            {
                Sender.SendMessage(
                    $"{ChatColors.Red} Error! You do not have permission to set homes for your faction!");
                return;
            }

            f.addHome(new Faction.HomeData(Sender, name));
        }

        [Command(Name = "f help", Description = "Get Help with all Faction Commands")]
        [FactionCommand]
        public string fhelp(CorePlayer Sender, int page = 1)
        {
            var a = new List<string>();
            a.Add("/f accept - Accept Faction Invite");
            a.Add("/f admin - OP Only");
            a.Add("/f balance - Faction Balance");
            a.Add("/f chat [Message] | /f c [Message] - Send message to faction only");
            a.Add("/f claim [radius] - Claim Land");
            a.Add("/f create <name> - Create a Faction");
            a.Add("/f del - Delete Faction");
            a.Add("/f demote <player> - Demote player in faction");
            a.Add("/f deny - Deny faction invite");
            a.Add("/f deposit <amount> - Add money to faction ballance");
            a.Add("/f desc [Description] - Set description for Faction");
            a.Add("/f help [page] - View All Commands");
            a.Add("/f home - Teleport to faction home");
            a.Add("/f info <faction> - View faction's info");
            a.Add("/f invite <player> - Invite player to join your faction");
            a.Add("/f join <faction> - Join an open faction");
            a.Add("/f kick <player> - Kick player from faction");
            a.Add("/f kits - Coming Soons");
            a.Add("/f leader <player> - Transfer leadership to another player");
            a.Add("/f leave [Leave message]- Leave faction");
            a.Add("/f leader <player> - Give another player leadership of faction");
            a.Add("/f list [page] - List all factions");
            a.Add("/f map - Show map of area");
            a.Add("/f mission - Show all mission commands");
            a.Add("/f motd <InternalPlayerSettings> - Set faction MOTD ");
            a.Add("/f overclaim [radius] - Overclaim land ");
            a.Add("/f perk - View All Faction Perks ");
            a.Add("/f power - View faction's power");
            a.Add("/f privacy - Change faction privacy between Open and Closed");
            a.Add("/f Promote <player> - Promote a player");
            a.Add("/f sethome - Set faction home");
            a.Add("/f unclaim [radius] - Unclaim faction chunks");
            a.Add("/f war <faction> - Declare War against faction");
            a.Add("/f wartp - Teleport to the war zone");
            a.Add("/f withdraw - Take money from faction's balance");

            var p = page;
            var to = p * 5;
            var from = to - 5;
            // 5 -> 0 ||| 10 -> 5
            var x = 0;
            var t = "";

            t += ChatColors.Gray + "-----" + ChatColors.Gold + ".<[Faction Command List]>." + ChatColors.Gray +
                 "-----\n";
            foreach (var value in a)
            {
                // 0 < 5 && 0 >= 0
                //   YES     YES
                //
                //0
                //1 2 3 4 5
                //0 < 10 && 0 >= 5
                if (!(x < to && x >= from))
                {
                    x++;
                    continue;
                }

                if (x > to) break;
                x++;
                t += value + "\n";
            }

            t += "------------------------------";
            return t;
        }

        [Command(Name = "f kick", Description = "Deposit Money into Faction")]
        [FactionCommand]
        public void fkick(CorePlayer Sender, Target t = null)
        {
            var f = Sender.getFaction();
            var perm = f.getPlayerRank(Sender);
            if (!perm.hasPerm(f.getPermSettings().AllowedToKick))
            {
                Sender.SendMessage($"{ChatColors.Red}Error! You do not have permission to kick a player!");
                return;
            }

            if (t != null && t.getPlayer() != null)
            {
                var p = t.getPlayer();
                var tp = f.getPlayerRank(p);
                if (!perm.hasPerm(tp))
                {
                    Sender.SendMessage(
                        $"{ChatColors.Red}Error! You can not kick a higher ranked player! Your Rank: {perm.toEnum()} {p.Username}'s Rank: {tp.toEnum()}");
                    return;
                }

                f.KickPlayer(p);
            }
            else
            {
                Sender.SendForm(new FactionKickListWindow(Sender));
            }
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

            fac.getSettings().addMoney(money);
            Sender.SendMessage(FactionsMain.NAME + ChatColors.Green + "$" + money + " Money Added to your Faction!");
            fac.BroadcastMessage(FactionsMain.NAME + ChatColors.Green + Sender.getName() + " has deposited $" + money +
                                 " Money to the faction account!");
        }

        [Command(Name = "f withdraw", Description = "Deposit Money into Faction")]
        [FactionCommand]
        public void fwithdraw(CorePlayer Sender, int money)
        {
            var fac = Sender.getFaction();
            if (money == null || money == 0)
            {
                Sender.SendMessage(FactionsMain.NAME + ChatColors.Gray + "Usage /f withdraw <amount>");
                return;
            }

            if (!Sender.MakeTransaction(money))
            {
                Sender.SendMessage(FactionsMain.NAME + ChatColors.Red + "You don't have " + money + " Money!");
                return;
            }

            Sender.SendMessage(FactionsMain.NAME + ChatColors.Green + "$" + money +
                               " Money Withdrawn from your Faction!");
            fac.BroadcastMessage(FactionsMain.NAME + ChatColors.Green + Sender.getName() + " has Withdrawn $" + money +
                                 " Money to the faction account!");
        }

        [Command(Name = "f demote", Description = "Demote Player in Faction")]
        [FactionCommand]
        public void fdemote(CorePlayer Sender, Target player = null)
        {
            var fac = Sender.getFaction();
            var pp = (CorePlayer) player.getPlayer();
            if (pp == null)
            {
                Sender.SendForm(new FactionPromoteDemoteWindow(false, Sender));
                return;
            }

            var pf = pp.getFaction();
            if (pf == null || !pf.getName().equalsIgnoreCase(fac.getName()))
            {
                Sender.SendMessage(FactionsMain.NAME + ChatColors.Red + "Target Player Not In Your Faction!");
                return;
            }

            var r = fac.getPlayerRank(Sender);
            var fr = fac.getPermSettings().getAllowedToPromote();
            if (r.hasPerm(fr)) fac.DemotePlayer(pp);
        }

        [Command(Name = "f promote", Description = "Demote Player in Faction")]
        [FactionCommand]
        public void fpromote(CorePlayer Sender, Target player = null)
        {
            var fac = Sender.getFaction();
            var pp = (CorePlayer) player.getPlayer();
            if (pp == null)
            {
                Sender.SendForm(new FactionPromoteDemoteWindow(true, Sender));
                return;
            }

            var pf = pp.getFaction();
            if (pf == null || !pf.getName().equalsIgnoreCase(fac.getName()))
            {
                Sender.SendMessage(FactionsMain.NAME + ChatColors.Red + "Target Player Not In Your Faction!");
                return;
            }

            var r = fac.getPlayerRank(Sender);
            var fr = fac.getPermSettings().getAllowedToPromote();
            if (r.hasPerm(fr)) fac.PromotePlayer(pp);
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
        //[ServerRankAttr(RankEnum.JrServerAdmin)]
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
        }


        private Faction getFactionFromPlayer(CorePlayer p)
        {
            var fac = p.getFaction();
            if (fac == null) p.SendMessage(ChatColors.Red + "Error! You must be in a faction to use this Command!");

            return fac;
        }

        [Command(Name = "f list", Description = "Open Faction List")]
        public void fList(CorePlayer Sender, string faction = null)
        {
            if (faction == null)
                Sender.SendForm(new FactionListWindow(Sender));
            else Sender.SendMessage($"{ChatColors.Yellow} /f list <Name> is not released yet!");
        }

        [Command(Name = "f info", Description = "Usage '/f info [faction]' | View your Faction Info or Others")]
        public void FInfo(CorePlayer Sender, string faction = null)
        {
            Sender.SendForm(new FactionListWindow(Sender, faction));
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