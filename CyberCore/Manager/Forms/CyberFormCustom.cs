using System;
using System.Collections.Generic;
using CyberCore.Manager.Factions;
using log4net;
using MiNET.UI;
using Newtonsoft.Json;

namespace CyberCore.Manager.Forms
{
    public class CyberFormCustom : CustomForm
    {
        [JsonIgnore] public MainForm FT;
        [JsonIgnore] public MainForm AFT;
        [JsonIgnore] public CyberCoreMain plugin = CyberCoreMain.GetInstance();
        [JsonIgnore] public Faction Fac = null;

        private static readonly ILog Log = LogManager.GetLogger(typeof(CyberFormCustom));
        
        public CyberFormCustom(MainForm ttype) 
        {
            FT = ttype;
        }
        public CyberFormCustom(MainForm ttype,  List<CustomElement> elements) 
        {
            FT = ttype;
            Content = elements;
        }

        public void addElement(CustomElement e)
        {
            if(Content == null)Content = new List<CustomElement>();
            Content.Add(e);
        }
        
        
    }
}