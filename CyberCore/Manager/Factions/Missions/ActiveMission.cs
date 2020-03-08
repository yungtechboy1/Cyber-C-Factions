﻿using System;
using System.Collections.Generic;
using CyberCore.Utils;
using MiNET;
using MiNET.Items;
using MiNET.Net;
using OpenAPI.Events.Block;

namespace CyberCore.Manager.Factions.Missions
{
    public class ActiveMission : Mission
    {
        public Faction faction;
    public Dictionary<String, int> BreakCount = new Dictionary<String, int>();
    public Dictionary<String, int> PlaceCount = new Dictionary<String, int>();
    public int KillCount = 0;

    public ActiveMission() {
        super(null, new ConfigSection());
    }
    public ActiveMission(FactionsMain main, Faction fac, ConfigSection cfg) {
        super(main, cfg);
        if(cfg.containsKey("BreakCount")){
            BreakCount = (Dictionary<String, int>) cfg.get("BreakCount");
        }
        if(cfg.containsKey("PlaceCount")){
            PlaceCount = (Dictionary<String, int>) cfg.get("PlaceCount");
        }
        if(cfg.containsKey("KillCount")){
            KillCount = cfg.getInt("KillCount");
        }
        faction = fac;
    }

    public ActiveMission(FactionsMain main, Faction fac, Mission mission) {
        super(main, mission);
        faction = fac;
    }

    public int GetKills() {
        return KillCount;
    }

    public void AddKill() {
        KillCount++;
    }

    public bool CheckBreak() {
        for (Map.Entry<String, int> a : Break.entrySet()) {
            int min = a.getValue();
            if (BreakCount.containsKey(a.getKey())) {
                int current = BreakCount.get(a.getKey());
                if (current < min) {
                    return false;
                }
            }else{
                return false;
            }
        }
        return true;
    }

    public bool CheckPlace() {
        for (Map.Entry<String, int> a : Place.entrySet()) {
            int min = a.getValue();
            if (PlaceCount.containsKey(a.getKey())) {
                int current = PlaceCount.get(a.getKey());
                if (current < min) {
                    return false;
                }
            }else{
                return false;
            }
        }
        return true;
    }

    public void BreakBlock(BlockBreakEvent ev) {
        int id = ev.getBlock().getId();
        int meta = ev.getBlock().getDamage();
        String key;
        if (meta != 0) {
            key = id + "|" + meta;
        } else {
            key = id + "";
        }
        if (Break.containsKey(key)) {
            if (BreakCount.containsKey(key)) {
                int c = BreakCount.get(key) + 1;
                BreakCount.put(key, c);
            } else {
                BreakCount.put(key, 1);
            }
        }
        CheckCompletion();
    }

    public void PlaceBlock(BlockPlaceEvent ev) {
        int id = ev.getBlock().getId();
        int meta = ev.getBlock().getDamage();
        String key;
        if (meta != 0) {
            key = id + "|" + meta;
        } else {
            key = id + "";
        }
        if (Place.containsKey(key)) {
            if (PlaceCount.containsKey(key)) {
                int c = PlaceCount.get(key) + 1;
                PlaceCount.put(key, c);
            } else {
                PlaceCount.put(key, 1);
            }
        }
        CheckCompletion();
    }

    public String PlaceBlockStatus() {
        String fnl = TextFormat.GRAY+"--------Place Block Status------\n";
        for (Map.Entry<String, int> a : Place.entrySet()) {
            int min = a.getValue();
            Item c;
            if(a.getKey().contains("|")){
                c = Item.get(int.parseInt(a.getKey().split("\\|")[0]),int.parseInt(a.getKey().split("\\|")[1]));
            }else{
                c = Item.get(int.parseInt(a.getKey()));
            }
            if (PlaceCount.containsKey(a.getKey())) {
                int current = PlaceCount.get(a.getKey());
                if (current >= min) {
                    fnl = fnl + TextFormat.GRAY+"["+ TextFormat.GREEN+"X"+TextFormat.GRAY+"]" +
                            "  " + TextFormat.AQUA+c.getName()+
                            TextFormat.GRAY+" | "+
                            TextFormat.GREEN+current+
                            TextFormat.GRAY+" / "+TextFormat.GREEN+min+"\n";
                }else{
                    fnl = fnl + TextFormat.GRAY+"["+ TextFormat.RED+"-"+TextFormat.GRAY+"]" +
                            "  " + TextFormat.AQUA+c.getName()+
                            TextFormat.GRAY+" | "+
                            TextFormat.YELLOW+current+
                            TextFormat.GRAY+" / "+TextFormat.YELLOW+min+"\n";
                }
            }else{
                fnl = fnl + TextFormat.GRAY+"["+ TextFormat.RED+"-"+TextFormat.GRAY+"]" +
                        "  " + TextFormat.AQUA+c.getName()+
                        TextFormat.GRAY+" | "+
                        TextFormat.RED+0+
                        TextFormat.GRAY+" / "+TextFormat.YELLOW+min+"\n";
            }
        }
        return fnl;
    }

