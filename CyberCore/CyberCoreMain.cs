using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using CyberCore.Manager.AuctionHouse;
using CyberCore.Manager.ClassFactory;
using CyberCore.Manager.Crate;
using CyberCore.Manager.Factions;
using CyberCore.Manager.FloatingText;
using CyberCore.Manager.Rank;
using CyberCore.Manager.Shop;
using CyberCore.Manager.Warp;
using CyberCore.Utils;
using CyberCore.Utils.Data;
using CyberCore.WorldGen;
using log4net;
using MiNET;
using MiNET.Blocks;
using MiNET.Net;
using MiNET.Plugins;
using MiNET.Plugins.Attributes;
using MiNET.Sounds;
using MiNET.Utils;
using MiNET.Worlds;
using OpenAPI;
using OpenAPI.Events;
using OpenAPI.Player;
using OpenAPI.Plugins;
using OpenAPI.World;

namespace CyberCore
{
    [OpenPluginInfo(Name = "CyberCore", Description = "CyberTech++ Core Plugin", Author = "YungTechBoy1",
        Version = "1.0.0.1-PA", Website = "CyberTechpp.com")]
    public class CyberCoreMain : OpenPlugin
    {
        // public Dictionary<String, Object> PlayerIdentification = new Dictionary<string, object>();
        public static ILog Log { get; private set; } = LogManager.GetLogger(typeof(CyberCoreMain));

        public void Configure(MiNetServer s)
        {
            // s.ServerManager = new CyberTechServerManager(s);
            s.PlayerFactory = new CyberPlayerFactory(API);
            Console.WriteLine("================Executed startup successfully. Replaced identity managment=========================");
            Log.Info("================Executed startup successfully. Replaced identity managment=========================");
            
        }
        
        private static CyberCoreMain instance { get; set; }

        // public Dictionary<String,Object> MasterConfig { get; }
        public CustomConfig MasterConfig { get; set; }
        public FactionsMain FM { get; private set; }
        public SqlManager SQL { get; private set; }
        public SqlManager WebSQL { get; private set; }
        public ServerSqlite ServerSQL { get; set; }
        public WarpManager WarpManager { get; set; }
        public ClassFactory ClassFactory { get; set; }
        public RankFactory RankFactory { get; set; }
        public UserSQL UserSQL { get; set; }
        public CrateMain CrateMain { get; set; }
        
        public FloatingTextFactory FTM { get; private set; }

        


        public bool isInSpawn(CorePlayer p)
        {
            return isInSpawn(p.KnownPosition, p.Level);
        }
        public bool isInSpawn(PlayerLocation pl, Level l)
        {
            var sp = l.SpawnPoint;
            var a = sp.DistanceTo(pl);
            return (a < 300);
        }

        public static CyberCoreMain GetInstance()
        {
            return instance;
        }
        
        
        [PacketHandler, Receive]
        public Packet TEST(McpeInventoryTransaction p, Player pp)
        {
            Console.WriteLine("AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA");
            Console.WriteLine("AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA");
            Console.WriteLine("AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA");
            Console.WriteLine("============"+pp.getName());
            Console.WriteLine("AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA");
            Console.WriteLine("AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA");
            Console.WriteLine("AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA");
            return p;
        }

        public CyberCoreMain()
        {
            MasterConfig = new CustomConfig(this, "Master");
            instance = this;
            SQL = new SqlManager(this);
            WebSQL = new SqlManager(this,"web");
            RankFactory = new RankFactory(this);
            ServerSQL = new ServerSqlite(this);
            UserSQL = new UserSQL(this);
            ClassFactory = new ClassFactory(this);
            WarpManager = new WarpManager(this);
            // MasterConfig = new Dictionary<String,Object>() {ConfigFileName = "MasterConfig.conf"};
          
        }

        private void RegisterCommands()
        {
            // getAPI().CommandManager.LoadCommands();
        }

