using System;
using System.Collections.Generic;
using CyberCore.Custom;
using CyberCore.Utils;
using CyberCore.Utils.Data;
using fNbt;
using MiNET.BlockEntities;
using MiNET.Blocks;
using MiNET.Items;
using MiNET.Net;
using MiNET.Utils;

namespace CyberCore.Manager.Shop
{
    public class NewShopInv : CyberGUIInventory
    {
        public NewShopInv() : base(850000,
            new ChestBlockEntity(), 54, new NbtList())
        {
            FillContentsSlots(null, true, true);
            SetPageContents();
        }

        public ShopPageEnum CurrentPageEnum = ShopPageEnum.Main;

        public enum ShopPageEnum
        {
            Main,
            C_Ore = 1,
            C_Weapons = 2,
            C_Armor = 3,
            C_Farming = 4,
            C_Building = 5,
            C_Potions = 6,
            C_Raiding = 7,
            C_Crafting = 8,
            C_NameTag = 9,
            C_Enchanting = 10,

            // C_,
            ConfirmBuy,
            ConfirmSell,
            MoneyBuy,
            MoneySell,
            Confirm
        }

        public void SetContentHotbar()
        {
            var a = new ShopStaticItems(Page);
            SetItem(45, a.Redglass);
            SetItem(48, a.Diamond);
            SetItem(49, a.Netherstar);
            SetItem(50, a.CatagoryChest);
// SetItem(50,a.CatagoryChest);
            SetItem(53, a.Greenglass);
            if (!HasNextPage) SetItem(53, new ItemAir());
        }

        private int Page = 1;

        public bool ItemSlected = false;

        public bool PurchaseScreen = false;

        public override void MakeSelection(int slot, CorePlayer p)
        {
            bool hotbar = false;
            bool cat = false;
            Console.WriteLine("SELECTION MADE AT " + slot);

            if (CurrentPageEnum == ShopPageEnum.Main)
            {
                switch (slot)
                {
                    case 20:
                        //Armor
                        CurrentPageEnum = ShopPageEnum.C_Armor;
                        cat = true;
                        break;
                    case 21:
                        CurrentPageEnum = ShopPageEnum.C_Building;
                        cat = true;
                        break;
                    case 22:
                        CurrentPageEnum = ShopPageEnum.C_Crafting;
                        cat = true;
                        break;
                    case 23:
                        CurrentPageEnum = ShopPageEnum.C_Enchanting;
                        cat = true;
                        break;
                    case 24:
                        CurrentPageEnum = ShopPageEnum.C_Farming;
                        cat = true;
                        break;
                    case 29:
                        CurrentPageEnum = ShopPageEnum.C_Ore;
                        cat = true;
                        break;
                    case 30:
                        CurrentPageEnum = ShopPageEnum.C_Potions;
                        cat = true;
                        break;
                    case 31:
                        CurrentPageEnum = ShopPageEnum.C_Raiding;
                        cat = true;
                        break;
                    case 32:
                        CurrentPageEnum = ShopPageEnum.C_Weapons;
                        cat = true;
                        break;
                    case 33:
                        CurrentPageEnum = ShopPageEnum.C_NameTag;
                        cat = true;
                        break;
                }

                SetPageContents();
            }

            switch (slot)
            {
                case 53:
                    if (!HasNextPage) break;
                    Page++;
                    SetPageContents();
                    hotbar = true;
                    break;
                case 50:
                    Page = 1;
                    CurrentPageEnum = ShopPageEnum.Main;
                    SetPageContents();
                    hotbar = true;
                    break;
                case 45:
                    hotbar = true;
                    if (Page > 1) Page--;
                    else
                    {
                        Page = 1;
                        CurrentPageEnum = ShopPageEnum.Main;
                    }

                    SetPageContents();
                    break;
                case 49:
                    SetPageContents();

                    hotbar = true;
                    break;
            }

            if (!hotbar && !cat)
            {
                switch (CurrentPageEnum)
                {
                    case ShopPageEnum.C_Armor:
                    case ShopPageEnum.C_Building:
                    case ShopPageEnum.C_Crafting:
                    case ShopPageEnum.C_Enchanting:
                    case ShopPageEnum.C_Farming:
                    case ShopPageEnum.C_Ore:
                    case ShopPageEnum.C_Potions:
                    case ShopPageEnum.C_Raiding:
                    case ShopPageEnum.C_Weapons:
                        Item i = GetSlot((byte) slot);
                        if (!i.getNamedTag().getBoolean("CANNOTBUY"))
                        {
                            SetBuyPage(i);
                            CurrentPageEnum = ShopPageEnum.Confirm;
                            ItemSlected = true;
                        }

                        break;
                    default:
                        break;
                }
            }


            SendInv(p);
        }

