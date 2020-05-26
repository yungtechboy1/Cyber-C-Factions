using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using AStarNavigator;
using CyberCore.Manager.Factions.Windows;
using CyberCore.Manager.FloatingText;
using CyberCore.Manager.Rank;
using CyberCore.Utils;
using fNbt;
using log4net.Core;
using MiNET;
using MiNET.BlockEntities;
using MiNET.Blocks;
using MiNET.Net;
using MiNET.Plugins;
using MiNET.Plugins.Attributes;
using MiNET.Utils;
using Mono.Cecil;
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
            FloatingTextFactory.AddFloatingText(new CyberFloatingTextContainer(CCM.FTM, p, text), true);
            p.SendMessage(ChatColors.Green + "Floating Text Added!");
        }

        [Command(Name = "ft reload", Description = "Reload floating text")]
        public void ftreload(CorePlayer p)
        {
            FloatingTextFactory.killall(true);
            FloatingTextFactory.SavedFloatingText.Clear();
            CCM.FTM.LoadFromSave();
            p.SendMessage(ChatColors.Green + "Floating REloaded Added!");
        }

        [Command(Name = "spawn", Description = "Teleport to spawn")]
        public void spawn(CorePlayer p)
        {
            p.delayTeleport(p.Level.SpawnPoint);
            p.SendMessage(ChatColors.Green + "Teleproting to spawn in 5 secs!");
        }
        
        public static readonly int WildMaxVal = 25000;
        [Command(Name = "wild", Description = "Teleport to random spot in the wild")]
        public void wild(CorePlayer p)
        {
            var r = new Random();
            var to = new PlayerLocation(r.Next(-WildMaxVal,WildMaxVal),100,r.Next(-WildMaxVal,WildMaxVal));
            p.Level.WorldProvider.GenerateChunkColumn(new ChunkCoordinates((int)to.X >> 4, (int)to.Z >> 4));
            p.delayTeleport(to,10);
            p.SendMessage(ChatColors.Green + "Teleproting to spawn in 10 secs!");
        }

        [Command(Name = "setspawn", Description = "Set a new spawn point")]
        [ServerRankAttr(RankEnum.Administrative)]
        public void setspawn(CorePlayer p)
        {
            p.Level.SpawnPoint = (PlayerLocation) p.KnownPosition.Clone();
            p.SendMessage(ChatColors.Green + "New Spawn Point Set!");
        }

        public static List<String> SpawnProtection = new List<string>();

        [Command(Name = "spawnprotection", Aliases = new[] {"sp"},
            Description = "Toggle Spawn Protection to Place and Remove Block")]
        [ServerRankAttr(RankEnum.Administrative)]
        public void spawnprotection(CorePlayer p)
        {
            if (SpawnProtection.Contains(p.Username))
            {
                SpawnProtection.Remove(p.Username);
                p.SendMessage(ChatColors.Yellow + "Spawn protection Re-Enabled for you! You can not break spawn blocks anymore");
            }
            else
            {
                SpawnProtection.Add(p.Username);
                p.SendMessage(ChatColors.Yellow + "Spawn protection Disabled! You can now break and place blocks at spawn");

            }
        }
        
         [Command(Name = "vi", Description = "Open a Chest")]
        public void OpenInventory(Player player)
        {
            //Log.Info("Command Executed!");
            player.SendMessage("Opening Chest...");
            
            /*BlockCoordinates coords = (BlockCoordinates) player.KnownPosition;
            coords.Y = 0;*/
            BlockCoordinates coords = new BlockCoordinates(0);

            //Block past = player.Level.GetBlock(coords);

            McpeUpdateBlock chest = McpeUpdateBlock.CreateObject();
            chest.blockRuntimeId = (byte) new Chest().GetRuntimeId();
            chest.coordinates = coords;
            chest.blockPriority = 0 & 15;
            player.SendPacket(chest);

            ChestBlockEntity blockEntity = new ChestBlockEntity {Coordinates = coords};
            NbtCompound compound = blockEntity.GetCompound();
            compound["CustomName"] = new NbtString("CustomName", "§5§k--§r §l§o§2Virtual Chest§r §5§k--§r");
            //player.Level.SetBlockEntity(blockEntity);
            McpeBlockEntityData chestEntity = McpeBlockEntityData.CreateObject();
            chestEntity.namedtag = new Nbt
            {
                NbtFile = new NbtFile
                {
                    BigEndian = false,
                    UseVarInt = true,
                    RootTag = compound
                }
            };
            chestEntity.coordinates = coords;
            player.SendPacket(chestEntity);

            //player.OpenInventory(coords);
            Inventory inventory = new Inventory(0, blockEntity, 1, new NbtList())
            {
                Type = 0,
                WindowsId = 10
            };
            
            //inventory.InventoryChange += new Action<Player, MiNET.Inventory, byte, Item>(player.OnInventoryChange);
            inventory.AddObserver(player);
            McpeContainerOpen mcpeContainerOpen = McpeContainerOpen.CreateObject(1L);
            mcpeContainerOpen.windowId = inventory.WindowsId;
            mcpeContainerOpen.type = inventory.Type;
            mcpeContainerOpen.coordinates = coords;
            mcpeContainerOpen.runtimeEntityId = 1L;
            player.SendPacket( mcpeContainerOpen);
            McpeInventoryContent inventoryContent = McpeInventoryContent.CreateObject(1L);
            inventoryContent.inventoryId = (uint) inventory.WindowsId;
            inventoryContent.input = inventory.Slots;
            player.SendPacket( inventoryContent);
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