        private void OnPlayerJoin(object o, PlayerEventArgs eventArgs)
        {
            Level level = eventArgs.Level;
            if (level == null) throw new ArgumentNullException(nameof(eventArgs.Level));

            Player player = eventArgs.Player;
            if (player == null) throw new ArgumentNullException(nameof(eventArgs.Player));

            if (player.CertificateData.ExtraData.Xuid != null && player.Username.Equals("yungtechboy1"))
            {
                player.ActionPermissions = ActionPermissions.Operator;
                player.CommandPermission = 4;
                player.PermissionLevel = PermissionLevel.Operator;
                player.SendAdventureSettings();
            }

            player.SendPlayerInventory();

            player.SendArmorForPlayer();
            player.SendEquipmentForPlayer();

            _players.TryAdd(player.Username, player);

            ThreadPool.QueueUserWorkItem(state =>
            {
                Thread.Sleep(2000);
                level.BroadcastMessage(
                    $"{ChatColors.Gold}[{ChatColors.Green}+{ChatColors.Gold}]{ChatFormatting.Reset} {player.Username} joined the server");
                var joinSound = new AnvilUseSound(level.SpawnPoint.ToVector3());
                joinSound.Spawn(level);

                //player.SendTitle(null, TitleType.Clear);
                player.SendTitle(null, TitleType.AnimationTimes, 6, 6, 20 * 10);

                player.SendTitle($"{ChatColors.White}This is Yungtech's MiNET test server", TitleType.SubTitle);
                player.SendTitle($"{ChatColors.Gold}Welcome {player.Username}!", TitleType.Title);
            });
        }

        private ConcurrentDictionary<string, Player> _players = new ConcurrentDictionary<string, Player>();

        private void OnPlayerLeave(object o, PlayerEventArgs eventArgs)
        {
            Level level = eventArgs.Level;
            if (level == null) throw new ArgumentNullException(nameof(eventArgs.Level));

            Player player = eventArgs.Player;
            if (player == null) throw new ArgumentNullException(nameof(eventArgs.Player));

            Player trash;
            _players.TryRemove(player.Username, out trash);

            level.BroadcastMessage(
                $"{ChatColors.Gold}[{ChatColors.Red}-{ChatColors.Gold}]{ChatFormatting.Reset} {player.Username} left the server");
            var leaveSound = new AnvilBreakSound(level.SpawnPoint.ToVector3());
            leaveSound.Spawn(level);
        }

        public OpenServer getServer()
        {
            return API.OpenServer;
        }

        public OpenApi getAPI()
        {
            return API;
        }

        private OpenApi API;

        public override void Enabled(OpenApi api)
        {
              
           
            
            
            API = api;
            API.OpenServer.PlayerFactory = new CyberPlayerFactory(API);
            Console.WriteLine("================Executed startup successfully. Replaced identity managment=========================");
            Log.Info("================Executed startup successfully. Replaced identity managment=========================");

            
            FM = new FactionsMain(this);
            
            AF = new AuctionFactory(this);
            api.EventDispatcher.RegisterEvents(new MasterListener());
            api.CommandManager.RegisterPermissionChecker(new FactionPermissionChecker(FM.FFactory));
            api.CommandManager.RegisterPermissionChecker(new FactionCommandChecker(this));
            api.CommandManager.RegisterPermissionChecker(new ServerRankChecker(this));
            api.CommandManager.LoadCommands(new FactionCommands(FM.FFactory));
            api.CommandManager.LoadCommands(new CyberCommands(this));

            
            FTM = new FloatingTextFactory(this);
            
            // CyberExperimentalWorldProvider generator;

            /* generator = new MiNET.Worlds.SuperflatGenerator(Dimension.Overworld)
             {
                 BlockLayers = new List<Block>()
                 {
                     new Bedrock(),
                     new Dirt(),
                     new Dirt(),
                     new Grass()
                 }
             };*/
            new BiomeManager();
            Log.Info("DA LENGTH OF TITTTTTTTTTT IS :"+AnvilWorldProvider.Convert.Count);
            // generator = new CyberExperimentalWorldProvider(Config.GetProperty("Level.Seed", 123123));
            //
            // Log.Info("DA LENGTH OF TITTTTTTTTTT IS :"+AnvilWorldProvider.Convert.Count);
            // var a = new AnvilWorldProvider(Config.GetProperty("PCWorldFolder", "CyberWorld"))
            // {
            //     MissingChunkProvider = generator
            // };
            
            Log.Info("DA LENGTH OF TITTTTTTTTTT IS :"+AnvilWorldProvider.Convert.Count);
            var level = new OpenLevel(api, api.LevelManager, Dimension.Overworld.ToString(),
                new CyberExperimentalWorldProvider(123123,Config.GetProperty("PCWorldFolder", "CyberWorld")),  api.LevelManager.EntityManager, GameMode.Creative,
                Difficulty.Peaceful);
            // generator.Level = level;
            // ((CyberExperimentalWorldProvider) ((AnvilWorldProvider)((WrappedCachedWorldProvider) level.WorldProvider).CachingWorldProvider).MissingChunkProvider).Level = level;

            api.LevelManager.LoadLevel(level);
            api.LevelManager.SetDefaultLevel(level);
            
            
            CrateMain = new CrateMain(this);
            
            // api.LevelManager.LevelCreated += delegate(object? sender, LevelEventArgs args)
            // {
            //     var a = args.Level;
            //     a.SpawnPoint = new PlayerLocation(0,2,0);
            // };
            // var l = getAPI().LevelManager.GetDefaultLevel();
            // FloatingTextFactory.AddFloatingText(new CyberFloatingTextContainer(FTM,l.SpawnPoint+new PlayerLocation(0,2,0) ,l));


            // getServer().PlayerFactory.PlayerCreated += (sender, args) =>
            // {
            //     Player player = args.Player;
            //     player.PlayerJoin += OnPlayerJoin;
            //     // player.PlayerJoin += MasterListener.joinEvent;
            //     player.PlayerLeave += OnPlayerLeave;
            //     player.Ticking += OnTicking;
            // };


            //Fix BlockFactory

            // Console.WriteLine("WAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA =>>");
            // var w = new Wood();
            // w.WoodType = "birch";
            // int id = BlockFactory.BlockPalette[w.GetRuntimeId()].Id;
            // Console.WriteLine($"{w.GetRuntimeId()} |||||| {id}");
            // string directoryName = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            // string path = Path.Combine(directoryName, "Blocks");
            // path = Path.Combine(path, "blockstates.json");
            // using (StreamReader file = File.OpenText(path))
            // {
            //     string json = file.ReadToEnd();
            //     BlockFactory.BlockPalette = BlockPalette.FromJson(json);
            //     BlockFactory.BlockStates = new HashSet<BlockStateContainer>(BlockFactory.BlockPalette);
            // }
            //
            // w = new Wood();
            // w.WoodType = "birch";
            // id = BlockFactory.BlockPalette[w.GetRuntimeId()].Id;
            // Console.WriteLine($"{w.GetRuntimeId()} || {id}");
            
            ShopFactory = new ShopFactory(this);

            // api.CommandManager.RegisterPermissionChecker(new FactionPermissionChecker(FactionManager));
            //
            // api.CommandManager.LoadCommands(CommandsClass);
            // api.CommandManager.LoadCommands(new FactionCommands(FactionManager));
        }

