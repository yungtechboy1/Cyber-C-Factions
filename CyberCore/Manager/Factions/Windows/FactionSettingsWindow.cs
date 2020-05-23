using System;
using System.Collections.Generic;
using CyberCore.Manager.Forms;
using MiNET;
using MiNET.UI;
using MiNET.Utils;

namespace CyberCore.Manager.Factions.Windows
{
    public class FactionSettingsWindow : CyberFormCustom
    {
        public class i : Input
        {
            public i(String title, String v = "", String ph = null)
            {
                Text = title;
                Value = v;
                Placeholder = ph;
            }
        }

        public FactionSettingsWindow(Faction f) : base(MainForm.Faction_Settings_Window)
        {
            var fs = f.getSettings();
            addElement(new i("Change MOTD", fs.getMOTD()));
            addElement(new i("Set Description", fs.getDescription()));
            addElement(new MiNET.UI.Label()
            {
                Text = ChatColors.Yellow +
                       "If you enable Faction Privacy then players will require an invite to join your faction!"
            });
            addElement(new Toggle()
            {
                Text = "Change Faction Privacy",
                Value = fs.getPrivacy() == 1
            });

            ExecuteAction = delegate(Player player, CustomForm form)
            {
                var f = ((CorePlayer) player).getFaction();
                var fm = (FactionPermSettingsWindow) form;
                var c = fm.Content;
                string motd = ((Input) c[0]).Value;
                string desc = ((Input) c[1]).Value;
                bool privacy = ((Toggle) c[2]).Value;
                var fs = f.getSettings();
                fs.setMOTD(motd);
                fs.setDescription(desc);
                fs.setPrivacy(privacy);
                fs.upload();

                player.SendMessage("Faction's Settings Have Been Updated!");
            };
        }
    }
}