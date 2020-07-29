using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using CyberCore.Utils;
using MiNET;
using MiNET.Blocks;
using MiNET.Net;
using MiNET.Utils;
using MiNET.Worlds;
using Newtonsoft.Json;
using OpenAPI;
using OpenAPI.Player;
using OpenAPI.World;

namespace CyberCore.Manager.FloatingText
{
    public class FloatingTextFactory
    {
        public CyberCoreMain CCM;

        private static Dictionary<long, List<String>> FTLastSentToRmv = new Dictionary<long, List<String>>();

        private static List<CyberFloatingTextContainer> ActiveFTList = new List<CyberFloatingTextContainer>();
        private static List<CyberFloatingTextContainer> AddToActiveList = new List<CyberFloatingTextContainer>();
        private static List<CyberFloatingTextContainer> RemoveFromActiveFTList = new List<CyberFloatingTextContainer>();
        static readonly object llock = new object();

        private HighPrecisionTimer _tickerHighPrecisionTimer;
        // private CustomConfig cfg = new CustomConfig(CyberCoreMain.GetInstance(), "MasterFloatingText");

        public FloatingTextFactory(CyberCoreMain c, List<CyberFloatingTextContainer> al = null)
        {
            if (al == null) al = new List<CyberFloatingTextContainer>();
            ActiveFTList = al;
            CCM = c;
            // start();
            //TODO
            // run();
            
            _tickerHighPrecisionTimer = new HighPrecisionTimer(500, run, false, false);
            //Create New Tick Function and exicute every 40 Ticks
            //Just Call run in here
        }

        public void LoadFromSave()
        {
            Console.WriteLine($"STTTTTTTTTTTTTTTTTTTTTT ");
            string directoryName = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            string path = Path.Combine(directoryName, "Plugins");
            path = Path.Combine(path, "MasterFloatingText.conf");
            string t = File.ReadAllText(path);
            List<CyberFloatingTextContainerData> d =
                JsonConvert.DeserializeObject<List<CyberFloatingTextContainerData>>(t);
            if (d != null)
            {
                foreach (var v in d)
                {
                    var ln = v.Lvl;
                    var l = CCM.getAPI().LevelManager.GetLevel(null, ln);
                    // Console.WriteLine($"LOADDDDDDDDINNNNNGGGGGGGGG {v.Syntax} || {ln} VS {API.LevelManager.GetDefaultLevel().LevelId} VS  {API.LevelManager.GetDefaultLevel().LevelName}");
                    if (l != null)
                    {
                        var vv = new CyberFloatingTextContainer(this, v, l);
                        AddFloatingText(vv);
                        SavedFloatingText.Add(vv);
                    }
                    else Console.WriteLine("ERRRRORRR!!!!!!!!");
                }
            }
            else
            {
                CyberCoreMain.Log.Error("============eeeeeeee111111111===================");
                CyberCoreMain.Log.Error("============eeeeeeee111111111===================");
                CyberCoreMain.Log.Error("============eeeeeeee111111111===================");
                CyberCoreMain.Log.Error("============eeeeeeee111111111===================");
            }
        }

        public void stop(bool save = true)
        {
            _tickerHighPrecisionTimer.Dispose();
            if (!save) return;
            
            List<CyberFloatingTextContainerData> d = new List<CyberFloatingTextContainerData>();
            foreach (var v in SavedFloatingText)
            {
                if (v is PopupFT) continue;
                v.FTData.PrepareForSave();
                
                // CyberCoreMain.Log.Error("SAVING FTM FOR "+v.FTData.Syntax);
                d.Add(v.FTData);
            }

            var dd = JsonConvert.SerializeObject(d);

            string directoryName = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            string path = Path.Combine(directoryName, "Plugins");
            path = Path.Combine(path, "MasterFloatingText.conf");
            File.WriteAllText(path, dd);
            
            // CyberCoreMain.Log.Error("SAVED TO  "+path+" |||||||| "+dd);
        }

        public static void killall(bool fr = false)
        {
            foreach (var a in ActiveFTList)
            {
                if (a is CyberFloatingTextContainer)
                {
                    a.kill();
                }
            }

            if (fr)
            {
                RemoveFromActiveFTList.AddRange(ActiveFTList);
                ActiveFTList.Clear();
            }
        }

        public List<CyberFloatingTextContainer> getLList()
        {
            List<CyberFloatingTextContainer> _l = new List<CyberFloatingTextContainer>();
            lock (llock)
            {
                ActiveFTList.AddRange(AddToActiveList);
                ActiveFTList = ActiveFTList.Except(RemoveFromActiveFTList).ToList();
                AddToActiveList.Clear();
                _l = ActiveFTList;
            }

            return _l;
        }

