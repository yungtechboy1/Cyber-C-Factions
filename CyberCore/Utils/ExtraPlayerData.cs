using System;
using System.Collections.Generic;
using CyberCore.Manager.Factions.Data;

namespace CyberCore.Utils
{
    public class ExtraPlayerData
    {
        //TODO Check FactionInviteData if Valid
        private List<FactionInviteData> fid;
        private InternalPlayerSettings ips;

        public List<FactionInviteData> FactionInviteData
        {
            get => fid;
            set => fid = value;
        }

        public InternalPlayerSettings InternalPlayerSettings
        {
            get => ips;
            set => ips = value;
        }

        public ExtraPlayerData(List<FactionInviteData> fid, InternalPlayerSettings ips = null)
        {
            this.fid = fid;
            this.ips = ips;
        }

        public ExtraPlayerData(InternalPlayerSettings ips = null)
        {
            fid = new List<FactionInviteData>();
        }

        public ExtraPlayerData(FactionInviteData fid, InternalPlayerSettings ips = null)
        {
            this.fid = new List<FactionInviteData>() {fid};
            this.ips = ips;
        }

        public int lastupdated = -1;

        public void update()
        {
            lastupdated = CyberUtils.getIntTime();
            var a = new List<FactionInviteData>();
            for (int i = 0; i > FactionInviteData.Count; i++)
            {
                if (!FactionInviteData[i].isValid())
                {
                    a.Add(FactionInviteData[i]);
                }
            }

            foreach (var f in a)
            {
                FactionInviteData.Remove(f);
            }
        }
    }
}