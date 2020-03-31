using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using CyberCore.Utils;
using fNbt;
using MiNET.Items;
using MiNET.Utils;

namespace CyberCore.Manager.Crate
{
    public class KeyData
    {
        //    int Item_ID = 0;
//    int Item_Meta = 0;
        Item ItemKey = null;
        String NBT_Key = "";
        String Key_Name = "";

        public Item getItemKey()
        {
            return ItemKey;
        }

        public String getNBT_Key()
        {
            return NBT_Key;
        }

        public String getKey_Name()
        {
            return Key_Name;
        }

        public KeyData(Item i, String name, String nbtkey)
        {
            if (i == null)
            {
                CyberCoreMain.Log.Error("KD >>> The Item given is null!");
            }

//    Item_ID = i.getId();
//    Item_Meta = i.getDamage();
            ItemKey = (Item) i.Clone();
            ItemKey.Count = (1);
            Key_Name = name;
            NBT_Key = nbtkey;
            if (!ItemKey.hasCompoundTag()) ItemKey.ExtraData = new NbtCompound();
            ItemKey.ExtraData = (ItemKey.getNamedTag().putString(CrateMain.CK, Key_Name));

//    Item_NBT
        }

        public KeyData(Dictionary<String,Object> c)
        {
            if (!c.ContainsKey("Item-ID") || !c.ContainsKey("Item-Meta") || !c.ContainsKey("Item-NBT") ||
                !c.ContainsKey("Key_Name") || !c.ContainsKey("NBT_Key"))
            {
                Console.WriteLine("Error! Invalid Config!!!!!!!!!!!!!!!!!!!!!");
                return;
            }

            int iid = (int) c["Item-ID"];
            int meta = (int) c["Item-Meta"];
            String nbt = (string) c["Item-NBT"];
            Key_Name = (string) c["Key_Name"];
            NBT_Key = (string) c["NBT_Key"];
            ItemKey = ItemFactory.GetItem((short)iid,(short) meta);
            Console.WriteLine("ITEMKEYTYYYYYYYYYYYY >>>>>>>>>>>> " + ItemKey);

            if (ItemKey == null)
            {
                Console.WriteLine("ERROROROROOROROQWEQ WEQE!~~!!@##@123123414");
                return;
            }

            if (nbt.Length != 0) ItemKey.setCompoundTag(Encoding.ASCII.GetBytes(nbt));
        }

        public Dictionary<String,Object> toConfig()
        {
//    if(ItemKey.hasCompoundTag())ItemKey.setNamedTag(ItemKey.getNamedTag());
            Dictionary<String,Object> c = new Dictionary<String,Object>();
            
            var nbt = new Nbt
            {
                NbtFile = new NbtFile
                {
                    BigEndian = false,
                    UseVarInt = true,
                    RootTag =  ItemKey.ExtraData
                }
            };
            
            String fnt = "";
            var a = new NbtFile();
            a.BigEndian = false;
            a.UseVarInt = true;
            a.RootTag = ItemKey.ExtraData;
            // byte[] bytes = NBTCompressionSteamTool.NBTCompressedStreamTools.a(a);
            var aa = (new MemoryStream());
            a.SaveToStream(aa, NbtCompression.AutoDetect);
            var aaa = new StreamReader(aa).ReadToEnd();

            if (ItemKey.ExtraData.HasValue) fnt = aaa;
            
            
            
            c.Add("Item-ID", ItemKey.Id);
            c.Add("Item-Meta", ItemKey.Metadata);
            c.Add("Item-NBT", ItemKey.hasCompoundTag() ? fnt : "");
            c.Add("Key_Name", Key_Name);
            c.Add("NBT_Key", NBT_Key);
            return c;
        }
    }
}