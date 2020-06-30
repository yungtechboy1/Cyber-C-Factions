using System;
using System.Collections.Generic;
using System.Text;
using CyberCore.Utils;
using fNbt;
using MiNET.Items;
using MiNET.Utils;

namespace CyberCore.Manager.Shop
{
 public class ShopMysqlData {
    private bool Enabled;
    private int ShopID = 0;
    private int ItemID = 0;
    private int ItemDamage = 0;
    private int Price = 0;
    private int SellPrice = 0;
    private int Quantity = 0;
    private String DisplayName = "N/A";
    private byte[] Namedtag = null;
    private bool isValid = true;
    private List<ShopCategory> Category = new List<ShopCategory>();
    public ShopMysqlData(Dictionary<String,Object> rs) {
        try {
            ShopID = (int) rs["ShopID"];
            ItemID =(int) rs["itemid"];
            Enabled = (bool) rs["enabled"];
            ItemDamage = (int)rs["itemdamage"];
            Quantity = (int)rs["quantity"];
            Price = (int)rs["cost"];
            SellPrice =(int) rs["sellprice"];
            String ns = (string) rs["nametag"];
            if (ns != null) Namedtag = Encoding.ASCII.GetBytes(ns);
            DisplayName = (string) rs["DisplayName"];
            try {
                String c = (string) rs["Category"];
                if (c != null && !c.IsNullOrEmpty()) {
                    String line;
                    if (c.Contains(":")) {
                        String[] cc = c.Split(":");
                        if (cc != null) {
                            foreach (String ccc in cc) {
                                ShopCategory? sc = CyberUtils.ShopCategoryFromString(ccc);
                                if (sc != null) Category.Add((ShopCategory)sc);
                            }
                        }
                    } else {
                        ShopCategory? sc = CyberUtils.ShopCategoryFromString(c);
                        if (sc != null) Category.Add((ShopCategory)sc);
                    }
                }
            } catch (Exception e) {
                Console.WriteLine("HAD CATEGGGGGGGGGOOOOOOOOOORRRYY EEEEEEERRRRRRROOOORRROOOO\n\n\n"+e);
            }
            Console.WriteLine("Loading Shop Item >" + ShopID + "|" + ItemID + "|" + Price + DisplayName);
        } catch (Exception e) {
            Console.WriteLine("ERRRRRRRRRR Loading Shop Item !!!!\n\n\n\n"+e);
            isValid = false;
        }
    }

    public bool isEnabled() {
        return Enabled;
    }

    public int getSellPrice() {
        return SellPrice;
    }

    public void setSellPrice(int sellPrice) {
        SellPrice = sellPrice;
    }

    public int getSellPrice(int c) {
        return SellPrice * c;
    }

    public Item getItem() {
        return getItem(false);
    }

    public Item getItem(bool pretty) {
        Item i = ItemFactory.GetItem((short)ItemID, (short)ItemDamage, Quantity);
        if (Namedtag != null) i.setCompoundTag(Namedtag);
        if (!pretty) {
            NbtCompound c = new NbtCompound();
            if (i.getNamedTag() != null) {
                c = i.getNamedTag();
            }
            c.putInt("ShopID", ShopID);
            i.ExtraData = c;
            i.setLore(ChatColors.Aqua + "Purchase Price: " + ChatColors.Green + Price,
                    ChatColors.Yellow + "Sell Price: " + ChatColors.Green + SellPrice,
                    "Click to Buy/Sell!",
                    ChatColors.DarkGray + "SID: " + ShopID,
                    ChatColors.DarkGray + "IID: " + ItemID + ":" + ItemDamage
            );
        }
        return i;
    }

    public Item getItemMainMenu() {
        return getItem();
    }

    public int getItemID() {
        return ItemID;
    }

    public void setItemID(int itemID) {
        ItemID = itemID;
    }

    public int getPrice() {
        return Price;
    }

    public void setPrice(int price) {
        Price = price;
    }

    public int getPrice(int c) {
        return Price * c;
    }

    public String getPrettyString(int q, bool buy) {
        String s = q + " " + getItem(true).getName() + " for ";
        if (buy) s += getPrice() * q;
        else s += getSellPrice() * q;
        return s;
    }

    public int getQuantity() {
        return Quantity;
    }

    public void setQuantity(int quantity) {
        Quantity = quantity;
    }

    public String getDisplayName() {
        return DisplayName;
    }

    public void setDisplayName(String displayName) {
        DisplayName = displayName;
    }

    public List<ShopCategory> getCategory() {
        return Category;
    }
}

}