
/**
 * Created by carlt_000 on 7/9/2016.
 */

using System;
using System.Collections.Generic;
using Faction2;
using Faction2.Commands;
using Faction2.Utils;
using Factions2;
using MiNET;
using MiNET.Effects;
using MiNET.Items;
using MiNET.Utils;
using Newtonsoft.Json.Linq;

public class PerkCmd : Commands {

    public PerkCmd(CommandSender s, String[] a, Faction_main m): base(s,a,"/f perk",m){
        senderMustBePlayer = true;
        senderMustBeMember = true;
        sendUsageOnFail = true;

        if(run()){
            RunCommand();
        }
    }

    
    private new void RunCommand(){
        if(Args.Length == 1){
            Sender.SendMessage(ChatColors.YELLOW+
                    "--------------------------\n" +
                    " - /f perk list [page]\n"+
                    " - /f perk claim <id>\n"+
                    "--------------------------"
            );
        }else if(Args.Length == 2){
            if(Args[1].equalsIgnoreCase("list")){
                SendList(1);
            }
        }else if(Args.Length == 3){
            if(Args[1].equalsIgnoreCase("claim")){
                //TODO GO AND FIND ALL PARSE AND FIX SO IT WONT CRASH!!!!!
                int id = int.Parse(Args[2]);
                ClaimPerk(id);
            }
        }
    }

    //TODO
    public void ClaimPerk(int id){   
        foreach(Perk b in Main.Perks)
        {
            string nname = b.name;
            string desc = b.desc;
            String iidd = b.id;
            int cxp = b.c_xp;
            int clevel = b.c_levels;
            int cmoney = b.c_money;
            if(cxp != 0 && !fac.TakeXp(cxp)){
                Sender.SendMessage(Faction_main.NAME+ChatColors.Red+"Error! You don't have enough faction XP! "+cxp+" XP is requiRed for this perk!");
                return;
            }
            if(clevel != 0 && fac.GetLevel() < clevel){
                Sender.SendMessage(Faction_main.NAME+ChatColors.Red+"Error! You'ur faction is not high enough in level! "+clevel+" Levels is requiRed for this perk!");
                return;
            }
            fac.TakeLevel(clevel);
            if(cmoney != 0 && fac.GetMoney() < cmoney){
                Sender.SendMessage(Faction_main.NAME+ChatColors.Red+"Error! You don't have enough faction money! $"+cmoney+" is requiRed for this perk!");
                return;
            }
            fac.TakeMoney(cmoney);
            
            fac.BroadcastMessage(Faction_main.NAME+ChatColors.GREEN+nname+" Perk Claimed!");
            /*
        *       cmds: []
                effects:
                 3:1: 6000
                items: []
                level: 0
                xp: 0
                money: 0
            * */
            string[] cmds = b.r_cmds;
            string[] effects = b.r_effects;
            string[] items = b.r_items;
            int level = b.r_level;
            int xp = b.r_xp;
            int money = b.r_money;
            if(effects.Length != 0){
                foreach(Player player in fac.GetOnlinePlayers()){
                    foreach(Map.Entry<String, Object> c: effect.entrySet()) {
                        String key = c.getKey();
                        int length = (int) c.getValue();
                        int eid;
                        int lvl = 1;
                        if (key.contains("|")) {
                            eid = int.parseInt(key.Split(new[] {"|"},[0]);
                            lvl = int.parseInt(key.split("\\|")[1]);
                        } else {
                            eid = int.parseInt(key);
                        }
                        Effect e = Effect.getEffect(eid);
                        e.setDuration(length);
                        e.setAmplifier(lvl);
                        player.addEffect(e);
                    }
                }
                fac.BroadcastMessage(Faction_main.NAME+ChatColors.GREEN+effect.size()+" Effects added to everyone!");
            }

            if(items.size() != 0){
                for(Player player: fac.GetOnlinePlayers()){
                    for(Map.Entry<String, Object> c: effect.entrySet()) {
                        String key = c.getKey();
                        int bid;
                        int bmeta = 0;
                        int bcount = 0;
                        if (key.contains("|")) {
                            bid = int.parseInt(key.split("\\|")[0]);
                            bmeta = int.parseInt(key.split("\\|")[1]);
                        } else {
                            bid = int.parseInt(key);
                        }
                        Item i = Item.get(bid,bmeta,bcount);
                        player.getInventory().addItem(i);
                    }
                }
                fac.BroadcastMessage(Faction_main.NAME+ChatColors.GREEN+items.size()+" Items added to everyone's Inventories!");
            }

            if(level != 0){
                fac.AddLevel(level);
                fac.BroadcastMessage(Faction_main.NAME+ChatColors.GREEN+level+" Levels added to Faction Level!");
            }
            if(xp != 0){
                fac.AddXP(xp);
                fac.BroadcastMessage(Faction_main.NAME+ChatColors.GREEN+xp+" XP added to Faction Experience!");
            }
            if(money != 0){
                fac.AddMoney(money);
                fac.BroadcastMessage(Faction_main.NAME+ChatColors.GREEN+"$"+money+" added to Faction Balance!");
            }
            return;
        }
        Sender.SendMessage(Faction_main.NAME+ChatColors.Red+"Error that id is invalid!");
    }

    public void SendList(int p){
        List<String> a = new List<>();
        for(Map.Entry<String, Object> c: Main.Perks.getAll().entrySet()){
            ConfigSection b = (ConfigSection) c.getValue();
            String iidd = ((int)b.get("id"))+"";
            String name = (String)b.get("name");
            String desc = (String)b.get("desc");
            a.add(ChatColors.GRAY+"["+ChatColors.GREEN+iidd+ChatColors.GRAY+"]"+ChatColors.AQUA+" "+name+ChatColors.YELLOW+" > "+ChatColors.GRAY+ desc);
        }

        int to = p * 5;
        int from = to - 5;
        // 5 -> 0 ||| 10 -> 5
        int x = 0;
        String t = "";

        t += ChatColors.GRAY+"-----"+ChatColors.Gold+".<[Faction Perk List]>."+ChatColors.GRAY+"-----\n";
        for(String vvalue : a){
            if(!(x < to && x >= from)){
                x++;
                continue;
            }
            if(x > to)break;
            x++;
            t += vvalue + " \n";
        }
        t += "------------------------------";
        Sender.SendMessage(t);
    }
}
