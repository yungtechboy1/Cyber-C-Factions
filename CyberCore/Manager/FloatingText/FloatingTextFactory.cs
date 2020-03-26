using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using CyberCore.Utils;
using MiNET;
using MiNET.Net;
using MiNET.Utils;
using MiNET.Worlds;
using OpenAPI;
using OpenAPI.Player;

namespace CyberCore.Manager.FloatingText
{
    public class FloatingTextFactory
    {
        public CyberCoreMain CCM;

        private static Dictionary<long, List<String>> FTLastSentToRmv = new Dictionary<long, List<String>>();

        private static List<CyberFloatingTextContainer> LList = new List<CyberFloatingTextContainer>();
        private static List<CyberFloatingTextContainer> ToAddList = new List<CyberFloatingTextContainer>();
        private static List<CyberFloatingTextContainer> ToRemoveList = new List<CyberFloatingTextContainer>();
        static readonly object llock = new object();

        public FloatingTextFactory(CyberCoreMain CCM)
        {
            new FloatingTextFactory(CCM, new List<CyberFloatingTextContainer>());
        }

        public FloatingTextFactory(CyberCoreMain c, List<CyberFloatingTextContainer> al)
        {
            LList = al;
            CCM = c;
            // start();
            //TODO
            run();
            //Create New Tick Function and exicute every 40 Ticks
            //Just Call run in here
        }

        public static void killall()
        {
//        ToRemoveList.addAll(LList);
//        LList.clear();
            foreach (var a in LList)
            {
                if (a is CyberFloatingTextContainer)
                {
                    a.kill();
                }
            }
        }

        public List<CyberFloatingTextContainer> getLList()
        {
            List<CyberFloatingTextContainer> _l = new List<CyberFloatingTextContainer>();
            lock (llock)
            {
                LList.AddRange(ToAddList);
                LList = LList.Except(ToRemoveList).ToList();
                ToAddList.Clear();
                _l = LList;
            }

            return _l;
        }

        public static void setLList(List<CyberFloatingTextContainer> l)
        {
            lock (llock)
            {
                LList = l;
            }
        }

        public static void AddFloatingText(CyberFloatingTextContainer ftc)
        {
            AddFloatingText(ftc, false);
        }

        public static void AddFloatingText(CyberFloatingTextContainer ftc, bool save)
        {
            if (save) CyberCoreMain.GetInstance().SavedFloatingText.Add(ftc);
            lock (llock)
            {
                ToAddList.Add(ftc);
//            System.out.println("added!");
            }
        }

        public static void AddToRemoveList(CyberFloatingTextContainer ftc)
        {
            lock (llock)
            {
                ToRemoveList.Add(ftc);
//            System.out.println("added!");
            }
        }

        public OpenApi API = CyberCoreMain.GetInstance().getAPI();

        public Dictionary<String, PlayerLocation> GetPlayerPoss()
        {
//        System.out.println("GP");
            Dictionary<String, PlayerLocation> playerposs = new Dictionary<String, PlayerLocation>();
            foreach (OpenPlayer p in CyberCoreMain.GetInstance().getAPI().PlayerManager.GetPlayers())
                playerposs.Add(p.getName(), p.KnownPosition);
//        System.out.println("EP"+playerposs.size());
            return playerposs;
        }

