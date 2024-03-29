﻿using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Numerics;
using System.Text;
using CyberCore.Manager.Factions;
using CyberCore.WorldGen.Biomes;
using fNbt;
using JetBrains.Annotations;
using MiNET;
using MiNET.Blocks;
using MiNET.Effects;
using MiNET.Items;
using MiNET.UI;
using MiNET.Utils;
using MiNET.Worlds;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using OpenAPI.Player;
using Target = MiNET.Plugins.Target;

namespace CyberCore.Utils
{
    public static class CyberUtilsExtender
    {
        public static bool IsNullOrEmpty(this string c)
        {
            return string.IsNullOrEmpty(c);
        }

        public static Faction getFaction(this Player c)
        {
            return CyberCoreMain.GetInstance().FM.FFactory.getPlayerFaction((CorePlayer) c);
        }

        public static Faction getFaction(this CorePlayer c)
        {
            return CyberCoreMain.GetInstance().FM.FFactory.getPlayerFaction(c);
        }

        public static CorePlayer toCorePlayer(this Player p)
        {
            return CyberCoreMain.GetInstance().getPlayer(p);
        }

        public static object getOrDefault(this Dictionary<string, object> d, string s, object de)
        {
            if (d.ContainsKey(s)) return d[s];

            return de;
        }


        public static bool isEmpty(this PlayerInventory i, int k)
        {
            var ii = i.Slots[k];
            return ii is ItemAir || ii.Id == 0 || ii.Id == -1;
        }

        public static NbtCompound putInt(this NbtCompound i, string key, int val)
        {
            i.Add(new NbtInt(key, val));
            return i;
        }

        public static bool hasCompoundTag(this Item i)
        {
            return i.ExtraData != null;
        }

        public static bool getBoolean(this NbtCompound i, string key)
        {
            if (i.Contains(key))
            {
                var a = (NbtByte) i[key];
                return a.Value == 0 ? false : true;
            }

            return false;
        }

        public static NbtCompound putBoolean(this NbtCompound i, string key, bool val)
        {
            i.Add(new NbtByte(key, (byte) (val ? 1 : 0)));
            return i;
        }

        public static NbtCompound getDisplayCompound(this Item i)
        {
            if (i.hasCompoundTag())
            {
                var a = i.ExtraData;
                if (a.Contains("display")) return (NbtCompound) a["display"];
            }

            return new NbtCompound
            {
                new NbtCompound("display")
            };
        }

        public static NbtCompound getDisplayCompound(this NbtCompound i)
        {
            if (i != null)
            {
                if (i.Contains("display")) return (NbtCompound) i["display"];
                i.Add(new NbtCompound("display"));
                return (NbtCompound) i["display"];
            }

            i = new NbtCompound
            {
                new NbtCompound("display")
            };
            return (NbtCompound) i["display"];
        }

        public static Item setLore(this Item i, List<string> lines)
        {
            return setLore(i, lines.ToArray());
        }

        public static string[] getLore(this Item i)
        {
            var tag = getDisplayCompound(i);
            if (tag.Contains("Lore"))
            {
                var a = new List<string>();
                foreach (var v in (NbtList) tag["Lore"]) a.Add(v.StringValue);
                return a.ToArray();
            }

            ;
            var lore = new NbtList("Lore");
            tag.Add(lore);
            return new string[0];
        }

        public static FactionRank toFactionRank(this FactionRankEnum e)
        {
            return FactionRank.getRankFromFactionRankEnum(e);
        }

        public static RequestType toFactionRank(this RequestTypeEnum e)
        {
            return RequestType.fromEnum(e);
        }

        public static NbtList getLoreList(this NbtCompound i)
        {
            var tag = getDisplayCompound(i);
            if (tag.Contains("Lore")) return (NbtList) tag["Lore"];
            var lore = new NbtList("Lore");
            tag.Add(lore);
            return (NbtList) tag["Lore"];
        }

        public static NbtList getLoreList(this Item i)
        {
            var tag = getDisplayCompound(i);
            if (tag.Contains("Lore")) return (NbtList) tag["Lore"];
            var lore = new NbtList("Lore");
            tag.Add(lore);
            return (NbtList) tag["Lore"];
        }

        public static void clear(this PlayerInventory i, int slot)
        {
            i.Slots[slot] = new ItemAir();
        }

