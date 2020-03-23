using System;
using System.Collections.Generic;
using System.Numerics;
using CyberCore.Utils;
using log4net;
using MiNET;
using MiNET.Blocks;
using MiNET.Items;

namespace CyberCore.Manager.Crate
{
    public class CrateMain
    {
        public static ILog Log { get; private set; } = LogManager.GetLogger(typeof(CrateMain));
        public static readonly String CK = "CrateKey";
        public List<String> PrimedPlayer = new List<String>();
        public List<String> SetKeyPrimedPlayer = new List<String>();
        public List<String> SetCrateItemPrimedPlayer = new List<String>();
        public Dictionary<Vector3, CrateObject> CrateChests = new Dictionary<Vector3, CrateObject>();
        public Dictionary<String, KeyData> CrateKeys = new Dictionary<String, KeyData>();

        public Dictionary<String, Long> eids = new Dictionary<String, Long>();

        //    private ConfigSection CrateLocations = new ConfigSection();
        public ConfigSection cratetxt = new ConfigSection();
        public CyberCoreMain CCM;
        public List<String> ViewCrateItems = new List<String>();

        public List<String> RemoveCrate = new List<String>();

        //    public Dictionary<String, FloatingTextParticle> cratetxt = new Dictionary<>();
        private Dictionary<String, CrateData> CrateMap = new Dictionary<String, CrateData>();
        private CustomConfig c;
        private CustomConfig cc;
        private CustomConfig ck;

        //    public final String CrateKeyNBTKey =
        public CrateMain(CyberCoreMain ccm)
        {
            CCM = ccm;
                Log.Info("Loaded Crates System");
//        CCM.getServer().getPluginManager().registerEvents(new CrateListener(CCM), CCM);
            c = new CustomConfig(ccm,"crate-locations.yml");
            ck = new CustomConfig(ccm,"crate-keys.yml");
            cc = new CustomConfig(ccm,"crate-data.yml");
//        c.save();
//        ck.save();
//        cc.save();

            ConfigSection cl = c.getRootSection();
            ConfigSection cd = cc.getRootSection();
            System.out.println(cd);
            System.out.println("Loading Crate Data");
            Map<String, Object> cdd = cd.getAllMap();
            if (cc.loaded)
            {
                CrateData ccd = new CrateData("DEFAULT");
                cc.set("DEFAULT", ccd.toConfig());
                CrateMap.put(ccd.Key, ccd);
                cc.save();
            }
            else
            {
                System.out.println("CD SIZE +=====>>" + cd.size());
                for (Object o :
                cdd.values()) {
                    if (o instanceof ConfigSection) {
                        ConfigSection c = (ConfigSection) o;
                        String nme = c.getString("Name");
                        String key = c.getString("Key");
                        if (nme == null || nme.length() == 0)
                        {
                            CCM.getLogger().error("Error loading Crate Map! No Name Found for this config! E2545");
                            continue;
                        }

                        CCM.getLogger().info("[CRATE] Crate " + nme + " | " + key + " Has been loaded!");
                        CrateMap.put(key, new CrateData(c));
                    }
                }
            }

            System.out.println("Loading Crate Locations");
            if (cl.isEmpty())
            {
            }
            else
            {
                System.out.println("CL SIZE +=====>>" + cl.size());
                for (Object o :
                cl.getAllMap().values()) {
                    if (o instanceof ConfigSection) {
                        ConfigSection c = (ConfigSection) o;
                        String nme = c.getString("Key");
                        CrateData cda = CrateMap.getOrDefault(nme, null);
//                    ConfigSection cccc = c.getSection("Loc");;
                        Position po = new Position(c.getDouble("x"), c.getDouble("y"), c.getDouble("z"),
                            Server.getInstance().getLevelByName(c.getString("level")));
//                    Position po = (Position) c.get("Loc");
                        CrateObject co = new CrateObject(po, cda);
                        if (cda != null)
                        {
                            CrateChests.put(po.asBlockVector3().asVector3(), co);
                        }
                        else
                        {
                            CyberCoreMain.getInstance().getLogger().error("Error Loading Chest Crate Location! " + nme);
                        }
                    }
                }
            }

            System.out.println("Loading Crate Keys");
            ConfigSection CKC = ck.getRootSection();
            if (cl.isEmpty())
            {
            }
            else
            {
                System.out.println("CKC SIZE +=====>>" + CKC.size());
                for (Object o :
                CKC.getAllMap().values()) {
                    if (o instanceof ConfigSection) {
                        ConfigSection c = (ConfigSection) o;
                        String nme = c.getString("Key_Name");
                        if (nme == null || nme.length() == 0)
                        {
                            System.out.println("Error! The Key_Name was Null!");
                            return;
                        }

                        CrateKeys.put(nme, new KeyData(c));
                        CyberCoreMain.getInstance().getLogger().info("Loaded " + nme + " Crate Key!");
                    }
                }
            }
        }

        public static boolean isItemKey(Item i)
        {
            if (i == null || i.getId() == 0) return false;
            return i.hasCompoundTag() && i.getNamedTag().contains(CrateMain.CK);
        }

        public String getKeyIDFromKey(Item i)
        {
            if (!i.hasCompoundTag() || !i.getNamedTag().contains(CrateMain.CK)) return null;
            return i.getNamedTag().getString(CrateMain.CK);
        }

        public Dictionary<String, CrateData> getCrateMap()
        {
            return CrateMap;
        }

