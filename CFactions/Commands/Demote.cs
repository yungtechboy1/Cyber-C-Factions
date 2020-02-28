/**
 * Created by carlt_000 on 7/9/2016.
 */

using System;
using Faction2.Commands;
using Faction2.Utils;
using Factions2;
using MiNET;
using MiNET.Utils;

namespace Faction2.Commands
{

public class Demote : Commands {

    public static readonly int RECRUIT = 1;
    public static readonly int MEMBER = 2;
    public static readonly int OFFICER = 3;
    public static readonly int GENERAL = 4;
    public static readonly int LEADER = 5;

    public Demote(CommandSender s, String[] a, Faction_main m) : base(s, a, "/f Demote <player>", m){
        senderMustBeInFaction = true;
        senderMustBeMember = true;
        senderMustBePlayer = true;
        sendFailReason = true;
        sendUsageOnFail = true;

        if (run()) {
            RunCommand();
        }
    }

    
    public new void RunCommand() {
        if(Args.Length < 2){
            Sender.SendMessage(Faction_main.NAME+ChatColors.Red+"Usage /f demote <player>");
            return;
        }
        Player pp = Main.Server.LevelManager.FindPlayer(Args[1]);
        if (pp == null){
            Sender.SendMessage(Faction_main.NAME+ChatColors.Red+"Player Is Not Online!");;
            return;
        }
        String ppn = pp.Username;
        if (Main.GetPlayerFaction(ppn) == null) {
            Sender.SendMessage(Faction_main.NAME+ChatColors.Red + "Target Player Not In Your Faction!");
            return;
        }
        if (Main.GetPlayerFaction(Sender.getName()) == null || !Main.GetPlayerFaction(Sender.GetPlayer().Username).Equals(Main.GetPlayerFaction(ppn))) {
            Sender.SendMessage(Faction_main.NAME+ChatColors.Red + "Player is not in this faction!");
            return;
        }
        //Perms System
        int perm = fac.GetPlayerPerm(Sender.getName());
        int fromperm = fac.GetPlayerPerm(ppn);
        bool SameRankDemote = false;

        if(fromperm == GENERAL && ((SameRankDemote && perm == GENERAL)||perm > GENERAL)){
            fac.AddOfficer(ppn);
            fac.DelGeneral(ppn);
            Sender.SendMessage(Faction_main.NAME+ChatColors.Green+ppn+" Has been demoted to General");
            pp.SendMessage(Faction_main.NAME+ChatColors.Green+"You have been demoted to General");
        }else if(fromperm == OFFICER && ((SameRankDemote && perm == OFFICER)||perm > OFFICER)){
            fac.AddMember(ppn);
            fac.DelOfficer(ppn);
            Sender.SendMessage(Faction_main.NAME+ChatColors.Green+ppn+" Has been demoted to Officer");
            pp.SendMessage(Faction_main.NAME+ChatColors.Green+"You have been demoted to Officer");
        }else if(fromperm == MEMBER && ((SameRankDemote && perm == MEMBER)||perm > MEMBER)){
            fac.AddRecruit(Sender.getName());
            fac.DelMember(Sender.getName());
            Sender.SendMessage(Faction_main.NAME+ChatColors.Green+ppn+" Has been demoted to Member");
            pp.SendMessage(Faction_main.NAME+ChatColors.Green+"You have been demoted to Member");
        }else if(fromperm == RECRUIT){
            Sender.SendMessage(Faction_main.NAME+ChatColors.Red+" You can not Demote a Recruit! That is the lowest rank! You may kick them though!!");
        }else if(fromperm == LEADER){
            Sender.SendMessage(Faction_main.NAME+ChatColors.Red+" You can not Demote the Leader!");
        }else{
            if(perm == RECRUIT){
                Sender.SendMessage(Faction_main.NAME+ChatColors.Red+"You must be Higher than a Recruit to Demote this Person!");
            }else if(perm == MEMBER){
                Sender.SendMessage(Faction_main.NAME+ChatColors.Red+"You must be Higher than a Member to Demote this Person!");
            }else if(perm == OFFICER){
                Sender.SendMessage(Faction_main.NAME+ChatColors.Red+"You must be Higher than a Officer to Demote this Person!");
            }else if(perm == GENERAL){
                Sender.SendMessage(Faction_main.NAME+ChatColors.Red+"You must be Higher than a General to Demote this Person!");
            }
        }
    }
}
}