        public AuctionFactory AF { get; set; }
        public ShopFactory ShopFactory { get; set; }

        [PacketHandler, Receive]
        public Packet HandleIncoming(McpeMovePlayer packet,Player p)
        {
            Console.WriteLine("_--------------"+p.GetType());
            return packet; // Process
        }
        [PacketHandler, Receive]
        public Packet HandleIncoming2(McpeMovePlayer packet)
        {
            Console.WriteLine("_--------------22222222222");
            return packet; // Process
        }
        
        private void OnTicking(object sender, PlayerEventArgs e)
        {
            var player = e.Player;
            var level = player.Level;

            // if (level.TickTime % 10 == 0 && player.Inventory.GetItemInHand() is CustomTestItem item)
            // {
            //  player.SendMessage("0x" + item.SomeVariable.ToString("X"), MessageType.Popup);
            // }
        }

        public override void Disabled(OpenApi api)
        {
        
            CrateMain.save();
            FTM.stop();
            // api.CommandManager.UnloadCommands(CommandsClass);
        }

        public CorePlayer getPlayer(Player name)
        {
            return (CorePlayer) getAPI().PlayerManager.getPlayer(name.Username);
        }
        public CorePlayer getPlayer(string name)
        {
            Log.Info("GET CORE PLAYER USED)))))))))))))))))))))))))))))))))))))))))))))))))))))))))");
            return (CorePlayer) getAPI().PlayerManager.getPlayer(name);
        }

            public double getTicksPerSecond()
        {
            //50 = Good
            long avg = 50;
            foreach (var l in getAPI().LevelManager.Levels)
            {
                long a = l.AvarageTickProcessingTime;
                avg = (a + avg) / 2;
            }

            return 1000d / avg;
        }

        public double getTicksPerSecond(Level l)
        {
            //50 = Good
            long avg = 50;
            long a = l.AvarageTickProcessingTime;
            avg = (a + avg) / 2;


            return 1000d / avg;
        }

        public List<Player> getOnlinePlayers()
        {
            List<Player> a =  new List<Player>();
            foreach (var l in getAPI().LevelManager.Levels)
            {
               a.AddRange(l.GetAllPlayers());
            }

            return a;
        }
        public int getOnlinePlayersCount()
        {
            int amt = 0;
            foreach (var l in getAPI().LevelManager.Levels)
            {
                int a = l.GetAllPlayers().Length;
                amt += a;
            }

            return amt;
        }
        public Rank2 getPlayerRank(String p) {
            return RankFactory.getPlayerRank(p);
        }


        public Rank2 getPlayerRank(CorePlayer p) {
            return RankFactory.getPlayerRank(p);
        }
    }
}