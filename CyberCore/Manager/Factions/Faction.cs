using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Numerics;
using System.Threading.Tasks;
using CyberCore.Manager.Factions.Missions;
using CyberCore.Manager.Factions.Windows;
using CyberCore.Utils;
using log4net;
using MiNET;
using MiNET.Utils;
using MiNET.Worlds;
using MySql.Data.MySqlClient;
using OpenAPI.Events.Block;
using OpenAPI.Events.Entity;
using OpenAPI.Player;
using static CyberCore.Manager.Factions.FactionRank;
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
        public Dictionary<String, FactionRank> PlayerRanks = new Dictionary<String, FactionRank>();

        public ActiveMission AM = null;
        public List<AllyRequest> AR = new List<AllyRequest>();
        public Dictionary<String, Object> SC_Map = new Dictionary<String, Object>();
        public FactionSettings Settings ;

        public List<Type> NeededfromsettingsType = new List<Type>()
        {
            typeof(String), typeof(String), typeof(int), typeof(String), typeof(String), typeof(int),
            typeof(int), typeof(int), typeof(int), typeof(decimal), typeof(int), typeof(int)
        };

        public List<String> NeededfromsettingsString = new List<String>()
        {
            ("DisplayName"), ("Name"), ("MOTD"), ("Description"), ("Perm")
        };

        public List<String> NeededfromsettingsInt = new List<String>()
        {
            ("MaxPlayers"), ("Power"),"Rich",
            ("Money"), ("Points"), ("XP"), "Level","Privacy"
        };

        public List<String> NeededfromsettingsDouble = new List<String>()
        {
            ("PowerBonus")
        };

        // public List<String> Neededfromsettings = new List<String>()
        // {
        //     ("DisplayName"), ("Name"), ("MaxPlayers"), ("MOTD"), ("Description"), ("Privacy"), ("Perm"),,
        //     ("Level"), "Rich"
        // };

        //Keeping Everyting Synced
        protected bool Clean = true;
        protected int Global_Cache_Lastupload = 0;

        protected int Home_Cache_Lastupload = 0;

        //Not needed to be updated
        List<String> LastFactionChat = new List<String>();
        List<String> LastAllyChat = new List<String>();
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
        public FactionsMain Main { get; private set; }
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


        public Faction(FactionsMain main, String name, Player p)
        {
            Settings = new FactionSettings(this);
            Main = main;
            Name = name;
            onCreation();
            LC = new FactionLocalCache(this);
            // getSettings().setDisplayName(name, true);
            addPlayer(p, FactionRankEnum.Leader);
        }

        public Faction(FactionsMain main, String name, bool newfac = false, String displayname = null)
        {
            Main = main;
            Name = name;
            LC = new FactionLocalCache(this);
            Settings = new FactionSettings(this, true);
            if (displayname != null) getSettings().setDisplayName(displayname, true);
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
//                Main.FFactory.FacList.put(p.getName().ToLower(), Name);
//            }
//        }
//        for (String m : officers) {
//            PlayerRanks.put(m, FactionRank.Officer);
//            Player p = Main.getServer().getPlayerExact(m);
//            if (p != null) {
//                Main.FFactory.FacList.put(p.getName().ToLower(), Name);
//            }
//        }
//        for (String m : generals) {
//            PlayerRanks.put(m, FactionRank.General);
//            Player p = Main.getServer().getPlayerExact(m);
//            if (p != null) {
//                Main.FFactory.FacList.put(p.getName().ToLower(), Name);
//            }
//        }
//
//        for (String m : recruits) {
//            PlayerRanks.put(m, FactionRank.Recruit);
//            Player p = Main.getServer().getPlayerExact(m);
//            if (p != null) {
//                Main.FFactory.FacList.put(p.getName().ToLower(), Name);
//            }
//        }
//        Player p = Main.getServer().getPlayerExact(Leader);
//        if (p != null) Main.FFactory.FacList.put(p.getName().ToLower(), Name);
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
                List<Dictionary<string, object>> q =
                    Main.CCM.SQL.executeSelect($"SELECT * FROM Master WHERE `faction` LIKE '{getName()}'");
                PlayerRanks.Clear();
                foreach (var a in q)
                {
                    String pn = (string) a["player"];
                    FactionRank fr = getRankFromString((string) a["rank"]);
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

        public int GetFromSettingsExactInt(String key)
        {
            try
            {
                List<Dictionary<string, object>> q =
                    Main.CCM.SQL.executeSelect($"select * from `PermSettings` where Name = '{getName()}'");
                if (q.Count != 0)
                {
                    return (int) q[0][key];
                }

                return Int32.MinValue;
            }
            catch (Exception e)
            {
                return Int32.MinValue;
            }
        }

        public String GetFromSettingsExactString(String key)
        {
            try
            {
                List<Dictionary<string, object>> q =
                    Main.CCM.SQL.executeSelect($"select * from `PermSettings` where Name = '{getName()}'");
                if (q.Count != 0)
                {
                    return (string) q[0][key];
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
                    List<Dictionary<string, object>> q =
                        Main.CCM.SQL.executeSelect($"select * from `Settings` where `Name` = '{getName()}'");
                    if (q == null)
                    {
                        SC_Map = null;
                        return null;
                    }


                    if (q.Count != 0)
                    {
                        //TODO combine
                        foreach (var k in NeededfromsettingsString)
                        {
                            Object v = q[0][k];
                            if (v != null) a.Add(k, v);
                        }

                        foreach (var k in NeededfromsettingsInt)
                        {
                            Object v = q[0][k];
                            if (v != null) a.Add(k, v);
                        }

                        foreach (var k in NeededfromsettingsDouble)
                        {
                            Object v = q[0][k];
                            if (v != null) a.Add(k, v);
                        }

                        Dictionary<String, Object> ret = new Dictionary<String, Object>();

                        foreach (KeyValuePair<String, Object> entry in a)
                        {
                            ret.Add(entry.Key, entry.Value);
                        }

                        SC_Map = ret;
                        return a;
                    }

                    CyberCoreMain.Log.Error("Error with Faction PermSettings Cache E39942!BBBeeeeeeeeeeBBB");
                    return null;
                }
                catch (Exception e)
                {
                    CyberCoreMain.Log.Error("Error with Faction PermSettings Cache E39942!AbbbbbbbnAA", e);

                    return null;
                }
            }
            else
            {
                if (SC_Map == null)
                {
                    CyberCoreMain.Log.Error("Error! SC_MAP WAS NOT REQUIRED TO UPDATE AND NO VALUES FOUND E3322334!");
                    return null;
                }
                else
                {
                    return SC_Map;
                }
            }
        }

        //TODO
        // @TODO

        private void onCreation()
        {
            getSettings().setDisplayName(getName());
            try
            {
                //Update PermSettings
                CyberCoreMain.GetInstance().SQL.Insert(
                    $"INSERT INTO `Settings` VALUES('{getName()}','{getSettings().getDisplayName()}'," +
                    getSettings().getMaxPlayers() + "," +
                    getSettings().getPowerBonus() +
                    $",'{getSettings().getMOTD()}','{getSettings().getDescription()}'," + getSettings().getPrivacy() +
                    $",'{getPermSettings().export()}'," +
                    getSettings().getPower() + "," + getSettings().getMoney() + "," + getSettings().getRich() +
                    "," + getSettings().getXP() + "," + getSettings().getLevel() + "," +
                    getSettings().getPoints() + ")");
                CyberCoreMain.Log.Error(
                    "EGOOOODDDDDDDDDn PermSettings Cache E39942!BBgfggggwww122222222222222222222222222222222222222222222222222gBBBB");
                return;
            }
            catch (Exception e)
            {
                CyberCoreMain.Log.Error("Error with Faction PermSettings Cache E399dddddddaaaaaaaaaa42!AAA", e);
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
            String pn = pp.Username.ToLower();
            switch (cr.toEnum())
            {
                case FactionRankEnum.Recruit:
                    PlayerRanks[pn] = Member;
                    updatePlayerRankinDB(pp, Member);
                    break;
                case FactionRankEnum.Member:
                    PlayerRanks[pn] = FactionRank.Officer;
                    updatePlayerRankinDB(pp, FactionRank.Officer);
                    break;
                case FactionRankEnum.Officer:
                    PlayerRanks[pn] = FactionRank.General;
                    updatePlayerRankinDB(pp, FactionRank.General);
                    break;
                case FactionRankEnum.General:
                    if (!leaderConfirm) break;
                    String l = GetLeader();
                    PlayerRanks[l] = FactionRank.General;
                    PlayerRanks[pn] = FactionRank.Leader;
                    updatePlayerRankinDB(pp, FactionRank.Leader);
                    updatePlayerRankinDB(l, FactionRank.General);
//                Leader = (pn);
                    break;
            }
        }

        public void DemotePlayer(Player pp)
        {
            FactionRank cr = getPlayerRank(pp);
            String pn = pp.Username.ToLower();
            switch (cr.toEnum())
            {
                case FactionRankEnum.Member:
                    PlayerRanks[pn] = FactionRank.Recruit;
                    updatePlayerRankinDB(pp, FactionRank.Recruit);
                    break;
                case FactionRankEnum.Officer:
                    PlayerRanks[pn] = FactionRank.Member;
                    updatePlayerRankinDB(pp, FactionRank.Member);
                    break;
                case FactionRankEnum.General:
                    PlayerRanks[pn] = FactionRank.Officer;
                    updatePlayerRankinDB(pp, FactionRank.Officer);
                    break;
            }
        }

        public void updatePlayerRankinDB(OpenPlayer p, FactionRank r)
        {
            updatePlayerRankinDB(p.Username, r);
        }

        public void updatePlayerRankinDB(Player p, FactionRank r)
        {
            updatePlayerRankinDB(p.Username, r);
        }

        public void updatePlayerRankinDB(String n, FactionRank rr)
        {
            var c = CyberCoreMain.GetInstance().SQL;
            try
            {
                c.Insert($"UPDATE INTO Master SET rank = '{rr.getName()}'WHERE player LIKE '{n}'");
            }
            catch (Exception e)
            {
                CyberCoreMain.Log.Error($"Faction Error {getName()} >> ", e);
                Console.WriteLine(
                    "Error sending plots to DB FOR RANK UPDATE!!! Please report Error 'E190 t'o an admin");
            }
        }

        public Faction GetAllyFromName(String name)
        {
            String found = null;
            name = name.ToLower();
            int delta = 2147483647;
            List<String> var1 = LC.getAllies();

            foreach (var v in var1)
            {
                String fn = v;
                if (fn.ToLower().StartsWith(name) || fn.ToLower().Contains(name))
                {
                    int curDelta = fn.Length - name.Length;
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
            return PlayerRanks.Count;
        }

        public void KickPlayer(Player p)
        {
            String pn = p.getName();
            if (!PlayerRanks.ContainsKey(pn))
            {
                Console.WriteLine("Error! " + pn + " Dose not exist in Faction " + getName());
                return;
            }

            var c = CyberCoreMain.GetInstance().SQL;
            try
            {
                c.Insert($"DELETE FROM Master WHERE player LIKE '{pn}' AND faction LIKE '{getName()}'");
            }
            catch (Exception e)
            {
                CyberCoreMain.Log.Error("Error tring to delete player from DB! Please report Error 'E22D t'o an admin",
                    e);
            }

            PlayerRanks.Remove(pn);
            BroadcastMessage(FactionsMain.NAME + ChatColors.Yellow + p.getName() +
                             " has been  kicked from the faction!");
            p.SendMessage(FactionsMain.NAME + ChatColors.Yellow + $"You Have Been Kicked From {getDisplayName()}!!!");
            getSettings().TakePower(2);
        }


        public static ILog Log { get; private set; } = LogManager.GetLogger(typeof(Faction));

        public void SendFactionChatWindow(Player cp)
        {
            cp.SendForm(new FactionChatFactionWindow(CyberUtils.cloneListString(LastFactionChat)));
        }

        public void HandleFactionChatWindow(String frc, Player cp)
        {
            if (frc == null)
            {
                Console.WriteLine("Error @ 12255");
                return;
            }

            String msg = frc;
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
            return FactionsMain.GetInstance().FFactory.PM.getFactionPlots(getName());
        }

        public bool AddPlots(int chunkx, int chunkz, Player player)
        {
            if (!FactionsMain.GetInstance().FFactory.PM.addPlot(chunkx, chunkz, getName()))
            {
                player.SendMessage("Error! That plot is claimed E:33421");
                return false;
            }

            return true;
        }

        public bool DelPlots(int chunkx, int chunkz, Player playe)
        {
            return DelPlots(chunkx, chunkz, playe, false);
        }

        public bool DelPlots(int chunkx, int chunkz, Player player, bool overclaim)
        {
            if (!FactionsMain.GetInstance().FFactory.PM.delPlot(chunkx, chunkz, getName(), overclaim))
            {
                player.SendMessage("Error deleting Plot E339211");
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


        public void HandleKillEvent(EntityKilledEvent e)
        {
            if (GetActiveMission() != null && e.Entity.HealthManager.LastDamageSource is Player)
            {
                Player k = (Player) e.Entity.HealthManager.LastDamageSource;
                if (IsInFaction(k)) GetActiveMission().AddKill();
            }
        }

        public void HandleBreakEvent(BlockBreakEvent e)
        {
            if (GetActiveMission() != null)
            {
                GetActiveMission().BreakBlock(e);
            }
        }

        public void HandlePlaceEvent(BlockPlaceEvent e)
        {
            if (GetActiveMission() != null)
            {
                GetActiveMission().PlaceBlock(e);
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
                SetActiveMission(int.Parse(id));
            }
        }

        public void AcceptNewMission(int id, Player Sender)
        {
            if (GetActiveMission() != null)
            {
                Sender.SendMessage(FactionsMain.NAME + ChatColors.Red + "Error you already have a mission!!");
                return;
            }

            if (CompletedMissionIDs.Contains(id))
            {
                Sender.SendMessage(FactionsMain.NAME + ChatColors.Red +
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
//                BroadcastMessage(FactionsMain.NAME+ChatColors.Aqua+mission.name+ChatColors.Green+" Faction mission accepted!");
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
                RetrieveActiveMission(int.Parse(id));
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
//                        ActiveMission activeMission = new ActiveMission(Main, this,(Dictionary<String,Object>) Main.AM.get(getName()));
//                        SetActiveMission(activeMission);
//                        //SetActiveMission(new ActiveMission(Main,this,mission));
//                        BroadcastMessage(FactionsMain.NAME+ChatColors.Aqua+mission.name+ChatColors.Green+" Faction mission accepted!");
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
            CompletedMissionIDs.Add(mission.id);
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
            CompletedMissionIDs.Add(mission);
        }
//
//    public void SetMoney(int value) {
////        Money = value;
//        Connection c = CyberCoreMain.GetInstance().FM.FFactory.getMySqlConnection();
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
//        Connection c = CyberCoreMain.GetInstance().FM.FFactory.getMySqlConnection();
//        try {
//            Statement s = c.createStatement();
//            MySqlDataReader r = s.executeQuery("SELECT Money FROM Settings WHERE Name LIKE '" + getName() + "'");
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
//            t = ChatColors.Green + "Gained +" + dif;
//        } else {
//            t = ChatColors.Red + "Lost -" + Math.abs(dif);
//        }
//        BroadcastPopUp(ChatColors.Gray + "Faction now has " + ChatColors.Green + value + ChatColors.Gray + " PowerAbstract!" + t);
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

        public Dictionary<String, HomeData> GetHome(bool force = false)
        {
            if (HomeCC.needsUpdate() || HomeCacheData == null || force)
            {
                HomeCC.updateLastUpdated();
                Dictionary<String, HomeData> f = new Dictionary<String, HomeData>();

                var a = Main.CCM.SQL.executeSelect($"SELECT * FROM `Homes` WHERE `faction` LIKE '{getName()}'");
                foreach (var aa in a)
                {
                    int hid = (int) aa["homeid"];
                    String name = (string) aa["name"];
                    String lvln = (string) aa["level"];
                    String faction = (string) aa["faction"];
                    int xx = (int) aa["x"];
                    int yy = (int) aa["y"];
                    int zz = (int) aa["z"];
                    HomeData h = HomeData.phrase(new Vector3(xx, yy, zz), lvln, name, faction);
                    h.HomeID = hid;
                    if (h.isValid())
                    {
                        f[name] = h;
                    }
                }

                HomeCacheData = f;
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
        // public Dictionary<String, Position> GetHome_V1()
        // {
        //     Dictionary<String, Position> f = new Dictionary<>();
        //     String h = (String) GetFromSettings("Home");
        //     if (h.contains("\\]"))
        //     {
        //         String[] a = h.split("\\]");
        //         for (String aa in //         a) {
        //             String[] h1 = aa.split("\\|");
        //             if (h1.length == 5)
        //             {
        //                 try
        //                 {
        //                     int x = int.parseInt(h1[0]);
        //                     int y = int.parseInt(h1[1]);
        //                     int z = int.parseInt(h1[2]);
        //                     String lvl = h1[3];
        //                     Level l = Server.GetInstance().getLevelByName(lvl);
        //                     if (l == null)
        //                     {
        //                         CyberCoreMain.Log.Error(
        //                             "COULD NOT LOAD FACCCTION HOME FOR " + getName() + " BECAUSE HOME AT " + x +
        //                             " | " + y + " | " + z + " LEVEL NAME IS NOT VALID!!! LEVEVL NAME:" + lvl);
        //                         continue;
        //                     }
        //
        //                     String nme = h1[4];
        //                     f.put(nme, new Position(x, y, z, l));
        //                 }
        //                 catch (Exception e)
        //                 {
        //                     CyberCoreMain.Log.Error(
        //                         "Error! Exception While tring to get " + getName() + "'s Faction Homes!", e);
        //                 }
        //             }
        //             else
        //             {
        //                 CyberCoreMain.Log.Error("Home Syntax error for " + aa);
        //             }
        //         }
        //     }
        //     else
        //     {
        //         String[] hhh = h.split("\\|");
        //         if (hhh.length == 5)
        //         {
        //             try
        //             {
        //                 int x = int.parseInt(hhh[0]);
        //                 int y = int.parseInt(hhh[1]);
        //                 int z = int.parseInt(hhh[2]);
        //                 String lvl = hhh[3];
        //                 Level l = Server.GetInstance().getLevelByName(lvl);
        //                 if (l == null)
        //                 {
        //                     CyberCoreMain.Log.Error(
        //                         "111COULD NOT LOAD FACCCTION HOME FOR " + getName() + " BECAUSE HOME AT " + x +
        //                         " | " + y + " | " + z + " LEVEL NAME IS NOT VALID!!! LEVEVL NAME:" + lvl);
        //                     return null;
        //                 }
        //
        //                 String nme = hhh[4];
        //                 f.put(nme, new Position(x, y, z, l));
        //             }
        //             catch (Exception e)
        //             {
        //                 CyberCoreMain.Log.Error(
        //                     "111Error! Exception While tring to get " + getName() + "'s Faction Homes!", e);
        //             }
        //         }
        //         else
        //         {
        //             CyberCoreMain.Log.Error("111Home Syntax error for " + h);
        //         }
        //     }
        //
        //     return f;
        // }
        public bool DelHome(int h)
        {
            Main.CCM.SQL.Insert($"DELETE * FROM `Homes` WHERE `homeid` = {h};");
            return true;
        }

        public bool addHome(HomeData h)
        {
            if (!h.isValid())
            {
                Console.WriteLine("H NOT VALIDDDDDDDDDD!!!!!!!!!!!!!!!!!!!!!!!!!!!!! E3323109");
                return false;
            }

            Vector3 hv = h.getVector3();
            Main.CCM.SQL.Insert($"INSERT INTO `Homes` VALUES (null,{hv.X},{hv.Y},{hv.Z},'{h.getL().LevelName}," +
                                $"'{h.getFaction()},'{h.getName()}') ;");

            HomeCC.invalidate();

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

        // public Dictionary<String, Object> GetWarData()
        // {
        //     if (War != null && Main.War.ContainsKey(War))
        //     {
        //         return (Dictionary<String, Object>) Main.War.get(War);
        //     }
        //
        //     return null;
        // }

        public bool AtWar()
        {
            return War != null;
        }

        // public bool AtWar(String fac)
        // {
        //     if (War != null)
        //     {
        //         return ((Dictionary<String, Object>) Main.War.get(War)).GetString("defenders").equalsIgnoreCase(fac);
        //     }
        //
        //     return false;
        // }

        /**
     * @param fac    Faction to be added as enemy
     * @param player Player who added the Faction as an Emeney
     */
        public void AddEnemy(Faction fac, Player player)
        {
            if (!FactionsMain.GetInstance().FFactory.RM.addEnemyRelationship(getName(), fac.getName()))
            {
                player.SendMessage("Error adding faction as Enemy!! E800");
                return;
            }

            fac.BroadcastMessage(getSettings().getDisplayName() + " has added your faction as an enemy!");
            BroadcastMessage(fac.getSettings().getDisplayName() + " has been set as an Enemy of your faction by " +
                             player.DisplayName);
        }


//    public void AddCooldown(int secs){
//        Map<String, Object> cd = Main.CD.getAll();
//        int time = (int)(Calendar.GetInstance().getTime().getTime()/1000);
//        cd.put(getName(),time+secs);
//    }
//    public bool HasWarCooldown(){
//        Map<String, Object> cd = Main.CD.getAll();
//        int time = (int)(Calendar.GetInstance().getTime().getTime()/1000);
//        if (cd.ContainsKey(getName())){
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
        public void RemoveEnemy(Faction fac, Player p)
        {
            if (!FactionsMain.GetInstance().FFactory.RM.removeEnemyRelationship(getName(), fac.getName()))
            {
                p.SendMessage("Error adding faction as Enemy!! E816");
                return;
            }

            fac.BroadcastMessage(getSettings().getDisplayName() + " is no longer an enemy!");
            BroadcastMessage(fac.getSettings().getDisplayName() +
                             " is no longer set as an Enemy of your faction by " + p.DisplayName);
        }

        public List<String> GetEnemies()
        {
            return FactionsMain.GetInstance().FFactory.RM.getFactionEnemy(getName());
        }

        public bool isEnemy(String fac)
        {
            return FactionsMain.GetInstance().FFactory.RM.isEnemy(getName(), fac);
        }

        public bool AddAlly(String fac)
        {
            return FactionsMain.GetInstance().FFactory.RM.addAllyRelationship(getName(), fac);
        }

        public void RemoveAlly(Faction fac)
        {
            if (fac == null) return;
            RemoveAlly(fac.getName());
        }

        public bool RemoveAlly(String fac)
        {
            return FactionsMain.GetInstance().FFactory.RM.removeAllyRelationship(getName(), fac);
        }

        public List<String> GetAllies()
        {
            return FactionsMain.GetInstance().FFactory.RM.getFactionAllies(getName());
        }

        //Can Attack Nuetral but can not Attack Allys!
        public bool isNeutral(Player player)
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

        public bool isAllied(Player player)
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
            return FactionsMain.GetInstance().FFactory.RM.isAllys(getName(), fac);
        }

        public void AddInvite(Player player, long value, Player sender, FactionRank fr)
        {
            if (!addRequest(RequestType.Faction_Invite, null, player, value, sender))
            {
                player.SendMessage(
                    "Error sending Faction Invite Request! Please report Error 'E332FI' to an admin");
                return;
            }

            Invites[player.getName().ToLower()] =
                new Invitation(getName(), player.getName(), sender.getName(), value, fr);


            Main.CCM.SQL.Insert($"INSERT INTO `Requests` VALUES (null,{RequestType.Faction_Invite},'{getName()}'" +
                                $",'{sender.getName()}','{player.getName()}','{fr.getName()}')");
        }

//    public void SetInvite(Map<String, int> Invs) {
//        Invites = Invs;
//    }

//    public Map<String, int> GetInvite() {
//        return Invites;
//    }

        public void DelInvite(String name)
        {
            Main.CCM.SQL.Insert(
                $"DELETE * from `Requests` where `faction` LIKE ' {getName()} ' AND `player` LIKE '{name}' AND `TYPE` = {RequestType.Faction_Invite};");

            Invites.Remove(name);
        }

        public bool AcceptInvite(Player p)
        {
            String name = p.getName();
            Invitation i = HasInvite(name);
            if (i == null)
            {
                //No Invite
                p.SendMessage("Error! You are not invited to join '" + getSettings().getDisplayName() + "'!");
                return false;
            }

            if (!i.isValid())
            {
                //Invite Timed out
                p.SendMessage("Error! You're invite has expired!");
                DelInvite(name);
                return false;
            }

            DelInvite(name);
            FactionRank r = i.getRank();
            switch (r.toEnum())
            {
                case FactionRankEnum.General:
                    addPlayer(p, FactionRankEnum.General, i.getInvitedBy());
                    break;
                case FactionRankEnum.Member:
                    addPlayer(p, FactionRankEnum.Member, i.getInvitedBy());
                    break;
                case FactionRankEnum.Officer:
                    addPlayer(p, FactionRankEnum.Officer, i.getInvitedBy());
                    break;
                default:
                case FactionRankEnum.Recruit:
                    addPlayer(p, FactionRankEnum.Recruit, i.getInvitedBy());
                    break;
            }

            BroadcastMessage(FactionsMain.NAME + ChatColors.Green + name + " Has joined your faction!");
            return true;
        }

        public void DenyInvite(String name)
        {
            DelInvite(name);
        }

        public Invitation HasInvite(Player name)
        {
            return HasInvite(name.getName());
        }

        public Invitation HasInvite(String name)
        {
            var q = Main.CCM.SQL.executeSelect(
                $"select * from `Requests` where `faction` LIKE '{getName()}' AND `player` LIKE '{name}' AND `TYPE` = {RequestType.Faction_Invite};");

            foreach (var a in q)
            {
                return new Invitation(getName(), name, q.GetString("player"),
                    (long) q.GetInt64("expires"),
                    getRankFromString(q.GetString("data")));  
            }

            return null;

//        return Invites.ContainsKey(name.ToLower());
        }

        public String GetLeader()
        {
            foreach (var a in PlayerRanks)
            {
                if (a.Value.toEnum() == Leader.toEnum()) return a.Key;
            }

            Console.WriteLine("Errror!!!!!!! ETF E993");
            return null;
        }

//
//    public void SetLeader(String leader) {
//        Leader = leader;
//    }

        public void removePlayer(Player p)
        {
            Main.CCM.SQL.Insert(
                $"DELETE FROM Master WHERE player LIKE '{p.getName()}'");
            PlayerRanks.Remove(p.getName());
        }

        public bool addPlayer(OpenPlayer p, FactionRankEnum r = FactionRankEnum.Recruit, String invitedby = null)
        {
            return addPlayer(p.getName(), r, invitedby);
        }

        public bool addPlayer(Player p, FactionRankEnum r = FactionRankEnum.Recruit, String invitedby = null)
        {
            return addPlayer(p.getName(), r, invitedby);
        }

        public bool addPlayer(CorePlayer p, FactionRankEnum r = FactionRankEnum.Recruit, String invitedby = null)
        {
            return addPlayer(p.getName(), r, invitedby);
        }

        public bool addPlayer(String p, FactionRankEnum r = FactionRankEnum.Recruit, String invitedby = null)
        {
            if (invitedby != null)
            {
                invitedby = "'" + invitedby + "'";
            }
            else invitedby = "null";

            if (CyberCoreMain.GetInstance().FM.FFactory.isPlayerInFaction(p))
            {
                //Playing in faction
                Player pp = Main.CCM.getAPI().PlayerManager.getPlayer(p);
                if (pp != null)
                    pp.SendMessage("Error you are currently in a faction and can not join a new one!!!");
                return false;
            }


            Main.CCM.SQL.Insert(
                $"INSERT INTO Master VALUES ('{p}','{getName()}',{CyberUtils.getTick()},{invitedby},'{r.toFactionRank().getName()}')");
            PlayerRanks[p] = r.toFactionRank();
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
            if (PlayerRanks.ContainsKey(n)) return PlayerRanks[n].hasPerm(FactionRank.Recruit);
            return false;
        }

        public bool IsMember(String n)
        {
            if (PlayerRanks.ContainsKey(n)) return PlayerRanks[n].hasPerm(FactionRank.Member);
            return false;
        }

        public bool IsOfficer(String n)
        {
            if (PlayerRanks.ContainsKey(n)) return PlayerRanks[n].hasPerm(FactionRank.Officer);
            return false;
        }

        public bool IsGeneral(String n)
        {
            if (PlayerRanks.ContainsKey(n)) return PlayerRanks[n].hasPerm(FactionRank.General);
            return false;
        }

        public bool IsInFaction(Player player)
        {
            return IsInFaction(player.getName());
        }

        public bool IsInFaction(String n)
        {
            foreach (String m in PlayerRanks.Keys)
                if (n.equalsIgnoreCase(m))
                    return true;
            return false;
        }

        public void MessageAllys(String message)
        {
            BroadcastMessage(message);
            foreach (String ally in GetAllies())
            {
                Faction af = Main.FFactory.getFaction(ally);
                if (af != null) af.BroadcastMessage(message);
            }
        }

        public String GetFactionNameTag(String p)
        {
            FactionRank fr = getPlayerRank(p);
            return fr.GetChatPrefix() + ChatFormatting.Reset + " - " + getSettings().getDisplayName();
        }

        public String GetFactionNameTag(Player p)
        {
            FactionRank fr = getPlayerRank(p);
            return fr.GetChatPrefix() + ChatFormatting.Reset + " - " + getSettings().getDisplayName();
        }

        public void BroadcastMessage(String message)
        {
            BroadcastMessage(message, FactionRank.All);
        }

        CacheChecker PRC = new CacheChecker("PR", 120);

        public FactionRank getPlayerRank(String p)
        {
//        PlayerRanks
            if (PlayerRanks.ContainsKey(p)) return PlayerRanks[p];

            var q = Main.CCM.SQL.executeSelect($"SELECT  * FROM Master WHERE player LIKE '{p}'");
            if (q.Read())
            {
                FactionRank fr = getRankFromString(q.GetString("rank"));
                return fr;
            }

            Console.WriteLine("Error! Could not get rank from DB!!! E113e");
            return None;
        }

        public FactionRank getPlayerRank(OpenPlayer p)
        {
            return getPlayerRank(p.getName().ToLower());
        }

        public FactionRank getPlayerRank(Player p)
        {
            return getPlayerRank((Player) p);
        }

        public void BroadcastMessage(String message, FactionRank rank)
        {
            foreach (var a in PlayerRanks)
            {
                if (a.Value.hasPerm(rank))
                {
                    Player p = Main.CCM.getPlayer(a.Key);
                    if (p == null) continue;
                    p.SendMessage(message);
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
            foreach (var a in PlayerRanks)
            {
                if (a.Value.hasPerm(r))
                {
                    CorePlayer p = Main.CCM.getPlayer(a.Key);
                    if (p == null) continue;
                    p.AddPopup(new Popup() {Message = message + "\n" + subtitle});
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
            List<Player> aa = new List<Player>();
            foreach (var a in PlayerRanks)
            {
                Player p = Main.CCM.getPlayer(a.Key);
                if (p == null) continue;
                aa.Add(p);
            }

            return aa;
        }

        public String BossBarText()
        {
            /*return ChatColors.GOLD+""+ChatColors.BOLD+"====§eTERRA§6TIDE===="+ChatColors.RESET+"\n\n"+
                    "§6"+GetDisplayName()+" §b: §aLEVEL §b: §3"+GetLevel()+"\n"+
                     "§eXP §b: §6"+GetXP()+" §a/ §b"+calculateRequireExperience(GetLevel());*/
            return ChatColors.Gold + "" + ChatFormatting.Bold + "====§eTERRA§6TIDE====" + ChatFormatting.Reset +
                   "\n\n" +
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
            Main.FFactory.Top[getName()] = getSettings().getMoney();
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
            Main.FFactory.Rich[getName()] = Settings.getRich();
        }

        public void AddAllyRequest(Faction fac)
        {
            AddAllyRequest(fac, null, -1);
        }

        public void AddAllyRequest(Faction fac, Player cp)
        {
            AddAllyRequest(fac, cp, -1);
        }

        public List<AllyRequest> getAllyRequests()
        {
            var q = Main.CCM.SQL.executeSelect(
                $"SELECT * FROM Requestes WHERE type LIKE '{RequestType.Ally.ToString()}' AND target = '{getName()}'");

            List<int> dellist = new List<int>();
            List<AllyRequest> list = new List<AllyRequest>();
            if (q.Count != 0)
            {
                String fn = q.GetString("faction");
                Faction f = FactionsMain.GetInstance().FFactory.getFaction(fn);
                if (f == null)
                {
                    dellist.Add(q.GetInt32("id"));
                }

                AllyRequest ar = new AllyRequest(f, q.GetInt64("expires"));
                list.Add(ar);
            }

            return list;


            return null;
        }

        public bool addRequest(RequestType rt, Faction fac, Player player, long timeout, Player sender)
        {
            String sn = null;
            String pn = null;
            if (sender != null) sn = sender.getName();
            if (player != null) pn = player.getName();
            Main.CCM.SQL.Insert(
                $"INSERT INTO `Requests` VALUES (null,{rt.toEnum()},'{fac.getName()}','{getName()}',{timeout},'{sn}')");
            return true;
        }

        /**
     * Adds ally request to this faction
     *
     * @param fac     Faction Requesting to be an Ally
     * @param player  The Player who snet the Invite
     * @param timeout DateTimem to String when request expires
     */
        public void AddAllyRequest(Faction fac, Player player, int timeout)
        {
            if (!addRequest(RequestType.Ally, fac, null, timeout, player))
            {
                player.SendMessage("Error sending Ally Request! Please report Error 'E332RA' to an admin");
                return;
            }

            //        Connection c = CyberCoreMain.GetInstance().FM.FFactory.getMySqlConnection();
//        try {
//            Statement s = c.createStatement();
//            //1 = Ally Request
//            //0 = Friend Request
//            //2 = ?????
//            //CyberCoreMain.GetInstance().getIntTime
//            s.executeQuery(String.format("INSERT INTO `Requests` VALUES (null,1,'%s',null,'%s'," + timeout + ")", fac.getName(), getName(), timeout));
//            c.close();
////        Main.FFactory.allyrequest.put(getName(), fac.getName());
//        } catch (Exception e) {
//            e.printStackTrace();
//            player.SendMessage("Error sending Ally Request! Please report Error 'E332R' to an admin");
//            return;
//        }


            BroadcastMessage(ChatColors.Aqua + "[ArchFactions] " + fac.getSettings().getDisplayName() +
                             " wants to be Ally's with you!");
//        BroadcastMessage(ChatColors.Aqua + "[ArchFactions] Respond to the request using `/f inbox`");
            player.SendMessage(ChatColors.Aqua + "[ArchFactions] Ally request sent to " +
                               getSettings().getDisplayName());

            AR.Add(new AllyRequest(fac, timeout));
            FactionRank r = getPermSettings().getAllowedToAcceptAlly();
            switch (r.toEnum())
            {
                case FactionRankEnum.Recruit:
                    BroadcastMessage(
                        fac.getSettings().getDisplayName() +
                        "'s Faction would like to become your ally! View the offer in /f inbox", FactionRank.All);
                    break;
                case FactionRankEnum.Member:
                    BroadcastMessage(
                        fac.getSettings().getDisplayName() +
                        "'s Faction would like to become your ally! View the offer in /f inbox",
                        FactionRank.Member);
                    break;
                case FactionRankEnum.Officer:
                    BroadcastMessage(
                        fac.getSettings().getDisplayName() +
                        "'s Faction would like to become your ally! View the offer in /f inbox",
                        FactionRank.Officer);
                    break;
                case FactionRankEnum.General:
                    BroadcastMessage(
                        fac.getSettings().getDisplayName() +
                        "'s Faction would like to become your ally! View the offer in /f inbox",
                        FactionRank.General);
                    break;
                case FactionRankEnum.Leader:
                    BroadcastMessage(
                        fac.getSettings().getDisplayName() +
                        "'s Faction would like to become your ally! View the offer in /f inbox",
                        FactionRank.Leader);
                    break;
            }
        }

        public void AddFactionChatMessage(String message, Player p)
        {
            FactionRank r = getPlayerRank(p);
            message = ChatColors.Gray + "[" + r.GetChatPrefix() + ChatColors.Gray + "] - " + r.getChatColor() +
                      p.DisplayName + ChatColors.Gray + " > " + ChatColors.White + message;
            BroadcastMessage("Faction> " + message);
            LastFactionChat.Insert(0, message);
            if (LastFactionChat.Count > getPermSettings().getMaxFactionChat())
            {
                LastFactionChat.RemoveAt(LastFactionChat.Count - 1);
            }
        }

        public void AddAllyChatMessage(String message, Player p)
        {
            FactionRank r = getPlayerRank(p);
            message = ChatColors.Gray + "[" + r.GetChatPrefix() + ChatColors.Gray + "] - " + r.getChatColor() +
                      p.DisplayName + ChatColors.Gray + " > " + ChatColors.White + message;
            BroadcastMessage("Ally> " + message);
            LastAllyChat.Insert(0, message);
            if (LastAllyChat.Count > getPermSettings().getMaxAllyChat())
            {
                LastAllyChat.RemoveAt(LastAllyChat.Count - 1);
            }
        }

        //    
        public void save()
        {
            //Save Settings
            getSettings().upload();
            //Save Player Ranks
            foreach (var m in PlayerRanks)
            {
                Main.CCM.SQL.Insert(
                    $"UPDATE `Master` SET `rank` = '{m.Value.getName()}' WHERE `Master`.`player` = '{m.Key}'");
            }
            //Save Plots - All Saved immedatelly to the cloud
            //

//
//        Connection c = CyberCoreMain.GetInstance().FM.FFactory.getMySqlConnection();
//        try {
//            getServer().getLogger().error("DELETEING Faction " + fn + "!");
//            Statement stmt = c.createStatement();
//            stmt.executeUpdate(String.format("DELETE FROM `allies` WHERE `factiona` LIKE '%s' OR `factionb` LIKE '%s';", fn, fn));
//            stmt.close();
//        } catch (Exception ex) {
//            getServer().getLogger().info(ex.getClass().getName() + ":9 " + ex.getMessage() + " > " + ex.getStackTrace()[0].getLineNumber() + " ? " + ex.getCause());
//        }
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

        public void addPlayerToGlobalList(Player p, String name)
        {
            FactionsMain.GetInstance().FFactory.FacList[p.getName().ToLower()] = name;
        }

        public class HomeDataFaction : HomeData
        {
            public HomeDataFaction(Vector3 pos, string lvln, string name, String f) : base(pos, lvln, name)
            {
                faction = f;
            }

            public HomeDataFaction(Vector3 pos, Player p, string name, String f) : base(pos, p, name)
            {
                faction = f;
            }
        }

        public class HomeDataPlayer : HomeData
        {
            public HomeDataPlayer(Vector3 pos, string lvln, string name, Player p) : base(pos, lvln, name)
            {
                owner = p.Username;
                owneruuid = p.ClientUuid.ToString();
            }

            public HomeDataPlayer(Vector3 pos, Player p, string name) : base(pos, p, name)
            {
                owner = p.Username;
                owneruuid = p.ClientUuid.ToString();
            }
        }

        public enum HomeDataType
        {
            Unknown = 0,
            Faction = 1,
            Player = 2,
        }

        public class HomeData
        {
            public String LvlName = null;
            public int x = 0, y = 0, z = 0;
            public String name = null;

            public int HomeID = -1;

            //FACTION ONLY
            public String faction = null;

            //PLAYER ONLY
            public String owner = null;
            public String owneruuid = null;

            public String getOwnerName()
            {
                return owner;
            }

            public String getOwnerUUID()
            {
                return owneruuid;
            }

            public String getFactionName()
            {
                return faction;
            }

            public static HomeData phrase(Vector3 pos, string lvln, String name, string fac = null, Player p = null)
            {
                if (fac != null)
                {
                    return new HomeDataFaction(pos, lvln, name, fac);
                }
                else if (p != null)
                {
                    return new HomeDataPlayer(pos, lvln, name, p);
                }
                else
                {
                    return new HomeData(pos, lvln, name);
                }
            }

            public static HomeData phrase(Vector3 pos, Player p, String name, string fac = null)
            {
                if (fac != null)
                {
                    return new HomeDataFaction(pos, p, name, fac);
                }
                else
                {
                    return new HomeDataPlayer(pos, p, name);
                }

                // else
                // {
                //     return new HomeData(pos,p,name);
                // }
            }

            public HomeData(Vector3 pos, string lvln, String name)
            {
                x = (int) pos.X;
                y = (int) pos.Y;
                z = (int) pos.Z;
                LvlName = lvln;
                this.name = name;
                var ln = lvln;
                Level tl = CyberCoreMain.GetInstance().getAPI().LevelManager.GetLevel(null, ln);
                if (tl == null)
                {
                    CyberCoreMain.Log.Error("Error! HOME DATA FOR FACTION " + getFaction() + " HOME AT " +
                                            getVector3() + " WITH INVALID LEVEL NAME: " + ln);
                }

                LvlName = ln;
            }

            public HomeData(Vector3 pos, Player p, String name)
            {
                x = (int) pos.X;
                y = (int) pos.Y;
                z = (int) pos.Z;
                LvlName = p.Level.LevelName;
                this.name = name;
                var ln = p.Level.LevelName;
                Level tl = CyberCoreMain.GetInstance().getAPI().LevelManager.GetLevel(null, ln);
                if (tl == null)
                {
                    CyberCoreMain.Log.Error("Error! HOME DATA FOR FACTION " + getFaction() + " HOME AT " +
                                            getVector3() + " WITH INVALID LEVEL NAME: " + ln);
                }

                LvlName = ln;
            }

            public HomeData(Player p, String name)
            {
                x = (int) p.KnownPosition.X;
                y = (int) p.KnownPosition.Y;
                z = (int) p.KnownPosition.Z;
                LvlName = p.Level.LevelName;
                this.name = name;
                var ln = p.Level.LevelName;
                Level tl = CyberCoreMain.GetInstance().getAPI().LevelManager.GetLevel(null, ln);
                if (tl == null)
                {
                    CyberCoreMain.Log.Error("Error! HOME DATA FOR FACTION " + getFaction() + " HOME AT " +
                                            getVector3() + " WITH INVALID LEVEL NAME: " + ln);
                }

                LvlName = ln;
            }
            // public HomeData(int x, int y, int z, Level lvlName, String name, String fac, int hid)
            // {
            //     HomeID = hid;
            //     this.x = x;
            //     this.y = y;
            //     this.z = z;
            //     this.LvlName = lvlName;
            //     faction = fac;
            //     this.name = name;
            // }
            //
            // public HomeData(int x, int y, int z, String ln, String name, String fac, int hid)
            // {
            //     HomeID = hid;
            //     this.x = x;
            //     this.y = y;
            //     this.z = z;
            //     Level tl = CyberCoreMain.GetInstance().getAPI().LevelManager.GetLevel(null,ln);
            //     if (tl == null)
            //     {
            //         CyberCoreMain.Log.Error("Error! HOME DATA FOR FACTION " + getFaction() + " HOME AT " +
            //                    getVector3() + " WITH INVALID LEVEL NAME: " + ln);
            //     }
            //     else LvlName = tl;
            //
            //     faction = fac;
            //     this.name = name;
            // }

            public int getHomeID()
            {
                return HomeID;
            }

            public Vector3 getVector3()
            {
                return new Vector3(x, y, z);
            }

            public PlayerLocation getPosition()
            {
                if (getL() == null) return null;
                return new PlayerLocation(x, y, z);
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

            public Level getL()
            {
                return CyberCoreMain.GetInstance().getAPI().LevelManager.GetLevel(null, LvlName);
                // return l;
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
//        Connection c = CyberCoreMain.GetInstance().FM.FFactory.getMySqlConnection();
//        try {
//            Statement s = c.createStatement();
//            //1 = Ally Request
//            //0 = Friend Request
//            //2 = ?????
//            //CyberCoreMain.GetInstance().getIntTime
//            s.executeQuery(String.format("INSERT INTO `Requests` VALUES (null,1,'%s',null,'%s'," + timeout + ")", fac.getName(), getName(), timeout));
//            c.close();
////        Main.FFactory.allyrequest.put(getName(), fac.getName());
//        } catch (Exception e) {
//            e.printStackTrace();
//            player.SendMessage("Error sending Ally Request! Please report Error 'E332R' to an admin");
//            return;
//        }

        public class AllyRequest
        {
            long Timeout;
            Faction F;


            public AllyRequest(Faction f, long timeout = -1)
            {
                F = f;
                Timeout = timeout;
            }
        }
    }
}
//
// [AttributeUsage(AttributeTargets.Field)]
// public class NonVolatileAttribute : Attribute
// {
// }