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
            FillContentsSlots();
            setContentHotbar();
            SendPage();
        }

        public void SendPage()
        {
            switch (CurrentPageEnum)
            {
                case ShopPageEnum.Main:
                    int s = 2 * 9;
                    int lp = 2;
                    int rp = 2;
                    
                    
                    
                    
                    break;
                //Main,
                    // C_Ore,
                // C_Weapons,
                // C_Armor,
                // C_Farming,
                // C_Building,
                // C_Potions,
                // C_Raiding,
                // C_Crafting,
                // C_NameTag,
                // C_Enchating,
                // // C_,
                // ConfirmBuy,
                // ConfirmSell,
                // MoneyBuy,
                // MoneySell,
            }
            
        }

        public ShopPageEnum CurrentPageEnum = ShopPageEnum.Main;
        
        public enum ShopPageEnum
        {
            Main,
            C_Ore,
            C_Weapons,
            C_Armor,
            C_Farming,
            C_Building,
            C_Potions,
            C_Raiding,
            C_Crafting,
            C_NameTag,
            C_Enchating,
            // C_,
            ConfirmBuy,
            ConfirmSell,
            MoneyBuy,
            MoneySell,
            
        }
        
        public void setContentHotbar()
        {
            var a = new ShopStaticItems(Page);
            SetItem(45, a.Redglass);
            SetItem(48, a.Diamond);
            SetItem(49, a.Netherstar);
            SetItem(50, a.CatagoryChest);
// SetItem(50,a.CatagoryChest);
            SetItem(53, a.Greenglass);
        }

        private int Page = 1;

        public override void MakeSelection(int slot, CorePlayer p)
        {
            Console.WriteLine("SELECTION MADE AT " + slot);
            switch (slot)
            {
             case 53:
                Page++;
                SetPageContents();
                SendInv(p);
                break;
            case 45:
                if(Page > 1)Page--;
                SetPageContents();
                SendInv(p);
                break;
            case 49:
                SetPageContents();
                SendInv(p);

                break;
           
        }
        }

        public void SetPageContents()
        {
            setContentHotbar();
            int max = 45;
            int to = max * Page;
            int from = to - max;
            FillContentsSlots();
            var r = CyberCoreMain.GetInstance().SQL
                .executeSelect($"SELECT * FROM `Shop` WHERE `MID` <= {to} AND `MID` > {from};");
            byte i = 0;
            Console.WriteLine("START");
            foreach (var d in r)
            {
                Console.WriteLine("STARTing #" + i + " || " + d["ID"]);
                int id = 0;
                int meta = 0;
                string ids = (string) d["ID"];
                if (ids.Contains(";"))
                {
                    var a = ids.Split(";");
                    id = int.Parse(a[0]);
                    meta = int.Parse(a[1]);
                }
                else
                {
                    id = int.Parse(ids);
                }

                if (id == 0) continue;

                Console.WriteLine("ID NOT NULL FOR #" + i + " AND IS " + id);
                Item itm = null;
                if (id <= 255)
                {
                    var b = BlockFactory.GetBlockById(id);
                    itm = new ItemBlock(b);
                    itm.setCustomName(b.Name);
                }
                else
                {
                    itm = ItemFactory.GetItem((short) id, (short) meta);
                    itm.setCustomName(itm.getName());
                }

                if (itm == null) continue;
                Console.WriteLine("ITEM FOUND FOR  #" + i + " AND IS " + itm.Id);

                var n = itm.getNamedTag();

                Console.WriteLine("SET BUY FOR ITEM  #" + i + $" AND IS {d["Buy"]} AND TYPE {d["Buy"].GetType()}");
                n.Add(new NbtDouble("buy", Decimal.ToDouble((decimal) d["Buy"])));

                Console.WriteLine("SET SELL FOR ITEM  #" + i + " AND IS " + id);
                n.Add(new NbtDouble("sell", Decimal.ToDouble((decimal) d["Sell"])));

                Console.WriteLine("SET CUSTOMNAME FOR ITEM  #" + i + " AND IS " + id);
                itm.setCustomName($"{ChatColors.Aqua}=={ChatColors.White}{itm.getCustomName()}{ChatColors.Aqua}==\n" +
                                  $"{ChatColors.Green}BUY 1 : ${d["Buy"]}\n" +
                                  $"{ChatColors.Yellow}Sell 1 : ${d["Sell"]}");

                Console.WriteLine("SET NBT FOR ITEM  #" + i + " AND IS " + id);
                SetItem(i, itm);
                i++;
            }
        }
    }
}