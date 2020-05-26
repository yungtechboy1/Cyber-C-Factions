using System;
using MiNET.Items;

namespace CyberCore
{
    public class  CorePlayerItemData
    {
        public int Id { get; set; }
        public int Metadata { get; set; }
        public int Count { get; set; }

        public CorePlayerItemData(int id, int metadata = 0, int count = 1)
        {
            Id = id;
            Metadata = metadata;
            Count = count;
        }

        public static CorePlayerItemData CreateObject(Item i)
        {
            Console.WriteLine("AAAAAAAAAAAAAAAAAAASAVING WITH ID"+i.Id);
            return new CorePlayerItemData(i.Id,i.Metadata,i.Count);
            
        }

        public Item toItem()
        {
            return ItemFactory.GetItem((short) Id, (short)Metadata, Count);
        }
    }
}