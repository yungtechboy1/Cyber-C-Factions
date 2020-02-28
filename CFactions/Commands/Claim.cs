using System;
using Faction2.Utils;
using Factions2;
using MiNET;
using MiNET.Utils;

namespace Faction2.Commands
{
    public class Claim : Commands
    {
        public Claim(CommandSender s, string[] a, Faction_main m) : base(s, a, "/f claim [radius = 1]", m)
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
            int Radius = GetintAtArgs(1, 1);
            if (Radius > 1)
            {
                int rr = Radius*Radius;
                int money = (5000*rr);
                int power = (1*rr);
                if (fac.GetMoney() > money)
                {
                    Sender.SendMessage(Faction_main.NAME + ChatColors.Red + "Your Faction does not have $" + money +
                                       " in your faction account!");
                    return;
                }
                if (fac.GetPower() < power)
                {
                    Sender.SendMessage(Faction_main.NAME + ChatColors.Red + "Your Faction does not have enough power!");
                    return;
                }
                for (int x = (-1* Radius); x < Radius; x++)
                {
                    for (int z = (-1*Radius); z < Radius; z++)
                    {
                        int xx = ((int) Sender.GetPlayer().KnownPosition.X >> 4) + x;
                        int zz = ((int) Sender.GetPlayer().KnownPosition.Z >> 4) + z;
                        ClaimLand(xx, zz);
                    }
                }
            }
            else
            {
                int x = (int)Sender.GetPlayer().KnownPosition.X >> 4;
                int z = (int)Sender.GetPlayer().KnownPosition.Z >> 4;
                //amount = (100) * Main.prefs["PlotPrice"];
                int money = 5000;
                int power = 1;
                if (fac.GetMoney() < money)
                {
                    Sender.SendMessage(Faction_main.NAME + ChatColors.Red + "Your Faction does not have $" + money +
                                       " in your faction account!");
                    return;
                }
                if (fac.GetPower() < power)
                {
                    Sender.SendMessage(Faction_main.NAME + ChatColors.Red + "Your Faction does not have " + power +
                                       " Power!");
                    return;
                }
                if (Main.FF.PlotsList.ContainsKey(x + "|" + z))
                {
                    Sender.SendMessage(Faction_main.NAME + ChatColors.Red + "That land is already Claimed by" + Main.FF.PlotsList[x + "|" + z] + "'s Faction!!");
                    return;
                }
                Sender.SendMessage(Faction_main.NAME + ChatColors.Green + "Purchase Successful! $5000 and 1 Power Withdrawn To Purchase This Chunk!");
                fac.TakeMoney(money);
                fac.AddPlots(x + "|" + z);
                fac.TakePower(power);
                Main.FF.PlotsList.Add(x + "|" + z, fac.GetName());
            }
        }

        private void ClaimLand(int x, int z)
        {
            int money = 5000;
            int power = 1;
            if (fac.GetMoney() < money)
            {
                Sender.SendMessage(Faction_main.NAME + ChatColors.Red + "Your Faction does not have $" + money +
                                   " in your faction account to claim Chunk at X:" + x + " Z:" + z + "!");
                return;
            }
            if (fac.GetPower() < power)
            {
                Sender.SendMessage(Faction_main.NAME + ChatColors.Red + "Your Faction does not have " + power +
                                   " Power to claim Chunk at X:" + x + " Z:" + z + "!");
                return;
            }
            if (Main.FF.PlotsList.ContainsKey(x + "|" + z))
            {
                Sender.SendMessage(Faction_main.NAME + ChatColors.Red + "That Chunk at X:" + x + " Z:" + z +
                                   " is already Claimed by" + Main.FF.PlotsList[x + "|" + z] + "'s Faction!!");
                return;
            }
            Sender.SendMessage(Faction_main.NAME + ChatColors.Green +
                               "Purchase Successful! $5000 and 1 Power Withdrawn To Purchase Chunk at X:" + x + " Z:" +
                               z + "!");
            fac.TakeMoney(money);
            fac.AddPlots(x + "|" + z);
            fac.TakePower(power);
            Main.FF.PlotsList.Add(x + "|" + z, fac.GetName());
        }
    }
}