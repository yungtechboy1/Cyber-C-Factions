using System;

namespace CyberCore.Manager.Factions
{
    public class FactionPermSettings
    {
        private FactionRank AllowedToViewInbox = FactionRank.Officer;
    private FactionRank AllowedToAcceptAlly = FactionRank.General;
    private FactionRank AllowedToEditSettings = FactionRank.Leader;
    private FactionRank AllowedToPromote = FactionRank.Member;
    private FactionRank AllowedToKick = FactionRank.General;
    //    private FactionRank AllowedToDemote = FactionRank.Member;
    private int MaxFactionChat = 30;
    private int MaxAllyChat = 30;
    private int WeeklyFactionTax = 0;
    private FactionRank AllowedToInvite = FactionRank.Member;
    private FactionRank DefaultJoinRank = FactionRank.Recruit;
    private FactionRank AllowedToClaim = FactionRank.General;
    private FactionRank AllowedToWinthdraw = FactionRank.General;
    private FactionRank AllowedToSetHome = FactionRank.General;

    public FactionPermSettings() {

    }

    public FactionPermSettings(String i) {
        String[] ii = i.Split("\\|");
        if (ii.Length != 11) {
            Console.WriteLine("Error importing factions settings! Expected length 10, got " + ii.Length);
            return;
        }
        FactionRank avi = FactionRankMethods.getRankFromString(ii[0]);
        if (avi != null) AllowedToViewInbox = avi;

        FactionRank aaa = FactionRankMethods.getRankFromString(ii[1]);
        if (aaa != null) AllowedToAcceptAlly = avi;

        FactionRank aes = FactionRankMethods.getRankFromString(ii[2]);
        if (aes != null) AllowedToEditSettings = aes;

        FactionRank ap = FactionRankMethods.getRankFromString(ii[3]);
        if (ap != null) AllowedToPromote = ap;

        FactionRank atk = FactionRankMethods.getRankFromString(ii[4]);
        if (atk != null) AllowedToKick = avi;

        FactionRank ati = FactionRankMethods.getRankFromString(ii[5]);
        if (ati != null) AllowedToInvite = ati;

        FactionRank djr = FactionRankMethods.getRankFromString(ii[6]);
        if (djr != null) DefaultJoinRank = djr;

        FactionRank atc = FactionRankMethods.getRankFromString(ii[7]);
        if (atc != null) AllowedToClaim = atc;

        FactionRank atw = FactionRankMethods.getRankFromString(ii[8]);
        if (atw != null) AllowedToWinthdraw = atw;

        FactionRank ash = FactionRankMethods.getRankFromString(ii[9]);
        if (avi != null) AllowedToViewInbox = ash;

        try {
            int iii = int.Parse(ii[10]);
            WeeklyFactionTax = iii;
        } catch (Exception e) {
            Console.WriteLine("Error parseing WeeklyFactionTax! from " + ii[10]);
        }


    }

    public String export() {
        String e = "";
        e += FactionRankMethods.getRankFromString(AllowedToViewInbox) + "|";
        e += FactionRankMethods.getRankFromString(AllowedToAcceptAlly) + "|";
        e += FactionRankMethods.getRankFromString(AllowedToEditSettings) + "|";
        e += FactionRankMethods.getRankFromString(AllowedToPromote) + "|";
        e += FactionRankMethods.getRankFromString(AllowedToKick) + "|";
        e += FactionRankMethods.getRankFromString(AllowedToInvite) + "|";
        e += FactionRankMethods.getRankFromString(DefaultJoinRank) + "|";
        e += FactionRankMethods.getRankFromString(AllowedToClaim) + "|";
        e += FactionRankMethods.getRankFromString(AllowedToWinthdraw) + "|";
        e += FactionRankMethods.getRankFromString(AllowedToSetHome) + "|";
        e += getWeeklyFactionTax() + "|";
        return e;
    }

    public int getWeeklyFactionTax() {
        return WeeklyFactionTax;
    }

    public void setWeeklyFactionTax(int weeklyFactionTax) {
        WeeklyFactionTax = weeklyFactionTax;
    }

    public FactionRank getAllowedToWinthdraw() {
        return AllowedToWinthdraw;
    }

    public void setAllowedToWinthdraw(FactionRank allowedToWinthdraw) {
        AllowedToWinthdraw = allowedToWinthdraw;
    }

    public FactionRank getAllowedToSetHome() {
        return AllowedToSetHome;
    }

    public void setAllowedToSetHome(FactionRank allowedToSetHome) {
        AllowedToSetHome = allowedToSetHome;
    }

    public FactionRank getAllowedToPromote() {
        return AllowedToPromote;
    }

    public void setAllowedToPromote(FactionRank allowedToPromote) {
        AllowedToPromote = allowedToPromote;
    }

//    public FactionRank getAllowedToDemote() {
//        return AllowedToDemote;
//    }
//
//    public void setAllowedToDemote(FactionRank allowedToDemote) {
//        AllowedToDemote = allowedToDemote;
//    }

    public int getMaxFactionChat() {
        return MaxFactionChat;
    }

    public void setMaxFactionChat(int maxFactionChat) {
        MaxFactionChat = maxFactionChat;
    }

    public int getMaxAllyChat() {
        return MaxAllyChat;
    }

    public void setMaxAllyChat(int maxAllyChat) {
        MaxAllyChat = maxAllyChat;
    }

    public FactionRank getAllowedToViewInbox() {
        return AllowedToViewInbox;
    }

    public void setAllowedToViewInbox(FactionRank allowedToViewInbox) {
        AllowedToViewInbox = allowedToViewInbox;
    }

    public FactionRank getAllowedToAcceptAlly() {
        return AllowedToAcceptAlly;
    }

    public void setAllowedToAcceptAlly(FactionRank allowedToAcceptAlly) {
        AllowedToAcceptAlly = allowedToAcceptAlly;
    }

    public FactionRank getAllowedToInvite() {
        return AllowedToInvite;
    }

    public void setAllowedToInvite(FactionRank allowedToInvite) {
        AllowedToInvite = allowedToInvite;
    }

    public FactionRank getDefaultJoinRank() {
        return DefaultJoinRank;
    }

    public void setDefaultJoinRank(FactionRank defaultJoinRank) {
        DefaultJoinRank = defaultJoinRank;
    }

    public FactionRank getAllowedToClaim() {
        return AllowedToClaim;
    }

    public void setAllowedToClaim(FactionRank allowedToClaim) {
        AllowedToClaim = allowedToClaim;
    }

    public FactionRank getAllowedToKick() {
        return AllowedToKick;
    }

    public void setAllowedToKick(FactionRank allowedToKick) {
        AllowedToKick = allowedToKick;
    }

    public FactionRank getAllowedToEditSettings() {
        return AllowedToEditSettings;
    }

    public void setAllowedToEditSettings(FactionRank allowedToEditSettings) {
        this.AllowedToEditSettings = allowedToEditSettings;
    }
    }
}