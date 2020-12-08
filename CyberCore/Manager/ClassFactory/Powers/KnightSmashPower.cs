using System.Numerics;
using System.Threading;
using CyberCore.CustomEnums;
using CyberCore.Manager.ClassFactory.Powers;
using CyberCore.Utils;
using MiNET.Entities;

namespace CyberCore.Manager.ClassFactory.Classes
{
    public class KnightSmashPower : PowerHotBarInt
    {
        public KnightSmashPower() : base(new AdvancedPowerEnum(PowerEnum.KnightSmash),new PowerSettings(false,true,true,true))
        {
            isDefaultPower = true;
            MainPowerType = PowerType.Ability;
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

        private long starttick = 0;

        protected override ClassLevelingManagerXPLevel getDefaultClassLevelingManager()
        {
            return new ClassLevelingManagerXPLevel();
        }

        public override void onAbilityActivate()
        {
            // getPlayer().Velocity.Y;
            // getPlayer().SendMovePlayer();
            Entity.Direction d = getPlayer().GetDirectionEmum();
            Vector3 a = getPlayer().Velocity;
            var aa = d.toVector3()*1.3f;
            getPlayer().Velocity = aa;
            starttick = CyberUtils.getTick();
            long max = starttick + (20 * 60); //60 secs max
            while (true)
            {
                if (WhileAbilityRunning())
                {
                    PlayerClass.P.SendMessage("BOW YOU LANDED!!!!!!");
                    return;
                }
                if (max >= CyberUtils.getTick()) return;
                Thread.Sleep(250);
            }
        }

        private bool WhileAbilityRunning()
        {
            if (PlayerClass.P.IsOnGround) return true;
            var pos = PlayerClass.P.KnownPosition + new Vector3(0,-1,0);
            var b = PlayerClass.P.Level.GetBlock(pos);
            if (b.Id == 0) return true;
            return false;
        }

        public override PowerEnum getType()
        {
            return PowerEnum.KnightSmash;
        }
        
        public override string getName()
        {
            return "Smash";
        }

    }
}