        public static bool isNull(this Item i)
        {
            return i == null || i.Id == -1 || i.Id == 0 || i.Count == 0;
        }

        public static bool isFull(this PlayerInventory i, Item ii = null)
        {
            foreach (var s in i.Slots)
            {
                if (ii != null && s.Equals(ii)) return false;
                if (s == null || s.Id == -1 || s.Id == 0) return false;
            }

            return true;
        }

        public static void setContents(this PlayerInventory i, List<Item> l)
        {
            if (l.Count == 0) return;
            i.Clear();
            var a = 0;
            for (var ii = 0; ii < l.Count; ++ii)
            {
                if (a >= l.Count) return;
                i.Slots[ii] = l[a++];
            }
        }

        public static string getMsg(this FactionErrorString f)
        {
            return toString(f);
        }

        public static string toString(this FactionErrorString f)
        {
            return FactionErrorStringMethod.toString(f);
        }

        public static NbtCompound putString(this NbtCompound i, string k, string v)
        {
            if (i.Contains(k)) i.Remove(k);
            i.Add(new NbtString(k, v));
            return i;
        }

        public static Player getPlayer(this Target t)
        {
            if (t.Players != null && t.Players.Length != 0) return t.Players[0];

            return null;
        }

        public static DateTime toDateTimeFromLongTime(this long l)
        {
            // return getTick();
            return new DateTime(l * TimeSpan.TicksPerSecond);
        }

        public static Item addLore(this Item i, params string[] lines)
        {
            var tag = getDisplayCompound(i);

            var lore = i.getLoreList();
            var var4 = lines;
            var var5 = lines.Length;

            for (var var6 = 0; var6 < var5; ++var6)
            {
                var line = var4[var6];
                //OR Maybe try ""
                lore.Add(new NbtString(line));
            }

            if (tag.Contains("Lore")) tag.Remove("Lore");
            tag.Add(lore);

            return i;
        }

        public static Item setLore(this Item i, params string[] lines)
        {
            var tag = getDisplayCompound(i);

            var lore = new NbtList("Lore");
            var var4 = lines;
            var var5 = lines.Length;

            for (var var6 = 0; var6 < var5; ++var6)
            {
                var line = var4[var6];
                //OR Maybe try ""
                lore.Add(new NbtString(line));
            }

            if (tag.Contains("Lore")) tag.Remove("Lore");
            tag.Add(lore);

            return i;
        }

        public static NbtCompound getNamedTag(this Item i)
        {
            if (i.ExtraData == null) i.ExtraData = new NbtCompound("");
            return i.ExtraData;
        }

        public static Item setCompoundTag(this Item i, byte[] n)
        {
            var a = new NbtFile();
            a.LoadFromBuffer(n, 0, n.Length, NbtCompression.ZLib);
            i.ExtraData = (NbtCompound) a.RootTag;
            return i;
        }

        public static Item setCompoundTag(this Item i, NbtCompound n)
        {
            i.ExtraData = n;
            return i;
        }

        public static bool equalsIgnoreCase(this string s, string ss)
        {
            return s.Equals(ss, StringComparison.CurrentCultureIgnoreCase);
        }

        public static bool hasCustomName(this Item i)
        {
            if (i.ExtraData == null) return false;

            Console.WriteLine("OK>......22222...");
            var tag = i.ExtraData;
            if (!tag.Contains("display")) return false;
            Console.WriteLine("OK>.........");
            var tag1 = tag["display"];
            if (tag1 != null && tag1 is NbtString) return true;

            return false;
        }

        public static string getCustomName(this Item i)
        {
            if (i.ExtraData != null) return i.GetType().Name;

            var tag = i.ExtraData;
            if (tag.Contains("display"))
            {
                var tag1 = tag.Get("display");
                // if (tag1.Contains("Name") &&
                //     ((CompoundTag) tag1).get("Name") instanceof StringTag) {
                //     return ((CompoundTag) tag1).getString("Name");
                // }
            }

            return "";
        }

        public static Item clearCustomName(this Item i)
        {
            if (i.ExtraData == null) return i;

            var tag = i.ExtraData;
            if (tag.Contains("display") && tag.Get("display") is NbtCompound)
            {
                var a = tag.Get<NbtCompound>("display");
                a.Remove("Name");
                if (a.Count == 0) tag.Remove("display");

                i.ExtraData = tag;
            }

            return i;
        }

