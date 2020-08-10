using System.ComponentModel.DataAnnotations;
using CyberCore.Custom;
using CyberCore.Utils;
using CyberCore.Utils.Data;
using fNbt;
using MiNET.BlockEntities;
using MiNET.Blocks;
using MiNET.Items;
using MiNET.Utils;

namespace CyberCore.Manager.Shop
{
    public class NewShopInv :CyberInventory
    {
        private int Page = 1;

        private CorePlayer player;
        // ShopStaticItems SI = new ShopStaticItems();
        public NewShopInv(CorePlayer p) : base(850000,
            new ChestBlockEntity(), 54, new NbtList())
        {
            Type = 0;
            WindowsId = 10;
            DisplayCatagories();
            player = p;
        }

        public int getPage()
        {
            return Page;

        }
        
        public void DisplayCatagories()
        {
            Clear();
            // setCurrentPage(CurrentPageEnum.Catagories);
             ShopStaticItems si = new ShopStaticItems(getPage());
             SetSlot(player,0,si.RaidingCatagory);
             SetSlot(player,1,new ItemApple());
//             for (int i = 0; i < 5; i++)
//             {
//                 for (int ii = 0; ii < 9; ii++)
//                 {
//                     Item bi = null;
//                     int slot = (i * 9) + ii;
//                     if (i == 0 || i == 4 || ii == 8 || ii == 7 || ii == 0 || ii == 1)
//                     {
// //                    bi = new ItemBlock(new BlockGlassPaneStained(0));
//                         bi = new ItemBlock(new Bedrock());
//                         bi.setCustomName("N/A");
//                     }
//                     else
//                     {
//                         if (i == 1)
//                         {
//                             if (ii == 2)
//                             {
//                                 bi = (Item) si.Spawner.Clone();
//                             }
//                             else if (ii == 3)
//                             {
//                                 bi = (Item) si.FoodCatagoty.Clone();
//                             }
//                             else if (ii == 4)
//                             {
//                                 bi = (Item) si.WeaponsCatagory.Clone();
//                             }
//                             else if (ii == 5)
//                             {
//                                 bi = (Item) si.BuildingCatagory.Clone();
//                             }
//                             else if (ii == 6)
//                             {
//                                 bi = (Item) si.RaidingCatagory.Clone();
//                             }
//                         }
//                     }
//
//                     if (bi == null)
//                     {
//                         bi = new ItemBlock(new Bedrock());
//                         bi.setCustomName(ChatColors.Gray + "FEATURE CURRENTLY DISABLED!");
//                     }
// SetSlot(player,(byte) slot,bi);
//                     // setItem(slot, bi, true);
//                 }
//             }
        }
    }
}