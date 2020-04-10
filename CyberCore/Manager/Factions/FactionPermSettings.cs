using System;
using CyberCore.Manager.Factions;
using Newtonsoft.Json;
using static CyberCore.Manager.Factions.FactionRank;

namespace CyberCore.Manager.Factions
{
    public class FactionPermSettingsData
    {
        public FactionRank AllowedToViewInbox { get; set; } = Officer;
        public FactionRank AllowedToAcceptAlly { get; set; } = General;
        public FactionRank AllowedToEditSettings { get; set; } = Leader;
        public FactionRank AllowedToPromote { get; set; } = Member;
        public FactionRank AllowedToKick { get; set; } = General;
        public int MaxFactionChat { get; set; } = 30;
        public int MaxAllyChat { get; set; } = 30;
        public int WeeklyFactionTax { get; set; } = 0;
        public FactionRank AllowedToInvite { get; set; } = Member;
        public FactionRank DefaultJoinRank { get; set; } = Recruit;
        public FactionRank AllowedToClaim { get; set; } = General;
        public FactionRank AllowedToWinthdraw { get; set; } = General;
        public FactionRank AllowedToSetHome { get; set; } = General;

        public FactionPermSettingsData()
        {
            
        }
        public FactionPermSettingsData(FactionPermSettings f)
        {
            AllowedToViewInbox = f.AllowedToClaim;
            AllowedToAcceptAlly = f.AllowedToAcceptAlly;
            AllowedToEditSettings = f.AllowedToEditSettings;
            AllowedToPromote = f.AllowedToPromote;
            AllowedToKick = f.AllowedToKick;
            MaxFactionChat = f.MaxFactionChat;
            MaxAllyChat = f.MaxAllyChat;
            WeeklyFactionTax = f.WeeklyFactionTax;
            AllowedToInvite = f.AllowedToInvite;
            DefaultJoinRank = f.DefaultJoinRank;
            AllowedToClaim = f.AllowedToClaim;
            AllowedToWinthdraw = f.AllowedToWinthdraw;
            AllowedToSetHome = f.AllowedToSetHome;
        }
    }

    public class FactionPermSettings
    {
        public FactionRank AllowedToViewInbox = Officer;
        public FactionRank AllowedToAcceptAlly = General;
        public FactionRank AllowedToEditSettings = Leader;
        public FactionRank AllowedToPromote = Member;

        public FactionRank AllowedToKick = General;

        //    private FactionRank AllowedToDemote = FactionRank.Member;
        public int MaxFactionChat = 30;
        public int MaxAllyChat = 30;
        public int WeeklyFactionTax = 0;
        public FactionRank AllowedToInvite = Member;
        public FactionRank DefaultJoinRank = Recruit;
        public FactionRank AllowedToClaim = General;
        public FactionRank AllowedToWinthdraw = General;
        public FactionRank AllowedToSetHome = General;

        public FactionPermSettings()
        {
        }
        
        public FactionPermSettings(FactionPermSettingsData f)
        {
            AllowedToViewInbox = f.AllowedToClaim;
            AllowedToAcceptAlly = f.AllowedToAcceptAlly;
            AllowedToEditSettings = f.AllowedToEditSettings;
            AllowedToPromote = f.AllowedToPromote;
            AllowedToKick = f.AllowedToKick;
            MaxFactionChat = f.MaxFactionChat;
            MaxAllyChat = f.MaxAllyChat;
            WeeklyFactionTax = f.WeeklyFactionTax;
            AllowedToInvite = f.AllowedToInvite;
            DefaultJoinRank = f.DefaultJoinRank;
            AllowedToClaim = f.AllowedToClaim;
            AllowedToWinthdraw = f.AllowedToWinthdraw;
            AllowedToSetHome = f.AllowedToSetHome;
        }

