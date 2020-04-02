using System;
using System.Collections.Generic;

namespace CyberCore.Manager.Factions.Missions
{
    public class ActiveMissionData : MissionData
    {

        public Dictionary<String, int> BreakCount  { get; set; }= new Dictionary<String, int>();
        public Dictionary<String, int> PlaceCount { get; set; } = new Dictionary<String, int>();
        public int KillCount  { get; set; }= 0;

        public String Faction = "";
        
        public ActiveMissionData(ActiveMission mission) :base(mission)
        {
            if(mission.faction != null)Faction = mission.faction.getName();
            BreakCount = mission.BreakCount;
            PlaceCount = mission.PlaceCount;
            KillCount = mission.KillCount;
        }
        
    }
}