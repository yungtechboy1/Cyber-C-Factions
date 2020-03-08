using System;
using System.Collections.Generic;
using CyberCore.Utils;

namespace CyberCore.Manager.Factions
{
    public class FactionLocalCache
    {
        private Faction Fac;

        private List<String> Allies = new List<String>();
        private List<String> Enemies = new List<String>();

        private int LastUpdated = 0;

        public FactionLocalCache(Faction faction) {
            Fac = faction;
        }

        public List<String> getAllies() {
            return Allies;
        }

        public Faction getFac() {
            return Fac;
        }

        public List<String> getEnemies() {
            return Enemies;
        }

        public int getLastUpdated() {
            return LastUpdated;
        }

        public void addAlly(String name){
            Allies.Add(name);
        }

        public void Update(){

        }

        public int getTimeToInt(){
            return CyberUtils.getIntTime();
        }
    }
}