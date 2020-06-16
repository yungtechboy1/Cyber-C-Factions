using System;
using System.Collections.Generic;
using System.IO;
using CyberCore.Utils.Data;
using Newtonsoft.Json;

namespace CyberCore.Manager.Crate.Data
{
    public class CrateKeyDataManager : JsonDataManager
    {
       
        [JsonIgnore]
        private CyberCoreMain CCM;
        public Dictionary<String, CrateKeyData> KeyData = new Dictionary<string, CrateKeyData>();
        public CrateKeyDataManager(CyberCoreMain ccm) : base("crate-keys")
        {
            CCM = ccm;
        }
        
        
        public new T load<T>() where T : CrateKeyDataManager
        {
            var path = FileLocation + ".json";
            path = Path.Combine( "Plugins", path);
            if (!File.Exists(path)) return null;
            var data = File.ReadAllText(path);
            if (data.Length == 0) return null;
            var d = (CrateKeyDataManager) JsonConvert.DeserializeObject<T>(data);
            if (d.KeyData == null)
            {
                Console.WriteLine("WTF BROOOOO2222 NOOOOO!!!");
                d.KeyData = new Dictionary<string, CrateKeyData>();
                return (T)d;
            }
            return (T)d;
        }
    }
}