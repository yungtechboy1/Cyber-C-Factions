using System;
using System.Collections.Generic;
using CyberCore.Utils;
using Newtonsoft.Json;

namespace CyberCore.Manager.Crate
{
    public class CrateData
    {
        // Random RND = new Random();
        public List<ItemChanceData> PossibleItems = new List<ItemChanceData>();
        public String Key = new Random().Next(0, 10000) + "__UNNAMED CRATE__";
        public String Name = "__UNNAMED CRATE__";
        public String SubName = "==========";

        public List<String> KeyItems = new List<String>();
//    public Item Key;


        public CrateData(String name)
        {
            Name = name;
        }

//     public CrateData(Dictionary<String,Object> c) {
//         if (c.containsKey("Key")) Key = c.getString("Key");
//         if (c.containsKey("Name")) Name = c.getString("Name");
//         if (c.containsKey("SubName")) SubName = c.getString("SubName");
//         if (c.containsKey("KeyItems")) KeyItems = new List<>(c.getStringList("KeyItems"));
//         if (c.containsKey("PossibleItems")) {
//             if (c.get("PossibleItems") instanceof String)
//                 PossibleItems = PossibleItemsFromJSON(c.getString("PossibleItems"));
//             else
//                 PossibleItems = PossibleItemsFromConfig(c.getSection("PossibleItems"));
//         }
// //        if(c.containsKey("PossibleItems"))PossibleItems = (List<ItemChanceData>)c.get("PossibleItems");
//
// //        if (c.containsKey(PossibleItemsKey)) {
//
// //        }
//     }

        public String toJSON()
        {
            return JsonConvert.SerializeObject(this);
        }

        // public Dictionary<String,Object> getPossibleItemsToConfig() {
        //     Dictionary<String,Object> c = new Dictionary<String,Object>();
        //     int k = 0;
        //     for (ItemChanceData icd : PossibleItems) {
        //         c.put(k++ + "", icd.export());
        //     }
        //     return c;
        // }
//    public List<Dictionary<String,Object>> getPossibleItemsToConfig() {
//        List<Dictionary<String,Object>> c = new  List<>();
//       int k = 0;
//       for(ItemChanceData icd : PossibleItems){
//           c.add(icd.export());
//       }
//       return c;
//    }

        // public List<ItemChanceData> PossibleItemsFromConfig(Dictionary<String,Object> j) {
        //     List<ItemChanceData> a = new List<>();
        //     for (Object o : j.getAllMap().values()) {
        //         if (o instanceof Dictionary<String,Object>) {
        //             Dictionary<String,Object> c = (Dictionary<String,Object>) o;
        //             ItemChanceData icd = new ItemChanceData(c);
        //             a.add(icd);
        //         }
        //     }
        //     return a;
        // }

        //https://www.newtonsoft.com/json/help/html/SerializingJSON.htm
        public static CrateData FromJSON(String j)
        {
            return JsonConvert.DeserializeObject<CrateData>(j);
        }


        //https://github.com/google/gson/blob/master/UserGuide.md#TOC-Serializing-and-Deserializing-Collection-with-Objects-of-Arbitrary-Types
        public String export()
        {
            return toJSON();
        }


//     public Dictionary<String,Object> toConfig() {
//         Dictionary<String,Object> c = new Dictionary<String,Object>();
// //        c.put("Key", Key.getId() + ":" + Key.getDamage());
// //        c.put("Key_ID", Key_ID);
// //        c.put("Key_Meta", Key_Meta);
// //        c.put("Key_NBT", Key_NBT);
//         c.put("KeyItems", KeyItems);
//         c.put("Key", Key);
//         c.put("Name", Name);
//         c.put("SubName", SubName);
// //        c.put("PossibleItems", getPossibleItemsToJSON());
//         c.put("PossibleItems", getPossibleItemsToConfig());
// //        c.put("PossibleItems", PossibleItems);
//         return c;
//     }
    }
}