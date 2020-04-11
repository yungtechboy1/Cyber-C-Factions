﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Numerics;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CyberCore.Manager.Factions.Data;
using CyberCore.Utils;
using log4net;
using MiNET;
using MiNET.Utils;
using MySql.Data.MySqlClient;
using OpenAPI;
using OpenAPI.Player;

namespace CyberCore.Manager.Factions
{
    public class FactionFactory
    {
        static StringComparer dd = StringComparer.OrdinalIgnoreCase;
        public Dictionary<String, Faction> LocalFactionCache = new Dictionary<String, Faction>(dd);

        public static ILog Log { get; private set; } = LogManager.GetLogger(typeof(FactionFactory));
        public Dictionary<String, String> FacList = new Dictionary<String, String>(dd);

//    public Map<String, String> PlotsList = new TreeMap<>;
        public PlotManager PM;
        public RelationshipManager RM;
        public Dictionary<String, String> allyrequest = new Dictionary<String, String>(dd);
        public Dictionary<String, String> War = new Dictionary<String, String>(dd); //Attacking V Defending
        public Dictionary<String, int> Top = new Dictionary<String, int>(dd);
        public Dictionary<String, int> Rich = new Dictionary<String, int>(dd);
        public FactionsMain Main;

        private Dictionary<String, List<FactionInviteData>>
            InvList = new Dictionary<String, List<FactionInviteData>>(dd);

        private List<String> bannednames = new List<String>()
        {
            ("wilderness"), ("safezone"), ("peace")
        };

        private static FactionFactory instance { get; set; }

        public static FactionFactory GetInstance()
        {
            return instance;
        }

        public FactionFactory(FactionsMain main)
        {
            instance = this;
            Main = main;
            RM = new RelationshipManager(this);
            PM = new PlotManager(this);
        }


        public void addFactionInvite(FactionInviteData fid)
        {
            if (InvList.ContainsKey(fid.getPlayerName()))
            {
                List<FactionInviteData> fidl = InvList[fid.getPlayerName()];
                fidl.Add(fid);
                InvList.Add(fid.getPlayerName(), fidl);
            }
            else
            {
                List<FactionInviteData> fidl = new List<FactionInviteData>();
                fidl.Add(fid);
                InvList.Add(fid.getPlayerName(), fidl);
            }
        }

        public void updatePlayerFactionInvites(String cpn)
        {
            if (InvList.ContainsKey(cpn))
            {
                List<FactionInviteData> fidl = InvList[cpn];
                bool clean = false;
                while (!clean)
                {
                    clean = true;
                    if (fidl.Count == 0) continue;
                    for (int i = 0; i < fidl.Count - 1; i++)
                    {
                        FactionInviteData fid = fidl[i];
                        if (!fid.isValid())
                        {
                            fidl.Remove(fidl[i]);
                            clean = false;
                            break;
                        }
                    }
                }
            }
        }

        public void updatePlayerFactionInvites(Player p)
        {
            updatePlayerFactionInvites(p.Username);
        }

        public void updatePlayerFactionInvites(OpenPlayer cp)
        {
            updatePlayerFactionInvites(cp.Username);
        }

        public List<String> getFactionsPlayerIsInvitedTo(String cpn)
        {
            updatePlayerFactionInvites(cpn);
            List<String> data = new List<String>();
            if (InvList.ContainsKey(cpn))
            {
                List<FactionInviteData> fidl = InvList[cpn];
                foreach (FactionInviteData fid in fidl)
                {
                    if (fid.isValid()) data.Add(fid.getFaction());
                }

                return data;
            }

            return null;
        }

        public List<String> getFactionsPlayerIsInvitedTo(OpenPlayer p)
        {
            return getFactionsPlayerIsInvitedTo(p.Username);
        }

        public List<String> getFactionsPlayerIsInvitedTo(Player cp)
        {
            return getFactionsPlayerIsInvitedTo(cp.Username);
        }

        private FactionsMain getMain()
        {
            return Main;
        }

        // private Server getServer()
        // {
        //     return Server.getInstance();
        // }


        public SqlManager getSQLManager()
        {
            return getMain().CCM.SQL;
        }

        public void RemoveFaction(Faction fac)
        {
            SqlManager c = getSQLManager();
            try
            {
                String name = fac.getName();
                foreach (KeyValuePair<string, FactionRank> v in fac.PlayerRanks)
                {
                    FacList.Remove(v.Key);
                }

                LocalFactionCache.Remove(name);
                c.Insert($"DELETE FROM `allies` WHERE `factiona` LIKE '{name}' OR `factionb` LIKE '{name}';");
                c.Insert($"DELETE FROM `plots` WHERE `faction` LIKE {name};");
                c.Insert($"DELETE FROM `confirm` WHERE `faction` LIKE {name};");
                c.Insert($"DELETE FROM `home` WHERE `faction` LIKE {name};");
                c.Insert($"DELETE FROM `Master` WHERE `faction` LIKE {name};");
                c.Insert($"DELETE FROM `Master` WHERE `faction` LIKE {name};");
            }
            catch (Exception ex)
            {
                CyberCoreMain.Log.Error(ex);
                CyberCoreMain.Log.Error("LINE aSDASD1112222222222222");
            }
        }

