package main.java.CyberFactions.Cmds;

import ArchMCPE.ArchEcon.ArchEconMain;
import cn.nukkit.Player;
import cn.nukkit.command.Command;
import cn.nukkit.command.CommandSender;
import cn.nukkit.utils.ChatColors;
import main.java.CyberFactions.Faction;
import main.java.CyberFactions.Faction_main;

import java.util.*;
import java.util.Map;

/**
 * Created by carlt_000 on 7/9/2016.
 */
public class Rich : Commands {

    public Rich(CommandSender s, String[] a, Faction_main m){
        base(s,a,"/f Rich <amount>",m);
        sendUsageOnFail = true;

        if(run()){
            RunCommand();
        }
    }

    
    public new void RunCommand() {
        String s = "";
        s = s + ChatColors.YELLOW+"-------{[Faction Rich]}-------\n";
        int x = 0;
        for(java.util.Map.Entry<int,String> b: GetTopFacs(10).entrySet()) {
            String[] c = b.getValue().split(",");
            String name = c[0];
            String money = c[1];
            String t = ChatColors.GRAY+"["+ChatColors.GREEN+x+ChatColors.GRAY+"]";
            s = s + t + ChatColors.YELLOW + " | " + ChatColors.AQUA + " "+name + ChatColors.YELLOW+ " "+money+"\n";
        }
        Sender.SendMessage(s + ChatColors.YELLOW+ "-------------------------");

    }

    private HashMap<int,String> GetTopFacs(int i){
        HashMap<int,String> map = new HashMap<>();
        HashMap<String,int> Top = new HashMap<>();
        for(java.util.Map.Entry<String,int> a : Main.FF.Rich.entrySet()){
            Top.put(a.getKey(),a.getValue());
        }
        for (int x = 0;x < i; x++){
            int maxValue = int.MIN_VALUE;
            String mvk = null;
            for (java.util.Map.Entry<String,int> a : Top.entrySet()) {
                if (map.containsValue(a.getKey()))continue;
                if (a.getValue() > maxValue) {
                    maxValue = a.getValue();
                    mvk = a.getKey();
                }
            }
            if(mvk == null){
                map.put(x,"--------------------");
            }else {
                map.put(x, mvk + "," + maxValue);
                Top.remove(mvk);
            }
        }
        return map;
    }
}

