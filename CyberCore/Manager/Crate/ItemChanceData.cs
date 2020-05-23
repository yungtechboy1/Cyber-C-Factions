using System;
using System.IO;
using CyberCore.Utils;
using fNbt;
using MiNET.Blocks;
using MiNET.Items;
using Newtonsoft.Json.Serialization;

namespace CyberCore.Manager.Crate
{
    public class ItemChanceData
    {
        public int Chance { get;  set; }
        public short ItemID{ get;  set; }
        public short ItemMeta{ get;  set; }
        public int Max_Count{ get;  set; }
        public string NBT { get;  set; } = ""; //NBT_HEX

        public ItemChanceData(Item i, int chance, int max_Count)
        {
            ItemID = i.Id;
            ItemMeta = i.Metadata;
            NBT = i.ExtraData.NBTToString();
            Max_Count = max_Count;
            Chance = chance;
        }

        // public ItemChanceData(Dictionary<String,Object> c)
        // {
        //     super(c);
        //     importcs(c);
        // }

        public ItemChanceData(short itemID, short itemMeta, int chance, short max_count = 1, string NBT = null)
        {
            ItemID = itemID;
            ItemMeta = itemMeta;
            this.NBT = NBT;
            Max_Count = max_count;
            Chance = chance;
        }

        public Item getItem()
        {
            Item i;
            try
            {
                i = ItemFactory.GetItem(ItemID, ItemMeta, 1);
                // i.ExtraData = 
                var v = NBTToByte();
                var a = new NbtFile();
                a.LoadFromBuffer(v,0,v.Length,NbtCompression.ZLib);
                i.ExtraData = (NbtCompound) a.RootTag;
                // i = Item.get(ItemID, ItemMeta, 1, NBTToByte());
                CyberCoreMain.Log.Error("ICD >>> NAME>>" + i.getName());
            }
            catch (Exception e)
            {
                CyberCoreMain.Log.Error("EEEEEEEEEEEEEEEEEEEERRRRRRRRRRRRRRRRRR",e);
                return ItemFactory.GetItem(0);
            }

            return i;
        }


        public byte[] NBTToByte()
        {
            if (string.IsNullOrEmpty(NBT)) return new byte[0];
            return parseHexBinary(NBT);
        }


        private static int hexToBin(char ch) {
            if ('0' <= ch && ch <= '9') {
                return ch - 48;
            } else if ('A' <= ch && ch <= 'F') {
                return ch - 65 + 10;
            } else {
                return 'a' <= ch && ch <= 'f' ? ch - 97 + 10 : -1;
            }
        }
        
        public static byte[] parseHexBinary(String s) {
            int len = s.Length;
            if (len % 2 != 0) {
                throw new Exception("hexBinary needs to be even-length: " + s);
            } else {
                byte[] o = new byte[len / 2];

                for(int i = 0; i < len; i += 2) {
                    int h = hexToBin(s[i]);
                    int l = hexToBin(s[i + 1]);
                    if (h == -1 || l == -1) {
                        throw new Exception("contains illegal character for hexBinary: " + s);
                    }

                    o[i / 2] = (byte)(h * 16 + l);
                }

                return o;
            }
        }

        public Item check()
        {
            var i = getItem();
            if (i == null) return null;
            Random rnd = new Random();
            if (rnd.Next(0, 100) < Chance)
            {
                while (rnd.Next(0, 100) < Chance * 2)
                {
                    if (i.Count >= Max_Count) break;
                    i.Count = (byte) (i.Count + 1);
                }

                return i;
            }

            return null;
        }

        public void updateDataFromItem(Item item)
        {
            String fnt = "";
            var a = new NbtFile();
            a.BigEndian = false;
            a.UseVarInt = true;
            a.RootTag = item.ExtraData;
            // byte[] bytes = NBTCompressionSteamTool.NBTCompressedStreamTools.a(a);
            var aa = (new MemoryStream());
            a.SaveToStream(aa, NbtCompression.AutoDetect);
            var aaa = new StreamReader(aa).ReadToEnd();

            if (item.ExtraData.HasValue) fnt = aaa;
            NBT = fnt;
            ItemMeta = item.Metadata;
            ItemID = item.Id;
            
        }
    }
}