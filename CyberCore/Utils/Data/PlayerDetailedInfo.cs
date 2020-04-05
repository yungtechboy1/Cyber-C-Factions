using System;
using System.Collections.Generic;
using LibNoise.Combiner;
using MiNET;

namespace CyberCore.Utils.Data
{
    public class PlayerDetailedInfo
    {
        public List<String> IP { get; set; } = new List<string>();
        public List<long> ClientID { get; set; } = new List<long>();
        public List<String> ClientUUID { get; set; } = new List<string>();

        public List<LoginData> LoginData { get; set; } = new List<LoginData>();

        public PlayerDetailedInfo()
        {
        }

        public PlayerDetailedInfo(Player p)
        {
            addDetailsFromPlayer(p);
        }

        public void onLogin(Player p)
        {
            addDetailsFromPlayer(p);
            LoginData.Add(new LoginData(p));
        }

        public void addDetailsFromPlayer(Player p)
        {
            string ip = p.EndPoint.Address.ToString();
            long cid = p.ClientId;
            string uuid = p.ClientUuid.ToString();
            IP.Add(ip);
            ClientID.Add(cid);
            ClientUUID.Add(uuid);
        }
    }

    public class LoginData
    {
        public long Time { get; set; }
        public String IP { get; set; }
        public long ClientID { get; set; }
        public String ClientUUID { get; set; }

        public LoginData()
        {
            
        }
        public LoginData(Player p)
        {
            Time = CyberUtils.getLongTime();
            IP = p.EndPoint.Address.ToString();
            ClientID = p.ClientId;
            ClientUUID = p.ClientUuid.ToString();
        }
    }
}