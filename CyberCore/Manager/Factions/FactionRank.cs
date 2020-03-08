using System;
using log4net;
using MiNET;
using MiNET.Utils;

namespace CyberCore.Manager.Factions
{
    public enum FactionRank
    {
        Recruit = 0,
        Member = 1,
        Officer = 2,
        General = 3,
        Leader = 4,
        All = -1
    }

    static class FactionRankMethods
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(FactionRankMethods));

        public static FactionRank getRankFromString(String a)
        {
            try
            {
                int i = int.Parse(a);
                foreach (FactionRank f in Enum.GetValues(typeof(FactionRank)))
                {
                    if ((int) f == i) return f;
                }

                CyberCoreMain.Log.Error("Error Parsing ");
                return FactionRank.Recruit;
            }
            catch (Exception e)
            {
                Console.WriteLine("Error attempting to parse Rank String to Int to Rank");
                return FactionRank.Recruit;
            }
        }
        public static FactionRank getRankFromString(FactionRank a)
        {
            try
            {
                int i = (int)(a);
                foreach (FactionRank f in Enum.GetValues(typeof(FactionRank)))
                {
                    if ((int) f == i) return f;
                }

                CyberCoreMain.Log.Error("Error Parsing ");
                return FactionRank.Recruit;
            }
            catch (Exception e)
            {
                Console.WriteLine("Error attempting to parse Rank String to Int to Rank");
                return FactionRank.Recruit;
            }
        }

        public static FactionRank getRankFromForm(int a)
        {
            switch (a)
            {
                case 0:
                    return FactionRank.Leader;
                case 1:
                    return FactionRank.General;
                case 2:
                    return FactionRank.Officer;
                case 3:
                    return FactionRank.Member;
                case 4:
                    return FactionRank.Recruit;
                default:
                    return FactionRank.Recruit;
            }
        }

        public static int getFormPower(FactionRank r)
        {
            if ((int)r == -1) return (int)r;
            switch (r)
            {
                case FactionRank.Leader:
                    return 0;
                case FactionRank.General:
                    return 1;
                case FactionRank.Officer:
                    return 2;
                case FactionRank.Member:
                    return 3;
                case FactionRank.Recruit:
                    return 4;
                default:
                    return 0;
            }
        }

        public static int getPower(FactionRank r)
        {
            return (int)r;
        }

        public static string getChatColor(FactionRank r)
        {
            switch ((int)r)
            {
                case 0:
                    return ChatColors.Gray;
                case 1:
                    return ChatColors.Aqua;
                case 2:
                    return ChatColors.Yellow;
                case 3:
                    return ChatColors.Green;
                case 4:
                    return ChatColors.Gold;
                default:
                    return ChatColors.LightPurple;
            }
        }

//FactionCommandWindow.java:40
        public static bool hasPerm(FactionRank src,FactionRank target)
        {
            if (target == null) return false;
            return (int)src >= (int)target;
        }


        public static void SendFailReason(FactionRank target, Player p)
        {
            p.SendMessage(ChatColors.Red + "Error! You must be a " + getName(target) + " to use this command!");
        }

        public static String getName(FactionRank t)
        {
            switch ((int)t)
            {
                case 0:
                    return "Recruit";
                case 1:
                    return "Member";
                case 2:
                    return "General";
                case 3:
                    return "Officer";
                case 4:
                    return "Leader";
                default:
                    return "Unknown-" + (int)t;
            }
        }

        public static String GetChatPrefix(FactionRank t)
        {
            switch ((int)t)
            {
                case 0:
                    return ChatColors.Gray + "R";
                case 1:
                    return ChatColors.Aqua + "M";
                case 2:
                    return ChatColors.Yellow + "G";
                case 3:
                    return ChatColors.Green + "O";
                case 4:
                    return ChatColors.Gold + "L";
                default:
                    return ChatColors.LightPurple + "-";
            }
        }
    }
}