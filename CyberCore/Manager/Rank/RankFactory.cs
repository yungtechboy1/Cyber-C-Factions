using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using CyberCore.CustomEnums;
using CyberCore.Manager.Factions;
using CyberCore.Utils;
using CyberCore.Utils.Cooldowns;
using log4net;
using MiNET;
using Newtonsoft.Json.Linq;

namespace CyberCore.Manager.Rank
{
    public class RankFactory
    {
        public String DB_TABLE = "mcpe";

        //TODO Remove on leave
        public Dictionary<String, Rank2> RankCache = new Dictionary<String, Rank2>();
        public Dictionary<String, CoolDown> RankCacheCooldown = new Dictionary<String, CoolDown>();
        CyberCoreMain Main;
        public RankList2 List;

        public Dictionary<int, Rank2> ranks = new Dictionary<int, Rank2>();

        public RankFactory(CyberCoreMain main, RankFactoryData r = null)
        {
            if (r == null) r = new RankFactoryData();
            Main = main;
            List = new RankList2(main);
            // loadRanks(r);
        }
        //
        // public void loadDefault()
        // {
        //     ranks[RankList.PERM_GUEST.getID()] =  RankList2.GuestRank;
        // }

        public static ILog Log { get; private set; } = LogManager.GetLogger(typeof(RankFactory));

        public void loadRanks(RankFactoryData r)
        {
            CyberCoreMain.Log.Info("Loading Ranks...");
        }

        /// <summary>
        /// CAN NOT EQUAL 0
        /// </summary>
        /// <param name="name"></param>
        /// <returns>0 for No and any value above 1 for true</returns>
        public int getUserIDFromMCPEName(String name)
        {
            var a = Main.WebSQL.executeSelect(
                // $"SELECT * FROM `xf_user_field_value` WHERE `field_value` LIKE '{name}' AND `field_id` = CAST(6d6370656964 AS BINARY)");
                $"SELECT * FROM `xf_user_field_value` WHERE `field_value` LIKE '{name}' AND `field_id` = 'mcpeid'");
            // CyberCoreMain.Log.Error("WHOAAAA WTF IS THIS!!!::" + a.Count);
            if (a.Count > 0)
            {
                return a[0].GetInt32("user_id");
            }
            // CyberCoreMain.Log.Error("WHOAAAA WTF IS THIS!!!::" + a.Count);
            return 0;
        }

        public int getUserIDFromMCPEUUID(String uuid)
        {
            var a = Main.WebSQL.executeSelect(
                $"SELECT * FROM `xf_user_field_value` WHERE `field_value` LIKE '{uuid}' AND `field_id` = CAST(0x6d63706575756964 AS BINARY)");
            if (a.Count > 0)
            {
                return a[0].GetInt32("user_id");
            }

            return 0;
        }

        public List<int> getRankIDsFromUserID(int id)
        {
            var l = new List<int>();
            if (id == 0) return l;
            var a = Main.WebSQL.executeSelect("SELECT * FROM `xf_user` WHERE `user_id` = " + id);
            if (a.Count != 0)
            {
                l.Add(a.GetInt32("user_group_id"));
                var z = a.GetString("secondary_group_ids");
                if (z.Length > 0)
                {
                    var zz = z.Split(",");
                    foreach (var zzz in zz)
                    {
                        int outt;
                        if (int.TryParse(zzz, out outt))
                        {
                            l.Add(outt);
                        }
                    }
                }
            }

            return l;
        }

        public Rank2 getPlayerRank(CorePlayer p)
        {
            if (p == null) return RankList2.getInstance().getRankFromID(RankEnum.Guest);
            String uuid = p.ClientUuid.ToString();
            String name = p.getName().ToLower();
            if (RankCache.ContainsKey(name))
            {
                if (RankCacheCooldown.ContainsKey(name))
                {
                    var r = RankCacheCooldown[name];
                    if (r.isValid()) return RankCache[name];
                    RankCacheCooldown.Remove(name);
                    RankCache.Remove(name);
                }
                else
                {
                    addCooldownToPlayer(name);
                    return RankCache[name];
                }
            }

            var uid1 = getUserIDFromMCPEName(name);
            var uid2 = getUserIDFromMCPEUUID(uuid);
            
            Rank2 hr = null;
            if (uid1 != 0 || uid2 != 0)
            {
                CyberCoreMain.Log.Warn($"{uid1} ||||| {uid2}");
                var a = getRankIDsFromUserID(uid1);
                CyberCoreMain.Log.Warn($"{uid1} ||||| {uid2} ||||| {a.Count}");
                if (uid1 != uid2 && uid2 != 0)
                {
                    var aa = getRankIDsFromUserID(uid2);
                    a.AddRange(aa);
                }

                var aaa = a.Distinct().ToList(); //Removes Duplicates
                var rr = getAllRanksFromIntList(aaa);
                hr = getHigestRankFromList(rr);
            }

            if (hr == null) hr = RankList2.getInstance().getRankFromID(RankEnum.Guest);
            RankCache[name] = hr;
            addCooldownToPlayer(name);
            return hr;
            
        }
        public Rank2 getPlayerRank(String p)
        {
            if (p == null) return RankList2.getInstance().getRankFromID(RankEnum.Guest);
            String name = p.ToLower();
            if (RankCache.ContainsKey(name))
            {
                if (RankCacheCooldown.ContainsKey(name))
                {
                    var r = RankCacheCooldown[name];
                    if (r.isValid()) return RankCache[name];
                    RankCacheCooldown.Remove(name);
                    RankCache.Remove(name);
                }
                else
                {
                    addCooldownToPlayer(name);
                    return RankCache[name];
                }
            }

            var uid1 = getUserIDFromMCPEName(name);
            Rank2 hr = null;
            if (uid1 != 0)
            {
                var a = getRankIDsFromUserID(uid1);


                var aaa = a.Distinct().ToList(); //Removes Duplicates
                var rr = getAllRanksFromIntList(aaa);
                hr = getHigestRankFromList(rr);

            }

            if (hr == null) hr = RankList2.getInstance().getRankFromID(RankEnum.Guest);
            RankCache[name] = hr;
            addCooldownToPlayer(name);
            return hr;
            
        }

