package main.java.CyberFactions.Cmds;

import cn.nukkit.Player;
import cn.nukkit.command.Command;
import cn.nukkit.command.CommandSender;
import cn.nukkit.math.Vector3;
import cn.nukkit.utils.ChatColors;
import main.java.CyberFactions.Faction;
import main.java.CyberFactions.Faction_main;

import java.util.Calendar;

/**
 * Created by carlt_000 on 7/9/2016.
 */
public class Power : Commands {


    public Power(CommandSender s, String[] a, Faction_main m) :
        base(s, a, "/f Power", m){
        senderMustBeInFaction = true;
        senderMustBePlayer = true;
        sendFailReason = true;
        sendUsageOnFail = true;

        if (run()) {
            RunCommand();
        }
    }

    
    public new void RunCommand() {
        Sender.SendMessage(Faction_main.NAME+ChatColors.LIGHT_PURPLE + "Your Faction Has " + fac.GetPower());
    }
}