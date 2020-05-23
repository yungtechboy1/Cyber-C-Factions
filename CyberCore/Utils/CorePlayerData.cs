using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using CyberCore.Manager.AuctionHouse;
using MiNET;
using MiNET.Blocks;
using MiNET.Items;
using MiNET.Utils;
using Newtonsoft.Json;

namespace CyberCore.Utils
{
    public class CorePlayerData
    {
        public String PlayerName { get; set; }
        public PlayerLocation Location { get; set; }
        public String Level { get; set; }
        public List<Item> Inv { get; set; }
        public int CurrentHealth { get; set; }

        public CorePlayerData()
        {
        }

        public CorePlayerData(CorePlayer p)
        {
            PlayerName = p.getName();
            Location = p.KnownPosition;
            Level = p.Level.LevelName;
            Inv = p.Inventory.Slots;
            CurrentHealth = p.HealthManager.Health;
        }

        public String toJSON()
        {
            return JsonConvert.SerializeObject(this);
        }

        public static void SaveToFile(CorePlayerData d)
        {
            if (!Directory.Exists("Players")) Directory.CreateDirectory("Players");
            // if (!File.Exists("Plugins/CyberTech/config.json")) File.WriteAllText("Plugins/CyberTech/config.json", "[]");

            string path;
            string ConfigFileName = d.PlayerName;
            path = Path.Combine("Players", ConfigFileName + ".conf");
            // FilePath = path;
            Console.WriteLine((object) ("=?????????>>>>>>>>>>>>222>>>>>>>>>>Trying to load config-file " + path));
            if (File.Exists(path))
            {
                File.Delete(path);
                File.WriteAllText(path,d.toJSON());
                var data = File.ReadAllText(path);
                Console.WriteLine((object) ("!@@!@#!@3333###@@@!@@@Loading config-file " + path));
            }
            else
            {
                
                File.WriteAllText(path,d.toJSON());
                Console.WriteLine((object) ("|||||22222222222222222222||||||NO File found at " + path));
                
            }
        }

        public static CorePlayerData LoadFromFile(CorePlayer d)
        {
            if (!Directory.Exists("Players")) Directory.CreateDirectory("Players");
            // if (!File.Exists("Plugins/CyberTech/config.json")) File.WriteAllText("Plugins/CyberTech/config.json", "[]");

            string path;
            string ConfigFileName = d.Username.ToLower();
            path = Path.Combine("Players", ConfigFileName + ".conf");
            // FilePath = path;
            Console.WriteLine((object) ("=?????????>>>>>>>>>>>>>>>>>>>>>>Trying to load config-file " + path));
            if (File.Exists(path))
            {
                var data = File.ReadAllText(path);
                Console.WriteLine((object) ("!@@!@#!@###@@@!@@@Loading config-file " + path));
                return JsonConvert.DeserializeObject<CorePlayerData>(data);
            }
            else
            {
                Console.WriteLine((object) ("|||||||||||NO File found at " + path));
                return null;
            }
        }

        public void LoadToPlayer(CorePlayer p)
        {
            // p.Teleport();
            p.SpawnPosition = Location;
            // var l = CyberCoreMain.GetInstance().getAPI().LevelManager.GetLevel(null, Level);
            // l.AddEntity();
            // p.SpawnLevel();
            // p.Level = l;
        }
    }
}