using System.Collections.Generic;
using CyberCore.CustomEnums;

namespace CyberCore.Manager.ClassFactory.Classes
{
    public class Knight : BaseClass
    {
        public Knight(CyberCoreMain main, CorePlayer player, Dictionary<string, object> data = null) : base(main, player, data)
        {

        }

        public Knight(CyberCoreMain main) : base(main)
        {
            
        }

        public override void AddClassPowers()
        {
            ClassPowers.Add(new KnightSmashPower());
        }
        //
        // public void RegisterWithMain()
        // {
        //     CCM.ClassFactory.RegisterClass(this);
        // }

        public override PrimalPowerType getPowerSourceType()
        {
            return PrimalPowerType.Moon;
        }

        public override ClassType getTYPE()
        {
            return ClassType.Class_Knight;
        }

        public override void SetPowers()
        {
            // addPossiblePower();
            AddPossiblePower(new KnightSmashPower());
        }

        public override void initBuffs()
        {
            addBuff(new Buff(BuffType.Health, 2));
            addBuff(new Buff(BuffType.Damage, 1));
            addDeBuff(new DeBuff(BuffType.Damage_Long, 1));
            addBuff(new Buff(BuffType.Swing, 1));
            addDeBuff(new DeBuff(BuffType.Hunger, 1));
            addBuff(new Buff(BuffType.Defense, 2));
            addDeBuff(new DeBuff(BuffType.Speed, 2));
            addDeBuff(new DeBuff(BuffType.HealthRegin, 1));
            addDeBuff(new DeBuff(BuffType.Jump, 2));
            addDeBuff(new DeBuff(BuffType.Dodge, 1));
            addBuff(new Buff(BuffType.HeavyDamage, 2));
        }

        public override object RunPower(PowerEnum powerid, params object[] args)
        {
            throw new System.NotImplementedException();
        }

        public override string getName()
        {
            return "Knight";
        }
    }
}