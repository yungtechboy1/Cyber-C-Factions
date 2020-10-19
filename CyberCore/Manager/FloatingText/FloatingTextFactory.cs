using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using CyberCore.Utils;
using LibNoise.Combiner;
using log4net.Util.TypeConverters;
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

        private static List<GenericFloatingTextEntity> ActiveFTList = new List<GenericFloatingTextEntity>();
        private static List<GenericFloatingTextEntity> AddToActiveList = new List<GenericFloatingTextEntity>();
        private static List<GenericFloatingTextEntity> RemoveFromActiveFTList = new List<GenericFloatingTextEntity>();
        static readonly object llock = new object();

        private HighPrecisionTimer _tickerHighPrecisionTimer;
        // private CustomConfig cfg = new CustomConfig(CyberCoreMain.GetInstance(), "MasterFloatingText");

        public FloatingTextFactory(CyberCoreMain c, List<GenericFloatingTextEntity> al = null)
        {
            instance = this;
            if (al == null) al = new List<GenericFloatingTextEntity>();
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
                SavedFloatingText = new List<GenericFloatingTextEntity>();

                foreach (var v in d)
                {
                    var ln = v.Lvl;
                    var l = CCM.getAPI().LevelManager.GetLevel(null, ln);
                    // Console.WriteLine($"LOADDDDDDDDINNNNNGGGGGGGGG {v.Syntax} || {ln} VS {API.LevelManager.GetDefaultLevel().LevelId} VS  {API.LevelManager.GetDefaultLevel().LevelName}");
                    if (l != null)
                    {
                        var vv = new CyberGenericFloatingTextContainer(this, v, l);
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

            List<GenericCyberFloatingTextContainerData> d = new List<GenericCyberFloatingTextContainerData>();
            foreach (var v in SavedFloatingText)
            {
                if (v is PopupFT) continue;
                ((CyberGenericFloatingTextContainer) v).FTData.PrepareForSave();

                // CyberCoreMain.Log.Error("SAVING FTM FOR "+v.FTData.Syntax);
                d.Add(((CyberGenericFloatingTextContainer) v).FTData);
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
                if (a is CyberGenericFloatingTextContainer aa)
                    aa.kill();
                else if (a is PopupFT aaa)
                    aaa.kill();
            }

            if (fr)
            {
                RemoveFromActiveFTList.AddRange(ActiveFTList);
                ActiveFTList.Clear();
            }
        }

        public List<GenericFloatingTextEntity> GetMastListAndAddNewTexts()
        {
            List<GenericFloatingTextEntity> _l = new List<GenericFloatingTextEntity>();
            lock (llock)
            {
                ActiveFTList.AddRange(AddToActiveList);
                ActiveFTList = ActiveFTList.Except(RemoveFromActiveFTList).ToList();
                AddToActiveList.Clear();
                _l = ActiveFTList;
            }

            return _l;
        }

        public static void setLList(List<GenericFloatingTextEntity> l)
        {
            lock (llock)
            {
                ActiveFTList = l;
            }
        }

        public static List<GenericFloatingTextEntity> SavedFloatingText { get; set; } =
            new List<GenericFloatingTextEntity>();

        public static void AddFloatingText(GenericFloatingTextEntity ftc, bool save = false)
        {
            if (save) SavedFloatingText.Add(ftc);
            lock (llock)
            {
                AddToActiveList.Add(ftc);
//            CyberCoreMain.Log.Error("Was LOG ||"+"added!");
            }
        }

        public static void AddToRemoveList(CyberGenericFloatingTextContainer ftc)
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
                foreach (CyberGenericFloatingTextContainer gftc in GetMastListAndAddNewTexts())
                {
                    if (gftc == null)
                    {
                        CyberCoreMain.Log.Error("FTF>> ERROR! Loading FT!!! " + gftc);
                        continue;
                    }

                    if (!gftc.CanTick(tick))
                    {
                        // CyberCoreMain.Log.Error("FTF >>> ERROR! FT NOT READY TO TICK YET"+a);
                        continue;
                    }

                    int aa = 0;
                    // CyberCoreMain.Log.Error("Was LOG ||"+"|||>||"+a.GetText(null));
                    //Create Blank array if not present!
                    if (!FTLastSentToRmv.ContainsKey(gftc.EntityId))
                        FTLastSentToRmv.Add(gftc.EntityId, new List<String>());

//                    CyberCoreMain.Log.Error("Was LOG ||"+"OU");
                    gftc.OnUpdate(tick);
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
                    if (gftc.KnownPosition == null || gftc.FTData.Lvl == null)
                    {
                        if (l == null) CyberCoreMain.Log.Error("FTF > Level is nulel TOO");
                        CyberCoreMain.Log.Error(
                            "FTF > Error!!! Floating Text Syntax: " + gftc.FTData.Syntax + "||" + gftc.ToString());
                        continue;
                    }

                    // CyberCoreMain.Log.Error("Was LOG ||000" + aa++ +"|||||||||||"+a.FTData.Lvl);
                    try
                    {
                        l = API.LevelManager.GetLevel(null, gftc.FTData.Lvl);
                    }
                    catch (Exception e)
                    {
                        CyberCoreMain.Log.Error("DDDDDDDDAAAAAAA ERROR" + aa++, e);
                    }

                    // CyberCoreMain.Log.Error("Was LOG ||111" + aa++);
                    if (l == null)
                    {
                        CyberCoreMain.Log.Error("FTF > Level is null TOO1111");
                        continue;
                    }

                    // CyberCoreMain.Log.Error("Was LOG ||22222" + aa++);

                    var allPlayers = l.GetAllPlayers();
                    if (allPlayers == null || allPlayers.Length == 0)
                    {
                        // CyberCoreMain.Log.Error("Was NO PLATERS NEAR");
                        continue;
                    }

                    // CyberCoreMain.Log.Error("Was LOG ||" + aa++);
                    foreach (var p in allPlayers)
                    {
                        String player = p.getName();
//                        CyberCoreMain.Log.Error("Was LOG ||"+"2222"+player);
                        PlayerLocation ppos = p.KnownPosition;
                        if (gftc.KnownPosition.DistanceTo(ppos) > 200) continue;
                        //TODO many implement a Quick Check?
                        //Check HERE If X is less that 100
                        //To Save resources
                        if (!gftc.FTData.Lvl.equalsIgnoreCase(p.Level.LevelId))
                            continue; //Not same World & 100+ Blocks away
                        ap.Add(player);
                        app.Add((CorePlayer) p);
//                        CyberCoreMain.Log.Error("Was LOG ||"+"AP");
                        //Remove Player we just added cuz We dont need to remove the FT particle from them!
                        if (FTLastSentToRmv[gftc.EntityId].Count > 0) FTLastSentToRmv[gftc.EntityId].Remove(player);
                    }

                    //TODO Alt - Chunk Packet!

                    // CyberCoreMain.Log.Error("Was LOG ||" + aa++);
                    // Console.WriteLine("CHECK IF DAT MF GOTTA GO >> "+gftc.FTData._CE_Done);
                    if (gftc.FTData._CE_Done)
                    {
                        Console.WriteLine("Killin Time");
                        FTLastSentToRmv[gftc.EntityId].AddRange(ap);
                        KillUnnneded(FTLastSentToRmv[gftc.EntityId], gftc.EntityId, gftc.FTData.Lvl);
                        // ap.Clear();
                        AddToRemoveList(gftc);
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
                        KillUnnneded(FTLastSentToRmv[gftc.EntityId], gftc.EntityId, gftc.Level);
                        gftc.HaldleSendP(app);
                        AddPlayersToFT(gftc, app);
                    }
                    catch (Exception e)
                    {
                        CyberCoreMain.Log.Error("EERRRZZZZ SENDING ASENDP ", e);
                    }

                    //Add All Players who we sent the packet to the remove list just in case they walk away
                    FTLastSentToRmv[gftc.EntityId].AddRange(ap);
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

        public Dictionary<CyberGenericFloatingTextContainer, List<String>> PlayersLoadedToFT =
            new Dictionary<CyberGenericFloatingTextContainer, List<string>>();

        private void AddPlayersToFT(CyberGenericFloatingTextContainer gftc, List<CorePlayer> app)
        {
            if (!PlayersLoadedToFT.ContainsKey(gftc)) PlayersLoadedToFT.Add(gftc, new List<string>());
            var d = PlayersLoadedToFT[gftc];
            foreach (var p in app)
            {
                var un = p.Username;
                if (!d.Contains(un))
                {
                    d.Add(un);
                }
            }
            PlayersLoadedToFT[gftc] = d;

        }


        private void KillUnnneded(List<string> s, long eid, string aLvl)
        {
            // Console.WriteLine("STARTING KILL WIHT STRING LVL");
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
            // Console.WriteLine("STARTING KILL WIHT LVL");
            foreach (var p in s)
            {
                // Console.WriteLine("STARTING KILL WIHT LVL TO PLAYER "+p);
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
                .Replace("\r", "")
                .Replace("&&", "§");
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

        public void removePlayer(CorePlayer player)
        {
        }

        private static FloatingTextFactory instance;
        public static FloatingTextFactory getInstance()
        {
            return instance;
        }
    }
}