using System;
using CyberCore.Utils;
using fNbt;
using MiNET.BlockEntities;
using MiNET.Blocks;
using MiNET.Items;

namespace CyberCore.Custom
{
    public class CyberGUIInventory : CyberInventory
    {
        public CyberGUIInventory(int id, BlockEntity blockEntity, short inventorySize, NbtList slots) : base(id,
            blockEntity, inventorySize, slots)
        {
        }


        public void ClearContentsSlots()
        {
            int a = Size - 9;
            for (int i = 0; i < a; i++)
            {
                SetItem((byte) i, new ItemAir());
            }
        }

        public void FillContentsSlots(Item item = null, bool fullfill = false,bool color = false)
        {
            if (item == null)
            {
                StainedGlass itm = new StainedGlass();
                itm.Color = "gray";
                item = new ItemBlock(itm);
            }

            var iitem = item;

            int yy = 6;
            // if (fullfill)
            // {
            //     yy++;
            //     // a += 9;
            // }
            for (int x = 0; x < 9; x++){//X
            for (int y = 0; y < yy; y++)//Y
            {
                int s = y * 9 + x;
                Console.WriteLine($"#{s} || X:{x} || Y:{y}");
                if (color && (x == 0 ||y == 0 ||x == 8 ||y == yy-1))
                {
                    StainedGlass itm = new StainedGlass();
                    itm.Color = "Red";
                    item = new ItemBlock(itm);
                    item.setCustomName($"#{s} || X:{x} || Y:{y}");
                }
                else
                {
                    item = (Item) iitem.Clone();
                    item.setCustomName($"#{s} || X:{x} || Y:{y}");
                    
                }
                if(!(Size <= s))SetItem((byte) s, item);
                

            }
            }
            // for (int i = 0; i < a; i++)
            // {
            // }
        }

        public virtual void MakeSelection(int slot, CorePlayer p)
        {
            SendInv(p);
        }
    }
}