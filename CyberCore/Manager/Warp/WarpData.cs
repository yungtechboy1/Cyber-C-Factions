using System;
using CyberCore.Utils;
using MiNET;
using MiNET.Utils;
using MiNET.Worlds;

namespace CyberCore.Manager.Warp
{
    public class WarpData
    {
        public String name { get; set; }
        public double x { get; set; }
        public double y { get; set; }
        public double z { get; set; }
        public String level { get; set; }

        public WarpData(String name, double x, double y, double z, String level)
        {
            this.name = name;
            this.x = x;
            this.y = y;
            this.z = z;
            this.level = level;
        }

        public String getName()
        {
            return name;
        }

        public double getX()
        {
            return x;
        }

        public double getY()
        {
            return y;
        }

        public double getZ()
        {
            return z;
        }

        public String getLevel()
        {
            return level;
        }

        public void TeleportPlayer(Player p)
        {
            var l = CyberCoreMain.GetInstance().getAPI().LevelManager.GetLevel(null, level);
            if (l == null)
            {
                CyberCoreMain.Log.Error($"Error Finding level {level}");
                return;
            }

            if (!l.LevelId.equalsIgnoreCase(p.Level.LevelId))
            {
                p.Level.RemovePlayer(p);
                l.AddPlayer(p,true);
            }
            p.Teleport(toLocation());
        }

        public PlayerLocation toLocation()
        {
            return new PlayerLocation(x, y, z);
        }
    }
}