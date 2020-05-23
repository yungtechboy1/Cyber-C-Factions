using CyberCore.Manager.Forms;
using MiNET;
using MiNET.UI;
using MiNET.Utils;

namespace CyberCore.Manager.Factions.Windows
{
    public class FactionConfirmDelete : CyberFormModal
    {
        public FactionConfirmDelete() : base(MainForm.Faction_Delete_Confirm, "CyberFactions | Faction Delete Confirmation", "Confirm and Delete", "Cancel", ChatColors.Red+""+ChatFormatting.Bold+"WARNING!!!!\n Are you sure you want to delete your faction?")
        {
            ExecuteAction = delegate(Player player, ModalForm form, bool arg3)
            {
                var _Fac = ((CorePlayer) player).getFaction();
                if(arg3 && _Fac != null) {
                    player.SendMessage(FactionsMain.NAME+ ChatColors.Green + "Faction Deleted!");
                    _Fac.BroadcastMessage(FactionsMain.NAME + ChatColors.Yellow + "!!~~!!Faction has been Deleted by " + player.Username);
                    CyberCoreMain.GetInstance().FM.FFactory.RemoveFaction(_Fac);
                }
              };
        }
    }
}