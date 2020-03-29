using System;
using System.Collections.Generic;
using CyberCore.Manager.Factions.Data;
using CyberCore.Utils.Data;
using MiNET;

namespace CyberCore.Utils
{
    public class ExtraPlayerData
    {
        //TODO Check FactionInviteData if Valid
        public List<FactionInviteData> FactionInviteData { get; set; } = new List<FactionInviteData>();
        public InternalPlayerSettings InternalPlayerSettings{ get; set; }= new InternalPlayerSettings();
        public PlayerDetailedInfo PlayerDetailedInfo { get; set; } = null;


        public ExtraPlayerData()
        {
            
        }
        public ExtraPlayerData(Player p)
        {
        PlayerDetailedInfo = new PlayerDetailedInfo(p);

        }
        public ExtraPlayerData(Player p ,List<FactionInviteData> fid, InternalPlayerSettings ips = null)
        {
            PlayerDetailedInfo = new PlayerDetailedInfo(p);
            FactionInviteData = fid;
            if(ips != null) InternalPlayerSettings = ips;
        }

        
        // public ExtraPlayerData(FactionInviteData fid, InternalPlayerSettings ips = null)
        // {
        //     FactionInviteData = new List<FactionInviteData>() {fid};
        //     InternalPlayerSettings = ips;
        // }

        public int lastupdated = -1;

        public void update()
        {
            lastupdated = CyberUtils.getLongTime();
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