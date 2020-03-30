using System;

namespace CyberCore.CustomEnums
{
    public struct RankList
    {
        public static readonly RankList sPERM_GUEST = new RankList(0, "Guest");
        public static readonly RankList PERM_MEMBER = new RankList(1, "Member");
        public static readonly RankList PERM_VIP = new RankList(3, "VIP");
        public static readonly RankList PERM_OP = new RankList(20, "SuperOP");
            
        private RankList(int id, String name)
        {
            Name = name;
            ID = id;
            chat_prefix = "&7";
        }

        public int ID { get; set; }

        public string Name { get; set; }
        public string chat_prefix { get; set; }

        public int getID()
        {
            return ID;
        }

        public string getName()
        {
            return Name;
        }

        public string getChat_prefix()
        {
            return chat_prefix;
        }
    }
}