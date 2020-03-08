using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Factions2;
using MiNET;
using MiNET.Utils;
using Newtonsoft.Json.Linq;

namespace Faction2
{
    public class Faction
    {
        public enum FactionPosition
        {
            Slave = 1,
            Recruit = 2,
            Member = 3,
            Officer = 4,
            Generals = 5,
            Leader = 6
        };

        private Faction_main _main;
        public string _name { get; private set; }
        public string _displayName { get; private set; }
        public string Leader { get; private set; }
        public List _recruits { get; private set; }
        public List _members { get; private set; }
        public List _officers { get; private set; }
        public List _generals { get; private set; }
        public string _motd { get; private set; }
        public string _desc { get; private set; }
        public List _fChat { get; private set; } = new List();
        public List _fAlly { get; private set; } = new List();
        public List _plots { get; private set; } = new List();
        public List _allies { get; private set; } = new List();
        public List _enemies { get; private set; } = new List();
        public string _war { get; private set; } = null;
        public IDictionary<string, long> _invites { get; private set; } = new Dictionary<string, long>();
        public int _maxPlayers { get; private set; } = 15;
        public int _powerBonus { get; private set; } = 1;
        public int _privacy { get; private set; } = 0;
        public int _perms { get; private set; } = 0;
        public int _power { get; private set; } = 0;
        public int _rich { get; private set; } = 0;

        //Active Mission ID
        public int AMID { get; private set; } = -1;

        public int Points { get; private set; } = 0;
        public int XP { get; private set; } = 0;
        public int Level { get; private set; } = 0;

        public List _completedMissionIDs { get; private set; } = new List();

        public ActiveMission AM { get; private set; } = null;

        public int _money { get; private set; } = 0;
        public PlayerLocation Home { get; private set; } = new PlayerLocation(0, 0, 0);

        public Faction(Faction_main main, string name, string displayname, string leader, List members,
            List generals, List officers, List recruits)
        {
            _main = main;
            _name = name;
            _displayName = displayname;
            Leader = leader;
            _members = members;
            _recruits = recruits;
            _generals = generals;
            _officers = officers;
            Player p;
            foreach (string m in _members)
            {
                p = _main.Server.LevelManager.FindPlayer(m);
                if (p != null)
                {
                    _main.FF.FacList.Add(p.Username.ToLower(), _name);
                }
            }
            p = null;
            foreach (string m in _officers)
            {
                p = _main.Server.LevelManager.FindPlayer(m);
                if (p != null)
                {
                    _main.FF.FacList.Add(p.Username.ToLower(), _name);
                }
            }
            p = null;
            foreach (string m in _generals)
            {
                p = _main.Server.LevelManager.FindPlayer(m);
                if (p != null)
                {
                    _main.FF.FacList.Add(p.Username.ToLower(), _name);
                }
            }
            p = null;
            foreach (string m in _recruits)
            {
                p = _main.Server.LevelManager.FindPlayer(m);
                if (p != null)
                {
                    _main.FF.FacList.Add(p.Username.ToLower(), _name);
                }
            }
            p = _main.Server.LevelManager.FindPlayer(Leader);
            if (p != null) _main.FF.FacList.Add(p.Username.ToLower(), _name);
        }

        public override string ToString()
        {
            return GetName();
        }

        public Faction SetMain(Faction_main m)
        {
            _main = m;
            return this;
        }

        public void SetPlots(List value)
        {
            _plots = value;
        }

        public List GetPlots()
        {
            return _plots;
        }

        public void AddPlots(string plot)
        {
            _plots.Add(plot);
        }

        public void DelPlots(string plot)
        {
            _plots.Remove(plot);
        }

        public void SetMaxPlayers(int value)
        {
            _maxPlayers = value;
        }

        public int GetMaxPlayers()
        {
            return _maxPlayers;
        }

        public void SetPowerBonus(int value)
        {
            _powerBonus = value;
        }

        public int GetPowerBonus()
        {
            return _powerBonus;
        }

        public int CalculateMaxPower()
        {
            int TP = GetNumberOfPlayers();
            return TP*10;
            //Lets do 20 Instead of 10
        }

        public int GetNumberOfPlayers()
        {
            return _generals.Count + _officers.Count + _members.Count + 1;
        }

        public void SetPrivacy(int value)
        {
            _privacy = value;
        }

        public int GetPrivacy()
        {
            return _privacy;
        }


        public void SetDesc(string value)
        {
            _desc = value;
        }

        public string GetDesc()
        {
            return _desc;
        }


