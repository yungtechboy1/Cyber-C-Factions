using System;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CyberCore.Manager.ClassFactory
{
    // ReSharper disable once InconsistentNaming
    public class ClassLevelingManagerXPLevel
    {
        public bool ContinusXP = false;
        public int Level = 0;
        public int MaxLevel = 100;
        public int XP = 0;


        public ClassLevelingManagerXPLevel(int XP, int level, int maxLevel)
        {
            this.XP = XP;
            Level = level;
            MaxLevel = maxLevel;
        }

        public ClassLevelingManagerXPLevel(int XP = 0, int level = 0)
        {
            this.XP = XP;
            if (XP == 0 && level != 0) XP = GetXPFromLevel(level);
        }

        /// <summary>
        /// Int is the Level that the Player now is
        /// </summary>
        public event EventHandler<int> LevelUp;

        public event EventHandler<int> XPChange;
        public event EventHandler<int> LevelChange;

        public int getMaxLevel()
        {
            return MaxLevel;
        }

        public void setMaxLevel(int maxLevel)
        {
            MaxLevel = maxLevel;
        }


        protected int GetLevelFromXp(int xp = -1)
        {
            if (xp == -1) xp = XP;
            var tcl = 1;
            while (true)
            {
                if (tcl <= 15)
                {
                    var a = 2 * (tcl - 1) + 7;
                    if (xp >= a)
                    {
                        tcl++;
                        continue;
                    }

                    break;
                }

                if (tcl <= 30)
                {
                    var a = 5 * (tcl - 1) - 38;
                    if (xp >= a)
                    {
                        tcl++;
                        continue;
                    }
                }
                else
                {
                    var a = 9 * (tcl - 1) - 158;
                    if (xp >= a)
                    {
                        tcl++;
                        continue;
                    }
                }

                break;
            }

            return tcl - 1;
        }


        protected int GetXPFromLevel(int level)
        {
            if (level == 0) return 0;
            var xp = 0;
            var cl = level;
            if (cl <= 15)
            {
                var a = 2 * (cl - 1) + 7;
                return a;
            }

            if (cl <= 30)
            {
                var a = 5 * (cl - 1) - 38;
                return a;
            }
            else
            {
                var a = 9 * (cl - 1) - 158;
                return a;
            }
        }

        public int getLevel()
        {
            return Level;
        }

        public int getXP()
        {
            return XP;
        }

        public int getCurrentXP()
        {
            return XP;
        }

        public int getNextLevelXP()
        {
            return GetXPFromLevel(getLevel()+1);
        }

        public int getDisplayXP()
        {
            return XP - getNextLevelXP();
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
            var o = JObject.Parse(cs);
            var a = (int) o["XP"];
            addXP(a);
        }

        public void getXPNeededForNextLevel()
        {
            throw new NotImplementedException();
        }
    }
}