        public Rank2 getHigestRankFromList(List<Rank2> l)
        {
            Rank2 fr = null;
            foreach (var r in l)
            {
                if (fr == null)
                {
                    fr = r;
                    continue;
                }

                if (fr.Weight < r.Weight) fr = r;
            }

            return fr;
        }

        public List<Rank2> getAllRanksFromIntList(List<int> l)
        {
            var r = new List<Rank2>();
            foreach (int a in l)
            {
                var v = List.getRankFromID(a);
                if (v != null) r.Add(v);
            }

            return r;
        }

        public void addCooldownToPlayer(String name)
        {
            RankCacheCooldown[name] = getDefaultCooldown();
        }
        
        public CoolDown getDefaultCooldown()
        {
            return new CoolDown(60 * 5); //5 Mins
        }

        public String GetRankStringFromGroup(String group)
        {
            String r = null;
            if (group.equalsIgnoreCase("11")) r = "steve+";
            if (group.equalsIgnoreCase("9")) r = "hero";
            if (group.equalsIgnoreCase("12")) r = "vip";
            if (group.equalsIgnoreCase("12")) r = "vip+";
            if (group.equalsIgnoreCase("10")) r = "legend";
            if (group.equalsIgnoreCase("17")) r = "mod1";
            if (group.equalsIgnoreCase("18")) r = "mod2";
            if (group.equalsIgnoreCase("19")) r = "mod3";
            //if(group.equalsIgnoreCase("18"))r = "mod4";
            //if(group.equalsIgnoreCase("19"))r = "mod5";
            if (group.equalsIgnoreCase("14")) r = "admin1";
            if (group.equalsIgnoreCase("15")) r = "admin2";
            if (group.equalsIgnoreCase("16")) r = "admin3";
            if (group.equalsIgnoreCase("6")) r = "tmod";
            if (group.equalsIgnoreCase("7")) r = "tmod";
            //if(group.equalsIgnoreCase("11"))r = "tmod";
            //if(group.equalsIgnoreCase("13"))r = "scrub";
            if (group.equalsIgnoreCase("3")) r = "op";
            if (group.equalsIgnoreCase("8")) r = "op";
            if (group.equalsIgnoreCase("24")) r = "op";
            if (group.equalsIgnoreCase("25")) r = "yt";
            if (group.equalsIgnoreCase("28")) r = "adventurer";
            if (group.equalsIgnoreCase("29")) r = "conquerer";
            if (group.equalsIgnoreCase("27")) r = "tourist";
            if (group.equalsIgnoreCase("26")) r = "islander";
            //@TODO Add Member and Member+

            return r;
        }


        /**
     * Returns rank from 0 - 8
     * 0 -> No Admin Rank
     * 8 -> OP
     *
     * @param rank Rank Name String
     * @return int
         */
        public int RankAdminRank(string rank)
        {
            if (rank == null)
                return 0;
            if (rank.equalsIgnoreCase("TMOD"))
                return 1;
            if (rank.equalsIgnoreCase("MOD1") || rank.equalsIgnoreCase("yt"))
                return 2;
            if (rank.equalsIgnoreCase("MOD2"))
                return 3;
            if (rank.equalsIgnoreCase("MOD3"))
                return 4;
            if (rank.equalsIgnoreCase("ADMIN1"))
                return 5;
            if (rank.equalsIgnoreCase("ADMIN2"))
                return 6;
            if (rank.equalsIgnoreCase("ADMIN3"))
                return 7;
            if (rank.equalsIgnoreCase("OP")) return 8;

            return 0;
        }
    }
}