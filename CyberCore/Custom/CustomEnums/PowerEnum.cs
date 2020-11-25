using System;
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
        AbilityHotbar,
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
        StartRunning,
        Running,
        StopRunning,
        Fail
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


        public override bool Equals(Object obj)
        {
            if (obj is LockedSlot)
            {
                return Slot == ((LockedSlot) obj).Slot;
            }
            else
            {
                return false;
            }
        }

        public int getSlot()
        {
            return Slot;
        }

        public bool Equals(LockedSlot obj)
        {
            return (Slot == obj.Slot);
        }

        public static LockedSlot FromInt(in int to)
        {
            if (to == 7)
            {
                return LockedSlot.SLOT_7;
            }
            else if (to == 8)
            {
                return LockedSlot.SLOT_8;
            }
            else if (to == 9)
            {
                return LockedSlot.SLOT_9;
            }

            return LockedSlot.NA;
        }
    }

    public struct PowerEnum
    {
        private static readonly int a;
        public static readonly PowerEnum Unknown = new PowerEnum(a++, "Unknown");
        public static readonly PowerEnum KnightSmash = new PowerEnum(a++, "KnightSmash");
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


        public override bool Equals(object? obj)
        {
            if (obj != null && obj is PowerEnum)
            {
                return ((PowerEnum) obj).ID == ID;
            }

            return false;
        }

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

        private PowerEnum(int id = -1, string nm = "NO NAME")
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