        // ReSharper disable once InconsistentNaming
        public void SetMOTD(string value)
        {
            _motd = value;
        }

        // ReSharper disable once InconsistentNaming
        public string GetMOTD()
        {
            return _motd;
        }

        public void SetPerm(int value)
        {
            _perms = value;
        }

        public int GetPerm()
        {
            return _perms;
        }

        public int GetPerm(int key)
        {
            try
            {
                return int.Parse((_perms + "").Substring(key));
            }
            catch (Exception ignore)
            {
                return -1; //or 0
            }
        }

        public void SetPoints(int value)
        {
            Points = value;
        }

        public int GetPoints()
        {
            return Points;
        }

        public void AddPoints(int pts)
        {
            SetPoints(GetPoints() + Math.Abs(pts));
        }

        public void TakePoints(int pts)
        {
            int a = GetPoints() - pts;
            if (a < 0) SetPoints(0);
            SetPoints(a);
        }

        public void SetLevel(int value)
        {
            Level = value;
            UpdateBossBar();
        }

        public int GetLevel()
        {
            return Level;
        }

        public String GetName()
        {
            return _name;
        }

        public void AddLevel(int lvl)
        {
            SetLevel(GetLevel() + Math.Abs(lvl));
        }

        public void TakeLevel(int lvl)
        {
            int a = GetLevel() - lvl;
            if (a < 0) SetLevel(0);
            SetLevel(a);
        }

        public int calculateRequireExperience(int level)
        {
            if (level >= 30)
                return 112 + (level - 30)*9*100;
            if (level >= 15)
                return 37 + (level - 15)*5*100;
            return 7 + level*2*100;
        }

        public void CalculateXp()
        {
            int xp = GetXp();
            int lvl = GetLevel();
            while (xp >= calculateRequireExperience(lvl))
            {
                xp = xp - calculateRequireExperience(lvl);
                lvl++;
            }
            SetXP(xp);
            SetLevel(lvl);
        }

        public void AddXp(int add)
        {
            if (add == 0) return;
            int now = GetXp();
            int Added = now + add;
            int level = GetLevel();
            int most = calculateRequireExperience(level);
            while (Added >= most)
            {
//Level Up!
                Added = Added - most;
                most = calculateRequireExperience(++level);
            }
            SetXP(Added);
            SetLevel(level);
        }

        public void SetXP(int value)
        {
            XP = value;
            UpdateBossBar();
        }

        public void SetXPCalculate(int value)
        {
            int level = GetLevel();
            int most = calculateRequireExperience(level);
            while (value >= most)
            {
//Level Up!
                value = value - most;
                most = calculateRequireExperience(++level);
            }
            SetXP(value);
            SetLevel(level);
        }

        public int GetXpPercent()
        {
            Double d = ((XP/(double) calculateRequireExperience(GetLevel()))*100);
            return (int) d;
        }

        public int GetXp()
        {
            return XP;
        }

        public bool TakeXp(int xp)
        {
            int ox = GetXp();
            int olvl = GetLevel(); 
            int x = GetXp();
            int lvl = GetLevel();
            while (x < xp)
            {
                if (lvl == 0)
                {
                    SetXP(ox);
                    SetLevel(olvl);
                    return false;
                }
                x += calculateRequireExperience(--lvl);
            }
            int a = x - xp;
            SetXP(a);
            SetLevel(lvl);
            return true;
        }
//
//        public void RegisterEvents()
//        {
//            Player p;
//            p.PlayerLeave;
//        }

        //TODO
        //Just create a method that is a PLayer Tick Listener Event that listens foreach a players Health Manager to say it has died!
        /*public void HandleKillEvent(PlayerDeathEvent eevent) {
            if (GetActiveMission() != null)
            {
                GetActiveMission().AddKill();
            }
        }
        public void HandleBreakEvent(BlockBreakEvent eevent) {
            if (GetActiveMission() != null)
            {
                GetActiveMission().BreakBlock(eevent);
            }
        }
        public void HandlePlaceEvent(BlockPlaceEvent eevent)
        {
            if (GetActiveMission() != null)
            {
                GetActiveMission().PlaceBlock(eevent);
            }
        }*/

