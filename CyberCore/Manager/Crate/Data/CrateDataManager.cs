using System;
using System.Collections.Generic;
using System.IO;
using CyberCore.Utils.Data;
using Newtonsoft.Json;

namespace CyberCore.Manager.Crate.Data
{
    public class CrateDataManager : JsonDataManager
    {
       
        [JsonIgnore]
        private CyberCoreMain CCM;
        public Dictionary<String, CrateData> Data = new Dictionary<string, CrateData>();
        public CrateDataManager(CyberCoreMain ccm) : base("crate-data")
        {
            CCM = ccm;
        }
        
        
        public new T load<T>() where T : CrateDataManager
        {
            var path = FileLocation + ".json";
            path = Path.Combine( "Plugins", path);
            if (!File.Exists(path)) return null;
            var data = File.ReadAllText(path);
            if (data.Length == 0) return null;
            var d = (CrateDataManager) JsonConvert.DeserializeObject<T>(data);
            if (d.Data == null)
            {
                Console.WriteLine("WTF BROOOOO NOOOOO!!!");
                d.Data = new Dictionary<string, CrateData>();
                return (T)d;
            }
            return (T)d;
        }
    }
}