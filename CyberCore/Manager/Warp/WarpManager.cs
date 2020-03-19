using System;
using System.Collections.Generic;

namespace CyberCore.Manager.Warp
{
    public class WarpManager
    {
        public Dictionary<String, WarpData> WarpList { get; set; } = new Dictionary<string, WarpData>();
        private CyberCoreMain Main;

        public WarpManager(CyberCoreMain main)
        {
            Main = main;
        }

        public void AddWarp(WarpData wd)
        {
            WarpList.Add(wd.getName(), wd);
        }

        public void AddWarp(String name, double x, double y, double z, String level)
        {
            WarpList.Add(name.ToLower(), new WarpData(name, x, y, z, level));
        }

        public void RemoveWarp(String name)
        {
            WarpList.Remove(name);
        }

        public WarpData GetWarp(String name)
        {
            if (!WarpList.ContainsKey(name.ToLower())) return null;
            return WarpList[name.ToLower()];
        }
    }
}