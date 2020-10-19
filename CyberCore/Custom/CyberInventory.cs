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

        public CyberInventory(int id, BlockEntity blockEntity, short inventorySize, NbtList slots) : base(id,
            blockEntity, inventorySize, slots)
        {
        }

        public event Action<Player, Inventory> InventoryJoin;
        public event Action<Player, Inventory> InventoryLeave;

        public void SendInv(CorePlayer p)
        {
            var containerSetContent = McpeInventoryContent.CreateObject();
            containerSetContent.inventoryId = 10;
            containerSetContent.input = Slots;
            p.SendPacket(containerSetContent);
        }

        private NbtList GetSlots2()
        {
            var nbtList = new NbtList("Items");
            for (byte index = 0; (int) index < (int) Size; ++index)
            {
                var slot = Slots[index];
                nbtList.Add(new NbtCompound
                {
                    new NbtByte("Count", slot.Count),
                    new NbtByte("Slot", index),
                    new NbtShort("id", slot.Id),
                    new NbtShort("Damage", slot.Metadata)
                });
            }

            return nbtList;
        }

        public void SetItem(byte slot, Item itemStack)
        {
            Slots[slot] = itemStack;
            BlockEntity.GetCompound()["Items"] = GetSlots2();
            // this.OnInventoryChange(player, slot, itemStack);
        }

        protected virtual void PlayerJoin(Player player)
        {
            var PJ = InventoryJoin;
            if (PJ == null)
                return;
            PJ(player, this);
        }

        protected virtual void PlayerLeave(Player player)
        {
            var PL = InventoryLeave;
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
            for (var j = 0; j < Size; j++)
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
                            != item.ExtraData.NBTToByteArray()) return index;
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