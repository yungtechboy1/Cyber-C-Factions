using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Factions2;
using MiNET;
using MiNET.Items;
using MiNET.Net;
using MiNET.Utils;
using MiNET.Worlds;
using Newtonsoft.Json.Linq;

namespace Faction2
{
    public class ActiveMission : Mission
    {
        public Faction _Faction { get; set; }
        public JObject BreakCount { get; set; }
        public JObject PlaceCount { get; set; }
        public int KillCount = 0;

        public ActiveMission(Faction_main _main, Mission mission) : base(_main, mission)
        {
        }

        public ActiveMission(Faction_main main, Faction fac, JObject cfg) : base(main, cfg)
        {
            if (cfg["BreakCount"] != null) BreakCount = (JObject) cfg["BreakCount"];
            if (cfg["PlaceCount"] != null) BreakCount = (JObject) cfg["PlaceCount"];
            if (cfg["KillCount"] != null) BreakCount = (JObject) cfg["KillCount"];
            _Faction = fac;
        }

        public ActiveMission(Faction_main main, Faction fac, Mission mission) : base(main, mission)
        {
            _Faction = fac;
        }

        public int GetKills()
        {
            return KillCount;
        }

        public void AddKill()
        {
            KillCount++;
        }

        public Boolean CheckBreak()
        {
            foreach (KeyValuePair<string, int> property in Break)
            {
                int min = property.Value;
                if (BreakCount[property.Key] != null)
                {
                    int current = (int) BreakCount[property.Key];
                    if (current < min) return false;
                }
                else
                {
                    return false;
                }
            }
            return true;
        }

        public Boolean CheckPlace()
        {
            foreach (KeyValuePair<string, int> property in Place)
            {
                int min = property.Value;
                if (PlaceCount[property.Key] != null)
                {
                    int current = (int) PlaceCount[property.Key];
                    if (current < min) return false;
                }
                else
                {
                    return false;
                }
            }
            return true;
        }

        public void BreakBlock(BlockBreakEventArgs ev)
        {
            int id = ev.Block.Id;
            int meta = ev.Block.Metadata;
            String key;
            if (meta != 0)
            {
                key = id + "|" + meta;
            }
            else
            {
                key = id + "";
            }
            if (Break.ContainsKey(key))
            {
                if (BreakCount[key] != null)
                {
                    int c = (int) BreakCount[key] + 1;
                    BreakCount[key] = c;
                }
                else
                {
                    BreakCount[key] = 1;
                }
            }
            CheckCompletion();
        }

        public void PlaceBlock(BlockPlaceEventArgs ev)
        {
            int id = ev.TargetBlock.Id;
            int meta = ev.TargetBlock.Metadata;
            String key;
            if (meta != 0)
            {
                key = id + "|" + meta;
            }
            else
            {
                key = id + "";
            }
            if (Place.ContainsKey(key))
            {
                if (PlaceCount[key] != null)
                {
                    int c = (int) PlaceCount[key] + 1;
                    PlaceCount[key] = c;
                }
                else
                {
                    PlaceCount[key] = 1;
                }
            }
            //TODO CREATE A TICK CLASS TO ONLY TICK ACTIVE MISSION CHECKCOMPLETION
            CheckCompletion();
        }

