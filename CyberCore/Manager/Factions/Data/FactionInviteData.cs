using System;
using System.Collections;
using CyberCore.Utils;
using OpenAPI.Player;

namespace CyberCore.Manager.Factions.Data
{
    public class FactionInviteData
    {
        String PlayerName;
        long TimeStamp = -1;
        String Faction;
        String InvitedBy;
        public FactionRank FacRank;

        public FactionInviteData(String playerName, String faction, long timeStamp = -1, String invitedBy = null,
            FactionRankEnum fr = FactionRankEnum.Recruit)
        {
            PlayerName = playerName;
            TimeStamp = timeStamp;
            Faction = faction;
            InvitedBy = invitedBy;
            FacRank = fr.toFactionRank();
        }

        public String getPlayerName()
        {
            return PlayerName;
        }

        public long getTimeStamp()
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

        public bool isValid(long time)
        {
            return time < TimeStamp;
        }

        public bool isValid()
        {
            return CyberUtils.getLongTime() < TimeStamp;
        }

        public FailReason failReason = FailReason.None;

        public enum FailReason
        {
            None,
            Request_Not_In_Faction,
            Request_Not_In_MySQL
        }
        
        public bool DoubleCheckFaction()
        {
            var f = CyberCoreMain.GetInstance().FM.FFactory.getFaction(Faction);
            if (f != null)
            {
                var a = f.Invites[getPlayerName().ToLower()];
                if (a != null)
                {
                    if (a.DoubleCheckMysql())
                    {
                        return true;
                    }
                    else
                    {
                        a.failReason = FailReason.Request_Not_In_MySQL;
                        a.TimeStamp = 0;
                    }
                }
                else
                    return true;

            }

            return false;
        }
        public bool DoubleCheckMysql()
        {
            var a = CyberCoreMain.GetInstance().SQL
                .executeSelect(
                    $"SELECT * FROM `FactionInvites` WHERE expires LIKE '{TimeStamp}' AND target LIKE '{PlayerName}' AND faction LIKE '{Faction}' AND rank LIKE '{FacRank}'");
            return (a.Count != 0);
        }

        public void DenyInvite()
        {
            var f = FactionFactory.GetInstance().getFaction(Faction);
            if (f != null)
            {
                
                
            }
            else
            {
                CyberCoreMain.GetInstance().SQL.Insert(
                    $"DELETE * from `FactionInvites` where `faction` LIKE '{Faction}' AND `target` LIKE '{getPlayerName()}';");
                TimeStamp = -1;

            }
        }
    }
}