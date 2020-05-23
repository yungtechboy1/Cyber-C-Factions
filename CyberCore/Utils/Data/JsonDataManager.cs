using System;
using Newtonsoft.Json;

namespace CyberCore.Utils.Data
{
    public abstract class JsonDataManager
    {
        [JsonIgnore]
        public string FileLocation { get; set; }

        [JsonIgnore] public bool Loaded { get; } = false;

        public JsonDataManager(String fileLocation)
        {
            FileLocation = fileLocation;
        }


        public void load()
        {
            
        }
        public void save()
        {
            
        }
    }
}