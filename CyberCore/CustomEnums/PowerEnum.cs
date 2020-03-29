using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CyberCore.Utils;

namespace CyberCore.CustomEnums
{
    public enum PowerType
    {
        None,
        Regular,
        Ability,
        Hotbar
    }

    public enum PrimalPowerType
    {
        Stars,
        Ocean,
        Earth,
        Sun,
        Moon
    }

    public enum HotbarStatus
    {
        Success,
        Fail,
        Cooldown
    }

    public enum PowerState
    {
        Idle,
        Fail
    }

    public enum LevelingType
    {
        None,
        XPLevel,
        Stage
    }

    public struct StageEnum
    {
        public static readonly StageEnum NA = new StageEnum(-1);
        public static readonly StageEnum STAGE_1 = new StageEnum(1);
        public static readonly StageEnum STAGE_2 = new StageEnum(2);
        public static readonly StageEnum STAGE_3 = new StageEnum(3);
        public static readonly StageEnum STAGE_4 = new StageEnum(4);
        public static readonly StageEnum STAGE_5 = new StageEnum(5);
        public static readonly StageEnum STAGE_6 = new StageEnum(6);
        public static readonly StageEnum STAGE_7 = new StageEnum(7);
        public static readonly StageEnum STAGE_8 = new StageEnum(8);
        public static readonly StageEnum STAGE_9 = new StageEnum(9);
        public static readonly StageEnum STAGE_10 = new StageEnum(10);

        public int Level;
        public string Name;

        public StageEnum(int lvl)
        {
            Level = lvl;
            var nme = lvl == -1 ? "NA" : "Stage " + lvl;
            Name = nme;
        }

        public static StageEnum getStageFromInt(int i)
        {
            switch (i)
            {
                case 1:
                    return STAGE_1;
                case 2:
                    return STAGE_2;
                case 3:
                    return STAGE_3;
                case 4:
                    return STAGE_4;
                case 5:
                    return STAGE_5;
                case 6:
                    return STAGE_6;
                case 7:
                    return STAGE_7;
                case 8:
                    return STAGE_8;
                case 9:
                    return STAGE_9;
                case 10:
                    return STAGE_10;
                default:
                    return NA;
            }
        }

        public int getValue()
        {
            return Level;
        }

        public string getDisplayName()
        {
            return Name;
        }
    }


    public struct LockedSlot
    {
        public static readonly LockedSlot NA = new LockedSlot();
        public static readonly LockedSlot SLOT_9 = new LockedSlot(8);
        public static readonly LockedSlot SLOT_8 = new LockedSlot(7);
        public static readonly LockedSlot SLOT_7 = new LockedSlot(6);

        public int Slot;

        public LockedSlot(int slt = -1)
        {
            Slot = slt;
        }

        public int getSlot()
        {
            return Slot;
        }
    }

    public struct PowerEnum
    {
        private static readonly int a;
        public static readonly PowerEnum Unknown = new PowerEnum(a++, "Unknown");
        public static readonly PowerEnum MercenaryDoubleTake = new PowerEnum(a++, "MercenaryDoubleTake");
        public static readonly PowerEnum MercenaryRegeneration = new PowerEnum(a++, "MercenaryRegeneration");
        public static readonly PowerEnum HolyKnightHeal = new PowerEnum(a++, "HolyKnightHeal");
        public static readonly PowerEnum TNTAirStrike = new PowerEnum(a++, "TNTAirStrike");
        public static readonly PowerEnum MineLife = new PowerEnum(a++, "MineLife");
        public static readonly PowerEnum MinerOreKnowledge = new PowerEnum(a++, "MinerOreKnowledge");
        public static readonly PowerEnum TNTSpecalist = new PowerEnum(a++, "TNTSpecalist");
        public static readonly PowerEnum MercenaryBlindingStrike = new PowerEnum(a++, "MercenaryBlindingStrike");
        public static readonly PowerEnum MercenaryDisarm = new PowerEnum(a++, "MercenaryDisarm");
        public static readonly PowerEnum DragonJumper = new PowerEnum(a++, "DragonJumper");
        public static readonly PowerEnum KnightSandShield = new PowerEnum(a++, "KnightSandShield");
        public static readonly PowerEnum DarkKnightPosionousStench = new PowerEnum(a++, "DarkKnightPosionousStench");
        public static readonly PowerEnum FireStomp = new PowerEnum(a++, "FireStomp");
        public static readonly PowerEnum FireBox = new PowerEnum(a++, "FireBox");
        public static readonly PowerEnum DoubleTime = new PowerEnum(a++, "DoubleTime");
        public static readonly PowerEnum AntidotePower = new PowerEnum(a++, "AntidotePower");
        public static readonly PowerEnum ThunderStrike = new PowerEnum(a++, "ThunderStrike");
        public static readonly PowerEnum Swappa = new PowerEnum(a++, "Swappa");
        public static readonly PowerEnum UnEven = new PowerEnum(a++, "UnEven");
        public static readonly PowerEnum RagePower = new PowerEnum(a++, "RagePower");

        public static readonly PowerEnum LastStand = new PowerEnum(a++, "LastStand");
        // public static readonly PowerEnum Unknown = new PowerEnum(a++,"Unknown");
        // public static readonly PowerEnum Unknown = new PowerEnum(a++,"Unknown");

        public static Dictionary<string, PowerEnum> GetFieldValues(PowerEnum obj)
        {
            return obj.GetType()
                .GetFields(BindingFlags.Public | BindingFlags.Static)
                .Where(f => f.FieldType == typeof(PowerEnum))
                .ToDictionary(f => f.Name,
                    f => (PowerEnum) f.GetValue(null));
        }

        public static PowerEnum fromint(int i, PowerEnum obj)
        {
            if (GetFieldValues(obj).Count < i)
            {
                var valueCollection = GetFieldValues(obj).Values;
                if (valueCollection != null)
                {
                    var c = 0;
                    foreach (var e in valueCollection)
                        if (c++ == i)
                            return e;
                }
            }

            return Unknown;
        }

        public static PowerEnum fromstr(string i, PowerEnum obj)
        {
            foreach (var e in GetFieldValues(obj))
                if (e.Key.equalsIgnoreCase(i))
                    return e.Value;

            return Unknown;
        }

        public string Name;

        private PowerEnum(int id, string nm)
        {
            ID = id;
            chat_prefix = "&7";
            Name = nm;
        }

        public int ID { get; set; }

        public string chat_prefix { get; set; }

        public int getID()
        {
            return ID;
        }

        public string getChat_prefix()
        {
            return chat_prefix;
        }
    }
}