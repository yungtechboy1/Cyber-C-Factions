using System;
using System.Collections.Generic;
using CyberCore.Utils;
using fNbt;
using MiNET.Items;
using MiNET.Utils;

namespace CyberCore.Manager.AuctionHouse
{
    public class AuctionItemData
    {
        private readonly int Cost;
        private readonly Item item;
        private readonly int masterid = -1;
        private readonly string Soldby; //UUID
        private readonly string Soldbyn; //Display Name

        public AuctionItemData(Item i, int cost, CorePlayer seller)
        {
            item = i;
            Cost = cost;
            Soldby = seller.ClientUuid.ToString();
            Soldbyn = seller.DisplayName;
        }

        // public AuctionItemData(ResultSet rs) throws Exception {
        //     int item_id = rs.getInt("item-id");
        //     int item_meta = rs.getInt("item-meta");
        //     int item_count = rs.getInt("item-count");
        //     byte[] namedtag = rs.getString("namedtag").getBytes();
        //     Cost = rs.getInt("cost");
        //     String soldbyn = rs.getString("soldbyn");
        //     String soldby = rs.getString("soldby");
        //     int mid = rs.getInt("master_id");
        //
        //     item = Item.get(item_id, item_meta, item_count);
        //     item.setCompoundTag(namedtag);
        //
        //     Soldby = soldby;
        //     Soldbyn = soldbyn;
        //     masterid = mid;
        //
        // }

        public AuctionItemData(Dictionary<string, object> v)
        {
            var nt = (byte[]) v["namedtag"];
            item = ItemFactory.GetItem((short)(int) v["item-id"], (short)(int) v["item-meta"], (int) v["item-count"]);
            if (nt != null && nt.Length != 0)
                try
                {
                    var a = new NbtFile();
                    a.LoadFromBuffer(nt, 0, nt.Length, NbtCompression.ZLib);
                    item.ExtraData = (NbtCompound) a.RootTag;
                }
                catch (Exception e)
                {
                    CyberCoreMain.Log.Error("AID >>> ERROR TRING TO FORMAT NAMEDTAG!!!!");
                }

            Cost = (int) v["cost"];
            Soldby = (string) v["soldby"];
            Soldbyn = (string) v["soldbyn"];
            masterid = (int) v["master_id"];
        }

        public Item MakePretty()
        {
            var titem = (Item) item.Clone();
            var tag = new NbtCompound();
            if (titem.ExtraData.HasValue) tag = titem.ExtraData;
            if (!tag.Contains("ah-data")) tag.Add(new NbtCompound("ah-data"));

            var tt = (NbtCompound) tag.Get("ah-data");
            if (tt == null)
            {
                CyberCoreMain.Log.Error("AID>>> TT WAS NULL");
                return item;
            }
            if (titem.getCustomName() != "" && tag.Contains("ah-data") && tag.Get("ah-data") is
                NbtCompound)
                tt.Add(new NbtString("Name", titem.getCustomName()));

            tt.Add(new NbtInt("masterid", masterid));
            tt.Add(new NbtInt("cost", Cost));
            tt.Add(new NbtString("soldbyn", Soldbyn));
            ;
            tt.Add(new NbtString("soldby", Soldby));

            titem.ExtraData = tag;


            var cn = titem.getCustomName();

            if (cn.equalsIgnoreCase("")) cn = titem.getName();

            cn += ChatFormatting.Reset + "\n" + ChatColors.Aqua +
                  "-------------" + ChatFormatting.Reset + "\n" +
                  ChatColors.Green + "$" + Cost + ChatFormatting.Reset + "\n" +
                  ChatColors.Gold+ "Sold By: " + Soldbyn
                // + TextFormat.RESET + "\n" +TextFormat.BLACK+"{#"+id;
                ;

            titem.setCustomName(cn);
            return (Item) titem.Clone();
        }

        public void AddToDB()
        {
        }

        public Item getItem()
        {
            return item;
        }

        public int getCost()
        {
            return Cost;
        }

        public string getSoldby()
        {
            return Soldby;
        }

        public string getSoldbyn()
        {
            return Soldbyn;
        }

        public int getMasterid()
        {
            return masterid;
        }

        public string toString()
        {
            return item.getName() + " | " + item.getCustomName() + " | " + Soldby + " | " + masterid;
        }

        public Item getKeepItem()
        {
            Item titem = (Item) item.Clone();
            NbtCompound tag;
            if (titem.ExtraData.HasValue) tag = titem.ExtraData;
            else tag = new NbtCompound();
            
            var tt = (NbtCompound) tag.Get("ah-data");
            if (tt == null)
            {
                CyberCoreMain.Log.Error("AID2222>>> TT WAS NULL");
                return null;
            }
                
                if (tt.Contains("Name")) titem.setCustomName(((NbtString)tt.Get("Name"))?.Value);
                tag.Remove("ah-data");

            titem.ExtraData = (tag);
            return (Item) titem.Clone();
        }
    }
}