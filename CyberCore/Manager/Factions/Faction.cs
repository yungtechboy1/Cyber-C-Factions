﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using CyberCore.Manager.Factions.Missions;
using CyberCore.Utils;
using Google.Protobuf.WellKnownTypes;
using MiNET;
using MySql.Data.MySqlClient;
using OpenAPI.Events.Block;
using Type = System.Type;

namespace CyberCore.Manager.Factions
{
    public class Faction
    {
        /**
 * Created by carlt_000 on 5/13/2016.
 */
////@TODO Make this so I can Call a Faction Class....

//    public String Leader;
        //    public List<String> Recruits = new List<>();
//    public List<String> Members = new List<>();
//    public List<String> Officers = new List<>();
//    public List<String> Generals = new List<>();
//   Needs Updating

        //VOLATILE - Things that can change locally without automatically being updated in DB
        [NonVolatile] public Dictionary<String, FactionRank> PlayerRanks = new Dictionary<String, FactionRank>();

        public ActiveMission AM = null;
        public List<AllyRequest> AR = new List<AllyRequest>();
        public Dictionary<String, Object> SC_Map = new Dictionary<String, Object>();
        public FactionSettings Settings;

        public List<Type> NeededfromsettingsType = new List<Type>()
        {
            typeof(String), typeof(String), typeof(int), typeof(String), typeof(String), typeof(int),
            typeof(int), typeof(int), typeof(int), typeof(decimal), typeof(int), typeof(int)
        };

        public List<String> Neededfromsettings = new List<String>()
        {
            ("DisplayName"), ("Name"), ("MaxPlayers"), ("MOTD"), ("Description"), ("Privacy"), ("Perm"), ("Power"),
            ("Money"), ("Points"), ("XP"), ("PowerBonus"), ("Level"), "Rich"
        };

        //Keeping Everyting Synced
        protected bool Clean = true;
        protected int Global_Cache_Lastupload = 0;

        protected int Home_Cache_Lastupload = 0;

        //Not needed to be updated
        LinkedList<String> LastFactionChat = new LinkedList<String>();
        LinkedList<String> LastAllyChat = new LinkedList<String>();
        Dictionary<String, HomeData> HomeCacheData = new Dictionary<String, HomeData>();
        CacheChecker PlayerRanksCC = new CacheChecker("PlayerRanks");
        private CacheChecker SettingCC = new CacheChecker("PermSettings");
        private CacheChecker HomeCC = new CacheChecker("Home");

        private CacheChecker GlobalCC = new CacheChecker("Global");

        //NON-VOLATILE - Things are changed both Locally and in DB at the Same Time
        // @NonVolatile

        private String Name;

        //    @NonVolatile
//    private String DisplayName;
        //    @NonVolatile
//    private String MOTD;
//    @NonVolatile
//    private String Desc;
        private FactionsMain Main;
        private FactionLocalCache LC;
        private String War = null;

        private Dictionary<String, Invitation> Invites = new Dictionary<String, Invitation>();

        //    private int MaxPlayers = 15;
//    private Double PowerBonus = 1d;
//    private int Privacy = 0;
//    private int Perms = 0;
//    private int Power = 0;
//    private int Rich = 0;
//    private int Points = 0;
//    private int XP = 0;
//    private int Level = 0;
        //todo Save faction PermSettings
        private List<int> CompletedMissionIDs = new List<int>();

        //    private int Money = 0;
//    private Vector3 Home = new Vector3(0, 0, 0);
//    private int Global_Cache_UpdateEverySecs = 60 * 15;
//    private int Settings_Cache_UpdateEverySecs = 60 * 15;
//    private int Home_Cache_UpdateEverySecs = 60 * 15;
//    private int Settings_Cache_Lastupload = 0;
        private int UpdateEverySecs = 60 * 5;

        public Faction(FactionsMain main, String name, CorePlayer cp)
        {
            LC = new FactionLocalCache(this);
            this(main, name, true);
            addPlayer(cp, FactionRank.Leader, null);
        }

        public Faction(FactionsMain main, String name, bool newfac)
        {
            Main = main;
            Name = name;
            LC = new FactionLocalCache(this);
            Settings = new FactionSettings(this, true);
            if (newfac)
                onCreation();
            else
                loadFromDB();
        }

        public Faction(FactionsMain main, String name, String displayname, bool newfac = false)
        {
            Main = main;
            Name = name;
            LC = new FactionLocalCache(this);
            Settings = new FactionSettings(this, false);
            getSettings().setDisplayName(displayname, true);
            if (newfac)
                onCreation();
            else
                loadFromDB();
        }

//    public Faction(FactionsMain main, String name, String displayname, String leader, List<String> members, List<String> generals, List<String> officers, List<String> recruits) {
//        Main = main;
//        Name = name;
//        getSettings().setDisplayName(displayname);
//        for (String m : members) {
//            PlayerRanks.put(m, FactionRank.Member);
//            Player p = Main.getServer().getPlayerExact(m);
//            if (p != null) {
//                Main.FFactory.FacList.put(p.getName().toLowerCase(), Name);
//            }
//        }
//        for (String m : officers) {
//            PlayerRanks.put(m, FactionRank.Officer);
//            Player p = Main.getServer().getPlayerExact(m);
//            if (p != null) {
//                Main.FFactory.FacList.put(p.getName().toLowerCase(), Name);
//            }
//        }
//        for (String m : generals) {
//            PlayerRanks.put(m, FactionRank.General);
//            Player p = Main.getServer().getPlayerExact(m);
//            if (p != null) {
//                Main.FFactory.FacList.put(p.getName().toLowerCase(), Name);
//            }
//        }
//
//        for (String m : recruits) {
//            PlayerRanks.put(m, FactionRank.Recruit);
//            Player p = Main.getServer().getPlayerExact(m);
//            if (p != null) {
//                Main.FFactory.FacList.put(p.getName().toLowerCase(), Name);
//            }
//        }
//        Player p = Main.getServer().getPlayerExact(Leader);
//        if (p != null) Main.FFactory.FacList.put(p.getName().toLowerCase(), Name);
//        onCreation();
//    }

        public void reloadPlayerRanks()
        {
            reloadPlayerRanks(false);
        }

        //CJ LOOK HERE
        //TODO ALSO FIX PROMOTE AND DEMOTE TO SEND DIRECTLY TO SERVER
        public void reloadPlayerRanks(bool force)
        {
            if (!PlayerRanksCC.needsUpdate() && !force) return;
            if (Main == null) Console.WriteLine("Error 1111111111111111111111111111111111111111");
            if (Main.FFactory == null)
                Console.WriteLine("Error 1111111111111111111111111111111111111111222222222222222222222222222");
            try
            {
                MySqlDataReader q = Main.CCM.SQL.Query($"SELECT * FROM Master WHERE `faction` LIKE '{getName()}'");
                PlayerRanks.Clear();
                while (q.Read())
                {
                    String pn = q.GetString("player");
                    FactionRank fr = FactionRankMethods.getRankFromString(q.GetString("rank"));
                    PlayerRanks.Add(pn, fr);
                }

                PlayerRanksCC.updateLastUpdated();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Console.WriteLine("Error tring to UPDATE player RANKS for FACTION " + getName() +
                                  "! Please report Error 'E21112D t'o an admin");
            }
        }

        public Object GetFromSettings(String key)
        {
            Dictionary<String, Object> a = GetAllSettings();
            if (!a.ContainsKey(key)) return null;
            return a[key];
        }

        public int GetFromSettingsExact(String key)
        {
            try
            {
                MySqlDataReader q = Main.CCM.SQL.Query($"select * from `PermSettings` where Name = '{getName()}'");
                if (q.Read())
                {
                    return q.GetInt32(key);
                }

                return Int32.MinValue;
            }
            catch (Exception e)
            {
                return Int32.MinValue;
            }
        }