        public void save()
        {
            ConfigSection config = new ConfigSection();
            for (Object o :
            CrateMap.values()) {
                CrateData cd = (CrateData) o;
                config.put(cd.Name, cd.toConfig());
            }
            ConfigSection config2 = new ConfigSection();
            int k = 0;
            for (Object o :
            CrateChests.values()) {
                CrateObject cd = (CrateObject) o;
                config2.put("" + k++, cd.toConfig());
            }
            k = 0;
            ConfigSection config3 = new ConfigSection();
            for (Object o :
            CrateKeys.values()) {
                KeyData zd = (KeyData) o;
                config3.put("" + k++, zd.toConfig());
            }
            cc.setAll(config);
            cc.save();
            c.setAll(config2);
            c.save();
            ck.setAll(config3);
            ck.save();
        }


        public void reload()
        {
            c = new Config(new File(CCM.getDataFolder(), "crate-locations.yml"), Config.YAML,
                new LinkedDictionary<String, Object>()
                {
                });
            cc = new Config(new File(CCM.getDataFolder(), "crate-data.yml"), Config.YAML,
                new LinkedDictionary<String, Object>()
                {
                });

            ConfigSection cd = cc.getRootSection();
            Map<String, Object> cdd = cd.getAllMap();
            if (cd.isEmpty())
            {
                CrateData ccd = new CrateData("DEFAULT");
                cc.set("DEFAULT", ccd.toConfig());
                cc.save();
            }
            else
            {
                System.out.println("CD SIZE +=====>>" + cd.size());
                for (Object o :
                cdd.values()) {
                    if (o instanceof ConfigSection) {
                        ConfigSection c = (ConfigSection) o;
//                    System.out.println("THIS IS A >>>>" + o + "+" + o.getClass());
                        String nme = c.getString("Name");
                        if (nme == null || nme.length() == 0)
                        {
                            CCM.getLogger().error("Error loading Crate Map! No Name Found for this config! E2545");
                            continue;
                        }

                        CCM.getLogger().info("[CRATE] Reloading Crate" + nme + "!");
                        CrateMap.put(nme, new CrateData(c));
                    }
                }
            }
        }

        public String Vector3toKey(Vector3 v)
        {
            return v.floor().toString();
        }

        public void addCrate(CorePlayer cp, Vector3 v)
        {
            cp.showFormWindow(new AdminCrateChooseCrateWindow(v, this));
//        CrateLocations.put(Vector3toKey(v),)
        }

        public void createCrate(String name, Block b)
        {
            Dictionary<String, Object> data = new Dictionary<>();
            data.put("Item-NBT-Tag", "DefaultKey");
            String save = b.getX() + ":" + b.getY() + ":" + b.getZ();
            data.put("Loc", save);
            data.put("Prizes", new List<String>());
            c.set(name, data);
            c.save();
        }

        public boolean isKey(String cratename, Item i)
        {
            Dictionary<String, Object> data = (Dictionary<String, Object>) c.get(cratename);
            return i.getNamedTag().contains((String) data.get("Item-NBT-Tag"));
        }

        public CrateObject isCrate(Vector3 b)
        {
            if (CrateChests.containsKey(b)) return CrateChests.get(b);
            return null;
        }

        public void showCrate(Block b, Player p)
        {
            p.sendMessage("Opened"); //Debug
            BlockEventPacket pk = new BlockEventPacket();
            pk.x = (int) b.getX();
            pk.y = (int) b.getY();
            pk.z = (int) b.getZ();
            pk.case1 = 1;
            pk.case2 = 2;
            p.dataPacket(pk);
            CustomFloatingTextParticle ft = new CustomFloatingTextParticle(
                new Vector3(b.getFloorX() + .5, b.getFloorY() + 1, b.getFloorZ() + .5), "",
                TextFormat.OBFUSCATED + "§b|||||||||§r" + TextFormat.RED + "ROLLING Item" + TextFormat.OBFUSCATED +
                "§b|||||||||");
            DataPacket[] packets = ft.encode();
            if (packets.length == 1)
            {
                p.dataPacket(packets[0]);
            }
            else
            {
                for (DataPacket packet :
                packets) {
                    p.dataPacket(packet);
                }
            }

            cratetxt.put(p.getName(), ft.entityId);
        }

        public void hideCrate(Vector3 b, Player p)
        {
            p.sendMessage("Closed"); //Debug
            BlockEventPacket pk = new BlockEventPacket();
            pk.x = (int) b.getX();
            pk.y = (int) b.getY();
            pk.z = (int) b.getZ();
            pk.case1 = 1;
            pk.case2 = 0;
            p.dataPacket(pk);
            if (cratetxt.containsKey(p.getName()))
            {
                RemoveEntityPacket pk2 = new RemoveEntityPacket();
                pk2.eid = cratetxt.getLong(p.getName());
                p.dataPacket(pk2);
                cratetxt.remove(p.getName());
            }
        }

        public void addCrateKey(KeyData keyData)
        {
            if (keyData == null) return;
            CrateKeys.put(keyData.NBT_Key, keyData);
        }

        public void rollCrate(Block b, Player player)
        {
            CrateObject co = isCrate(b);
            if (co != null)
            {
                List<Item> items = co.getPossibleItems();


                ConfigSection data = new ConfigSection()
                {
                    {
                        put("PlayerName", player.getName());
                        put("slot", -1);
                        put("possible-items", items);
                        put("crate-name", co.CD.Name);
                        put("pos", b.getLocation().asBlockVector3().asVector3());
                    }
                };
                showCrate(b, player);
                new CrateTickThread(player.getName(), items, co.CD.Name, b.getLocation().asBlockVector3().asVector3());
//            Server.getInstance().getScheduler().scheduleTask(new RollTick(this, data));
            }
        }
    }
}