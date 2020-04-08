using System;
using System.Collections.Generic;
using CyberCore.Manager.Factions.Windows;
using CyberCore.Utils;
using MiNET.Plugins;
using MiNET.Plugins.Attributes;
using OpenAPI.Player;

namespace CyberCore.Manager.Factions
{
    public class FactionCommands
    {
        private FactionFactory Manager;

        public FactionCommands(FactionFactory manager)
        {
            Manager = manager;
        }
        private List<String> bannednames = new List<String>() {
            ("wilderness"),
            ("safezone"),
            ("peace")
        };
        //GOOD TO KNOW
        //typeof(OpenPlayer).IsAssignableFrom(parameter.ParameterType)

        [Command(Name = "f create",Description = "Create a new faction")]
        // [FactionPermission(FactionRankEnum.None)]
        public void FCreate(OpenPlayer p)
        {
            if (p is CorePlayer)
            {
                CyberCoreMain.Log.Error("YOYOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOO");
                CyberCoreMain.Log.Error("YOYOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOO");
                CyberCoreMain.Log.Error("YOYOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOO");
                CyberCoreMain.Log.Error(((CorePlayer)p).GetRank().Name+" <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<< WORKZ");
                CyberCoreMain.Log.Error("YOYOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOO");
                CyberCoreMain.Log.Error("YOYOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOO");
            }
            else
            {
                CyberCoreMain.Log.Error("11111111111111111111111111111111111111111111111111YOYOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOO");
            }
            p.SendForm(new FactionCreate0());
        }

        [Command(Name = "f invite2", Description = "Create a new faction")]
        // [FactionPermission(FactionRankEnum.None)]
        public void FInvite2(OpenPlayer Sender, Target invited)
        {
            
            var m = "";
            Sender.SendMessage(m);
            CyberCoreMain.Log.Error("==========================================");
            CyberCoreMain.Log.Error(m);
            CyberCoreMain.Log.Error(invited);
            CyberCoreMain.Log.Error(invited.Entities.Length);
            CyberCoreMain.Log.Error(invited.Players.Length);
            CyberCoreMain.Log.Error(invited.Rules.Length);
            CyberCoreMain.Log.Error(invited.Selector.Length);
            CyberCoreMain.Log.Error("==========================================");
        }
        [Command(Name = "f invite",Description = "Create a new faction")]
        public void FInvite(CorePlayer Sender, CorePlayer invited)
        {
            if (invited == null) {
                Sender.SendMessage(FactionErrorString.Error_CMD_Invite_UnableToFindPlayer.getMsg() + "!@ SUPER ERROR!!!@@@");
                return;
            }
            if (null != Manager.getPlayerFaction(invited)) {
                //TODO Allow Setting to ignore Faction messages
                Sender.SendMessage(FactionErrorString.Error_CMD_Invite_PlayerInFaction.getMsg());
                return;
            }
            Sender.showFormWindow(new FactionInviteChooseRank(Sender, invited));
        }
        
        
        
        
    }
}