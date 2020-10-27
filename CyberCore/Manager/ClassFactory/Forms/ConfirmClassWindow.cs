using CyberCore.Manager.Forms;
using MiNET;
using MiNET.UI;
using Newtonsoft.Json;

namespace CyberCore.Manager.ClassFactory.Forms
{
    public class ConfirmClassWindow : CyberFormModal

    {
        [JsonIgnore] public BaseClass C;
        public ConfirmClassWindow(BaseClass c) : base(MainForm.Class_Choose_Class_Confirm, "Class Change Confirmation - ", "Accept Class - ", "< Go Back", "")
        {
            C = c;
            Content = c.getConfirmWindowMessage();
            ExecuteAction = delegate(Player player, ModalForm form, bool arg3)
            {
                if (arg3)
                {
                    CorePlayer p = (CorePlayer) player;
                    p.SetPlayerClass(C);
                }
            };
        }
    }
}