        public static Item ClearShopTags(this Item i)
        {
            if (i.ExtraData == null) return i;

            var tag = i.ExtraData;
            if (tag.Contains("display")) tag.Remove("display");
            if (tag.Contains("buy")) tag.Remove("buy");
            if (tag.Contains("sell")) tag.Remove("sell");
            if (tag.Contains("backup") && tag["backup"] is NbtCompound b)
            {
                i.ExtraData = b;
                i.ExtraData.Name = "";
                // var b = (NbtCompound)
            }

            return i;
        }

        public static Item setCustomName(this Item i, [CanBeNull] string name)
        {
            try
            {
                if (string.IsNullOrEmpty(name)) return i.clearCustomName();

                if (i.ExtraData == null) i.ExtraData = new NbtCompound("");

                var tag = i.ExtraData;
                if (tag != null && tag.Contains("display") && tag.Get("display") is NbtCompound)
                {
                    var a = tag.Get<NbtCompound>("display");
                    if (a.Contains("Name")) a.Remove("Name");
                    a.Add(new NbtString("Name", name));
                }
                else
                {
                    tag.Add(new NbtCompound("display")
                    {
                        new NbtString("Name", name)
                    });
                }

                i.ExtraData = tag;
            }
            catch (Exception e)
            {
                CyberCoreMain.Log.Error(e);
            }

            return i;
        }

        public static Item addToCustomName(this Item i, [CanBeNull] string name)
        {
            if (string.IsNullOrEmpty(name)) return i;

            var n = i.getCustomName();
            n += "\n" + name;
            i.setCustomName(n);


            return i;
        }


        public static string toCyberString(this Vector3 i)
        {
            return i.X + "|" + i.Y + "|" + i.Z;
        }

        public static Vector3? V3FromCyberString(this string i)
        {
            var s = i.Split("|");
            if (s.Length != 3) return null;
            return new Vector3(int.Parse(s[0]), int.Parse(s[1]), int.Parse(s[2]));
        }

        public static string getName(this Block i)
        {
            return i.GetType().Name;
        }

        public static string getName(this Item i)
        {
            return i.hasCustomName() ? i.getCustomName() : i.GetType().Name;
        }

        public static bool CheckNbtSame(this NbtCompound i, NbtCompound ii)
        {
            var i1 = i.NBTToByteArray();
            var ii1 = i.NBTToByteArray();
            return i1 == ii1;
        }

        public static int GetX(this AdvancedBiome.BorderChunkDirection c)
        {
            switch (c)
            {
                case AdvancedBiome.BorderChunkDirection.North:
                case AdvancedBiome.BorderChunkDirection.South:
                    return 0;
                case AdvancedBiome.BorderChunkDirection.West:
                case AdvancedBiome.BorderChunkDirection.NW:
                case AdvancedBiome.BorderChunkDirection.SW:
                    return -1;
                case AdvancedBiome.BorderChunkDirection.NE:
                case AdvancedBiome.BorderChunkDirection.SE:
                case AdvancedBiome.BorderChunkDirection.East:
                    return 1;
                default:
                    return 0;
            }
        }

        public static int GetZ(this AdvancedBiome.BorderChunkDirection c)
        {
            switch (c)
            {
                case AdvancedBiome.BorderChunkDirection.North:
                case AdvancedBiome.BorderChunkDirection.NE:
                case AdvancedBiome.BorderChunkDirection.NW:
                    return 1;
                case AdvancedBiome.BorderChunkDirection.SE:
                case AdvancedBiome.BorderChunkDirection.SW:
                case AdvancedBiome.BorderChunkDirection.South:
                    return -1;
                case AdvancedBiome.BorderChunkDirection.West:
                case AdvancedBiome.BorderChunkDirection.East:
                default:
                    return 0;
            }
        }

