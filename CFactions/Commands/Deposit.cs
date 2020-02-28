using System;
using Faction2.Commands;
using Faction2.Utils;
using Factions2;
using MiNET.Utils;

public class Deposit : Commands
{
    public Deposit(CommandSender s, String[] a, Faction_main m) : base(s, a, "/f deposit <amount>", m)
    {
        senderMustBePlayer = true;
        senderMustBeMember = true;
        senderMustBeInFaction = true;
        sendUsageOnFail = true;

        if (run())
        {
            RunCommand();
        }
    }


    private new void RunCommand()
    {
        if (Args.Length < 2)
        {
            SendUseage();
            return;
        }
        try
        {
            int money = int.Parse(Args[1]);
            if (!Main.Econ.GetData(Sender.GetPlayer()).TakeMoney(money))
            {
                Sender.SendMessage(Faction_main.NAME + ChatColors.Red + "You don't have " + money + " Money!");
                return;
            }
            fac.AddMoney(money);
            Sender.SendMessage(Faction_main.NAME + ChatColors.Green + "$" + money + " Money Added to your Faction!");
            fac.BroadcastMessage(Faction_main.NAME + ChatColors.Green + Sender.getName() + " has deposited $" + money +
                                 " Money to the faction account!");
        }
        catch (Exception e)
        {
            SendUseage();
        }
    }
}