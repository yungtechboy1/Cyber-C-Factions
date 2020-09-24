using System;
using System.Collections.Generic;
using System.Text;
using CyberCore.Manager.Forms;
using CyberCore.Utils;
using MiNET;
using MiNET.Blocks;
using MiNET.UI;
using MiNET.Utils;
using Button = MiNET.UI.Button;

namespace CyberCore.Manager.Factions.Windows
{
    public class FactionInfoWindow : CyberFormSimple
    {
        public FactionInfoWindow(CorePlayer cp, Faction ffaction, int listpage = -1, string listfac = null) : base(
            MainForm.Faction_Info_Other,
            "CyberFactions | " + ffaction.getDisplayName() + ChatFormatting.Reset + " Faction info")
        {
            bool a = false;
            if (cp.getFaction() != null && cp.getFaction().getName().equalsIgnoreCase(ffaction.getName()))
            {
                a = true;
                Title = ChatColors.LightPurple + "[*]" + Title;
            }

            var sb = new StringBuilder();
            if (a) sb.Append(ChatColors.LightPurple + "====CURRENT FACTION====").Append("\n");
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


            addButton("View All Players",
                delegate(Player player, SimpleForm form) { player.SendForm(new FIWAllPlayers(ffaction)); });
            addButton("View Allies",
                delegate(Player player, SimpleForm form) { player.SendForm(new FIWViewRelation(ffaction)); });
            addButton("View Enemies",
                delegate(Player player, SimpleForm form) { player.SendForm(new FIWViewRelation(ffaction,false)); });
            addButton("Open Inbox",
                delegate(Player player, SimpleForm form) { player.SendForm(new FactionInboxWindow((CorePlayer)player)); });
            addButton("Report Faction",
                delegate(Player player, SimpleForm form) { player.SendForm(new ReportFactionWindow(ffaction)); });
            if (listpage != -1)
                addButton("Go Back To List",
                    delegate(Player player, SimpleForm form)
                    {
                        player.SendForm(new FactionListWindow((CorePlayer) player, listfac, listpage));
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

    public class FIWAllPlayers : CyberFormSimple
    {
        public FIWAllPlayers(Faction fac) : base(MainForm.Faction_List_Window,
            fac + "'s Faction Players")
        {
            Content = $"Below is all the players from this faction";
            var f = fac;
            addButton("View All Players",
                delegate(Player player, SimpleForm form)
                {
                    player.SendForm(new FactionInfoWindow((CorePlayer) player, fac));
                });
            foreach (var a in f.PlayerRanks)
            {
                addButton($"{a.Value.getChatColor()}- [{a.Value.GetChatPrefix()}] {a.Key}");
            }
        }
    }

    public class FIWViewRelation : CyberFormSimple
    {
        public FIWViewRelation(Faction fac, bool allies = true) : base(MainForm.Faction_List_Window,
            fac + "'s Faction Players")
        {
            Content = $"Below is all the players from this faction";
            var f = fac;
            foreach (var a in allies?f.GetAllies():f.GetEnemies())
            {
                var ff = fac.Main.FFactory.getFaction(a);
                if (ff == null) continue;
                addButton($" {ChatColors.DarkAqua}[{a}]",
                    delegate(Player player, SimpleForm form)
                    {
                        player.SendForm(new FactionInfoWindow((CorePlayer) player, fac));
                    });
            }

            addButton("View All Players",
                delegate(Player player, SimpleForm form)
                {
                    player.SendForm(new FactionInfoWindow((CorePlayer) player, fac));
                });
            foreach (var a in f.PlayerRanks)
            {
                addButton($"{a.Value.getChatColor()}- [{a.Value.GetChatPrefix()}] {a.Key}");
            }
        }
    }
}