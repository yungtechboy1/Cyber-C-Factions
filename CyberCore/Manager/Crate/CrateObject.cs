using MiNET.Utils;

namespace CyberCore.Manager.Crate
{
    public class CrateObject
    {
        public PlayerLocation Location;
        public CrateData CD;
        private bool isinit = false;
        private bool ftloaded = false;
        private CyberFloatingTextContainer ft = null;


        public CrateObject(Position p, CrateData cd)
        {
            Location = p;
            CD = cd;
            if (cd != null) init();
        }

        public String getDisplayText()
        {
            if (CD != null)
            {
                return "===|Crate|===\n" + CD.Name + "\n" + CD.SubName;
            }

            return "===|Crate|===";
        }

        public void init()
        {
            isinit = true;
            ftloaded = true;
            ft = new CyberFloatingTextContainer(CyberCoreMain.getInstance().FTM, Location, getDisplayText());
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

        //    public CrateObject(ConfigSection c) {
//        if (c.containsKey("Key")) CD = CyberCoreMain.getInstance().CrateMain.getCrateMap().get(c.getString("Key"));
//        if (c.containsKey("Loc")) Location = (Position) c.get("Loc");
////        super(c);
//    }

        public ConfigSection toConfig()
        {
            ConfigSection cf = new ConfigSection();
            CyberCoreMain.Log.Error("Was LOG ||"+"1111" + Location);
            cf.put("x", Location.x);
            cf.put("y", Location.y);
            cf.put("z", Location.z);
            CyberCoreMain.Log.Error("Was LOG ||"+"1111");
            if (Location.getLevel() != null) cf.put("level", Location.getLevel().getName());
            CyberCoreMain.Log.Error("Was LOG ||"+"1111");
            if (CD != null) cf.put("Key", CD.Key);
            CyberCoreMain.Log.Error("Was LOG ||"+"1111");
//        cf.put("Loc", Location);
//        cf.put("Loc", .put.put("y",));
            return cf;
        }

        public List<String> getPossibleKeys()
        {
            return CD == null ? null : CD.KeyItems;
        }

        public bool checkKey(Item hand)
        {
            List<String> pk = getPossibleKeys();
            if (pk == null || pk.size() == 0 || !hand.hasCompoundTag()) return false;
            String n = hand.getNamedTag().getString(CrateMain.CK);
            return pk.contains(n);
        }

        public List<Item> getPossibleItems()
        {
            List<Item> pi = new List<>();
            for (ItemChanceData icd :
            CD.PossibleItems) {
                Item i = icd.check();
                if (i == null || i.isNull()) continue;
                pi.add(i);
            }
            return pi;
        }
    }
}