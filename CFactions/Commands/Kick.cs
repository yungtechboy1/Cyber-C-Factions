
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

public class Kick : Commands {

    public static readonly int RECRUIT = 1;
    public static readonly int MEMBER = 2;
    public static readonly int OFFICER = 3;
    public static readonly int GENERAL = 4;
    public static readonly int LEADER = 5;

    public Kick(CommandSender s, String[] a, Faction_main m) : base(s, a, "/f Kick <player>", m){
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
        if (Args.Length < 2) {
            SendUseage();
            return;
        }
        Player pp = Main.Server.LevelManager.FindPlayer(Args[1]);
        if (pp == null) {
            Sender.SendMessage(Faction_main.NAME+ChatColors.Red + "Player Is Not Online or Does Not Exist!");
            return;
        }
        String ppn = pp.Username;
        Faction ofaction = Main.FF.GetPlayerFaction(pp);
        if (ofaction == null) {
            Sender.SendMessage(Faction_main.NAME+ChatColors.Red + "Player Not In Faction!");
            return;
        }
        String fn = fac.GetName();
        if (!ofaction.GetName().ToLower().Equals(fn.ToLower())) {
            Sender.SendMessage(Faction_main.NAME+ChatColors.Red + "Player is not in this faction!");
            return;
        }
        //Perms System
        int perm = fac.GetPlayerPerm(Sender.getName());
        int ppnperm = fac.GetPlayerPerm(ppn);

        if (perm > ppnperm) {
            if (ppnperm == LEADER) {
                Sender.SendMessage(Faction_main.NAME+ChatColors.Red + "You can not kick your leader!");
                return;
            } else if (ppnperm == GENERAL) {
                fac.DelGeneral(ppn);
            } else if (ppnperm == OFFICER) {
                fac.DelOfficer(ppn);
            } else if (ppnperm == MEMBER) {
                fac.DelMember(ppn);
            } else if (ppnperm == RECRUIT) {
                fac.DelRecruit(ppn);
            }
            Sender.SendMessage(Faction_main.NAME+ChatColors.Green + "You successfully kicked " + ppn + "!");
            pp.SendMessage(Faction_main.NAME+ChatColors.Green + "You Have Been Kicked From factionName!!!");
            Main.FF.FacList.Remove(ppn);
            fac.TakePower(2);
            Main.CC.ReloadNameTag(pp);
            Main.sendBossBar(pp);

        } else if (perm == ppnperm) {
            Sender.SendMessage(Faction_main.NAME+ChatColors.Yellow + "You can not kick those who are the same rank as you!");
        } else if (perm < ppnperm) {
            Sender.SendMessage(Faction_main.NAME+ChatColors.Red + "You can not kick those who are a higher rank than you!");
        }
    }
}