using System;
using Faction2.Utils;
using Factions2;
using MiNET.Utils;

namespace Faction2.Commands
{
    public class Chat : Commands
    {
        public Chat(CommandSender s, String[] a, Faction_main m) : base(s, a, "/f chat [Text]", m)
        {
            senderMustBeInFaction = true;
            senderMustBePlayer = true;
            sendFailReason = true;
            sendUsageOnFail = true;

            if (run())
            {
                RunCommand();
            }
        }


        private new void RunCommand()
        {
            String chat = "";
            int a = 0;
            if (Args.Length == 2)
            {
                String name = Sender.getName().ToLower();
                if (Args[1] == ("on"))
                {
                    if (fac.getFAlly().Contains(name)) fac.getFAlly().Remove(name);
                    Sender.SendMessage(ChatColors.Green + "Faction Chat Activated!");
                    return;
                }
                else if (Args[1] == ("off"))
                {
                    if (fac.getFChat().Contains(name))
                        Sender.SendMessage(ChatColors.Red + "Error! Faction Chat is already off!");
                    else fac.getFChat().Add(name);
                    return;
                }
            }
            foreach (string c in Args)
            {
                a++;
                if (a == 1) continue;
                chat += c + " ";
            }
            String n = Sender.getName();
            if (fac.Leader.ToLower().Equals(Sender.getName().ToLower()))
                fac.BroadcastMessage(ChatColors.Yellow + "~***[" + n + "]***~: " + chat);
            else if (fac.IsGeneral(Sender.getName()))
                fac.BroadcastMessage(ChatColors.Yellow + "~**[" + n + "]**~: " + chat);
            else if (fac.IsOfficer(Sender.getName()))
                fac.BroadcastMessage(ChatColors.Yellow + "~*[" + n + "]*~: " + chat);
            else if (fac.IsMember(Sender.getName()))
                fac.BroadcastMessage(ChatColors.Yellow + "~[" + n + "]~: " + chat);
            else
                fac.BroadcastMessage(ChatColors.Yellow + "-[" + n + "]-: " + chat);
        }
    }
}