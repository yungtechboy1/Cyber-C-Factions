using System;
using System.Text.RegularExpressions;
using CyberCore.Manager.Forms;
using MiNET;
using MiNET.UI;
using MiNET.Utils;
using Input = MiNET.Plugins.Input;

namespace CyberCore
{
    public class LinkWindow : CyberFormCustom
    {
        public LinkWindow() : base(MainForm.Link_Window)
        {
            Title = "Link Email With Account";
            addLabel(
                "Please note that by associating an email address with your MCPE account. Whoever has access to that email can manage your players data online!");
            addInput("Email", "Forum Email Address", "@gmail.com");
            ExecuteAction = delegate(Player player, CustomForm form)
            {
                var frc = (CyberFormCustom) form;
                var e = frc.getInputResponse(1);
                var r = new Regex("[\\w*].*@[\\w*.]*",RegexOptions.IgnoreCase);
                var m = r.Match(e);
                if (!m.Success)
                {player.SendMessage(
                        $"{ChatColors.Yellow}Error! That Account Email is not in the correct format! Try again! Email {e}");
                    return;
                }
                
                bool a = CyberCoreMain.GetInstance().SQL.CanSetAccountEmail(e);
                if (a)
                {
                    ((CorePlayer) player).getPlayerSettingsData().Email = e;
                    ((CorePlayer) player).getPlayerSettingsData()
                        .save(CyberCoreMain.GetInstance(), (CorePlayer) player);
                    player.SendMessage($"{ChatColors.Green}Success! Account Email Set to: {e}");
                    var ccm = CyberCoreMain.GetInstance();
                    var rr = ccm.ForumWebSQL.executeSelect($"SELECT * FROM `xf_user` WHERE `email` LIKE '{e}'");
                    if (rr != null && rr.Count >= 1)
                    {
                        int uid = (int)(UInt32) rr[0]["user_id"];
                        ccm.ForumWebSQL.Insert(
                            $"UPDATE `xf_user_field_value` SET `field_value` = '{player.Username}' WHERE `xf_user_field_value`.`user_id` = {uid} AND `xf_user_field_value`.`field_id` = CAST(0x6d6370656964 AS BINARY);");
                    }
                }
                else
                {
                    player.SendMessage(
                        $"{ChatColors.Yellow}Error! That Account Email is already in use! Please have that account disassociate that email address and try again! Email {e}");
                }
            };
        }
    }
}