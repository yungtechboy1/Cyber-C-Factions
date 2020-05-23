using System.Text;
using CyberCore.Manager.Forms;
using MiNET;
using MiNET.UI;
using MiNET.Utils;

namespace CyberCore.Manager.Factions.Windows
{
    public class FactionInfoWindow : CyberFormSimple
    {
        public FactionInfoWindow(Faction ffaction, int listpage = -1, string listfac = null) : base(MainForm.Faction_Info_Other,
            "CyberFactions | " + ffaction.getDisplayName() + ChatFormatting.Reset + " Faction info")
        {
            var sb = new StringBuilder();
            sb.Append(formatString("Faction name", ffaction.getDisplayName())).Append("\n");
            sb.Append(formatString("Leader", ffaction.GetLeader())).Append("\n");
            sb.Append(formatString("# of Players", ffaction.getPlayerCount() + "")).Append("\n");
            sb.Append(formatString("Max # of Players", ffaction.GetMaxPlayers())).Append("\n");
            sb.Append(formatString("MOTD", ffaction.getSettings().getMOTD())).Append("\n");
            sb.Append(formatString("Desc", ffaction.getSettings().getDescription())).Append("\n");
            sb.Append(formatString("PowerAbstract", ffaction.GetPower())).Append("\n");
            sb.Append(formatString("Land Owned", ffaction.GetPlots().Count)).Append("\n");
            sb.Append(formatString("Money", ffaction.GetMoney())).Append("\n");
            sb.Append(formatString("XP", ffaction.getSettings().getXP())).Append("\n");
            sb.Append(formatString("Level", ffaction.getSettings().getLevel())).Append("\n");
            Content = sb.ToString();
            
            

            addButton("View All Players", CommingSoon);
            addButton("View Allies", CommingSoon);
            addButton("View Enemies", CommingSoon);
            addButton("Open Inbox", CommingSoon);
            addButton("Report Faction", CommingSoon);
            if(listpage != -1)addButton("Go Back To List", delegate(Player player, SimpleForm form) {
            player.SendForm(new FactionListWindow(listfac,listpage));
              });

        }
        
        private void CommingSoon(Player arg1, SimpleForm arg2)
        {
            arg1.SendMessage(ChatColors.Yellow + ChatFormatting.Bold + "Comming Soon!");
        }

        private string formatString(string keyname, object val)
        {
            return ChatColors.Yellow + keyname + ": " + ChatColors.Aqua + val;
        }
        
    }
}