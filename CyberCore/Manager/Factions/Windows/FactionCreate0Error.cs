using System;
using System.Collections.Generic;
using CyberCore.Manager.Forms;
using CyberCore.Utils;
using MiNET;
using MiNET.UI;

namespace CyberCore.Manager.Factions.Windows
{
    public class FactionCreate0Error: CyberFormCustom

    {
        public FactionCreate0Error(FactionErrorString e) : base(MainForm.Faction_Create_0_Error)
        {
            Title = "CyberCore | Create Faction (Error!)";
            ExecuteAction = delegate(Player player, CustomForm form)
            {

                var _plugin = CyberCoreMain.GetInstance();
                String fn = ((Input)form.Content[1]).Value;
                FactionErrorString? r = null;
                if (string.IsNullOrEmpty(fn))r = FactionErrorString.Error_NameTooShort;
                if (r == null)
                {
                    r = _plugin.FM.FFactory.CheckFactionName(fn);
                }

                if (r != null ) {
                    player.SendForm(new FactionCreate0Error((FactionErrorString) r));
                    return;
                }
                String desc = ((Input)form.Content[2]).Value;
                String motd = ((Input)form.Content[3]).Value;
                bool privacy = ((Toggle)form.Content[5]).Value;

                Faction f = _plugin.FM.FFactory.CreateFaction(fn, player, desc, motd, privacy);
                if (f == null) player.SendMessage(FactionErrorString.Error_SA223.getMsg()+"!!!!!++11<<");

                return;
            };
            addLabel(e.getMsg());
            addInput("Desired Faction Name");
            addInput("Faction Description");
            addInput("Message Of The Day (MOTD)",CyberTexts.Default_Faction_Description);
            addLabel(CyberTexts.Lable_FactionPrivacy);
            addToggle("Faction Privacy Protection");
        }
    }
}