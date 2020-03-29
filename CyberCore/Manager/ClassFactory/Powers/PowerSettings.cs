namespace CyberCore.Manager.ClassFactory.Powers
{
    public class PowerSettings
    {
        private bool isPassive = false;
        private bool isHotbar = false;
        private bool isAbility = false;
        private bool isEffect = false;

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