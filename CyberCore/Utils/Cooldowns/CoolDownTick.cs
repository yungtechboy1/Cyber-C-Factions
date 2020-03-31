using System;

namespace CyberCore.Utils.Cooldowns
{
    public class CoolDownTick
    {
        protected long Tick = -1;
    public String Key = null;

    public CoolDownTick() {
    }

    public long getCurrentTick()
    {
        return CyberUtils.getTick();
    }


    public String toString() {
        String s = "";
        if(isValid()){
            long d = getTick() -getCurrentTick();
            if(d <= 20) return "1 Sec";
            if(d < 20*60){
                //Less Than a Min
                return ((int)(d/20))+" Secs";
            }else if(d < 20*60*60){//72k
                //Less than an hour
                long dd = d/20;
                long dds = dd%60;
                long ddm = (dd/60);
                return ddm+" Mins and "+dds+" Secs";
            }else if(d < 20*60*60*24){
                //Less than a Day
                long dd = d/20;
                long dds = dd%60;
                long ddm = (dd/60);
                long ddh = (ddm/60);
                ddm = dds%60;
                return ddh+"Hours "+ddm+" Mins and "+dds+" Secs";
            }else{
        return (d/20*60*60*24)+" Days Left";
            }
        }else{
            return "[COOLDOWN PAST]";
        }
    }

    
    public CoolDownTick(int t) {
        Tick = t;
    }

    /***
     *
     * @param name
     * @param secs
     */
    public CoolDownTick(String name,int secs, int mins, int hrs) {
//        t = tick;
        Key = name;
        setTimeSecs(secs, mins, hrs);
    }

    public CoolDownTick(String name,int secs, int mins) {
//        t = tick;
        Key = name;
        setTimeSecs(secs, mins);
    }

    public CoolDownTick(String name,int ticks) {
//        t = tick;
        Key = name;
        setTimeTick(ticks);
    }

    public CoolDownTick(String name) {
//        t = tick;
        Key = name;
    }

    public CoolDownTick AddTo(int s) {
        Tick = +s;
        return this;
    }

    public long getTick() {
        return Tick;
    }

    private void setTick(long ticks) {
        Tick = ticks;
    }

    public CoolDownTick setTimeSecs(int hrs, int mins, int secs) {
        return setTimeSecs(secs + (60 * mins) + (60 * 60 * hrs));
    }

    public CoolDownTick setTimeSecs(int mins, int secs) {
        return setTimeSecs(secs + (60 * mins));
    }

    public CoolDownTick setTimeSecs(int secs) {
        setTick(getCurrentTick() + (20*secs));
        return this;
    }

    public CoolDownTick setTimeTick(int ticks) {
        setTick(getCurrentTick() + ticks);
        return this;
    }

    public String getKey() {
        return Key;
    }

    public void setKey(String key) {
        Key = key;
    }

    public bool isValid() {
            return (isValidTick());
    }

    private bool isValidTick() {
        long ct =getCurrentTick();
//        Console.WriteLine(ct+" > "+Time);
        return ct < Tick;
    }
    }
}