package main.java.CyberFactions.Cmds;

import cn.nukkit.Player;
import cn.nukkit.command.Command;
import cn.nukkit.command.CommandSender;
import cn.nukkit.utils.ChatColors;
import main.java.CyberFactions.Faction;
import main.java.CyberFactions.Faction_main;

/**
 * Created by carlt_000 on 7/9/2016.
 */
public class Privacy : Commands {


    public Privacy(CommandSender s, String[] a, Faction_main m){
        base(s,a,"/f privacy <on/off>",m);
        senderMustBeInFaction = true;
        senderMustBeGeneral = true;
        senderMustBePlayer = true;
        sendFailReason = true;
        sendUsageOnFail = true;

        if(run()){
            RunCommand();
        }
    }

    
    private new void RunCommand(){
        if(Args.length == 2) {
            if (Args[1].equalsIgnoreCase("on")) {
                fac.SetPrivacy(1);
                Sender.SendMessage(Faction_main.NAME + ChatColors.GREEN + "Faction Privacy is Now On!");
            } else if (Args[1].equalsIgnoreCase("off")) {
                Sender.SendMessage(Faction_main.NAME + ChatColors.GREEN + "Faction Privacy is Now Off!");
                fac.SetPrivacy(0);
            } else {
                SendUseage();
                return;
            }
        }
    }
}
