using System;
using System.Collections.Generic;

namespace CyberCore.Utils
{
    public class InternalPlayerSettings
    {
        private bool ShowDamageTags = true;
        private bool ShowAdvancedDamageTags = false;
        private bool HudOff = false;
        private bool HudClassOff = false;
        private bool HudPosOff = false;
        private bool HudFactionOff = false;
        private bool AllowFactionRequestPopUps = false;

        public InternalPlayerSettings(Dictionary<String, Object> v)
        {
            if (v.ContainsKey("HUD_OFF")) HudOff = (bool) v["HUD_OFF"];
            if (v.ContainsKey("HUD_CLASS_OFF")) HudClassOff = (bool) v["HUD_CLASS_OFF"];
            if (v.ContainsKey("HUD_POS_OFF")) HudPosOff = (bool) v["HUD_POS_OFF"];
            if (v.ContainsKey("HUD_FAC_OFF")) HudFactionOff = (bool) v["HUD_FAC_OFF"];
        }

        public InternalPlayerSettings(bool hudOff = false, bool hudClassOff = false, bool hudPosOff = false,
            bool hudFactionOff = false)
        {
            HudOff = hudOff;
            HudClassOff = hudClassOff;
            HudPosOff = hudPosOff;
            HudFactionOff = hudFactionOff;
        }

        public InternalPlayerSettings()
        {
        }


        public bool getShowDamageTags()
        {
            return ShowDamageTags;
        }

        public void setShowDamageTags(bool showDamageTags)
        {
            ShowDamageTags = showDamageTags;
        }

        public bool getShowAdvancedDamageTags()
        {
            return ShowAdvancedDamageTags;
        }

        public void setShowAdvancedDamageTags(bool showAdvancedDamageTags)
        {
            ShowAdvancedDamageTags = showAdvancedDamageTags;
        }

        public bool isAllowFactionRequestPopUps()
        {
            return AllowFactionRequestPopUps;
        }

        public void setAllowFactionRequestPopUps(bool allowFactionRequestPopUps)
        {
            AllowFactionRequestPopUps = allowFactionRequestPopUps;
        }

        //TDOD
        // public void updateFromWindow(FormResponseCustom frc) {
        //     setShowDamageTags(frc.getToggleResponse(0));
        //     setShowAdvancedDamageTags(frc.getToggleResponse(1));
        //     setHudOff(!frc.getToggleResponse(2));
        //     setHudClassOff(!frc.getToggleResponse(3));
        //     setHudFactionOff(!frc.getToggleResponse(4));
        //     setHudPosOff(!frc.getToggleResponse(5));
        // }

        public bool isHudOff()
        {
            return HudOff;
        }

        public void setHudOff(bool hudOff)
        {
            HudOff = hudOff;
        }

        public void setHudOff()
        {
            HudOff = true;
            HudClassOff = true;
            HudPosOff = true;
            HudFactionOff = true;
        }

        public bool isHudClassOff()
        {
            return HudClassOff;
        }

        public void setHudClassOff(bool hudClassOff)
        {
            HudClassOff = hudClassOff;
        }

        public void setHudClassOff()
        {
            setHudClassOff(!isHudClassOff());
        }

        public bool isHudPosOff()
        {
            return HudPosOff;
        }

        public void setHudPosOff(bool hudPosOff)
        {
            HudPosOff = hudPosOff;
        }

        public void setHudPosOff()
        {
            setHudPosOff(!isHudPosOff());
        }

        public bool isHudFactionOff()
        {
            return HudFactionOff;
        }

        public void setHudFactionOff(bool hudFactionOff)
        {
            HudFactionOff = hudFactionOff;
        }

        public void setHudFactionOff()
        {
            setHudFactionOff(!isHudFactionOff());
        }

        public void TurnOnHUD()
        {
            HudOff = false;
            HudClassOff = false;
            HudPosOff = false;
            HudFactionOff = false;
        }
    }
}

// public CompoundTag toCompoundTag()
// {
//     return new CompoundTag("CoreSettings")
//     {
//         {
//             putbool("HudOff", HudOff);
//             putbool("HudClassOff", HudClassOff);
//             putbool("HudPosOff", HudPosOff);
//             putbool("HudFactionOff", HudFactionOff);
//         }
//     };
// }