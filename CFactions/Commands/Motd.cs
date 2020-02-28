using System;
using Faction2.Commands;
using Faction2.Utils;
using Factions2;
using MiNET.Utils;

public class Motd : Commands {

    public Motd(CommandSender s, String[] a, Faction_main m) : base(s,a,"/f motd <Description>",m){
        senderMustBePlayer = true;
        senderMustBeOfficer = true;
        sendUsageOnFail = true;

        if(run()){
            RunCommand();
        }
    }

    
    private new void RunCommand(){
        //@todo
        if(Args.Length < 2){
            SendUseage();
            return;
        }
        String desc = GetStringAtArgs(1,"A ArchMCPE Faction!");
        fac.SetMOTD(desc);
        Sender.SendMessage(Faction_main.NAME+ChatColors.GREEN+" Faction MOTD changed!");
    }
}
