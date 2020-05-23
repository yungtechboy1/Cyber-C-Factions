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
        [JsonIgnore] private static readonly ILog Log = LogManager.GetLogger(typeof(CyberFormModal));
        [JsonIgnore] public MainForm AFT;
        [JsonIgnore] public Faction Fac = null;
        [JsonIgnore] public MainForm FT;
        [JsonIgnore] public CyberCoreMain plugin = CyberCoreMain.GetInstance();

        public CyberFormModal(MainForm ttype, string title, string trueButtonText, string falseButtonText,
            string content = "")
        {
            FT = ttype;
            Title = title;
            Button1 = trueButtonText;
            Button2 = falseButtonText;
            Content = content;
        }

        public CyberFormModal(MainForm ttype, MainForm attype, string title, string trueButtonText,
            string falseButtonText, string content = "")
        {
            FT = ttype;
            AFT = attype;
            Button1 = trueButtonText;
            Button2 = falseButtonText;
            Content = content;
        }

        public new Action<Player, ModalForm, bool> ExecuteAction { get; set; }

        public void Execute(Player player, bool state)
        {
            var executeAction = ExecuteAction;
            executeAction?.Invoke(player, this, state);
        }

        public override void FromJson(string json, Player player)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings()
            {
                PreserveReferencesHandling = PreserveReferencesHandling.None,
                Formatting = Formatting.Indented
            };
            bool? nullable = JsonConvert.DeserializeObject<bool?>(json);
            Log.Debug((object) ("Form JSON\n" + JsonConvert.SerializeObject((object) nullable, settings)));
            if (nullable.HasValue && nullable.Value)
                Execute(player, true);
            else
                Execute(player, false);
            //===========================
        }
    }
}