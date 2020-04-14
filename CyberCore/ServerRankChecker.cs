using CyberCore.CustomEnums;
using CyberCore.Manager.Factions;
using CyberCore.Manager.Rank;
using CyberCore.Utils;
using OpenAPI.Commands;
using OpenAPI.Player;

namespace CyberCore
{
    public class ServerRankChecker : CommandPermissionChecker<ServerRankAttrAttribute>
    {
        private CyberCoreMain Manager { get; set; }

        public ServerRankChecker(CyberCoreMain manager)
        {
            Manager = manager;
        }

        public override bool HasPermission(ServerRankAttrAttribute attr, OpenPlayer player)
        {
            Rank2 rr =Manager.getPlayerRank((CorePlayer) player);
            return true;//FIX
        }
    }
    
    public class ServerRankAttrAttribute : CommandPermissionAttribute
    {
        public Rank2 Permission { get; set; }

        public ServerRankAttrAttribute(Rank2 permission)
        {
            Permission = permission;
        }
    }
}