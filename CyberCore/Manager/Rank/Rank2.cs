using System;

namespace CyberCore.Manager.Rank
{
    public class Rank2
    {
        public String display_name = "";
        public Rank2(int id, String name,int weight =-1)
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
        
        public int ID { get; set; }
        public int Weight { get; set; }

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