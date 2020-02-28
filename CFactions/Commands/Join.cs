/**
 * Created by carlt_000 on 7/9/2016.
 */

using System;
using Faction2;
using Faction2.Commands;
using Faction2.Utils;
using Factions2;
using MiNET;
using MiNET.Utils;

public class Join :  Commands {

    public Join(CommandSender s, String[] a, Faction_main m) : base(s,a,"/f join <faction>",m){
        senderMustBePlayer = true;
        senderMustBeMember = true;
        sendUsageOnFail = true;

        if(run()){
            RunCommand();
        }
    }

    
    //@TODO
    private new void RunCommand(){
        if(Args.Length <= 1){
            Sender.SendMessage(Faction_main.NAME+ChatColors.Gray+"Usage /f join <faction>");
            return;
        }
        Faction f = Main.FF.factionPartialName(Args[1]);
        if(f == null){
            Sender.SendMessage(Faction_main.NAME+ChatColors.Red+"Faction not Found!");
            return;
        }
        if(f.GetPrivacy() == 1){
            Sender.SendMessage(Faction_main.NAME+ChatColors.Red+"Error! You can not join a private Faction!");
            return;
        }
        f.AddRecruit(Sender.getName().ToLower());
        Main.FF.FacList.Add(Sender.getName().ToLower(), f.GetName());

        Sender.SendMessage(Faction_main.NAME+ChatColors.Red+"Congrads! You Just Joined "+fac.GetDisplayName()+" Faction!");
        f.BroadcastMessage(Faction_main.NAME+ChatColors.Green+Sender.getName()+" Has joinded your faction!");
        Main.CC.ReloadNameTag(Sender.GetPlayer());
    }
}
