using CyberCore.Manager.Forms;
using Newtonsoft.Json;

namespace CyberCore.Manager.Factions.Windows
{
    public class InboxMessageWindow : CyberFormSimple
    {
        [JsonIgnore]
        private InboxMessage Message;

        public InboxMessageWindow(InboxMessage msg) : base(MainForm.Inbox_Message_View)
        {
            Message = msg;
            Content = msg.Message;
            addButton("Reply > ");
            addButton("Delete X ");
        }
    }
}