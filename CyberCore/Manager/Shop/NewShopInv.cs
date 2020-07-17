﻿using System;
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

        public override void MakeSelection(int slot, CorePlayer p)
        {
            Console.WriteLine("SELECTION MADE AT " + slot);

            if (CurrentPageEnum == ShopPageEnum.Main)
            {
                switch (slot)
                {
                    case 20:
                        //Armor
                        CurrentPageEnum = ShopPageEnum.C_Armor;
                        break;
                    case 21:
                        CurrentPageEnum = ShopPageEnum.C_Building;
                        break;
                    case 22:
                        CurrentPageEnum = ShopPageEnum.C_Crafting;
                        break;
                    case 23:
                        CurrentPageEnum = ShopPageEnum.C_Enchanting;
                        break;
                    case 24:
                        CurrentPageEnum = ShopPageEnum.C_Farming;
                        break;
                    case 29:
                        CurrentPageEnum = ShopPageEnum.C_Ore;
                        break;
                    case 30:
                        CurrentPageEnum = ShopPageEnum.C_Potions;
                        break;
                    case 31:
                        CurrentPageEnum = ShopPageEnum.C_Raiding;
                        break;
                    case 32:
                        CurrentPageEnum = ShopPageEnum.C_Weapons;
                        break;
                    case 33:
                        CurrentPageEnum = ShopPageEnum.C_NameTag;
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
                    break;
                case 50:
                    Page = 1;
                    CurrentPageEnum = ShopPageEnum.Main;
                    SetPageContents();
                    break;
                case 45:
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

                    break;
            }

            SendInv(p);
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