        public void SaveAllFactions()
        {
            try
            {
                SqlManager stmt = getSQLManager();
                /*Statement stmt2 = getMySqliteConnection2().createStatement();
                String sql = "CREATE TABLE IF NOT EXISTS \"master\" " +
                        "(player VARCHAR PRIMARY KEY UNIQUE     NOT NULL," +
                        " faction           VARCHAR    NOT NULL, " +
                        " rank            VARCHAR     NOT NULL)";
                String sql1 = "CREATE TABLE IF NOT EXISTS \"allies\" (  " +
                        "                        `id` int NOT NULL PRIMARY KEY AUTOINCREMENT,  " +
                        "                        `factiona` VARCHAR NOT NULL,  " +
                        "                        `factionb` varchar NOT NULL,  " +
                        "                        `timestamp` TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP  " +
                        "                        );";
                String sql2 = "CREATE TABLE IF NOT EXISTS \"confirm\" (" +
                        "`player`TEXT NOT NULL," +
                        "`faction`TEXT NOT NULL," +
                        "`timestamp`int," +
                        "`id`int NOT NULL PRIMARY KEY AUTOINCREMENT" +
                        ")";
                String sql3 = "CREATE TABLE  IF NOT EXISTS \"home\" (" +
                        "                     `faction` varchar NOT NULL UNIQUE, " +
                        "                     `x` int(250) NOT NULL, " +
                        "                     `y` int(250) NOT NULL, " +
                        "                     `z` int(250) NOT NULL, " +
                        "                     PRIMARY KEY(faction) " +
                        "                    );";
                String sql4 = "CREATE TABLE IF NOT EXISTS \"plots\" (  " +
                        "            `id` int NOT NULL PRIMARY KEY AUTOINCREMENT,  " +
                        "            `faction` varchar(250) NOT NULL,  " +
                        "            `x` int(250) NOT NULL,  " +
                        "            `z` int(250) NOT NULL  " +
                        "            );";
                String sql6 = "CREATE TABLE IF NOT EXISTS \"InternalPlayerSettings\" (" +
                        "`faction`varchar(250) NOT NULL UNIQUE," +
                        "`max`int(250) NOT NULL," +
                        "`powerbonus`int(50) NOT NULL," +
                        "`MOTD`varchar(1000) NOT NULL," +
                        "`displayname`varchar(1000) NOT NULL," +
                        "`desc`varchar(1000) NOT NULL," +
                        "`prem`int NOT NULL," +
                        "`privacy`int(11) NOT NULL," +
                        "`power`int NOT NULL," +
                        "`money`int NOT NULL," +
                        "PRIMARY KEY(faction)" +
                        ")";
                //String sql7 = "CREATE TABLE IF NOT EXISTS war ( attacker varchar(250) NOT NULL PRIMARY KEY, defender varchar(250) NOT NULL, start int(250) NOT NULL)";
                stmt2.executeUpdate(sql);
                stmt2.executeUpdate(sql1);
                stmt2.executeUpdate(sql2);
                stmt2.executeUpdate(sql3);
                stmt2.executeUpdate(sql4);
                stmt2.executeUpdate(sql6);*/
                //stmt.executeUpdate("DELETE FROM allies; DELETE FROM confirm; DELETE FROM home; DELETE FROM plots; DELETE FROM InternalPlayerSettings; DELETE FROM master;");
                CyberCoreMain.Log.Info("Going to save: " + LocalFactionCache.Count);
                stmt.Insert("BEGIN;");
                String yaml = "";
                foreach (KeyValuePair<string, Faction> e in LocalFactionCache)
                {
                    try
                    {
                        Faction fac = e.Value;
                        String name = e.Key;
//                    List<String> allies = fac.GetAllies();
//                    List<String> enemies = fac.GetEnemies();
//                    List<String> plots = fac.GetPlots();
////                    Map<String, int> invites = fac.GetInvite();
//                    Vector3 home = fac.GetHome();
//                    String motd = fac.GetMOTD();
//                    String displayName = fac.getDisplayName();
//                    String desc = fac.GetDesc();
//                    FactionPermSettings perms = fac.GetPerm();
//                    int powerbonus = fac.GetPowerBonus();
//                    int privacy = fac.GetPrivacy();
//                    int maxplayers = fac.GetMaxPlayers();
//                    int power = fac.GetPower();
//                    int money = fac.GetMoney();
//
//                    //@TODO
//                    int point = fac.GetPoints();
//                    int xp = fac.GetXP();
//                    int lvl = fac.GetLevel();
//                    int rich = fac.GetRich();
                        String am = "";
//                    if (fac.GetActiveMission() != null) {
//                        am = fac.GetActiveMission().id + "";
//                        try {
//                            ActiveMission a = fac.GetActiveMission();
//                            a.faction = null;
//                            a.Main = null;
//                            Main.AM.set(fac.getName(), a.ToDictionary());
//                         /*Yaml y = new Yaml();
//                        CyberUtils.writeFile(new File(Main.getDataFolder().toString() + "/missions/" + fac.getName() + ".yml"), y.dump(a));
//                        ;
//                        YamlWriter reader = new YamlWriter(new FileWriter(Main.getDataFolder().toString()+"/missions/"+fac.getName()+".yml"));
//                        reader.write(a);
//                        reader.close();*/
//                        } catch (Exception ex) {
//                            getServer().getLogger().info(ex.getClass().getName() + ":9 " + ex.getMessage() + " > " + ex.getStackTrace()[0].getLineNumber() + " ? " + ex.getCause());
//                            am = "";
//                        } /*catch (IOException var8) {
//                        Server.getInstance().getLogger().logException(var8);
//                    }*/
//                    }
                        fac.save();

                        List<int> CMID1 = fac.GetCompletedMissions();
                        String CMID = "";
                        if (CMID1.Count > 1)
                        {
                            bool f = true;
                            foreach (var aa in CMID1)
                            {
                                if (!f)
                                {
                                    CMID = CMID + ",";
                                }
                                else
                                {
                                    f = false;
                                }

                                CMID = CMID + aa;
                            }
                        }
                        else if (CMID1.Count != 0)
                        {
                            foreach (var aa in CMID1)
                            {
                                CMID = CMID + aa;
                            }
                        }

//                    stmt.executeUpdate(String.format("DELETE FROM `allies` WHERE `factiona` LIKE '%s' OR `factionb` LIKE '%s';", name, name));
//                    for (String ally : allies) {
//                        stmt.executeUpdate(String.format("INSERT INTO `allies` VALUES (null,'%s','%s','');", name, ally));
//                    }

//                    stmt.executeUpdate(String.format("DELETE FROM `enemies` WHERE `factiona` LIKE '%s' OR `factionb` LIKE '%s';", name, name));
//                    for (String enemy : enemies) {
//                        stmt.executeUpdate(String.format("INSERT INTO `enemies` VALUES (null,'%s','%s','');", name, enemy));
//                    }

//                    stmt.executeUpdate("DELETE FROM `plots` WHERE `faction` LIKE '" + name + "';");
//                    for (String plot : plots) {
//                        String[] p = plot.split("|");
//                        stmt.executeUpdate(String.format("INSERT INTO `plots` VALUES (null,'%s','%s','%s');", name, p[0], p[1]));
//                    }
//                    stmt.executeUpdate("DELETE FROM `confirm` WHERE `faction` LIKE '" + name + "';");
//                    for (Map.Entry<String, int> ee : invites.entrySet()) {
//                        stmt.executeUpdate(String.format("INSERT INTO `confirm` VALUES ('%s','%s','%s',null);", ee.getKey(), name, ee.getValue()));
//                    }
//
//                    stmt.executeUpdate(String.format("DELETE FROM `home` WHERE `faction` LIKE '%s';", name));
//
//                    if (home != null) {
//                        stmt.executeUpdate(String.format("INSERT INTO `home` VALUES ('" + name + "','%s','%s','%s') ;", home.getX   (), home.getY(), home.getZ()));
//                    }
                        //stmt2.executeUpdate(String.format("DELETE FROM `home` WHERE `faction` LIKE '%s';",name));
                        //stmt2.executeUpdate(String.format("INSERT INTO `home` VALUES ('"+name+"',%s,%s,%s) ;",home.getX(),home.getY(),home.getZ()));
//                    Console.WriteLine(String.format("INSERT INTO `Master` VALUES ('%s','%s','%s','%s','%s','%s','%s','%s','%s','%s','%s','%s','%s','%s','%s','%s');"
//                            , name, maxplayers, powerbonus, motd, displayName, desc, perms, privacy, power, money, point, xp, lvl, CMID, am, rich));
//                    CyberCoreMain.Log.Error(String.format("INSERT INTO `Master` VALUES ('%s','%s','%s','%s','%s','%s','%s','%s','%s','%s','%s','%s','%s','%s','%s','%s');"
//                            , name, maxplayers, powerbonus, motd, displayName, desc, perms, privacy, power, money, point, xp, lvl, CMID, am, rich));
//                    stmt.executeUpdate(String.format("DELETE FROM `Master` WHERE `faction` LIKE '%s';", fac.getName()));
//                    stmt.executeUpdate(String.format("INSERT INTO `Master` VALUES ('%s','%s','%s','%s','%s','%s','%s','%s','%s','%s','%s','%s','%s','%s','%s','%s');"
//                            , name, maxplayers, powerbonus, motd, displayName, desc, perms, privacy, power, money, point, xp, lvl, CMID, am, rich));
//                    //stmt2.executeUpdate(String.format("DELETE FROM `Master` WHERE `faction` LIKE '%s';",name));
//                    //stmt2.executeUpdate(String.format("INSERT INTO `Master` VALUES ('%s','%s','%s','%s','%s','%s','%s','%s','%s','%s'); ",name,maxplayers,powerbonus,motd,displayName,desc,perms,privacy,power,money));
//                    //Saving Members, Leader, And Officers
//                    stmt.executeUpdate(String.format("DELETE FROM `Master` WHERE `faction` = '%s';", name));
//                    for (String member : fac.GetRecruits()) {
//                        stmt.executeUpdate(String.format("DELETE FROM `Master` WHERE `player` = '%s';", member));
//                        stmt.executeUpdate(String.format("INSERT IGNORE INTO `Master`(`player`,`faction`,`rank`) VALUES ('%s','%s','%s');", member, name, "Recruits"));
//                    }
//                    //for(String member: fac.GetRecruits())stmt2.executeUpdate(String.format("INSERT OR IGNORE INTO `Master`(`player`,`faction`,`rank`) VALUES (%s,%s,%s);",member,name,"Recruits"));
//                    for (String member : fac.GetMembers()) {
//                        stmt.executeUpdate(String.format("DELETE FROM `Master` WHERE `player` = '%s';", member));
//                        stmt.executeUpdate(String.format("INSERT IGNORE INTO `Master`(`player`,`faction`,`rank`) VALUES ('%s','%s','%s');", member, name, "Member"));
//                        //for(String member: fac.GetMembers())stmt2.executeUpdate(String.format("INSERT OR IGNORE INTO `Master`(`player`,`faction`,`rank`) VALUES ('%s','%s','%s');",member,name,"Member"));
//                    }
//                    for (String member : fac.GetOfficers()) {
//                        stmt.executeUpdate(String.format("DELETE FROM `Master` WHERE `player` = '%s';", member));
//                        stmt.executeUpdate(String.format("INSERT IGNORE INTO `Master`(`player`,`faction`,`rank`) VALUES ('%s','%s','%s');", member, name, "Officer"));
//                        //for(String member: fac.GetOfficers())stmt2.executeUpdate(String.format("INSERT OR IGNORE INTO `Master`(`player`,`faction`,`rank`) VALUES ('%s','%s','%s');",member,name,"Officer"));
//                    }
//                    for (String member : fac.GetGenerals()) {
//                        stmt.executeUpdate(String.format("DELETE FROM `Master` WHERE `player` = '%s';", member));
//                        stmt.executeUpdate(String.format("INSERT INTO `Master`(`player`,`faction`,`rank`) VALUES ('%s','%s','%s');", member, name, "General"));
//                    }
//                    //for(String member: fac.GetGenerals())stmt2.executeUpdate(String.format("INSERT OR IGNORE INTO `Master`(`player`,`faction`,`rank`) VALUES ('%s','%s','%s');",member,name,"General"));
//                    Console.WriteLine(fac.getName() + " > " + fac.GetLeader());
//                    stmt.executeUpdate(String.format("DELETE FROM `Master` WHERE `player` = '%s';", fac.GetLeader()));
//                    stmt.executeUpdate(String.format("INSERT INTO `Master`(`player`,`faction`,`rank`) VALUES ('%s','%s','%s');", fac.GetLeader(), name, "Leader"));
//                    //stmt2.executeUpdate(String.format("INSERT INTO `Master`(`player`,`faction`,`rank`) VALUES (''%s'',''%s'',''%s'');",fac.GetLeader(),name,"Leader"));
                        CyberCoreMain.Log.Info(ChatColors.Green + "[Factions] Saving Faction " + name);
                    }
                    catch (Exception ex)
                    {
                        CyberCoreMain.Log.Error(ChatColors.Red + "[Factions] Error! Faction " + e.Key, ex);
                        // getServer().getLogger().info(ex.getClass().getName() + ":77 " + ex.getMessage());
                    }
                }

                stmt.Insert("COMMIT;");
                //stmt2.close();
            }
            catch (Exception ex)
            {
                CyberCoreMain.Log.Error(":7 ", ex);
            }
        }

/*
    public bool PlayerExistsInDB(String player, String fac) {
        try {
            MySqlDataReader r = this.ExecuteQuerySQL(String.format("select count(*) from master where `faction` LIKE '%s' , `player` LIKE '%s'",fac,player));
            if(r == null)return false;
            if(r.next())if(r.getInt(1) > 0)return true;
            r.close();
            return false;
        } catch (Exception e) {
            return false;
        }
    }*/