        public static void setLList(List<CyberFloatingTextContainer> l)
        {
            lock (llock)
            {
                ActiveFTList = l;
            }
        }

        public static List<CyberFloatingTextContainer> SavedFloatingText { get; set; } =
            new List<CyberFloatingTextContainer>();

        public static void AddFloatingText(CyberFloatingTextContainer ftc, bool save = false)
        {
            if (save) SavedFloatingText.Add(ftc);
            lock (llock)
            {
                AddToActiveList.Add(ftc);
//            CyberCoreMain.Log.Error("Was LOG ||"+"added!");
            }
        }

        public static void AddToRemoveList(CyberFloatingTextContainer ftc)
        {
            lock (llock)
            {
                RemoveFromActiveFTList.Add(ftc);
//            CyberCoreMain.Log.Error("Was LOG ||"+"added!");
            }
        }

        public OpenApi API = CyberCoreMain.GetInstance().getAPI();

        public Dictionary<String, PlayerLocation> GetPlayerPoss()
        {
//        CyberCoreMain.Log.Error("Was LOG ||"+"GP");
            Dictionary<String, PlayerLocation> playerposs = new Dictionary<String, PlayerLocation>();
            foreach (OpenPlayer p in CyberCoreMain.GetInstance().getAPI().PlayerManager.GetPlayers())
                playerposs.Add(p.getName(), p.KnownPosition);
//        CyberCoreMain.Log.Error("Was LOG ||"+"EP"+playerposs.size());
            return playerposs;
        }

