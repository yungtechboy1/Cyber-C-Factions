using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using CyberCore.Custom.Items;
using CyberCore.CustomEnums;
using CyberCore.Utils;
using CyberCore.Utils.Cooldowns;
using fNbt;
using MiNET.Items;
using MiNET.Utils;

namespace CyberCore.Manager.ClassFactory.Powers
{
    /***
     * @Deprecated DO NO USE ANY MORE
     */
    public abstract class PowerHotBarInt : PowerAbstract
    {
        public static String getPowerHotBarItemNamedTagKey = "PowerHotBarItem";

        public static void RemoveAnyItemsInSlot(CorePlayer cp, LockedSlot ls)
        {
            Item i = cp.Inventory.Slots[ls.getSlot()];
            if (i != null)
            {
                Console.WriteLine($"ITEM IS IN SLOT {ls.getSlot()}");
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
            
            cp.SendPlayerInventory();
        }


    // @Override
    // public InventoryClickEvent InventoryClickEvent(InventoryClickEvent e) {
    //     getPlayer().sendMessage(ChatColors.RED + "Error! You can not change your Class Slot");
    //     if (e.getSlot() == LS.getSlot()) {
    //         if (e.getSourceItem().getNamedTag() != null && !e.getSourceItem().getNamedTag().contains(getPowerHotBarItemNamedTagKey)) {
    //             e.setCancelled();
    //             getPlayer().sendMessage(ChatColors.RED + "Error! You can not change your Class Slot");
    //         }else{
    //             getPlayer().sendMessage(ChatColors.YELLOW + "SAME SLOT THO!");
    //         }
    //     }
    //     return e;
    //
    // }
    private long LastHotBarCheck = 0;
    private long LastHotBarApproved = 0;

    public void initHotbar(CorePlayer cp)
    {
        Console.WriteLine("============================================STARTING INTI PROCESS ===============================");
        Console.WriteLine("============================================STARTING INTI PROCESS ===============================");
        Console.WriteLine($"============================================STARTING INTI PROCESS{getLS().Slot} ===============================");
        if (getLS().Slot != LockedSlot.NA.Slot)
        {
            RemoveAnyItemsInSlot(cp,getLS());
        }
    }
    public virtual bool CanUpdateHotBar(long tick = -1)
    {
        //Get Tick if not set
        
        if (tick == -1)
        {
            tick = CyberUtils.getTick();
        }
        Item i = getHotbarItem_Cooldown();
        if ((LastHotBarCheck + (10)) > tick) return false;
        LastHotBarCheck = tick;
        LastHotBarApproved = tick;
        return true;
    }

    /**
     * Returns a True only if HotBar Should be Changed Back
     */
        public bool updateHotbar(LockedSlot ls, CoolDown c, PowerAbstract p)
        {
            Console.WriteLine($"============================================UPDATING HOTBAR {ls.getSlot()} ===============================");

            if (ls.Equals(LockedSlot.NA)) return false;
            if (c == null || c.isValid(true))
            {
                setPowerAvailable(p);
                Console.WriteLine("ACTIVE POWER");
            }
            else
            {
                Console.WriteLine("UNNNNNNNNACTIVE POWER");
                setPowerUnAvailable(p);
            }
            getPlayer().SendPlayerInventory();
            return true;
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
                addNamedTag(p, FormatActiveItemLore(getActiveItem()), p.getSafeName(), "Active");
//        getPlayer().getInventory().setHeldItemIndex(LS.getSlot());
        }
        public void setPowerUnAvailable(PowerAbstract p)
        {
            p.getPlayer().Inventory.Slots[p.getLS().getSlot()] =
                addNamedTag(p, FormatUnActiveItemLore(getUnActiveItem()), p.getSafeName(), "Idle");
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

        public virtual string[] GetActiveTextFormatSyntax()
        {
            var a = new List<String>();
            a.Add($"{ChatColors.Green} {getDispalyName()}{ChatColors.Green} is ready to be used now!");
            a.Add($" Current XP: {XPManager.getDisplayXP()} of {XPManager.getXP()-XPManager.getNextLevelXP()}");
            a.Add($" Current Level: {XPManager.getLevel()} of {XPManager.getMaxLevel()}");
            return a.ToArray();
        }
        public virtual string[] GetUnActiveTextFormatSyntax()
        {
            var a = new List<String>();
            a.Add($"{ChatColors.Green} {getDispalyName()}{ChatColors.Green} is ready to be used now!");
            a.Add($" Current XP: {XPManager.getDisplayXP()} of {XPManager.getXP()-XPManager.getNextLevelXP()}");
            a.Add($" Current Level: {XPManager.getLevel()} of {XPManager.getMaxLevel()}");
            return a.ToArray();
        }

        public virtual Item FormatActiveItemLore(Item i)
        {
            i.addLore(GetActiveTextFormatSyntax());
            return i;
        }
        public virtual Item FormatUnActiveItemLore(Item i)
        {
            i.addLore(GetUnActiveTextFormatSyntax());
            return i;
        }
        
        public virtual Item getActiveItem()
        {
            return new ItemApple();
            // return new ItemSlimeBall(){Count = 5};
        }
        public virtual  Item getUnActiveItem()
        {
            return new ItemRedstone();
        }

        protected PowerHotBarInt(AdvancedPowerEnum ape, BaseClass b, PowerSettings ps = null ) : base(ape, ps, b)
        {
            
        }
        protected PowerHotBarInt( AdvancedPowerEnum ape, PowerSettings ps = null) : base( ape, ps)
        {
        }

        protected PowerHotBarInt(BaseClass b, AdvancedPowerEnum ape) : base(b, ape)
        {
        }

        /**
         * DEPRECEATED
         */
        protected PowerHotBarInt(BaseClass b) : base(b)
        {
        }

    
    }
}