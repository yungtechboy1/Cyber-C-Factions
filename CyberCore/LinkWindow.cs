using CyberCore.Manager.Forms;
using MiNET;
using MiNET.UI;
using Input = MiNET.Plugins.Input;

namespace CyberCore
{
    public class LinkWindow : CyberFormCustom
    {
        public LinkWindow() : base(MainForm.Link_Window)
        {
            addInput("Email","Forum Email Address","@gmail.com");
            ExecuteAction = delegate(Player player, CustomForm form)
            {
                var frc = (CyberFormCustom)form;
                ((CorePlayer)player).getPlayerSettingsData().Email = frc.getInputResponse(0);
                ((CorePlayer) player).getPlayerSettingsData().save(CyberCoreMain.GetInstance(), (CorePlayer) player);
            };
        }
    }
}