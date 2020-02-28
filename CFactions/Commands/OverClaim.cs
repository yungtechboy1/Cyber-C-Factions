
using System;
using Faction2;
using Faction2.Commands;
using Faction2.Utils;
using Factions2;
using MiNET;
using MiNET.Utils;

public class OverClaim : Commands {

    public OverClaim(CommandSender s, String[] a, Faction_main m) :
        base(s, a, "/f overclaim [radius = 1]", m){
        senderMustBeInFaction = true;
        senderMustBePlayer = true;
        sendFailReason = true;
        sendUsageOnFail = true;

        if (run()) {
            RunCommand();
        }
    }

    
    public new void RunCommand() {
        int Radius = GetintAtArgs(1, 1);
        if (Radius > 1) {
            int rr = Radius * Radius;
            int money = (5000 * rr);
            int power = (3 * rr);
            if (fac.GetMoney() > money) {
                Sender.SendMessage(Faction_main.NAME + ChatColors.Red + "Your Faction does not have $" + money + " in your faction account!");
                return;
            }
            if (fac.GetPower() < power) {
                Sender.SendMessage(Faction_main.NAME + ChatColors.Red + "Your Faction does not have "+power+" power!");
                return;
            }
            for (int x = (-1 * Radius); x < Radius; x++) {
                for (int z = (-1 * Radius); z < Radius; z++) {
                    int xx = ((int) Sender.GetPlayer().KnownPosition.X >> 4) + x;
                    int zz = ((int) Sender.GetPlayer().KnownPosition.Z >> 4) + z;
                    OverClaimLand(xx, zz);
                }
            }
        } else {
            int x = ((int) Sender.GetPlayer().KnownPosition.X >> 4);
            int z = ((int) Sender.GetPlayer().KnownPosition.Z >> 4);
            OverClaimLand(x,z);
        }

    }

    private void OverClaimLand(int x, int z) {
        int money = 5000;
        int power = 3;
        if (fac.GetMoney() < money) {
            Sender.SendMessage(Faction_main.NAME + ChatColors.Red + "Your Faction does not have $" + money + " in your faction account to claim Chunk at X:" + x + " Z:" + z + "!");
            return;
        }
        if (fac.GetPower() < power) {
            Sender.SendMessage(Faction_main.NAME + ChatColors.Red + "Your Faction does not have " + power + " Power to claim Chunk at X:" + x + " Z:" + z + "!");
            return;
        }
        if (!Main.FF.PlotsList.ContainsKey(x + "|" + z)) {
            Sender.SendMessage(Faction_main.NAME + ChatColors.YELLOW + "That Chunk at X:" + x + " Z:" + z + " is not Claimed by a faction and is being skipped!");
            return;
        }
        Faction fac2 = Main.FF.GetFaction(Main.FF.PlotsList[x + "|" + z]);
        if (fac.GetName().equalsIgnoreCase("peace")){
            Sender.SendMessage(Faction_main.NAME+ChatColors.Red+"Error! You can not overclaim peace!");
            return;
        }
        if (fac2.GetPower() < fac2.GetPlots().Count) {
            Sender.SendMessage(Faction_main.NAME + ChatColors.GREEN + "Plot Overclaim Successful! $5000 and 3 Power to over ClaimChunk at X:" + x + " Z:" + z + "!");
            fac.TakeMoney(money);
            fac.AddPlots(x + "|" + z);
            fac2.DelPlots(x + "|" + z);
            fac.TakePower(power);
            Main.FF.PlotsList.Remove(x + "|" + z);
            Main.FF.PlotsList.Add(x + "|" + z, fac.GetName());
        }else{
//TODO "??!????@??@?!??!??@??@?@??!??!?!?!?!?
        }
    }
}
