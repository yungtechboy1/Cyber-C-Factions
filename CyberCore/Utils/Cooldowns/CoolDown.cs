using System;

namespace CyberCore.Utils.Cooldowns
{
    public class CoolDown
    {
        public String Key = null;
        protected long Time = -1;

        public CoolDown()
        {
        }

        public CoolDown(int t)
        {
            Time = t;
        }

        /***
         *
         * @param name
         * @param secs
         */
        public CoolDown(String name, int secs, int mins, int hrs)
        {
//        t = tick;
            Key = name;
            setTimeSecs(secs, mins, hrs);
        }

        public CoolDown(String name, int secs, int mins)
        {
//        t = tick;
            Key = name;
            setTimeSecs(secs, mins);
        }

        public CoolDown(String name, long secs)
        {
//        t = tick;
            Key = name;
            setTimeSecs(secs);
        }

        public void Reset(int secs, int mins = 0, int hrs = 0)
        {
            setTimeSecs(secs, mins, hrs);
        }

        public CoolDown(String name)
        {
//        t = tick;
            Key = name;
        }

        public CoolDown AddTo(int s)
        {
            Time = +s;
            return this;
        }

        public long getTime()
        {
            return Time;
        }

        private void setTime(long time)
        {
            Time = time;
        }

        public CoolDown setTimeSecs(int secs, int mins = 0, int hrs = 0)
        {
            setTime(CyberUtils.getLongTime() + secs + (60 * mins) + (60 * 60 * hrs));
            return this;
        }
        public CoolDown setTimeSecs(long secs, int mins = 0, int hrs = 0)
        {
            setTime(CyberUtils.getLongTime() + secs + (60 * mins) + (60 * 60 * hrs));
            return this;
        }

        public String getKey()
        {
            return Key;
        }

        public void setKey(String key)
        {
            Key = key;
        }

        public bool isValid()
        {
            return isValidTime();
        }

        public String toString()
        {
            return "CoolDown{" +
                   "Key='" + Key + '\'' +
                   ", Time=" + Time +
                   '}';
        }

        private bool isValidTime()
        {
            long ct = CyberUtils.getLongTime();
//        CyberCoreMain.Log.Error("Was LOG ||"+ct+" > "+Time);
            return ct < Time;
        }
    }
}