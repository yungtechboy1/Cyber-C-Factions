﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Reflection;
using log4net;
using MiNET;
using MiNET.Worlds;
using OpenAPI.Plugins;

namespace CyberCore.Utils
{
    public class CustomConfig
    {
        private readonly ILog Log = LogManager.GetLogger(typeof(CustomConfig));
        public string ConfigFileName = "server.conf";

        private IReadOnlyDictionary<string, string> KeyValues { get; set; }

        public CustomConfig(OpenPlugin plugin,string fileName =  null)
        {
            if (fileName != null) ConfigFileName = fileName;
            try
            {
                string userName = Environment.UserName;
                string data = string.Empty;
                string directoryName = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
                Log.Info("DIRECTORY >> "+directoryName);
                if (directoryName != null)
                {
                    string path = Path.Combine(directoryName, "server." + userName + ".conf");
                    Log.Info((object) ("Trying to load config-file " + path));
                    if (File.Exists(path))
                    {
                        data = File.ReadAllText(path);
                    }
                    else
                    {
                        path = Path.Combine(directoryName, ConfigFileName);
                        Log.Info((object) ("Trying to load config-file " + path));
                        if (File.Exists(path))
                            data = File.ReadAllText(path);
                    }
            
                    Log.Info((object) ("Loading config-file " + path));
                }

                LoadValues(data);
            }
            catch (Exception ex)
            {
                Log.Warn((object) "Error configuring parser", ex);
            }
        }

        private void LoadValues(string data)
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            string str1 = data;
            string[] separator = new string[3]
            {
                "\r\n",
                "\n",
                Environment.NewLine
            };
            foreach (string str2 in str1.Split(separator, StringSplitOptions.RemoveEmptyEntries))
            {
                string str3 = str2.Trim();
                if (!str3.StartsWith("#") && str3.Contains("="))
                {
                    string[] strArray = str3.Split('=', 2, StringSplitOptions.None);
                    string lower = strArray[0].ToLower();
                    string str4 = strArray[1];
                    Log.Debug((object) (lower + "=" + str4));
                    if (!dictionary.ContainsKey(lower))
                        dictionary.Add(lower, str4);
                }
            }

            KeyValues =
                (IReadOnlyDictionary<string, string>) new ReadOnlyDictionary<string, string>(
                    (IDictionary<string, string>) dictionary);
        }

        public ServerRole GetProperty(string property, ServerRole defaultValue)
        {
            string str = ReadString(property);
            if (str == null)
                return defaultValue;
            switch (str.ToLower())
            {
                case "1":
                case "node":
                    return ServerRole.Node;
                case "0":
                case "proxy":
                    return ServerRole.Proxy;
                case "2":
                case "full":
                    return ServerRole.Full;
                default:
                    return defaultValue;
            }
        }

        public GameMode GetProperty(string property, GameMode defaultValue)
        {
            string str = ReadString(property);
            if (str == null)
                return defaultValue;
            switch (str.ToLower())
            {
                case "0":
                case "survival":
                    return GameMode.Survival;
                case "1":
                case "creative":
                    return GameMode.Creative;
                case "2":
                case "adventure":
                    return GameMode.Adventure;
                case "3":
                case "spectator":
                    return GameMode.Spectator;
                default:
                    return defaultValue;
            }
        }

        public bool GetProperty(string property, bool defaultValue)
        {
            try
            {
                string str = ReadString(property);
                return str == null ? defaultValue : Convert.ToBoolean(str);
            }
            catch
            {
                return defaultValue;
            }
        }

        public int GetProperty(string property, int defaultValue)
        {
            try
            {
                string str = ReadString(property);
                return str == null ? defaultValue : Convert.ToInt32(str);
            }
            catch
            {
                return defaultValue;
            }
        }

        public long GetProperty(string property, long defaultValue)
        {
            try
            {
                string str = ReadString(property);
                return str == null ? defaultValue : Convert.ToInt64(str);
            }
            catch
            {
                return defaultValue;
            }
        }

        public ulong GetProperty(string property, ulong defaultValue)
        {
            try
            {
                string str = ReadString(property);
                return str == null ? defaultValue : Convert.ToUInt64(str);
            }
            catch
            {
                return defaultValue;
            }
        }

        public Difficulty GetProperty(string property, Difficulty defaultValue)
        {
            string str = ReadString(property);
            if (str == null)
                return defaultValue;
            switch (str.ToLower())
            {
                case "0":
                case "peaceful":
                    return Difficulty.Peaceful;
                case "1":
                case "easy":
                    return Difficulty.Easy;
                case "2":
                case "normal":
                    return Difficulty.Normal;
                case "3":
                case "hard":
                    return Difficulty.Hard;
                case "hardcore":
                    return Difficulty.Hardcore;
                default:
                    return defaultValue;
            }
        }

        public string GetProperty(string property, string defaultValue)
        {
            return ReadString(property) ?? defaultValue;
        }

        private string ReadString(string property)
        {
            property = property.ToLower();
            return !KeyValues.ContainsKey(property) ? (string) null : KeyValues[property];
        }
    }
}