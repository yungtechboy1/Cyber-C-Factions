using System;
using System.Collections.Generic;
using CyberCore.Utils;
using MiNET.Items;

namespace CyberCore.Manager.Factions.Missions
{
    public class Mission
    {
        public FactionsMain Main;
    public String name;
    public String desc;
    public int id;
    public bool enabled;
    public Dictionary<String, int> Break = new Dictionary<String, int>();
    public Dictionary<String, int> Place = new Dictionary<String, int>();
    public List<Item> ItemReq = new List<Item>();
    public int Kill = 0;
    public int XPReward = 0;
    public int MoneyReward = 0;
    public List<Item> ItemReward = new List<Item>();
    public List<MissionTask> Tasks = new List<MissionTask>();
    public int PointReward = 0;


    public Mission(FactionsMain main, Mission mission) {
        Main = mission.Main;
        name = mission.name;
        desc = mission.desc;
        id = mission.id;
        enabled = mission.enabled;
        Break = mission.Break;
        Place = mission.Place;
        ItemReq = mission.ItemReq;
        Kill = mission.Kill;
        XPReward = mission.XPReward;
        MoneyReward = mission.MoneyReward;
        ItemReward = mission.ItemReward;
        PointReward = mission.PointReward;
        Main = main;
    }
/*
    public Mission(FactionsMain main, Dictionary<String,Object> config) {
        Main = main;
        name = (String)config.get("name");
        desc = (String)config.get("desc");
        id = (int)config.get("id");
        enabled = (bool) config.get("enabled");
        Dictionary<String,Object> requirement = (Dictionary<String,Object>) config.get("requirement");
        Dictionary<String,Object> reward = (Dictionary<String,Object>) config.get("reward");
        if (requirement != null) {
            //break
            if (requirement.containsKey("break")) {
                Dictionary<String,Object> brk = (Dictionary<String,Object>) requirement.get("break");
                if (brk != null && brk.entrySet().size() > 0) {
                    for (Map.Entry<String, Object> a : brk.entrySet()) {
                        String key = a.getKey() + "";
                        int val = (int) a.getValue();
                        Break.put(key, val);
                    }
                }
            }
            //place
            if (requirement.containsKey("place")) {
                Dictionary<String,Object> plc = (Dictionary<String,Object>) requirement.get("place");
                if (plc != null && plc.entrySet().size() > 0) {
                    for (Map.Entry<String, Object> a : plc.entrySet()) {
                        String key = a.getKey() + "";
                        int val = (int) a.getValue();
                        Place.put(key, val);
                    }
                }
            }
            //item
            if (requirement.containsKey("item")) {
                Dictionary<String,Object> itm = requirement.getSection("item");
                if (itm.entrySet().size() > 0) {
                    for (Map.Entry<String, Object> a : itm.entrySet()) {
                        String key = a.getKey();
                        int val = (int) a.getValue();

                        int bid = 0;
                        int bmeta = 0;
                        int bcount = 0;
                        if (key.contains("|")) {
                            bid = int.parseInt(key.split("\\|")[0]);
                            bmeta = int.parseInt(key.split("\\|")[1]);
                        } else {
                            bid = int.parseInt(key);
                        }
                        bcount = (int) a.getValue();
                        Item i = Item.get(bid, bmeta, bcount);
                        ItemReq.add(i);
                    }
                }
            }
            //kill
            if (requirement.containsKey("kill")) Kill = (int) requirement.get("kill");
        }
        if (reward != null) {
            XPReward = reward.getInt("xp");
            PointReward = reward.getInt("point");
            MoneyReward = reward.getInt("money");
            //item
            if (reward.containsKey("item")) {
                Dictionary<String,Object> itm = reward.getSection("item");
                if (itm.entrySet().size() > 0) {
                    for (Map.Entry<String, Object> a : itm.entrySet()) {
                        String key = a.getKey();
                        int val = (int) a.getValue();

                        int bid = 0;
                        int bmeta = 0;
                        int bcount = 0;
                        if (key.contains("|")) {
                            bid = int.parseInt(key.split("\\|")[0]);
                            bmeta = int.parseInt(key.split("\\|")[1]);
                        } else {
                            bid = int.parseInt(key);
                        }
                        bcount = (int) a.getValue();
                        Item i = Item.get(bid, bmeta, bcount);
                        ItemReward.add(i);
                    }
                }
            }

        }
    }
    
    */

    public List<Item> getItemReward() {
        return ItemReward;
    }

    public void setItemReward(List<Item> itemReward) {
        ItemReward = itemReward;
    }
    }
}