        public String PlaceBlockStatus()
        {
            String fnl = ChatColors.Gray + "--------Place Block Status------\n";
            foreach (KeyValuePair<string, int> a in Place)
            {
                int min = a.Value;
                Item c;
                if (a.Key.Contains("|"))
                {
                    int bid = int.Parse(a.Key.Split(new[] {"|"}, StringSplitOptions.RemoveEmptyEntries)[0]);
                    int bmeta = int.Parse(a.Key.Split(new[] {"|"}, StringSplitOptions.RemoveEmptyEntries)[1]);
                    c = ItemFactory.GetItem((short) bid, (short) bmeta);
                }
                else
                {
                    c = ItemFactory.GetItem((short) int.Parse(a.Key));
                }
                if (PlaceCount[a.Key] != null)
                {
                    int current = (int) PlaceCount[a.Key];
                    if (current >= min)
                    {
                        fnl = fnl + ChatColors.Gray + "[" + ChatColors.Green + "X" + ChatColors.Gray + "]" +
                              "  " + ChatColors.Aqua + c.GetType().FullName +
                              ChatColors.Gray + " | " +
                              ChatColors.Green + current +
                              ChatColors.Gray + " / " + ChatColors.Green + min + "\n";
                    }
                    else
                    {
                        fnl = fnl + ChatColors.Gray + "[" + ChatColors.Red + "-" + ChatColors.Gray + "]" +
                              "  " + ChatColors.Aqua + c.GetType().FullName +
                              ChatColors.Gray + " | " +
                              ChatColors.Yellow + current +
                              ChatColors.Gray + " / " + ChatColors.Yellow + min + "\n";
                    }
                }
                else
                {
                    fnl = fnl + ChatColors.Gray + "[" + ChatColors.Red + "-" + ChatColors.Gray + "]" +
                          "  " + ChatColors.Aqua + c.GetType().FullName +
                          ChatColors.Gray + " | " +
                          ChatColors.Red + 0 +
                          ChatColors.Gray + " / " + ChatColors.Yellow + min + "\n";
                }
            }
            return fnl;
        }

        public String BreakBlockStatus()
        {
            String fnl = ChatColors.Gray + "--------Break Block Status------\n";
            foreach (KeyValuePair<string, int> a in Break)
            {
                int min = a.Value;
                Item c;
                if (a.Key.Contains("|"))
                {
                    int bid = int.Parse(a.Key.Split(new[] {"|"}, StringSplitOptions.RemoveEmptyEntries)[0]);
                    int bmeta = int.Parse(a.Key.Split(new[] {"|"}, StringSplitOptions.RemoveEmptyEntries)[1]);
                    c = ItemFactory.GetItem((short) bid, (short) bmeta);
                }
                else
                {
                    c = ItemFactory.GetItem((short) int.Parse(a.Key));
                }
                if (BreakCount[a.Key] != null)
                {
                    int current = (int) BreakCount[a.Key];
                    if (current >= min)
                    {
                        fnl = fnl + ChatColors.Gray + "[" + ChatColors.Green + "X" + ChatColors.Gray + "]" +
                              "  " + ChatColors.Aqua + c.GetType().FullName +
                              ChatColors.Gray + " | " +
                              ChatColors.Green + current +
                              ChatColors.Gray + " / " + ChatColors.Green + min + "\n";
                    }
                    else
                    {
                        fnl = fnl + ChatColors.Gray + "[" + ChatColors.Red + "-" + ChatColors.Gray + "]" +
                              "  " + ChatColors.Aqua + c.GetType().FullName +
                              ChatColors.Gray + " | " +
                              ChatColors.Yellow + current +
                              ChatColors.Gray + " / " + ChatColors.Yellow + min + "\n";
                    }
                }
                else
                {
                    fnl = fnl + ChatColors.Gray + "[" + ChatColors.Red + "-" + ChatColors.Gray + "]" +
                          "  " + ChatColors.Aqua + c.GetType().FullName +
                          ChatColors.Gray + " | " +
                          ChatColors.Red + 0 +
                          ChatColors.Gray + " / " + ChatColors.Yellow + min + "\n";
                }
            }
            return fnl;
        }