        public static AdvancedBiome.BorderChunkDirection Opposite(this AdvancedBiome.BorderChunkDirection c)
        {
            switch (c)
            {
                case AdvancedBiome.BorderChunkDirection.North:
                    return AdvancedBiome.BorderChunkDirection.South;
                case AdvancedBiome.BorderChunkDirection.East:
                    return AdvancedBiome.BorderChunkDirection.West;
                case AdvancedBiome.BorderChunkDirection.South:
                    return AdvancedBiome.BorderChunkDirection.North;
                case AdvancedBiome.BorderChunkDirection.West:
                    return AdvancedBiome.BorderChunkDirection.East;
                case AdvancedBiome.BorderChunkDirection.NW:
                    return AdvancedBiome.BorderChunkDirection.SE;
                case AdvancedBiome.BorderChunkDirection.NE:
                    return AdvancedBiome.BorderChunkDirection.SW;
                case AdvancedBiome.BorderChunkDirection.SW:
                    return AdvancedBiome.BorderChunkDirection.NE;
                case AdvancedBiome.BorderChunkDirection.SE:
                    return AdvancedBiome.BorderChunkDirection.NW;
            }

            Console.WriteLine("ERROR OPPOSITE GAVE AN NONE!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
            return AdvancedBiome.BorderChunkDirection.None;
        }

        public static AdvancedBiome.ChunkSide Opposite(this AdvancedBiome.ChunkSide c)
        {
            switch (c)
            {
                case AdvancedBiome.ChunkSide.North:
                    return AdvancedBiome.ChunkSide.South;
                case AdvancedBiome.ChunkSide.East:
                    return AdvancedBiome.ChunkSide.West;
                case AdvancedBiome.ChunkSide.West:
                    return AdvancedBiome.ChunkSide.East;
                case AdvancedBiome.ChunkSide.South:
                    return AdvancedBiome.ChunkSide.North;
            }

            return AdvancedBiome.ChunkSide.NA;
        }


        public static AdvancedBiome.ChunkCorner Opposite(this AdvancedBiome.ChunkCorner c)
        {
            switch (c)
            {
                case AdvancedBiome.ChunkCorner.NorthWest:
                    return AdvancedBiome.ChunkCorner.SouthEast;
                case AdvancedBiome.ChunkCorner.NorthEast:
                    return AdvancedBiome.ChunkCorner.SouthWest;
                case AdvancedBiome.ChunkCorner.SouthWest:
                    return AdvancedBiome.ChunkCorner.NorthEast;
                case AdvancedBiome.ChunkCorner.SouthEast:
                    return AdvancedBiome.ChunkCorner.NorthWest;
            }

            return AdvancedBiome.ChunkCorner.NA;
        }

        public static List<int> GetSlotsOfItem(this PlayerInventory inv, Item itm, bool CheckNametag = true)
        {
            var a = new List<int>();
            for (byte index = 0; (int) index < inv.Slots.Count; ++index)
            {
                var i = inv.Slots[index];
                if (i == null || i.Id == 0) continue;
                if (i.Id != itm.Id) continue;
                if (i.Metadata != itm.Metadata) continue;
                if (CheckNametag && !i.ExtraData.CheckNbtSame(itm.ExtraData)) continue;
                a.Add(index);
            }

            return a;
        }


        /// <summary>
        ///     Take the amount of itm from player Invetory
        ///     This Function will check all the Player's Slots
        ///     To Try and remove the Amount of input item
        ///     Returns False if Amount is not enough or an error occured
        /// </summary>
        /// <param name="inv"></param>
        /// <param name="itm"></param>
        /// <param name="CheckNametag"></param>
        /// <returns></returns>
        public static bool TakeItem(this PlayerInventory inv, Item itm, bool CheckNametag = true)
        {
            var a = inv.GetSlotsOfItem(itm, CheckNametag);
            if (a.Count == 0) return false;
            //ActuallyRemoveList
            var ActuallyRemoveList = new List<int>();
            // PartialRemoveInt
            var PartialRemoveInt = -1;
            var takeamt = itm.Count;
            var takeamt1 = takeamt;

            foreach (var aa in a)
            {
                var i = inv.Slots[aa];
                if (takeamt > i.Count)
                {
                    ActuallyRemoveList.Add(aa);
                    takeamt -= i.Count;
                }
                else if (takeamt == i.Count)
                {
                    ActuallyRemoveList.Add(aa);
                    takeamt = 0;
                    break;
                }
                else
                {
                    PartialRemoveInt = aa;
                    takeamt = 0;
                    break;
                }
            }

            if (takeamt > 0) return false;

            foreach (var ari in ActuallyRemoveList)
            {
                var i = inv.Slots[ari];
                Console.WriteLine($" SLOT {ari} HAD A COUNT OF {i.Count} Which is now 0");
                inv.Slots[ari] = new ItemAir();
            }

            if (takeamt1 > 0)
                Console.WriteLine("GREATTTTT GO AHEAD");
            else
                Console.WriteLine("AHHHHHHHH TakeAmt was >>> " + takeamt1);

            if (PartialRemoveInt != -1)
            {
                var pri = inv.Slots[PartialRemoveInt];
                if (pri.Count < takeamt1) Console.WriteLine($"WHAT WHY WAS THIS MORE!!! {pri.Count} < {takeamt1}");
                pri.Count -= takeamt1;
                inv.Slots[PartialRemoveInt] = pri;
            }

            if (takeamt1 != 0)
                Console.WriteLine("WHAT TAKE AMT 1 WAS NOT 0 >>> " + takeamt1);
            else
                Console.WriteLine("TakeAmt was ========= 0  AFTER ");
            inv.Player.SendPlayerInventory();
            return true;

            //
            // // List<int> rmlist = new List<int>();
            // for (byte index = 0; (int) index < inv.Slots.Count; ++index)
            // {
            //     var i = inv.Slots[index];
            //     if (i == null || i.Id == 0) continue;
            //     if (i.Id != itm.Id) continue;
            //     if (i.Metadata != itm.Metadata) continue;
            //     if (CheckNametag && !i.ExtraData.CheckNbtSame(itm.ExtraData)) continue;
            //     if (takeamt > i.Count)
            //     {
            //         takeamt -= i.Count;
            //         i = null;
            //     }
            //     else
            //     {
            //         i.Count -= takeamt;
            //         return true;
            //     }
            // }
            //
            // return amt;
        }

        /// <summary>
        ///     WILL NOT FIND AIR
        /// </summary>
        /// <param name="inv"></param>
        /// <param name="itm"></param>
        /// <param name="CheckNametag"></param>
        /// <returns></returns>
        public static int GetCountOfItem(this PlayerInventory inv, Item itm, bool CheckNametag = true)
        {
            var amt = 0;
            foreach (var i in inv.Slots)
            {
                if (i == null || i.Id == 0) continue;
                if (i.Id != itm.Id) continue;
                if (i.Metadata != itm.Metadata) continue;
                if (CheckNametag && !i.ExtraData.CheckNbtSame(itm.ExtraData)) continue;
                amt += i.Count;
            }

            return amt;
        }

        /// <summary>
        ///     WILL NOT FIND AIR
        /// </summary>
        /// <param name="inv"></param>
        /// <param name="itm"></param>
        /// <param name="CheckNametag"></param>
        /// <returns></returns>
        public static int GetCountOfItem(this Inventory inv, Item itm, bool CheckNametag = true)
        {
            var amt = 0;
            foreach (var i in inv.Slots)
            {
                if (i == null || i.Id == 0) continue;
                if (i.Id != itm.Id) continue;
                if (i.Metadata != itm.Metadata) continue;
                if (CheckNametag && !i.ExtraData.CheckNbtSame(itm.ExtraData)) continue;
                amt += i.Count;
            }

            return 0;
        }

        public static bool HasEmptySlot(this PlayerInventory inv)
        {
            foreach (var i in inv.Slots)
                if (i == null || i.Id == 0)
                    return true;

            return false;
        }

        public static bool ContainsItem(this PlayerInventory i, Item itm, bool checkcount = false,
            bool checknbt = false)
        {
            var a = GetItemSlot(i, itm, checkcount, checknbt);
            return a == -1 ? false : true;
        }

        public static bool ContainsItem(this Inventory i, Item itm, bool checkcount = false, bool checknbt = false)
        {
            var a = GetItemSlot(i, itm, checkcount, checknbt);
            return a == -1 ? false : true;
        }

        public static int GetItemSlot(this PlayerInventory i, Item itm, bool checkcount = false, bool checknbt = false)
        {
            if (itm == null) return -1;
            for (byte index = 0; (int) index < i.Slots.Count; ++index)
            {
                var si = i.Slots[index];
                if (si.Id == itm.Id && si.Metadata == itm.Metadata)
                {
                    if (checkcount)
                        if (si.Count != itm.Count)
                            continue;

                    if (checknbt)
                    {
                        var n1 = si.getNamedTag().NBTToByteArray();
                        var n2 = itm.getNamedTag().NBTToByteArray();
                        if (n1 != n2) continue;
                    }

                    return index;
                }
            }

            return -1;
        }

        public static int GetItemSlot(this Inventory i, Item itm, bool checkcount = false, bool checknbt = false)
        {
            if (itm == null) return -1;
            for (byte index = 0; (int) index < i.Slots.Count; ++index)
            {
                var si = i.Slots[index];
                if (si.Id == itm.Id && si.Metadata == itm.Metadata)
                {
                    if (checkcount)
                        if (si.Count != itm.Count)
                            continue;

                    if (checknbt)
                    {
                        var n1 = si.getNamedTag().NBTToByteArray();
                        var n2 = itm.getNamedTag().NBTToByteArray();
                        if (n1 != n2) continue;
                    }

                    return index;
                }
            }

            return -1;
        }

        // public static int getID(this MainForm f)
        // {
        //     return f;
        // }

        public static PlayerLocation Safe(this PlayerLocation l, Level lvl)
        {
            var h = lvl.GetHeight(new BlockCoordinates(l));
            var b = lvl.GetBlock(new Vector3(l.X, h + 1, l.Z));
            var bb = lvl.GetBlock(new Vector3(l.X, h + 1 + 1, l.Z));
            var bbb = lvl.GetBlock(new Vector3(l.X, h + 1 + 2, l.Z));
            if ((b.Id == 0) & (bb.Id == 0)) return l;
            if (b.Id != 0 && bb.Id == 0 && bbb.Id == 0) return l;
            b = lvl.GetBlock(new Vector3(l.X, l.Y, l.Z));
            bb = lvl.GetBlock(new Vector3(l.X, l.Y + 1, l.Z));
            bbb = lvl.GetBlock(new Vector3(l.X, l.Y + 2, l.Z));
            if ((b.Id == 0) & (bb.Id == 0)) return l;
            if (b.Id != 0 && bb.Id == 0 && bbb.Id == 0) return l;
            //SAFE BUFFLE
            //5x5x5

            for (var i = (int) l.Y; i < 255; i++)
            {
                var bid = lvl.GetBlock(new Vector3(l.X, i, l.Z));
                var bid2 = lvl.GetBlock(new Vector3(l.X, i + 1, l.Z));
                var bid3 = lvl.GetBlock(new Vector3(l.X, i + 2, l.Z));
                if (bid.Id == 0 && bid2.Id == 0)
                {
                    l.Y = i;
                    return l;
                }

                if (bid.Id != 00 && bid2.Id == 0 && bid3.Id == 0)
                {
                    l.Y = i + 1;
                    return l;
                }
            }

            return l;
        }

        public static BlockCoordinates Safe(this BlockCoordinates l, Level lvl)
        {
            var h = lvl.GetHeight(l);
            l.Y = h + 1;
            return l;
        }

        public static BlockCoordinates Floor(this BlockCoordinates l)
        {
            return new BlockCoordinates((int) Math.Floor((double) l.X), (int) Math.Floor((double) l.Y),
                (int) Math.Floor((double) l.Z));
        }


        public static void showFormWindow(this OpenPlayer p, Form f)
        {
            p.SendForm(f);
        }

        public static string GetString(this DbDataReader r, string txt)
        {
            return r.GetString(r.GetOrdinal(txt));
        }

        public static string GetString2(this DbDataReader r, string txt)
        {
            return r.GetString(r.GetOrdinal(txt));
        }

        public static int GetInt32(this DbDataReader r, string txt)
        {
            return r.GetInt32(r.GetOrdinal(txt));
        }

        public static int GetInt322(this DbDataReader r, string txt)
        {
            return r.GetInt32(r.GetOrdinal(txt));
        }

        public static long GetInt64(this DbDataReader r, string txt)
        {
            return r.GetInt64(r.GetOrdinal(txt));
        }

        public static double GetDouble(this DbDataReader r, string txt)
        {
            return r.GetDouble(r.GetOrdinal(txt));
        }

        public static void setItemInHand(this PlayerInventory pi, Item item)
        {
            var ci = pi.GetItemInHand();
            var c = pi.GetItemSlot(ci);
            if (c != -1) pi.Slots[c] = item;
            pi.SendSetSlot(c);
        }

        // public static int GetItemSlot(this PlayerInventory pi, Item item, bool checkNBT = true)
        // {
        //     for (byte index = 0; (int) index < pi.Slots.Count; ++index)
        //     {
        //         var i = pi.Slots[index];
        //
        //         if (i.Id == item.Id && i.Metadata == item.Metadata)
        //         {
        //             if (checkNBT)
        //             {
        //                 if (item.ExtraData != null && i.ExtraData != null && i.ExtraData.NBTToString()
        //                     .equalsIgnoreCase(item.ExtraData.NBTToString())) return index;
        //                 return -1;
        //             }
        //
        //             return index;
        //         }
        //     }
        //
        //     return -1;
        // }

        public static string ToJsonStatic(this Form f)
        {
            Console.WriteLine("AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA");
            Console.WriteLine("AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA");
            Console.WriteLine("==============================================");
            return JsonConvert.SerializeObject(f, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                DefaultValueHandling = DefaultValueHandling.Include,
                MissingMemberHandling = MissingMemberHandling.Error,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                Formatting = Formatting.Indented,
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });
        }

