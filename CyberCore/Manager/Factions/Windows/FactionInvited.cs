using System;
using Newtonsoft.Json;
using CyberCore.Manager.Forms;
using CyberCore.Utils;
using MiNET;
using MiNET.Blocks;
using MiNET.UI;
using MiNET.Utils;
using OpenAPI.Player;

namespace CyberCore.Manager.Factions.Windows
{
    public class FactionInvited : CyberFormModal
    {
        [JsonIgnore] private String _fn;
        public FactionInvited(String pn, String fn) : base(MainForm.Faction_Invited, "CyberFactions | Faction Invite",
            "Greetings " + pn + "!\n The faction " + fn + " would like to recruit you!",
            "Accept Faction Invite", "Deny Faction Invite")
        {
            _fn = fn;
            ExecuteAction = onRun;
        }


        private void onRun(Player player, ModalForm modal, bool b)
        {
            //TODO THIS DOES NOT CHECK BOOL
            var a = ((CorePlayer)player).EPD;
            var aa = a.FactionInviteData;
            var aac = aa.Count;
            if (aac != 0)
            {
                var last = aa[aac - 1];
                var fn = last.getFaction();
                Faction f = FactionFactory.GetInstance().getFaction(fn);
                if (f == null)
                {
                    //Accept
                    if (f.AcceptInvite(player))
                        player.SendMessage("Welcome to " + f.getDisplayName());
                    else
                        player.SendMessage("Error! Invite timed out!");
                }
                else
                {
                    CyberCoreMain.Log.Error("Faction invited Error! Faction could not be found!");
                    player.SendMessage(ChatColors.Red + " Internal Error E335122!");
                }
            }
        }
    }
}