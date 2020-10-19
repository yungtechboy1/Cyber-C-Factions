using System;
using System.Text;
using CyberCore.Utils;
using fNbt;
using MiNET.Items;

namespace CyberCore
{
    public class  CorePlayerItemData
    {
        public int Id { get; set; }
        public int Metadata { get; set; }
        public int Count { get; set; }
        public byte[] NBT { get; set; }

        public CorePlayerItemData(int id, int metadata = 0, int count = 1, byte[] nbt = null)
        {
            Id = id;
            Metadata = metadata;
            Count = count;
            NBT = nbt;
        }

        public static CorePlayerItemData CreateObject(Item i)
        {
            Console.WriteLine("AAAAAAAAAAAAAAAAAAASAVING WITH ID"+i.Id+"||||"+i.getNamedTag().NBTToByteArray()+"|||"+i.getNamedTag().Count);
            return new CorePlayerItemData(i.Id,i.Metadata,i.Count,i.getNamedTag().NBTToByteArray());
            
        }

        public Item toItem()
        {
            var  i=  ItemFactory.GetItem((short) Id, (short)Metadata, Count);
            if(NBT != null)i.setCompoundTag((NBT.BytesToCompound()));
            return i;
        }
    }
}