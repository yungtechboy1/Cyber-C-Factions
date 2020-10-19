using CyberCore.Manager.Forms;
using MiNET;
using MiNET.UI;
using MiNET.Utils;

namespace CyberCore.Manager.Factions.Windows
{
    public class ReportFactionWindow : CyberFormCustom
    {
        public ReportFactionWindow(Faction fac) : base(MainForm.Faction_List_Window)
        {
            Title = $"{ChatColors.Red} Report => " + fac + "'s Faction <=";
            // Content = $"Below is all the players from this faction";
            addInput("Please explain why you are reporting and what rules are being broken.");
            ExecuteAction = delegate(Player player, CustomForm form)
            {
                player.SendMessage($"{ChatColors.Yellow}Error! Sorry this form is not working!\n Please go to UnlimitedMC.net to report any suspicious activity!");
            };
        }
        
        
    }
}