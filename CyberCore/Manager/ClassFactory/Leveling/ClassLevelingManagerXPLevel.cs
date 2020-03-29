using System;
using System.IO;
using System.Text;
using CyberCore.CustomEnums;
using CyberCore.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CyberCore.Manager.ClassFactory
{
    public class ClassLevelingManagerXPLevel : ClassLevelingManager
    {
           private int XP = 0;
    private int MaxLevel = 100;

    public ClassLevelingManagerXPLevel(int XP, int maxLevel) {
        this.XP = XP;
        MaxLevel = maxLevel;
    }

    public ClassLevelingManagerXPLevel(int XP = 0) {
        this.XP = XP;
    }

    public int getMaxLevel() {
        return MaxLevel;
    }

    public void setMaxLevel(int maxLevel) {
        MaxLevel = maxLevel;
    }

//    
//    public PowerAbstract.StageEnum getStage() {
//        return PowerAbstract.StageEnum.getStageFromInt();
//    }

    public override StageEnum getStage() {
//        if (getLT() == LevelingType.Stage) return StageEnum.getStageFromInt(Stage);
        return StageEnum.getStageFromInt(1 + (getLevel() / 20));
    }

    protected int XPNeededToLevelUp(int CurrentLevel) {
//        if(CurrentLevel == 0)return 0;
//        int cl = NextLevel - 1;
        int cl = CurrentLevel;
        if (cl <= 15) {
            return 2 * (cl) + 7;
        } else if (cl <= 30) {
            return 5 * (cl) - 38;
        } else {
            return 9 * (cl) - 158;
        }
    }

    public int getLevel() {
        int x = getXP();
        int l = 0;
        while (true) {
            int a = XPNeededToLevelUp(l);
            if (a < x) {
                x -= a;
                l++;
            } else {
                break;
            }
        }
        return l;
    }

    public int getXP() {
        return XP;
    }

    protected int getDisplayXP() {
        return XP - XPNeededToLevelUp(getLevel());
    }

    protected void addXP(int a) {
        XP += Math.Abs(a);
        //TOdo Check to sese if Level Up is inorder!
//        if(XP > XPNeededToLevelUp(getLevel()));
    }

    protected void takeXP(int a) {
        XP -= Math.Abs(a);
    }

    
    public override string exportConfig()
    {
        var sb = new StringBuilder();
        var sw = new StringWriter(sb);

        using (JsonWriter writer = new JsonTextWriter(sw))
        {
            writer.Formatting = Formatting.Indented;

            writer.WriteStartObject();
            writer.WritePropertyName("XP");
            writer.WriteValue(getXP());
            writer.WriteEnd();
            writer.WriteEndObject();
        }

        return sb.ToString();
    }

    public override void importConfig(string cs)
    {
        JObject o = JObject.Parse(@cs);
        var a = (int) o["XP"];
        addXP(a);
    }


    public override LevelingType getType() {
        return LevelingType.XPLevel;
    }


    public override void setMaxStage(StageEnum stage) {

    }
    }
}