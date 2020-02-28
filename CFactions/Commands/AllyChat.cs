using System;
using Faction2.Utils;
using Factions2;
using MiNET.Utils;

namespace Faction2.Commands
{
/**
 * Created by carlt_000 on 7/9/2016.
 */
    public class AllyChat : Commands
    {
        public AllyChat(CommandSender s, string[] a, Faction_main m) : base(s, a, "/f allychat [Text]", m)
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
                //todo Have FAlly and FChat be on by default when a Player Logs in
                if (Args[1] == ("on"))
                {
                    if (fac.getFAlly().Contains(name)) fac.getFAlly().Remove(name);
                    Sender.SendMessage(ChatColors.Green + "Ally Chat Activated!");
                    return;
                }
                else if (Args[1] == ("off"))
                {
                    if (fac.getFAlly().Contains(name))
                        Sender.SendMessage(ChatColors.Red + "Error! Ally Chat is alreayd off!");
                    else
                        fac.getFAlly().Add(name.ToLower());
                    return;
                }
            }
            foreach (String c in Args)
            {
                a++;
                if (a == 1) continue;
                chat += c + " ";
            }
            String n = Sender.getName();
            if (fac.Leader.ToLower().Equals(Sender.getName().ToLower()))
                fac.MessageAllys(ChatColors.Yellow + "~***[" + n + "]***~: " + chat);
            else if (fac.IsGeneral(Sender.getName()))
                fac.MessageAllys(ChatColors.Yellow + "~**[" + n + "]**~: " + chat);
            else if (fac.IsOfficer(Sender.getName()))
                fac.MessageAllys(ChatColors.Yellow + "~*[" + n + "]*~: " + chat);
            else if (fac.IsMember(Sender.getName()))
                fac.MessageAllys(ChatColors.Yellow + "~[" + n + "]~: " + chat);
            else
                fac.MessageAllys(ChatColors.Yellow + "-[" + n + "]-: " + chat);
        }
    }
}