using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using CyberCore.Manager.AuctionHouse;
using CyberCore.Manager.Factions.Windows;
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
        [JsonIgnore] public List<Item> Inv { get; set; }
        [JsonIgnore] public List<CorePlayerItemData> _Inv { get; set; } = new List<CorePlayerItemData>();
        public string _InvJSON { get; set; }
        public int CurrentHealth { get; set; }


        public CorePlayerData()
        {
        }

        public CorePlayerData(CorePlayer p)
        {
            PlayerName = p.Username;
            Location = p.KnownPosition;
            Level = p.Level.LevelId;
            Inv = new List<Item>(p.Inventory.Slots);
            CurrentHealth = p.HealthManager.Health;
        }

        public String toJSON()
        {
            _Inv.Clear();
            foreach (var i in Inv)
            {
                if (i == null || i.Id == 0) continue;
                Console.WriteLine("ADD AN ITEM================================================");
                _Inv.Add(CorePlayerItemData.CreateObject(i));
                Console.WriteLine($"------>>>> HAS NBT: " + (i.hasCompoundTag() ? "Yes" : "no"));
                Console.WriteLine($"------>>>> HAS Custom Name: " + (i.hasCustomName() ? "Yes" : "no"));
            }

            _InvJSON = JsonConvert.SerializeObject(_Inv);
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
                File.WriteAllText(path, d.toJSON());
                Console.WriteLine((object) ("!@@!@#!@3333###@@@!@@@Loading config-file " + path));
            }
            else
            {
                File.WriteAllText(path, d.toJSON());
                Console.WriteLine((object) ("|||||22222222222222222222||||||NO File found at " + path));
            }

            Console.WriteLine($" {d.PlayerName} HAS BEEN SAVED AT {d.Location}");
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
            // p.SpawnPosition = Location;
            // p.KnownPosition = Location;
            p.Inventory.Clear();
            if (_InvJSON == null)
            {
                Console.WriteLine("ERRORR INVJSON WAS NULKL");
                return;
            }

            _Inv = JsonConvert.DeserializeObject<List<CorePlayerItemData>>(_InvJSON);
            if (_Inv == null)
            {
                Console.WriteLine("ERRORR _Inv WAS NULKL");
                return;
            }

            foreach (var d in _Inv)
            {
                var i = d.toItem();
                p.Inventory.AddItem(i, false);
                Console.WriteLine("ADDING ITEM " + i.getName() + " || " + d.Id);
            }

            p.SendPlayerInventory();
            p.Teleport(Location.Safe(CyberCoreMain.GetInstance().getAPI().LevelManager.GetLevel(null, Level)));

            Console.WriteLine($" {PlayerName} SHOULD BE LOADED AT {Location} AND {_Inv.Count}");
            // var l = CyberCoreMain.GetInstance().getAPI().LevelManager.GetLevel(null, Level);
            // l.AddEntity();
            // p.SpawnLevel();
            // p.Level = l;
        }
    }
}