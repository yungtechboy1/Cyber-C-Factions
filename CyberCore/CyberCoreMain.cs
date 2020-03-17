using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using CyberCore.Manager.Factions;
using CyberCore.Utils;
using log4net;
using MiNET;
using MiNET.Net;
using MiNET.Sounds;
using MiNET.Utils;
using MiNET.Worlds;
using OpenAPI;
using OpenAPI.Player;
using OpenAPI.Plugins;

namespace CyberCore
{
    [OpenPluginInfo(Name = "CyberCore", Description = "CyberTech++ Core Plugin", Author = "YungTechBoy1",
        Version = "1.0.0.0-PA", Website = "CyberTechpp.com")]
    public class CyberCoreMain : OpenPlugin
    {
        public static ILog Log { get; private set; } = LogManager.GetLogger(typeof(CyberCoreMain));
        private static CyberCoreMain instance { get; set; }
        public ConfigSection MasterConfig { get; }
        public FactionsMain FM { get; private set; }
        public SqlManager SQL { get; private set; }


        public static CyberCoreMain GetInstance()
        {
            return instance;
        }

        public CyberCoreMain()
        {
            instance = this;
            MasterConfig = new ConfigSection() {ConfigFileName = "MasterConfig.conf"};
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

            getServer().PlayerFactory.PlayerCreated += (sender, args) =>
            {
                Player player = args.Player;
                player.PlayerJoin += OnPlayerJoin;
                player.PlayerLeave += OnPlayerLeave;
                player.Ticking += OnTicking;
            };

            // api.CommandManager.RegisterPermissionChecker(new FactionPermissionChecker(FactionManager));
            //
            // api.CommandManager.LoadCommands(CommandsClass);
            // api.CommandManager.LoadCommands(new FactionCommands(FactionManager));
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
    }
}