        public String GetFromSettingsExact(String key)
        {
            try
            {
                MySqlDataReader q = Main.CCM.SQL.Query($"select * from `PermSettings` where Name = '{getName()}'");
                if (q.Read())
                {
                    return q.GetString(key);
                }

                return null;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public Dictionary<String, Object> GetAllSettings(bool forceupdate)
        {
            if (forceupdate)
            {
                SC_Map = null;
            }

            return GetAllSettings();
        }

        public Dictionary<String, Object> GetAllSettings()
        {
            if (SettingCC.needsUpdate())
            {
                SettingCC.updateLastUpdated();

                Dictionary<String, Object> a = new Dictionary<String, Object>();
                try
                {
                    MySqlDataReader q = Main.CCM.SQL.Query($"select * from `Settings` where `Name` = '{getName()}'");
                    if (q == null)
                    {
                        SC_Map = null;
                        return null;
                    }

                    if (q.Read())
                    {
                        foreach (var k in Neededfromsettings)
                        {
                            Object v = q.getObject(k);
                            if (v != null) a.put(k, v);
                        }


                        for (String k :
                        Neededfromsettings) {
                            Object v = r.getObject(k);
                            if (v != null) a.put(k, v);
                        }
                        SC_Map = (Dictionary<String, Object>) a.clone();
                        return a;
                    }

                    CyberCoreMain.getInstance().getLogger()
                        .error("Error with Faction PermSettings Cache E39942!BBBeeeeeeeeeeBBB");
                    return null;
                }
                catch (Exception e)
                {
                    CyberCoreMain.getInstance().getLogger()
                        .error("Error with Faction PermSettings Cache E39942!AbbbbbbbnAA", e);

                    return null;
                }
            }
            else
            {
                if (SC_Map == null)
                {
                    CyberCoreMain.getInstance().getLogger()
                        .error("Error! SC_MAP WAS NOT REQUIRED TO UPDATE AND NO VALUES FOUND E3322334!");
                    return null;
                }
                else
                {
                    return SC_Map;
                }
            }
        }

        //TODO
        @TODO

        private void onCreation()
        {
            getSettings().setDisplayName(getName());
            try
            {
                //Update PermSettings
                FactionsMain.getInstance().FFactory.getMySqlConnection().createStatement().executeUpdate(
                    String.format(
                        "INSERT INTO `Settings` VALUES('%s','%s'," + getSettings().getMaxPlayers() + "," +
                        getSettings().getPowerBonus() + ",'%s','%s'," + getSettings().getPrivacy() + ",'%s'," +
                        getSettings().getPower() + "," + getSettings().getMoney() + "," + getSettings().getRich() +
                        "," + getSettings().getXP() + "," + getSettings().getLevel() + "," +
                        getSettings().getPoints() + ")", getName(), getSettings().getDisplayName(),
                        getSettings().getMOTD(), getSettings().getDescription(), getPermSettings().export()));
                CyberCoreMain.getInstance().getLogger()
                    .error(
                        "Error with Faction PermSettings Cache E39942!BBgfggggwww122222222222222222222222222222222222222222222222222gBBBB");
                return;
            }
            catch (Exception e)
            {
                CyberCoreMain.getInstance().getLogger()
                    .error("Error with Faction PermSettings Cache E399dddddddaaaaaaaaaa42!AAA", e);
                return;
            }
        }

        private void loadFromDB()
        {
            getSettings().download();
            reloadPlayerRanks(true);
        }

        public int getUpdateEveryTick()
        {
            return UpdateEverySecs * 20;
        }

        public bool onTick(int tick)
        {
            reloadPlayerRanks(true);
            return true;
        }


        public FactionSettings getSettings()
        {
            return Settings;
        }

        public FactionPermSettings getPermSettings()
        {
            return getSettings().getPermSettings();
        }

        public void PromotePlayer(Player pp)
        {
            PromotePlayer(pp, false);
        }

        public void PromotePlayer(Player pp, bool leaderConfirm)
        {
            FactionRank cr = getPlayerRank(pp);
            String pn = pp.getName().toLowerCase();
            switch (cr)
            {
                case Recruit:
                    PlayerRanks.replace(pn, FactionRank.Member);
                    updatePlayerRankinDB(pp, FactionRank.Member);
                    break;
                case Member:
                    PlayerRanks.replace(pn, FactionRank.Officer);
                    updatePlayerRankinDB(pp, FactionRank.Officer);
                    break;
                case Officer:
                    PlayerRanks.replace(pn, FactionRank.General);
                    updatePlayerRankinDB(pp, FactionRank.General);
                    break;
                case General:
                    if (!leaderConfirm) break;
                    String l = GetLeader();
                    PlayerRanks.replace(l, FactionRank.General);
                    PlayerRanks.replace(pn, FactionRank.Leader);
                    updatePlayerRankinDB(pp, FactionRank.Leader);
                    updatePlayerRankinDB(l, FactionRank.General);
//                Leader = (pn);
                    break;
            }
        }

        public void DemotePlayer(Player pp)
        {
            FactionRank cr = getPlayerRank(pp);
            String pn = pp.getName().toLowerCase();
            switch (cr)
            {
                case Member:
                    PlayerRanks.replace(pn, FactionRank.Recruit);
                    updatePlayerRankinDB(pp, FactionRank.Recruit);
                    break;
                case Officer:
                    PlayerRanks.replace(pn, FactionRank.Member);
                    updatePlayerRankinDB(pp, FactionRank.Member);
                    break;
                case General:
                    PlayerRanks.replace(pn, FactionRank.Officer);
                    updatePlayerRankinDB(pp, FactionRank.Officer);
                    break;
            }
        }

        public void updatePlayerRankinDB(Player p, FactionRank r)
        {
            updatePlayerRankinDB(p.getName(), r);
        }

        public void updatePlayerRankinDB(CorePlayer p, FactionRank r)
        {
            updatePlayerRankinDB(p.getName(), r);
        }

        public void updatePlayerRankinDB(String n, FactionRank rr)
        {
            Connection c = CyberCoreMain.getInstance().FM.FFactory.getMySqlConnection();
            try
            {
                Statement s = c.createStatement();
                s.executeUpdate("UPDATE INTO Master SET rank = '" + rr.getName() + "'WHERE player LIKE '" + n +
                                "'");
                c.close();
            }
            catch (Exception e)
            {
                e.printStackTrace();
                Console.WriteLine(
                    "Error sending plots to DB FOR RANK UPDATE!!! Please report Error 'E190 t'o an admin");
            }
        }

        public Faction GetAllyFromName(String name)
        {
            String found = null;
            name = name.toLowerCase();
            int delta = 2147483647;
            List<String> var1 = LC.getAllies();
            Iterator<String> var4 = var1.iterator();

            while (var4.hasNext())
            {
                String fn = var4.next();
                if (fn.toLowerCase().startsWith(name) || fn.toLowerCase().contains(name))
                {
                    int curDelta = fn.length() - name.length();
                    if (curDelta < delta)
                    {
                        found = fn;
                        delta = curDelta;
                    }

                    if (curDelta == 0)
                    {
                        found = fn;
                    }
                }
            }

            return Main.FFactory.getFaction(found);
        }

        public int getPlayerCount()
        {
            return PlayerRanks.size();
        }

        public void KickPlayer(Player p)
        {
            String pn = p.getName();
            if (!PlayerRanks.containsKey(pn))
            {
                Console.WriteLine("Error! " + pn + " Dose not exist in Faction " + getName());
                return;
            }

            Connection c = CyberCoreMain.getInstance().FM.FFactory.getMySqlConnection();
            try
            {
                Statement s = c.createStatement();
                s.executeUpdate("DELETE FROM Master WHERE player LIKE '" + pn + "' AND faction LIKE '" + getName() +
                                "'");
                c.close();
            }
            catch (Exception e)
            {
                e.printStackTrace();
                Console.WriteLine("Error tring to delete player from DB! Please report Error 'E22D t'o an admin");
            }

            PlayerRanks.remove(pn);
            BroadcastMessage(FactionsMain.NAME + TextFormat.YELLOW + p.getName() +
                             " has been  kicked from the faction!");
            p.sendMessage(FactionsMain.NAME + TextFormat.GREEN + "You Have Been Kicked From factionName!!!");
            getSettings().TakePower(2);
        }

        public void SendFactionChatWindow(CorePlayer cp)
        {
            cp.showFormWindow(new FactionChatFactionWindow((LinkedList<String>) LastFactionChat.clone()));
        }

        public void HandleFactionChatWindow(FormResponseCustom frc, CorePlayer cp)
        {
            if (frc == null)
            {
                Console.WriteLine("Error @ 12255");
                return;
            }

            String msg = frc.getInputResponse(0);
            if (msg == null)
            {
                //No Message Send?
                //CLose windows
                return;
            }

            AddFactionChatMessage(msg, cp);
            SendFactionChatWindow(cp);
        }

        //@todo LMAO THIS IS NOT EVEN CLOSE TO being correcty XD
        public String toString()
        {
            return getName();
        }

        public List<String> GetPlots()
        {
            return FactionsMain.getInstance().FFactory.PM.getFactionPlots(getName());
        }

        public bool AddPlots(int chunkx, int chunkz, CorePlayer player)
        {
            if (!FactionsMain.getInstance().FFactory.PM.addPlot(chunkx, chunkz, getName()))
            {
                player.sendMessage("Error! That plot is claimed E:33421");
                return false;
            }

            return true;
        }

        public bool DelPlots(int chunkx, int chunkz, CorePlayer playe)
        {
            return DelPlots(chunkx, chunkz, playe, false);
        }

        public bool DelPlots(int chunkx, int chunkz, CorePlayer player, bool overclaim)
        {
            if (!FactionsMain.getInstance().FFactory.PM.delPlot(chunkx, chunkz, getName(), overclaim))
            {
                player.sendMessage("Error deleting Plot E339211");
                return false;
            }

            return true;
        }

//    public void SetMaxPlayers(int value) {
//        MaxPlayers = value;
//    }
//
//    public int GetMaxPlayers() {
//        return MaxPlayers;
//    }
//
//    public void SetPowerBonus(Double value) {
//        PowerBonus = value;
//    }
//
//    public Double GetPowerBonus() {
//        return PowerBonus;
//    }

        public int CalculateMaxPower()
        {
            int TP = GetNumberOfPlayers();
            return TP * 10;
            //Lets do 20 Instead of 10
        }

        public int GetNumberOfPlayers()
        {
            return getPlayerCount();
        }

//    public void SetPrivacy(int value) {
//        Privacy = value;
//    }
//
//    public int GetPrivacy() {
//        return Privacy;
//    }

        public String getName()
        {
            return Name;
        }

//   M

        public FactionPermSettings GetPerm()
        {
            return getPermSettings();
        }

//    public int GetPerm(int key) {
//        try {
//            return int.parseInt(Perms.toString().substring(key));
//        } catch (Exception ignore) {
//            return null;
//        }
//    }

//    public void SetPoints(int value) {
//        if (value == null) value = 0;
//        Points = value;
//    }
//
//    public int GetPoints() {
//        return Points;
//    }

//    public void AddPoints(int pts) {
//        SetPoints(GetPoints() + Math.abs(pts));
//    }
//
//    public void TakePoints(int pts) {
//        int a = GetPoints() - pts;
//        if (a < 0) SetPoints(0);
//        SetPoints(a);
//    }

//    public void SetLevel(int value) {
//        Level = value;
//        UpdateBossBar();
//    }
//
//    public int GetLevel() {
//        return Level;
//    }

//    public void AddLevel(int lvl) {
//        SetLevel(GetLevel() + Math.abs(lvl));
//    }
//
//    public void TakeLevel(int lvl) {
//        int a = GetLevel() - lvl;
//        if (a < 0) SetLevel(0);
//        SetLevel(a);
//    }


        public void HandleKillEvent(PlayerDeathEvent event) {
            if (GetActiveMission() != null)
            {
                GetActiveMission().AddKill();
            }
        }

        public void HandleBreakEvent(BlockBreakEvent event) {
            if (GetActiveMission() != null)
            {
                GetActiveMission().BreakBlock(event);
            }
        }

        public void HandlePlaceEvent(BlockPlaceEvent event) {
            if (GetActiveMission() != null)
            {
                GetActiveMission().PlaceBlock(event);
            }
        }

        public void SetActiveMission(String id)
        {
            if (id == null || id.equalsIgnoreCase(""))
            {
                SetActiveMission();
            }
            else
            {
                SetActiveMission(int.parseInt(id));
            }
        }

        public void AcceptNewMission(int id, CommandSender Sender)
        {
            if (GetActiveMission() != null)
            {
                Sender.sendMessage(FactionsMain.NAME + TextFormat.RED + "Error you already have a mission!!");
                return;
            }

            if (CompletedMissionIDs.contains(id))
            {
                Sender.sendMessage(FactionsMain.NAME + TextFormat.RED +
                                   "Error you have already completed this mission!!");
                return;
            }

            SetActiveMission(id);
        }

        public void SetActiveMission(int id)
        {
//        for(Mission mission: Main.Missions){
//            if(mission.id.equals(id)) {
//                SetActiveMission(new ActiveMission(Main,this,mission));
//                BroadcastMessage(FactionsMain.NAME+TextFormat.AQUA+mission.name+TextFormat.GREEN+" Faction mission accepted!");
//            }
//        }
        }

        public void RetrieveActiveMission(String id)
        {
            if (id == null || id.equalsIgnoreCase(""))
            {
                SetActiveMission();
            }
            else
            {
                RetrieveActiveMission(int.parseInt(id));
            }
        }

        public void RetrieveActiveMission(int id)
        {
//        if(Main.AM.exists(getName())){
//            for(Mission mission: Main.Missions){
//                if(mission.id.equals(id)){
//                    try {
//                        /*YamlConfig yamlConfig = new YamlConfig();
//                        yamlConfig.setClassTag("ActiveMission",ActiveMission.class);
//                        yamlConfig.setClassTag("tag:yaml.org,2002:cn.nukkit.item.ItemBlock",ItemBlock.class);
//                        YamlReader reader = new YamlReader(new FileReader(Main.getDataFolder().toString()+"/missions/"+getName()+".yml"),yamlConfig);
//                        ActiveMission activeMission = reader.read(ActiveMission.class);
//                        Console.WriteLine(activeMission.name)*/;
//                        ActiveMission activeMission = new ActiveMission(Main, this,(ConfigSection) Main.AM.get(getName()));
//                        SetActiveMission(activeMission);
//                        //SetActiveMission(new ActiveMission(Main,this,mission));
//                        BroadcastMessage(FactionsMain.NAME+TextFormat.AQUA+mission.name+TextFormat.GREEN+" Faction mission accepted!");
//                        return;
//                    }catch(Exception ex){
//                        ex.printStackTrace();
//                    }
//                }
//            }
//        }
//        SetActiveMission();
        }

        public void SetActiveMission()
        {
            AM = null;
        }

        public void SetActiveMission(ActiveMission mission)
        {
            AM = mission;
        }

        public ActiveMission GetActiveMission()
        {
            return AM;
        }

        public void CompleteMission(ActiveMission mission)
        {
            CompletedMissionIDs.add(mission.id);
            AM = null;
        }

        public void SetCompletedMissisons(List<int> value)
        {
            CompletedMissionIDs = value;
        }

        public List<int> GetCompletedMissions()
        {
            return CompletedMissionIDs;
        }

        public void AddCompletedMission(int mission)
        {
            CompletedMissionIDs.add(mission);
        }
//
//    public void SetMoney(int value) {
////        Money = value;
//        Connection c = CyberCoreMain.getInstance().FM.FFactory.getMySqlConnection();
//        try {
//            Statement s = c.createStatement();
//            s.executeUpdate("UPDATE Settings SET Money = " + value + " WHERE Name LIKE '" + getName() + "' VALUES ");
//        } catch (Exception e) {
//            e.printStackTrace();
//            return;
//        }
//        return;
////        UpdateTopResults();
//    }
//
//    /**
//     * @return null | Int
//     */
//    public int GetMoney() {
//        Connection c = CyberCoreMain.getInstance().FM.FFactory.getMySqlConnection();
//        try {
//            Statement s = c.createStatement();
//            ResultSet r = s.executeQuery("SELECT Money FROM Settings WHERE Name LIKE '" + getName() + "'");
//            List<AllyRequest> list = new List<>();
//            if (r.next()) {
//                int m = r.getInt("Money");
//                c.close();
//                return m;
//            }
//        } catch (Exception e) {
//            e.printStackTrace();
//            return null;
//        }
//        return null;
//    }
//
//    public void AddMoney(int money) {
//        SetMoney(GetMoney() + Math.abs(money));
//    }
//
//    public void TakeMoney(int money) {
//        int a = GetMoney() - money;
//        if (a < 0) SetMoney(0);
//        SetMoney(a);
//    }

//    public int GetRich() {
//        return Rich + GetMoney();
//    }
//
//    public void SetRich(int rich) {
//        Rich = rich;
//    }

        public void CalcualteRich()
        {
            //Level lvl = Main.getServer().getLevelByName("world");

            //Main.getServer().getScheduler().scheduleAsyncTask(new FactionRichAsyncSingle(Main,lvl,this));
            /*int value = 0;
            if(lvl == null)return value;
            for(String plot: GetPlots()){
            String key = plot.split("\\|")[0] + "|" + plot.split("\\|")[1];
                int sx = int.parseInt(plot.split("\\|")[0]) << 4;
                int sz = int.parseInt(plot.split("\\|")[1]) << 4;
                for (int x = 0; x < 64; x++) {
                    for (int y = 0; y < 128; y++) {
                        for (int z = 0; z < 64; z++) {
                            int fx = x + sx;
                            int fz = z + sz;
                            Block b = lvl.getBlock(new Vector3(fx,y,fz));
                            String kkey = "";
                            if(b.getDamage() != 0){
                                kkey = b.getId() + "|" + b.getDamage();
                            }else{
                                kkey = b.getId()+"";
                            }
                            if(Main.BV.exists(kkey))value += (int) Main.BV.get(kkey);
                        }
                    }
                }
            }*/
            //return value;
        }

        public int GetMaxPower()
        {
            return CalculateMaxPower();
        }

//    public void SetPower(int value) {
//        int dif = value - GetPower();
//        String t = "";
//        if (dif > 0) {
//            t = TextFormat.GREEN + "Gained +" + dif;
//        } else {
//            t = TextFormat.RED + "Lost -" + Math.abs(dif);
//        }
//        BroadcastPopUp(TextFormat.GRAY + "Faction now has " + TextFormat.GREEN + value + TextFormat.GRAY + " PowerAbstract!" + t);
//        Power = value;
//    }
//
//    public int GetPower() {
//        return Power;
//    }

//    public void AddPower(int power) {
//        int t = GetPower() + Math.abs(power);
//        if (t > CalculateMaxPower()) {
//            SetPower(CalculateMaxPower());
//        } else {
//            SetPower(t);
//        }
//    }
//
//    public void TakePower(int power) {
//        int a = GetPower() - power;
//        if (a < 0) {
//            SetPower(0);
//        } else {
//            SetPower(a);
//        }
//    }

        public Dictionary<String, HomeData> GetHomes()
        {
            return GetHome();
        }

        public Dictionary<String, HomeData> GetHome()
        {
            if (HomeCC.needsUpdate() || HomeCacheData == null)
            {
                HomeCC.updateLastUpdated();
                Dictionary<String, HomeData> f = new Dictionary<>();
                Connection c = CyberCoreMain.getInstance().FM.FFactory.getMySqlConnection();
                try
                {
                    Statement s = c.createStatement();
                    ResultSet r = s.executeQuery("SELECT * FROM `Homes` WHERE `faction` LIKE '" + getName() + "'");
                    while (r.next())
                    {
                        int hid = r.getInt("homeid");
                        String name = r.getString("name");
                        String lvln = r.getString("level");
                        String faction = r.getString("faction");
                        int xx = r.getInt("x");
                        int yy = r.getInt("y");
                        int zz = r.getInt("z");
                        HomeData h = new HomeData(xx, yy, zz, lvln, name, faction, hid);
                        if (h.isValid())
                        {
                            f.put(name, h);
                        }
                    }

                    if (f.size() == 0)
                    {
                        HomeCacheData = null;
                    }
                    else
                    {
                        HomeCacheData = (Dictionary<String, HomeData>) f.clone();
                    }

                    c.close();
//        Main.FFactory.allyrequest.put(getName(), fac.getName());
                }
                catch (Exception e)
                {
                    CyberCoreMain.getInstance().getLogger().error("Error GETTING HOMES FROM SQL E33112A", e);
                    return null;
                }

                return f;
            }
            else
            {
                return HomeCacheData;
            }
        }

        /**
     * HOME FORMAT
     * <p>
     * X|Y|Z|Level|Name]X|Y|Z|Level|Name]
     *
     * @return
     */
        @Deprecated

        public Dictionary<String, Position> GetHome_V1()
        {
            Dictionary<String, Position> f = new Dictionary<>();
            String h = (String) GetFromSettings("Home");
            if (h.contains("\\]"))
            {
                String[] a = h.split("\\]");
                for (String aa :
                a) {
                    String[] h1 = aa.split("\\|");
                    if (h1.length == 5)
                    {
                        try
                        {
                            int x = int.parseInt(h1[0]);
                            int y = int.parseInt(h1[1]);
                            int z = int.parseInt(h1[2]);
                            String lvl = h1[3];
                            cn.nukkit.level.Level l = Server.getInstance().getLevelByName(lvl);
                            if (l == null)
                            {
                                CyberCoreMain.getInstance().getLogger().error(
                                    "COULD NOT LOAD FACCCTION HOME FOR " + getName() + " BECAUSE HOME AT " + x +
                                    " | " + y + " | " + z + " LEVEL NAME IS NOT VALID!!! LEVEVL NAME:" + lvl);
                                continue;
                            }

                            String nme = h1[4];
                            f.put(nme, new Position(x, y, z, l));
                        }
                        catch (Exception e)
                        {
                            CyberCoreMain.getInstance().getLogger().error(
                                "Error! Exception While tring to get " + getName() + "'s Faction Homes!", e);
                        }
                    }
                    else
                    {
                        CyberCoreMain.getInstance().getLogger().error("Home Syntax error for " + aa);
                    }
                }
            }
            else
            {
                String[] hhh = h.split("\\|");
                if (hhh.length == 5)
                {
                    try
                    {
                        int x = int.parseInt(hhh[0]);
                        int y = int.parseInt(hhh[1]);
                        int z = int.parseInt(hhh[2]);
                        String lvl = hhh[3];
                        cn.nukkit.level.Level l = Server.getInstance().getLevelByName(lvl);
                        if (l == null)
                        {
                            CyberCoreMain.getInstance().getLogger().error(
                                "111COULD NOT LOAD FACCCTION HOME FOR " + getName() + " BECAUSE HOME AT " + x +
                                " | " + y + " | " + z + " LEVEL NAME IS NOT VALID!!! LEVEVL NAME:" + lvl);
                            return null;
                        }

                        String nme = hhh[4];
                        f.put(nme, new Position(x, y, z, l));
                    }
                    catch (Exception e)
                    {
                        CyberCoreMain.getInstance().getLogger().error(
                            "111Error! Exception While tring to get " + getName() + "'s Faction Homes!", e);
                    }
                }
                else
                {
                    CyberCoreMain.getInstance().getLogger().error("111Home Syntax error for " + h);
                }
            }

            return f;
        }

        public bool DelHome(int h)
        {
            Connection c = CyberCoreMain.getInstance().FM.FFactory.getMySqlConnection();
            try
            {
                Statement s = c.createStatement();
                s.executeUpdate("DELETE * FROM `Homes` WHERE `homeid` = " + h + ";");
                c.close();
            }
            catch (Exception e)
            {
                e.printStackTrace();
                CyberCoreMain.getInstance().getLogger()
                    .error("Error DELETING HOME ! Please report Error 'E30DDDR' to an admin ID:" + h, e);
                return false;
            }

            return true;
        }

        public bool addHome(HomeData h)
        {
            if (!h.isValid())
            {
                Console.WriteLine("H NOT VALIDDDDDDDDDD!!!!!!!!!!!!!!!!!!!!!!!!!!!!! E3323109");
                return false;
            }

            Connection c = CyberCoreMain.getInstance().FM.FFactory.getMySqlConnection();
            try
            {
                Statement s = c.createStatement();
                //CyberCoreMain.getInstance().getIntTime
                Vector3 hv = h.getVector3();
                s.executeUpdate("INSERT INTO `Homes` VALUES (null,+" + hv.getX() + "," + hv.getY() + "," +
                                hv.getZ() + ",'" + h.getL().getName() + ",'" + h.getFaction() + ",'" + h.getName() +
                                "') ;");
                c.close();
                HomeCC.invalidate();
//        Main.FFactory.allyrequest.put(getName(), fac.getName());
            }
            catch (Exception e)
            {
                e.printStackTrace();
                CyberCoreMain.getInstance().getLogger()
                    .error("Error SETTING HOME ! Please report Error 'E3022asDR' to an admin" + h.toString(), e);
                return false;
            }

            return true;
        }

        public void StartWar(String key)
        {
            War = key;
        }

        public void EndWar()
        {
            War = null;
        }

        public ConfigSection GetWarData()
        {
            if (War != null && Main.War.containsKey(War))
            {
                return (ConfigSection) Main.War.get(War);
            }

            return null;
        }

        public bool AtWar()
        {
            return War != null;
        }

        public bool AtWar(String fac)
        {
            if (War != null)
            {
                return ((ConfigSection) Main.War.get(War)).getString("defenders").equalsIgnoreCase(fac);
            }

            return false;
        }

        /**
     * @param fac    Faction to be added as enemy
     * @param player Player who added the Faction as an Emeney
     */
        public void AddEnemy(Faction fac, CorePlayer player)
        {
            if (!FactionsMain.getInstance().FFactory.RM.addEnemyRelationship(getName(), fac.getName()))
            {
                player.sendMessage("Error adding faction as Enemy!! E800");
                return;
            }

            fac.BroadcastMessage(getSettings().getDisplayName() + " has added your faction as an enemy!");
            BroadcastMessage(fac.getSettings().getDisplayName() + " has been set as an Enemy of your faction by " +
                             player.getDisplayName());
        }


//    public void AddCooldown(int secs){
//        Map<String, Object> cd = Main.CD.getAll();
//        int time = (int)(Calendar.getInstance().getTime().getTime()/1000);
//        cd.put(getName(),time+secs);
//    }
//    public bool HasWarCooldown(){
//        Map<String, Object> cd = Main.CD.getAll();
//        int time = (int)(Calendar.getInstance().getTime().getTime()/1000);
//        if (cd.containsKey(getName())){
//            if (time >= (int)cd.get(getName())){
//                cd.remove(getName());
//                return false;
//            }
//            return true;
//        }
//        return false;
//    }

        /**
     * @param fac Faction to be removed as an enemy
     * @param p   Player who Removes faction as enemy
     */
        public void RemoveEnemy(Faction fac, CorePlayer p)
        {
            if (!FactionsMain.getInstance().FFactory.RM.removeEnemyRelationship(getName(), fac.getName()))
            {
                p.sendMessage("Error adding faction as Enemy!! E816");
                return;
            }

            fac.BroadcastMessage(getSettings().getDisplayName() + " is no longer an enemy!");
            BroadcastMessage(fac.getSettings().getDisplayName() +
                             " is no longer set as an Enemy of your faction by " + p.getDisplayName());
        }

        public List<String> GetEnemies()
        {
            return FactionsMain.getInstance().FFactory.RM.getFactionEnemy(getName());
        }

        public bool isEnemy(String fac)
        {
            return FactionsMain.getInstance().FFactory.RM.isEnemy(getName(), fac);
        }

        public bool AddAlly(String fac)
        {
            return FactionsMain.getInstance().FFactory.RM.addAllyRelationship(getName(), fac);
        }

        public void RemoveAlly(Faction fac)
        {
            if (fac == null) return;
            RemoveAlly(fac.getName());
        }

        public bool RemoveAlly(String fac)
        {
            return FactionsMain.getInstance().FFactory.RM.removeAllyRelationship(getName(), fac);
        }

        public List<String> GetAllies()
        {
            return FactionsMain.getInstance().FFactory.RM.getFactionAllies(getName());
        }

        //Can Attack Nuetral but can not Attack Allys!
        public bool isNeutral(CorePlayer player)
        {
            Faction f = player.getFaction();
            if (f != null) return isNeutral(f);
            return true;
        }

        public bool isNeutral(String name)
        {
            return !isAllied(name) && !isEnemy(name);
        }

        public bool isNeutral(Faction fac)
        {
            return isNeutral(fac.getName());
        }

        public bool isAllied(CorePlayer player)
        {
            Faction f = player.getFaction();
            if (f != null) return isAllied(f);
            return false;
        }

        public bool isAllied(Faction fac)
        {
            return isAllied(fac.getName());
        }

        public bool isAllied(String fac)
        {
            return FactionsMain.getInstance().FFactory.RM.isAllys(getName(), fac);
        }

        public void AddInvite(CorePlayer player, int value, CorePlayer sender, FactionRank fr)
        {
            if (!addRequest(RequestType.Faction_Invite, null, player, value, sender))
            {
                player.sendMessage(
                    "Error sending Faction Invite Request! Please report Error 'E332FI' to an admin");
                return;
            }

            Invites.put(player.getName().toLowerCase(),
                new Invitation(getName(), player.getName(), sender.getName(), value, fr));

            Connection c = CyberCoreMain.getInstance().FM.FFactory.getMySqlConnection();
            try
            {
                Statement stmt = c.createStatement();
                stmt.executeUpdate("INSERT INTO `Requests` VALUES (null," + RequestType.Faction_Invite.getKey() +
                                   ",'" + getName() + "','" + sender.getName() + "','" + player.getName() + "','" +
                                   fr.getName() + "')");
                stmt.close();
                c.close();
            }
            catch (Exception e)
            {
                e.printStackTrace();
                throw new RuntimeException(e);
            }
        }

//    public void SetInvite(Map<String, int> Invs) {
//        Invites = Invs;
//    }

//    public Map<String, int> GetInvite() {
//        return Invites;
//    }

        public void DelInvite(String name)
        {
            Connection c = CyberCoreMain.getInstance().FM.FFactory.getMySqlConnection();
            try
            {
                Statement stmt = c.createStatement();
                stmt.executeUpdate("DELETE * from `Requests` where `faction` LIKE '" + getName() +
                                   "' AND `player` LIKE '" + name + "' AND `TYPE` =  " +
                                   RequestType.Faction_Invite.getKey() + ";");
            }
            catch (Exception e)
            {
                e.printStackTrace();
                throw new RuntimeException(e);
            }

            Invites.remove(name);
        }

        public bool AcceptInvite(CorePlayer p)
        {
            String name = p.getName();
            Invitation i = HasInvite(name);
            if (i == null)
            {
                //No Invite
                p.sendMessage("Error! You are not invited to join '" + getSettings().getDisplayName() + "'!");
                return false;
            }

            if (!i.isValid())
            {
                //Invite Timed out
                p.sendMessage("Error! You're invite has expired!");
                DelInvite(name);
                return false;
            }

            DelInvite(name);
            FactionRank r = i.getRank();
            switch (r)
            {
                case General:
                    addPlayer(p, FactionRank.General, i.getInvitedBy());
                    break;
                case Member:
                    addPlayer(p, FactionRank.Member, i.getInvitedBy());
                    break;
                case Officer:
                    addPlayer(p, FactionRank.Officer, i.getInvitedBy());
                    break;
                default:
                case Recruit:
                    addPlayer(p, FactionRank.Recruit, i.getInvitedBy());
                    break;
            }

            BroadcastMessage(FactionsMain.NAME + TextFormat.GREEN + name + " Has joined your faction!");
            return true;
        }

        public void DenyInvite(String name)
        {
            DelInvite(name);
        }

        public Invitation HasInvite(CorePlayer name)
        {
            return HasInvite(name.getName());
        }

        public Invitation HasInvite(String name)
        {
            Connection c = CyberCoreMain.getInstance().FM.FFactory.getMySqlConnection();
            try
            {
                Statement stmt = c.createStatement();
                ResultSet r = stmt.executeQuery("select * from `Requests` where `faction` LIKE '" + getName() +
                                                "' AND `player` LIKE '" + name + "' AND `TYPE` =  " +
                                                RequestType.Faction_Invite.getKey() + ";");
                if (r == null) return null;
                if (r.next())
                {
                    return new Invitation(getName(), name, r.getString("player"), r.getInt("expires"),
                        FactionRank.getRankFromString(r.getString("data")));
                }
                else
                {
                    return null;
                }
            }
            catch (Exception e)
            {
                e.printStackTrace();
                throw new RuntimeException(e);
            }

//        return Invites.containsKey(name.toLowerCase());
        }

        public String GetLeader()
        {
            for (Map.Entry < String, FactionRank > a : PlayerRanks.entrySet())
            {
                if (a.getValue() == FactionRank.Leader) return a.getKey();
            }
            Console.WriteLine("Errror!!!!!!! ETF E993");
            return null;
        }

//
//    public void SetLeader(String leader) {
//        Leader = leader;
//    }

        public void removePlayer(CorePlayer p)
        {
            Connection c = CyberCoreMain.getInstance().FM.FFactory.getMySqlConnection();
            try
            {
                Statement s = c.createStatement();
                s.executeUpdate("DELETE FROM Master WHERE player LIKE '" + p.getName() + "'");
                s.close();
                c.close();
            }
            catch (Exception e)
            {
                e.printStackTrace();
                Console.WriteLine("Error sending Deleting Player to DB!!! Please report Error 'E2100 t'o an admin");
            }

            PlayerRanks.remove(p.getName());
        }

        public bool addPlayer(CorePlayer p, FactionRank r, String invitedby)
        {
            return addPlayer(p.getName(), r, invitedby);
        }

        public bool addPlayer(Player p, FactionRank r, String invitedby)
        {
            return addPlayer(p.getName(), r, invitedby);
        }

        public bool addPlayer(String p, FactionRank r, String invitedby)
        {
            if (invitedby != null)
            {
                invitedby = "'" + invitedby + "'";
            }
            else invitedby = "null";

            if (CyberCoreMain.getInstance().FM.FFactory.isPlayerInFaction(p))
            {
                //Playing in faction
                Player pp = Server.getInstance().getPlayer(p);
                if (pp != null)
                    pp.sendMessage("Error you are currently in a faction and can not join a new one!!!");
                return false;
            }

            Connection c = CyberCoreMain.getInstance().FM.FFactory.getMySqlConnection();
            try
            {
                Statement s = c.createStatement();
                s.executeUpdate("INSERT INTO Master VALUES ('" + p + "','" + getName() + "','" +
                                CyberCoreMain.getInstance().getIntTime() + "'," + invitedby + ",'" + r.getName() +
                                "')");
                s.close();
                c.close();
            }
            catch (Exception e)
            {
                e.printStackTrace();
                Console.WriteLine("Error sending Adding Player to DB!!! Please report Error 'E22190 t'o an admin");
            }

            PlayerRanks.put(p, r);
            return true;
        }

        public bool IsMember(Player p)
        {
            return IsMember(p.getName());
        }

        public bool IsOfficer(Player p)
        {
            return IsOfficer(p.getName());
        }

        public bool IsGeneral(Player p)
        {
            return IsGeneral(p.getName());
        }

        public bool IsRecruit(Player p)
        {
            return IsRecruit(p.getName());
        }

        public bool IsRecruit(String n)
        {
            if (PlayerRanks.containsKey(n)) return PlayerRanks.get(n).hasPerm(FactionRank.Recruit);
            return false;
        }

        public bool IsMember(String n)
        {
            if (PlayerRanks.containsKey(n)) return PlayerRanks.get(n).hasPerm(FactionRank.Member);
            return false;
        }

        public bool IsOfficer(String n)
        {
            if (PlayerRanks.containsKey(n)) return PlayerRanks.get(n).hasPerm(FactionRank.Officer);
            return false;
        }

        public bool IsGeneral(String n)
        {
            if (PlayerRanks.containsKey(n)) return PlayerRanks.get(n).hasPerm(FactionRank.General);
            return false;
        }

        public bool IsInFaction(Player player)
        {
            return IsInFaction(player.getName());
        }

        public bool IsInFaction(String n)
        {
            for (String m :
            PlayerRanks.keySet()) if (n.equalsIgnoreCase(m)) return true;
            return false;
        }

        public void MessageAllys(String message)
        {
            BroadcastMessage(message);
            for (String ally :
            GetAllies()) {
                Faction af = Main.FFactory.getFaction(ally);
                if (af != null) af.BroadcastMessage(message);
            }
        }

        public String GetFactionNameTag(String p)
        {
            FactionRank fr = getPlayerRank(p);
            return fr.GetChatPrefix() + TextFormat.RESET + " - " + getSettings().getDisplayName();
        }

        public String GetFactionNameTag(Player p)
        {
            FactionRank fr = getPlayerRank(p);
            return fr.GetChatPrefix() + TextFormat.RESET + " - " + getSettings().getDisplayName();
        }

        public void BroadcastMessage(String message)
        {
            BroadcastMessage(message, FactionRank.All);
        }

        CacheChecker PRC = new CacheChecker("PR", 120);

        public FactionRank getPlayerRank(String p)
        {
//        PlayerRanks
            if (PlayerRanks.containsKey(p)) return PlayerRanks.get(p);
            Connection c = CyberCoreMain.getInstance().FM.FFactory.getMySqlConnection();
            try
            {
                Statement s = c.createStatement();
                ResultSet r = s.executeQuery("SELECT  * FROM Master WHERE player LIKE '" + p + "'");
                if (r.next())
                {
                    FactionRank fr = FactionRank.getRankFromString(r.getString("rank"));
                    return fr;
                }
                else
                {
                    Console.WriteLine("Error! Could not get rank from DB!!! E113e");
                }

                c.close();
            }
            catch (Exception e)
            {
                e.printStackTrace();
                Console.WriteLine(
                    "Error sending plots to DB FOR RANK UPDATE!!! Please report Error 'E190 t'o an admin");
            }

//        if (IsMember(p)) rank = FactionRank.Member;
//        if (IsOfficer(p)) rank = FactionRank.Officer;
//        if (IsGeneral(p)) rank = FactionRank.General;
//        if (Leader.equalsIgnoreCase(p)) rank = FactionRank.Leader;
//        return rank;
            return null;
        }

        public FactionRank getPlayerRank(Player p)
        {
            return getPlayerRank(p.getName().toLowerCase());
        }

        public FactionRank getPlayerRank(CorePlayer p)
        {
            return getPlayerRank((Player) p);
        }

        public void BroadcastMessage(String message, FactionRank rank)
        {
            for (Map.Entry < String, FactionRank > a : PlayerRanks.entrySet())
            {
                if (a.getValue().hasPerm(rank))
                {
                    Player p = Main.getServer().getPlayerExact(a.getKey());
                    if (p == null) continue;
                    p.sendMessage(message);
                }
            }
        }

        public void BroadcastPopUp(String message)
        {
            BroadcastPopUp(message, "");
        }

        public void BroadcastPopUp(String message, String subtitle)
        {
            BroadcastPopUp(message, subtitle, FactionRank.Recruit);
        }

        public void BroadcastPopUp(String message, String subtitle, FactionRank r)
        {
            for (Map.Entry < String, FactionRank > a : PlayerRanks.entrySet())
            {
                if (a.getValue().hasPerm(r))
                {
                    Player p = Main.getServer().getPlayerExact(a.getKey());
                    if (p == null) continue;
                    p.sendPopup(message, subtitle);
                }
            }
        }

        public int GetPlayerPerm(String name)
        {
            FactionRank fr = getPlayerRank(name);
            return fr.getPower();
        }

        public List<Player> GetOnlinePlayers()
        {
            List<Player> aa = new List();
            for (Map.Entry < String, FactionRank > a : PlayerRanks.entrySet())
            {
                Player p = Main.getServer().getPlayerExact(a.getKey());
                if (p == null) continue;
                aa.add(p);
            }
            return aa;
        }

        public String BossBarText()
        {
            /*return TextFormat.GOLD+""+TextFormat.BOLD+"====§eTERRA§6TIDE===="+TextFormat.RESET+"\n\n"+
                    "§6"+GetDisplayName()+" §b: §aLEVEL §b: §3"+GetLevel()+"\n"+
                     "§eXP §b: §6"+GetXP()+" §a/ §b"+calculateRequireExperience(GetLevel());*/
            return TextFormat.GOLD + "" + TextFormat.BOLD + "====§eTERRA§6TIDE====" + TextFormat.RESET + "\n\n" +
                   "§e" + getSettings().getDisplayName() + " §b: §aLEVEL §b: §3" + getSettings().getLevel() + "\n" +
                   "§eXP §b: §a" + getSettings().getXP() + " §a/ §3" +
                   getSettings().calculateRequireExperience(getSettings().getLevel());
        }

        public void UpdateBossBar()
        {
//        for(Player player: GetOnlinePlayers()){
//            Main.sendBossBar(player,this);
//        }
        }

        public void UpdateTopResults()
        {
            Main.FFactory.Top.put(getName(), getSettings().getMoney());
        }


//    public List<String> getFChat() {
//        return FChat;
//    }
//
//    public void setFChat(List<String> FChat) {
//        this.FChat = FChat;
//    }

        public void UpdateRichResults()
        {
            Main.FFactory.Rich.put(getName(), Settings.getRich());
        }

        public void AddAllyRequest(Faction fac)
        {
            AddAllyRequest(fac, null, -1);
        }

        public void AddAllyRequest(Faction fac, CorePlayer cp)
        {
            AddAllyRequest(fac, cp, -1);
        }

        public List<AllyRequest> getAllyRequests()
        {
            Connection c = CyberCoreMain.getInstance().FM.FFactory.getMySqlConnection();
            try
            {
                Statement s = c.createStatement();

                ResultSet r = s.executeQuery("SELECT * FROM Requestes WHERE type LIKE '" +
                                             RequestType.Ally.getKey() + "' AND target = '" + getName() + "'");

                List<int> dellist = new List<>();
                List<AllyRequest> list = new List<>();
                while (r.next())
                {
                    String fn = r.getString("faction");
                    Faction f = FactionsMain.getInstance().FFactory.getFaction(fn);
                    if (f == null)
                    {
                        dellist.add(r.getInt("id"));
                    }

                    AllyRequest ar = new AllyRequest(f, r.getInt("expires"));
                    list.add(ar);
                }

                c.close();
                return list;
            }
            catch (Exception e)
            {
                e.printStackTrace();
                return null;
            }
        }

        public bool addRequest(RequestType rt, Faction fac, CorePlayer player, int timeout, CorePlayer sender)
        {
            Connection c = CyberCoreMain.getInstance().FM.FFactory.getMySqlConnection();
            String sn = null;
            String pn = null;
            if (sender != null) sn = sender.getName();
            if (player != null) pn = player.getName();

            try
            {
                Statement s = c.createStatement();
                //1 = Ally Request
                //0 = Friend Request
                //2 = ?????
                //CyberCoreMain.getInstance().getIntTime
                switch (rt)
                {
                    case Ally:
                        //Null,0,getname(),fac.getname(),timeout,inviter
                        s.executeQuery("INSERT INTO `Requests` VALUES (null," + rt.getKey() + ",'" + fac.getName() +
                                       "','" + getName() + "'," + timeout + ",'" + sn + "')");
                    case Faction_Invite:
                        if (pn == null) return false;
                        //Null,1,getname(),player.getname(),timeout,inviter
                        s.executeQuery("INSERT INTO `Requests` VALUES (null," + rt.getKey() + ",'" + getName() +
                                       "','" + pn + "'," + timeout + ",'" + sn + "')");

                    default:
                }

                c.close();
//        Main.FFactory.allyrequest.put(getName(), fac.getName());
            }
            catch (Exception e)
            {
                e.printStackTrace();
                return false;
            }

            return true;
        }

        /**
     * Adds ally request to this faction
     *
     * @param fac     Faction Requesting to be an Ally
     * @param player  The Player who snet the Invite
     * @param timeout DateTimem to String when request expires
     */
        public void AddAllyRequest(Faction fac, CorePlayer player, int timeout)
        {
            if (!addRequest(RequestType.Ally, fac, null, timeout, player))
            {
                player.sendMessage("Error sending Ally Request! Please report Error 'E332RA' to an admin");
                return;
            }

            //        Connection c = CyberCoreMain.getInstance().FM.FFactory.getMySqlConnection();
//        try {
//            Statement s = c.createStatement();
//            //1 = Ally Request
//            //0 = Friend Request
//            //2 = ?????
//            //CyberCoreMain.getInstance().getIntTime
//            s.executeQuery(String.format("INSERT INTO `Requests` VALUES (null,1,'%s',null,'%s'," + timeout + ")", fac.getName(), getName(), timeout));
//            c.close();
////        Main.FFactory.allyrequest.put(getName(), fac.getName());
//        } catch (Exception e) {
//            e.printStackTrace();
//            player.sendMessage("Error sending Ally Request! Please report Error 'E332R' to an admin");
//            return;
//        }


            BroadcastMessage(TextFormat.AQUA + "[ArchFactions] " + fac.getSettings().getDisplayName() +
                             " wants to be Ally's with you!");
//        BroadcastMessage(TextFormat.AQUA + "[ArchFactions] Respond to the request using `/f inbox`");
            player.sendMessage(TextFormat.AQUA + "[ArchFactions] Ally request sent to " +
                               getSettings().getDisplayName());

            AR.add(new AllyRequest(fac, timeout));
            FactionRank r = getPermSettings().getAllowedToAcceptAlly();
            switch (r)
            {
                case Recruit:
                    BroadcastMessage(
                        fac.getSettings().getDisplayName() +
                        "'s Faction would like to become your ally! View the offer in /f inbox", FactionRank.All);
                    break;
                case Member:
                    BroadcastMessage(
                        fac.getSettings().getDisplayName() +
                        "'s Faction would like to become your ally! View the offer in /f inbox",
                        FactionRank.Member);
                    break;
                case Officer:
                    BroadcastMessage(
                        fac.getSettings().getDisplayName() +
                        "'s Faction would like to become your ally! View the offer in /f inbox",
                        FactionRank.Officer);
                    break;
                case General:
                    BroadcastMessage(
                        fac.getSettings().getDisplayName() +
                        "'s Faction would like to become your ally! View the offer in /f inbox",
                        FactionRank.General);
                    break;
                case Leader:
                    BroadcastMessage(
                        fac.getSettings().getDisplayName() +
                        "'s Faction would like to become your ally! View the offer in /f inbox",
                        FactionRank.Leader);
                    break;
            }
        }

        public void AddFactionChatMessage(String message, CorePlayer p)
        {
            FactionRank r = getPlayerRank(p);
            message = TextFormat.GRAY + "[" + r.GetChatPrefix() + TextFormat.GRAY + "] - " + r.getChatColor() +
                      p.getDisplayName() + TextFormat.GRAY + " > " + TextFormat.WHITE + message;
            BroadcastMessage("Faction> " + message);
            LastFactionChat.addFirst(message);
            if (LastFactionChat.size() > getPermSettings().getMaxFactionChat())
            {
                LastFactionChat.removeLast();
            }
        }

        public void AddAllyChatMessage(String message, CorePlayer p)
        {
            FactionRank r = getPlayerRank(p);
            message = TextFormat.GRAY + "[" + r.GetChatPrefix() + TextFormat.GRAY + "] - " + r.getChatColor() +
                      p.getDisplayName() + TextFormat.GRAY + " > " + TextFormat.WHITE + message;
            BroadcastMessage("Ally> " + message);
            LastAllyChat.addFirst(message);
            if (LastAllyChat.size() > getPermSettings().getMaxAllyChat())
            {
                LastAllyChat.removeLast();
            }
        }

        //    @Deprecated
        public void save()
        {
            //Save Settings
            getSettings().upload();
            //Save Player Ranks
            Connection c = CyberCoreMain.getInstance().FM.FFactory.getMySqlConnection();
            try
            {
                Statement stmt = c.createStatement();
                for (Map.Entry < String, FactionRank > m : PlayerRanks.entrySet())
                {
                    try
                    {
                        stmt.executeUpdate(String.format(
                            "UPDATE `Master` SET `rank` = '%s' WHERE `Master`.`player` = '&s'",
                            m.getValue().getName(), m.getKey()));
                    }
                    catch (Exception e)
                    {
                        e.printStackTrace();
                    }
                }
                stmt.close();
            }
            catch (Exception e)
            {
                e.printStackTrace();
            }
            //Save Plots - All Saved immedatelly to the cloud
            //

//
//        Connection c = CyberCoreMain.getInstance().FM.FFactory.getMySqlConnection();
//        try {
//            getServer().getLogger().error("DELETEING Faction " + fn + "!");
//            Statement stmt = c.createStatement();
//            stmt.executeUpdate(String.format("DELETE FROM `allies` WHERE `factiona` LIKE '%s' OR `factionb` LIKE '%s';", fn, fn));
//            stmt.close();
//        } catch (Exception ex) {
//            getServer().getLogger().info(ex.getClass().getName() + ":9 " + ex.getMessage() + " > " + ex.getStackTrace()[0].getLineNumber() + " ? " + ex.getCause());
//        }
        }

        private Server getServer()
        {
            return Server.getInstance();
        }

        public int GetMoney()
        {
            return getSettings().getMoney();
        }

        public int GetPower()
        {
            return getSettings().getPower();
        }

        public void TakeMoney(int money)
        {
            getSettings().takeMoney(money);
        }

        public void TakePower(int power)
        {
            getSettings().TakePower(power);
        }

        public String getDisplayName()
        {
            return getSettings().getDisplayName();
        }

        public int GetPrivacy()
        {
            return getSettings().getPrivacy();
        }

        public int GetMaxPlayers()
        {
            return getSettings().getMaxPlayers();
        }

        public void addPlayerToGlobalList(CorePlayer p, String name)
        {
            FactionsMain.getInstance().FFactory.FacList.put(p.getName().toLowerCase(), name);
        }

        public class HomeData
        {
            cn.nukkit.level.Level l = null;
            private int x = 0, y = 0, z = 0;
            private String name = null;
            private int HomeID = -1;
            private String faction = null;

            public HomeData(int x, int y, int z, cn.nukkit.level.Level l, String name, String fac, int hid)
            {
                HomeID = hid;
                this.x = x;
                this.y = y;
                this.z = z;
                this.l = l;
                faction = fac;
                this.name = name;
            }

            public HomeData(int x, int y, int z, String ln, String name, String fac, int hid)
            {
                HomeID = hid;
                this.x = x;
                this.y = y;
                this.z = z;
                Level tl = Server.getInstance().getLevelByName(ln);
                if (tl == null)
                {
                    CyberCoreMain.getInstance().getLogger()
                        .error("Error! HOME DATA FOR FACTION " + getFaction() + " HOME AT " +
                               getVector3().toString() + " WITH INVALID LEVEL NAME: " + ln);
                }
                else l = tl;

                faction = fac;
                this.name = name;
            }

            public int getHomeID()
            {
                return HomeID;
            }

            public Vector3 getVector3()
            {
                return new Vector3(x, y, z);
            }

            public Position getPosition()
            {
                if (getL() == null) return null;
                return new Position(x, y, z, getL());
            }

            public String getFaction()
            {
                return faction;
            }

            public int getX()
            {
                return x;
            }

            public int getY()
            {
                return y;
            }

            public int getZ()
            {
                return z;
            }

            public cn.nukkit.level.Level getL()
            {
                return l;
            }

            public String getName()
            {
                return name;
            }

            public bool isValid()
            {
                return (getL() != null);
            }
        }


        //TODO
//        Connection c = CyberCoreMain.getInstance().FM.FFactory.getMySqlConnection();
//        try {
//            Statement s = c.createStatement();
//            //1 = Ally Request
//            //0 = Friend Request
//            //2 = ?????
//            //CyberCoreMain.getInstance().getIntTime
//            s.executeQuery(String.format("INSERT INTO `Requests` VALUES (null,1,'%s',null,'%s'," + timeout + ")", fac.getName(), getName(), timeout));
//            c.close();
////        Main.FFactory.allyrequest.put(getName(), fac.getName());
//        } catch (Exception e) {
//            e.printStackTrace();
//            player.sendMessage("Error sending Ally Request! Please report Error 'E332R' to an admin");
//            return;
//        }

        public class AllyRequest
        {
            int Timeout = -1;
            Faction F;

            public AllyRequest(Faction f)
            {
                this(f, -1);
            }

            public AllyRequest(Faction f, int timeout)
            {
                F = f;
                Timeout = timeout;
            }
        }
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class NonVolatileAttribute : Attribute
    {
    }