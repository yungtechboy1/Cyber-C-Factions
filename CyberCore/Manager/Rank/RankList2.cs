using System;
using System.Collections.Generic;
using CyberCore.CustomEnums;

namespace CyberCore.Manager.Rank
{
    public class RankList2
    {
        public static readonly Rank2 GuestRank = new Rank2(-1,"Guest");
        private CyberCoreMain Main;
        public Dictionary<String,Rank2> RankList = new Dictionary<String,Rank2>();
        public RankList2(CyberCoreMain m)
        {
            Main = m;
            preloadRanks();
        }
        public RankChatFormat chat_format = RankChatFormat.Default;

        public void addRank(Rank2 r)
        {
            var id = r.getID();
            foreach (var v in RankList.Values)
            {
                if (v.getID() == id)
                {
                    CyberCoreMain.Log.Error("ERROR! RankList Error! Can not add Rank "+r.getName()+" because ID is the Same as "+v.Name);
                    return;
                }
            }
            RankList[r.Name] = r;
        }
        
        public void preloadRanks()
        {
            addRank(GuestRank);
            addRank(new Rank2(5,"Member",1));
            addRank(new Rank2(10,"Helper",2));
            addRank(new Rank2(11,"Builder",3));
            addRank(new Rank2(12,"Citizen",4));
            addRank(new Rank2(13,"Citizen+",5));
            addRank(new Rank2(14,"Representative",6));
            addRank(new Rank2(15,"Senator",7));
            addRank(new Rank2(16,"General",8));
            addRank(new Rank2(8," Jr Server Mod",9));
            addRank(new Rank2(6,"Jr Server Admin",10));
            addRank(new Rank2(9,"Server Moderator",11));
            addRank(new Rank2(7,"Server Admin",12));
            addRank(new Rank2(3,"Administrative",13));
        }

        public Rank2 getRankFromID(int i)
        {
            foreach (var r in RankList.Values)
            {
                if (r.ID == i) return r;
            }

            return null;
        }
    }
}