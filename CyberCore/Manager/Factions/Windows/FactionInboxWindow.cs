using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
using CyberCore.Manager.Factions.Data;
using CyberCore.Manager.Forms;
using CyberCore.Utils;
using MiNET;
using MiNET.UI;
using MiNET.Utils;
using OpenAPI.Utils;

namespace CyberCore.Manager.Factions.Windows
{
    public class FactionInboxWindow : CyberFormSimple
    {
        public FactionInboxWindow(CorePlayer p) : base(MainForm.Faction_Inbox_Main,
            $"{p.getFaction().getDisplayName()}'s {ChatFormatting.Reset} Inbox")
        {
            Faction f = p.getFaction();
            List<Faction.AllyRequest> a = f.getAllyRequests();
            List<FactionMessage> m = f.getMessages();
            List<InboxMessage> master = new List<InboxMessage>();
            master.AddRange(formatToInboxMessage(a));
            master.AddRange(formatToInboxMessage(m));
            foreach (InboxMessage msg in master)
            {
                string title = "";
                title += msg.ReadData == 0 ? $"{ChatColors.Green}[|] - " : "";
                title += $"{ChatColors.Yellow}F: {msg.Sender} {ChatColors.Aqua}{msg.Date}";
                addButton(title, delegate(Player player, SimpleForm form)
                {
                    player.SendForm(new InboxMessageWindow(msg));
                });
            }
        }


        public List<InboxMessage> formatToInboxMessage(List<FactionMessage> ml)
        {
            List<InboxMessage> msgs = new List<InboxMessage>();
            foreach (FactionMessage a in ml)
            {
                DateTime att = a.Date;
                TimeSpan att2 = a.Date - DateTime.Now;
                String mm = $"==============\n" +
                            $"{ChatColors.Yellow}|||=====================================|||\n" +
                            $"{ChatColors.Yellow}|||==FROM: {a.Sender} {ChatColors.Yellow}==|||\n" +
                            $"{ChatColors.Aqua}|||=====================================|||\n" +
                            $"{ChatColors.Green}|||=====================================|||" +
                            new MessageFormater(a.Message, 80, $"{ChatColors.Aqua}|||{ChatColors.Green}",
                                $"{ChatColors.Aqua}|||").run() +
                            $"{ChatColors.Green}|||===|||\n" +
                            $"{ChatColors.Green}|||=====================================|||\n" +
                            $"{ChatColors.Green}|||EXPIRES: {att} ============|||\n" +
                            $"{ChatColors.Green}|||EXPIRES: {att2.Days}{att2.Hours}{att2.Minutes}{att2.Seconds}{att2.Milliseconds}{att2.Ticks} ============|||\n" +
                            $"{ChatColors.Green}|||EXPIRES: {att2.Days} ============|||\n" +
                            $"{ChatColors.Green}|||EXPIRES: {att2.Hours} ============|||\n" +
                            $"{ChatColors.Green}|||EXPIRES: {att2.Minutes} ============|||\n" +
                            $"{ChatColors.Green}|||EXPIRES: {att2.Seconds} ============|||\n" +
                            $"{ChatColors.Green}|||EXPIRES: {att2.Milliseconds} ============|||\n" +
                            $"{ChatColors.Green}|||EXPIRES: {att2.Ticks} ============|||\n" +
                            $"{ChatColors.Green}|||EXPIRES: {att2.Days} ============|||\n" +
                            $"{ChatColors.Green}|||=====================================|||\n" +
                            $"{ChatColors.Green}|||=====================================|||\n" +
                            $"{ChatColors.Yellow}|||==|==|||\n" +
                            $"\n" +
                            $"\n" +
                            $"";
                var m = new InboxMessage(a.Sender, a.Date.ToString(), mm, a.Read);
                a.Read = 1;
                msgs.Add(m);
            }

            return msgs;
        }

        public List<InboxMessage> formatToInboxMessage(List<Faction.AllyRequest> ml)
        {
            List<InboxMessage> msgs = new List<InboxMessage>();
            foreach (var a in ml)
            {
                DateTime att = a.Timeout.toDateTimeFromLongTime();
                TimeSpan att2 = a.Timeout.toDateTimeFromLongTime() - DateTime.Now;
                var m = new InboxMessage(a.F.getDisplayName(), a.Timeout.toDateTimeFromLongTime().ToString(),
                    $"==============\n" +
                    $"{ChatColors.Aqua}=============================" +
                    $"{ChatColors.Aqua}=====Faction Ally Message====" +
                    $"{ChatColors.Aqua}=============================" +
                    $"{ChatColors.Yellow}|||=====================================|||\n" +
                    $"{ChatColors.Yellow}|||==The Faction {a.F.getDisplayName()} {ChatColors.Yellow} HAS REQUESTED AN ALLY==|||\n" +
                    $"{ChatColors.Aqua}|||==RELATIONSHIP STATUS WITH YOUR FACTION==|||\n" +
                    $"{ChatColors.Aqua}|||==SHOULD YOU CHOOSE TO ACCEPT THE ABOVE==|||\n" +
                    $"{ChatColors.Yellow}|||==FACTION WOULD BE ABLE TO ACCESS FACTION HOMES==|||\n" +
                    $"{ChatColors.Yellow}|||==(THIS CAN ALSO BE TURNED OFF IN THE FACTION SETTINGS)==|||\n" +
                    $"{ChatColors.Aqua}|||=====================================|||\n" +
                    $"{ChatColors.Green}|||=====================================|||\n" +
                    $"{ChatColors.Green}|||EXPIRES: {att} ============|||\n" +
                    $"{ChatColors.Green}|||EXPIRES: {att2.Days}{att2.Hours}{att2.Minutes}{att2.Seconds}{att2.Milliseconds}{att2.Ticks} ============|||\n" +
                    $"{ChatColors.Green}|||EXPIRES: {att2.Days} ============|||\n" +
                    $"{ChatColors.Green}|||EXPIRES: {att2.Hours} ============|||\n" +
                    $"{ChatColors.Green}|||EXPIRES: {att2.Minutes} ============|||\n" +
                    $"{ChatColors.Green}|||EXPIRES: {att2.Seconds} ============|||\n" +
                    $"{ChatColors.Green}|||EXPIRES: {att2.Milliseconds} ============|||\n" +
                    $"{ChatColors.Green}|||EXPIRES: {att2.Ticks} ============|||\n" +
                    $"{ChatColors.Green}|||EXPIRES: {att2.Days} ============|||\n" +
                    $"{ChatColors.Green}|||=====================================|||\n" +
                    $"{ChatColors.Green}|||=====================================|||\n" +
                    $"{ChatColors.Yellow}|||==|==|||\n" +
                    $"\n" +
                    $"\n" +
                    $"", 0);
                msgs.Add(m);
            }

            return msgs;
        }
    }

    public class InboxMessage
    {
        public String Sender;
        public String Date;
        public String Message;
        public int ReadData;

        public InboxMessage(string sender, string date, string message, int readData)
        {
            Sender = sender;
            Date = date;
            Message = message;
            ReadData = readData;
        }
    }
}