using System.Net;
using MiNET;
using MiNET.Utils;
using OpenAPI;
using OpenAPI.Events;
using OpenAPI.Events.Player;
using OpenAPI.Player;

namespace CyberCore
{
    public class CyberPlayerFactory : OpenPlayerManager
    {
        private OpenApi api;
        public CyberPlayerFactory(OpenApi plugin) : base(plugin)
        {
            api = plugin;
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