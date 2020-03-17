using System;
using CyberCore.Manager.Factions.Data;

namespace CyberCore.Utils
{
    public class ExtraPlayerData
    {
        private FactionInviteData fid;
        private InternalPlayerSettings ips;

        public FactionInviteData FactionInviteData
        {
            get => fid;
            set => fid = value;
        }

        public InternalPlayerSettings InternalPlayerSettings
        {
            get => ips;
            set => ips = value;
        }

        public ExtraPlayerData(FactionInviteData fid = null, InternalPlayerSettings ips = null)
        {
            this.fid = fid;
            this.ips = ips;
        }
    }
}