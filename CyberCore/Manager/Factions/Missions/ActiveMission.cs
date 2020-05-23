using System;
using System.Collections.Generic;
using CyberCore.Utils;
using MiNET;
using MiNET.Items;
using MiNET.Net;
using MiNET.Utils;
using OpenAPI.Events.Block;

namespace CyberCore.Manager.Factions.Missions
{
    public class ActiveMission : Mission
    {
        
    public Faction faction;
    public Dictionary<String, int> BreakCount { get; set; } = new Dictionary<String, int>();
    public Dictionary<String, int> PlaceCount { get; set; } = new Dictionary<String, int>();
    public int KillCount  { get; set; }= 0;

    public ActiveMission(FactionsMain fm, Faction f,MissionData m) : base(fm,m)
    {
        faction = f;
    }
    public ActiveMission(FactionsMain main, Faction f, ActiveMissionData amd): base(main) {
        
        faction = f;
        BreakCount = amd.BreakCount;
        PlaceCount = amd.PlaceCount;
        KillCount = amd.KillCount;
    }

    // public ActiveMission(FactionsMain main, Faction fac, Mission mission) {
    //     super(main, mission);
    //     faction = fac;
    // }

    public int GetKills() {
        return KillCount;
    }

    public void AddKill() {
        KillCount++;
    }