        public static byte[] StringToNBTByte(this string s)
        {
            return Encoding.ASCII.GetBytes(s);
        }

        public static NbtCompound BytesToCompound(this byte[] b)
        {
            if (b == null || b.Length == 0) return null;
            var aa = new MemoryStream();
            aa.Write(b, 0, b.Length);
            var a = new NbtFile();
            a.BigEndian = false;
            a.UseVarInt = true;
            a.LoadFromBuffer(b, 0, b.Length, NbtCompression.None);
            return (NbtCompound) a.RootTag;
        }

        public static byte[] NBTToByteArray(this NbtCompound c)
        {
            if (c == null || c.Count == 0) return new byte[0];
            var a = new NbtFile();
            a.BigEndian = false;
            a.UseVarInt = true;
            a.RootTag = c;
            return a.SaveToBuffer(NbtCompression.None);
        }

        public static int getNextOpenSlot(this Inventory i)
        {
            for (var j = 0; j < i.Size; j++)
            {
                var itm = i.Slots[j];
                if (itm == null || itm.Id == 0) return j;
            }

            return -1;
        }

        public static bool hasEffect(this CorePlayer p, EffectType name)
        {
            return p.Effects.ContainsKey(name);
        }

        public static Effect getEffect(this CorePlayer p, EffectType name)
        {
            return p.Effects.ContainsKey(name) ? p.Effects[name] : null;
        }

