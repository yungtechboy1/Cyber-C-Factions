package main.java.CyberFactions.Cmds;

import cn.nukkit.Player;
import cn.nukkit.command.Command;
import cn.nukkit.command.CommandSender;
import cn.nukkit.utils.ChatColors;
import main.java.CyberFactions.Faction;
import main.java.CyberFactions.Faction_main;

import java.util.List;

/**
 * Created by carlt_000 on 7/9/2016.
 */
public class Withdraw : Commands {

    public Withdraw(CommandSender s, String[] a, Faction_main m){
        base(s,a,"/f withdraw <amount>",m);
        senderMustBePlayer = true;
        senderMustBeGeneral = true;
        sendUsageOnFail = true;
        senderMustBeInFaction = true;

        if(run()){
            RunCommand();
        }
    }

    
    private new void RunCommand(){
        if(Args.length < 2){
            SendUseage();
            return;
        }
        int money = int.parseInt(Args[1]);
        if(fac.GetMoney() < money){
            Sender.SendMessage(Faction_main.NAME+ChatColors.Red+"Your faction doesn't have $"+money+" Money!");
            return;
        }
        fac.TakeMoney(money);
        Main.Econ.GiveMoney(Sender.getName(),money);
        Sender.SendMessage(Faction_main.NAME+ChatColors.GREEN+"$"+money+" Money taken to your Faction!");
    }
}
