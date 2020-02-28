
using System;
using System.Globalization;
using Faction2;
using Faction2.Commands;
using Faction2.Utils;
using Factions2;
using MiNET;
using MiNET.Utils;

public class Invite : Commands {


    public Invite(CommandSender s, String[] a, Faction_main m) : base (s, a, "/f Invite <player>", m){
        senderMustBeInFaction = true;
        senderMustBeMember = true;
        senderMustBePlayer = true;
        sendFailReason = true;
        sendUsageOnFail = true;

        if (run()) {
            RunCommand();
        }
    }

    
    public new void RunCommand() {
        if(Args.Length != 2) {
            SendUseage();
            return;
        }
        if(fac == null) {
            Sender.SendMessage(Faction_main.NAME+ChatColors.Red+"Your Not in a faction!");
            return;
        }
        if(fac.GetNumberOfPlayers() >= fac.GetMaxPlayers()) {
            Sender.SendMessage(Faction_main.NAME+ChatColors.Red+"Faction is full. Please kick players to make room.\n");//+ChatColors.Red+"Or pay to upgrade your faction limit!");
            return;
        }

        Player invited = Main.Server.LevelManager.FindPlayer(Args[1]);
        if(invited == null) {
            Sender.SendMessage(Faction_main.NAME+ChatColors.Red+"No Player By That Name Is Online!");
            return;
        }
        if(null == Main.FF.GetPlayerFaction(invited)) {
            Sender.SendMessage(Faction_main.NAME+ChatColors.Red+"Player is currently in a faction");
            return;
        }
        //PERMS
        /*int perm = fac.GetPerm(4);
        if(perm < fac.GetPlayerPerm(Sender.getName())){
            if(perm == 1)Sender.SendMessage(ChatColors.Red+"Only Members and above may invite!");
            if(perm == 2)Sender.SendMessage(ChatColors.Red+"Only Officers and above may invite!");
            if(perm == 3)Sender.SendMessage(ChatColors.Red+"Only Generals and above may invite!");
            if(perm == 4)Sender.SendMessage(ChatColors.Red+"Only your Leader may invite!");
            return;
        }*/

        long time = DateTime.Now.Ticks;
        fac.AddInvite(invited.Username.ToLower(),time);
        Main.FF.InvList.Add(invited.Username.ToLower(),fac.GetName());

        Sender.SendMessage(Faction_main.NAME+ChatColors.Green+"Successfully invited "+invited.Username+"!");
        invited.SendMessage(Faction_main.NAME+ChatColors.Yellow+"You have been invited to faction.\n"+ChatColors.Green+"Type '/f accept' or '/f deny' into chat to accept or deny!");
    }
}