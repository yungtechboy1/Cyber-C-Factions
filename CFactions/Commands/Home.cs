using System;
using System.Numerics;
using Faction2;
using Faction2.Commands;
using Faction2.Utils;
using Factions2;
using MiNET;
using MiNET.Utils;

public class Home : Commands {


    public Home(CommandSender s, String[] a, Faction_main m) : base(s, a, "/f Home [faction]", m){
        senderMustBeInFaction = true;
        senderMustBePlayer = true;
        sendFailReason = true;
        sendUsageOnFail = true;

        if (run()) {
            RunCommand();
        }
    }

    
    public new void RunCommand() {
        if(Args.Length == 2){
            Faction ofaction = Main.FF.factionPartialName(Args[1]);
            if(ofaction == null){
                Sender.SendMessage(Faction_main.NAME+ChatColors.Red+"No Faction Found By That Name!");
                return;
            }
            if(!fac.isAllied(ofaction)){
                Sender.SendMessage(Faction_main.NAME+ChatColors.Red+"You Are Not Allied With that Faction!!!");
                return;
            }
            PlayerLocation home = ofaction.GetHome();
            if(home.Y == 0){
                Sender.SendMessage(Faction_main.NAME+ChatColors.Gold+""+ofaction+"'s does not have a Home is not set!");
                return;
            }
            Sender.GetPlayer().Teleport(home);
            Sender.SendMessage(Faction_main.NAME+ChatColors.Green+"Teleported to "+ofaction.GetName()+"'s home!");
            return;
        }else{
            if(fac == null){
                Sender.SendMessage(Faction_main.NAME+ChatColors.Red+"Your Not In a Faction!");
                return;
            }
            PlayerLocation home = fac.GetHome();
            if(home.Y == 0){
                Sender.SendMessage(Faction_main.NAME+ChatColors.Gold+"Your faction dose not have a Home is not set!");
                return;
            }
            Sender.GetPlayer().Teleport(home);
            Sender.SendMessage(Faction_main.NAME+ChatColors.Green+"Teleported to faction home!");
            return;
        }
    }
}