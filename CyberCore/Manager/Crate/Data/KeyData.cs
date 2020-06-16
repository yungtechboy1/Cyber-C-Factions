using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using CyberCore.Manager.Crate.Data;
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
        public Item ItemKey = null;
        public String NBT_Key = "";
        public String Key_Name = "";

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

        public KeyData(CrateKeyData c)
        {
            short iid =  c.Item_ID;
            short meta =  c.Item_Meta;
            String nbt =  c.Item_NBT;
            Key_Name =  c.Key_Name;
            NBT_Key =  c.NBT_Key;
            ItemKey = ItemFactory.GetItem(iid, meta);
            Console.WriteLine("ITEMKEYTYYYYYYYYYYYY >>>>>>>>>>>> " + ItemKey);

            if (ItemKey == null)
            {
                Console.WriteLine("ERROROROROOROROQWEQ WEQE!~~!!@##@123123414");
                return;
            }

            if (nbt.Length != 0) ItemKey.setCompoundTag(Encoding.ASCII.GetBytes(nbt));
        }
        
        [ObsoleteAttribute("This method is obsolete. Call CallNewMethod instead.", true)]
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

        public CrateKeyData toConfig()
        {
            
            if(ItemKey.ExtraData == null)ItemKey.ExtraData = new NbtCompound();
            String fnt = "";
            var a = new NbtFile();
            a.BigEndian = false;
            a.UseVarInt = true;
            a.RootTag = ItemKey.ExtraData;
            var aa = (new MemoryStream());
            a.SaveToStream(aa, NbtCompression.AutoDetect);
            var aaa = new StreamReader(aa).ReadToEnd();

            if (ItemKey.ExtraData.HasValue) fnt = aaa;
            
            var z = new CrateKeyData()
            {
                Item_ID = ItemKey.Id,
                Item_Meta = ItemKey.Metadata,
                NBT_Key = NBT_Key,
                Item_NBT = ItemKey.hasCompoundTag() ? fnt : "",
                Key_Name = Key_Name
            };
            return z;
        }
    }
}