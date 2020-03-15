﻿using System;
using CyberCore.Utils;

namespace CyberCore.Manager.Factions
{
    public class Invitation
    {
        private String fac = null;
        private String PlayerName;
        private String InvitedBy;
        private FactionRank Rank = FactionRank.Recruit;
        private int Timeout;

//    public Invitation(Faction f, String playerName, int timeout, FactionRank r) {
//        fac = f.getName();
//        PlayerName = playerName;
//        Timeout = timeout;
//        Rank = r;
//    }
//
//    public Invitation(Faction f, Player sender, int value, FactionRank r) {
//        fac = f.getName();
//        PlayerName = sender.getName();
//        Timeout = value;
//        Rank = r;
//    }
//
//    public Invitation(String f, Player sender, int value, FactionRank r) {
//        fac = f;
//        PlayerName = sender.getName();
//        Timeout = value;
//        Rank = r;
//    }

        public Invitation(String faction, String player, String invitedby, int value, FactionRank r)
        {
            fac = faction;
            InvitedBy = invitedby;
            PlayerName = player;
            Timeout = value;
            Rank = r;
        }

        public String getInvitedBy()
        {
            return InvitedBy;
        }

        public void setInvitedBy(String invitedBy)
        {
            InvitedBy = invitedBy;
        }

        public FactionRank getRank()
        {
            return Rank;
        }

        public void setRank(FactionRank rank)
        {
            Rank = rank;
        }

        public String getFac()
        {
            return fac;
        }

        public void setFac(Faction f)
        {
            fac = f.getName();
        }

        public void setFac(String f)
        {
            fac = f;
        }

        public String getPlayerName()
        {
            return PlayerName;
        }

        public void setPlayerName(String playerName)
        {
            PlayerName = playerName;
        }

        public int getTimeout()
        {
            return Timeout;
        }

        public void setTimeout(int timeout)
        {
            Timeout = timeout;
        }

        // @Override
        public String toString()
        {
            return CyberUtils.toStringCode(new String[]
            {
                getFac(), getPlayerName(), getTimeout() + ""
            });
        }


        public bool isValid()
        {
            return !(CyberUtils.getIntTime() > getTimeout());
        }
    }
}