using System;
using MiNET.Utils;
using OpenAPI.Commands;
using OpenAPI.Player;

namespace CyberCore.Manager.Factions
{
    public class FactionCommandChecker : CommandPermissionChecker<FactionCommandAttribute>
    {
        public FactionCommandChecker(CyberCoreMain manager)
        {
            Manager = manager;
        }

        private CyberCoreMain Manager { get; }

        public override bool HasPermission(FactionCommandAttribute attr, OpenPlayer player)
        {
            var f = ((CorePlayer) player).getFaction();
            var b = f != null;
            if(!b)player.SendMessage(ChatColors.Red+"Error! You must be in a Faction to run this command!");
            return b;
        }
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class FactionCommandAttribute : CommandPermissionAttribute
    {
        
    }
}