using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using fNbt;
using Factions2;
using MiNET.Items;
using Newtonsoft.Json.Linq;

namespace Faction2
{
    public class Mission
    {
        public Faction_main _main;
        public String name;
        public String desc;
        public int id;
        public bool enabled;
        public Dictionary<String, int> Break = new Dictionary<String, int>();
        public Dictionary<String, int> Place = new Dictionary<String, int>();
        public Item[] ItemReq = new Item[] { };
        public int Kill = 0;
        public int XPReward = 0;
        public int MoneyReward = 0;
        public Item[] ItemReward = new Item[] { };

        /// <summary>
        /// 0 - Only Leader can get reward!
        /// 1 - Only the person who typed the command can get it
        /// 2 - Everyone Online gets it
        /// </summary>
        public int ItemRewardType = 0;

        public int PointReward = 0;


        public Mission(Faction_main main, Mission mission)
        {
            _main = mission._main;
            name = mission.name;
            desc = mission.desc;
            id = mission.id;
            enabled = mission.enabled;
            Break = mission.Break;
            Place = mission.Place;
            ItemReq = mission.ItemReq;
            Kill = mission.Kill;
            XPReward = mission.XPReward;
            MoneyReward = mission.MoneyReward;
            ItemReward = mission.ItemReward;
            ItemRewardType = mission.ItemRewardType;
            PointReward = mission.PointReward;
            if (_main == null) _main = main;
        }

        public Mission(Faction_main _main, JObject config)
        {
            _main = _main;
            name = (String) config.GetValue("name");
            desc = (String) config.GetValue("desc");
            id = (int) config.GetValue("id");
            ItemRewardType = (int) config.GetValue("irt");
            enabled = (bool) config.GetValue("enabled");
            JObject requirement = (JObject) config.GetValue("requirement");
            JObject reward = (JObject) config.GetValue("reward");
            if (requirement != null)
            {
                //break
                if (requirement["break"] != null)
                {
                    //var values = person.ToObject<Dictionary<string, object>>();
                    JObject brk = (JObject) requirement["break"];
                    Break = brk.ToObject<Dictionary<string, int>>();
                }
                //place
                if (requirement["place"] != null)
                {
                    //var values = person.ToObject<Dictionary<string, object>>();
                    JObject plc = (JObject) requirement["place"];
                    Place = plc.ToObject<Dictionary<string, int>>();
                }
                //item
                if (requirement["item"] != null)
                {
                    JObject itm = (JObject) requirement["item"];
                    if (itm.Count > 0)
                    {
                        foreach (KeyValuePair<string, JToken> property in itm)
                        {
                            String key = property.Key;
                            int icount = (int) property.Value;

                            int bid = 0;
                            int bmeta = 0;
                            if (key.Contains("|"))
                            {
                                bid = int.Parse(key.Split(new[] {"|"}, StringSplitOptions.RemoveEmptyEntries)[0]);
                                bmeta = int.Parse(key.Split(new[] {"|"}, StringSplitOptions.RemoveEmptyEntries)[1]);
                            }
                            else
                            {
                                bid = int.Parse(key);
                            }
                            Item i = ItemFactory.GetItem((short) bid, (short) bmeta, icount);
                            ItemReq[ItemReq.Length] = i;
                        }
                    }
                }
                //kill
                if (requirement["kill"] != null) Kill = (int) requirement["kill"];
            }
            if (reward != null)
            {
                XPReward = (int) reward["xp"];
                PointReward = (int) reward["point"];
                MoneyReward = (int) reward["money"];
                //item
                if (reward["item"] != null)
                {
                    JObject itm = (JObject) reward["item"];
                    if (itm.Count > 0)
                    {
                        foreach (KeyValuePair<string, JToken> a in itm)
                        {
                            String key = a.Key;
                            int val = (int) a.Value;

                            int bid = 0;
                            int bmeta = 0;
                            string edata;
                            bid = int.Parse(key.Split(new[] {"|"}, StringSplitOptions.RemoveEmptyEntries)[0]);
                            bmeta = int.Parse(key.Split(new[] {"|"}, StringSplitOptions.RemoveEmptyEntries)[1]);
                            edata = key.Split(new[] {"|"}, StringSplitOptions.RemoveEmptyEntries)[2];
                            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(edata);
                            Item i = ItemFactory.GetItem((short) bid, (short) bmeta, val);
                            NbtFile c = new NbtFile();
                            c.LoadFromBuffer(buffer, 0, buffer.Length, NbtCompression.None);
                            //TODO
                            // i.ExtraData = c.RootTag;
                            ItemReward[ItemReward.Length] = i;
                        }
                    }
                }
            }
        }

        public Item[] GetItemReward()
        {
            return ItemReward;
        }

        public void SetItemReward(Item[] itemReward)
        {
            ItemReward = itemReward;
        }

        /*
        * for(Map.Entry<String,Object> a: plc.entrySet()){
                            String key = a.getKey();
                            int bid = 0;
                            int bmeta = 0;
                            int bcount = 0;
                            if(key.contains(|")){
                                bid = int.parseInt(key.split("\\|")[0]);
                                bmeta = int.parseInt(key.split("\\|")[1]);
                            }else{
                                bid = int.parseInt(key);
                            }
                            bcount = (int)a.getValue();
                            Item i = Item.get(bid,bmeta,bcount);
                        }
        * */
    }
}