using CyberCore.CustomEnums;
using CyberCore.Utils;

namespace CyberCore.Manager.ClassFactory
{
    public abstract class ClassLevelingManager
    {
        public abstract StageEnum getStage();
        public abstract string exportConfig();

        public abstract void importConfig(string cs);

        public abstract LevelingType getType();

        public abstract void setMaxStage(StageEnum stage);
    }
}