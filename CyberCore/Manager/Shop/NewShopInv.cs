using CyberCore.Custom;
using fNbt;
using MiNET.BlockEntities;
using MiNET.Blocks;
using MiNET.Items;

namespace CyberCore.Manager.Shop
{
    public class NewShopInv :CyberInventory
    {
        public NewShopInv() : base(850000,
            new ChestBlockEntity(), 54, new NbtList())
        {
            SetItem(0,new ItemApple());
            SetItem(1,new ItemFireworks());
            SetItem(2,new ItemBlock(new Tnt()));
        }
    }
}