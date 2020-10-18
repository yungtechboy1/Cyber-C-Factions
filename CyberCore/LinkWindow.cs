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
            addLabel("Please note that by associating an email address with your MCPE account. Whoever has access to that email can manage your players data online!");
            addInput("Email","Forum Email Address","@gmail.com");
            ExecuteAction = delegate(Player player, CustomForm form)
            {
                var frc = (CyberFormCustom)form;
                var e = frc.getInputResponse(1);
                ((CorePlayer)player).getPlayerSettingsData().Email = e;
                ((CorePlayer) player).getPlayerSettingsData().save(CyberCoreMain.GetInstance(), (CorePlayer) player);
                player.SendMessage($"{ChatColors.Green}Success! Account Email Set to: {e}");
            };
        }
    }
}