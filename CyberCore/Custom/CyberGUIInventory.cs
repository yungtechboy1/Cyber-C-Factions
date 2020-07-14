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

        public void FillContentsSlots(Item item = null)
        {
            if (item == null)
            {
                StainedGlass itm = new StainedGlass();
                itm.Color = "gray";
                item = new ItemBlock(itm);
            }

            int a = Size - 9;
            for (int i = 0; i < a; i++)
            {
                SetItem((byte) i, item);
            }
        }

        public virtual void MakeSelection(int slot, CorePlayer p)
        {
            SendInv(p);
        }
    }
}