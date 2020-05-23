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

        public String getInputResponse(int k)
        {
            return ((Input) Content[k]).Value;
        }
        
        public void addLabel(String txt)
        {
            addElement(new Label()
            {
                Text = txt
            });
        }
        public void addToggle(String txt, bool def = false)
        {
            addElement(new Toggle()
            {
                Text = txt,
                Value = def
            });
        }
        public void addInput(String title,string placeholder = "",string defvalue = "")
        {
            addElement(new Input()
            {
                Text = title,
                Placeholder = placeholder,
                Value = defvalue
            });
        }
        
        public void addElement(CustomElement e)
        {
            if(Content == null)Content = new List<CustomElement>();
            Content.Add(e);
        }
        
        
    }
}