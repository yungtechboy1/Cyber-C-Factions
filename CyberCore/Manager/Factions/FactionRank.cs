using System;
using log4net;
using MiNET;
using MiNET.Utils;

namespace CyberCore.Manager.Factions
{
    public enum FactionRankEnum
    {
        Recruit = 0,
        Member = 1,
        Officer = 2,
        General = 3,
        Leader = 4,
        All = -1,
        None = -2,
    }

    public struct FactionRank
    {
        public static readonly FactionRank None = new FactionRank(FactionRankEnum.None);
        public static readonly FactionRank Recruit = new FactionRank(FactionRankEnum.Recruit);
        public static readonly FactionRank Member = new FactionRank(FactionRankEnum.Member);
        public static readonly FactionRank Officer = new FactionRank(FactionRankEnum.Officer);
        public static readonly FactionRank General = new FactionRank(FactionRankEnum.General);
        public static readonly FactionRank Leader = new FactionRank(FactionRankEnum.Leader);
        public static readonly FactionRank All = new FactionRank(FactionRankEnum.All);
        private static readonly ILog Log = LogManager.GetLogger(typeof(FactionRank));

        public FactionRankEnum Id;

        public FactionRank(FactionRankEnum id)
        {
            Id = id;
        }

        public static FactionRank getRankFromString(String a)
        {
            try
            {
                int i = int.Parse(a);
                foreach (FactionRank f in Enum.GetValues(typeof(FactionRank)))
                {
                    if ((int) f.Id == i) return f;
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

        public FactionRank getRankFromString(FactionRank a)
        {
            try
            {
                int i = (int) a.Id;
                //TODO might give error!
                foreach (FactionRank f in Enum.GetValues(typeof(FactionRank)))
                {
                    if ((int) f.Id == i) return f;
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

        public FactionRank getRankFromForm(int a)
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

        public static FactionRank getRankFromFactionRankEnum(FactionRankEnum a)
        {
            switch (a)
            {
                case FactionRankEnum.Leader:
                    return FactionRank.Leader;
                case FactionRankEnum.General:
                    return FactionRank.General;
                case FactionRankEnum.Officer:
                    return FactionRank.Officer;
                case FactionRankEnum.Member:
                    return FactionRank.Member;
                case FactionRankEnum.Recruit:
                    return FactionRank.Recruit;
                default:
                    Log.Error("Error tring to Parse " + a + " TO FactionRank");
                    return FactionRank.None;
            }
        }

        public int getFormPower(FactionRank r)
        {
            if ((int) r.Id == -1) return (int) r.Id;
            if (Leader.Id == r.Id) return 0;
            if (General.Id == r.Id) return 1;
            if (Officer.Id == r.Id) return 2;
            if (Member.Id == r.Id) return 3;
            if (Recruit.Id == r.Id) return 4;
            return 0;
        }

        public int getPower()
        {
            return (int) Id;
        }

        public string getChatColor()
        {
            switch ((int) Id)
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
        public bool hasPerm(FactionRank target)
        {
            return Id >= target.Id;
        }


        public void SendFailReason(FactionRank target, Player p)
        {
            p.SendMessage(ChatColors.Red + "Error! You must be a " + target.getName() + " to use this command!");
        }

        public String getName()
        {
            switch ((int) Id)
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
                    return "Unknown-" + Id;
            }
        }

        public String GetChatPrefix()
        {
            switch ((int)Id)
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

        public FactionRankEnum toEnum()
        {
            return Id;
        }
    }
}