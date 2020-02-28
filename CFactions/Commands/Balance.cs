using System;
using Faction2.Utils;
using Factions2;
using MiNET.Utils;

namespace Faction2.Commands
{
/**
 * Created by carlt_000 on 7/9/2016.
 */
    public class Balance : Commands
    {
        public Balance(CommandSender s, String[] a, Faction_main m) : base(s, a, "/f balance", m){
            senderMustBePlayer = true;
            senderMustBeInFaction = true;
            sendUsageOnFail = true;
            if (run())RunCommand();
        }

        private new void RunCommand()
        {
            int money = fac.GetMoney();
            Sender.SendMessage(Faction_main.NAME + ChatColors.Green + "Your Faction has " + ChatColors.Aqua + money);
            fac.UpdateTopResults();
        }
    }
}