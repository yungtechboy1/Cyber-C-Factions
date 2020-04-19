using System;
using System.Collections.Generic;
using CyberCore.Manager.Forms;
using MiNET;
using MiNET.UI;

namespace CyberCore.Manager.Factions.Windows
{
    public class FactionSettingsWindow : CyberFormCustom
    {
        public FactionSettingsWindow(Faction f) : base(MainForm.Faction_Settings_Window)
        {
            FactionPermSettings fs = f.getPermSettings();
        List<String> LGOMR = new List<String>() {
            ("Leader"),
            ("General"),
            ("Officer"),
            ("Memeber"),
            ("Recruit")
        };
        List<String> GOMR = new List<String>() {
            ("General"),
            ("Officer"),
            ("Memeber"),
            ("Recruit")
        };
        List<String> LGOM = new List<String>() {
            ("Leader"),
            ("General"),
            ("Officer"),
            ("Memeber")
        };
        List<String> LGO = new List<String>() {
            ("Leader"),
            ("General"),
            ("Officer")
        };
        List<String> MR = new List<String>() {
            ("Member"),
            ("Recruit")
        };
        addElement(new Dropdown()
        {
            Text = "Allowed to View Inbox",
            Options = LGOMR,
            Value = fs.getAllowedToViewInbox().getFormPower()
        });
        addElement(new dd("Allowed to Accept Ally`", LGOM, fs.getAllowedToAcceptAlly().getFormPower()));
        addElement(new dd("Allowed to Edit Settings", LGO, fs.getAllowedToEditSettings().getFormPower()));
        addElement(new dd("Allowed to Kick Players", LGOM, fs.getAllowedToKick().getFormPower()));
        addElement(new dd("Allowed to Promote/Demote", LGOM, fs.getAllowedToPromote().getFormPower()));
        addElement(new dd("Allowed to Invite", LGOMR, fs.getAllowedToInvite().getFormPower()));
        addElement(new dd("Allowed to Claim", LGOM, fs.getAllowedToClaim().getFormPower()));
        addElement(new dd("Allowed to Withdraw", LGOM, fs.getAllowedToWinthdraw().getFormPower()));
        addElement(new dd("Allowed to SetHome", LGOM, fs.getAllowedToSetHome().getFormPower()));
        addElement(new dd("Default Join Rank", MR, fs.getDefaultJoinRank().getFormPower()));
        addElement(new Slider()
        {
            Text = "Weekly Faction Tax",
            Max = 10000,
            Min = 0,
            Step = 100,
            Value = fs.getWeeklyFactionTax()
        });
        ExecuteAction = delegate(Player player, CustomForm form)
        {
            var f = ((CorePlayer) player).getFaction();
            var fm = (FactionSettingsWindow) form;
            var c = fm.Content;
            int avi = ((Dropdown)c[0]).Value;
            int aaa = ((Dropdown)c[1]).Value;
            int aes = ((Dropdown)c[2]).Value;
            int akp = ((Dropdown)c[3]).Value;
            int apd = ((Dropdown)c[4]).Value;
            int ai = ((Dropdown)c[5]).Value;
            int ac = ((Dropdown)c[6]).Value;
            int aw = ((Dropdown)c[7]).Value;
            int ash = ((Dropdown)c[8]).Value;
            int djr = ((Dropdown)c[9]).Value;
            float wft = ((Slider) c[10]).Value;
            FactionPermSettings fs = f.getPermSettings();
            fs.setAllowedToViewInbox(FactionRank.getRankFromForm(avi));
            fs.setAllowedToAcceptAlly(FactionRank.getRankFromForm(aaa));
            fs.setAllowedToEditSettings(FactionRank.getRankFromForm(aes));
            fs.setAllowedToKick(FactionRank.getRankFromForm(akp));
            fs.setAllowedToPromote(FactionRank.getRankFromForm(apd));
            fs.setAllowedToInvite(FactionRank.getRankFromForm(ai));
            fs.setAllowedToClaim(FactionRank.getRankFromForm(ac));
            fs.setAllowedToWinthdraw(FactionRank.getRankFromForm(aw));
            fs.setAllowedToSetHome(FactionRank.getRankFromForm(ash));
            fs.setDefaultJoinRank(FactionRank.getRankFromForm(djr));
            fs.setWeeklyFactionTax((int)wft);
            f.getSettings().upload();

            player.SendMessage("Faction's Settings Have Been Updated!");
        };
        }
    }

    public class dd : Dropdown
    {
        public dd(String title, List<String> l, int v)
        {
            Text = title;
            Options = l;
            Value = v;
        }
    }
}