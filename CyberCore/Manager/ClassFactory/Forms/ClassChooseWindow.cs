using CyberCore.Manager.Forms;
using MiNET;
using MiNET.UI;

namespace CyberCore.Manager.ClassFactory.Forms
{
    public class ClassChooseWindow : CyberFormSimple
    {
        public ClassChooseWindow(CorePlayer p) : base(MainForm.Class_Choose_Class)
        {
            // var pc = p.getPlayerClass();
            // if (pc != null)
            // {
            //     addButton("");
            // }

            foreach (var bc in CyberCoreMain.GetInstance().ClassFactory.getRegisteredClasses())
            {
                addButton(bc.getDisplayName(),delegate(Player player, SimpleForm form) { player.SendForm(new ConfirmClassWindow(bc)); });
            }
        }
    }
}