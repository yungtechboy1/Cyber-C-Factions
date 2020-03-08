using System;
using System.Collections;
using Faction2.Commands;
using Faction2.Utils;
using Factions2;
using MiNET.Utils;

public class Help : Commands {

    public Help(CommandSender s, String[] a, Faction_main m) : base(s, a, "/f help [page]", m){
        senderMustBePlayer = true;
        sendFailReason = true;
        sendUsageOnFail = true;

        if (run()) {
            RunCommand();
        }
    }

    
    public new void RunCommand() {
        List a = new List();
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
        a.Add("/f motd <Motd> - Set faction MOTD ");
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

        int p = GetintAtArgs(1,1);
        int to = p * 5;
        int from = to - 5;
        // 5 -> 0 ||| 10 -> 5
        int x = 0;
        String t = "";

        t += ChatColors.Gray+"-----"+ChatColors.Gold+".<[Faction Command List]>."+ChatColors.Gray+"-----\n";
        foreach(String value in a){
            // 0 < 5 && 0 >= 0
            //   YES     YES
            //
            //0
            //1 2 3 4 5
            //0 < 10 && 0 >= 5
            if(!(x < to && x >= from)){
                x++;
                continue;
            }
            if(x > to)break;
            x++;
            t += value + "\n";

        }
        t += "------------------------------";
        Sender.SendMessage(t);
    }
}