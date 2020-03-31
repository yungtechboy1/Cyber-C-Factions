using System;
using CyberCore.Custom.Items;
using CyberCore.CustomEnums;
using CyberCore.Utils;
using CyberCore.Utils.Cooldowns;
using fNbt;
using MiNET.Items;
using MiNET.Utils;

namespace CyberCore.Manager.ClassFactory.Powers
{
    public interface PowerHotBarInt
    {
        public static String getPowerHotBarItemNamedTagKey = "PowerHotBarItem";

        public static void RemoveAnyItemsInSlot(CorePlayer cp, LockedSlot ls)
        {
            Item i = cp.Inventory.Slots[ls.getSlot()];
            if (i != null)
            {
                //i.getNamedTag() != null
                if (i.getNamedTag() == null || !i.getNamedTag().Contains(getPowerHotBarItemNamedTagKey))
                {
                    if (cp.Inventory.isFull())
                    {
                        cp.Inventory.Slots[ls.getSlot()] = new ItemAir();
                        cp.Level.DropItem(cp.KnownPosition.ToVector3(), i);
                    }
                    else
                    {
                        for (int ii = 0; ii < cp.Inventory.Slots.Count; ii++)
                        {
                            if (ii == ls.getSlot()) continue;
                            Item iii = cp.Inventory.Slots[ii];
                            if (iii.isNull())
                            {
                                cp.Inventory.Slots[ii] = i;
                                break;
                            }
                        }
                    }
                }
            }

            cp.Inventory.clear(ls.getSlot());
        }


//    @Override
//    public InventoryClickEvent InventoryClickEvent(InventoryClickEvent e) {
//        getPlayer().sendMessage(ChatColors.RED + "Error! You can not change your Class Slot");
//        if (e.getSlot() == LS.getSlot()) {
//            if (e.getSourceItem().getNamedTag() != null && !e.getSourceItem().getNamedTag().contains(getPowerHotBarItemNamedTagKey)) {
//                e.setCancelled();
//                getPlayer().sendMessage(ChatColors.RED + "Error! You can not change your Class Slot");
//            }else{
//                getPlayer().sendMessage(ChatColors.YELLOW + "SAME SLOT THO!");
//            }
//        }
//        return e;
//
//    }
        bool canUpdateHotBar(int tick);

        public void updateHotbar(LockedSlot ls, CoolDownTick c, PowerAbstract p)
        {
            if (ls.Equals(LockedSlot.NA)) return;
            if (c == null || !c.isValid())
            {
                setPowerAvailable(p);
                Console.WriteLine("ACTIVE POWER");
            }
            else
            {
                Console.WriteLine("UNNNNNNNNACTIVE POWER");
                setPowerUnAvailable(p);
            }
        }
        public void antiSpamCheck(PowerAbstract p)
        {
//        int slot = 0;
            bool k = false;
            for (int slot = 0; slot < p.getPlayer().Inventory.Slots.Count; slot++)
            {
                if (slot == p.getLS().getSlot()) continue;
                bool g = false;
                foreach (LockedSlot ls in p.PlayerClass.getLockedSlots()){
                    if (ls.getSlot() == slot)
                    {
                        g = true;
                        break;
                    }
                }
                if (g) continue;
                //Checking other ActivePowers
//            if()
                Item i = p.getPlayer().Inventory.Slots[slot];
                if (i.getNamedTag() != null)
                {
                    if (i.getNamedTag().Contains(getPowerHotBarItemNamedTagKey))
                    {
                        p.getPlayer().Inventory.clear(slot);
                        k = true;
                    }
                }

                slot++;
            }

//        if (k) p.getPlayer().kick("Please do not spam system!");
        }
        public void setPowerAvailable(PowerAbstract p)
        {
            p.getPlayer().Inventory.Slots[p.getLS().getSlot()] =
                addNamedTag(p, getActiveItem(), p.getSafeName(), "Active");
//        getPlayer().getInventory().setHeldItemIndex(LS.getSlot());
        }
        public void setPowerUnAvailable(PowerAbstract p)
        {
            p.getPlayer().Inventory.Slots[p.getLS().getSlot()] =
                addNamedTag(p, getUnAvailableItem(), p.getSafeName(), "Idle");
        }
        public Item addNamedTag(PowerAbstract p, Item i, String key, String val)
        {
            if (p.Cooldown == null || !p.Cooldown.isValid())
            {
                i.setCustomName(ChatColors.Green + "Power: " + p.getDispalyName());
                i.setLore(ChatColors.Green + "Ready to Use",
                    ChatColors.Green + "Costs: " + p.getPowerSourceCost() + " " +
                    p.PlayerClass.getPowerSourceType() + " ");
            }
            else
            {
                i.setCustomName(ChatColors.Red + "Power: " + p.getDispalyName());
                i.setLore(p.Cooldown.toString(),
                    ChatColors.Green + "Costs: " + p.getPowerSourceCost() + " " +
                    p.PlayerClass.getPowerSourceType() + " ");
            }

            NbtCompound ct = i.getNamedTag();
            if (ct == null) ct = new NbtCompound();
            ct.putBoolean(getPowerHotBarItemNamedTagKey, true);
            i.ExtraData = (ct);
            if (key == null) return i;
           
            i.ExtraData = ( ct.putString(key, val));
            return i;
        }
        public Item getAvailableItem()
        {
            return new ItemSlimeBall();
        }
        public Item getActiveItem()
        {
            return new ItemSlimeBall(){Count = 5};
        }
        public Item getUnActiveItem()
        {
            return new ItemRedstone();
        }
        public Item getUnAvailableItem()
        {
            return new ItemRedstone();
        }
    }
}