namespace CyberCore.Manager.ClassFactory.Powers
{
    public class PowerSettings
    {
        public bool isPassive = false;
        public bool isHotbar = false;
        public bool isAbility = false;
        public bool isEffect = false;

        public PowerSettings()
        {

        }

        public PowerSettings(bool ip, bool isHotbar, bool isAbility, bool isEffect)
        {
            isPassive = ip;
            this.isHotbar = isHotbar;
            this.isAbility = isAbility;
            this.isEffect = isEffect;
        }
    }
}