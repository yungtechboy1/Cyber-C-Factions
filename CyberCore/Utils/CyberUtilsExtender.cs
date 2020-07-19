using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text.Json.Serialization;
using System.Xml.Schema;
using CyberCore.Manager.Factions;
using CyberCore.Manager.Shop;
using fNbt;
using JetBrains.Annotations;
using MiNET;
using MiNET.Blocks;
using MiNET.Effects;
using MiNET.Items;
using MiNET.UI;
using MiNET.Utils;
using MiNET.Worlds;
using OpenAPI.Player;
using Org.BouncyCastle.Asn1.X509;
using Target = MiNET.Plugins.Target;

namespace CyberCore.Utils
{
    public static class CyberUtilsExtender
    {
        public static bool IsNullOrEmpty(this String c)
        {
            return String.IsNullOrEmpty(c);
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
            else
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

        public static String getMsg(this FactionErrorString f)
        {
            return toString(f);
        }

        public static String toString(this FactionErrorString f)
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
            if (t.Players != null && t.Players.Length != 0)
            {
                return t.Players[0];
            }

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
            String[] s = i.Split("|");
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
                    {
                        if (si.Count != itm.Count) continue;
                    }
                    if (checknbt)
                    {
                        var n1 = si.getNamedTag().NBTToString();
                        var n2 = itm.getNamedTag().NBTToString();
                        if (!n1.equalsIgnoreCase(n2))
                        {
                            continue;
                        }
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
                    {
                        if (si.Count != itm.Count) continue;
                    }
                    if (checknbt)
                    {
                        var n1 = si.getNamedTag().NBTToString();
                        var n2 = itm.getNamedTag().NBTToString();
                        if (!n1.equalsIgnoreCase(n2))
                        {
                            continue;
                        }
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
            int h = lvl.GetHeight(new BlockCoordinates(l));
            l.Y = h + 1;
            for (int i = 255; i > 0; i--)
            {
                var bid = lvl.GetBlock(new Vector3(l.X, i, l.Z));
                if (bid.Id != 0)
                {
                    l.Y = i + 1;
                    break;
                }
            }

            return l;
        }

        public static BlockCoordinates Safe(this BlockCoordinates l, Level lvl)
        {
            int h = lvl.GetHeight(l);
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

        public static string GetString(this DbDataReader r, String txt)
        {
            return r.GetString(r.GetOrdinal(txt));
        }

        public static string GetString2(this DbDataReader r, String txt)
        {
            return r.GetString(r.GetOrdinal(txt));
        }

        public static int GetInt32(this DbDataReader r, String txt)
        {
            return r.GetInt32(r.GetOrdinal(txt));
        }

        public static int GetInt322(this DbDataReader r, String txt)
        {
            return r.GetInt32(r.GetOrdinal(txt));
        }

        public static long GetInt64(this DbDataReader r, String txt)
        {
            return r.GetInt64(r.GetOrdinal(txt));
        }

        public static double GetDouble(this DbDataReader r, String txt)
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

        public static String NBTToString(this NbtCompound c)
        {
            String fnt = "";
            var a = new NbtFile();
            a.BigEndian = false;
            a.UseVarInt = true;
            a.RootTag = c;
            // byte[] bytes = NBTCompressionSteamTool.NBTCompressedStreamTools.a(a);
            var aa = (new MemoryStream());
            a.SaveToStream(aa, NbtCompression.AutoDetect);
            var aaa = new StreamReader(aa).ReadToEnd();

            if (c.HasValue) fnt = aaa;
            else CyberCoreMain.Log.Error("NBTTOSTRING C NO VALUIE!!");
            return fnt;
        }


        public static int getNextOpenSlot(this Inventory i)
        {
            for (int j = 0; j < i.Size; j++)
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

        public static String GetString(this Dictionary<string, object> list, string player)
        {
            return (String) list[player];
        }

        public static double getDouble(this List<Dictionary<string, object>> list, string player)
        {
            return (double) list[0][player];
        }

        public static int GetInt32(this List<Dictionary<string, object>> list, string player)
        {
            return (int) Convert.ToInt32(list[0][player]);
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