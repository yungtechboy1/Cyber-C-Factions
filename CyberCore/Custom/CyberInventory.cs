using System;
using System.Collections.Generic;
using CyberCore.Utils;
using fNbt;
using MiNET;
using MiNET.BlockEntities;
using MiNET.Items;
using MiNET.Net;

namespace CyberCore.Custom
{
    public class CyberInventory : Inventory
    {
        public bool AdminMode = false;

        public event Action<Player, Inventory> InventoryJoin;
        public event Action<Player, Inventory> InventoryLeave;

        public CyberInventory(int id, BlockEntity blockEntity, short inventorySize, NbtList slots) : base(id,
            blockEntity, inventorySize, slots)
        {
        }

        public void SendInv(CorePlayer p)
        {
            McpeInventoryContent containerSetContent = McpeInventoryContent.CreateObject();
            containerSetContent.inventoryId = 10;
            containerSetContent.input = Slots;
            p.SendPacket(containerSetContent);
        }

        private NbtList GetSlots2()
        {
            NbtList nbtList = new NbtList("Items");
            for (byte index = 0; (int) index < (int) this.Size; ++index)
            {
                Item slot = this.Slots[(int) index];
                nbtList.Add((NbtTag) new NbtCompound()
                {
                    (NbtTag) new NbtByte("Count", slot.Count),
                    (NbtTag) new NbtByte("Slot", index),
                    (NbtTag) new NbtShort("id", slot.Id),
                    (NbtTag) new NbtShort("Damage", slot.Metadata)
                });
            }

            return nbtList;
        }

        public void SetItem(byte slot, Item itemStack)
        {
            this.Slots[(int) slot] = itemStack;
            this.BlockEntity.GetCompound()["Items"] = (NbtTag) GetSlots2();
            // this.OnInventoryChange(player, slot, itemStack);
        }

        protected virtual void PlayerJoin(Player player)
        {
            Action<Player, Inventory> PJ = InventoryJoin;
            if (PJ == null)
                return;
            PJ(player, this);
        }

        protected virtual void PlayerLeave(Player player)
        {
            Action<Player, Inventory> PL = InventoryLeave;
            if (PL == null)
                return;
            PL(player, this);
        }

        public new void AddObserver(Player player)
        {
            base.AddObserver(player);
        }

        public bool isEmpty(int k)
        {
            var ii = Slots[k];
            return ii is ItemAir || ii.Id == 0 || ii.Id == -1;
        }

        public void clear(int slot)
        {
            Slots[slot] = new ItemAir();
        }

        public bool isFull(Item ii = null)
        {
            foreach (var s in Slots)
            {
                if (ii != null && s.Equals(ii)) return false;
                if (s == null || s.Id == -1 || s.Id == 0) return false;
            }

            return true;
        }

        public int getNextOpenSlot(Inventory i)
        {
            for (int j = 0; j < Size; j++)
            {
                var itm = Slots[j];
                if (itm == null || itm.Id == 0) return j;
            }

            return -1;
        }

        public int GetItemSlot(PlayerInventory pi, Item item, bool checkNBT = true)
        {
            for (byte index = 0; (int) index < Slots.Count; ++index)
            {
                var i = Slots[index];

                if (Id == item.Id && i.Metadata == item.Metadata)
                {
                    if (checkNBT)
                    {
                        if (item.ExtraData != null && i.ExtraData != null && i.ExtraData.NBTToByteArray()
                            != (item.ExtraData.NBTToByteArray())) return index;
                        return -1;
                    }

                    return index;
                }
            }

            return -1;
        }

        public void Clear()
        {
            for (byte index = 0; (int) index < (int) Size; ++index)
                Slots.Add(new ItemAir());
        }

        public void setContents(List<Item> l)
        {
            if (l.Count == 0) return;
            Clear();
            var a = 0;
            for (var ii = 0; ii < l.Count; ++ii)
            {
                if (a >= l.Count) return;
                Slots[ii] = l[a++];
            }
        }
    }
}