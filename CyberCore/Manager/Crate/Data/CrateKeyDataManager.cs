using System;
using System.Collections.Generic;
using CyberCore.Utils.Data;
using Newtonsoft.Json;

namespace CyberCore.Manager.Crate.Data
{
    public class CrateKeyDataManager : JsonDataManager
    {
       
        [JsonIgnore]
        private CyberCoreMain CCM;
        public Dictionary<String, CrateKeyData> Data;
        public CrateKeyDataManager(CyberCoreMain ccm, string fileLocation) : base(fileLocation)
        {
            CCM = ccm;
        }
        
    }
}