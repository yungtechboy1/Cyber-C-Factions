using MiNET;
using MiNET.Net;

namespace Faction2.Utils
{
	public class CyberTechServer : IServer
	{
		private readonly MiNetServer _server;

		protected CyberTechServer()
		{
		}

		public CyberTechServer(MiNetServer server)
		{
			_server = server;
		}

		public virtual IMcpeMessageHandler CreatePlayer(INetworkHandler session, PlayerInfo playerInfo)
		{
			CyberPlayer player = ((CyberPlayerFactory)_server.PlayerFactory)?.CreatePlayerCyberPlayer(_server, session.GetClientEndPoint(), playerInfo);
			player.NetworkHandler = session;
			player.CertificateData = playerInfo.CertificateData;
			player.Username = playerInfo.Username;
			player.ClientUuid = playerInfo.ClientUuid;
			player.ServerAddress = playerInfo.ServerAddress;
			player.ClientId = playerInfo.ClientId;
			player.Skin = playerInfo.Skin;
			player.PlayerInfo = playerInfo;

			return player;
		}
	}
}