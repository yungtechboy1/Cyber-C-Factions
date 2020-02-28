
using System;
using Faction2;
using Faction2.Commands;
using Faction2.Utils;
using Factions2;
using MiNET;
using MiNET.Utils;

public class Leader : Commands {


    public Leader(CommandSender s, String[] a, Faction_main m) : base (s, a, "/f leader <player>", m){
        senderMustBeInFaction = true;
        senderMustBeLeader = true;
        senderMustBePlayer = true;
        sendFailReason = true;
        sendUsageOnFail = true;

        if (run()) {
            RunCommand();
        }
    }

    
    public new void RunCommand() {
        if(fac.Leader.equalsIgnoreCase(Sender.getName())) {
            if(Args.Length <= 1){
                SendUseage();
                return;
            }
            Player pp = Main.Server.LevelManager.FindPlayer(Args[1]);
            if (pp != null){
                String ppn = pp.Username;
                if(Main.FF.GetPlayerFaction(Sender.GetPlayer()).GetName().equalsIgnoreCase(Main.FF.GetPlayerFaction(ppn).GetName())) {
                    int r = fac.GetPlayerPerm(ppn);
                    if(r == 0)fac.DelRecruit(ppn);
                    if(r == 1)fac.DelMember(ppn);
                    if(r == 2)fac.DelOfficer(ppn);
                    if(r == 3)fac.DelGeneral(ppn);
                    fac.SetLeader(ppn.ToLower());
                    fac.AddMember(Sender.getName());
                    fac.BroadcastMessage(Faction_main.NAME+ChatColors.Yellow+""+ppn+" Is your New Leader!");
                    Sender.SendMessage(Faction_main.NAME+ChatColors.Yellow+"You are no longer leader!");
                    pp.SendMessage(Faction_main.NAME+ChatColors.Yellow+"You are now leader of factionName!");
                } else {
                    Sender.SendMessage(Faction_main.NAME+ChatColors.Red+"Add player to faction first!");
                }
            } else {
                Sender.SendMessage(Faction_main.NAME+ChatColors.Red+"Player Not Online or Found!");
            }
        } else {
            Sender.SendMessage(Faction_main.NAME+ChatColors.Red+"You must be leader to use this");
        }
    }
}