        /*
        public void SetActiveMission()
            {
                SetActiveMission(-1);
            }
            public void SetActiveMission(int id)
            {
                if (id == -1) return;
                    SetActiveMission(id);
            }
            */
        public void AcceptNewMission(int id, Player sender)
        {
            if (GetActiveMission() != null)
            {
                sender.SendMessage(Faction_main.NAME + ChatColors.Red + "Error you already have a mission!!");
                return;
            }
            if (_completedMissionIDs.Contains(id))
            {
                sender.SendMessage(Faction_main.NAME + ChatColors.Red +
                                   "Error you have already completed this mission!!");
                return;
            }
            SetActiveMission(id);
        }

        public void SetActiveMission(int id)
        {
            foreach (Mission mission in _main.Missions)
            {
                if (mission.id == id)
                {
                    SetActiveMission(new ActiveMission(_main, this, mission));
                    BroadcastMessage(Faction_main.NAME + ChatColors.Aqua + mission.name + ChatColors.Green +
                                     " Faction mission accepted!");
                }
            }
        }

        /// <summary>
        /// Get Player's Current Active Mission form FactionMain Dictionary
        /// </summary>
        /// <param name="id">Player's Name</param>
        public ActiveMission RetrieveActiveMission(Player p)
        {
            return RetrieveActiveMission(p.Username);
        }

        public ActiveMission RetrieveActiveMission(string name)
        {
            if (_main.AM.ContainsKey(name))
            {
                AM = _main.AM[name];
            }
            return SetActiveMission();
        }

        public ActiveMission SetActiveMission()
        {
            AM = null;
            return null;
        }

        public ActiveMission SetActiveMission(ActiveMission mission)
        {
            AM = mission;
            _main.AM.Add(_name, mission);
            return mission;
        }

        public ActiveMission GetActiveMission()
        {
            return AM;
        }

        public void CompleteMission(ActiveMission mission)
        {
            _completedMissionIDs.Add(mission.id);
            AM = null;
        }

        public void SetCompletedMissisons(List value)
        {
            _completedMissionIDs = value;
        }

        public List GetCompletedMissions()
        {
            return _completedMissionIDs;
        }

        public void AddCompletedMission(int mission)
        {
            _completedMissionIDs.Add(mission);
        }

        public void SetMoney(int value)
        {
            _money = value;
            UpdateTopResults();
        }

        public int GetMoney()
        {
            return _money;
        }

        public void AddMoney(int money)
        {
            SetMoney(GetMoney() + Math.Abs(money));
        }

        public void TakeMoney(int money)
        {
            int a = GetMoney() - money;
            if (a < 0) SetMoney(0);
            SetMoney(a);
        }

        public int GetRich()
        {
            return _rich + GetMoney();
        }

        public void SetRich(int rich)
        {
            _rich = rich;
        }

