using System;
using System.IO;
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


        
        public T load<T>() where T : JsonDataManager
        {
            var path = FileLocation + "1.json";
            path = Path.Combine( "Plugins", path);
            if (!File.Exists(path)) return null;
            var data = File.ReadAllText(path);
            if (data.Length == 0) return null;
return JsonConvert.DeserializeObject<T>(data);
        }

      
        public void save()
        {
            var path = FileLocation + ".json";
            path = Path.Combine( "Plugins", path);
            File.WriteAllText(path,JsonConvert.SerializeObject(this));

        }
    }
}