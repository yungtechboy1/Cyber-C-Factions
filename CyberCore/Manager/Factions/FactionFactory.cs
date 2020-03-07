﻿using System;
using System.Collections.Generic;
using System.Numerics;
using CyberCore.Utils;
using MiNET;
using MySql.Data.MySqlClient;

namespace CyberCore.Manager.Factions
{
    public class FactionFactory
    {
        static StringComparer dd = StringComparer.OrdinalIgnoreCase;
        public Dictionary<String, Faction> LocalFactionCache = new Dictionary<String, Faction>(dd);

        public Dictionary<String, String> FacList = new Dictionary<String, String>(dd);

//    public Map<String, String> PlotsList = new TreeMap<>;
        public PlotManager PM;
        public RelationshipManager RM = new RelationshipManager(this);
        public Dictionary<String, String> allyrequest = new Dictionary<String, String>(dd);
        public Dictionary<String, String> War = new Dictionary<String, String>(dd); //Attacking V Defending
        public Dictionary<String, int> Top = new Dictionary<String, int>(dd);
        public Dictionary<String, int> Rich = new Dictionary<String, int>(dd);
        public FactionsMain Main;

        private Dictionary<String, List<FactionInviteData>>
            InvList = new Dictionary<String, List<FactionInviteData>(dd);

        private List<String> bannednames = new List<String>()
        {
            ("wilderness"), ("safezone"), ("peace")
        };

        public FactionFactory(FactionsMain main)
        {
            Main = main;
            PM = new PlotManager(this);
        }
        
        
    public void addFactionInvite(FactionInviteData fid) {
        if (InvList.ContainsKey(fid.getPlayerName())) {
            ArrayList<FactionInviteData> fidl = InvList.get(fid.getPlayerName());
            fidl.add(fid);
            InvList.put(fid.getPlayerName(), fidl);
        } else {
            ArrayList<FactionInviteData> fidl = new ArrayList<>();
            fidl.add(fid);
            InvList.put(fid.getPlayerName(), fidl);
        }
    }

    public void updatePlayerFactionInvites(String cpn) {
        if (InvList.ContainsKey(cpn)) {

            ArrayList<FactionInviteData> fidl = InvList.get(cpn);
            bool clean = false;
            while (!clean) {
                clean = true;
                if (fidl.size() == 0 || fidl.isEmpty()) continue;
                for (int i = 0; i < fidl.size() - 1; i++) {
                    FactionInviteData fid = fidl.get(i);
                    if (!fid.isValid()) {
                        fidl.remove(i);
                        clean = false;
                        break;
                    }
                }
            }
        }
    }

    public void updatePlayerFactionInvites(Player p) {
        updatePlayerFactionInvites(p.getName());

    }

    public void updatePlayerFactionInvites(CorePlayer cp) {
        updatePlayerFactionInvites(cp.getName());
    }

    public ArrayList<String> getFactionsPlayerIsInvitedTo(String cpn) {
        updatePlayerFactionInvites(cpn);
        ArrayList<String> data = new ArrayList<>();
        if (InvList.ContainsKey(cpn)) {
            ArrayList<FactionInviteData> fidl = InvList.get(cpn);
            for (FactionInviteData fid : fidl) {
                if (fid.isValid()) data.add(fid.getFaction());
            }
            return data;
        } else
            return null;
    }

    public ArrayList<String> getFactionsPlayerIsInvitedTo(Player p) {
        return getFactionsPlayerIsInvitedTo(p.getName());
    }

    public ArrayList<String> getFactionsPlayerIsInvitedTo(CorePlayer cp) {
        return getFactionsPlayerIsInvitedTo(cp.getName());
    }

    private FactionsMain getMain() {
        return Main;
    }

    private Server getServer() {
        return Server.getInstance();
    }

    /**
     * Use PM.getFactionFromPlot(x,z);
     * @param x
     * @param z
     * @return String of Faction that ownes the plot
     */
    @Deprecated
    public String getPlotOwner(Integer x, Integer z) {
        return PM.getFactionFromPlot(x,z);
    }

    public MySqlConnection getMySqlConnection() {

//        Console.WriteLine("WOR ==============================HOWEEEE");
        if(Main == null)Console.WriteLine("WOR HOWEEEE");
        if(Main.FactionData == null){
            Console.WriteLine("WOR 2222222222222HOWEEEE");
            Main.FactionData = new FactionSQL(CyberCoreMain.getInstance());
        }
        MySqlConnection c=  Main.FactionData.connectToDb();
        if(c == null)Console.WriteLine("NOOO WHYYYY U NULL BBYY##217");
        return c;
    }

