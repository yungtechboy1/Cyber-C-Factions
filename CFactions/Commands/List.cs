
/**
 * Created by carlt_000 on 7/9/2016.
 */

using System;
using System.Collections.Generic;
using Faction2;
using Faction2.Commands;
using Faction2.Utils;
using Factions2;
using MiNET.Utils;

public class List : Commands {

    public List(CommandSender s, String[] a, Faction_main m) : base(s, a, "/f List [Page]", m){
        sendFailReason = true;
        sendUsageOnFail = true;

        if (run()) {
            RunCommand();
        }
    }

    
    public new void RunCommand() {
        int p = GetintAtArgs(1,1);
        int to = p * 5;
        int from = to - 5;
        // 5 -> 0 ||| 10 -> 5
        int x = 0;

        String t = "";
        String a = "";

        t += ChatColors.Gray+"-------------"+ChatColors.Gold+".<[List]>."+ChatColors.Gray+"-------------\n";
        t += ChatColors.Gray+"------------Land-Pwr-Max---------\n";
        foreach(KeyValuePair<string,Faction> e in Main.FF.List){
            Faction f = e.Value;
            a = "";
            // 0 < 5 && 0 >= 0
            //   YES     NO
            //0
            //1 2 3 4 5
            //0 < 10 && 0 >= 5
            if(!(x < to && x >= from)){
                x++;
                continue;
            }
            if(x > to)break;

            x++;
            //Privacy
            a += ChatColors.DarkGreen+""+x+" > ";
            if (f.GetPrivacy() == 1) {
                a += ChatColors.Red + "[C]";
            } else {
                a += ChatColors.Green + "[O]";
            }
            a += ChatColors.White + "---[ ";
            a += ChatColors.DarkAqua+"" + f.GetPlots().Count;
            a += ChatColors.White + " / ";
            a += ChatColors.DarkAqua +""+ f.GetPower();
            a += ChatColors.White + " / ";
            a += ChatColors.DarkAqua+"" + f.CalculateMaxPower();
            a += ChatColors.White + " ]--- " + ChatColors.Yellow + f.GetDisplayName();
            a += ChatColors.Gray + "[" + f.GetNumberOfPlayers() + "]";
            t += a + "\n";

        }
        t += "------------------------------";
        Sender.SendMessage(t);
    }
}