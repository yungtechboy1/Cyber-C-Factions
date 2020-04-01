using System;
using System.Collections.Generic;
using CyberCore.Utils.Data;
using Newtonsoft.Json;

namespace CyberCore.Manager.Crate.Data
{
    public class CrateDataManager : JsonDataManager
    {
       
        [JsonIgnore]
        private CyberCoreMain CCM;
        public Dictionary<String, CrateData> Data;
        public CrateDataManager(CyberCoreMain ccm, string fileLocation) : base(fileLocation)
        {
            CCM = ccm;
        }
        
    }
}