using System;

namespace CyberCore.Utils
{
    public class CacheChecker
    {
        private String Name;
        private long LastUpdated = -1;
        private int UpdateEverySecs = 60*15;//15 Mins

        public CacheChecker(String name) {
            Name = name;
        }

        public CacheChecker(String name,int updateEverySecs) {
            Name = name;
            UpdateEverySecs = updateEverySecs;
        }
        public CacheChecker(String name, int lastUpdated, int updateEverySecs) {
            Name = name;
            LastUpdated = lastUpdated;
            UpdateEverySecs = updateEverySecs;
        }

        public bool needsUpdate(){
            return getLastUpdated()+getUpdateEverySecs() < getTime();
        }

        public void updateLastUpdated() {
            LastUpdated = getTime();
        }

        public void setUpdateEverySecs(int updateEverySecs) {
            UpdateEverySecs = updateEverySecs;
        }

        public String getName() {
            return Name;
        }

        public long getLastUpdated() {
            return LastUpdated;
        }

        public int getUpdateEverySecs() {
            return UpdateEverySecs;
        }

        public long getTime(){
            return CyberUtils.getLongTime();
        }

        public void invalidate() {
            LastUpdated = -1;
        }
    }
}