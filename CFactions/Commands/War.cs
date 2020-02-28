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
public class War : Commands {

    public War(CommandSender s, String[] a, Faction_main m){
        base(s,a,"/f war <Faction>",m);
        senderMustBeInFaction = true;
        senderMustBeGeneral = true;
        sendFailReason = true;
        sendUsageOnFail = true;
        if(run()){
            RunCommand();
        }
    }

    
    private new void RunCommand(){
        if (!(Sender instanceof Player)) {
            return;
        }
        Faction target = GetFactionAtArgs(1);
        if(target == null){
            Sender.SendMessage(Faction_main.NAME+"Error the faction containing '"+GetStringAtArgs(1)+"' could not be found!");
        }else{
            if (target.AtWar()){
                Sender.SendMessage(Faction_main.NAME+ChatColors.Red+"Error, That target faction is currently at war!");
                return;
            }
            if (fac.AtWar()){
                Sender.SendMessage(Faction_main.NAME+ChatColors.Red+"Error, you are already in a war!");
                return;
            }
            if (target.HasWarCooldown()){
                Sender.SendMessage(Faction_main.NAME+ChatColors.Red+"Error, that faction has a war cooldown and cannot bet attacked right now... Try again later!");
                return;
            }

            Float tp = (float) target.GetMaxPower();
            Float fp = (float) fac.GetMaxPower();
            Float ratio = tp / fp;//40/50 = 4/5th = .8
            if(.5f > ratio){
                Sender.SendMessage(Faction_main.NAME+ChatColors.Red+"Error, That target faction is not powerful enough for your faction to attack. The Power Ration is "+ratio+" and it must be above .5 for you to attack!");
                return;
            }else if(ratio > 2f){
                Sender.SendMessage(Faction_main.NAME+ChatColors.Red+"Error, That target faction is too powerful for your faction to attack. The Power Ration is "+ratio+" and must be below 2 for you to attack!");
                return;
            }

            //ENEMY 40/100 FRIENDLY 50/70
            //      .40             .714
            //MIN 16 Power for Attack
            // ROUND(50*(((40/100)+1)/2))
            // ROUND(50*(((.4)+1)/2))
            // ROUND(50*((1.4)/2))
            // ROUND(50*( .7 ))
            // ROUND(50*( .7 ))
            // ROUND( 35 )


            int takepower = Math.round(fac.GetPower()*(((target.GetPower()/target.GetMaxPower())+1)/2));
            if (takepower < Math.floor(target.GetPower()*.4))takepower = (int) Math.floor(target.GetPower()*.2);

            if (fac.GetPower() < takepower){
                Sender.SendMessage(Faction_main.NAME+ChatColors.Red+"Error, Your faction does not have "+takepower+" Power needed for this attack!");
                return;
            }

            fac.TakePower(takepower);
            int time = (int)(Calendar.getInstance().getTime().getTime()/1000);
            Main.DeclareWar(target,fac);




        }

    }
}