        public static PlayerLocation Add(this PlayerLocation pl, PlayerLocation l)
        {
            return new PlayerLocation(pl.X + l.X, pl.Y + l.Y, pl.Z + l.Z);
        }

        public static PlayerLocation Subtract(this PlayerLocation pl, PlayerLocation l)
        {
            return new PlayerLocation(pl.X - l.X, pl.Y - l.Y, pl.Z - l.Z);
        }

        public static PlayerLocation Multiply(this PlayerLocation pl, PlayerLocation l)
        {
            return new PlayerLocation(pl.X * l.X, pl.Y * l.Y, pl.Z * l.Z);
        }

        public static PlayerLocation Divide(this PlayerLocation pl, PlayerLocation l)
        {
            return new PlayerLocation(pl.X / l.X, pl.Y / l.Y, pl.Z / l.Z);
        }


        [CanBeNull]
        public static OpenPlayer getPlayer(this OpenPlayerManager p, string name)
        {
            foreach (var players in p.GetPlayers())
                if (players.getName().equalsIgnoreCase(name))
                    return players;

            return null;
        }

        public static string getName(this Player p)
        {
            return p.Username;
        }

        public static int GetInt32(this Dictionary<string, object> list, string player)
        {
            return (int) list[player];
        }

        public static string GetString(this Dictionary<string, object> list, string player)
        {
            return (string) list[player];
        }

        public static double getDouble(this List<Dictionary<string, object>> list, string player)
        {
            return (double) list[0][player];
        }

        public static int GetInt32(this List<Dictionary<string, object>> list, string player)
        {
            return Convert.ToInt32(list[0][player]);
        }

        public static bool Read(this List<Dictionary<string, object>> list)
        {
            return list.Count != 0;
        }

        public static string GetString(this List<Dictionary<string, object>> list, string player)
        {
            return (string) list[0][player];
        }

        public static long GetInt64(this List<Dictionary<string, object>> list, string player)
        {
            return (long) list[0][player];
        }
    }
}