    public String BreakBlockStatus() {
        String fnl = TextFormat.GRAY+"--------Break Block Status------\n";
        for (Map.Entry<String, int> a : Break.entrySet()) {
            int min = a.getValue();
            Item c;
            if(a.getKey().contains("|")){
                c = Item.get(int.parseInt(a.getKey().split("\\|")[0]),int.parseInt(a.getKey().split("\\|")[1]));
            }else{
                c = Item.get(int.parseInt(a.getKey()));
            }
            if (BreakCount.containsKey(a.getKey())) {
                int current = BreakCount.get(a.getKey());
                if (current >= min) {
                    fnl = fnl + TextFormat.GRAY+"["+ TextFormat.GREEN+"X"+TextFormat.GRAY+"]" +
                            "  " + TextFormat.AQUA+c.getName()+
                            TextFormat.GRAY+" | "+
                            TextFormat.GREEN+current+
                            TextFormat.GRAY+" / "+TextFormat.GREEN+min+"\n";
                }else{
                    fnl = fnl + TextFormat.GRAY+"["+ TextFormat.RED+"-"+TextFormat.GRAY+"]" +
                            "  " + TextFormat.AQUA+c.getName()+
                            TextFormat.GRAY+" | "+
                            TextFormat.YELLOW+current+
                            TextFormat.GRAY+" / "+TextFormat.YELLOW+min+"\n";
                }
            }else{
                fnl = fnl + TextFormat.GRAY+"["+ TextFormat.RED+"-"+TextFormat.GRAY+"]" +
                        "  " + TextFormat.AQUA+c.getName()+
                        TextFormat.GRAY+" | "+
                        TextFormat.RED+0+
                        TextFormat.GRAY+" / "+TextFormat.YELLOW+min+"\n";
            }
        }
        return fnl;
    }

    public String ItemStatus() {
        String fnl = TextFormat.GRAY+"--------Item Status--------\n";
        Dictionary<String, int> map = new Dictionary<>();
        for (Map.Entry<UUID, Player> a : Main.getServer().getOnlinePlayers().entrySet()) {
            if (!faction.IsInFaction(a.getValue())) {
                continue;
            }
            PlayerInventory inv = a.getValue().getInventory();
            for (Item b : inv.getContents().values()) {
                for (Item c : ItemReq) {
                    if (b.equals(c, true, false)) {
                        String key = b.getId() + "|" + b.getDamage();
                        if (map.containsKey(key)) map.put(key, map.get(key) + b.getCount());
                        else map.put(key, b.getCount());
                    }
                }
            }
        }
        bool fail = false;
        for (Item c : ItemReq) {
            String key = c.getId() + "|" + c.getDamage();
            if (map.containsKey(key)) {
                float a = map.get(key) / c.getCount();
                if (a > 1) a = 1;
                int v = Math.round(a * 100f);
                // [X]  TEST | 10 / 100
                if (v != 100) {
                    fnl = fnl + TextFormat.GRAY+"["+ TextFormat.RED+"-"+TextFormat.GRAY+"]" +
                            "  " + TextFormat.AQUA+c.getName()+
                            TextFormat.GRAY+" | "+
                            TextFormat.YELLOW+map.get(key)+
                            TextFormat.GRAY+" / "+TextFormat.YELLOW+c.getCount()+"\n";
                }else{
                    fnl = fnl + TextFormat.GRAY+"["+ TextFormat.GREEN+"X"+TextFormat.GRAY+"]" +
                            "  " + TextFormat.AQUA+c.getName()+
                            TextFormat.GRAY+" | "+
                            TextFormat.GREEN+map.get(key)+
                            TextFormat.GRAY+" / "+TextFormat.GREEN+c.getCount()+"\n";
                }
                ;
            } else {
                fnl = fnl + TextFormat.GRAY+"["+ TextFormat.RED+"-"+TextFormat.GRAY+"]" +
                        "  " + TextFormat.AQUA+c.getName()+
                        TextFormat.GRAY+" | "+
                        TextFormat.RED+0+
                        TextFormat.GRAY+" / "+TextFormat.YELLOW+c.getCount()+"\n";
            }
        }
        return fnl;
    }

