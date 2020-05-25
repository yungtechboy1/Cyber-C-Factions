using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using AStarNavigator;
using CyberCore.Manager.Factions.Windows;
using CyberCore.Manager.FloatingText;
using CyberCore.Manager.Rank;
using CyberCore.Utils;
using log4net.Core;
using MiNET;
using MiNET.Blocks;
using MiNET.Plugins.Attributes;
using MiNET.Utils;
using OpenAPI.Utils;
using Level = MiNET.Worlds.Level;

namespace CyberCore
{
    public class CyberCommands
    {
        public static
            Dictionary<String, BlockCoordinates> pos1 = new Dictionary<string, BlockCoordinates>();

        Dictionary<String, BlockCoordinates> pos2 = new Dictionary<string, BlockCoordinates>();
        public static CyberCoreMain CCM;

        public CyberCommands(CyberCoreMain cyberCoreMain)
        {
            CCM = cyberCoreMain;
        }
        
        [Command(Name = "rank", Description = "Rank Options")]
        public void rank(CorePlayer p)
        {
            p.RefreshCommands();
            //a
            p.SendForm(new CyberFormRankWindow(p));
        }
        
        
        [Command(Name = "ft add", Description = "Add floating text")]
        public void ftadd(CorePlayer p, string text = null)
        {
            FloatingTextFactory.AddFloatingText(new CyberFloatingTextContainer(CCM.FTM,p,text),true);
            p.SendMessage(ChatColors.Green+"Floating Text Added!");
        }
        [Command(Name = "ft reload", Description = "Reload floating text")]
        public void ftreload(CorePlayer p)
        {
            
            FloatingTextFactory.killall(true);
            CCM.FTM.LoadFromSave();
            p.SendMessage(ChatColors.Green+"Floating REloaded Added!");
        }
        //
        // [Command(Name = "we p1", Description = "Set Point 1")]
        // // [ServerRankAttr(RankEnum.Administrative)]
        // public void wep1(CorePlayer p)
        // {
        //     var pp = new BlockCoordinates(p.KnownPosition) - BlockCoordinates.Down;
        //     pos1[p.getName().ToLower()] = pp;
        // }
        //
        // [Command(Name = "we p2", Description = "Set Point 1")]
        // // [ServerRankAttr(RankEnum.Administrative)]
        // public void wep2(CorePlayer p)
        // {
        //     var pp = new BlockCoordinates(p.KnownPosition) - BlockCoordinates.Down;
        //     pos2[p.getName().ToLower()] = pp;
        // }
        //
        // [Command(Name = "we cutpaste", Description = "Set Point 1")]
        // // [ServerRankAttr(RankEnum.Administrative)]
        // public void wecutpaste(CorePlayer p)
        // {
        //     var d = new ThreeDBlockContainer(pos1[p.getName().ToLower()], pos2[p.getName().ToLower()], p.Level);
        //     d.Place(p.KnownPosition.ToBlockCoordinates(), p.Level);
        // }
    }

    public class TwoDBlockContainer
    {
        public Block[,] DataId = new Block[256 * 4, 256 * 4];
        // public int[,] DataMeta = new int[256 * 4, 256 * 4];

        public void addData(int x, int z, Block data)
        {
            DataId[x, z] = data;
        }
    }

    public class ThreeDBlockContainer
    {
        public List<TwoDBlockContainer> Data = new List<TwoDBlockContainer>();

        public ThreeDBlockContainer(BlockCoordinates p1, BlockCoordinates p2, Level l)
        {
            // int xd = Math.Max(p1.X, p2.X) - Math.Min(p1.X, p2.X);
            // int yd = Math.Max(p1.Y, p2.Y) - Math.Min(p1.Y, p2.Y);
            // int zd = Math.Max(p1.Z, p2.Z) - Math.Min(p1.Z, p2.Z);
            int xx = 0;
            int zz = 0;
            for (int y = Math.Min(p1.Y, p2.Y); y <= Math.Max(p1.Y, p2.Y); y++)
            {
                var d = new TwoDBlockContainer();
                for (int x = Math.Min(p1.X, p2.X); x <= Math.Max(p1.X, p2.X); x++)
                {
                    for (int z = Math.Min(p1.Z, p2.Z); z <= Math.Max(p1.Z, p2.Z); z++)
                    {
                        var b = l.GetBlock(x, y, z);
                        d.addData(xx, zz, b);
                        zz++;
                    }

                    xx++;
                }
                    Data.Add(d);
            }
        }

        public void Place(BlockCoordinates toBlockCoordinates, Level l)
        {
            int y = 0;
            foreach (TwoDBlockContainer d in Data)
            {
                for (int x = 0; x < d.DataId.GetLength(0); x++)
                for (int z = 0; z < d.DataId.GetLength(1); z++)
                {
                    var bid = d.DataId[x, z];
                    bid.Coordinates = toBlockCoordinates + new BlockCoordinates(x, y, z);
                    l.SetBlock(bid);
                }

                y++;
            }
        }
    }
}