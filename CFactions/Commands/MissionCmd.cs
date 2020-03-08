using System;
using System.Collections;
using Faction2;
using Faction2.Commands;
using Faction2.Utils;
using Factions2;
using MiNET.Utils;

public class MissionCmd : Commands
{
    public MissionCmd(CommandSender s, String[] a, Faction_main m) : base(s, a, "/f mission [radius = 1]", m)
    {
        senderMustBeInFaction = true;
        senderMustBePlayer = true;
        senderMustBeGeneral = true;
        sendFailReason = true;
        sendUsageOnFail = true;

        if (run())
        {
            RunCommand();
        }
    }


    private new void RunCommand()
    {
        if (Args.Length == 1)
        {
            Sender.SendMessage(ChatColors.Yellow + "--------------------------\n" +
                               " - /f mission cancel\n" +
                               " - /f mission accept <id>\n" +
                               " - /f mission list [page]\n" +
                               " - /f mission status\n" +
                               " - /f mission claim\n" +
                               "--------------------------"
            );
        }
        else if (Args.Length == 2)
        {
            if (Args[1].equalsIgnoreCase("cancel"))
            {
                fac.SetActiveMission();
                fac.BroadcastMessage(Faction_main.NAME + ChatColors.YELLOW + "Faction mission canceled!");
            }
            else if (Args[1].equalsIgnoreCase("list"))
            {
                SendList(1);
            }
            else if (Args[1].equalsIgnoreCase("status"))
            {
                if (fac.GetActiveMission() != null)
                {
                    Sender.SendMessage(fac.GetActiveMission().BreakBlockStatus() + "\n" +
                                       fac.GetActiveMission().PlaceBlockStatus() + "\n" +
                                       fac.GetActiveMission().ItemStatus());
                }
                else
                {
                    Sender.SendMessage(Faction_main.NAME + ChatColors.Red +
                                       "Your faction doesn't have an active mission!");
                }
            }
            else if (Args[1].equalsIgnoreCase("claim"))
            {
                if (fac.GetActiveMission() != null)
                {
                    if (fac.GetActiveMission().CheckCompletion(true) != 0)
                    {
                        Sender.SendMessage(Faction_main.NAME + ChatColors.Red +
                                           "Error! You have not completed all requirements of the Mission");
                    }
                }
                else
                {
                    Sender.SendMessage(Faction_main.NAME + ChatColors.Red +
                                       "Your faction doesn't have an active mission!");
                }
            }
        }
        else if (Args.Length == 3)
        {
            if (Args[1].equalsIgnoreCase("accept"))
            {
                try
                {
                    int id = int.Parse(Args[2]);
                    fac.AcceptNewMission(id, Sender.GetPlayer());
                }
                catch (Exception ex)
                {
                    Sender.SendMessage(Faction_main.NAME + ChatColors.Red + "Error! Usage: /f mission accept <id>");
                }
            }
            else if (Args[1].equalsIgnoreCase("list"))
            {
                try
                {
                    SendList(int.Parse(Args[2]));
                }
                catch (Exception ex)
                {
                    Sender.SendMessage(Faction_main.NAME + ChatColors.Red + "Error! Usage: /f mission list <id>");
                }
            }
        }
    }

    public void SendList(int p)
    {
        List a = new List();
        foreach (Mission mission in Main.Missions){
            if (fac.GetCompletedMissions().Contains(mission.id))
            {
                //@TODO Decide weather to show this or just show the Red...
                a.Add(ChatColors.GRAY + "[" + ChatColors.Red + mission.id + ChatColors.GRAY + "]" + ChatColors.AQUA +
                      " " + mission.name + ChatColors.YELLOW + " > " + ChatColors.GRAY + mission.desc);
            }
            else
            {
                a.Add(ChatColors.GRAY + "[" + ChatColors.GREEN + mission.id + ChatColors.GRAY + "]" + ChatColors.AQUA +
                      " " + mission.name + ChatColors.YELLOW + " > " + ChatColors.GRAY + mission.desc);
            }
        }

        int to = p*5;
        int from = to - 5;
        // 5 -> 0 ||| 10 -> 5
        int x = 0;
        String t = "";

        t += ChatColors.GRAY + "-----" + ChatColors.Gold + ".<[Faction Mission List]>." + ChatColors.GRAY + "-----\n";
        foreach (String vvalue in a){
            if (!(x < to && x >= from))
            {
                x++;
                continue;
            }
            if (x > to) break;
            x++;
            t += vvalue + " \n";
        }
        t += "------------------------------";
        Sender.SendMessage(t);
    }
}