    public Item CheckPlayerItems() {
        Dictionary<String, int> map = new Dictionary<>();
        if(Main == null){
            System.out.println("WTF!!??!?! Really!");
            return new Item(1,0,0);
        }
        for (Map.Entry<UUID, Player> a : Main.getServer().getOnlinePlayers().entrySet()) {
            if (!faction.IsInFaction(a.getValue())) continue;
            PlayerInventory inv = a.getValue().getInventory();
            for (Item b : inv.getContents().values()) {
                for (Item c : ItemReq) {
                    if (b.equals(c, true, false)) {
                        String key = b.getId() + "|" + b.getDamage();
                        if (map.containsKey(key)) {
                            map.put(key, map.get(key) + b.getCount());
                        } else {
                            map.put(key, b.getCount());
                        }
                    }
                }
            }
        }
        bool fail = false;
        for (Item c : ItemReq) {
            String key = c.getId() + "|" + c.getDamage();
            if (map.containsKey(key)) {
                float a = map.get(key) / c.getCount();
                if (a > 1) a = 1;
                int v = Math.round(a * 100f);
                if (v != 100) return c;
            } else {
                return c;
            }
        }
        return null;
    }

    public int CheckCompletion() {
        return CheckCompletion(false);
    }
    public int CheckCompletion(bool checkitems) {
        if (checkitems && CheckPlayerItems() != null) {
            return 1;
        }
        if (!CheckPlace()) {
            return 2;
        }
        if (!CheckBreak()) {
            return 3;
        }
        if(Kill < KillCount){
            return 4;
        }
        if(faction == null){
            Main.plugin.getLogger().info("FACTION NUL!?!??!?! ETFaaa");
            return 5;
        }
        faction.CompleteMission(this);
        String msg = TextFormat.GREEN + name + " Mission completed! You rewards have been given!";
        faction.BroadcastMessage(FactionsMain.NAME + msg);
        GiveReward();
        return 0;

    }

    public void GiveReward() {
        if (ItemReward.size() > 0) {
            for (Map.Entry<UUID, Player> a : Main.getServer().getOnlinePlayers().entrySet()) {
                if (!faction.IsInFaction(a.getValue()))continue;
                for (Item i : ItemReward) {
                    if (!faction.IsInFaction(a.getValue())) continue;
                    a.getValue().getInventory().addItem(i);
                    a.getValue().getInventory().sendContents(a.getValue());
                }
            }
        }
        if (MoneyReward != 0) faction.getSettings().addMoney(MoneyReward);
        if (XPReward != 0) faction.getSettings().addXP(XPReward);
        if (PointReward != 0) faction.getSettings().addPoints(PointReward);
    }

    public ConfigSection ToHashMap() {
        ConfigSection config = new ConfigSection();
        config.set("name", name);
        config.set("desc", desc);
        config.set("id", id);
        config.set("enabled", enabled);
        ConfigSection bb = new ConfigSection();
        ConfigSection ii = new ConfigSection();
        ConfigSection iii = new ConfigSection();
        ConfigSection pp = new ConfigSection();
        ConfigSection requirement = new ConfigSection() {{
            put("break", bb);
            put("place", pp);
            put("item", ii);
        }};
        ConfigSection reward = new ConfigSection();

        config.set("requirement", requirement);
        config.set("reward", reward);
        if (Break.size() > 0) {
            for (Map.Entry<String, int> a : Break.entrySet()) {
                String key = a.getKey();
                int val = a.getValue();
                bb.put(key, val);
            }
        }
        //place
        if (Place.size() > 0) {
            for (Map.Entry<String, int> a : Place.entrySet()) {
                String key = a.getKey();
                int val = a.getValue();
                pp.put(key, val);
            }
        }
        //item
        if (ItemReq.size() > 0) {
            for (Item a : ItemReq) {
                String key = "";
                if (a.getDamage() != 0) {
                    key = a.getId() + "|" + a.getDamage();
                } else {
                    key = a.getId() + "";
                }
                ii.set(key, a.getCount());
            }
        }
        requirement.set("kill", Kill);

        reward.set("xp", XPReward);
        reward.set("point", PointReward);
        reward.set("money", MoneyReward);
        reward.set("xp", XPReward);
        reward.set("item", iii);
        //item
        if (ItemReward.size() > 0) {
            for (Item a : ItemReward) {
                String key = "";
                if (a.getDamage() != 0) {
                    key = a.getId() + "|" + a.getDamage();
                } else {
                    key = a.getId() + "";
                }
                iii.set(key, a.getCount());
            }
        }
        config.set("BreakCount", BreakCount);
        config.set("PlaceCount", PlaceCount);
        config.set("KillCount", KillCount);
        return config;
    }
    }
}