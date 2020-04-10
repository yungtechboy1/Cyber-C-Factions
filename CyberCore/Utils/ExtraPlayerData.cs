using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using CyberCore.Manager.Factions;
using CyberCore.Manager.Factions.Data;
using CyberCore.Utils.Data;
using MiNET;
using MiNET.Utils;
using Newtonsoft.Json;

namespace CyberCore.Utils
{
    public class ExtraPlayerData
    {
        // public ExtraPlayerData(FactionInviteData fid, InternalPlayerSettings ips = null)
        // {
        //     FactionInviteData = new List<FactionInviteData>() {fid};
        //     InternalPlayerSettings = ips;
        // }
        private CorePlayer Player;
        public long lastupdated = -1;


        public ExtraPlayerData()
        {
        }

        public ExtraPlayerData(CorePlayer p, Dictionary<String, Object> load = null)
        {
            Player = p;
            if (load != null)
            {
                if (load.ContainsKey("Player"))
                {
                    String pp = (string) load["Player"];
                    if (!pp.equalsIgnoreCase(p.getName()))
                    {
                        CyberCoreMain.Log.Error($"ERROOR RETURNED DATA FOR PLAYER {Player.getName()} IS FOR {pp}");
                        return;
                    }
                }
                else
                {
                    CyberCoreMain.Log.Error("EPD ERROR! PlAYER FIELD IS NULL!!!!");
                    return;
                }

                var a = JsonConvert.DeserializeObject<List<FactionInviteData>>((string) load["FactionInviteData"]);
                var aa = JsonConvert.DeserializeObject<InternalPlayerSettings>((string) load["InternalPlayerSettings"]);
                var aaa = JsonConvert.DeserializeObject<PlayerDetailedInfo>((string) load["PlayerDetailedInfo"]);
                if (a != null) FactionInviteData = a;
                if (aa != null) InternalPlayerSettings = aa;
                if (aaa != null) PlayerDetailedInfo = aaa;
                else PlayerDetailedInfo = new PlayerDetailedInfo(p);
            }
            else
                PlayerDetailedInfo = new PlayerDetailedInfo(p);
        }

        public Dictionary<String, Object> toSQL()
        {
            return new Dictionary<string, object>()
            {
                {"Player", Player.getName().ToLower()},
                {"FactionInviteData", JsonConvert.SerializeObject(FactionInviteData)},
                {"InternalPlayerSettings", JsonConvert.SerializeObject(InternalPlayerSettings)},
                {"PlayerDetailedInfo", JsonConvert.SerializeObject(PlayerDetailedInfo)},
            };
        }

        public ExtraPlayerData(Player p, List<FactionInviteData> fid, InternalPlayerSettings ips = null)
        {
            PlayerDetailedInfo = new PlayerDetailedInfo(p);
            FactionInviteData = fid;
            if (ips != null) InternalPlayerSettings = ips;
        }

        //TODO Check FactionInviteData if Valid
        public List<FactionInviteData> FactionInviteData { get; set; } = new List<FactionInviteData>();
        public InternalPlayerSettings InternalPlayerSettings { get; set; } = new InternalPlayerSettings();
        public PlayerDetailedInfo PlayerDetailedInfo { get; set; } = null;

        public void update()
        {
            lastupdated = CyberUtils.getLongTime();
            var a = new List<FactionInviteData>();
            for (var i = 0; i > FactionInviteData.Count; i++)
                if (!FactionInviteData[i].isValid())
                    a.Add(FactionInviteData[i]);

            foreach (var f in a)
            {
                FactionInviteData.Remove(f);
                String fn = f.getFaction();
                Faction F = CyberCoreMain.GetInstance().FM.FFactory.getFaction(fn);
                var i = f.getInvitedBy();
                var ii = CyberUtils.getPlayer(i);
                if (F != null)
                {
                    F.DelInvite(Player.Username.ToLower());
                    F.BroadcastMessage($"{ChatColors.Yellow} INVITE TIMEOUT : {Player.DisplayName} has failed to accept your faction's invite in enough time!");
                }
                Player.SendMessage($"{ChatColors.Yellow} FACTION INVITE TIMEOUT : You took too long to accept the invite from {fn}");
                if(ii != null)ii.SendMessage($"{ChatColors.Yellow} INVITE TIMEOUT : {Player.DisplayName} has failed to accept your invite in enough time!");
            }
        }

        public void upload()
        {
            var main = CyberCoreMain.GetInstance();
            var a =main.SQL.executeSelect($"SELECT * FROM `EPD` WHERE EPD.player = '{Player.Username}'");
            var z = toSQL();
            if (a.Count != 0)
            {
                main.SQL.Insert($"DELETE FROM `EPD` WHERE player = '{Player.Username}'");
            }
            // {"Player", Player.getName().ToLower()},
                // {"FactionInviteData", JsonConvert.SerializeObject(FactionInviteData)},
                // {"InternalPlayerSettings", JsonConvert.SerializeObject(InternalPlayerSettings)},
                // {"PlayerDetailedInfo", JsonConvert.SerializeObject(PlayerDetailedInfo)},
                main.SQL.Insert(
                    $"INSERT INTO `EPD` VALUES ('{Player.getName().ToLower()}','{z["FactionInviteData"]}','{z["InternalPlayerSettings"]}','{z["PlayerDetailedInfo"]}')");
            }
        }
    }
