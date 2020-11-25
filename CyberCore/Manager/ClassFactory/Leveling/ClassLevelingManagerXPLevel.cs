using System;
using System.IO;
using System.Text;
using CyberCore.CustomEnums;
using CyberCore.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CyberCore.Manager.ClassFactory
{
    // ReSharper disable once InconsistentNaming
    public class ClassLevelingManagerXPLevel
    {
        public int MaxLevel = 100;
        public int XP = -1;
        public int NextLevelXP = -1;
        public bool ContinusXP = false;


        public ClassLevelingManagerXPLevel(int XP, int maxLevel)
        {
            this.XP = XP;
            MaxLevel = maxLevel;
        }

        public ClassLevelingManagerXPLevel(int XP = 0)
        {
            this.XP = XP;
        }

        public int getMaxLevel()
        {
            return MaxLevel;
        }

        public void setMaxLevel(int maxLevel)
        {
            MaxLevel = maxLevel;
        }


        protected int XPNeededToLevelUp(int CurrentLevel)
        {
//        if(CurrentLevel == 0)return 0;
//        int cl = NextLevel - 1;
            int cl = CurrentLevel;
            if (cl <= 15)
            {
                return 2 * (cl) + 7;
            }
            else if (cl <= 30)
            {
                return 5 * (cl) - 38;
            }
            else
            {
                return 9 * (cl) - 158;
            }
        }

        public int GetLevel()
        {
            int x = getXP();
            int l = 0;
            while (true)
            {
                int a = XPNeededToLevelUp(l);
                if (a < x)
                {
                    x -= a;
                    l++;
                }
                else
                {
                    break;
                }
            }

            return l;
        }

        public int getXP()
        {
            return XP;
        }

        protected int getDisplayXP()
        {
            return XP - XPNeededToLevelUp(GetLevel());
        }

        protected void addXP(int a)
        {
            XP += Math.Abs(a);
            //TOdo Check to sese if Level Up is inorder!
//        if(XP > XPNeededToLevelUp(getLevel()));
        }

        protected void takeXP(int a)
        {
            XP -= Math.Abs(a);
        }


        public string exportConfig()
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

        public void importConfig(string cs)
        {
            JObject o = JObject.Parse(@cs);
            var a = (int) o["XP"];
            addXP(a);
        }
    }
}