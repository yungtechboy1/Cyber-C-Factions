namespace CyberCore.Manager.ClassFactory.Powers
{
    public class PowerSettings
    {
        public bool isPassive = false;
        //Hotbar Power
        public bool isHotbar = false;
        //Power that Gets Ticked While Active
        public bool isAbility = false;
        //Power Gets Ticked at Start and Stop
        public bool isEffect = false;

        public PowerSettings()
        {

        }

        public PowerSettings(bool ip = false, bool isHotbar = true, bool isAbility = false, bool isEffect = false)
        {
            isPassive = ip;
            this.isHotbar = isHotbar;
            this.isAbility = isAbility;
            this.isEffect = isEffect;
        }
    }
}