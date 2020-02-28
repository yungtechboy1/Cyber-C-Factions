using System;
using Faction2.Utils;
using Factions2;
using MiNET.Utils;

namespace Faction2.Commands
{

/**
 * Created by carlt_000 on 7/9/2016.
 */
public class Delete : Commands {

    public Delete(CommandSender s, String[] a, Faction_main m) : base(s, a, "/f delete", m){
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
        if (fac.Leader.Equals(Sender.getName(), StringComparison.OrdinalIgnoreCase)) {
            Sender.SendMessage(Faction_main.NAME+ChatColors.Green + "Faction Deleted!");
            fac.BroadcastMessage(Faction_main.NAME+ChatColors.Yellow+"!!~~!!Faction has been Deleted by "+Sender.getName());
            Main.FF.RemoveFaction(fac);
        } else {
            Sender.SendMessage(Faction_main.NAME+ChatColors.Red + "You are not the leader!");
        }
    }
}
}