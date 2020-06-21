using System;
using System.Collections.Generic;
using System.IO;
using CyberCore.Utils.Data;
using Newtonsoft.Json;

namespace CyberCore.Manager.Crate.Data
{
    public class CrateLocationDataManager : JsonDataManager
    {
        [JsonIgnore]
        private CyberCoreMain CCM;
        public Dictionary<String, CrateLocationData> LocData = new Dictionary<string, CrateLocationData>();
        public CrateLocationDataManager(CyberCoreMain ccm) : base("crate-locations")
        {
            CCM = ccm;
        }
        
        public T load<T>() where T : CrateLocationDataManager
        {
            var path = FileLocation + ".json";
            path = Path.Combine( "Plugins", path);
            if (!File.Exists(path)) return null;
            var data = File.ReadAllText(path);
            if (data.Length == 0) return null;
            var d = (CrateLocationDataManager) JsonConvert.DeserializeObject<T>(data);
            if (d.LocData == null)
            {
                Console.WriteLine("WTF BROOOOO2222 NOOOOO!!!");
                d.LocData = new Dictionary<string, CrateLocationData>();
                return (T)d;
            }
            return (T)d;
        }
    }
}