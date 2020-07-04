using CyberCore.Custom;
using fNbt;
using MiNET.BlockEntities;

namespace CyberCore.Manager.Shop
{
    public class NewShopInv :CyberInventory
    {
        public NewShopInv() : base(850000,
            new ChestBlockEntity(), 54, new NbtList())
        {
        }
    }
}