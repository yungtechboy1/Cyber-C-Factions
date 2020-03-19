using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection.Metadata.Ecma335;
using CyberCore.Manager.Factions;
using CyberCore.Manager.Warp;
using MiNET;
using MiNET.Plugins;

namespace CyberCore.Utils.Data
{
    public class ServerSqlite
    {
        public CyberCoreMain Plugin;


        public ServerSqlite(CyberCoreMain plugin)
        {
            Plugin = plugin;
        }


        /**
     * No Need to load warps from SQL... These are global Warps too
     */
        public void LoadAllWarps()
        {
            List<Dictionary<String, Object>> data = Plugin.SQL.executeSelect("SELECT * FROM `Warps`");
            if (data == null)
            {
                CyberCoreMain.Log.Error("Error Loading Warps from Sqlite!");
                return;
            }
            else
            {
                CyberCoreMain.Log.Info("Loading " + data.Count + " Warps!");
            }


            foreach (var v in data)
            {
                Plugin.WarpManager.AddWarp(new WarpData((String) v["name"], (double) v["x"],
                    (double) v["y"],
                    (double) v["z"], (String) v["level"]));
            }
        }

        public void LoadPlayer(Player p)
        {
            if (p == null) Console.WriteLine("PLAYER NULL");
            try
            {
                LoadHomes(p);
                LoadSettings(p);
//            LoadRank(p);
                LoadClass(p);
                Faction f = Plugin.FM.FFactory.getPlayerFaction(p);
                if (f != null)
                {
                    Plugin.FM.FFactory.FacList.Add(p.getName().ToLower(),f.getName());
                    // p. = f.getName();
                }
            }
            catch (Exception e)
            {
                CyberCoreMain.Log.Error("EEEEE11122223333", e);
            }
        }

        private void LoadClass(Player p)
        {
            Plugin.ClassFactory.GetClass(p, true);
        }

//    private void LoadRank(Player p) {
//        try {
//            List<Dictionary<String, Object>> data = executeSelect("SELECT * FROM `Ranks` WHERE `uuid` LIKE '" + p.getUniqueId() + "'");
//            if (data == null || data.size() == 0) {
//                CyberCoreMain.getInstance().getLogger().error("No Ranks found for "+p.getName());
//                p.SetRank(RankList.PERM_GUEST);
//                return;
//            } else {
//                Plugin.getLogger().info("Loading " + data.size() + " Ranks!");
//            }
//
//            for (Dictionary<String, Object> v : data) {
//                String rn = (String)v.get("rank");
//                if(rn.equalsIgnoreCase(RankList.PERM_GUEST.getName())){
//                    if(p.GetRank().getId() < RankList.PERM_GUEST.getID()){
//                        p.SetRank(RankList.PERM_GUEST);
//                    }
//                }else if(rn.equalsIgnoreCase(RankList.PERM_MEMBER.getName())){
//                    if(p.GetRank().getId() < RankList.PERM_MEMBER.getID()){
//                        p.SetRank(RankList.PERM_MEMBER);
//                    }
//                }else if(rn.equalsIgnoreCase(RankList.PERM_OP.getName())){
//                    if(p.GetRank().getId() < RankList.PERM_OP.getID()){
//                        p.SetRank(RankList.PERM_OP);
//                    }
//                }else if(rn.equalsIgnoreCase(RankList.PERM_VIP.getName())){
//                    if(p.GetRank().getId() < RankList.PERM_VIP.getID()){
//                        p.SetRank(RankList.PERM_VIP);
//                    }
//                }
//            }
//
//        } catch (SQLException e) {
//            e.printStackTrace();
//        }
//        p.SetRank(RankList.PERM_GUEST);
//        return;
//    }

//    public void UnLoadPlayer(Player p) {
//        SaveHomes((Player) p);
//        SaveSettings((Player) p);
//    }

        public void UnLoadPlayer(Player p)
        {
            SaveHomes(p);
            SaveSettings(p);
            Plugin.ClassFactory.SaveClassToFile(p);
        }

        private void LoadHomes(Player p)
        {
            try
            {
                List<Dictionary<String, Object>> data =
                    executeSelect("SELECT * FROM `PlayerHomes` WHERE `owneruuid` LIKE '" + p.getUniqueId() + "'");
                if (data == null)
                {
                    CyberCoreMain.getInstance().getLogger().error("Error Loading Warps from Sqlite!");
                    return;
                }
                else
                {
                    Plugin.getLogger().info("Loading " + data.size() + " Warps!");
                }

                for (Dictionary < String, Object > v : data)
                {
                    p.AddHome(new Faction.HomeData((String) v.get("name"), (Double) v.get("x"), (Double) v.get("y"),
                        (Double) v.get("z"), (String) v.get("level"), p));
                }
            }
            catch (SQLException e)
            {
                e.printStackTrace();
            }
        }

        private void LoadSettings(Player p)
        {
            UserSQL u = Plugin.UserSQL;
            Plugin.getLogger().info("Starting loading " + p.getName() + "'s Server Data...Maybe");
            PlayerSettingsData pd = u.getPlayerSettingsData(p);
            p.setPlayerSettingsData(pd);
        }

        private void SaveHomes(Player p)
        {
            try
            {
                executeUpdate("DELETE FROM `PlayerHomes` WHERE `owneruuid` LIKE '" + p.getUniqueId() + "'");
                for (Faction.HomeData h :
                p.HD) {
                    executeUpdate("INSERT INTO `PlayerHomes` VALUES (0,'" + h.getName() + "'," + h.getX() + "," +
                                  h.getY() +
                                  "," + h.getZ() + ",'" + h.getLevel() + "','" + h.getOwner() + "','" +
                                  h.getOwneruuid() +
                                  "')");
                }
                Plugin.getLogger().info("Homes saved for " + p.getName());
//            p.sendTip("Homes Saved!");
            }
            catch (SQLException e)
            {
                e.printStackTrace();
            }
        }

        private void SaveSettings(Player p)
        {
            UserSQL u = Plugin.UserSQL;
            Plugin.getLogger().info("Starting SAVING FOR  " + p.getName() + "'s Server Data...Maybe");
            u.savePlayerSettingData(p);
        }
    }
}