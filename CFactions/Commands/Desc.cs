
using System;
using Faction2.Commands;
using Faction2.Utils;
using Factions2;
using MiNET.Utils;

public class Desc : Commands {

    public Desc(CommandSender s, String[] a, Faction_main m) : base(s,a,"/f desc <Description>",m){
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
            Sender.SendMessage(Faction_main.NAME+ChatColors.Gray+"Usage /f desc <Description>");
            return;
        }
        String desc = GetStringAtArgs(1,"A ArchMCPE Faction!");
        fac.SetDesc(desc);
        Sender.SendMessage(Faction_main.NAME+ChatColors.Gray+" Faction description changed!");
    }
}
