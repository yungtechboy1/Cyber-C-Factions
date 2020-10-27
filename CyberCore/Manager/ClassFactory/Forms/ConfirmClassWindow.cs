using CyberCore.Manager.Forms;
using MiNET;
using MiNET.UI;

namespace CyberCore.Manager.ClassFactory.Forms
{
    public class ConfirmClassWindow : CyberFormModal

    {
        public ConfirmClassWindow(BaseClass c) : base(MainForm.Class_Choose_Class_Confirm, "Class Change Confirmation - ", "Accept Class - ", "< Go Back", "")
        {
            Content = c.getConfirmWindowMessage();
            ExecuteAction = delegate(Player player, ModalForm form, bool arg3)
            {
                if (arg3)
                {
                    CorePlayer p = (CorePlayer) player;
                    p.SetPlayerClass(c);
                }
            };
        }
    }
}