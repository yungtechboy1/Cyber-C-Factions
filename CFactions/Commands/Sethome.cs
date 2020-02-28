package main.java.CyberFactions.Cmds;

import cn.nukkit.Player;
import cn.nukkit.command.Command;
import cn.nukkit.command.CommandSender;
import cn.nukkit.utils.ChatColors;
import main.java.CyberFactions.Faction;
import main.java.CyberFactions.Faction_main;

import java.util.Calendar;

/**
 * Created by carlt_000 on 7/9/2016.
 */
public class Sethome : Commands {


    public Sethome(CommandSender s, String[] a, Faction_main m) :
        base(s, a, "/f sethome <player>", m){
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
        if (!fac.Leader.equalsIgnoreCase(Sender.getName()) && !Main.isOfficer(Sender.getName())) {
            Sender.SendMessage(Faction_main.NAME+ChatColors.Red + "You Must Be a Leader or Officer!");
            return;
        }
        int fp = fac.GetPower();
        if (fp < 1) {
            Sender.SendMessage(Faction_main.NAME+ChatColors.Red + "You Must Have 1 Faction Power!");
            return;
        }
        double x = ((Player) Sender).getX();
        double z = ((Player) Sender).getZ();
        //@todo Should they own the Chunk???
        String co = Main.GetChunkOwner((int) x >> 4, (int) z >> 4);


        if (co != null && co.equalsIgnoreCase(fac.GetName())) {
            fac.SetHome((Player) Sender);
            fac.TakePower(1);
            Sender.SendMessage(Faction_main.NAME+ChatColors.GREEN + "Home updated!");
        } else {
            Sender.SendMessage(Faction_main.NAME+ChatColors.Red + "You must claim the land to set a home there!!!");
            return;
        }
    }
}