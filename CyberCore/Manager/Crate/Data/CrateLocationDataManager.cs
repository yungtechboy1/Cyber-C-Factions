using System;
using System.Collections.Generic;
using CyberCore.Utils.Data;
using Newtonsoft.Json;

namespace CyberCore.Manager.Crate.Data
{
    public class CrateLocationDataManager : JsonDataManager
    {
        [JsonIgnore]
        private CyberCoreMain CCM;
        public Dictionary<String, CrateLocationData> Data;
        public CrateLocationDataManager(CyberCoreMain ccm, string fileLocation) : base(fileLocation)
        {
            CCM = ccm;
        }
    }
}