using System;

namespace CyberCore.Manager.Rank
{
    public class Rank2
    {
        public String display_name = "";
        public Rank2(RankEnum id, String name,int weight =-1)
        {
            Name = name;
            Weight = weight;
            ID = id;
            display_name = name;
            chat_prefix = "&7";
        }

        public bool hasPerm(Rank2 r)
        {
            return Weight >= r.Weight;
        }
        public bool hasPerm(RankEnum r)
        {
            var rr = RankList2.getInstance().getRankFromID(r);
            if (rr == null) return true;
            return Weight >= rr.Weight;
        }
        
        
        public Rank2 toRank2()
        {
            return (Rank2) this;
        }
        
        public RankEnum ID { get; set; }
        public int Weight { get; set; }

        public string Name { get; set; }
        public string chat_prefix { get; set; }

        public RankEnum getIdEnum()
        {
            return ID;
        }
        public int getID()
        {
            return (int) ID;
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