        public List<Dictionary<string, object>> ExecuteQuerySQL(String s)
        {
            try
            {
                SqlManager c = getSQLManager();
                if (c == null)
                {
                    Console.WriteLine("WOW ERROR WITH MySqlConnection E3324332!!! ");
                    return null;
                }

                List<Dictionary<string, object>> r = c.executeSelect(s);
                //this.getServer().getLogger().info( s );
//            stmt.close();
                return r;
            }
            catch (Exception ex)
            {
                CyberCoreMain.Log.Info(ex.GetType().Name + ":8.1 ", ex);
                return null;
            }
        }

        public List<Dictionary<string, object>> executeSelect(String query)
        {
            return executeSelecta(query).Result;
        }
        public async Task<List<Dictionary<string, object>>> executeSelecta(String query)
        {
            List<Dictionary<String, Object>> data = new List<Dictionary<String, Object>>();
            List<String> cols = new List<string>();
            DataTable schema = null;


            // Log.Info("DDDDDDDDDDDDDDDPASSSSSSSSS 1" + Main.CCM.SQL.ConnectionString);
            using (var con = new MySqlConnection(Main.CCM.SQL.ConnectionString))
            {
                await con.OpenAsync();
                // Log.Info("DDDDDDDDDDDDDDDPASSSSSSSSS 1.1");
                using (var schemaCommand = new MySqlCommand(query, con))
                {
                    // Log.Info("DDDDDDDDDDDDDDDPASSSSSSSSS 1.2");
                    using (var reader = await schemaCommand.ExecuteReaderAsync(CommandBehavior.SchemaOnly))
                    {
                        // Log.Info("DDDDDDDDDDDDDDDPASSSSSSSSS 1.3");
                        schema = reader.GetSchemaTable();
                        // Log.Info("DDDDDDDDDDDDDDDPASSSSSSSSS 1.4");
                    }
                }

                // Log.Info("DDDDDDDDDDDDDDDPASSSSSSSSS 2");

                foreach (DataRow col in schema.Rows)
                {
                    cols.Add(col.Field<String>("ColumnName"));
                }


                // Log.Info("DDDDDDDDDDDDDDDPASSSSSSSSS 3");

                using (var schemaCommand = new MySqlCommand(query, con))
                {
                    using (var reader = await schemaCommand.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            // int i = 0;
                            Dictionary<String, Object> aa = new Dictionary<string, object>();
                            foreach (var co in cols)
                            {
                                aa.Add(co, reader[co]);
                                // i++;
                            }
                            
                            data.Add(aa);
                        }
                    }
                }
            }

