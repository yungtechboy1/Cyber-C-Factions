using System.Diagnostics.Contracts;
using CyberCore.CustomEnums;
using CyberCore.Utils;

namespace CyberCore.Manager.ClassFactory
{
    public abstract class ClassLevelingManager
    {
        public int XP = -1;
        public int NextLevelXP = -1;
        public bool ContinusXP = false;
        
        public abstract StageEnum getStage();
        public abstract string exportConfig();

        public abstract void importConfig(string cs);

        public abstract LevelingType getType();

        public abstract void setMaxStage(StageEnum stage);

        public abstract int getXP();
    }
}