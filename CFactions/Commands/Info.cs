using System;
using System.Text.RegularExpressions;
using Faction2;
using Faction2.Commands;
using Faction2.Utils;
using Factions2;
using MiNET.Utils;

public class Info :  Commands {

    public static readonly int RECRUIT = 1;
    public static readonly int MEMBER = 2;
    public static readonly int OFFICER = 3;
    public static readonly int GENERAL = 4;
    public static readonly int LEADER = 5;
    public Info(CommandSender s, String[] a, Faction_main m) : base(s, a, "/f info <player>", m){
        senderMustBeInFaction = true;
        senderMustBePlayer = true;
        sendUsageOnFail = true;
        if (run()) {
            RunCommand();
        }
    }

    
    public new void RunCommand() {
        if (Args.Length > 1) {
            //TODO Reginx
            /*
             * Regex regex = new Regex(@"\d+");
        Match match = regex.Match("Dot 55 Perls");
        if (match.Success)
        {
             * 
             */
            Regex r = new Regex("^[a-zA-Z0-9]*");
            if (!r.Match(Args[1]).Success) {
                Sender.SendMessage(ChatColors.Red + "Invalid Faction Name");
                return;
            }
            Faction ffaction = Main.FF.GetFaction(Args[1]);
            if (ffaction == null) {
                Faction lc = Main.FF.factionPartialName(Args[1]);
                if (lc == null) {
                    Sender.SendMessage(ChatColors.Red + "Faction does not exist");
                    return;
                } else {
                    ffaction = lc;
                }
            }
            String dn = ffaction.GetDisplayName();
            Sender.SendMessage("-------------------------");
            String faa = "";
            Sender.SendMessage(dn);
            Sender.SendMessage(ChatColors.Yellow+"Leader: " +ChatColors.Aqua+ ffaction.GetLeader());
            Sender.SendMessage(ChatColors.Yellow+"# of Players: " +ChatColors.Aqua+ ffaction.GetNumberOfPlayers());
            int max = ffaction.GetMaxPlayers();
            Sender.SendMessage(ChatColors.Yellow+"Max # of Players: " +ChatColors.Aqua+ max);
            Sender.SendMessage(ChatColors.Yellow+"MOTD: " +ChatColors.Aqua+ ffaction.GetMOTD());
            Sender.SendMessage(ChatColors.Yellow+"Desc: " +ChatColors.Aqua+ ffaction.GetDesc());
            Sender.SendMessage(ChatColors.Yellow+"Power: " +ChatColors.Aqua+ ffaction.GetPower());
            Sender.SendMessage("-------------------------");
        } else {
            if (fac == null) {
                if (Main.FF.GetPlayerFaction(Sender.GetPlayer()) == null) {
                    Sender.SendMessage(ChatColors.Red + "[CyberTech] You are not in a Faction!");
                }
                Sender.SendMessage(ChatColors.Red + "[CyberTech] You are not in a Faction!");
                return;
            }
            String dn = fac.GetDisplayName();
            Sender.SendMessage("-------------------------");
            String faa = "";
            Sender.SendMessage(ChatColors.Yellow+"Faction name: "+ChatColors.Aqua+dn);
            Sender.SendMessage(ChatColors.Yellow+"Leader: " +ChatColors.Aqua+ fac.GetLeader());
            Sender.SendMessage(ChatColors.Yellow+"# of Players: " +ChatColors.Aqua+ fac.GetNumberOfPlayers());
            int max = fac.GetMaxPlayers();
            Sender.SendMessage(ChatColors.Yellow+"Max # of Players: " +ChatColors.Aqua+ max);
            Sender.SendMessage(ChatColors.Yellow+"MOTD: " +ChatColors.Aqua+ fac.GetMOTD());
            Sender.SendMessage(ChatColors.Yellow+"Desc: " +ChatColors.Aqua+ fac.GetDesc());
            Sender.SendMessage(ChatColors.Yellow+"Power: " +ChatColors.Aqua+ fac.GetPower()+" of "+fac.CalculateMaxPower());
            Sender.SendMessage(ChatColors.Yellow+"Land Owned: " +ChatColors.Aqua+ fac.GetPlots().Count);
            Sender.SendMessage(ChatColors.Yellow+"Money: " +ChatColors.Aqua+ fac.GetMoney());
            Sender.SendMessage(ChatColors.Yellow+"Money: " +ChatColors.Aqua+ fac.GetMoney());
            Sender.SendMessage(ChatColors.Yellow+"XP: " +ChatColors.Aqua+ fac.GetXp()+" / "+fac.calculateRequireExperience(fac.GetLevel()));
            Sender.SendMessage(ChatColors.Yellow+"Level: " +ChatColors.Aqua+ fac.GetLevel());
            Sender.SendMessage("-------------------------");
        }
    }
}