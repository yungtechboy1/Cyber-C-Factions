using System;
using System.Collections.Generic;
using CyberCore.CustomEnums;
using static CyberCore.Manager.Rank.RankEnum;

namespace CyberCore.Manager.Rank
{
    public class RankList2
    {
        private static RankList2 instance;
        // public static readonly Rank2 GuestRank = new Rank2(-1,"Guest");
        private CyberCoreMain Main;
        public Dictionary<String,Rank2> RankList = new Dictionary<String,Rank2>();
        public RankList2(CyberCoreMain m)
        {
            Main = m;
            instance = this;
            preloadRanks();
        }

        public static RankList2 getInstance()
        {
            return instance;
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
            addRank(new GuestRank());
            addRank(new MemberRank());
            addRank(new HelperRank());
            addRank(new BuilderRank());
            addRank(new CitizenRank());
            addRank(new CitizenpRank());
            addRank(new NobleManRank());
            addRank(new KnightRank());
            addRank(new GeneralRank());
            addRank(new DukeRank());
            addRank(new KingRank());
            addRank(new JrServerModRank());
            addRank(new JrServerAdminRank());
            addRank(new ServerModeratorRank());
            addRank(new ServerAdminRank());
            addRank(new AdminRank());
            // addRank();
            // addRank();
            // addRank();
            // addRank();
            // addRank(new Rank2(5,"Member",1));
            // addRank(new Rank2(10,"Helper",2));
            // addRank(new Rank2(11,"Builder",3));
            // addRank(new Rank2(12,"Citizen",4));
            // addRank(new Rank2(13,"Citizen+",5));
            // addRank(new Rank2(14,"Representative",6));
            // addRank(new Rank2(15,"Senator",7));
            // addRank(new Rank2(16,"General",8));
            // addRank(new Rank2(8," Jr Server Mod",9));
            // addRank(new Rank2(6,"Jr Server Admin",10));
            // addRank(new Rank2(9,"Server Moderator",11));
            // addRank(new Rank2(7,"Server Admin",12));
            // addRank(new Rank2(3,"Administrative",13));
        }

        public Rank2 getRankFromID(int i)
        {
            foreach (var r in RankList.Values)
            {
                if (r.getID() == i) return r;
            }

            return null;
        }
        public Rank2 getRankFromID(RankEnum i)
        {
            foreach (var r in RankList.Values)
            {
                if (r.ID == i) return r;
            }

            return null;
        }
    }

    public enum RankEnum
    {
        Guest = -1,
        Member = 5,
        Helper = 10,
        Builder = 11,
        Citizen = 12,
        Citizenp = 13,
        Nobleman = 14,
        Knight = 15,
        Duke = 18,
        Prince = 20,
        King = 28,
        General = 16,
        JrServerMod = 8,
        JrServerAdmin = 6,
        ServerModerator = 9,
        ServerAdmin = 7,
        Administrative = 3,
    }
 

    public class GuestRank : Rank2
    {
        public GuestRank() : base(Guest, "Guest")
        {
            
        }
    }
    public class MemberRank : Rank2
    {
        public MemberRank() : base(Member, "Member", 1)
        {
            
        }
    }

    public class HelperRank : Rank2
    {
        public HelperRank() : base(Helper,"Helper",2){
            
        }
    }
    public class BuilderRank : Rank2
    {
        public BuilderRank() : base(Builder,"Builder",3){
            
        }
    }
    public class CitizenRank : Rank2
    {
        public CitizenRank() : base(Citizen,"Citizen",4){
            
        }
    }
    public class CitizenpRank : Rank2
    {
        public CitizenpRank() : base(Citizenp,"Citizen+",5){
            
        }
    }
    public class NobleManRank : Rank2
    {
        public NobleManRank() : base(Nobleman,"NobleMan",6){
            
        }
    }
    public class KnightRank : Rank2
    {
        public KnightRank() : base(Knight,"Senator",7){
            
        }
    }
    public class GeneralRank : Rank2
    {
        public GeneralRank() : base(General,"General",8){
            
        }
    }
    public class PrinceRank : Rank2
    {
        public PrinceRank() : base(Prince,"Prince",9){
            
        }
    }
    public class KingRank : Rank2
    {
        public KingRank() : base(King,"King",11){
            
        }
    }
    public class DukeRank : Rank2
    {
        public DukeRank() : base(Duke,"Duke",10){
            
        }
    }
    public class JrServerModRank : Rank2
    {
        public JrServerModRank() : base(JrServerMod," Jr Server Mod",20){
            
        }
    }
    public class JrServerAdminRank : Rank2
    {
        public JrServerAdminRank() : base(JrServerAdmin,"Jr Server Admin",21){
            
        }

    }
    public class ServerModeratorRank : Rank2
    {
        public ServerModeratorRank() : base(ServerModerator,"Server Moderator",22){
            
        }
    }
    public class ServerAdminRank : Rank2
    {
        public ServerAdminRank() : base(ServerAdmin,"Server Admin",23){
            
        }
    }
    public class AdminRank : Rank2
    {
        public AdminRank() : base(Administrative,"Administrative",23){
            
        }
    }
    
    
}