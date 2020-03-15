using System;
using CyberCore.Manager.Factions.Data;

namespace CyberCore.Utils
{
    public class ExtraPlayerData
    {
        public FactionInviteData fid;
        public InternalPlayerSettings ips;

        public ExtraPlayerData(FactionInviteData fid = null, InternalPlayerSettings ips = null)
        {
            this.fid = fid;
            this.ips = ips;
        }
    }
}