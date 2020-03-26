using System;
using System.Collections.Generic;
using CyberCore.Utils;
using MiNET.UI;

namespace CyberCore
{
    public class CoreSettings
    {
        private bool ShowDamageTags = true;
        private bool ShowAdvancedDamageTags = false;
        private bool HudOff = false;
        private bool HudClassOff = false;
        private bool HudPosOff = false;
        private bool HudFactionOff = false;
        private bool AllowFactionRequestPopUps = false;

        public CoreSettings(Dictionary<String, Object> v)
        {
            HudOff = (bool) v.getOrDefault("HUD_OFF", false);
            HudClassOff = (bool) v.getOrDefault("HUD_CLASS_OFF", false);
            HudPosOff = (bool) v.getOrDefault("HUD_POS_OFF", false);
            HudFactionOff = (bool) v.getOrDefault("HUD_FAC_OFF", false);
        }

        public CoreSettings(bool hudOff, bool hudClassOff, bool hudPosOff, bool hudFactionOff)
        {
            HudOff = hudOff;
            HudClassOff = hudClassOff;
            HudPosOff = hudPosOff;
            HudFactionOff = hudFactionOff;
        }

        public CoreSettings()
        {
            HudOff = false;
            HudClassOff = false;
            HudPosOff = false;
            HudFactionOff = false;
            AllowFactionRequestPopUps = true;
        }

        // public CoreSettings(CompoundTag ct) {
        //     super();
        //     if (ct.contains("HudOff")) HudOff = ct.getBoolean("HudOff");
        //     if (ct.contains("HudClassOff")) HudClassOff = ct.getBoolean("HudClassOff");
        //     if (ct.contains("HudPosOff")) HudPosOff = ct.getBoolean("HudPosOff");
        //     if (ct.contains("HudFactionOff")) HudFactionOff = ct.getBoolean("HudFactionOff");
        // }

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

        public void updateFromWindow(CustomForm frc)
        {
            int i = 0;
            setShowDamageTags(((Toggle) frc.Content[i++]).Value);
            setShowAdvancedDamageTags(((Toggle) frc.Content[i++]).Value);
            setHudOff(((Toggle) frc.Content[i++]).Value);
            setHudClassOff(((Toggle) frc.Content[i++]).Value);
            setHudFactionOff(((Toggle) frc.Content[i++]).Value);
            setHudPosOff(((Toggle) frc.Content[i]).Value);
        }

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

        // public CompoundTag toCompoundTag()
        // {
        //     return new CompoundTag("CoreSettings")
        //     {
        //         {
        //             putBoolean("HudOff", HudOff);
        //             putBoolean("HudClassOff", HudClassOff);
        //             putBoolean("HudPosOff", HudPosOff);
        //             putBoolean("HudFactionOff", HudFactionOff);
        //         }
        //     };
        // }
    }
}