        public String ItemStatus()
        {
            String fnl = ChatColors.Gray + "--------Item Status--------\n";
            Dictionary<String, int> map = new Dictionary<string, int>();
            foreach (KeyValuePair<String, Player> a in _main.Server.GetOnlinePlayers())
            {
                if (!_Faction.IsInFaction(a.Value))
                {
                    continue;
                }
                PlayerInventory inv = a.Value.Inventory;
                foreach (Item b in inv.Slots)
                {
                    foreach (Item c in ItemReq)
                    {
                        //TODO CHeck this! Might need to create my own equals function!
                        //Might work!
                        if (b._Equals(c))
                        {
                            //TODO FIX THE GETDAMAGE() bug Where b.`GetDamage()`
                            //TODO NEEDS TO BE METADATA NOT GETDAMAGE!!!
                            //BUG HUGE
                            //IMPORTANT WOWO
                            //IM HIGH
                            //OFF LYFE
                            //AND LYFT!
                            String key = b.Id + "|" + b.Metadata;
                            if (map.ContainsKey(key)) map[key] = (map[key] + b.Count);
                            else map.Add(key, b.Count);
                        }
                    }
                }
            }
            foreach (Item c in ItemReq)
            {
                //So how do i want to do this loop system?
                //I could do a loop to find the item and use the continue process
                String key = c.Id + "|" + c.Metadata;
                if (map.ContainsKey(key))
                {
                    //   FOUND IN INV / NEED 5
                    //              10/2
                    //              a = 5
                    float a = map[key]/c.Count;
                    if (a > 1) a = 1;
                    int v = (int) Math.Round(a*100d);
                    // [X]  TEST | 10 / 100
                    if (v == 100)
                    {
                        //[-] ITEM NAME | 10 / 10
                        //U+2714
                        fnl = fnl + ChatColors.Gray + "[" + ChatColors.Red + "✔" + ChatColors.Gray + "]" +
                              "  " + ChatColors.Aqua + c.GetType().FullName +
                              ChatColors.Gray + " | " +
                              ChatColors.Green + map[key] +
                              ChatColors.Gray + " / " + ChatColors.Green + c.Count + "\n";
                    }
                    else
                    {
                        //U+2715
                        fnl = fnl + ChatColors.Gray + "[" + ChatColors.Green + "✕" + ChatColors.Gray + "]" +
                              "  " + ChatColors.Aqua + c.GetType().FullName +
                              ChatColors.Gray + " | " +
                              ChatColors.Yellow + map[key] +
                              ChatColors.Gray + " / " + ChatColors.Red + c.Count + "\n";
                    }
                    ;
                }
                else
                {
                    fnl = fnl + ChatColors.Gray + "[" + ChatColors.Red + "✕" + ChatColors.Gray + "]" +
                          "  " + ChatColors.Aqua + c.GetType().FullName +
                          ChatColors.Gray + " | " +
                          ChatColors.Red + 0 +
                          ChatColors.Gray + " / " + ChatColors.Red + c.Count + "\n";
                }
            }
            return fnl;
        }

        public Item CheckPlayerItems()
        {
            Dictionary<String, int> map = new Dictionary<string, int>();
            if (_main == null)
            {
                //Todo Take out.... I think this is useless!
                Console.WriteLine("WTF!!??!?! Really!");
                return ItemFactory.GetItem(1);
            }
            foreach (KeyValuePair<string, Player> a in _main.Server.GetOnlinePlayers())
            {
                if (!_Faction.IsInFaction(a.Value)) continue;
                PlayerInventory inv = a.Value.Inventory;
                foreach (Item b in inv.Slots)
                {
                    foreach (Item c in ItemReq)
                    {
                        if (b._Equals(c))
                        {
                            String key = b.Id + "|" + b.Metadata;
                            if (map.ContainsKey(key)) map[key] = (map[key] + b.Count);
                            else map.Add(key, b.Count);
                        }
                    }
                }
            }
            Boolean fail = false;
            foreach (Item c in ItemReq)
            {
                String key = c.Id + "|" + c.Metadata;
                if (map.ContainsKey(key))
                {
                    float a = map[key]/c.Count;
                    if (a > 1) a = 1;
                    int v = (int) Math.Round(a*100f);
                    if (v != 100) return c;
                }
                else
                {
                    return c;
                }
            }
            return null;
        }

        public int CheckCompletion()
        {
            return CheckCompletion(false);
        }

        public int CheckCompletion(Boolean checkitems)
        {
            if (checkitems && CheckPlayerItems() != null)
            {
                return 1;
            }
            if (!CheckPlace())
            {
                return 2;
            }
            if (!CheckBreak())
            {
                return 3;
            }
            if (Kill < KillCount)
            {
                return 4;
            }
            if (_Faction == null)
            {
                Console.WriteLine("FACTION NUL!?!??!?! WTFaaa");
                return 5;
            }
            if (!GiveReward()) return 6; //ERROR GIVING REWARD
            _Faction.CompleteMission(this);
            String msg = ChatColors.Green + name + " Mission completed! You rewards have been given!";
            _Faction.BroadcastMessage(Faction_main.NAME + msg);
            GiveReward();
            return 0;
        }