        public FactionPermSettings(String i)
        {
            String[] ii = i.Split("|");
            if (ii.Length != 12)
            {
                Console.WriteLine("Error importing factions settings! Expected length 10, got " + ii.Length+"||"+ii);
                return;
            }

            FactionRank avi = getRankFromString(ii[0]);
            if (None.hasPerm(avi)) AllowedToViewInbox = avi;

            FactionRank aaa = getRankFromString(ii[1]);
            if (None.hasPerm(aaa)) AllowedToAcceptAlly = avi;

            FactionRank aes = getRankFromString(ii[2]);
            if (None.hasPerm(aes)) AllowedToEditSettings = aes;

            FactionRank ap = getRankFromString(ii[3]);
            if (None.hasPerm(ap)) AllowedToPromote = ap;

            FactionRank atk = getRankFromString(ii[4]);
            if (None.hasPerm(atk)) AllowedToKick = avi;

            FactionRank ati = getRankFromString(ii[5]);
            if (None.hasPerm(ati)) AllowedToInvite = ati;

            FactionRank djr = getRankFromString(ii[6]);
            if (None.hasPerm(djr)) DefaultJoinRank = djr;

            FactionRank atc = getRankFromString(ii[7]);
            if (None.hasPerm(atc)) AllowedToClaim = atc;

            FactionRank atw = getRankFromString(ii[8]);
            if (None.hasPerm(atw)) AllowedToWinthdraw = atw;

            FactionRank ash = getRankFromString(ii[9]);
            if (None.hasPerm(avi)) AllowedToViewInbox = ash;


            
            try
            {
                int iii = int.Parse(ii[10]);
                WeeklyFactionTax = iii;
            }
            catch (Exception e)
            {
                Console.WriteLine("Error parseing WeeklyFactionTax! from " + ii[10]);
            }
        }

        public String export()
        {
            return JsonConvert.SerializeObject(new FactionPermSettingsData(this));
        }

        public int getWeeklyFactionTax()
        {
            return WeeklyFactionTax;
        }

        public void setWeeklyFactionTax(int weeklyFactionTax)
        {
            WeeklyFactionTax = weeklyFactionTax;
        }

        public FactionRank getAllowedToWinthdraw()
        {
            return AllowedToWinthdraw;
        }

        public void setAllowedToWinthdraw(FactionRank allowedToWinthdraw)
        {
            AllowedToWinthdraw = allowedToWinthdraw;
        }

        public FactionRank getAllowedToSetHome()
        {
            return AllowedToSetHome;
        }

        public void setAllowedToSetHome(FactionRank allowedToSetHome)
        {
            AllowedToSetHome = allowedToSetHome;
        }

        public FactionRank getAllowedToPromote()
        {
            return AllowedToPromote;
        }

        public void setAllowedToPromote(FactionRank allowedToPromote)
        {
            AllowedToPromote = allowedToPromote;
        }

//    public FactionRank getAllowedToDemote() {
//        return AllowedToDemote;
//    }
//
//    public void setAllowedToDemote(FactionRank allowedToDemote) {
//        AllowedToDemote = allowedToDemote;
//    }

        public int getMaxFactionChat()
        {
            return MaxFactionChat;
        }

        public void setMaxFactionChat(int maxFactionChat)
        {
            MaxFactionChat = maxFactionChat;
        }

        public int getMaxAllyChat()
        {
            return MaxAllyChat;
        }

        public void setMaxAllyChat(int maxAllyChat)
        {
            MaxAllyChat = maxAllyChat;
        }

        public FactionRank getAllowedToViewInbox()
        {
            return AllowedToViewInbox;
        }

        public void setAllowedToViewInbox(FactionRank allowedToViewInbox)
        {
            AllowedToViewInbox = allowedToViewInbox;
        }

        public FactionRank getAllowedToAcceptAlly()
        {
            return AllowedToAcceptAlly;
        }

        public void setAllowedToAcceptAlly(FactionRank allowedToAcceptAlly)
        {
            AllowedToAcceptAlly = allowedToAcceptAlly;
        }

        public FactionRank getAllowedToInvite()
        {
            return AllowedToInvite;
        }

        public void setAllowedToInvite(FactionRank allowedToInvite)
        {
            AllowedToInvite = allowedToInvite;
        }

        public FactionRank getDefaultJoinRank()
        {
            return DefaultJoinRank;
        }

        public void setDefaultJoinRank(FactionRank defaultJoinRank)
        {
            DefaultJoinRank = defaultJoinRank;
        }

        public FactionRank getAllowedToClaim()
        {
            return AllowedToClaim;
        }

        public void setAllowedToClaim(FactionRank allowedToClaim)
        {
            AllowedToClaim = allowedToClaim;
        }

        public FactionRank getAllowedToKick()
        {
            return AllowedToKick;
        }

        public void setAllowedToKick(FactionRank allowedToKick)
        {
            AllowedToKick = allowedToKick;
        }

        public FactionRank getAllowedToEditSettings()
        {
            return AllowedToEditSettings;
        }

        public void setAllowedToEditSettings(FactionRank allowedToEditSettings)
        {
            this.AllowedToEditSettings = allowedToEditSettings;
        }
    }
}