    public void RemoveFaction(Faction fac) {
        MySqlConnection c = getMySqlConnection();
        try {
            String name = fac.getName();
            Statement stmt = c.createStatement();
            for (String m : fac.PlayerRanks.keySet()) {
                FacList.remove(m);
            }
            LocalFactionCache.remove(fac.getName());
            stmt.executeUpdate(String.format("DELETE FROM `allies` WHERE `factiona` LIKE '%s' OR `factionb` LIKE '%s';", name, name));
            stmt.executeUpdate(String.format("DELETE FROM `plots` WHERE `faction` LIKE '%s';", name));
            stmt.executeUpdate(String.format("DELETE FROM `confirm` WHERE `faction` LIKE '%s';", name));
            stmt.executeUpdate(String.format("DELETE FROM `home` WHERE `faction` LIKE '%s';", name));
            stmt.executeUpdate(String.format("DELETE FROM `Master` WHERE `faction` LIKE '%s';", name));
            stmt.executeUpdate(String.format("DELETE FROM `Master` WHERE `faction` LIKE '%s';", name));
            stmt.close();
        } catch (Exception ex) {
            ex.printStackTrace();
            getServer().getLogger().info(ex.getClass().getName() + ":9 " + ex.getMessage() + " > " + ex.getStackTrace()[0].getLineNumber() + " ? " + ex.getCause());
        }
    }

