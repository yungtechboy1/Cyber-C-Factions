using System;
using System.Collections.Generic;
using MiNET.Items;

namespace CyberCore.Manager.Factions.Missions
{
    public class MissionData
    {
        public String name { get; set; } = "N/A";
        public String desc { get; set; } = "No Desc";
        public int id { get; set; } = -1;
        public bool enabled { get; set; } = false;
        public Dictionary<String, int> Break { get; set; } = new Dictionary<String, int>();
        public Dictionary<String, int> Place { get; set; } = new Dictionary<String, int>();
        public List<MissionItemData> ItemReq { get; set; } = new List<MissionItemData>();
        public int Kill { get; set; } = 0;
        public int XPReward { get; set; } = 0;
        public int MoneyReward { get; set; } = 0;
        public List<MissionItemData> ItemReward { get; set; } = new List<MissionItemData>();
        public List<MissionTask> Tasks { get; set; } = new List<MissionTask>();
        public int PointReward { get; set; } = 0;

        public MissionData()
        {
            
        }
        public MissionData(Mission mission)
        {
            name = mission.name;
            desc = mission.desc;
            id = mission.id;
            enabled = mission.enabled;
            Break = mission.Break;
            Place = mission.Place;
            ItemReq = mission.ItemReq;
            Kill = mission.Kill;
            XPReward = mission.XPReward;
            MoneyReward = mission.MoneyReward;
            ItemReward = mission.ItemReward;
            PointReward = mission.PointReward;
        }
    }
}