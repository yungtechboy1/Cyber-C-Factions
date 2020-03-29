using CyberCore.CustomEnums;

namespace CyberCore.Manager.ClassFactory.Powers
{
    public abstract class StagePowerAbstract : PowerAbstract
    {
        public StagePowerAbstract(AdvancedPowerEnum ape) : base(ape){
        }
        public StagePowerAbstract(BaseClass b) : base(b, StageEnum.STAGE_1){
        }

        public StagePowerAbstract(BaseClass b, AdvancedPowerEnum ape) : base(b,ape) {
            
        }

//    public StagePowerAbstract(BaseClass b, int psc, double cost, PowerSettings ps) {
//        super(b,  new ClassLevelingManagerStage(),ps, psc, cost);
//    }

        public abstract StageEnum getMaxStage();

        public void setMaxStage(StageEnum s){
            if(getStageLevelManager() == null){
                CyberCoreMain.Log.Error("SPA>>> ERRORORORO RWTF HOW IS THIS NULLLLL!!!");
                return;
            }
            getStageLevelManager().setMaxStage(s);
        }
    }
}