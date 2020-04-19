using System.Collections.Generic;
using CyberCore.Manager.Forms;
using MiNET;
using MiNET.UI;

namespace CyberCore.Manager.Factions.Windows
{
    public class FactionJoinConfirm : CyberFormModal
    {

        public FactionJoinConfirm(int oldpage, Faction f) : base(MainForm.Faction_Join_Confirm, "Join Faction","Join Faction","Go Back")
        {
            Content = $"======|{f.getDisplayName()}|======\n" +
                      $"Description: {f.getSettings().getDescription()}\n" +
                      $"MOTD: {f.getSettings().getMOTD()}\n" +
                      $"Players Currently In Faction: {f.getPlayerCount()}\n" +
                      $"Joining Rank: {f.getPermSettings().DefaultJoinRank.toEnum()}\n" +
                      $"";
            ExecuteAction = delegate(Player player, ModalForm form, bool arg3) {
                if (arg3)
                {
                    f.addPlayer(player,f.getPermSettings().DefaultJoinRank.toEnum());
                }
                else
                {
                    if(oldpage != -1)player.SendForm(new FactionJoinListWindow(oldpage));
                }
            };
        }

    }
}