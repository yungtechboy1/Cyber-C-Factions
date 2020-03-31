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
        
        private static readonly ILog Log = LogManager.GetLogger(typeof(CyberFormSimple));


        public CyberFormSimple(MainForm ttype, List<Button> bl, String desc = "")
        {
            FT = ttype;
            Buttons = bl;
            Content = desc;
        }
        public CyberFormSimple(MainForm ttype, String desc = "")
        {
            FT = ttype;
            Buttons = new List<Button>();
            Content = desc;
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
        
    }
}