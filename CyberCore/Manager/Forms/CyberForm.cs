using System;
using CyberCore.Manager.Factions;
using log4net;
using MiNET;
using MiNET.Blocks;
using MiNET.UI;
using Newtonsoft.Json;

namespace CyberCore.Manager.Forms
{
    public class CyberForm : Form
    {
        [JsonIgnore] 
        public  MainForm FT;
        [JsonIgnore] 
        public  MainForm AFT;
        [JsonIgnore] 
        public  CyberCoreMain plugin = CyberCoreMain.GetInstance();
        [JsonIgnore] 
        public Faction Fac = null;
        
        private static readonly ILog Log = LogManager.GetLogger(typeof(CyberForm));

        public CyberForm(MainForm ft = MainForm.NULL, MainForm aft = MainForm.NULL)
        {
            FT = ft;
            AFT = aft;
        }

        public override void FromJson(string json, Player player)
        {
            var jsonSerializerSettings = new JsonSerializerSettings
            {
                PreserveReferencesHandling = PreserveReferencesHandling.None,
                Formatting = Formatting.Indented,
            };

            var parsedResult = JsonConvert.DeserializeObject<bool?>(json);
            Log.Debug($"Form JSON\n{JsonConvert.SerializeObject(parsedResult, jsonSerializerSettings)}");

            if (!parsedResult.HasValue) return;

            if (parsedResult.Value)
            {
                Execute(player);
            }
        }

        [JsonIgnore] public Action<Player, CyberForm> ExecuteAction { get; set; }

        public void Execute(Player player)
        {
            ExecuteAction?.Invoke(player, this);
        }

    }
}