        public void CalcualteRich()
        {
//Level lvl = __main.Server.getLevelByName("world");

//_main.Server.getScheduler().scheduleAsyncTask(new FactionRichAsyncSingle(_main,lvl,this));
/*int value = 0;
if(lvl == null)return value;
for(string plot: GetPlots()){
string key = plot.split("\\|")[0] + "|" + plot.split("\\|")[1];
    int sx = int.parseInt(plot.split("\\|")[0]) << 4;
    int sz = int.parseInt(plot.split("\\|")[1]) << 4;
    foreach (int x = 0; x < 64; x++) {
        foreach (int y = 0; y < 128; y++) {
            foreach (int z = 0; z < 64; z++) {
                int fx = x + sx;
                int fz = z + sz;
                Block b = lvl.getBlock(new PlayerLocation(fx,y,fz));
                string kkey = "";
                if(b.getDamage() != 0){
                    kkey = b.getId() + "|" + b.getDamage();
                }else{
                    kkey = b.getId()+"";
                }
                if(_main.BV.exists(kkey))value += (int) __main.BV.get(kkey);
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

        public void SetPower(int value)
        {
            int dif = value - GetPower();
            string t = "";
            if (dif > 0)
            {
                t = ChatColors.Green + "Gained +" + dif;
            }
            else
            {
                t = ChatColors.Red + "Lost -" + Math.Abs(dif);
            }
            BroadcastPopUp(ChatColors.Gray + "Faction now has " + ChatColors.Green + value + ChatColors.Gray +
                           " Power!" + t);
            _power = value;
        }

        public int GetPower()
        {
            return _power;
        }

        public void AddPower(int power)
        {
            int t = GetPower() + Math.Abs(power);
            if (t > CalculateMaxPower())
            {
                SetPower(CalculateMaxPower());
            }
            else
            {
                SetPower(t);
            }
        }

        public void TakePower(int power)
        {
            int a = GetPower() - power;
            if (a < 0)
            {
                SetPower(0);
            }
            else
            {
                SetPower(a);
            }
        }

        public PlayerLocation GetHome()
        {
            return Home;
        }

        public void SetHome(int x, int y, int z)
        {
            SetHome(new PlayerLocation(x, y, z));
        }

        public void SetHome(PlayerLocation pos)
        {
            Home = pos;
        }

        public void StartWar(string key)
        {
            _war = key;
        }

        public void EndWar()
        {
            _war = null;
        }

        //TODO WAR
//        public JObject GetWarData()
//        {
//            if (_war != null && __main.War.Contains(_war))
//            {
//                return (JObject) __main.War.(_war);
//            }
//            return null;
//        }
//
//        public bool AtWar()
//        {
//            if (_war != null) return true;
//            return false;
//        }
//
//        public bool AtWar(string fac)
//        {
//            if (_war != null)
//            {
//                if (((ConfigSection) __main.War.get(_war)).getstring("defenders").equalsIgnoreCase(fac))
//                {
//                    return true;
//                }
//            }
//            return false;
//        }
//
//        public void AddCooldown(int secs)
//        {
//            Map<string, Object> cd = __main.CD.getAll();
//            int time = (int) (Calendar.getInstance().getTime().getTime()/1000);
//            cd.Add(Username, time + secs);
//        }
//
//        public bool HasWarCooldown()
//        {
//            Map<string, Object> cd = __main.CD.getAll();
//            int time = (int) (Calendar.getInstance().getTime().getTime()/1000);
//            if (cd.ContainsKey(Username))
//            {
//                if (time >= (int) cd.get(Username))
//                {
//                    cd.Remove(Username);
//                    return false;
//                }
//                return true;
//            }
//            return false;
//        }

        public void SetEnemies(List list)
        {
            _enemies = list;
        }

        public void AddEnemy(string fac)
        {
            _enemies.Add(fac);
        }

        public void RemoveEnemy(string fac)
        {
            _enemies.Remove(fac);
        }

        public List GetEnemies()
        {
            return _enemies;
        }

        public bool isEnemy(string fac)
        {
            if (_enemies.Contains(fac.ToLower())) return true;
            return false;
        }

        public void SetAllies(List list)
        {
            _allies = list;
        }

        public void AddAlly(string fac)
        {
            _allies.Add(fac);
        }

        public void RemoveAlly(string fac)
        {
            _allies.Remove(fac);
        }

        public List GetAllies()
        {
            return _allies;
        }

        public bool isAllied(Faction fac)
        {
            return isAllied(fac.GetName());
        }
        public bool isAllied(string fac)
        {
            if (_allies.Contains(fac.ToLower())) return true;
            return false;
        }

        public void SetInvite(Dictionary<string, long> Invs)
        {
            _invites = Invs;
        }

        public IDictionary<string, long> GetInvite()
        {
            return _invites;
        }

        public void AddInvite(string Key, long Value)
        {
            _invites.Add(Key, Value);
        }

        public void DelInvite(string Key)
        {
            _invites.Remove(Key);
        }

        public bool AcceptInvite(string name)
        {
            if (_invites[name] > new DateTime().Ticks)
            {
                _members.Add(name.ToLower());
                DelInvite(name);
                return true;
            }
            DelInvite(name);
            return false;
        }

        public void DenyInvite(string name)
        {
            DelInvite(name);
        }

        public bool HasInvite(string name)
        {
            return _invites.ContainsKey(name);
        }

        public void SetMembers(List members)
        {
            _members = members;
        }

        public void SetOfficers(List members)
        {
            _officers = members;
        }

        public void SetGenerals(List members)
        {
            _generals = members;
        }

        public void SetRecruits(List members)
        {
            _recruits = members;
        }

        public void SetLeader(string leader)
        {
            Leader = leader;
        }

        public List GetMembers()
        {
            return _members;
        }

        public List GetOfficers()
        {
            return _officers;
        }

        public List GetGenerals()
        {
            return _generals;
        }

        public List GetRecruits()
        {
            return _recruits;
        }

        public string GetLeader()
        {
            return Leader;
        }

        public void AddMember(string name)
        {
            _members.Add(name);
        }

        public void AddOfficer(string name)
        {
            _officers.Add(name);
        }

        public void AddGeneral(string name)
        {
            _generals.Add(name);
        }

        public void AddRecruit(string name)
        {
            _recruits.Add(name);
        }

        public void DelMember(string name)
        {
            _members.Remove(name);
        }

        public void DelOfficer(string name)
        {
            _officers.Remove(name);
        }

        public void DelGeneral(string name)
        {
            _generals.Remove(name);
        }

        public void DelRecruit(string name)
        {
            _recruits.Remove(name);
        }

        public bool IsMember(Player p)
        {
            return IsMember(p.Username);
        }

        public bool IsOfficer(Player p)
        {
            return IsOfficer(p.Username);
        }

        public bool IsGeneral(Player p)
        {
            return IsGeneral(p.Username);
        }

        public bool IsRecruit(Player p)
        {
            return IsRecruit(p.Username);
        }

        public bool IsRecruit(string n)
        {
            foreach (string m in GetRecruits())
            {
                if (n == m) return true;
            }
            return false;
        }

        public bool IsMember(string n)
        {
            foreach (string m in GetMembers())
            {
                if (n == m) return true;
            }
            return false;
        }

        public bool IsOfficer(string n)
        {
            foreach (string m in GetOfficers())
            {
                if (n == m) return true;
            }
            return false;
        }

        public bool IsGeneral(string n)
        {
            foreach (string m in GetGenerals())
            {
                if (n == m) return true;
            }
            return false;
        }

        public bool IsInFaction(Player player)
        {
            return IsInFaction(player.Username);
        }

        public bool IsInFaction(string n)
        {
            foreach (string m in GetRecruits())
            {
                if (n == (m)) return true;
            }
            foreach (string m in GetMembers())
            {
                if (n == (m)) return true;
            }
            foreach (string m in GetOfficers())
            {
                if (n == (m)) return true;
            }
            foreach (string m in GetGenerals())
            {
                if (n == m) return true;
            }
            return n == (GetLeader());
        }

        public void SetDisplayName(string val)
        {
            _displayName = val;
        }

        public string GetDisplayName()
        {
            return _displayName;
        }

        public void MessageAllys(string message)
        {
            BroadcastMessage(message, 1);
            foreach (string ally in GetAllies())
            {
                Faction af = _main.FF.GetFaction(ally);
                af?.BroadcastMessage(message, 1);
            }
        }

        public string GetFactionNameTag(string p)
        {
            string prefix = "R";
            if (IsMember(p)) prefix = "M";
            if (IsOfficer(p)) prefix = "O";
            if (IsGeneral(p)) prefix = "G";
            if (Leader == p) prefix = "L";
            return prefix + "-" + _displayName;
        }

        public string GetFactionNameTag(Player p)
        {
            string prefix = "R";
            if (IsMember(p)) prefix = "M";
            if (IsOfficer(p)) prefix = "O";
            if (IsGeneral(p)) prefix = "G";
            if (Leader == p.Username) prefix = "L";
            return prefix + "-" + _displayName;
        }

        /// <summary>
        /// 
        ///
        /// </summary>
        /// <param name="message"></param>
        /// <param name="type"> 0 = N/A; 1 = Allay ; 2 = Fchat; </param>
        public void BroadcastMessage(string message, int type = 0)
        {
            Player p;
            foreach (string m in _members)
            {
                if (_fAlly.Contains(m.ToLower()) && type == 1) continue;
                if (_fChat.Contains(m.ToLower()) && type == 2) continue;
                p = _main.Server.LevelManager.FindPlayer(m);
                if (p != null)
                {
                    p.SendMessage(message);
                }
            }
            foreach (string m in _officers)
            {
                if (_fAlly.Contains(m.ToLower()) && type == 1) continue;
                if (_fChat.Contains(m.ToLower()) && type == 2) continue;
                p = _main.Server.LevelManager.FindPlayer(m);
                if (p != null)
                {
                    p.SendMessage(message);
                }
            }
            foreach (string m in _generals)
            {
                if (_fAlly.Contains(m.ToLower()) && type == 1) continue;
                if (_fChat.Contains(m.ToLower()) && type == 2) continue;
                p = _main.Server.LevelManager.FindPlayer(m);
                if (p != null)
                {
                    p.SendMessage(message);
                }
            }
            foreach (string m in _recruits)
            {
                if (_fAlly.Contains(m.ToLower()) && type == 1) continue;
                if (_fChat.Contains(m.ToLower()) && type == 2) continue;
                p = _main.Server.LevelManager.FindPlayer(m);
                if (p != null)
                {
                    p.SendMessage(message);
                }
            }
            if (_fAlly.Contains(Leader.ToLower()) && type == 1) return;
            if (_fChat.Contains(Leader.ToLower()) && type == 2) return;
            p = _main.Server.LevelManager.FindPlayer(Leader);
            if (p != null) p.SendMessage(message);
        }

        public void BroadcastPopUp(string message)
        {
            BroadcastPopUp(message, "");
        }

        public void BroadcastPopUp(string message, string subtitle)
        {
            Player p;
            foreach (string m in _members)
            {
                p = _main.Server.LevelManager.FindPlayer(m);
                if (p != null)
                {
                    Popup pop = new Popup();
                    pop.Message = (subtitle == "" ? "" : subtitle + "\n") + message;
                    p.AddPopup(pop);
                }
            }
            foreach (string m in _officers)
            {
                p = _main.Server.LevelManager.FindPlayer(m);
                if (p != null)
                {
                    Popup pop = new Popup();
                    pop.Message = (subtitle == "" ? "" : subtitle + "\n") + message;
                    p.AddPopup(pop);
                }
            }
            foreach (string m in _generals)
            {
                p = _main.Server.LevelManager.FindPlayer(m);
                if (p != null)
                {
                    Popup pop = new Popup();
                    pop.Message = (subtitle == "" ? "" : subtitle + "\n") + message;
                    p.AddPopup(pop);
                }
            }
            foreach (string m in _recruits)
            {
                p = _main.Server.LevelManager.FindPlayer(m);
                if (p != null)
                {
                    Popup pop = new Popup();
                    pop.Message = (subtitle == "" ? "" : subtitle + "\n") + message;
                    p.AddPopup(pop);
                }
            }
            p = _main.Server.LevelManager.FindPlayer(Leader);
            if (p != null)
            {
                Popup pop = new Popup();
                pop.Message = (subtitle == "" ? "" : subtitle + "\n") + message;
                p.AddPopup(pop);
            }
        }

        public int GetPlayerPerm(string name)
        {
            foreach (string player in GetRecruits()) if (player == (name)) return 1;
            foreach (string player in GetMembers()) if (player == (name)) return 2;
            foreach (string player in GetOfficers()) if (player == (name)) return 3;
            foreach (string player in GetGenerals()) if (player == (name)) return 4;
            if (GetLeader() == (name)) return 5;
            return 0;
        }

        public List GetOnlinePlayers()
        {
            Player p;
            List a = new List();
            foreach (string m in _members)
            {
                p = _main.Server.LevelManager.FindPlayer(m);
                if (p != null) a.Add(p);
            }
            foreach (string m in _officers)
            {
                p = _main.Server.LevelManager.FindPlayer(m);
                if (p != null) a.Add(p);
            }
            foreach (string m in _generals)
            {
                p = _main.Server.LevelManager.FindPlayer(m);
                if (p != null) a.Add(p);
            }
            foreach (string m in _recruits)
            {
                p = _main.Server.LevelManager.FindPlayer(m);
                if (p != null) a.Add(p);
            }
            p = _main.Server.LevelManager.FindPlayer(Leader);
            if (p != null) a.Add(p);
            return a;
        }

        public string BossBarText()
        {
/*return ChatColors.Gold+""+ChatColors.BOLD+"====§eTERRA§6TIDE===="+ChatColors.RESET+"\n\n"+
        "§6"+GetDisplayName()+" §b: §aLEVEL §b: §3"+GetLevel()+"\n"+
         "§eXP §b: §6"+GetXP()+" §a/ §b"+calculateRequireExperience(GetLevel());*/
            return ChatColors.Gold + "" + ChatFormatting.Bold + "====§eTERRA§6TIDE====" + ChatFormatting.Reset +
                   "\n\n" +
                   "§e" + GetDisplayName() + " §b: §aLEVEL §b: §3" + GetLevel() + "\n" +
                   "§eXP §b: §a" + GetXp() + " §a/ §3" + calculateRequireExperience(GetLevel());
        }

        public void UpdateBossBar()
        {
            foreach (Player player in GetOnlinePlayers())
            {
                _main.sendBossBar(player, this);
            }
        }

        public List getFAlly()
        {
            return _fAlly;
        }

        public List getFChat()
        {
            return _fChat;
        }

        public void setFAlly(List FAlly)
        {
            this._fAlly = FAlly;
        }

        public void setFChat(List FChat)
        {
            this._fChat = FChat;
        }

        public void UpdateTopResults()
        {
            _main.FF.Top.Add(_name, GetMoney());
        }

        public void UpdateRichResults()
        {
            _main.FF.Rich.Add(_name, GetRich());
        }
    }
}