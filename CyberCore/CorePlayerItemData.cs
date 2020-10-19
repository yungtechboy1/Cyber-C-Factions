using System;
using System.Text;
using CyberCore.Utils;
using MiNET.Items;

namespace CyberCore
{
    public class  CorePlayerItemData
    {
        public int Id { get; set; }
        public int Metadata { get; set; }
        public int Count { get; set; }
        public string NBT { get; set; }

        public CorePlayerItemData(int id, int metadata = 0, int count = 1, string nbt = "")
        {
            Id = id;
            Metadata = metadata;
            Count = count;
            NBT = nbt;
        }

        public static CorePlayerItemData CreateObject(Item i)
        {
            Console.WriteLine("AAAAAAAAAAAAAAAAAAASAVING WITH ID"+i.Id+"||||"+i.getNamedTag().NBTToString()+"|||"+i.getNamedTag().Count);
            return new CorePlayerItemData(i.Id,i.Metadata,i.Count,i.getNamedTag().NBTToString());
            
        }

        public Item toItem()
        {
            var  i=  ItemFactory.GetItem((short) Id, (short)Metadata, Count);
            if(!string.IsNullOrEmpty(NBT))i.setCompoundTag(NBT.StringToNBTByte());
            return i;
        }
    }
}