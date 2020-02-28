
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

public class Leave : Commands {

    public Leave(CommandSender s, String[] a, Faction_main m) : base(s, a, "/f leave [Message]", m){
        senderMustBeInFaction = true;
        senderMustBePlayer = true;
        sendFailReason = true;
        sendUsageOnFail = true;

        if (run()) {
            RunCommand();
        }
    }

    
    public new void RunCommand() {
        if (!fac.Leader.equalsIgnoreCase(Sender.getName())) {

            if(fac.IsMember( Sender.getName()))fac.DelMember(Sender.getName());
            if(fac.IsOfficer( Sender.getName()))fac.DelOfficer(Sender.getName());
            if(fac.IsGeneral( Sender.getName()))fac.DelGeneral(Sender.getName());
            if(fac.IsRecruit( Sender.getName()))fac.DelRecruit(Sender.getName());

            Sender.SendMessage(Faction_main.NAME+ChatColors.Green + "You successfully left faction");
            fac.TakePower(1);
            String lm = "";
            if(Args.Length >= 1){
                lm = "And has left with the following Message:"+ChatColors.Aqua;
                foreach(String a in Args){
                    lm = lm + " " + a;
                }
            }
            fac.BroadcastMessage(Faction_main.NAME+ChatColors.Yellow+Sender.getName()+" has Left the Faction!"+lm);
            if(Main.CC != null)Main.CC.ReloadNameTag(Sender.GetPlayer());
            Main.sendBossBar(Sender.GetPlayer());
            Main.FF.FacList.Remove(Sender.getName().ToLower());
        } else {
            Sender.SendMessage(Faction_main.NAME+"You are the leader of the faction... Please Do `/f del` if you wish to leave or pass leadership on to someone else!");
        }
    }
}