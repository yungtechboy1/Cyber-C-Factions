using System.Xml.Schema;
using CyberCore.Manager.Forms;
using CyberCore.Utils;
using MiNET;
using MiNET.UI;
using MiNET.Utils;

namespace CyberCore.Manager.Factions.Windows
{
    public class FactionChangeLeaderWindow : CyberFormModal
    {
        public FactionChangeLeaderWindow(CorePlayer currentleader,CorePlayer newleader) : base(MainForm.Faction_Leader_Change, "Transfer Faction Leadership To Another Player", "Transfer / Continue", "Cancel / Go Back")
        {
            Content = $"======| WARNING |======\n" +
                      "======| WARNING |======\n" +
                      "THIS WILL TRANSFER\n" +
                      "FACTION OWNERSHIP\n" +
                      $"===OF {currentleader.getFaction().getDisplayName()}\n" +
                      $"TO {newleader.getName()}\n" +
                      $"=| THIS CAN NOT BE UNDONE |=\n+" +
                      $"=| THIS CAN NOT BE UNDONE |=\n+" +
                      $"=| THIS CAN NOT BE UNDONE |=\n+";
            ExecuteAction += delegate(Player player, ModalForm form, bool arg3) {
                if (arg3)
                {
                    var fac = currentleader.getFaction();
                    fac.updatePlayerRankinDB(currentleader,FactionRank.General);
                    fac.updatePlayerRankinDB(newleader,FactionRank.Leader);
                    fac.reloadPlayerRanks(true);
                    
                    fac.BroadcastMessage(FactionsMain.NAME+ChatColors.Yellow+$"{newleader.getName()} Is your New Leader!");
                    player.SendMessage(FactionsMain.NAME+ChatColors.Yellow+"You are no longer leader!");
                }
              };
        }

        public FactionChangeLeaderWindow(MainForm ttype, MainForm attype, string title, string trueButtonText, string falseButtonText, string content = "") : base(ttype, attype, title, trueButtonText, falseButtonText, content)
        {
        }
    }
}