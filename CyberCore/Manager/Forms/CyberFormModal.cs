using System;
using CyberCore.Manager.Factions;
using log4net;
using MiNET;
using MiNET.UI;
using Newtonsoft.Json;

namespace CyberCore.Manager.Forms
{
    public class CyberFormModal : ModalForm
    {
        [JsonIgnore] public MainForm FT;
        [JsonIgnore] public MainForm AFT;
        [JsonIgnore] public CyberCoreMain plugin = CyberCoreMain.GetInstance();
        [JsonIgnore] public Faction Fac = null;

        private static readonly ILog Log = LogManager.GetLogger(typeof(CyberFormModal));

        public CyberFormModal(MainForm ttype, String title, String trueButtonText, String falseButtonText,
            String content = "") 
        {
            FT = ttype;
            Button1 = trueButtonText;
            Button2 = falseButtonText;
            Content = content;
        }

        public CyberFormModal(MainForm ttype, MainForm attype, String title, String trueButtonText,
            String falseButtonText, String content = "")
        {
            FT = ttype;
            AFT = attype;
            Button1 = trueButtonText;
            Button2 = falseButtonText;
            Content = content;
        }

    }
}