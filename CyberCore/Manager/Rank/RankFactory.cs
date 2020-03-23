﻿using System;
using System.Collections.Generic;
using CyberCore.Utils;
using MiNET;

namespace CyberCore.Manager.Rank
{
    public class RankFactory
    {
        public String DB_TABLE = "mcpe";

        public Dictionary<String, int> RankCache = new Dictionary<String, int>();
        CyberCoreMain Main;
        public ConfigSection GARC = new ConfigSection();
        public ConfigSection MRC = new ConfigSection();
        public ConfigSection SRC = new ConfigSection();

        public Dictionary<int, Rank> ranks = new Dictionary<int, Rank>();

        public RankFactory(CyberCoreMain main)
        {
            Main = main;
            loadRanks();
        }

        public void loadDefault()
        {
            ranks.put(RankList.PERM_GUEST.getID(), new Guest_Rank());
        }

        public void loadRanks()
        {
            Main.getLogger().info("Loading Ranks...");
            loadDefault();
            Config rankConf = new Config(new File(Main.getDataFolder(), "ranks.yml"));
            Dictionary<String, Object> Dictionary = rankConf.getSection("Ranks").getAllDictionary();
            for (String s :
            Dictionary.keySet()) {
                Main.getLogger().info("-===" + s + "===-");
                int index = rankConf.getInt("Ranks." + s + ".rank");
                String display = rankConf.getString("Ranks." + s + ".display_name");
                String chat_prefix = rankConf.getString("Ranks." + s + ".chat_prefix");
                if (!ranks.containsKey(index))
                {
//                Rank data = new Rank(index, display, chat_prefix);
//                ranks.put(index, data);
//                Main.getLogger().info("Rank: " + s + " [" + index + "]- loaded...");
                }
                else
                {
                    Rank rank = ranks.get(index);
                    rank.display_name = display;
                    rank.chat_prefix = chat_prefix;
                    Main.getLogger().info("Rank: " + s + " [" + index + "]- Updated!...");
                }
            }
        }

        public Rank getPlayerRank(Player p)
        {
            String uuid = p.ClientUuid.ToString();
            if (uuid == null) return new Guest_Rank();
            if (RankCache.ContainsKey(uuid))
            {
                var rid = RankCache[uuid];
                if(!ranks.ContainsKey(RankCache[uuid]))return new Guest_Rank();
                var r = ranks[rid];
                return r;
            }
            return new Guest_Rank())
            ;
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
        public int RankAdminRank(String rank)
        {
            if (rank == null)
            {
                return 0;
            }
            else if (rank.equalsIgnoreCase("TMOD"))
            {
                return 1;
            }
            else if (rank.equalsIgnoreCase("MOD1") || rank.equalsIgnoreCase("yt"))
            {
                return 2;
            }
            else if (rank.equalsIgnoreCase("MOD2"))
            {
                return 3;
            }
            else if (rank.equalsIgnoreCase("MOD3"))
            {
                return 4;
            }
            else if (rank.equalsIgnoreCase("ADMIN1"))
            {
                return 5;
            }
            else if (rank.equalsIgnoreCase("ADMIN2"))
            {
                return 6;
            }
            else if (rank.equalsIgnoreCase("ADMIN3"))
            {
                return 7;
            }
            else if (rank.equalsIgnoreCase("OP"))
            {
                return 8;
            }

            return 0;
        }
    }
}