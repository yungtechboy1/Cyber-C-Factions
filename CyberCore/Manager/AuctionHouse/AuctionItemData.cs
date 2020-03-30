using System;
using System.Collections.Generic;
using System.Text;
using CyberCore.Utils;
using fNbt;
using MiNET.Items;
using MiNET.Utils;
using MySql.Data.MySqlClient;

namespace CyberCore.Manager.AuctionHouse
{
    public class AuctionItemData
    {
        public int Cost;
        public Item item;
        public int masterid = -1;
        public string Soldby; //UUID
        public string Soldbyn; //Display Name

        public AuctionItemData(Item i, int cost, CorePlayer seller)
        {
            item = i;
            Cost = cost;
            Soldby = seller.ClientUuid.ToString();
            Soldbyn = seller.DisplayName;
        }

        public AuctionItemData(MySqlDataReader rs){
            int item_id = rs.GetInt32("item-id");
            int item_meta = rs.GetInt32("item-meta");
            int item_count = rs.GetInt32("item-count");
            byte[] namedtag = Encoding.ASCII.GetBytes(rs.GetString("namedtag"));
            Cost = rs.GetInt32("cost");
            String soldbyn = rs.GetString("soldbyn");
            String soldby = rs.GetString("soldby");
            int mid = rs.GetInt32("master_id");
        
            item = ItemFactory.GetItem((short)item_id, (short)item_meta, item_count);
            var a = new NbtFile();
            a.LoadFromBuffer(namedtag, 0, namedtag.Length, NbtCompression.ZLib);
            item.ExtraData = (NbtCompound) a.RootTag;
            // item.ExtraData = (namedtag);
        
            Soldby = soldby;
            Soldbyn = soldbyn;
            masterid = mid;
        
        }

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
                // + ChatColors.RESET + "\n" +ChatColors.BLACK+"{#"+id;
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