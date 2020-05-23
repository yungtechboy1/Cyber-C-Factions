using System.Collections.Generic;
using fNbt;
using MiNET.Items;

namespace CyberCore.Manager.Factions.Missions
{
    public class MissionItemData
    {
        public short Id { get; set; }

        public short Metadata { get; set; }

        public byte Count { get; set; }
        public NbtCompound ExtraData { get; set; }

        public MissionItemData(Item i)
        {
            Id = i.Id;
            Metadata = i.Metadata;
            Count = i.Count;
            ExtraData = i.ExtraData;
        }

        public MissionItemData(short id, short metadata = 0, byte count = 1, NbtCompound extraData = null)
        {
            Id = id;
            Metadata = metadata;
            Count = count;
            ExtraData = extraData;
        }

        public Item toItem()
        {
            Item i = ItemFactory.GetItem(Id, Metadata,Count);
            if(i != null)i.ExtraData = ExtraData;//Might need to save as String then Phrase back to NBTCompound
            return i;
        }

        public static List<MissionItemData> fromItemList(List<Item> i)
        {
            var a= new List<MissionItemData>();
            foreach (var ii in i)
            {
                a.Add(new MissionItemData(ii));
            }

            return a;
        }
    }
}