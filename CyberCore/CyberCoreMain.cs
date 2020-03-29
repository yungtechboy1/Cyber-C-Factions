﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using CyberCore.Manager.AuctionHouse;
using CyberCore.Manager.ClassFactory;
using CyberCore.Manager.Factions;
using CyberCore.Manager.FloatingText;
using CyberCore.Manager.Rank;
using CyberCore.Manager.Warp;
using CyberCore.Utils;
using CyberCore.Utils.Data;
using log4net;
using MiNET;
using MiNET.Net;
using MiNET.Plugins;
using MiNET.Sounds;
using MiNET.Utils;
using MiNET.Worlds;
using OpenAPI;
using OpenAPI.Events;
using OpenAPI.Player;
using OpenAPI.Plugins;

namespace CyberCore
{
    [OpenPluginInfo(Name = "CyberCore", Description = "CyberTech++ Core Plugin", Author = "YungTechBoy1",
        Version = "1.0.0.0-PA", Website = "CyberTechpp.com")]
    public class CyberCoreMain : OpenPlugin , IStartup
    {
        // public Dictionary<String, Object> PlayerIdentification = new Dictionary<string, object>();
        public static ILog Log { get; private set; } = LogManager.GetLogger(typeof(CyberCoreMain));

        public void Configure(MiNetServer s)
        {
            // s.ServerManager = new CyberTechServerManager(s);
            s.PlayerFactory = new CyberPlayerFactory(API);
            Log.Info("Executed startup successfully. Replaced identity managment.");
            
        }
        
        private static CyberCoreMain instance { get; set; }

        // public ConfigSection MasterConfig { get; }
        public CustomConfig MasterConfig { get; set; }
        public FactionsMain FM { get; private set; }
        public SqlManager SQL { get; private set; }
        public ServerSqlite ServerSQL { get; set; }
        public WarpManager WarpManager { get; set; }
        public ClassFactory ClassFactory { get; set; }
        public RankFactory RF { get; set; }

        public List<CyberFloatingTextContainer> SavedFloatingText { get; set; } =
            new List<CyberFloatingTextContainer>();


        public static CyberCoreMain GetInstance()
        {
            return instance;
        }

        public CyberCoreMain()
        {
            MasterConfig = new CustomConfig(this, "config.cfg");
            instance = this;
            ServerSQL = new ServerSqlite(this);
            WarpManager = new WarpManager(this);
            // MasterConfig = new ConfigSection() {ConfigFileName = "MasterConfig.conf"};
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
            SQL = new SqlManager(this);
            FM = new FactionsMain(this);
            
            AF = new AuctionFactory(this);

            getServer().PlayerFactory.PlayerCreated += (sender, args) =>
            {
                Player player = args.Player;
                player.PlayerJoin += OnPlayerJoin;
                player.PlayerJoin += MasterListener.joinEvent;
                player.PlayerLeave += OnPlayerLeave;
                player.Ticking += OnTicking;
            };

            foreach (var l in getServer().LevelManager.Levels)
            {
                l.BlockBreak += MasterListener.
            }

            getServer().LevelManager.Levels

            // api.CommandManager.RegisterPermissionChecker(new FactionPermissionChecker(FactionManager));
            //
            // api.CommandManager.LoadCommands(CommandsClass);
            // api.CommandManager.LoadCommands(new FactionCommands(FactionManager));
        }

        public AuctionFactory AF { get; set; }

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
            // api.CommandManager.UnloadCommands(CommandsClass);
        }

        public void HelloWorld(string message, [CallerMemberName] string memberName = "")
        {
            StackTrace stackTrace = new StackTrace();
            var method = stackTrace.GetFrame(1).GetMethod();
            Log.Info($"[TestPlugin] {(method.DeclaringType.FullName)}.{method.Name}: " + message);
        }

        public OpenPlayer getPlayer(string name)
        {
            return getAPI().PlayerManager.getPlayer(name);
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
        public Rank getPlayerRank(String p) {
            return getPlayerRank(getPlayer(p));
        }


        public Rank getPlayerRank(Player p) {
            return RF.getPlayerRank(p);
        }
    }
}