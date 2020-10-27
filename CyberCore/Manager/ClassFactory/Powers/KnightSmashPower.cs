using CyberCore.CustomEnums;
using CyberCore.Manager.ClassFactory.Powers;

namespace CyberCore.Manager.ClassFactory.Classes
{
    public class KnightSmashPower : PowerAbstract,PowerHotBarInt
    {
        public KnightSmashPower() : base(new AdvancedPowerEnum(PowerEnum.KnightSmash))
        {
            DefaultPower = true;
        }

        // public KnightSmashPower(BaseClass b, AdvancedPowerEnum ape, PowerSettings ps) : base(b, ape, ps)
        // {
        // }
        //
        // public KnightSmashPower(BaseClass b, AdvancedPowerEnum ape) : base(b, ape)
        // {
        // }
        //
        // public KnightSmashPower(BaseClass b) : base(b)
        // {
        // }
        //
        // public KnightSmashPower(BaseClass b, StageEnum stageEnum) : base(b, stageEnum)
        // {
        //     
        // }

        public override void onAbilityActivate()
        {
            // getPlayer().Velocity.Y;
            // getPlayer().SendMovePlayer();
            var a = getPlayer().Velocity;
            a.Y+=20f;
            getPlayer().Velocity = a;

        }

        public override PowerEnum getType()
        {
            return PowerEnum.KnightSmash;
        }

        public override string getName()
        {
            return "Smash";
        }

        public bool canUpdateHotBar(int tick)
        {
            return true;
        }
    }
}