    public bool CheckBreak() {
        foreach (var a in Break) {
            int min = a.Value;
            if (BreakCount.ContainsKey(a.Key)) {
                int current = BreakCount[a.Key];
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
        foreach (var a in Place) {
            int min = a.Value;
            if (PlaceCount.ContainsKey(a.Key)) {
                int current = PlaceCount[a.Key];
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
        int id = ev.Block.Id;
        int meta = ev.Block.GetState().Data;
        String key;
        if (meta != 0) {
            key = id + "|" + meta;
        } else {
            key = id + "";
        }
        if (Break.ContainsKey(key)) {
            if (BreakCount.ContainsKey(key)) {
                BreakCount[key]++;;
            } else {
                BreakCount[key] = 1;
            }
        }
        CheckCompletion();
    }

    public void PlaceBlock(BlockPlaceEvent ev) {
        int id = ev.Block.Id;
        int meta = ev.Block.GetState().Data;
        String key;
        if (meta != 0) {
            key = id + "|" + meta;
        } else {
            key = id + "";
        }
        if (Place.ContainsKey(key)) {
            if (PlaceCount.ContainsKey(key)) {
                PlaceCount[key]++;
            } else {
                PlaceCount.Add(key, 1);
            }
        }
        CheckCompletion();
    }

    public String PlaceBlockStatus() {
        String fnl = ChatColors.Gray+"--------Place Block Status------\n";
        foreach (var a in Place) {
            int min = a.Value;
            Item c;
            if(a.Key.Contains("|")){
                c = ItemFactory.GetItem((short) int.Parse(a.Key.Split("|")[0]),(short) int.Parse(a.Key.Split("|")[1]));
            }else{
                c = ItemFactory.GetItem((short) int.Parse(a.Key));
            }
            if (PlaceCount.ContainsKey(a.Key)) {
                int current = PlaceCount[a.Key];
                if (current >= min) {
                    fnl = fnl + ChatColors.Gray+"["+ ChatColors.Green+"X"+ChatColors.Gray+"]" +
                            "  " + ChatColors.Aqua+c.getName()+
                            ChatColors.Gray+" | "+
                            ChatColors.Green+current+
                            ChatColors.Gray+" / "+ChatColors.Green+min+"\n";
                }else{
                    fnl = fnl + ChatColors.Gray+"["+ ChatColors.Red+"-"+ChatColors.Gray+"]" +
                            "  " + ChatColors.Aqua+c.getName()+
                            ChatColors.Gray+" | "+
                            ChatColors.Yellow+current+
                            ChatColors.Gray+" / "+ChatColors.Yellow+min+"\n";
                }
            }else{
                fnl = fnl + ChatColors.Gray+"["+ ChatColors.Red+"-"+ChatColors.Gray+"]" +
                        "  " + ChatColors.Aqua+c.getName()+
                        ChatColors.Gray+" | "+
                        ChatColors.Red+0+
                        ChatColors.Gray+" / "+ChatColors.Yellow+min+"\n";
            }
        }
        return fnl;
    }

    public String BreakBlockStatus() {
        String fnl = ChatColors.Gray+"--------Break Block Status------\n";
        foreach (var a in Break) {
            int min = a.Value;
            Item c;
            if(a.Key.Contains("|")){
                c = ItemFactory.GetItem((short) int.Parse(a.Key.Split("|")[0]),(short) int.Parse(a.Key.Split("|")[1]));
            }else{
                c = ItemFactory.GetItem((short) int.Parse(a.Key));
            }
            if (BreakCount.ContainsKey(a.Key)) {
                int current = BreakCount[a.Key];
                if (current >= min) {
                    fnl = fnl + ChatColors.Gray+"["+ ChatColors.Green+"X"+ChatColors.Gray+"]" +
                            "  " + ChatColors.Aqua+c.getName()+
                            ChatColors.Gray+" | "+
                            ChatColors.Green+current+
                            ChatColors.Gray+" / "+ChatColors.Green+min+"\n";
                }else{
                    fnl = fnl + ChatColors.Gray+"["+ ChatColors.Red+"-"+ChatColors.Gray+"]" +
                            "  " + ChatColors.Aqua+c.getName()+
                            ChatColors.Gray+" | "+
                            ChatColors.Yellow+current+
                            ChatColors.Gray+" / "+ChatColors.Yellow+min+"\n";
                }
            }else{
                fnl = fnl + ChatColors.Gray+"["+ ChatColors.Red+"-"+ChatColors.Gray+"]" +
                        "  " + ChatColors.Aqua+c.getName()+
                        ChatColors.Gray+" | "+
                        ChatColors.Red+0+
                        ChatColors.Gray+" / "+ChatColors.Yellow+min+"\n";
            }
        }
        return fnl;
    }

    public String ItemStatus() {
        String fnl = ChatColors.Gray+"--------Item Status--------\n";
        Dictionary<String, int> map = new Dictionary<String, int>();
        foreach (var a in CyberCoreMain.GetInstance().getAPI().PlayerManager.GetPlayers()) {
            if (!faction.IsInFaction(a)) {
                continue;
            }
            PlayerInventory inv = a.Inventory;
            foreach (Item b in inv.Slots) {
                foreach (Item c in getItemReq()) {
                    if (b.Equals(c)) {
                        String key = b.Id + "|" + b.Metadata;
                        if (map.ContainsKey(key)) map[key] =+ b.Count;
                        else map[key] = b.Count;
                    }
                }
            }
        }
        bool fail = false;
        foreach (Item c in getItemReq()) {
            String key = c.Id + "|" + c.Metadata;
            if (map.ContainsKey(key)) {
                float a = map[key] / c.Count;
                if (a > 1) a = 1;
                int v = (int) Math.Round(a * 100f);
                // [X]  TEST | 10 / 100
                if (v != 100) {
                    fnl = fnl + ChatColors.Gray+"["+ ChatColors.Red+"-"+ChatColors.Gray+"]" +
                            "  " + ChatColors.Aqua+c.getName()+
                            ChatColors.Gray+" | "+
                            ChatColors.Yellow+map[key]+
                            ChatColors.Gray+" / "+ChatColors.Yellow+c.Count+"\n";
                }else{
                    fnl = fnl + ChatColors.Gray+"["+ ChatColors.Green+"X"+ChatColors.Gray+"]" +
                            "  " + ChatColors.Aqua+c.getName()+
                            ChatColors.Gray+" | "+
                            ChatColors.Green+map[key]+
                            ChatColors.Gray+" / "+ChatColors.Green+c.Count+"\n";
                }
                ;
            } else {
                fnl = fnl + ChatColors.Gray+"["+ ChatColors.Red+"-"+ChatColors.Gray+"]" +
                        "  " + ChatColors.Aqua+c.getName()+
                        ChatColors.Gray+" | "+
                        ChatColors.Red+0+
                        ChatColors.Gray+" / "+ChatColors.Yellow+c.Count+"\n";
            }
        }
        return fnl;
    }

    public Item CheckPlayerItems() {
        Dictionary<String, int> map = new Dictionary<String, int>();
        if(Main == null){
            CyberCoreMain.Log.Error("Was LOG ||"+"WTF!!??!?! Really!");
            return ItemFactory.GetItem(1,0,0);
        }
        foreach (var a in Main.CCM.getAPI().PlayerManager.GetPlayers()) {
            if (!faction.IsInFaction(a)) continue;
            PlayerInventory inv = a.Inventory;
            foreach (Item b in inv.Slots) {
                foreach (Item c in getItemReq()) {
                    if (b.Equals(c)) {
                        String key = b.Id + "|" + b.Metadata;
                        if (map.ContainsKey(key)) {
                            map[key] =+ b.Count;
                        } else {
                            map.Add(key, b.Count);
                        }
                    }
                }
            }
        }
        bool fail = false;
        foreach (Item c in getItemReq()) {
            String key = c.Id + "|" + c.Metadata;
            if (map.ContainsKey(key)) {
                float a = map[key] / c.Count;
                if (a > 1) a = 1;
                int v = (int) Math.Round(a * 100f);
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
            CyberCoreMain.Log.Info("FACTION NUL!?!??!?! ETFaaa");
            return 5;
        }
        faction.CompleteMission(this);
        String msg = ChatColors.Green + name + " Mission completed! You rewards have been given!";
        faction.BroadcastMessage(FactionsMain.NAME + msg);
        GiveReward();
        return 0;

    }

    public void GiveReward() {
        if (ItemReward.Count > 0) {
            foreach (var a in Main.CCM.getAPI().PlayerManager.GetPlayers()) {
                if (!faction.IsInFaction(a))continue;
                foreach (Item i in getItemReward()) {
                    if (!faction.IsInFaction(a)) continue;
                    a.Inventory.AddItem(i,true);
                    a.SendPlayerInventory();
                }
            }
        }
        if (MoneyReward != 0) faction.getSettings().addMoney(MoneyReward);
        if (XPReward != 0) faction.getSettings().addXP(XPReward);
        if (PointReward != 0) faction.getSettings().addPoints(PointReward);
    }

    public ActiveMissionData getAMD()
    {
        return new ActiveMissionData(this);
    }
    
    }
}