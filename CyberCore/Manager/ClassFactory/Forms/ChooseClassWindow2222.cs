using CyberCore.Manager.Forms;
using MiNET;
using MiNET.UI;

namespace CyberCore.Manager.ClassFactory.Forms
{
    public class ChooseClassWindow2222 : CyberFormModal
    {
        public ChooseClassWindow2222() : base(MainForm.Main_Class_Settings_Window, "UnlimitedMC Factions", "Continue >",
            "< Go Back", "To view FAQ(Frequently Ask Questions) regarding your class use /class faq or /class help")
        {
            ExecuteAction = delegate(Player player, ModalForm form, bool arg3)
            {
                if (arg3) player.SendForm(new ClassChooseWindow((CorePlayer) player));
            };
        }
    }
}