using CyberCore.Manager.Factions.Windows;
using CyberCore.Manager.Forms;
using MiNET;
using MiNET.UI;
using MiNET.Utils;
using Newtonsoft.Json;

namespace CyberCore
{
    public class LinkConfirmWindow: CyberFormModal
    {
        public LinkConfirmWindow() : base(MainForm.Link_Confirm_Window,"Are you sure you want to link your account?","Yes, I want to Link my account!","No Cancel")
        {
            // var ee = player.getPlayerSettingsData().isAccountLinked();
            // var e = player.getPlayerSettingsData().getLinkedAccount();
            Content = $"Are you sure you want to link your account to the Forums?\n" +
                      $"By doing so, you can manage your account from online.\n" +
                      $"{ChatColors.Yellow} Please note that only 1 Email/Forum Account can be used at a time.\n";
            // if (ee)
            // {
            //     Content += $"{ChatColors.Red}!!WARNING!! Your account is currently linked to `{e}`!\n" +
            //                $"By Continuing you will remove this account link!";
            // }
            ExecuteAction = delegate(Player player1, ModalForm form, bool arg3)
            {
                if (arg3)
                {
                    player1.SendForm(new LinkWindow());
                }
                
            };
        }

    }
}