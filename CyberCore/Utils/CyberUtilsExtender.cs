using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using CyberCore.Manager.Factions;
using CyberCore.Manager.Forms;
using fNbt;
using JetBrains.Annotations;
using Jose;
using MiNET;
using MiNET.Blocks;
using MiNET.Effects;
using MiNET.Items;
using MiNET.UI;
using MiNET.Utils;
using OpenAPI.Player;

namespace CyberCore.Utils
{
    public static class CyberUtilsExtender
    {
        public static String getMessage(this FactionErrorString c)
        {
            return FactionErrorStringMethod.toString(c);
        }

        public static Faction getFaction(this Player c)
        {
            return CyberCoreMain.GetInstance().FM.FFactory.getPlayerFaction(c);
        }

        public static Object getOrDefault(this Dictionary<String, Object> d, String s, Object de)
        {
            if (d.ContainsKey(s))
            {
                return d[s];
            }

            return de;
        }

        public static String getName(this FactionRank s)
        {
            return FactionRankMethods.getName(s);
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

        public static Item setLore(this Item i, List<String> lines)
        {
            return setLore(i, lines.ToArray());
        }

        public static String[] getLore(this Item i)
        {
            NbtCompound tag = getDisplayCompound(i);
            if (tag.Contains("Lore")) {
                List<String> a = new List<String>();
                foreach (NbtTag v in (NbtList)tag["Lore"])
                {
                    a.Add(v.StringValue);
                }
                return a.ToArray();
            };
            NbtList lore = new NbtList("Lore");
            tag.Add(lore);
            return new String[0];
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
            NbtCompound tag = getDisplayCompound(i);
            if (tag.Contains("Lore")) return (NbtList) tag["Lore"];
            NbtList lore = new NbtList("Lore");
            tag.Add(lore);
            return (NbtList) tag["Lore"];
        }
        public static NbtList getLoreList(this Item i)
        {
            NbtCompound tag = getDisplayCompound(i);
            if (tag.Contains("Lore")) return (NbtList) tag["Lore"];
            NbtList lore = new NbtList("Lore");
            tag.Add(lore);
            return (NbtList) tag["Lore"];
        }

        public static void clear(this PlayerInventory i,int slot)
        {
            i.Slots[slot] = new ItemAir();
        }
        public static bool isNull(this Item i)
        {
            return (i == null || i.Id == -1 || i.Id == 0 || i.Count == 0);
        }
        
        public static bool isFull(this PlayerInventory i, Item ii = null)
        {
            foreach (var s in i.Slots)
            {
                if (ii !=  null && s.Equals(ii)) return false;
                if (s == null || s.Id == -1 || s.Id == 0) return false;
            }

            return true;
        }

        public static void setContents(this PlayerInventory i, List<Item> l)
        {
            if (l.Count == 0) return;
            i.Clear();
            int a = 0;
            for (int ii = 0; ii < l.Count; ++ii)
            {
                if(a >= l.Count)return;
                i.Slots[ii] = l[a++];
            }
        }
        public static NbtCompound putString(this NbtCompound i, String k, string v)
        {
            i.Add(new NbtString(k,v));
            return i;
        }
        public static Item addLore(this Item i,params String[] lines)
        {
            NbtCompound tag = getDisplayCompound(i);

            NbtList lore = i.getLoreList();
            String[] var4 = lines;
            int var5 = lines.Length;

            for (int var6 = 0; var6 < var5; ++var6)
            {
                String line = var4[var6];
                //OR Maybe try ""
                lore.Add(new NbtString(line));
            }

            if(tag.Contains("Lore"))tag.Remove("Lore");
            tag.Add(lore);
            
            return i;
        }
        public static Item setLore(this Item i, params String[] lines)
        {
            NbtCompound tag = getDisplayCompound(i);

            NbtList lore = new NbtList("Lore");
            String[] var4 = lines;
            int var5 = lines.Length;

            for (int var6 = 0; var6 < var5; ++var6)
            {
                String line = var4[var6];
                //OR Maybe try ""
                lore.Add(new NbtString(line));
            }

            if(tag.Contains("Lore"))tag.Remove("Lore");
            tag.Add(lore);
            
            return i;
        }

        public static NbtCompound getNamedTag(this Item i)
        {
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

        public static String getChatColor(this FactionRank s)
        {
            return FactionRankMethods.getChatColor(s);
        }

        public static int getFormPower(this FactionRank s)
        {
            return FactionRankMethods.getFormPower(s);
        }

        public static FactionRank getRankFromForm(this FactionRank s, int ss)
        {
            return FactionRankMethods.getRankFromForm(ss);
        }

        public static FactionRank getRankFromString(this FactionRank s, String ss)
        {
            return FactionRankMethods.getRankFromString(ss);
        }

        public static bool equalsIgnoreCase(this String s, String ss)
        {
            return s.Equals(ss, StringComparison.CurrentCultureIgnoreCase);
        }

        public static bool hasCustomName(this Item i)
        {
            if (i.ExtraData == null)
            {
                return false;
            }
            else
            {
                var tag = i.ExtraData;
                if (!tag.Contains("display"))
                {
                    return false;
                }
                else
                {
                    var tag1 = tag.Get("display");
                    if (tag1.StringValue != null) return true;
                }
            }

            return false;
        }

        public static String getCustomName(this Item i)
        {
            if (i.ExtraData != null)
            {
                return i.GetType().Name;
            }
            else
            {
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
        }

        public static Item clearCustomName(this Item i)
        {
            if (i.ExtraData == null) return i;

            var tag = i.ExtraData;
            if (tag.Contains("display") && tag.Get("display") is NbtCompound)
            {
                var a = tag.Get<NbtCompound>("display");
                a.Remove("Name");
                if (a.Count == 0)
                {
                    tag.Remove("display");
                }

                i.ExtraData = tag;
            }

            return i;
        }

        public static Item setCustomName(this Item i, String name)
        {
            if (String.IsNullOrEmpty(name)) return i.clearCustomName();

            var tag = i.ExtraData;
            if (tag.Contains("display") && tag.Get("display") is NbtCompound)
            {
                var a = tag.Get<NbtCompound>("display");
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
            return i;
        }


        public static String getName(this Item i)
        {
            return i.hasCustomName() ? i.getCustomName() : i.GetType().Name;
        }

        // public static int getID(this MainForm f)
        // {
        //     return f;
        // }

        public static void showFormWindow(this OpenPlayer p, Form f)
        {
            p.SendForm(f);
        }

        public static ExtraPlayerData GetExtraPlayerData(this OpenPlayer p)
        {
            return CyberUtils.getExtraPlayerData(p);
        }

        public static ExtraPlayerData GetExtraPlayerData(this Player p)
        {
            return CyberUtils.getExtraPlayerData(p);
        }

        public static Effect getEffect(this CorePlayer p, EffectType name)
        {
            return p.Effects.ContainsKey(name) ? p.Effects[name] : null;
        }
        [CanBeNull]
        public static OpenPlayer getPlayer(this OpenPlayerManager p, String name)
        {
            foreach (OpenPlayer players in p.GetPlayers())
            {
                if (players.getName().equalsIgnoreCase(name)) return players;
            }

            return null;
        }

        public static String getName(this Player p)
        {
            return p.Username;
        }
    }
}