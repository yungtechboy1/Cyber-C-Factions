using System;
using System.Collections.Generic;
using CyberCore.Manager.Factions;
using log4net;
using MiNET;
using MiNET.UI;
using Newtonsoft.Json;
using OpenAPI.Player;

namespace CyberCore.Manager.Forms
{
    public class CyberFormSimple : SimpleForm
    {
        
        [JsonIgnore] 
        public  MainForm FT;
        [JsonIgnore] 
        public  MainForm AFT;
        [JsonIgnore] 
        public  CyberCoreMain plugin = CyberCoreMain.GetInstance();
        [JsonIgnore] 
        public Faction Fac = null;
        
        [JsonIgnore] 
        private static readonly ILog Log = LogManager.GetLogger(typeof(CyberFormSimple));


        public CyberFormSimple(MainForm ttype, List<Button> bl, String title = "")
        {
            FT = ttype;
            Buttons = bl;
            Content = "";
            Title = title;
        }
        public CyberFormSimple(MainForm ttype, String title = "")
        {
            FT = ttype;
            Buttons = new List<Button>();
            Title = title;
            Content = "";
        }

        public void addButton(String txt,Action<Player, SimpleForm> a = null)
        {
            if(Buttons == null)Buttons = new List<Button>();
            var b = new Button()
            {
                Text = txt, 
               
            };
            if (a != null) b.ExecuteAction = a;
            Buttons.Add(b);
        }
        public void addButton(Button b)
        {
            if(Buttons == null)Buttons = new List<Button>();
            Buttons.Add(b);
        }
        
        public CyberFormSimple(MainForm ttype, MainForm attype, List<Button> bl, String desc = "") 
        {
            FT = ttype;
            AFT = attype;
            Buttons = bl;
            Content = desc;
        }

        [JsonIgnore]
        public int ClickedID = -1;
        public override void FromJson(string json, Player player)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings()
            {
                PreserveReferencesHandling = PreserveReferencesHandling.None,
                Formatting = Formatting.Indented
            };
            int? nullable = JsonConvert.DeserializeObject<int?>(json);
            Log.Debug((object) ("Form JSON\n" + JsonConvert.SerializeObject((object) nullable, settings)));
            if (!nullable.HasValue)
                return;
            ClickedID = nullable.Value;
            this.Buttons[nullable.Value].Execute(player, this);
        }
        
    }
}