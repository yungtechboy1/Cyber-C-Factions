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
            // for (byte i = 0; i < 54; i++)
            // {
            //     StainedGlass itm = new StainedGlass();
            //     itm.Color = "gray";
            //     SetItem(i, new ItemBlock(itm));
            // }

            var a = new ShopStaticItems();
            SetItem(45, a.Redglass);
            SetItem(48, a.Diamond);
            SetItem(49, a.Netherstar);
            SetItem(50, a.CatagoryChest);
// SetItem(50,a.CatagoryChest);
            SetItem(53, a.Greenglass);
        }

        private int Page = 0;

        public override void MakeSelection(int slot, CorePlayer p)
        {
            Console.WriteLine("SELECTION MADE AT "+slot);
            if (slot == 53)
            {
                Page++;
                
            }
        }

        public void SetPageContents()
        {
            int max = 45;
            int to = max * Page;
            int from = to - max;
            FillContentsSlots();
            var r = CyberCoreMain.GetInstance().SQL
                .executeSelect($"SELECT * FROM 'Shop' WHERE `MID` <= {to} AND `MID` > {from}");
            byte i = 0;
            foreach (var d in r)
            {
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
                Item itm = null;
                if (id <= 255)
                {
                    itm = new ItemBlock(BlockFactory.GetBlockById(id));
                }
                else
                {
                    itm = ItemFactory.GetItem((short) id, (short) meta);
                }

                if (itm == null) continue;
                var n = itm.getNamedTag();
                n.Add(new NbtDouble("buy", Double.Parse((String) d["Buy"])));
                n.Add(new NbtDouble("sell", Double.Parse((String) d["Sell"])));
                itm.setCustomName($"{ChatColors.Aqua}=={itm.getName()}==\n" +
                                  $"{ChatColors.Green}BUY 1 : ${d["Buy"]}" +
                                  $"{ChatColors.Yellow}Sell 1 : ${d["Sell"]}");
                SetItem(i, itm);
                i++;
            }
        }
    }
}