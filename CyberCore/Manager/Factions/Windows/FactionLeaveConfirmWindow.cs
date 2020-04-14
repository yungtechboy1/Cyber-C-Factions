using CyberCore.Manager.Forms;
using CyberCore.Utils;
using MiNET;
using MiNET.UI;
using MiNET.Utils;

namespace CyberCore.Manager.Factions.Windows
{
    public class FactionLeaveConfirmWindow : CyberFormModal
    {
        public FactionLeaveConfirmWindow(Faction f) : base(MainForm.Faction_Leave_Confirm, "Faction Leave Confirmation",
            "Leave", "Stay / Cancel", ChatColors.Red + "Are you sure you want to leave " + f.getDisplayName() + "?")
        {
            ExecuteAction = delegate(Player player, ModalForm form, bool arg3)
            {
                if (arg3)
                {
                    var p = (CorePlayer) player;
                    Faction fac = p.getFaction();
                    fac.removePlayer(p);

                    p.SendMessage(FactionsMain.NAME + ChatColors.Green + "You successfully left faction");
                    fac.TakePower(1);
                    fac.BroadcastMessage(
                        FactionsMain.NAME + ChatColors.Yellow + p.getName() + " has Left the Faction! ");
//            if(Main.CC != null)Main.CC.Setnametag((Player)p);
//            Main.CC.Setnametag((Player) p);
//            Main.sendBossBar((Player) p);
                    CyberCoreMain.GetInstance().FM.FFactory.FacList.Remove(p.getName().ToLower());
                }
            };
        }
    }
}