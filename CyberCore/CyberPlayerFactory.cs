using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Net;
using MiNET;
using MiNET.Net;
using MiNET.Utils;
using OpenAPI;
using OpenAPI.Events.Player;
using OpenAPI.Player;

namespace CyberCore
{
    public class CyberPlayerFactory : OpenPlayerManager
    {
        private readonly OpenApi api;

        public CyberPlayerFactory(OpenApi plugin) : base(plugin)
        {
            api = plugin;
        }

        private ConcurrentDictionary<UUID, CorePlayer> CPlayers { get; } = new ConcurrentDictionary<UUID, CorePlayer>();


        public new CorePlayer[] GetPlayers()
        {
            return CPlayers.Values.ToArray<CorePlayer>();
        }

        public new bool TryGetPlayer(string name, out CorePlayer player)
        {
            player = CPlayers.FirstOrDefault(
                x =>
                    x.Value.Username.StartsWith(name, StringComparison.InvariantCultureIgnoreCase)).Value;
            return player != null;
        }

        public new bool TryGetPlayers(string name, out CorePlayer[] player)
        {
            player = CPlayers
                .Where(x =>
                    x.Value.Username.StartsWith(name, StringComparison.InvariantCultureIgnoreCase))
                .Select(x => x.Value).ToArray();
            return player.Length != 0;
        }


        public override Player CreatePlayer(MiNetServer server, IPEndPoint endPoint, PlayerInfo playerInfo)
        {
            var player = new CorePlayer(server, endPoint, api);
            player.ClientUuid = playerInfo.ClientUuid;
            player.MaxViewDistance = Config.GetProperty("MaxViewDistance", 22);
            player.MoveRenderDistance = Config.GetProperty("MoveRenderDistance", 1);

            /*	if (!Players.TryAdd(playerInfo.ClientUuid, player))
                {
                    Log.Warn("Failed to add player to playermanager!");
                }*/
            //OnPlayerCreated?.Invoke(this, new PlayerCreatedEvent(player));
            api.EventDispatcher.DispatchEvent(new PlayerCreatedEvent(player));
            return player;
        }
    }
}