    public void SaveAllFactions() {
        try {
            Statement stmt = getMySqlConnection().createStatement();
            /*Statement stmt2 = getMySqliteConnection2().createStatement();
            String sql = "CREATE TABLE IF NOT EXISTS \"master\" " +
                    "(player VARCHAR PRIMARY KEY UNIQUE     NOT NULL," +
                    " faction           VARCHAR    NOT NULL, " +
                    " rank            VARCHAR     NOT NULL)";
            String sql1 = "CREATE TABLE IF NOT EXISTS \"allies\" (  " +
                    "                        `id` INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,  " +
                    "                        `factiona` VARCHAR NOT NULL,  " +
                    "                        `factionb` varchar NOT NULL,  " +
                    "                        `timestamp` TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP  " +
                    "                        );";
            String sql2 = "CREATE TABLE IF NOT EXISTS \"confirm\" (" +
                    "`player`TEXT NOT NULL," +
                    "`faction`TEXT NOT NULL," +
                    "`timestamp`INTEGER," +
                    "`id`INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT" +
                    ")";
            String sql3 = "CREATE TABLE  IF NOT EXISTS \"home\" (" +
                    "                     `faction` varchar NOT NULL UNIQUE, " +
                    "                     `x` int(250) NOT NULL, " +
                    "                     `y` int(250) NOT NULL, " +
                    "                     `z` int(250) NOT NULL, " +
                    "                     PRIMARY KEY(faction) " +
                    "                    );";
            String sql4 = "CREATE TABLE IF NOT EXISTS \"plots\" (  " +
                    "            `id` INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,  " +
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
                    "`prem`INTEGER NOT NULL," +
                    "`privacy`int(11) NOT NULL," +
                    "`power`INTEGER NOT NULL," +
                    "`money`INTEGER NOT NULL," +
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
            Main.plugin.getLogger().info("Going to save: " + LocalFactionCache.size());
            stmt.executeUpdate("BEGIN;");
            String yaml = "";
            for (Map.Entry<String, Faction> e : LocalFactionCache.entrySet()) {
                try {
                    Faction fac = e.getValue();
                    String name = e.getKey();
//                    ArrayList<String> allies = fac.GetAllies();
//                    ArrayList<String> enemies = fac.GetEnemies();
//                    ArrayList<String> plots = fac.GetPlots();
////                    Map<String, Integer> invites = fac.GetInvite();
//                    Vector3 home = fac.GetHome();
//                    String motd = fac.GetMOTD();
//                    String displayName = fac.getDisplayName();
//                    String desc = fac.GetDesc();
//                    FactionPermSettings perms = fac.GetPerm();
//                    Integer powerbonus = fac.GetPowerBonus();
//                    Integer privacy = fac.GetPrivacy();
//                    Integer maxplayers = fac.GetMaxPlayers();
//                    Integer power = fac.GetPower();
//                    Integer money = fac.GetMoney();
//
//                    //@TODO
//                    Integer point = fac.GetPoints();
//                    Integer xp = fac.GetXP();
//                    Integer lvl = fac.GetLevel();
//                    Integer rich = fac.GetRich();
                    String am = "";
//                    if (fac.GetActiveMission() != null) {
//                        am = fac.GetActiveMission().id + "";
//                        try {
//                            ActiveMission a = fac.GetActiveMission();
//                            a.faction = null;
//                            a.Main = null;
//                            Main.AM.set(fac.getName(), a.ToHashMap());
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

                    ArrayList<Integer> CMID1 = fac.GetCompletedMissions();
                    String CMID = "";
                    if (CMID1.size() > 1) {
                        bool f = true;
                        for (Integer aa : CMID1) {
                            if (!f) {
                                CMID = CMID + ",";
                            } else {
                                f = false;
                            }
                            CMID = CMID + aa;
                        }
                    } else if (CMID1.size() != 0) {
                        for (Integer aa : CMID1) {
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
//                        String[] p = plot.split("\\|");
//                        stmt.executeUpdate(String.format("INSERT INTO `plots` VALUES (null,'%s','%s','%s');", name, p[0], p[1]));
//                    }
//                    stmt.executeUpdate("DELETE FROM `confirm` WHERE `faction` LIKE '" + name + "';");
//                    for (Map.Entry<String, Integer> ee : invites.entrySet()) {
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
//                    CyberCoreMain.getInstance().getLogger().error(String.format("INSERT INTO `Master` VALUES ('%s','%s','%s','%s','%s','%s','%s','%s','%s','%s','%s','%s','%s','%s','%s','%s');"
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
                    Main.plugin.getLogger().info(TextFormat.GREEN + "[Factions] Saving Faction " + name);
                } catch (Exception ex) {
                    Main.plugin.getLogger().error(TextFormat.RED + "[Factions] Error! Faction " + e.getKey(), ex);
                    ex.printStackTrace();
                    getServer().getLogger().info(ex.getClass().getName() + ":77 " + ex.getMessage());
                }
            }
            stmt.executeUpdate("COMMIT;");
            stmt.close();
            //stmt2.close();

        } catch (Exception ex) {
            ex.printStackTrace();
            getServer().getLogger().error(":7 ", ex);
        }
    }

/*
    public bool PlayerExistsInDB(String player, String fac) {
        try {
            ResultSet r = this.ExecuteQuerySQL(String.format("select count(*) from master where `faction` LIKE '%s' , `player` LIKE '%s'",fac,player));
            if(r == null)return false;
            if(r.next())if(r.getInt(1) > 0)return true;
            r.close();
            return false;
        } catch (Exception e) {
            return false;
        }
    }*/

    public ResultSet ExecuteQuerySQL(String s) {
        try {

            MySqlConnection c = this.getMySqlConnection();
            if(c == null){
                Console.WriteLine("WOW ERROR WITH MySqlConnection E3324332!!! ");
                return null;
            }
            Statement stmt = c.createStatement();
            ResultSet r = stmt.executeQuery(s);
            //this.getServer().getLogger().info( s );
//            stmt.close();
            return r;
        } catch (Exception ex) {

            getServer().getLogger().info(ex.getClass().getName() + ":8.1 " + ex.getMessage(), ex);
            return null;
        }
    }

    public bool factionExistsInDB(String name) {
        try {
            ResultSet r = this.ExecuteQuerySQL(String.format("select count(*) from `Settings` where `Name` LIKE '%s'", name));
            if (r == null) return false;
            if (r.next()) if (r.getInt(1) > 0) return true;
            r.close();
            return false;
        } catch (Exception e) {
            return false;
        }
    }

    public Object GetFromSettings(String key, String faction) {
        try {
            ResultSet r = this.ExecuteQuerySQL(String.format("select * from `Settings` where `Name` = '%s'", faction));
            if (r == null) return null;
            if (r.next()) return r.getObject(key);
            return null;


        } catch (Exception e) {


            return null;
        }
    }

    public String GetDisplayName(String faction) {
        try {
            ResultSet r = this.ExecuteQuerySQL(String.format("select * from `settings` where faction = '%s'", faction));
            if (r == null) return null;
            if (r.next()) {
                return r.getString("DisplayName");
            }
            return null;
        } catch (Exception e) {
            throw new RuntimeException(e);
        }
    }

    public String GetLeader(String faction) {
        try {
            ResultSet r = this.ExecuteQuerySQL(String.format("select * from `Master` where `faction` = '%s' and `rank` LIKE 'leader'", faction));
            if (r == null) return null;
            if (r.next()) {
                return r.getString("player");
            }
            return null;
        } catch (Exception e) {
            throw new RuntimeException(e);
        }
    }

    public ArrayList<String> GetRecruits(String faction) {
        try {
            ArrayList<String> result = new ArrayList<>();
            ResultSet r = this.ExecuteQuerySQL(String.format("select * from `Master` where `faction` LIKE '%s' AND `rank` LIKE '%s'", faction, "recruit"));
            if (r == null) return null;
            while (r.next()) {
                result.add(r.getString("player").toLowerCase());
                FacList.put(r.getString("player").toLowerCase(), faction);
            }
            return result;
        } catch (Exception e) {
            throw new RuntimeException(e);
        }
    }

    public ArrayList<String> GetMemebrs(String faction) {
        try {
            ArrayList<String> result = new ArrayList<>();
            ResultSet r = this.ExecuteQuerySQL(String.format("select * from `Master` where `faction` LIKE '%s' AND `rank` LIKE '%s'", faction, "Member"));
            if (r == null) return null;
            while (r.next()) {
                result.add(r.getString("player").toLowerCase());
                FacList.put(r.getString("player").toLowerCase(), faction);
            }
            return result;
        } catch (Exception e) {
            throw new RuntimeException(e);
        }
    }

    public ArrayList<String> GetOfficers(String faction) {
        try {
            ArrayList<String> result = new ArrayList<>();
            ResultSet r = this.ExecuteQuerySQL(String.format("select * from `Master` where `faction` LIKE '%s' AND `rank` LIKE '%s'", faction, "Officer"));
            if (r == null) return null;
            while (r.next()) {
                result.add(r.getString("player").toLowerCase());
                FacList.put(r.getString("player").toLowerCase(), faction);
            }
            return result;
        } catch (Exception e) {
            throw new RuntimeException(e);
        }
    }

    public ArrayList<String> GetGenerals(String faction) {
        try {
            ArrayList<String> result = new ArrayList<>();
            ResultSet r = this.ExecuteQuerySQL(String.format("select * from `Master` where `faction` LIKE '%s' AND `rank` LIKE '%s'", faction, "General"));
            if (r == null) return null;
            while (r.next()) {
                result.add(r.getString("player").toLowerCase());
                FacList.put(r.getString("player").toLowerCase(), faction);
            }
            return result;
        } catch (Exception e) {
            throw new RuntimeException(e);
        }
    }

//    public ArrayList<String> GetPlots(String faction) {
//        try {
//            ArrayList<String> results = new ArrayList<>();
//            ResultSet r = this.ExecuteQuerySQL(String.format("select * from `plots` where `faction` LIKE '%s'", faction));
//            if (r == null) return null;
//            while (r.next()) {
//                results.add(r.getInt("x") + "|" + r.getInt("z"));
//                PlotsList.put(r.getInt("x") + "|" + r.getInt("z"), faction.toLowerCase());
//            }
//            return results;
//        } catch (Exception e) {
//            throw new RuntimeException(e);
//        }
//    }

    /**
     * @param faction
     * @return
     * @deprecated
     */
    public ConfigSection GetWars(String faction) {
        try {
            ConfigSection results = new ConfigSection();

            ResultSet r = this.ExecuteQuerySQL(String.format("select * from `war` where `attackingfaction` LIKE '%s'", faction));
            if (r == null) return null;
            while (r.next()) {
                War.put(r.getString("attackingfaction"), r.getString("defendingfaction"));
                String df = r.getString("defendingfaction");
                results.set("attackingfaction", r.getString("attackingfaction"));
                results.set("defendingfaction", r.getString("defendingfaction"));
                results.set("start", r.getInt("start"));
                results.set("stop", r.getInt("stop"));
            }
            return results;
        } catch (Exception e) {
            throw new RuntimeException(e);
        }
    }

    public ArrayList<String> GetAllFactionsNames() {
        Main.plugin.getLogger().info("GETTINGALL FACS");
        ArrayList<String> results = new ArrayList<>();
        try {
            ResultSet r = this.ExecuteQuerySQL("select * from `Settings`");
            if (r == null) {
                Console.WriteLine("WTF THIS IS NULL TOOOOOOO E33746!");
                return null;
            }
            while (r.next()) {
                String ff = r.getString("Name");
                Main.plugin.getLogger().info("FOUNDDDDDDD FACCCCCCCCCCCCCCC" + ff);
                if (!results.contains(ff)) results.add(ff);
            }
            r.close();
//            r.getStatement().close();
            return results;
        } catch (Exception e) {
            Main.plugin.getLogger().info("EEE", e);
//            throw new RuntimeException(e);
            return results;
        }
    }

    public ArrayList<String> GetAllies(String faction) {
        try {
            ArrayList<String> results = new ArrayList<>();
            ResultSet r = this.ExecuteQuerySQL(String.format("select * from `relationships` where `factiona` LIKE '%s' OR `factionb` LIKE '%s'", faction, faction));
            if (r == null) return null;
            while (r.next()) {
                if (r.getString("factiona").equalsIgnoreCase(faction)) results.add(r.getString("factionb"));
                if (r.getString("factionb").equalsIgnoreCase(faction)) results.add(r.getString("factiona"));
            }
            return results;
        } catch (Exception e) {
            throw new RuntimeException(e);
        }
    }

    public ArrayList<String> GetEnemies(String faction) {
        try {
            ArrayList<String> results = new ArrayList<>();
            ResultSet r = this.ExecuteQuerySQL(String.format("select * from `enemies` where `factiona` LIKE '%s' OR `factionb` LIKE '%s'", faction, faction));
            if (r == null) return null;
            while (r.next()) {
                if (r.getString("factiona").equalsIgnoreCase(faction)) results.add(r.getString("factionb"));
                if (r.getString("factionb").equalsIgnoreCase(faction)) results.add(r.getString("factiona"));
            }
            return results;
        } catch (Exception e) {
            throw new RuntimeException(e);
        }
    }

    public Vector3 GetHome(String faction) {
        try {
            ResultSet r = this.ExecuteQuerySQL(String.format("select * from `home` where `faction` LIKE '%s'", faction));
            if (r == null) return null;
            if (r.next()) return new Vector3(r.getInt("x"), r.getInt("y"), r.getInt("z"));
        } catch (Exception e) {
            throw new RuntimeException(e);
        }
        return null;
    }

    public Map<String, Integer> GetInvites(String faction) {
        try {
            Map<String, Integer> result = new HashMap<>();
            ResultSet r = this.ExecuteQuerySQL(String.format("select * from `confirm` where `faction` LIKE '%s'", faction));
            if (r == null) return null;
            while (r.next()) {
                FactionInviteData fid = new FactionInviteData(r.getString("player"), r.getInt("timestamp"), faction);
                result.put(r.getString("player").toLowerCase(), r.getInt("timestamp"));
                addFactionInvite(fid);
//                InvList.put(r.getString("player").toLowerCase(), fid);
            }
            return result;
        } catch (Exception e) {
            throw new RuntimeException(e);
        }
    }

    public ArrayList<Integer> GetCompletedMissions(String faction) {
        ArrayList<Integer> a = new ArrayList<>();
        String cmid = (String) GetFromSettings("cmid", faction);
        if (cmid == null || cmid.equalsIgnoreCase("")) return a;
        if (cmid.contains(",")) {
            for (String b : cmid.split(",")) {
                a.add(Integer.parseInt(b));
            }
        } else {
            a.add(Integer.parseInt(cmid));
        }
        return a;
    }

    public Faction CreateFaction(String name, CorePlayer p) {
        return CreateFaction(name, p, "Just a CyberTech Faction", false);
    }

    public FactionErrorString CheckFactionName(String name) {
        if (!name.matches("^[a-zA-Z0-9]*")) {
            return Error_OnlyNumbersNLetters;
        }
        if (bannednames.contains(name.toLowerCase())) {
            return Error_BannedName;
        }
        if (Main.factionExists(name)) {
            return Error_FactionExists;
        }
        if (name.length() > 20) {
            return Error_NameTooLong;
        }
        if (name.length() < 3) {
            return Error_NameTooShort;
        }
        return null;
    }

    public Faction CreateFaction(String name, CorePlayer p, String motd, bool privacy) {
        return CreateFaction(name, p, CyberTexts.Default_Faction_MOTD, motd, privacy);
    }

    public Faction CreateFaction(String name, CorePlayer p, String desc, String motd, bool privacy) {

        if (p.Faction != null) {
            p.sendMessage(Error_InFaction.getMsg());
            return null;
        } if(factionExistsInDB(name)){
            p.sendMessage(Error_FactionExists.getMsg());
            return null;
        }


        Faction fac = new Faction(Main, name,p);
        Console.WriteLine(fac + " <<<<<< FFFFFFFFFFFFFFFF");
        LocalFactionCache.put(name.toLowerCase(), fac);
        fac.addPlayerToGlobalList(p,name);
        fac.getSettings().setPower(2,true);
        fac.getSettings().setMOTD(motd,true);
        fac.getSettings().setDescription(desc,true);
        fac.getSettings().setPrivacy(privacy ? 1 : 0,true);
        p.sendMessage(Success_FactionCreated.getMsg());
        p.Faction = fac.getName();
        RegitsterToRich(fac);
//@Todo
        //        fac.save();
        return fac;
    }

    public void RegitsterToRich(String f, int cash) {
        Rich.put(f, cash);
    }

    public void RegitsterToRich(Faction f) {
        RegitsterToRich(f.getName(), f.getSettings().getMoney());
    }

    public String factionPartialName(String name) {
        try {
            ResultSet r = this.ExecuteQuerySQL(String.format("select * from `Setting` where `Name`= '%s'", name + "%"));
            if (r == null) return null;
            if (r.next()) {
                return r.getString("Name");
            } else {
                return null;
            }
        } catch (Exception e) {
            throw new RuntimeException(e);
        }
    }

    public Faction getPlayerFaction(Player name) {
        if (name instanceof CorePlayer) {
            String ff = ((CorePlayer) name).Faction;
            if (ff != null) return getFaction(ff);
        }
        Faction f = getPlayerFaction(name.getName().toLowerCase());
        if (name instanceof CorePlayer) {
            if (f != null) {
                ((CorePlayer) name).Faction = f.getName();
            } else {
                ((CorePlayer) name).Faction = null;
            }
        }
        return f;
    }

    public Faction getPlayerFaction(CommandSender name) {
        return getPlayerFaction(name.getName().toLowerCase());
    }

    public Faction IsPlayerInFaction(CorePlayer p) {
        if (p.Faction != null) return getFaction(p.Faction);
        String f = null;
        if (FacList.ContainsKey(p.getName().toLowerCase())) f = FacList.get(p.getName().toLowerCase());
        if (f == null) f = GetFactionFromMember(p.getName());
        Console.WriteLine("FACCCCC >>>>>>> " + f);
        if (f == null || f.length() == 0) return null;
        return getFaction(f);
    }

    public String GetFactionFromMember(String faction) {
        try {
            Console.WriteLine(String.format("select * from `Master` where `player` LIKE '%s'", faction));
            ResultSet r = this.ExecuteQuerySQL(String.format("select * from `Master` where `player` LIKE '%s'", faction));
            if (r == null) return null;
            while (r.next()) {
                return r.getString("faction").toLowerCase();
            }
            return null;
        } catch (Exception e) {
            CyberCoreMain.getInstance().getLogger().error("ERROR 1544", e);
        }
        return null;

    }

    public Faction getPlayerFaction(String name) {
        if (name != null && FacList.ContainsKey(name.toLowerCase())) {
            return getFaction(FacList.get(name.toLowerCase()));
        }
        return null;
    }

    public Faction getFaction(String name) {
        if (name == null) return null;
        if (LocalFactionCache.ContainsKey(name.toLowerCase())) {
            //getServer().getLogger().debug("In Cache");
            return LocalFactionCache.get(name.toLowerCase());
        } else if (factionExistsInDB(name)) {
            Console.WriteLine("In DB11111111111111111111111111111");
            getServer().getLogger().info("In DB11111111111111111111111111111");
            //if (List.ContainsKey(name.toLowerCase())) return List.get(name.toLowerCase());
            //No leader == No Faction!
            if (GetLeader(name) == null && !name.equalsIgnoreCase("peace") && !name.equalsIgnoreCase("wilderness")) {
             CyberCoreMain.getInstance().getLogger().error("Error Loading Faction: "+name+"!!!");
                return null;
            }
//            Faction fac = new Faction(Main, name, (String) GetFromSettings("displayname", name), GetLeader(name), GetMemebrs(name), GetOfficers(name), GetGenerals(name), GetRecruits(name));
            Faction fac = new Faction(Main, name, false);
//            fac.SetPlots(GetPlots(name));
//            fac.SetMaxPlayers((Integer) GetFromSettings("MaxPlayers", name));
////            fac.SetPowerBonus((Integer) GetFromSettings("powerbonus", name));
//            fac.SetMOTD((String) GetFromSettings("MOTD", name));
//            fac.SetDesc((String) GetFromSettings("Description", name));
//            fac.SetPrivacy((Integer) GetFromSettings("Privacy", name));
//            fac.SetPerm((String) GetFromSettings("Perm", name));
//            fac.SetHome(GetHome(name));
////            fac.SetAllies(GetAllies(name));
////            fac.SetEnemies(GetEnemies(name));
////            fac.SetInvite(GetInvites(name));
//            fac.SetDisplayName(GetDisplayName(name));
//            fac.SetPower((Integer) GetFromSettings("Power", name));
//            fac.SetMoney((Integer) GetFromSettings("Money", name));
//            fac.SetPoints((Integer) GetFromSettings("Points", name));
//            fac.SetXP((Integer) GetFromSettings("XP", name));
//            fac.SetLevel((Integer) GetFromSettings("Level", name));
//            fac.RetrieveActiveMission((String) GetFromSettings("ActivevMission", name));
//            fac.SetRich((Integer) GetFromSettings("Rich", name));
//            fac.SetCompletedMissisons(GetCompletedMissions(name));
            LocalFactionCache.put(fac.getName().toLowerCase(), fac);
            return fac;
        }
        Main.plugin.getLogger().error("WTF NOTHING FOUND AT 374!" + name);
        return null;
    }


    public ArrayList<Faction> GetAllOpenFactions() {
        ArrayList<Faction> found = new ArrayList<>();
        try {
            ResultSet r = this.ExecuteQuerySQL("select * from `Settings` where `privacy`= '1'");
            if (r == null) return null;
            while (r.next()) {
//                return r.getString("faction");
                Faction f = Main.FFactory.getFaction(r.getString("faction"));
                if (f != null) found.add(f);
            }
        } catch (Exception e) {
            Main.plugin.getLogger().error("ERROR GETTING ALL OPEN FACTIONS", e);
        }
        return found;
    }

    public ArrayList<Faction> GetAllOpenFactions(String name) {
        ArrayList<Faction> found = new ArrayList<>();
        try {
            ResultSet r = this.ExecuteQuerySQL("select * from `Settings` where `privacy`= '1' and `faction` LIKE '" + name + "'");
            if (r == null) return null;
            while (r.next()) {
//                return r.getString("faction");
                Faction f = Main.FFactory.getFaction(r.getString("faction"));
                if (f != null) found.add(f);
            }
        } catch (Exception e) {
            Main.plugin.getLogger().error("ERROR GETTING ALL OPEN FACTIONS", e);
        }
        return found;
    }

    /**
     * Returns if Faction if chunk is claimed and Null if not claimed
     * @param x
     * @param z
     * @return Null | Faction
     */
    public Faction checkPlot(int x, int z) {
        try {
            String pid = x+"|"+z;
            ResultSet r = this.ExecuteQuerySQL("select * from plots where `plotid`= '"+pid+"'");
            if (r == null) return null;
            if(r.next()) {
//                return r.getString("faction");
                Faction f = Main.FFactory.getFaction(r.getString("faction"));
                if (f == null)return null;
                return f;
            }
        } catch (Exception e) {
            Main.plugin.getLogger().error("ERROR WHILE CHECKING PLOT", e);
        }
        return null;

    }

    public bool isPlayerInFaction(CorePlayer p) {
        return isPlayerInFaction(p.getName());
    }
    public bool isPlayerInFaction(Player p) {
        return isPlayerInFaction(p.getName());
    }
    public bool isPlayerInFaction(String p) {
        try {
            ResultSet r = this.ExecuteQuerySQL("select * from Master where `player`= '"+p+"'");
            if (r == null) return false;
            if(r.next()) {
                r.close();
                return true;
            }
            return false;
        } catch (Exception e) {
            Main.plugin.getLogger().error("ERROR WHILE CHECKING PLOT", e);
        }
        return true;
    }
    }
}