using System;
using log4net;
using MiNET;
using MiNET.Utils;

namespace CyberCore.Manager.Factions
{
    public struct FactionRank
    {
        public static readonly FactionRank Recruit = new FactionRank(0);
        public static readonly FactionRank Member = new FactionRank(1);
        public static readonly FactionRank Officer = new FactionRank(2);
        public static readonly FactionRank General = new FactionRank(3);
        public static readonly FactionRank Leader = new FactionRank(4);
        public static readonly FactionRank All = new FactionRank(-1);
        private static readonly ILog Log = LogManager.GetLogger(typeof(FactionRank));

        public int Id;

        public FactionRank(int id)
        {
            Id = id;
        }

        public FactionRank getRankFromString(String a)
        {
            try
            {
                int i = int.Parse(a);
                foreach (FactionRank f in Enum.GetValues(typeof(FactionRank)))
                {
                    if (f.Id == i) return f;
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
                int i = a.Id;
                //TODO might give error!
                foreach (FactionRank f in Enum.GetValues(typeof(FactionRank)))
                {
                    if (f.Id == i) return f;
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

        public int getFormPower(FactionRank r)
        {
            if (r.Id == -1) return r.Id;
            if (Leader.Id == r.Id) return 0;
            if (General.Id == r.Id) return 1;
            if (Officer.Id == r.Id) return 2;
            if (Member.Id == r.Id) return 3;
            if (Recruit.Id == r.Id) return 4;
            return 0;
        }

        public int getPower(FactionRank r)
        {
            return r.Id;
        }

        public string getChatColor(FactionRank r)
        {
            switch (r.Id)
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
            p.SendMessage(ChatColors.Red + "Error! You must be a " + getName(target) + " to use this command!");
        }

        public String getName(FactionRank t)
        {
            switch (t.Id)
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
                    return "Unknown-" + t.Id;
            }
        }

        public String GetChatPrefix(FactionRank t)
        {
            switch (t.Id)
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