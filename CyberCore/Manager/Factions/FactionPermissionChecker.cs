using CyberCore.Utils;
using OpenAPI.Commands;
using OpenAPI.Player;

namespace CyberCore.Manager.Factions
{
    public class FactionPermissionChecker : CommandPermissionChecker<FactionPermissionAttribute>
    {
        private FactionFactory Manager { get; set; }

        public FactionPermissionChecker(FactionFactory manager)
        {
            Manager = manager;
        }

        public override bool HasPermission(FactionPermissionAttribute attr, OpenPlayer player)
        {
            if (!Manager.isPlayerInFaction(player)) return false;
            var r = Manager.getPlayerFaction(player).getPlayerRank(player);
            return r.hasPerm(attr.Permission.toFactionRank());
        }
    }
    
    public class FactionPermissionAttribute : CommandPermissionAttribute
    {
        public FactionRankEnum Permission { get; set; }

        public FactionPermissionAttribute(FactionRankEnum permission)
        {
            Permission = permission;
        }
    }
}