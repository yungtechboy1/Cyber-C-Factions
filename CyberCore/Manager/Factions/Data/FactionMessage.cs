using System;
using System.Collections.Generic;
using Jose;

namespace CyberCore.Manager.Factions.Data
{
    public class FactionMessage
    {
        public int ID { get; set; } = -1;
        public String Sender { get; set; }
        public String Target { get; set; }
        public String Message { get; set; }
        public DateTime Date { get; set; }
        public int Read { get; set; }
        // public int ID { get; set; }
        public FactionMessage(String sender, String target, String message)
        {
            Sender = sender;
            Target = target;
            Message = message;
            Date = DateTime.Now;
            Read = 0;
        }

        public void uploadMessage()
        {
            CyberCoreMain.GetInstance().SQL.Insert($"INSERT INTO `Inbox` VALUES (null,'{Sender}','{Target}','{Date.ToString()}',{Read},'{Message}')");
        }
        public FactionMessage(Dictionary<String, Object> a)
        {
            ID = (int) a["ID"];
            Read = (int) a["Read"];
            Sender = (string) a["Sender"];
            Target = (string) a["Target"];
            Message = (string) a["Message"];
            Date = DateTime.Parse((string) a["Date"]);
            
        }
    }
}