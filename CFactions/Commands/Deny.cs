
using System;
using Faction2.Commands;
using Faction2.Utils;
using Factions2;
using MiNET.Utils;

public class Deny : Commands {

    public Deny(CommandSender s, String[] a, Faction_main m) : base(s, a, "/f Deny", m){
        senderMustBePlayer = true;
        sendFailReason = true;
        sendUsageOnFail = true;

        if (run()) {
            RunCommand();
        }
    }

    
    public new void RunCommand() {
        String player = Sender.getName();
        String lowercaseName = player;

        if(Main.FF.InvList.ContainsKey(Sender.getName().ToLower())){
            fac.DenyInvite(Sender.getName().ToLower());
            Sender.SendMessage(Faction_main.NAME+ChatColors.Yellow + "Faction Invite Denied!");
            fac.BroadcastMessage(Faction_main.NAME+ChatColors.Yellow+Sender.getName()+" Has denied to join your faction!");
        }
    }
}