        private void SetBuyPage(Item item)
        {
            //TODO ASDASDASD
            var si = new ShopStaticItems();
            if (item == null)
            {
                StainedGlass itm = new StainedGlass();
                itm.Color = "gray";
                item = new ItemBlock(itm);
                item.getNamedTag().Add(new NbtByte("CANNOTBUY", 1));
            }

            var iitem = item;

            int yy = 6;
            HasNextPage = false;
            for (int x = 0; x < 9; x++)
            {
                //X
                for (int y = 0; y < yy; y++) //Y
                {
                    int s = y * 9 + x;
                    Console.WriteLine($"#{s} || X:{x} || Y:{y}");

                    if (y == 5)
                    {
                        var b = new RedstoneBlock();
                        var i = new ItemBlock(b);
                        i.setCustomName($"{ChatColors.Red}{ChatFormatting.Bold}Go Back");
                        i.getNamedTag().Add(new NbtByte("CANNOTBUY", 1));
                        SetItem((byte)s,i);
                    }else
                        switch (x)
                    {
                        case 0:
                            SetItem((byte)s,si.AddX64);
                            break;
                        case 1:
                            SetItem((byte)s,si.AddX32);
                            break;
                        case 2:
                            SetItem((byte)s,si.AddX10);
                            break;
                        case 3:
                            SetItem((byte)s,si.AddX1);
                            break;
                        case 4:
                            SetItem((byte)s,item);
                            break;
                        case 5:
                            SetItem((byte)s,si.RmvX1);
                            break;
                        case 6:
                            SetItem((byte)s,si.RmvX10);
                            break;
                        case 7:
                            SetItem((byte)s,si.RmvX32);
                            break;
                        case 8:
                            SetItem((byte)s,si.RmvX64);
                            break;
                        
                    }
                    //
                    //
                    // if (x >= 3)
                    // {
                    //     SetItem(s,si.AddX1N);
                    //     
                    // }else if (x == 4)
                    // {
                    //     
                    // }
                    // else
                    // {
                    //     
                    // }
                    
                    // if (x == 0 || y == 0 || x == 8 || y == yy - 1)
                    // {
                    //     StainedGlass itm = new StainedGlass();
                    //     itm.Color = "Red";
                    //     item = new ItemBlock(itm);
                    //     item.setCustomName($"#{s} || X:{x} || Y:{y}");
                    //     item.getNamedTag().Add(new NbtByte("CANNOTBUY", 0));
                    // }
                    // else
                    // {
                    //     item = (Item) iitem.Clone();
                    //     item.setCustomName($"#{s} || X:{x} || Y:{y}");
                    //     item.getNamedTag().Add(new NbtByte("CANNOTBUY", 0));
                    // }
                    //
                    // if (!(Size <= s)) SetItem((byte) s, item);
                }
            }
        }

        private void SetPageArmor(bool send, CorePlayer p = null)
        {
        }

        public int[] GetIDMetaFromString(string s)
        {
            int id = 0;
            int meta = 0;
            if (s.Contains(";"))
            {
                var a = s.Split(";");
                id = int.Parse(a[0]);
                meta = int.Parse(a[1]);
            }
            else
            {
                id = int.Parse(s);
            }

            return new[] {id, meta};
        }

        public Item GetItemFromIDMeta(int id, int meta)
        {
            Item itm = null;
            if (id <= 255)
            {
                Console.WriteLine($"{id} ||| {meta}");
                var b = CyberUtils.GetBlockFromIdMeta(id, meta);
                itm = new ItemBlock(b);
                itm.setCustomName(b.getName());
            }
            else
            {
                itm = ItemFactory.GetItem((short) id, (short) meta);
                itm.setCustomName(itm.getName());
            }

            return itm;
        }

        public bool HasNextPage = false;

