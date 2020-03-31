﻿using System;

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

        public CoolDown setTimeSecs(int hrs, int mins, int secs)
        {
            return setTimeSecs(secs + (60 * mins) + (60 * 60 * hrs));
        }

        public CoolDown setTimeSecs(int mins, int secs)
        {
            return setTimeSecs(secs + (60 * mins));
        }

        public CoolDown setTimeSecs(long secs)
        {
            setTime(CyberUtils.getLongTime() + secs);
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