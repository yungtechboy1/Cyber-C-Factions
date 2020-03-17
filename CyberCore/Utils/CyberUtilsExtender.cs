using System;
using CyberCore.Manager.Factions;
using CyberCore.Manager.Forms;
using JetBrains.Annotations;
using MiNET;
using MiNET.UI;
using MiNET.Utils;
using OpenAPI.Player;

namespace CyberCore.Utils
{
    public static class CyberUtilsExtender
    {

        public static String getMessage(this FactionErrorString c)
        {
            return FactionErrorStringMethod.toString(c);
        }
        public static Faction getFaction(this Player c)
        {
            return CyberCoreMain.GetInstance().FM.FFactory.getPlayerFaction(c);
        }

        public static String getName(this FactionRank s)
        {
            return FactionRankMethods.getName(s);
        }
        public static String getChatColor(this FactionRank s)
        {
            return FactionRankMethods.getChatColor(s);
        }
        public static int getFormPower(this FactionRank s)
        {
            return FactionRankMethods.getFormPower(s);
        }
        public static FactionRank getRankFromForm(this FactionRank s, int ss)
        {
            return FactionRankMethods.getRankFromForm(ss);
        }
        public static FactionRank getRankFromString(this FactionRank s, String ss)
        {
            return FactionRankMethods.getRankFromString(ss);
        }
        public static bool equalsIgnoreCase(this String s, String ss)
        {
            return s.Equals(ss, StringComparison.CurrentCultureIgnoreCase);
        }

        public static int getID(this MainForm f)
        {
            return f;
        }
        
        public static void showFormWindow(this OpenPlayer p, Form f)
        {
            p.SendForm(f);
        }

        public static ExtraPlayerData GetExtraPlayerData(this OpenPlayer p)
        {
            return CyberUtils.getExtraPlayerData(p);
        }public static ExtraPlayerData GetExtraPlayerData(this Player p)
        {
            return CyberUtils.getExtraPlayerData(p);
        }

        [CanBeNull]
        public static OpenPlayer getPlayer(this OpenPlayerManager p, String name)
        {
            foreach (OpenPlayer players in p.GetPlayers())
            {
                if (players.getName().equalsIgnoreCase(name)) return players;
            }

            return null;
        }
        public static String getName(this Player p)
        {
            return p.Username;
        }
        
    }
}