            // Log.Info("DDDDDDDDDDDDDDDPASSSSSSSSS 4");
            return data;
        }


        public bool factionExistsInDB(String name)
        {
            try
            {
                List<Dictionary<string, object>> r =
                    this.ExecuteQuerySQL($"select * from `Settings` where `Name` LIKE '{name}'");
                if (r == null) return false;
                if (r.Count != 0)
                    // if (r.GetInt32(1) > 0)
                    return true;
                return false;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public String GetFromSettingsString(String key, String faction)
        {
            try
            {
                List<Dictionary<string, object>> r =
                    this.ExecuteQuerySQL($"select * from `Settings` where `Name` = '{faction}'");
                if (r == null) return null;
                if (r.Count != 0) return (string) r[0][key];
                return null;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public int? GetFromSettingsInt(String key, String faction)
        {
            try
            {
                List<Dictionary<string, object>> r =
                    this.ExecuteQuerySQL($"select * from `Settings` where `Name` = '{faction}'");
                if (r == null) return null;
                if (r.Count != 0) return (int?) r[0][key];
                return null;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public Double? GetFromSettingsDouble(String key, String faction)
        {
            try
            {
                List<Dictionary<string, object>> r =
                    this.ExecuteQuerySQL($"select * from `Settings` where `Name` = '{faction}'");
                if (r == null) return null;
                if (r.Count != 0) return (double?) r[0][key];
                return null;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public String GetDisplayName(String faction)
        {
            try
            {
                List<Dictionary<string, object>> r =
                    this.ExecuteQuerySQL($"select * from `settings` where faction = '{faction}'");
                if (r == null) return null;
                if (r.Count != 0)
                {
                    return (string) r[0]["DisplayName"];
                }

                return null;
            }
            catch (Exception e)
            {
                CyberCoreMain.Log.Error(e);
                return null;
            }
        }

        public String GetLeader(String faction)
        {
            try
            {
                List<Dictionary<string, object>> r =
                    this.ExecuteQuerySQL(
                        $"select * from `Master` where `faction` = '{faction}' and `rank` LIKE 'leader'");
                if (r == null) return null;
                if (r.Count != 0)
                {
                    return (string) CyberUtilsExtender.GetString(r, "player");
                }

                return null;
            }
            catch (Exception e)
            {
                CyberCoreMain.Log.Error(e);
                return null;
            }
        }

        public List<String> GetRecruits(String faction)
        {
            try
            {
                List<String> result = new List<String>();
                List<Dictionary<string, object>> r =
                    this.ExecuteQuerySQL(
                        $"select * from `Master` where `faction` LIKE '{faction}' AND `rank` LIKE 'recruit'");
                if (r == null) return null;
                while (r.Count != 0)
                {
                    result.Add(CyberUtilsExtender.GetString(r, "player").ToLower());
                    FacList.Add(CyberUtilsExtender.GetString(r, "player").ToLower(), faction);
                }

                return result;
            }
            catch (Exception e)
            {
                CyberCoreMain.Log.Error(e);
                return null;
            }
        }

        public List<String> GetMemebrs(String faction)
        {
            try
            {
                List<String> result = new List<String>();
                List<Dictionary<string, object>> r =
                    this.ExecuteQuerySQL(
                        $"select * from `Master` where `faction` LIKE '{faction}' AND `rank` LIKE 'Member'");
                if (r == null) return null;
                while (r.Count != 0)
                {
                    result.Add(CyberUtilsExtender.GetString(r, "player").ToLower());
                    FacList.Add(CyberUtilsExtender.GetString(r, "player").ToLower(), faction);
                }

                return result;
            }
            catch (Exception e)
            {
                CyberCoreMain.Log.Error(e);
                return null;
            }
        }

        public List<String> GetOfficers(String faction)
        {
            try
            {
                List<String> result = new List<String>();
                List<Dictionary<string, object>> r =
                    this.ExecuteQuerySQL(
                        $"select * from `Master` where `faction` LIKE '{faction}' AND `rank` LIKE 'Officer'");
                if (r == null) return null;
                while (r.Count != 0)
                {
                    result.Add(CyberUtilsExtender.GetString(r, "player").ToLower());
                    FacList.Add(CyberUtilsExtender.GetString(r, "player").ToLower(), faction);
                }

                return result;
            }
            catch (Exception e)
            {
                CyberCoreMain.Log.Error(e);
                return null;
            }
        }

        public List<String> GetGenerals(String faction)
        {
            try
            {
                List<String> result = new List<String>();
                List<Dictionary<string, object>> r = this.ExecuteQuerySQL(
                    "select * from `Master` where `faction` LIKE '%s' AND `rank` LIKE '%s'");
                if (r == null) return null;
                while (r.Count != 0)
                {
                    result.Add(CyberUtilsExtender.GetString(r, "player").ToLower());
                    FacList.Add(CyberUtilsExtender.GetString(r, "player").ToLower(), faction);
                }

                return result;
            }
            catch (Exception e)
            {
                CyberCoreMain.Log.Error(e);
                return null;
            }
        }

//    public List<String> GetPlots(String faction) {
//        try {
//            List<String> results = new List<>();
//            List<Dictionary<string, object>> r = this.ExecuteQuerySQL(String.format("select * from `plots` where `faction` LIKE '%s'", faction));
//            if (r == null) return null;
//            while (r.next()) {
//                results.add(r.getInt("x") + "|" + r.getInt("z"));
//                PlotsList.put(r.getInt("x") + "|" + r.getInt("z"), faction.ToLower());
//            }
//            return results;
//        } catch (Exception e) {
//            
// CyberCoreMain.Log.Error(e);
// return null;
//        }
//    }

        /**
* @param faction
* @return
* 
*/
        public Dictionary<String, object> GetWars(String faction)
        {
            try
            {
                Dictionary<string, object> results = new Dictionary<string, object>();

                List<Dictionary<string, object>> r =
                    this.ExecuteQuerySQL($"select * from `war` where `attackingfaction` LIKE '{faction}'");
                if (r == null) return null;
                while (r.Count != 0)
                {
                    War.Add(CyberUtilsExtender.GetString(r, "attackingfaction"),
                        CyberUtilsExtender.GetString(r, "defendingfaction"));
                    String df = CyberUtilsExtender.GetString(r, "defendingfaction");
                    results.Add("attackingfaction", CyberUtilsExtender.GetString(r, "attackingfaction"));
                    results.Add("defendingfaction", CyberUtilsExtender.GetString(r, "defendingfaction"));
                    results.Add("start", CyberUtilsExtender.GetInt32(r, "start"));
                    results.Add("stop", CyberUtilsExtender.GetInt32(r, "stop"));
                }

                return results;
            }
            catch (Exception e)
            {
                CyberCoreMain.Log.Error(e);
                return null;
            }
        }

        public List<String> GetAllFactionsNames()
        {
            CyberCoreMain.Log.Info("GETTINGALL FACS");

            List<String> results = new List<String>();
            // try
            //
            // {
            Log.Info("PASSSSSSSSS 1");
            var r = executeSelect("select * from `Settings`");
            // if (r.IsClosed)
            // {
            //     Log.Error("WTFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF");
            //     Console.WriteLine("WTF THIS IS NULL TOOOOOOO E33746!");
            //     return null;
            // }
            Log.Info("PASSSSSSSSS 2");

            foreach (var a in r)
            {
                String ff = (string) a["Name"];
                CyberCoreMain.Log.Info("FOUNDDDDDDD FACCCCCCCCCCCCCCC" + ff);
                if (!results.Contains(ff)) results.Add(ff);
            }

            Log.Info("PASSSSSSSSS 3");
//            r.getStatement().close();
            return results;
            // }
            // catch (Exception e)
            // {
            //     CyberCoreMain.Log.Info("EEE", e);
            //     CyberCoreMain.Log.Error(e);
            //     return null;
            //     return results;
            // }
        }

        public List<String> GetAllies(String faction)
        {
            try
            {
                List<String> results = new List<String>();
                List<Dictionary<string, object>> r = this.ExecuteQuerySQL(
                    $"select * from `relationships` where `factiona` LIKE '{faction}' OR `factionb` LIKE '{faction}'");
                if (r == null) return null;
                while (r.Count != 0)
                {
                    if (CyberUtilsExtender.GetString(r, "factiona")
                        .Equals(faction, StringComparison.CurrentCultureIgnoreCase))
                        results.Add(CyberUtilsExtender.GetString(r, "factionb"));
                    if (CyberUtilsExtender.GetString(r, "factionb")
                        .Equals(faction, StringComparison.CurrentCultureIgnoreCase))
                        results.Add(CyberUtilsExtender.GetString(r, "factiona"));
                }

                return results;
            }
            catch (Exception e)
            {
                CyberCoreMain.Log.Error(e);
                return null;
            }
        }

        public List<String> GetEnemies(String faction)
        {
            try
            {
                List<String> results = new List<String>();
                List<Dictionary<string, object>> r = this.ExecuteQuerySQL(
                    $"select * from `enemies` where `factiona` LIKE '{faction}' OR `factionb` LIKE '{faction}'");
                if (r == null) return null;
                while (r.Count != 0)
                {
                    if (CyberUtilsExtender.GetString(r, "factiona")
                        .Equals(faction, StringComparison.CurrentCultureIgnoreCase))
                        results.Add(CyberUtilsExtender.GetString(r, "factionb"));
                    if (CyberUtilsExtender.GetString(r, "factionb")
                        .Equals(faction, StringComparison.CurrentCultureIgnoreCase))
                        results.Add(CyberUtilsExtender.GetString(r, "factiona"));
                }

                return results;
            }
            catch (Exception e)
            {
                CyberCoreMain.Log.Error(e);
                return null;
            }
        }

        public Vector3? GetHome(String faction)
        {
            try
            {
                List<Dictionary<string, object>> r =
                    this.ExecuteQuerySQL($"select * from `home` where `faction` LIKE '{faction}'");
                if (r == null) return null;
                if (r.Count != 0)
                    return new Vector3(CyberUtilsExtender.GetInt32(r, "x"), CyberUtilsExtender.GetInt32(r, "y"),
                        CyberUtilsExtender.GetInt32(r, "z"));
            }
            catch (Exception e)
            {
                CyberCoreMain.Log.Error(e);
                return null;
            }

            return null;
        }

        public Dictionary<String, int> GetInvites(String faction)
        {
            try
            {
                Dictionary<String, int> result = new Dictionary<String, int>();
                List<Dictionary<string, object>> r =
                    this.ExecuteQuerySQL($"select * from `confirm` where `faction` LIKE '{faction}'");
                if (r == null) return null;
                while (r.Count != 0)
                {
                    FactionInviteData fid =
                        new FactionInviteData(CyberUtilsExtender.GetString(r, "player"), faction,
                            CyberUtilsExtender.GetInt32(r, "timestamp"));
                    result.Add(CyberUtilsExtender.GetString(r, "player").ToLower(),
                        CyberUtilsExtender.GetInt32(r, "timestamp"));
                    addFactionInvite(fid);
//                InvList.put(r.getData("player").ToLower(), fid);
                }

                return result;
            }
            catch (Exception e)
            {
                CyberCoreMain.Log.Error(e);
                return null;
            }
        }

        public List<int> GetCompletedMissions(String faction)
        {
// List<int> a = new List<int>();
// String cmid = (String) GetFromSettingsString("cmid", faction);
// if (cmid == null || cmid.equalsIgnoreCase("")) return a;
// if (cmid.contains(","))
// {
//     for (String b :
//     cmid.split(",")) {
//         a.add(int.parseInt(b));
//     }
// }
// else
// {
//     a.add(int.parseInt(cmid));
// }
//
// return a;
            return null;
        }

        public Faction CreateFaction(String name, Player p)
        {
            return CreateFaction(name, p, "Just a CyberTech Faction", false);
        }

        public FactionErrorString? CheckFactionName(String name)
        {
            Regex regex = new Regex(@"^[a-zA-Z0-9]*");

            Match match = regex.Match(name);
            if (!match.Success)
            {
                return FactionErrorString.Error_OnlyNumbersNLetters;
            }

            if (bannednames.Contains(name.ToLower()))
            {
                return FactionErrorString.Error_BannedName;
            }

            if (Main.factionExists(name))
            {
                return FactionErrorString.Error_FactionExists;
            }

            if (name.Length > 20)
            {
                return FactionErrorString.Error_NameTooLong;
            }

            if (name.Length < 3)
            {
                return FactionErrorString.Error_NameTooShort;
            }

            return null;
        }

        public Faction CreateFaction(String name, Player p, String motd, bool privacy)
        {
            return CreateFaction(name, p, CyberTexts.Default_Faction_MOTD, motd, privacy);
        }

        public Faction CreateFaction(String name, Player p, String desc, String motd, bool privacy)
        {
            if (p.getFaction() != null)
            {
                p.SendMessage(FactionErrorString.Error_InFaction.getMsg());
                return null;
            }

            if (factionExistsInDB(name))
            {
                p.SendMessage(FactionErrorString.Error_FactionExists.getMsg());
                return null;
            }

            Faction fac = new Faction(Main, name, p);
            Console.WriteLine(fac + " <<<<<< FFFFFFFFFFFFFFFF");
            LocalFactionCache.Add(name.ToLower(), fac);
            fac.addPlayerToGlobalList(p, name);
            fac.getSettings().setPower(2, true);
            fac.getSettings().setMOTD(motd, true);
            fac.getSettings().setDescription(desc, true);
            fac.getSettings().setPrivacy(privacy ? 1 : 0, true);
            p.SendMessage(FactionErrorString.Success_FactionCreated.getMsg());

// p.Faction = fac.getName();
            RegitsterToRich(fac);
//@Todo
            //        fac.save();
            return fac;
        }

        public void RegitsterToRich(String f, int cash)
        {
            Rich.Add(f, cash);
        }

        public void RegitsterToRich(Faction f)
        {
            RegitsterToRich(f.getName(), f.getSettings().getMoney());
        }

        public String factionPartialName(String name)
        {
            try
            {
                List<Dictionary<string, object>> r =
                    this.ExecuteQuerySQL($"select * from `Setting` where `Name`= '{name}%'");
                if (r == null) return null;
                if (r.Count != 0)
                {
                    return CyberUtilsExtender.GetString(r, "Name");
                }
                else
                {
                    return null;
                }
            }
            catch (Exception e)
            {
                CyberCoreMain.Log.Error(e);
                return null;
            }
        }

        public FactionRank getPlayerRank(CorePlayer p)
        {
            var f = getPlayerFaction(p);
            if (f == null) return FactionRank.None;
            var a = f.getPlayerRank(p);
            return a;
        }
        public Faction getPlayerFaction(CorePlayer p)
        {
            p.Faction = null;
            String f = null;
            if (FacList.ContainsKey(p.Username.ToLower())) f = FacList[p.Username.ToLower()];
            if (f == null) f = GetFactionFromMember(p.Username);
            Console.WriteLine("FACCCCC >>>>>>> " + f);
            if (string.IsNullOrEmpty(f)) return null;
            FacList[p.Username.ToLower()] = f;
            var ff = getFaction(f);
            p.Faction = ff.getName();
            return ff;
        }

        public String GetFactionFromMember(String faction)
        {
            try
            {
                Console.WriteLine("select * from `Master` where `player` LIKE '{faction}'");
                List<Dictionary<string, object>> r =
                    this.ExecuteQuerySQL($"select * from `Master` where `player` LIKE '{faction}'");
                if (r == null) return null;
                while (r.Count != 0)
                {
                    return r.GetString("faction").ToLower();
                }

                return null;
            }
            catch (Exception e)
            {
                CyberCoreMain.Log.Error("ERROR 1544", e);
            }

            return null;
        }

        public Faction getFaction(String name)
        {
            if (name == null) return null;
            if (LocalFactionCache.ContainsKey(name.ToLower()))
            {
                //getServer().getLogger().debug("In Cache");
                return LocalFactionCache[name.ToLower()];
            }
            else if (factionExistsInDB(name))
            {
                // Console.WriteLine("In DB11111111111111111111111111111");
                // CyberCoreMain.Log.Info("In DB11111111111111111111111111111");
                //if (List.ContainsKey(name.ToLower())) return List.get(name.ToLower());
                //No leader == No Faction!
                if (GetLeader(name) == null && !name.equalsIgnoreCase("peace") && !name.equalsIgnoreCase("wilderness"))
                {
                    CyberCoreMain.Log.Error("Error Loading Faction: " + name + "!!!");
                    return null;
                }

//            Faction fac = new Faction(Main, name, (String) GetFromSettings("displayname", name), GetLeader(name), GetMemebrs(name), GetOfficers(name), GetGenerals(name), GetRecruits(name));
                Faction fac = new Faction(Main, name, false);
//            fac.SetPlots(GetPlots(name));
//            fac.SetMaxPlayers((int) GetFromSettings("MaxPlayers", name));
////            fac.SetPowerBonus((int) GetFromSettings("powerbonus", name));
//            fac.SetMOTD((String) GetFromSettings("MOTD", name));
//            fac.SetDesc((String) GetFromSettings("Description", name));
//            fac.SetPrivacy((int) GetFromSettings("Privacy", name));
//            fac.SetPerm((String) GetFromSettings("Perm", name));
//            fac.SetHome(GetHome(name));
////            fac.SetAllies(GetAllies(name));
////            fac.SetEnemies(GetEnemies(name));
////            fac.SetInvite(GetInvites(name));
//            fac.SetDisplayName(GetDisplayName(name));
//            fac.SetPower((int) GetFromSettings("Power", name));
//            fac.SetMoney((int) GetFromSettings("Money", name));
//            fac.SetPoints((int) GetFromSettings("Points", name));
//            fac.SetXP((int) GetFromSettings("XP", name));
//            fac.SetLevel((int) GetFromSettings("Level", name));
//            fac.RetrieveActiveMission((String) GetFromSettings("ActivevMission", name));
//            fac.SetRich((int) GetFromSettings("Rich", name));
//            fac.SetCompletedMissisons(GetCompletedMissions(name));
                LocalFactionCache.Add(fac.getName().ToLower(), fac);
                return fac;
            }

            CyberCoreMain.Log.Error("WTF NOTHING FOUND AT 374!" + name);
            return null;
        }


        public List<Faction> GetAllOpenFactions()
        {
            List<Faction> found = new List<Faction>();
            try

            {
                List<Dictionary<string, object>> r = this.ExecuteQuerySQL("select * from `Settings` where `privacy`= '1'");
                if (r == null) return null;
                while (r.Count != 0)
                {
//                return r.getData("faction");
                    Faction f = Main.FFactory.getFaction(CyberUtilsExtender.GetString(r, "faction"));
                    if (f != null) found.Add(f);
                }
            }
            catch (Exception e)
            {
                CyberCoreMain.Log.Error("ERROR GETTING ALL OPEN FACTIONS", e);
            }

            return found;
        }

        public List<Faction> GetAllOpenFactions(String name)
        {
            List<Faction> found = new List<Faction>();
            try

            {
                List<Dictionary<string, object>> r =
                    this.ExecuteQuerySQL($"select * from `Settings` where `privacy`= '1' and `faction` LIKE '{name}'");
                if (r == null) return null;
                while (r.Count != 0)
                {
//                return r.getData("faction");
                    Faction f = Main.FFactory.getFaction(CyberUtilsExtender.GetString(r, "faction"));
                    if (f != null) found.Add(f);
                }
            }
            catch (Exception e)
            {
                CyberCoreMain.Log.Error("ERROR GETTING ALL OPEN FACTIONS", e);
            }

            return found;
        }

        /**
* Returns if Faction if chunk is claimed and Null if not claimed
* @param x
* @param z
* @return Null | Faction
*/
        public Faction checkPlot(int x, int z)
        {
            try
            {
                String pid = x + "|" + z;
                List<Dictionary<string, object>> r = this.ExecuteQuerySQL($"select * from plots where `plotid`= '{pid}'");
                if (r == null) return null;
                if (r.Count != 0)
                {
//                return r.getData("faction");
                    Faction f = Main.FFactory.getFaction(CyberUtilsExtender.GetString(r, "faction"));
                    if (f == null) return null;
                    return f;
                }
            }
            catch (Exception e)
            {
                CyberCoreMain.Log.Error("ERROR WHILE CHECKING PLOT", e);
            }

            return null;
        }

        public bool isPlayerInFaction(OpenPlayer p)
        {
            return isPlayerInFaction(p.getName());
        }

        public bool isPlayerInFaction(Player p)
        {
            return isPlayerInFaction(p.getName());
        }

        public bool isPlayerInFaction(String p)
        {
            try
            {
                List<Dictionary<string, object>> r = this.ExecuteQuerySQL($"select * from Master where `player`= '{p}'");
                if (r == null) return false;
                if (r.Count != 0)
                {
                    return true;
                }

                return false;
            }
            catch (Exception e)
            {
                CyberCoreMain.Log.Error("ERROR WHILE CHECKING Player In Factions", e);
            }

            return true;
        }
    }
}