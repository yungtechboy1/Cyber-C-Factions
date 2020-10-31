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
            setTimeSecs(t);
        }

        /***
         *
         * @param name
         * @param secs
         */
        public CoolDown(String name, long secs, int mins = 0, int hrs = 0)
        {
//        t = tick;
            Key = name;
            setTimeSecs(secs, mins, hrs);
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

        public bool isValid(bool update = false)
        {
            return isValidTime(update);
        }

        public String toString()
        {
            return "CoolDown{" +
                   "Key='" + Key + '\'' +
                   ", Time=" + Time +
                   '}';
        }

        private bool isValidTime(bool update)
        {
            long ct = CyberUtils.getLongTime();

//        CyberCoreMain.Log.Error("Was LOG ||"+ct+" > "+Time);
            if (ct < Time)
                return false;
            if (update) Time = ct;
            return true;
        }
    }
}