using System;
using System.Collections;
using CyberCore.Utils;
using OpenAPI.Player;

namespace CyberCore.Manager.Factions.Data
{
    public class FactionInviteData
    {
        String PlayerName;
        int TimeStamp = -1;
        String Faction;
        String InvitedBy;
        FactionRank FacRank;

        public FactionInviteData(String playerName, String faction, int timeStamp = -1, String invitedBy = null,
            FactionRank fr = FactionRank.Recruit)
        {
            PlayerName = playerName;
            TimeStamp = timeStamp;
            Faction = faction;
            InvitedBy = invitedBy;
            FacRank = fr;
        }

        public String getPlayerName()
        {
            return PlayerName;
        }

        public int getTimeStamp()
        {
            return TimeStamp;
        }

        public String getFaction()
        {
            return Faction;
        }

        public String getInvitedBy()
        {
            return InvitedBy;
        }

        public bool isPlayer(OpenPlayer cp)
        {
            return cp.Username.Equals(PlayerName, StringComparison.CurrentCultureIgnoreCase);
        }

        public bool isValid(int time)
        {
            return time < TimeStamp;
        }

        public bool isValid()
        {
            return CyberUtils.getIntTime() < TimeStamp;
        }
    }
}