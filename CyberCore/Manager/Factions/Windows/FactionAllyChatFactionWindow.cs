using System;
using System.Collections.Generic;
using CyberCore.Manager.Forms;
using CyberCore.Utils;
using MiNET;
using MiNET.UI;

namespace CyberCore.Manager.Factions.Windows
{
    public class FactionAllyChatFactionWindow: CyberFormCustom
    {
        private String title = "CyberFactions | Faction Chat Window";
        public FactionAllyChatFactionWindow(List<String> ls) : base(MainForm.Faction_Chat_Faction)
        {
            Content = new List<CustomElement>();
            Content.Add(new Input()
            {
                Placeholder = "Type Message Here",
                Value = "Send Message"
            });
            
            if (ls == null) CyberCoreMain.Log.Error("EEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEE!!!!!!!!!21333111111");
            foreach (var s in ls)
            {
                Content.Add(new Label()
                {
                    Text = s
                });
            }

            ExecuteAction = onrun;
        }

        public void onrun(Player player, CustomForm customForm)
        {
            Fac = player.getFaction();
            if (Fac == null)
            {
                CyberCoreMain.Log.Error("Error With FactionChatFactionWindow! No Faction Found!!!!!");
                return;
            }
            Fac.HandleAllyFactionChatWindow((customForm.Content[0] as Input).Value,player.toCorePlayer());
        }
    }
}