        public bool GiveReward()
        {
            if (ItemReward.Length > 0)
            {
                foreach (KeyValuePair<String, Player> a in _main.Server.GetOnlinePlayers())
                {
                    if (!_Faction.IsInFaction(a.Value)) continue;

                    if (ItemRewardType == 0)
                    {
                        //Check if leader is online

                        if (_Faction != null)
                        {
                            if (_Faction.GetOnlinePlayers().Contains(_Faction.GetLeader()))
                            {
                                Player leader = _main.Server.LevelManager.FindPlayer(_Faction.GetLeader());
                                if (leader == null)
                                {
                                    _Faction.BroadcastMessage(
                                        "ERROR! A problem has occuRed while attempting to reward your leader! Error Code: 312");
                                    _Faction.BroadcastMessage("ERROR! Please Contact an Admin if problem persists!");
                                    Console.WriteLine("ERROR!: P312: Leader should be online and found!");
                                    return false;
                                }
                                foreach (Item I in ItemReward) leader.Inventory.SetFirstEmptySlot(I, true);
                                break;
                            }
                            else
                            {
                                //No Leader online
                                _Faction.BroadcastMessage("ERROR! Only your faction leader can collect the reward!");
                                return false;
                            }
                            
                        }else
                        {
                            Console.WriteLine("ERROR!: P2112: Mission does not have a faction associated with it!");
                            return false;
                        }
                        
                    }
                    //Only 1 random person in fac gets it!
                    if (ItemRewardType == 1)
                    {
                        foreach (Item I in ItemReward) a.Value.Inventory.SetFirstEmptySlot(I, true);
                        break;
                    }
                    if (ItemRewardType == 2)
                    {
                        foreach (Item I in ItemReward) a.Value.Inventory.SetFirstEmptySlot(I, true);
                    }
                }
            }
            if (MoneyReward != 0) _Faction.AddMoney(MoneyReward);
            if (XPReward != 0) _Faction.AddXp(XPReward);
            if (PointReward != 0) _Faction.AddXp(PointReward);
            return true;
        }

        public JObject ToHashMap()
        {
            JObject config = new JObject();
            config.Add("name", name);
            config.Add("desc", desc);
            config.Add("id", id);
            config.Add("enabled", enabled);
            JObject bb = new JObject();
            JObject ii = new JObject();
            JObject iii = new JObject();
            JObject pp = new JObject();
            JObject requirement = new JObject();
            JObject reward = new JObject();

            if (Break.Count > 0)
            {
                foreach (KeyValuePair<String, int> a in Break)
                {
                    String key = a.Key;
                    int val = a.Value;
                    bb.Add(key, val);
                }
            }
            //place
            if (Place.Count > 0)
            {
                foreach (KeyValuePair<String, int> a in Place)
                {
                    String key = a.Key;
                    int val = a.Value;
                    pp.Add(key, val);
                }
            }
            //item
            if (ItemReq.Length > 0)
            {
                foreach (Item a in ItemReq)
                {
                    String key = "";
                    if (a.GetDamage() != 0)
                    {
                        key = a.Id + "|" + a.GetDamage();
                    }
                    else
                    {
                        key = a.Id + "";
                    }
                    ii.Add(key, a.Count);
                }
            }
            requirement.Add("item", ii);

            reward.Add("point", PointReward);
            reward.Add("irt", ItemRewardType);
            reward.Add("money", MoneyReward);
            reward.Add("xp", XPReward);
            reward.Add("item", iii);
            //item
            if (ItemReward.Length > 0)
            {
                foreach (Item a in ItemReward)
                {
                    String key = a.Id + "|" + a.Metadata + "|" + a.ExtraData;
                    iii.Add(key, a.Count);
                }
            }
            config.Add("BreakCount", BreakCount);
            config.Add("PlaceCount", PlaceCount);
            config.Add("KillCount", KillCount);


            requirement.Add("break", bb);
            requirement.Add("place", pp);
            config.Add("requirement", requirement);
            config.Add("reward", reward);
            requirement.Add("kill", Kill);


            return config;
        }
    }
}