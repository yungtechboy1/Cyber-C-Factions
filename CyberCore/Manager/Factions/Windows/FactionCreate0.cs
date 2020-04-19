using System;
using System.Collections.Generic;
using CyberCore.Manager.Forms;
using CyberCore.Utils;
using MiNET;
using MiNET.UI;

namespace CyberCore.Manager.Factions.Windows
{
    public class FactionCreate0 : CyberFormCustom

    {
        public FactionCreate0() : base(MainForm.Faction_Create_0)
        {
            ExecuteAction = delegate(Player player, CustomForm form)
            {

                var _plugin = CyberCoreMain.GetInstance();
              String fn = ((Input)form.Content[0]).Value;
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
                String desc = ((Input)form.Content[1]).Value;
                String motd = ((Input)form.Content[2]).Value;
                bool privacy = ((Toggle)form.Content[4]).Value;

                Faction f = _plugin.FM.FFactory.CreateFaction(fn, player, desc, motd, privacy);
                if (f == null) player.SendMessage(FactionErrorString.Error_SA223.getMsg()+"!!!!!++11<<");
                ((CorePlayer)player).RefreshCommands();

                return;
            };
            Title = "CyberCore | Create Faction";
            addInput("Desired Faction Name");
            addInput("Faction Description");
            addInput("Message Of The Day (MOTD)",CyberTexts.Default_Faction_Description);
            addLabel(CyberTexts.Lable_FactionPrivacy);
            addToggle("Faction Privacy Protection");
        }

    }
}