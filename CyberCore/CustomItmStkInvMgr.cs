using System;
using System.Collections.Generic;
using System.Linq;
using MiNET;
using MiNET.Items;
using MiNET.Utils;

namespace CyberCore
{
    public class CustomItmStkInvMgr : ItemStackInventoryManager
    {
        private Player P;

        public CustomItmStkInvMgr(Player player) : base(player)
        {
            P = player;
        }

        public override List<StackResponseContainerInfo> HandleItemStackActions(int requestId,
            ItemStackActionList actions)
        {
            bool skip = false;
            List<StackResponseContainerInfo> responseContainerInfoList = new List<StackResponseContainerInfo>();
            uint num = 0;
            foreach (ItemStackAction action1 in (List<ItemStackAction>) actions)
            {
                Console.WriteLine($"AN ACTION WAS CALLED WITH {action1}");
                switch (action1)
                {
                    case TakeAction action:
                        if (action.Source.ContainerId == 7)
                        {
                            //Must be chest Inv!
                            if (P is CorePlayer CP)
                            {
                                var si = CP.ShopInv;
                                if (si != null)
                                {
                                    skip = true;
                                    Console.WriteLine(action.Destination.ContainerId + " |||||||||||1111||||||| " +
                                                      action.Destination.Slot);
                                    si.MakeSelection(action.Source.Slot, CP);
                                    continue;
                                }
                            }
                        }
                        continue;
                }
            }

            if (skip) return new List<StackResponseContainerInfo>();
            return base.HandleItemStackActions(requestId, actions);
// return responseContainerInfoList;
        }

        public void PTA(
            TakeAction action,
            List<StackResponseContainerInfo> stackResponses)
        {
            byte count = action.Count;
            StackRequestSlotInfo source = action.Source;
            StackRequestSlotInfo destination = action.Destination;
            //CHECK IF SOUCE IS 7
            Item obj1 = this.GetContainerItem1((int) source.ContainerId, (int) source.Slot);
            Console.WriteLine((object) string.Format("Take {0}", (object) obj1));
            Item obj2;
            if ((int) obj1.Count == (int) count || (int) obj1.Count - (int) count <= 0)
            {
                Console.WriteLine("THIS WAS CALLED 1");
                obj2 = obj1;
                obj1 = (Item) new ItemAir();
                obj1.UniqueId = 0;
                this.SetContainerItem1((int) source.ContainerId, (int) source.Slot, obj1);
            }
            else
            {
                Console.WriteLine("THIS WAS CALLED 22");
                obj2 = (Item) obj1.Clone();
                obj1.Count -= count;
                obj2.Count = count;
                obj2.UniqueId = Environment.TickCount;
            }

            Console.WriteLine($"THE CONTAINER ID IS {destination.ContainerId} AND SOURCE IS {source.ContainerId}");

            this.SetContainerItem1((int) destination.ContainerId, (int) destination.Slot, obj2);
            if (source.ContainerId == (byte) 21 || source.ContainerId == (byte) 22)
            {
                if (!(this.GetContainerItem1(21, 14) is ItemAir) && !(this.GetContainerItem1(22, 15) is ItemAir))
                    Enchantment.SendEnchantments(this.P, this.GetContainerItem1(21, 14));
                else
                    Enchantment.SendEmptyEnchantments(this.P);
            }

            stackResponses.Add(new StackResponseContainerInfo()
            {
                ContainerId = source.ContainerId,
                Slots = new List<StackResponseSlotInfo>()
                {
                    new StackResponseSlotInfo()
                    {
                        Count = obj1.Count,
                        Slot = source.Slot,
                        HotbarSlot = source.Slot,
                        StackNetworkId = obj1.UniqueId
                    }
                }
            });
            stackResponses.Add(new StackResponseContainerInfo()
            {
                ContainerId = destination.ContainerId,
                Slots = new List<StackResponseSlotInfo>()
                {
                    new StackResponseSlotInfo()
                    {
                        Count = obj2.Count,
                        Slot = destination.Slot,
                        HotbarSlot = destination.Slot,
                        StackNetworkId = obj2.UniqueId
                    }
                }
            });
        }


        private Item GetContainerItem1(int containerId, int slot)
        {
            if (this.P.UsingAnvil && containerId < 3)
                containerId = 13;
            Item obj1 = (Item) null;
            switch (containerId)
            {
                case 6:
                    Item obj2;
                    switch (slot)
                    {
                        case 0:
                            obj2 = this.P.Inventory.Helmet;
                            break;
                        case 1:
                            obj2 = this.P.Inventory.Chest;
                            break;
                        case 2:
                            obj2 = this.P.Inventory.Leggings;
                            break;
                        case 3:
                            obj2 = this.P.Inventory.Boots;
                            break;
                        default:
                            obj2 = (Item) null;
                            break;
                    }

                    obj1 = obj2;
                    break;
                case 7:
                    //I ONLY NEEDZ DIS
                    Console.WriteLine("CASE 7 WAS CALLED OH NO!!!!!");
                    // if (this.P._openInventory is Inventory openInventory)
                    // {
                    //   obj1 = openInventory.GetSlot((byte) slot);
                    //   break;
                    // }
                    break;
                case 12:
                case 27:
                case 28:
                    obj1 = this.P.Inventory.Slots[slot];
                    break;
                case 13:
                case 21:
                case 22:
                case 41:
                case 58:
                case 59:
                    obj1 = this.P.Inventory.UiInventory.Slots[slot];
                    break;
                case 33:
                    obj1 = this.P.Inventory.OffHand;
                    break;
                default:
                    Console.WriteLine((object) string.Format("Unknown containerId: {0}", (object) containerId));
                    break;
            }

            return obj1;
        }

        private void SetContainerItem1(int containerId, int slot, Item item)
        {
            if (this.P.UsingAnvil && containerId < 3)
                containerId = 13;
            switch (containerId)
            {
                case 6:
                    switch (slot)
                    {
                        case 0:
                            this.P.Inventory.Helmet = item;
                            return;
                        case 1:
                            this.P.Inventory.Chest = item;
                            return;
                        case 2:
                            this.P.Inventory.Leggings = item;
                            return;
                        case 3:
                            this.P.Inventory.Boots = item;
                            return;
                        default:
                            return;
                    }
                case 7:
                    Console.WriteLine("OH NO CASE 77 WAS CALLELDDDDDDDDD");
                    // if (!(this.P._openInventory is Inventory openInventory))
                    //   break;
                    // openInventory.SetSlot(this.P, (byte) slot, item);
                    break;
                case 12:
                case 27:
                case 28:
                    this.P.Inventory.Slots[slot] = item;
                    break;
                case 13:
                case 21:
                case 22:
                case 41:
                case 58:
                case 59:
                    this.P.Inventory.UiInventory.Slots[slot] = item;
                    break;
                case 33:
                    this.P.Inventory.OffHand = item;
                    break;
                default:
                    Console.WriteLine((object) string.Format("Unknown containerId: {0}", (object) containerId));
                    break;
            }
        }
    }
}