        public void SetPageContents()
        {
            HasNextPage = true;
            int max = 45;
            int to = max * Page;
            int from = to - max;
            FillContentsSlots();
            if (CurrentPageEnum == ShopPageEnum.Main)
            {
                var si = new ShopStaticItems(1);
                int s = 2 * 9;
                int lp = 2;
                int rp = 2;
                int start = s + lp;
                byte slot = (byte) (start);
                SetItem(slot++, si.C_Armor);
                SetItem(slot++, si.C_Building);
                SetItem(slot++, si.C_Crafting);
                SetItem(slot++, si.C_Enchanting);
                SetItem(slot++, si.C_Farming);
                slot = (byte) (start + 9);
                SetItem(slot++, si.C_Ore);
                SetItem(slot++, si.C_Potions);
                SetItem(slot++, si.C_Raiding);
                SetItem(slot++, si.C_Weapons);
                SetItem(slot++, si.C_NameTag);
            }
            else /*if (CurrentPageEnum == ShopPageEnum.C_Armor)*/
            {
                var r = CyberCoreMain.GetInstance().SQL
                    .executeSelect(
                        $"SELECT * FROM `Shop` WHERE `Category` = {(int) CurrentPageEnum} LIMIT {max} OFFSET {from};");
                byte i = 0;
                Console.WriteLine(">>>>>>>>WE GOT BACK " + r.Count);
                if (r.Count < max) HasNextPage = false;
                if (r.Count == 0 && Page != 1)
                {
                    Page = 1;
                    SetItem(53, new ItemAir());
                    return;
                }

                foreach (var d in r)
                {
                    Console.WriteLine("STARTing #" + i + " || " + d["ID"]);
                    int id = 0;
                    int meta = 0;
                    string ids = (string) d["ID"];
                    var a = GetIDMetaFromString(ids);
                    id = a[0];
                    meta = a[1];
                    if (id == 0) continue;

                    Console.WriteLine("ID NOT NULL FOR #" + i + " AND IS " + id);
                    Item itm = GetItemFromIDMeta(id, meta);
                    if (itm == null) continue;
                    Console.WriteLine("ITEM FOUND FOR  #" + i + " AND IS " + itm.Id);
                    var n = itm.getNamedTag();
                    n.Add(new NbtDouble("buy", Decimal.ToDouble((decimal) d["Buy"])));
                    n.Add(new NbtDouble("sell", Decimal.ToDouble((decimal) d["Sell"])));
                    Console.WriteLine("SET CUSTOMNAME FOR ITEM  #" + i + " AND IS " + id);
                    itm.setCustomName(
                        $"{ChatColors.Aqua}=={ChatColors.White}{itm.getCustomName()}{ChatColors.Aqua}==\n" +
                        $"{ChatColors.Green}BUY 1 : ${d["Buy"]}\n" +
                        $"{ChatColors.Yellow}Sell 1 : ${d["Sell"]}\n" +
                        $"{ChatColors.Gray}{d["MID"]}|{id}|{meta}");

                    Console.WriteLine("SET NBT FOR ITEM  #" + i + " AND IS " + id);
                    SetItem(i, itm);
                    i++;
                }
            }
            // else
            // else
            // {
            //     var r = CyberCoreMain.GetInstance().SQL
            //         .executeSelect($"SELECT * FROM `Shop` WHERE `MID` <= {to} AND `MID` > {from};");
            //     byte i = 0;
            //     Console.WriteLine("START");
            //     foreach (var d in r)
            //     {
            //         Console.WriteLine("STARTing #" + i + " || " + d["ID"]);
            //         int id = 0;
            //         int meta = 0;
            //         string ids = (string) d["ID"];
            //         if (ids.Contains(";"))
            //         {
            //             var a = ids.Split(";");
            //             id = int.Parse(a[0]);
            //             meta = int.Parse(a[1]);
            //         }
            //         else
            //         {
            //             id = int.Parse(ids);
            //         }
            //
            //         if (id == 0) continue;
            //
            //         Console.WriteLine("ID NOT NULL FOR #" + i + " AND IS " + id);
            //         Item itm = null;
            //         if (id <= 255)
            //         {
            //             var b = BlockFactory.GetBlockById(id);
            //             itm = new ItemBlock(b);
            //             itm.setCustomName(b.Name);
            //         }
            //         else
            //         {
            //             itm = ItemFactory.GetItem((short) id, (short) meta);
            //             itm.setCustomName(itm.getName());
            //         }
            //
            //         if (itm == null) continue;
            //         Console.WriteLine("ITEM FOUND FOR  #" + i + " AND IS " + itm.Id);
            //
            //         var n = itm.getNamedTag();
            //
            //         Console.WriteLine("SET BUY FOR ITEM  #" + i + $" AND IS {d["Buy"]} AND TYPE {d["Buy"].GetType()}");
            //         n.Add(new NbtDouble("buy", Decimal.ToDouble((decimal) d["Buy"])));
            //
            //         Console.WriteLine("SET SELL FOR ITEM  #" + i + " AND IS " + id);
            //         n.Add(new NbtDouble("sell", Decimal.ToDouble((decimal) d["Sell"])));
            //
            //         Console.WriteLine("SET CUSTOMNAME FOR ITEM  #" + i + " AND IS " + id);
            //         itm.setCustomName(
            //             $"{ChatColors.Aqua}=={ChatColors.White}{itm.getCustomName()}{ChatColors.Aqua}==\n" +
            //             $"{ChatColors.Green}BUY 1 : ${d["Buy"]}\n" +
            //             $"{ChatColors.Yellow}Sell 1 : ${d["Sell"]}");
            //
            //         Console.WriteLine("SET NBT FOR ITEM  #" + i + " AND IS " + id);
            //         SetItem(i, itm);
            //         i++;
            //     }
            // }

            SetContentHotbar();
        }
    }
}