            long lasttick = -1;
            public bool ReadyToRun = false;
        public void run(Object o)
        {
            // CyberCoreMain.Log.Error("RUNNING!!!!!!!!!!!");
//        CyberCoreMain.Log.Error("Was LOG ||"+"11111111111111111111");
//CyberCoreMain.Log.Error("Was LOG ||"+"======");
            if (API.LevelManager.Levels.Count == 0) return;
            if (!ReadyToRun)
            {
                LoadFromSave();
                ReadyToRun = true;
            }
            long tick = CyberUtils.getTick();
            if (tick != lasttick)
            {
                // CyberCoreMain.Log.Error("Was LOG ||"+"||||||||======");
                lasttick = tick;
//                Dictionary<String, PlayerLocation> ppss = GetPlayerPoss();
                foreach (CyberFloatingTextContainer a in getLList())
                {
                    if (a == null)
                    {
                        CyberCoreMain.Log.Error("FTF>> ERROR! Loading FT!!! " + a);
                        continue;
                    }

                    if (!a.CanTick(tick))
                    {
                        // CyberCoreMain.Log.Error("FTF>> ERROR! FT WONT TICK Now!!! " + a);
                        continue;
                        
                    }

                    int aa = 0;
                    // CyberCoreMain.Log.Error("Was LOG ||"+"|||>||"+a.GetText(null));
                    //Create Blank array if not present!
                    if (!FTLastSentToRmv.ContainsKey(a.EntityId)) FTLastSentToRmv.Add(a.EntityId, new List<String>());

//                    CyberCoreMain.Log.Error("Was LOG ||"+"OU");
                    a.OnUpdate(tick);
//                    CyberCoreMain.Log.Error("Was LOG ||"+"OU");
//                    int ftlt = a.LastUpdate;
// ?????????
// if (ftlt >= tick) continue;
                    // CyberCoreMain.Log.Error("Was LOG ||" + aa++);
                    List<String> ap = new List<String>();
                    List<CorePlayer> app = new List<CorePlayer>();
                    //For Each player with pos
//                    for (String player : ppss.keySet()) {
//                    CyberCoreMain.Log.Error("Was LOG ||"+"ERroror >> 1");
//                    CyberCoreMain.Log.Error("Was LOG ||"+"ERroror >> "+a.Lvl);
//                    CyberCoreMain.Log.Error("Was LOG ||"+"ERroror >> "+a.Syntax);
                    Level l = null;
                    if (a.KnownPosition == null || a.FTData.Lvl == null)
                    {
                        if (l == null) CyberCoreMain.Log.Error("FTF > Level is nulel TOO");
                        CyberCoreMain.Log.Error(
                            "FTF > Error!!! Floating Text Syntax: " + a.FTData.Syntax + "||" + a.ToString());
                        continue;
                    }

                    // CyberCoreMain.Log.Error("Was LOG ||000" + aa++ +"|||||||||||"+a.FTData.Lvl);
                    try
                    {
                        l = API.LevelManager.GetLevel(null, a.FTData.Lvl);
                    }
                    catch (Exception e)
                    {
                        
                        CyberCoreMain.Log.Error("DDDDDDDDAAAAAAA ERROR" + aa++,e);
                    }

                    // CyberCoreMain.Log.Error("Was LOG ||111" + aa++);
                    if (l == null)
                    {
                        CyberCoreMain.Log.Error("FTF > Level is null TOO1111");
                        continue;
                    }

                    // CyberCoreMain.Log.Error("Was LOG ||22222" + aa++);

                    var pc = l.GetAllPlayers();
                    if (pc == null || pc.Length == 0)
                    {
                        // CyberCoreMain.Log.Error("Was NO PLATERS NEAR");
                        continue;
                    }

                    // CyberCoreMain.Log.Error("Was LOG ||" + aa++);
                    foreach (var p in pc)
                    {
                        String player = p.getName();
//                        CyberCoreMain.Log.Error("Was LOG ||"+"2222"+player);
                        PlayerLocation ppos = p.KnownPosition;
                        if (a.KnownPosition.DistanceTo(ppos) > 200)
                        {
                            // CyberCoreMain.Log.Error("FT AND PLAYER TOO FAR");
                            continue;
                        }
                        //TODO many implement a Quick Check?
                        //Check HERE If X is less that 100
                        //To Save resources
                        if (!a.FTData.Lvl.equalsIgnoreCase(p.Level.LevelId))
                        {                        
CyberCoreMain.Log.Error("FT AND PLAYER NOT IN SAME LEVEL");
                            continue;//Not same World & 100+ Blocks away
                        }

                        ap.Add(player);
                        app.Add((CorePlayer) p);
//                        CyberCoreMain.Log.Error("Was LOG ||"+"AP");
                        //Remove Player we just added cuz We dont need to remove the FT particle from them!
                        if (FTLastSentToRmv[a.EntityId].Count > 0) FTLastSentToRmv[a.EntityId].Remove(player);
                    }

                    //TODO Alt - Chunk Packet!

                    // CyberCoreMain.Log.Error("Was LOG || CHECK IF DONE "+a.FTData._CE_Done);
                    if (a.FTData._CE_Done)
                    {
                        FTLastSentToRmv[a.EntityId].AddRange(ap);
                        KillUnnneded(FTLastSentToRmv[a.EntityId], a.EntityId, a.FTData.Lvl);
                        ap.Clear();
                        AddToRemoveList(a);
                        CyberCoreMain.Log.Error("CE ISSSSSS DONEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEE");
                        continue;
                    }

                    // CyberCoreMain.Log.Error("Was LOG ||" + aa++);
                    if (ap.Count == 0)
                    {
                        // CyberCoreMain.Log.Error("YEAH AP WAS NULLLLLLLLLL");
                        continue;
                    }
                    //Last time AP

                    // CyberCoreMain.Log.Error("Was LOG ||" + aa++ +"||||| "+app.Count);
                    //Remove Each player from FTLastSentToRmv
                    
                    try
                    {
                        KillUnnneded(FTLastSentToRmv[a.EntityId], a.EntityId, a.Level);
                        // CyberCoreMain.Log.Error("3333333333333333333333333333DONEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEE");

                        a.HaldleSendP(app);
                    }
                    catch (Exception e)
                    {
                        CyberCoreMain.Log.Error("EERRRZZZZ SENDING ASENDP ", e);
                    }

                    //Add All Players who we sent the packet to the remove list just in case they walk away
                    FTLastSentToRmv[a.EntityId].AddRange(ap);
                    // CyberCoreMain.Log.Error("DONEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEE");
                    //Should i send packes within the Thread?
                }
            }

            //A little faster than .1 of a sec (.06 to be exact...or 1 tick = 50millis and this is 60 millisecs)
            //Low key Every other 4 thiks is fine
            // try
            // {
            //     Thread.Sleep(200 * 2); //4 Ticks*2
            // }
            // catch (Exception e)
            // {
            //     //ignore
            // }
        }


        private void KillUnnneded(List<string> s, long eid, string aLvl)
        {
            Level l = CCM.getAPI().LevelManager.GetLevel(null, aLvl);
            if (l == null)
            {
                CyberCoreMain.Log.Error("FTF > Error LVL twas not foundets");
                return;
            }

            KillUnnneded(s, eid, l);
        }

