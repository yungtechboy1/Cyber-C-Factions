using System;
using System.Collections.Generic;
using System.Numerics;
using CyberCore.Manager.Crate.Data;
using CyberCore.Manager.FloatingText;
using CyberCore.Utils;
using MiNET.Items;
using MiNET.Utils;
using MiNET.Worlds;

namespace CyberCore.Manager.Crate
{
    public class CrateObject
    {
        public PlayerLocation Location;
        public Level Lvl;
        public CrateData CD;
        private bool isinit = false;
        private bool ftloaded = false;
        private CyberFloatingTextContainer ft = null;


        public CrateObject(PlayerLocation p,Level l, CrateData cd)
        {
            Location = p;
            CD = cd;
            Lvl = l;
            if (cd != null) init();
        }
        public CrateObject(PlayerLocation p,String l, CrateData cd)
        {
            Location = p;
            CD = cd;
            var ll = CyberCoreMain.GetInstance().getAPI().LevelManager.GetLevel(null,l);
            if(ll == null) CyberCoreMain.Log.Error("Crate Object could not find Level > "+l);
            Lvl = ll;
            if (cd != null) init();
        }

        public String getDisplayText()
        {
            if (CD != null)
            {
                return $"{ChatColors.Aqua}===|Crate|===\n" + CD.Name + "\n" + CD.SubName;
            }

            return $"{ChatColors.Aqua}===|Crate|===";
        }

        public void init()
        {
            isinit = true;
            ftloaded = true;
            ft = new CyberFloatingTextContainer(CyberCoreMain.GetInstance().FTM, Location+new Vector3(.5f,.5f,.5f), Lvl,getDisplayText());
            FloatingTextFactory.AddFloatingText(ft);
//        FloatingTextParticle
        }

        public void removeFT()
        {
            ft.kill();
        }

        public bool isInit()
        {
            return isinit;
        }

        public bool isFtloaded()
        {
            return ftloaded;
        }

        public CyberFloatingTextContainer getFt()
        {
            return ft;
        }

        //    public CrateObject(Dictionary<String,Object> c) {
//        if (c.containsKey("Key")) CD = CyberCoreMain.getInstance().CrateMain.getCrateMap().get(c.getString("Key"));
//        if (c.containsKey("Loc")) Location = (Position) c.get("Loc");
////        super(c);
//    }

        public CrateLocationData toLocConfig()
        {
            var a = new CrateLocationData()
            {
                Key = CD.Key,
                X = (int) Location.X,
                Y = (int) Location.Y,
                Z = (int) Location.Z,
                Level = Lvl.LevelId
            };
            return a;
        }

        public List<String> getPossibleKeys()
        {
            return CD == null ? null : CD.KeyItems;
        }

        public bool checkKey(Item hand)
        {
            List<String> pk = getPossibleKeys();
            if (pk == null || pk.Count == 0 || !hand.hasCompoundTag()) return false;
            String n = hand.getNamedTag().Get(CrateMain.CK).StringValue;
            return pk.Contains(n);
        }

        public List<Item> getPossibleItems()
        {
            List<Item> pi = new List<Item>();
            foreach (ItemChanceData icd in CD.PossibleItems) {
                Item i = icd.check();
                if (i == null || i.isNull()) continue;
                pi.Add(i);
            }
            return pi;
        }

        public void delete()
        {
            CyberCoreMain.GetInstance().CrateMain.DeleteCrate(Location);
        }
    }
}