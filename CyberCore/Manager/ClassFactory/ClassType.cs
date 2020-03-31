using System;

namespace CyberCore.Manager.ClassFactory
{
    public enum ClassType : int
    {
        Unknown = 0,
        Class_Miner_TNT_Specialist = 1,
        Class_Miner_MineLife = 2,
        Class_Offense_Mercenary = 3,
        Class_Offense_DragonSlayer = 4,
        Class_Magic_Enchanter = 5,
        Class_Rouge_Theif = 6,
        Class_Offense_Knight = 7,
        Class_Offense_Holy_Knight = 8,
        Class_Offense_Dark_Knight = 9,
        Class_Offense_Assassin = 10,
        Class_Offense_Raider = 11,
        Class_Magic_Sorcerer = 12,
        Class_Priest = 13
    }

    public static class ClassTypeExtender
    {
        public static int getInt(this ClassType i)
        {
            return (int) i;
        }

        public static ClassType fromInt(this ClassType a, int i)
        {
            foreach (ClassType aa in typeof(ClassType).GetEnumValues())
            {
                if ((int) aa == i) return aa;
            }

            return ClassType.Unknown;
        }
        public static ClassType fromInt(int i)
        {
            foreach (ClassType aa in typeof(ClassType).GetEnumValues())
            {
                if ((int) aa == i) return aa;
            }

            return ClassType.Unknown;
        }
    }
}