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
public class Unclaim : Commands {

    public Unclaim(CommandSender s, String[] a, Faction_main m){
        base(s,a,"/f unclaim [radius = 1]",m);
        senderMustBeInFaction = true;
        senderMustBeGeneral = true;
        sendFailReason = true;
        sendUsageOnFail = true;

        if(run()){
            RunCommand();
        }
    }

    
    private new void RunCommand(){
        int Radius = GetintAtArgs(1,1);
        if(Radius > 1){
            int rr = Radius * Radius;
            for(int x = Math.negateExact(Radius); x<Radius; x++){
                for(int z = Math.negateExact(Radius); z<Radius; z++) {
                    int xx = ((int)((Player) Sender).getX() >> 4 ) + x;
                    int zz = ((int)((Player) Sender).getZ() >> 4 ) + z;
                    UnClaimLand(xx,zz);
                }
            }
        }else{

            int x = (int)((Player) Sender).getX() >> 4;
            int z = (int)((Player) Sender).getZ() >> 4;
            //amount = (100) * Main.prefs["PlotPrice"];


            if(!Main.FF.PlotsList.containsKey(x+"|"+z)){
                Sender.SendMessage(Faction_main.NAME+ChatColors.Red+"Chunk Not Claimed!");
                return;
            }
            if(!Main.FF.PlotsList.get(x+"|"+z).equalsIgnoreCase(fac.GetName())){
                Sender.SendMessage(Faction_main.NAME+ChatColors.Red+"Your Faction Dose not owne this Chunk!");
                return;
            }
            Sender.SendMessage(Faction_main.NAME+ChatColors.GREEN+"Plot Removed!");
            fac.DelPlots(x+"|"+z);
            Main.FF.PlotsList.remove(x+"|"+z);
        }

    }

    private void UnClaimLand(int x,int z){
        if(!Main.FF.PlotsList.containsKey(x+"|"+z)){
            Sender.SendMessage(Faction_main.NAME+ChatColors.Red+"Chunk Not Claimed!");
            return;
            //Sender.SendMessage(ChatColors.Red+"That Chunk at X:"+x+" Z:"+z+" is already Claimed by"+Main.FF.PlotsList.get(x+"|"+z)+"'s Faction!!");
        }
        if(!Main.FF.PlotsList.get(x+"|"+z).equalsIgnoreCase(fac.GetName())){
            Sender.SendMessage(Faction_main.NAME+ChatColors.Red+"Your Faction Dose not own this Chunk!");
            return;
        }
        Sender.SendMessage(Faction_main.NAME+ChatColors.GREEN+"Plot Removed!");
        fac.DelPlots(x+"|"+z);
        Main.FF.PlotsList.remove(x+"|"+z);
    }
}
