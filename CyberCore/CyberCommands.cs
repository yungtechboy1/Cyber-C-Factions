using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Numerics;
using System.Runtime.CompilerServices;
using AStarNavigator;
using CyberCore.Manager.Crate;
using CyberCore.Manager.Crate.Form;
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
using Mono.Cecil.Cil;
using OpenAPI.Events.Player;
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
            var to = new PlayerLocation(r.Next(-WildMaxVal, WildMaxVal), 100, r.Next(-WildMaxVal, WildMaxVal));
            p.Level.WorldProvider.GenerateChunkColumn(new ChunkCoordinates((int) to.X >> 4, (int) to.Z >> 4));
            p.delayTeleport(to, 10);
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
                p.SendMessage(ChatColors.Yellow +
                              "Spawn protection Re-Enabled for you! You can not break spawn blocks anymore");
            }
            else
            {
                SpawnProtection.Add(p.Username);
                p.SendMessage(ChatColors.Yellow +
                              "Spawn protection Disabled! You can now break and place blocks at spawn");
            }
        }

        public static Dictionary<String, TPData> AcceptTPR = new Dictionary<string, TPData>();
        public static Dictionary<String, TPData> AcceptTPRH = new Dictionary<string, TPData>();

        public class TPData
        {
            public String Name;
            public long Tick;

            public TPData(String n)
            {
                Name = n;
                Tick = CyberUtils.getTick();
            }

            public TPData(CorePlayer p)
            {
                Name = p.getName();
                Tick = CyberUtils.getTick();
            }

            private int max = 60 * 5; //5 Mins

            public bool isValid()
            {
                return Tick + (20 * max) > CyberUtils.getTick();
            }
        }

        [Command(Name = "tpd", Description = "Deny all teleport requests")]
        public void tpd(CorePlayer player)
        {
            AcceptTPRH.Remove(player.getName().ToLower());
            AcceptTPR.Remove(player.getName().ToLower());
        }

        [Command(Name = "tpr", Description = "Request To Teleport To A Player")]
        public void tpr(CorePlayer player, Target t)
        {
            var tp = t.getPlayer();
            if (tp != null)
            {
                AcceptTPRH.Remove(tp.getName().ToLower());
                AcceptTPR[tp.getName().ToLower()] = new TPData(player.getName());
                tp.SendMessage($"{ChatColors.Aqua}[TP REQUEST] {player.getName()} has requested to teleport to you!");
                tp.SendMessage($"{ChatColors.Aqua}[TP REQUEST] Use /tpa to accept the request!");
                tp.SendMessage($"{ChatColors.Aqua}[TP REQUEST] Use /tpd to deny the request!");
                player.SendMessage($"{ChatColors.Green} Request sent to {tp.getName()}!");
            }

            player.SendMessage($"{ChatColors.Red} Error! No Player found!");
        }

        [Command(Name = "tprh", Description = "Request A Player to teleport to you")]
        public void tprh(CorePlayer player, Target t)
        {
            var tp = t.getPlayer();
            if (tp != null)
            {
                AcceptTPR.Remove(tp.getName().ToLower());
                AcceptTPRH[tp.getName().ToLower()] = new TPData(player.getName());
                tp.SendMessage(
                    $"{ChatColors.Aqua}[TP REQUEST] {player.getName()} has requested you to teleport to them!");
                tp.SendMessage($"{ChatColors.Aqua}[TP REQUEST] Use /tpa to accept the request!");
                tp.SendMessage($"{ChatColors.Aqua}[TP REQUEST] Use /tpd to deny the request!");
                player.SendMessage($"{ChatColors.Green} Request sent to {tp.getName()}!");
            }

            player.SendMessage($"{ChatColors.Red} Error! No Player found!");
        }

        [Command(Name = "tpa", Description = "Accept Teleport")]
        public void tpa(CorePlayer player)
        {
            CorePlayer tp;
            if (AcceptTPR.ContainsKey(player.getName().ToLower()))
            {
                var d = AcceptTPR[player.getName().ToLower()];
                if (!d.isValid())
                {
                    player.SendMessage(
                        $"{ChatColors.Red}[TP ACCEPT] Error! The Teleport request is over 5 Mins old and has expired! Error: TP3");
                }
                else
                {
                    tp = CCM.getPlayer(d.Name);
                    if (tp != null)
                    {
                        player.SendMessage($"{ChatColors.Green}[TP ACCEPT] Request has been accepted!");
                        tp.SendMessage($"{ChatColors.Green}[TP ACCEPT] Request has been accepted!");
                        tp.delayTeleport((Vector3) player.KnownPosition.Clone(), 10);
                    }
                    else
                    {
                        player.SendMessage(
                            $"{ChatColors.Red}[TP ACCEPT] Error! Player could not be found! Error: TP18");
                    }
                }

                AcceptTPR.Remove(player.getName().ToLower());
            }
            else if (AcceptTPRH.ContainsKey(player.getName().ToLower()))
            {
                var d = AcceptTPRH[player.getName().ToLower()];
                if (!d.isValid())
                {
                    player.SendMessage(
                        $"{ChatColors.Red}[TPH ACCEPT] Error! The Teleport request is over 5 Mins old and has expired! Error: TP33");
                }
                else
                {
                    tp = CCM.getPlayer(d.Name);
                    if (tp != null)
                    {
                        player.SendMessage($"{ChatColors.Green}[TPH ACCEPT] Request has been accepted!");
                        tp.SendMessage($"{ChatColors.Green}[TPH ACCEPT] Request has been accepted!");
                        player.delayTeleport((Vector3) tp.KnownPosition.Clone(), 10);
                    }
                    else
                    {
                        player.SendMessage(
                            $"{ChatColors.Red}[TPH ACCEPT] Error! Player could not be found! Error: TP138");
                    }
                }

                AcceptTPRH.Remove(player.getName().ToLower());
            }
        }

        [Command(Name = "cash", Aliases = new[] {"bal", "balance"}, Description = "View your cash balance")]
        public void cash(CorePlayer p)
        {
            var bal = p.getPlayerSettingsData().getCash();
            p.SendMessage($"{ChatColors.Green}[CASH] Your Balance is: " + bal);
        }

        [Command(Name = "bank", Aliases = new[] {"bank bal"}, Description = "View your bank balance")]
        public void bank(CorePlayer p)
        {
            var bal = p.getPlayerSettingsData().BankBal;
            p.SendMessage($"{ChatColors.Green}[BANK] Your Balance is: " + bal);
        }

        [Command(Name = "deposit", Aliases = new[] {"bank deposit"},
            Description = "Deposit your cash balance to the bank")]
        public void deposit(CorePlayer p, int amt)
        {
            if (!p.getPlayerSettingsData().addToBank(amt))
            {
                p.SendMessage($"{ChatColors.Red}[BANK] ERROR | You only have: $" + p.getPlayerSettingsData().getCash());
                return;
            }

            p.SendMessage($"{ChatColors.Green}[BANK] SUCCESS | ${amt} has been depositied to your account!");
            p.SendMessage(
                $"{ChatColors.Green}[BANK] Your Bank Balance is {ChatColors.Aqua}${p.getPlayerSettingsData().BankBal}");
            p.SendMessage(
                $"{ChatColors.Green}[BANK] Your Cash Balance is {ChatColors.Aqua}${p.getPlayerSettingsData().getCash()}");
        }

        [Command(Name = "withdraw", Aliases = new[] {"bank withdraw"}, Description = "Withdraw money from the bank")]
        public void withdraw(CorePlayer p, int amt)
        {
            if (!p.getPlayerSettingsData().takeFromBank(amt))
            {
                p.SendMessage($"{ChatColors.Red}[BANK] ERROR | You only have: $" + p.getPlayerSettingsData().BankBal +
                              " in the bank!");
                return;
            }

            p.SendMessage($"{ChatColors.Green}[BANK] SUCCESS | ${amt} has been withdrawn to your account!");
            p.SendMessage(
                $"{ChatColors.Green}[BANK] Your Bank Balance is {ChatColors.Aqua}${p.getPlayerSettingsData().BankBal}");
            p.SendMessage(
                $"{ChatColors.Green}[BANK] Your Cash Balance is {ChatColors.Aqua}${p.getPlayerSettingsData().getCash()}");
        }

        [Command(Name = "crate key set", Description = "Add a new Crate Key to a Chest")]
        public void CKS(CorePlayer p)
        {
            CCM.CrateMain.addPrimedPlayer(p.getName().ToLower(), CrateMain.CrateAction.AddKeyToCrate);
            p.SendMessage(
                $"{ChatColors.Green}[CRATE] Please hold the key item and tap a chest to add the key to the chest.");
        }

        [Command(Name = "crate del", Description = "Remove a crate")]
        public void CD(CorePlayer p)
        {
            CCM.CrateMain.addPrimedPlayer(p.getName().ToLower(), CrateMain.CrateAction.DelCrate);
            p.SendMessage(
                $"{ChatColors.Green}[CRATE] Please break the chest you want to remove");
        }

        [Command(Name = "crate viewitems", Description = "View Crate Possible Items")]
        public void CVI(CorePlayer p)
        {
            CCM.CrateMain.addPrimedPlayer(p.getName().ToLower(), CrateMain.CrateAction.ViewCrateItems);
            p.SendMessage(
                $"{ChatColors.Green}[CRATE] Please tap a crate to view possible items.");
        }

        [Command(Name = "crate additem", Description = "Add a new Item to a Crate")]
        public void CAI(CorePlayer p)
        {
            CCM.CrateMain.addPrimedPlayer(p.getName().ToLower(), CrateMain.CrateAction.AddItemToCrate);
            p.SendMessage(
                $"{ChatColors.Green}[CRATE] Please hold the item and tap a chest to add the item to the crate.");
        } [Command(Name = "crate create", Description = "Add a new Item to a Crate")]
        public void CC(CorePlayer p)
        {
            CCM.CrateMain.addPrimedPlayer(p.getName().ToLower(), CrateMain.CrateAction.AddCrate);
            p.SendMessage(
                $"{ChatColors.Green}[CRATE] Please tap a chest to convert to a crate.");
        }[Command(Name = "crate admin", Description = "View Crate Main Menu")]
        public void CA(CorePlayer p)
        {
            p.SendForm(new AdminCrateMainMenu());
        }

        // [Command(Name = "loan", Aliases = new []{"bank loan"},Description = "Request a loan from the bank")]
        // public void loan(CorePlayer p, int amt)
        // {
        //     if (!p.getPlayerSettingsData().takeLoanFromBank(amt))
        //     {
        //         
        //         p.SendMessage($"{ChatColors.Red}[BANK] ERROR | Your credit limit is: $"+p.getPlayerSettingsData().getMaxLoanAmount()+"!");
        //         return;
        //     }
        //     p.SendMessage($"{ChatColors.Green}[BANK] SUCCESS | You have borrowed ${amt}! You have 12 Days to pay it back! Your Daily payments are ${amt/12}");
        //     p.SendMessage($"{ChatColors.Green}[BANK] Your Bank Balance is {ChatColors.Aqua}${p.getPlayerSettingsData().BankBal}");
        //     p.SendMessage($"{ChatColors.Green}[BANK] Your Cash Balance is {ChatColors.Aqua}${p.getPlayerSettingsData().getCash()}");
        // }

        //TODO TEST
        [Command(Name = "vi", Description = "Open a Chest")]
        public void OpenInventory(CorePlayer player)
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
            player.SendPacket(mcpeContainerOpen);
            McpeInventoryContent inventoryContent = McpeInventoryContent.CreateObject(1L);
            inventoryContent.inventoryId = (uint) inventory.WindowsId;
            inventoryContent.input = inventory.Slots;
            player.SendPacket(inventoryContent);
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