        private void KillUnnneded(List<string> s, long eid, Level Lvl)
        {
            foreach (var p in s)
            {
                Level l = Lvl;
                if (l == null)
                {
                    CyberCoreMain.Log.Error("FTF > Error LVL twas not foundets");
                    continue;
                }

                OpenPlayer pp = null;
                if (!CCM.getAPI().PlayerManager.TryGetPlayer(p, out pp))
                {
                    CyberCoreMain.Log.Error("FTF > Error Da Playea twas not foundets");
                    continue;
                }

                kill(eid, pp);
            }
        }

        public String FormatText(String text, CorePlayer player)
        {
            return FormatText(text, player, false);
        }

        public String FormatText(String text, CorePlayer player, bool vertical)
        {
//        if(text == null)CyberCoreMain.Log.Error("Was LOG ||"+"----0");
//        if(CCM == null)CyberCoreMain.Log.Error("Was LOG ||"+"----1");
//        if(CCM.getServer() == null)CyberCoreMain.Log.Error("Was LOG ||"+"----2");
//        if(CCM.getServer().getOnlinePlayers() == null)CyberCoreMain.Log.Error("Was LOG ||"+"----3");
//        CyberCoreMain.Log.Error("Was LOG ||"+"----4"+CCM.getServer().getOnlinePlayers().size());
//        CyberCoreMain.Log.Error("Was LOG ||"+"----5"+CCM.getServer().getTicksPerSecondAverage());
//        CyberCoreMain.Log.Error("Was LOG ||"+"----6"+text);
            text = text.Replace("{online-players}", "" + CCM.getAPI().PlayerManager.GetPlayers().Length)
                .Replace("{ticks}", CyberCoreMain.GetInstance().getTicksPerSecond() + "") //TODO
                .Replace("`", "\n")
                .Replace("{&}", ChatColors.LightPurple + "");
            if (player != null) text = text.Replace("{name}", player.getName());
            if (player != null)
            {
                //Faction

                String pf = "No Faction";
                if (CCM.FM != null)
                {
                    var ff = CCM.FM.FFactory.getPlayerFaction(player);
                    if (ff != null) pf = ff.getDisplayName();
                    if (pf == null) pf = "No Faction";
                }

                //Kills
                Double kills = 0d; //Factions.GetKills(player.getName());
//            if(KDConfig.exists(player.getName().toLowerCase())){
//                kills = Double.parseDouble(((LinkedDictionary)KDConfig.get(player.getName().toLowerCase())).get("kills")+"");
//            }
                //Deaths
                Double deaths = 0d; //Factions.GetDeaths(player.getName());
//            if(KDConfig.exists(player.getName().toLowerCase())){
//                deaths = Double.parseDouble(((LinkedDictionary)KDConfig.get(player.getName().toLowerCase())).get("deaths")+"");
//            }
                //KDR
                Double kdr = kills / deaths; //Factions.GetKDR(player.getName());
                String rank = "Guest|";
                //DISABLED FOR NOW
                rank = CCM.getPlayerRank(player.getName()).display_name;
                if (rank == null) rank = "Guest";

                String tps = "" + CCM.getTicksPerSecond();
                String players = "" + CCM.getOnlinePlayersCount();
                String max = "" + CCM.getAPI().ServerInfo.MaxNumberOfPlayers;
                // String max = "" + CCM.getAPI().ServerInfo.;
                String money = "0";
//            ArchEconMain AA = (ArchEconMain) CCM.getServer().getPluginManager().getPlugin("ArchEcon");
//            if(AA != null){
//                money = ""+AA.getMoney(player.getName());
//            }

                text = text
                        .Replace("{faction}", pf)
                        .Replace("{kills}", kills + "")
                        .Replace("{deaths}", deaths + "")
                        .Replace("{kdr}", kdr + "")
                        .Replace("{rank}", rank)
                        .Replace("{tps}", tps)
                        .Replace("{players}", players)
                        .Replace("{max}", max)
                        .Replace("{money}", money)
                        .Replace("|n", "\n")
                    ;
            }
            else
            {
                text = text
                    .Replace("{faction}", "No Faction")
                    .Replace("{kills}", "N/A")
                    .Replace("{deaths}", "N/A")
                    .Replace("|n", "\n")
                    .Replace("{kdr}", "N/A");
            }

            if (vertical) text = text.Replace("|n", "\n");
//        CyberCoreMain.Log.Error("Was LOG ||"+text);
            return text;
        }

        public static void kill(long eid, Player p)
        {
            if (p == null) return;
            McpeRemoveEntity mcpeRemoveEntity = McpeRemoveEntity.CreateObject();
            mcpeRemoveEntity.entityIdSelf = eid;
            p.SendPacket(mcpeRemoveEntity);
            // Level.RelayBroadcast(players, mcpeRemoveEntity);
        }
    }
}