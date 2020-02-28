
/**
 * Created by carlt_000 on 7/9/2016.
 */

using System;
using Faction2.Commands;
using Faction2.Utils;
using Factions2;

public class Kits : Commands {

    public Kits(CommandSender s, String[] a, Faction_main m): base(s,a,"/f kist ",m){
        senderMustBePlayer = true;
        senderMustBeMember = true;
        sendUsageOnFail = true;

        if(run()){
            RunCommand();
        }
    }

    //@TODO
    
    private new void RunCommand(){
        Sender.SendMessage("Comming Soon!");
        return;
    }
}
