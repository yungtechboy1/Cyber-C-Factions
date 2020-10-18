using System;
using CyberCore.Manager.Factions;
using MiNET;
using MiNET.UI;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace CyberCore.Manager.Forms
{
    public class CyberFormModal : ModalForm
    {
        // [JsonIgnore] private static readonly ILog Log = LogManager.GetLogger(typeof(CyberFormModal));
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


        public string ToJSON2()
        {
            return JsonConvert.SerializeObject(this, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                DefaultValueHandling = DefaultValueHandling.Include,
                MissingMemberHandling = MissingMemberHandling.Error,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                Formatting = Formatting.Indented,
                ContractResolver = (IContractResolver) new CamelCasePropertyNamesContractResolver()
            });
        }

        public void Execute(Player player, bool state)
        {
            var executeAction = ExecuteAction;
            executeAction?.Invoke(player, this, state);
        }

        public override void FromJson(string json, Player player)
        {
            var settings = new JsonSerializerSettings
            {
                PreserveReferencesHandling = PreserveReferencesHandling.None,
                Formatting = Formatting.Indented
            };
            var nullable = JsonConvert.DeserializeObject<bool?>(json);
            Console.WriteLine((object) ("Form JSON\n" + JsonConvert.SerializeObject(nullable, settings)));
            if (nullable.HasValue && nullable.Value)
                Execute(player, true);
            else
                Execute(player, false);
            //===========================
        }
    }
}