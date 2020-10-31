using CyberCore.Manager.Forms;

namespace CyberCore.Manager.ClassFactory.Forms
{
    public class ClassLeaveClassConfirm : CyberFormModal

    {
        public ClassLeaveClassConfirm(BaseClass c) : base(MainForm.Class_Leave_Confirm, "Leave Class Confirmation", "Leave Class - ", "< Go Back", "")
        {
            Content = c.getConfirmWindowMessage();
        }
    }
}