using System;
using System.Collections.Generic;
using CyberCore.Custom;
using CyberCore.Utils;
using CyberCore.Utils.Data;
using fNbt;
using log4net.Util.TypeConverters;
using MiNET.BlockEntities;
using MiNET.Blocks;
using MiNET.Items;
using MiNET.Net;
using MiNET.Utils;

namespace CyberCore.Manager.Shop
{
    public class NewShopInv : CyberGUIInventory
    {
        private CorePlayer P;

        public NewShopInv(CorePlayer p) : base(850000,
            new ChestBlockEntity(), 54, new NbtList())
        {
            P = p;
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
            Confirm,
            Purchased
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

        public Item ItemSelected = null;

        public bool PurchaseScreen = false;

        public override void MakeSelection(int slot, CorePlayer p)
        {
            int x = slot % 9;
            int y = (int) Math.Floor((double) (slot / 9));

            Console.WriteLine($"XXXXXXXXXXXXXX{x}    YYYYYYYYYYYYYYYYYYYYYYYY{y}");
            bool hotbar = false;
            bool cat = false;
            Console.WriteLine("SELECTION MADE AT " + slot);

            Item i = GetSlot((byte) slot);
            switch (CurrentPageEnum)
            {
                case ShopPageEnum.Main:
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
                    break;
                case ShopPageEnum.C_Armor:
                case ShopPageEnum.C_Building:
                case ShopPageEnum.C_Crafting:
                case ShopPageEnum.C_Enchanting:
                case ShopPageEnum.C_Farming:
                case ShopPageEnum.C_Ore:
                case ShopPageEnum.C_Potions:
                case ShopPageEnum.C_Raiding:
                case ShopPageEnum.C_Weapons:
                    if (!i.getNamedTag().getBoolean("CANNOTBUY"))
                    {
                        ItemSelected = i;
                        SetBuyPage(i);
                        CurrentPageEnum = ShopPageEnum.Confirm;
                    }
                    else
                    {
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
                    }

                    break;
                case ShopPageEnum.Purchased:
                    ItemSelected = null;
                    PurchaseScreen = false;
                    CurrentPageEnum = ShopPageEnum.Main;
                    SetPageContents();
                    break;
                case ShopPageEnum.Confirm:
                    if (y == 5)
                    {
                        ItemSelected = null;
                        PurchaseScreen = false;
                        CurrentPageEnum = ShopPageEnum.Main;
                        SetPageContents();
                    }
                    else if (ItemSelected != null)
                    {
                        PurchaseScreen = true;

                        if (i.getNamedTag().Contains("ADD"))
                        {
                            StainedGlassPane bb = new StainedGlassPane();
                            bb.Color = "red";
                            Item fi = new ItemBlock(bb);
                            // p.getMoney()
                            double aa = ((NbtDouble)i.getNamedTag()["PRICE"]).Value;
                            fi.setCustomName(ChatColors.Red+"Error could not complete purchase for $" + aa);
                            if (P.Inventory.HasEmptySlot())
                            {
                                Console.WriteLine("HAS FREE SLOT");
                                if (p.TakeMoney(aa))
                                {
                                    var si = GetSlot((byte) slot);
                                    int a = ((NbtInt)si.ExtraData["ADD"]).Value;
                                    Console.WriteLine("HAS MONEY");
                                    bb.Color = "Green";
                                    fi = new ItemBlock(bb);
                                    fi.setCustomName(ChatColors.Green+"===|Purchase was successful |===\n" +
                                                     $"===| You Bought {a} |===\n" +
                                                     "===| for $" + aa+" |===\n" +
                                                     $"==| You Have {p.getMoney()} |==");
                                    Item pi = (Item) ItemSelected.Clone();
                                    pi = pi.ClearShopTags();
                                    pi.Count = (byte) a;
                                    p.Inventory.AddItem(pi,true);
                                    p.SendMessage("ITEM PURCHASEDD!!! \n"+pi);
                                }else
                                    Console.WriteLine("HAS NO MONEY");
                                
                            }
                            else
                            {
                                Console.WriteLine("NO FREE SLOT");
                                fi.setCustomName(ChatColors.Red +
                                                 "Error You Have no free slots in your inventory!\nPurchase Canceled");
                            }

                            CurrentPageEnum = ShopPageEnum.Purchased;
                            // Console.WriteLine();
                            FillContentsSlots(fi);
                        }
                        else if (i.getNamedTag().Contains("RMV"))
                        {
                            double ar = ((NbtDouble)i.getNamedTag()["PRICE"]).Value;
                            Item pi = (Item) ItemSelected.Clone();
                            pi = pi.ClearShopTags();
                            int m = P.Inventory.GetCountOfItem(pi);
                            pi.Count = (byte) m;
                            var si = GetSlot((byte) slot);
                            int a = ((NbtInt)si.ExtraData["RMV"]).Value;
                            
                            
                            
                            
                            
                            StainedGlassPane bb = new StainedGlassPane();
                            bb.Color = "red";
                            Item fi = new ItemBlock(bb);
                            // p.getMoney()
                            double aa = ((NbtDouble)i.getNamedTag()["PRICE"]).Value;
                            fi.setCustomName(ChatColors.Red+"Error could not complete Sell process for $" + aa);
                            //Has more than or equal items in Inv than Required
                                Console.WriteLine($"HAS {a} OF(<=) {m}");
                            if (a <= m && p.Inventory.TakeItem(pi))
                            {
                                    Console.WriteLine("Has TAKEN ITEMS"+pi);
                                    bb.Color = "Green";
                                    fi = new ItemBlock(bb);
                                    fi.setCustomName(ChatColors.Green+"===|Sale was successful |===\n" +
                                                     $"===| You Sold {a} |===\n" +
                                                     "===| for $" + aa+" |===");
                                    p.AddMoney(aa);
                                    p.SendMessage($"SOLDDD {a} ITEM(s) FOR {aa}!!! \n"+pi);
                                
                            }
                            else
                            {
                                Console.WriteLine($"PLAYER HAS ONLY {m} of "+pi);
                                fi.setCustomName(ChatColors.Red +
                                                 $"You do not have enough of {pi.getName()}\n" +
                                                 $"You need {a} but only have{m}\n" +
                                                 $"Purchase Canceled");
                            }

                            CurrentPageEnum = ShopPageEnum.Purchased;
                            // Console.WriteLine();
                            FillContentsSlots(fi);
                            
                        }
                        else
                        {
                            Console.WriteLine("AHHHHHHHHHH RMV AND ADD WAS NOT FOUND!!!");
                        }
                    }

                    break;
            }

            SendInv(p);
        }

        private void SetBuyPage(Item item)
        {
            //TODO ASDASDASD
            var si = new ShopStaticItems();
            // if (item == null)
            // {
            //     StainedGlass itm = new StainedGlass();
            //     itm.Color = "gray";
            //     item = new ItemBlock(itm);
            //     item.getNamedTag().Add(new NbtByte("CANNOTBUY", 1));
            // }

            // var iitem = item;

            double money = P.getMoney();
            double buyprice = ((NbtDouble) item.getNamedTag()["buy"]).Value;
            double sellprice = ((NbtDouble) item.getNamedTag()["sell"]).Value;
            // Item i;
            int m;
            int yy = 6;
            HasNextPage = false;

            Dictionary<int, Item> d = new Dictionary<int, Item>();
            Dictionary<int, Item> dn = new Dictionary<int, Item>();
            int dd = 0;
            d[dd++] = (Item) si.RmvX64.Clone();
            d[dd++] = (Item) si.RmvX32.Clone();
            d[dd++] = (Item) si.RmvX10.Clone();
            d[dd++] = (Item) si.RmvX1.Clone();
            d[dd++] = (Item) item.Clone();
            d[dd++] = (Item) si.AddX1.Clone();
            d[dd++] = (Item) si.AddX10.Clone();
            d[dd++] = (Item) si.AddX32.Clone();
            d[dd++] = (Item) si.AddX64.Clone();
            dd = 0;
            dn[dd++] = (Item) si.RmvX64N.Clone();
            dn[dd++] = (Item) si.RmvX32N.Clone();
            dn[dd++] = (Item) si.RmvX10N.Clone();
            dn[dd++] = (Item) si.RmvX1N.Clone();
            dn[dd++] = (Item) item.Clone();
            dn[dd++] = (Item) si.AddX1N.Clone();
            dn[dd++] = (Item) si.AddX10N.Clone();
            dn[dd++] = (Item) si.AddX32N.Clone();
            dn[dd++] = (Item) si.AddX64N.Clone();
            for (int x = 0; x < 9; x++)
            {
                //X
                for (int y = 0; y < yy; y++) //Y
                {
                    int s = y * 9 + x;
                    // Console.WriteLine($"#{s} || X:{x} || Y:{y}");

                    if (y == 5)
                    {
                        var b = new RedstoneBlock();
                        var ii = new ItemBlock(b);
                        ii.setCustomName($"{ChatColors.Red}{ChatFormatting.Bold}Go Back");
                        ii.getNamedTag().Add(new NbtByte("CANNOTBUY", 1));
                        SetItem((byte) s, ii);
                    }
                    else
                    {
                        // Console.WriteLine("MAybe THID");
                        var i = (Item) d[x].Clone();
                        // Console.WriteLine("MAybe THID");
                        var ni = (Item) dn[x].Clone();
                        // Console.WriteLine("MAybe THID");
                        switch (x)
                        {
                            //RMV OR SELL
                            case 0:
                            case 1:
                            case 2:
                            case 3:
                                Console.WriteLine("SELLLL");
                                Console.WriteLine($"OK SO {i.getNamedTag().Contains("PRICE")}");
                                m = ((NbtInt) i.getNamedTag()["RMV"]).Value;
                                Console.WriteLine("aaaaaaa");
                                Console.WriteLine("EEEEEEEEEEEEE" + ItemSelected);
                                Console.WriteLine("EEEEEEEEEEEEE" + ItemSelected == null);

                                // Item ii = (Item) item.Clone();
                                // Console.WriteLine("EEEEEEEEEEEEE" + ii == null);
                                int countOfItem = P.Inventory.GetCountOfItem(item);
                                Console.WriteLine("EEEEEEEEEEEEE" + countOfItem);
                                if (countOfItem != 0)
                                {
                                    // Item pi = P.Inventory.Slots[iin];
                                    //
                                    // Console.WriteLine("EEEEEEEEEEEEE" + pi);
                                    // Console.WriteLine("EEEEEEEEEEEEE" + pi.Count);
                                    if (countOfItem >= m)
                                    {
                                        Console.WriteLine("EE1111111333333333331111EEEEEEEEEEE");
                                        i.addToCustomName(
                                            $"{ChatColors.Green}Sell {m} {item.getName()} For {sellprice * m}\n" +
                                            $"{ChatColors.Yellow}You have {money}");
                                    }
                                    else
                                    {
                                        Console.WriteLine("EE111111133333333332222222222222111113333333331111EEEEEEEEEEE");
                                        i = ni;
                                        i.addToCustomName(
                                            $"{ChatColors.Red}Can Not Sell {m} {item.getName()} For {sellprice * m}\n" +
                                            $"You only have {countOfItem} but need {m} to sell!\n" +
                                            $"Obtain More and Try Again!\n" +
                                            $"{ChatColors.Yellow}You have {money}");
                                        
                                        
                                    }
                                }
                                else
                                {
                                    i = ni;
                                    Console.WriteLine("EE11111111111EEEEEEEEEEE");
                                    // i.getNamedTag().Add(new NbtByte("CANNOTBUY", 1));
                                    i.addToCustomName(
                                        $"{ChatColors.Red}Can Not Sell {m} {item.getName()} For {sellprice * m}\n" +
                                        $"You do not have any of this block!\n" +
                                        $"{ChatColors.Yellow}You have {money}");
                                }

                                Console.WriteLine("PRICE HAS BEEN CALLED!!!!!");
                                if (i.getNamedTag().Contains("PRICE")) i.getNamedTag().Remove("PRICE");
                                i.getNamedTag().Add(new NbtDouble("PRICE", sellprice * m));
                                SetItem((byte) s, i);
                                break;
                                break;
                            //ADD OR BUY
                            case 5:
                            case 6:
                            case 7:
                            case 8:
                                // var i = (Item) d[x].Clone();
                                Console.WriteLine("BUYYYYYYYY");
                                Console.WriteLine($"OK SO {i.getNamedTag().Contains("PRICE")}");
                                m = ((NbtInt) i.getNamedTag()["ADD"]).Value;
                                Console.WriteLine("EEEEEEEEBBBEEEEE");
                                if (money < m * buyprice)
                                {
                                    i = ni;
                                    Console.WriteLine("EEEEE33333BBBBB333333EEEEEEEE");
                                    if (i.getNamedTag().Contains("PRICE")) i.getNamedTag().Remove("PRICE");
                                    // i.getNamedTag().Add(new NbtByte("CANNOTBUY", 1));
                                    Console.WriteLine($"OK SO  NNBBB {i.getNamedTag().Contains("PRICE")}");
                                    i.addToCustomName(
                                        $"{ChatColors.Red}Can Not Buy {m} {item.getName()} For {buyprice * m}\n" +
                                        $"You have Only {money}");
                                }
                                else
                                {
                                    Console.WriteLine("EE1111111111BBBBBBBB1EEEEEEEEEEE");
                                    i.addToCustomName(
                                        $"{ChatColors.Green}Buy {m} {item.getName()} For {buyprice * m}\n" +
                                        $"{ChatColors.Yellow}You have {money}");
                                }

                                Console.WriteLine("PRICE BBBBBHAS BEEN CALLED!!!!!");
                                if (i.getNamedTag().Contains("PRICE")) i.getNamedTag().Remove("PRICE");
                                i.getNamedTag().Add(new NbtDouble("PRICE", buyprice * m));
                                SetItem((byte) s, i);
                                break;
                            // case 1:
                            //     i = si.AddX32;
                            //     m = ((NbtInt) i.getNamedTag()["ADD"]).Value;
                            //     i.addToCustomName($"Purchase {m} {item.getName()} For {buyprice * m}");
                            //     i.getNamedTag().Add(new NbtDouble("PRICE", buyprice * m));
                            //     SetItem((byte) s, i);
                            //     break;
                            // case 2:
                            //     i = si.AddX10;
                            //     m = ((NbtInt) i.getNamedTag()["ADD"]).Value;
                            //     i.addToCustomName($"Purchase {m} {item.getName()} For {buyprice * m}");
                            //     i.getNamedTag().Add(new NbtDouble("PRICE", buyprice * m));
                            //     SetItem((byte) s, i);
                            //     break;
                            // case 3:
                            //     i = si.AddX1;
                            //     m = ((NbtInt) i.getNamedTag()["ADD"]).Value;
                            //     i.addToCustomName($"Purchase {m} {item.getName()} For {buyprice * m}");
                            //     i.getNamedTag().Add(new NbtDouble("PRICE", buyprice * m));
                            //     SetItem((byte) s, i);
                            //     break;
                            case 4:
                                Console.WriteLine($"SOOO I IS " + i);
                                Console.WriteLine($"SOOO ITEM IS " + item);
                                // Console.WriteLine($"SOOO IITEM IS " + iitem);
                                SetItem((byte) s, item);
                                break;
                            // case 5:
                            //     i = si.RmvX1;
                            //     m = ((NbtInt) i.getNamedTag()["RMV"]).Value;
                            //     i.addToCustomName($"Purchase {m} {item.getName()} For {buyprice * m}");
                            //     i.getNamedTag().Add(new NbtDouble("PRICE", buyprice * m));
                            //     SetItem((byte) s, i);
                            //     break;
                            // case 6:
                            //     i = si.AddX10;
                            //     m = ((NbtInt) i.getNamedTag()["RMV"]).Value;
                            //     i.addToCustomName($"Purchase {m} {item.getName()} For {buyprice * m}");
                            //     i.getNamedTag().Add(new NbtDouble("PRICE", buyprice * m));
                            //     SetItem((byte) s, i);
                            //     break;
                            // case 7:
                            //     i = si.AddX32;
                            //     m = ((NbtInt) i.getNamedTag()["RMV"]).Value;
                            //     i.addToCustomName($"Purchase {m} {item.getName()} For {buyprice * m}");
                            //     i.getNamedTag().Add(new NbtDouble("PRICE", buyprice * m));
                            //     SetItem((byte) s, i);
                            //     break;
                            // case 8:
                            //     i = si.AddX64;
                            //     m = ((NbtInt) i.getNamedTag()["RMV"]).Value;
                            //     i.addToCustomName($"Purchase {m} {item.getName()} For {buyprice * m}");
                            //     i.getNamedTag().Add(new NbtDouble("PRICE", buyprice * m));
                            //     SetItem((byte) s, i);
                            //     break;
                        }
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