        public void run()
        {
            int lasttick = -1;
//        System.out.println("11111111111111111111");
//System.out.println("======");
            int tick = CyberUtils.getIntTime();
            if (tick != lasttick)
            {
//                System.out.println("||||||||======");
                lasttick = tick;
//                Dictionary<String, PlayerLocation> ppss = GetPlayerPoss();
                foreach (CyberFloatingTextContainer a in getLList())
                {
                    if (a == null)
                    {
                        CyberCoreMain.Log.Error("FTF>> ERROR! Loading FT!!! " + a);
                        continue;
                    }


//                    System.out.println("|||||"+ft.GetText(null));
                    //Create Blank array if not present!
                    if (!FTLastSentToRmv.ContainsKey(a.EntityId)) FTLastSentToRmv.Add(a.EntityId, new List<String>());

//                    System.out.println("OU");
                    a.OnUpdate(tick);
//                    System.out.println("OU");
//                    int ftlt = a.LastUpdate;
// ?????????
// if (ftlt >= tick) continue;

                    List<String> ap = new List<String>();
                    List<Player> app = new List<Player>();
                    //For Each player with pos
//                    for (String player : ppss.keySet()) {
//                    System.out.println("ERroror >> 1");
//                    System.out.println("ERroror >> "+a.Lvl);
//                    System.out.println("ERroror >> "+a.Syntax);
                    Level l = null;
                    if (a.KnownPosition == null || a.Lvl == null)
                    {
                        if (l == null) CyberCoreMain.Log.Error("FTF > Level is null TOO");
                        CyberCoreMain.Log.Error(
                            "FTF > Error!!! Floating Text Syntax: " + a.Syntax + "||" + a.ToString());
                        continue;
                    }

                    l = API.LevelManager.GetLevel(null, a.Lvl);

                    if (l == null)
                    {
                        CyberCoreMain.Log.Error("FTF > Level is null TOO1111");
                        continue;
                    }


                    var pc = l.GetAllPlayers();
                    if (pc == null || pc.Length == 0)
                    {
//                        System.out.println("ERroror roor ororororo E15072458");
                        continue;
                    }

                    foreach (var p in pc)
                    {
                        String player = p.getName();
//                        System.out.println("2222"+player);
                        PlayerLocation ppos = p.KnownPosition;
                        if (a.KnownPosition.DistanceTo(ppos) > 200) continue;
                        //TODO many implement a Quick Check?
                        //Check HERE If X is less that 100
                        //To Save resources
                        if (!a.Lvl.equalsIgnoreCase(p.Level.LevelName)) //Not same World & 100+ Blocks away
                            continue;
                        ap.Add(player);
                        app.Add(p);
//                        System.out.println("AP");
                        //Remove Player we just added cuz We dont need to remove the FT particle from them!
                        if (FTLastSentToRmv[a.EntityId].Count > 0) FTLastSentToRmv[a.EntityId].Remove(player);
                    }

                    //TODO Alt - Chunk Packet!

                    if (a._CE_Done)
                    {
                        FTLastSentToRmv[a.EntityId].AddRange(ap);
                        KillUnnneded(FTLastSentToRmv[a.EntityId], a.EntityId, a.Lvl);
                        ap.Clear();
                        AddToRemoveList(a);
                        continue;
                    }

                    if (ap.Count == 0) continue;
                    //Last time AP

                    //Remove Each player from FTLastSentToRmv
                    KillUnnneded(FTLastSentToRmv[a.EntityId], a.EntityId, a.Level);
                    a.HaldleSendP(app);
                    //Add All Players who we sent the packet to the remove list just in case they walk away
                    FTLastSentToRmv[a.EntityId].AddRange(ap);

                    //Should i send packes within the Thread?
                }
            }

            //A little faster than .1 of a sec (.06 to be exact...or 1 tick = 50millis and this is 60 millisecs)
            //Low key Every other 4 thiks is fine
            try
            {
                Thread.Sleep(200 * 2); //4 Ticks*2
            }
            catch (Exception e)
            {
                //ignore
            }
        }


        private void KillUnnneded(List<string> s, long eid, string aLvl)
        {
            Level l = CCM.getAPI().LevelManager.GetLevel(null, aLvl);
            if (l == null)
            {
                CyberCoreMain.Log.Error("FTF > Error LVL twas not foundets");
                return;
            }
            KillUnnneded(s,eid,l);
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
                if (CCM.getAPI().PlayerManager.TryGetPlayer(p, out pp))
                {
                    CyberCoreMain.Log.Error("FTF > Error Da Playea twas not foundets");
                    continue;
                }

                kill(eid, pp);
            }
        }

        public String FormatText(String text, Player player)
        {
            return FormatText(text, player, false);
        }

        public String FormatText(String text, Player player, bool vertical)
        {
//        if(text == null)System.out.println("----0");
//        if(CCM == null)System.out.println("----1");
//        if(CCM.getServer() == null)System.out.println("----2");
//        if(CCM.getServer().getOnlinePlayers() == null)System.out.println("----3");
//        System.out.println("----4"+CCM.getServer().getOnlinePlayers().size());
//        System.out.println("----5"+CCM.getServer().getTicksPerSecondAverage());
//        System.out.println("----6"+text);
            text = text.Replace("{online-players}", "" + CCM.getAPI().PlayerManager.GetPlayers().Length)
                .Replace("{ticks}", 20 + "") //TODO
                .Replace("`", "\n")
                .Replace("{&}", ChatColors.LightPurple + "");
            if (player != null) text = text.Replace("{name}", player.getName());
            if (player != null)
            {
                //Faction

                String pf = "No Faction";
                if (CCM.FM != null)
                {
                    pf = CCM.FM.getPlayerFaction(player.getName());
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
                rank = CCM.getPlayerRank(player.getName()).getDisplayName();
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
                        .Replace("\\|n", "\n")
                    ;
            }
            else
            {
                text = text
                    .Replace("{faction}", "No Faction")
                    .Replace("{kills}", "N/A")
                    .Replace("{deaths}", "N/A")
                    .Replace("\\|n", "\n")
                    .Replace("{kdr}", "N/A");
            }

            if (vertical) text = text.Replace("|n", "\n");
//        System.out.println(text);
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