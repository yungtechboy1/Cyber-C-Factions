using System;
using System.Collections.Generic;
using System.Numerics;
using CyberCore.Manager.Crate.Data;
using CyberCore.Manager.Crate.Form;
using CyberCore.Manager.FloatingText;
using CyberCore.Utils;
using CyberCore.Utils.Data;
using log4net;
using MiNET;
using MiNET.Blocks;
using MiNET.Items;
using MiNET.Net;
using MiNET.Utils;
using MiNET.Worlds;

namespace CyberCore.Manager.Crate
{
    public class CrateMain
    {
        public void DeleteCrate(Vector3 k)
        {
            CrateChests.Remove(k);
        }

        public void addPrimedPlayer(string n, CrateAction a)
        {
            RemovePrimedPlayer(n);
            PrimedPlayer.Add(n.ToLower(), a);
        }


        public enum CrateAction
        {
            Null,
            AddCrate,
            DelCrate,
            AddKeyToCrate,
            AddItemToCrate,
            ViewCrateItems,
        }


        public CrateAction TryGetCrateActionValue(string n)
        {
            CrateAction actiontype;
            if (PrimedPlayer.TryGetValue(n, out actiontype))
            {
                return actiontype;
            }
            else
            {
                return CrateAction.Null;
            }
        }


        public static ILog Log { get; private set; } = LogManager.GetLogger(typeof(CrateMain));
        public static readonly String CK = "CrateKey";
        private Dictionary<String, CrateAction> PrimedPlayer = new Dictionary<string, CrateAction>();
        public Dictionary<Vector3, CrateObject> CrateChests = new Dictionary<Vector3, CrateObject>();
        public Dictionary<String, KeyData> CrateKeys = new Dictionary<String, KeyData>();

        public Dictionary<String, long> eids = new Dictionary<String, long>();

        //    private Dictionary<String,Object> CrateLocations = new Dictionary<String,Object>();
        public Dictionary<String, long> cratetxt = new Dictionary<String, long>();
        public CyberCoreMain CCM;

        //    public Dictionary<String, FloatingTextParticle> cratetxt = new Dictionary<>();
        private Dictionary<String, CrateData> CrateMap = new Dictionary<String, CrateData>();
        private CrateLocationDataManager crateLocationDataManager;
        private CrateDataManager crateDataManager;
        private CrateKeyDataManager crateKeyDataManager;


        //    public final String CrateKeyNBTKey =
        public CrateMain(CyberCoreMain ccm)
        {
            CCM = ccm;
            Log.Info("Loaded Crates System");
//        CCM.getServer().getPluginManager().registerEvents(new CrateListener(CCM), CCM);
            crateLocationDataManager = new CrateLocationDataManager(ccm, "crate-locations.yml");
            crateKeyDataManager = new CrateKeyDataManager(ccm, "crate-keys.yml");
            crateDataManager = new CrateDataManager(ccm, "crate-data.yml");
//        c.save();
//        ck.save();
//        cc.save();

            init();
        }

        public void init()
        {
            Log.Error("Was LOG ||" + crateDataManager.Data);
            Log.Error("Was LOG ||" + "Loading Crate Data");
            //TODO CDD
            // Dictionary<String, Object> cdd = cd.Data;
            if (crateDataManager.Data.Count == 0)
            {
                CrateData ccd = new CrateData("DEFAULT");
                crateDataManager.Data["DEFAULT"] = ccd;
                CrateMap[ccd.Key] = ccd;
                crateDataManager.save();
            }
            else
            {
                Log.Error("Was LOG ||" + "CD SIZE +=====>>" + crateDataManager.Data.Count);
                foreach (CrateData o in crateDataManager.Data.Values)
                {
                    Log.Info("[CRATE] Crate " + o.Name + " | " + o.Key + " Has been loaded!");
                    CrateMap[o.Key] = o;
                }
            }

            Log.Error("Was LOG ||" + "Loading Crate Locations");
            if (crateLocationDataManager.Data.Count != 0)
            {
                Log.Error("Was LOG ||" + "CL SIZE +=====>>" + crateLocationDataManager.Data.Count);
                foreach (CrateLocationData o in crateLocationDataManager.Data.Values)
                {
                    String nme = o.Key;
                    CrateData cda = CrateMap[nme];
                    var po = new PlayerLocation(o.X, o.Y, o.Z);
                    var lvl = o.Level;
                    var l = CCM.getAPI().LevelManager.GetLevel(null, lvl);
                    if (l == null)
                    {
                        Log.Error("WHOAAAA!! Error Loading Crate Loaction LEVEL For " + nme);
                        continue;
                    }

                    CrateObject co = new CrateObject(po, l, cda);
                    if (cda != null)
                    {
                        CrateChests[po.ToVector3()] = co;
                    }
                    else
                    {
                        Log.Error("Error Loading Chest Crate Location! " + nme);
                    }
                }
            }


            Log.Error("Was LOG ||" + "Loading Crate Keys");
            Dictionary<string, CrateKeyData> CKC = crateKeyDataManager.Data;
            if (crateLocationDataManager.Data.Count == 0)
            {
            }
            else
            {
                Log.Error("Was LOG ||" + "CKC SIZE +=====>>" + CKC.Count);
                foreach (CrateKeyData o in crateKeyDataManager.Data.Values)
                {
                    String nme = o.Key_Name;
                    if (nme == null || nme.Length == 0)
                    {
                        Log.Error("Was LOG ||" + "Error! The Key_Name was Null!");
                        return;
                    }

                    CrateKeys[nme] = new KeyData(o);
                    Log.Info("Loaded " + nme + " Crate Key!");
                }
            }
        }

        public static bool isItemKey(Item i)
        {
            if (i == null || i.Id == 0) return false;
            return i.hasCompoundTag() && i.getNamedTag().Contains(CK);
        }

        public String getKeyIDFromKey(Item i)
        {
            if (!i.hasCompoundTag() || !i.getNamedTag().Contains(CK)) return null;
            return i.getNamedTag().Get(CK).StringValue;
        }

        public Dictionary<String, CrateData> getCrateMap()
        {
            return CrateMap;
        }

        public void save()
        {
            Dictionary<String, Object> config = new Dictionary<String, Object>();
            foreach (var o in CrateMap.Values)
            {
                crateDataManager.Data.Add(o.Key, o);
            }

            Dictionary<String, Object> config2 = new Dictionary<String, Object>();
            foreach (CrateObject o in CrateChests.Values)
            {
                crateLocationDataManager.Data[o.Location.ToVector3().ToString()] = o.toConfig();
            }

            foreach (KeyData o in CrateKeys.Values)
            {
                crateKeyDataManager.Data[o.getKey_Name()] = o.toConfig();
            }

            crateDataManager.save();
            crateLocationDataManager.save();
            crateKeyDataManager.save();
        }


        public void reload()
        {
            crateLocationDataManager = new CrateLocationDataManager(CCM, "crate-locations.yml");
            crateKeyDataManager = new CrateKeyDataManager(CCM, "crate-keys.yml");
            crateDataManager = new CrateDataManager(CCM, "crate-data.yml");
        }


        public void addCrate(CorePlayer cp, Vector3 v)
        {
            cp.showFormWindow(new AdminCrateChooseCrateWindow(v, this));
//        CrateLocations.put(Vector3toKey(v),)
        }

        public void createCrate(String name, Vector3 b, Level l)
        {
            var a = new CrateData(name);
            var aa = new CrateLocationData();
            aa.Key = name;
            aa.Level = l.LevelName;
            aa.X = (int) b.X;
            aa.Y = (int) b.Y;
            aa.Z = (int) b.Z;
            aa.ItemNBTTag = "DefaultKey";
            crateLocationDataManager.Data[name] = aa;
            crateLocationDataManager.save();

            // Dictionary<String, Object> data = new Dictionary<>();
            // data.put("Item-NBT-Tag", "DefaultKey");
            // String save = b.getX() + ":" + b.getY() + ":" + b.getZ();
            // data.put("Loc", save);
            // data.put("Prizes", new List<String>());
            // crateLocationDataManager.set(name, data);
            // crateLocationDataManager.save();
        }

        public bool isKey(String cratename, Item i)
        {
            CrateLocationData data = crateLocationDataManager.Data[cratename];
            return i.getNamedTag().Contains(data.ItemNBTTag);
        }

        public CrateObject isCrate(Block b)
        {
            return isCrate(b.Coordinates);
        }

        public CrateObject isCrate(Vector3 b)
        {
            if (CrateChests.ContainsKey(b)) return CrateChests[b];
            return null;
        }

        public void showCrate(BlockCoordinates b, Player p)
        {
            p.SendMessage("Opened"); //Debug

            var tileEvent = McpeBlockEvent.CreateObject();
            tileEvent.coordinates = b;
            tileEvent.case1 = 1;
            tileEvent.case2 = 2;
            p.Level.RelayBroadcast(tileEvent);

            // inventory.InventoryChange += OnInventoryChange;
            // inventory.AddObserver(this);


            CustomFloatingTextParticle ft = new CustomFloatingTextParticle(
                new Vector3((float) (b.X + .5), b.Y + 1, (float) (b.Z + .5)), p.Level, "",
                ChatFormatting.Obfuscated + "§b|||||||||§r" + ChatColors.Red + "ROLLING Item" +
                ChatFormatting.Obfuscated +
                "§b|||||||||");
            List<Packet> packets = ft.encode();
            if (packets.Count == 1)
            {
                p.SendPacket(packets[0]);
            }
            else
            {
                foreach (Packet packet in
                    packets)
                {
                    p.SendPacket(packet);
                }
            }

            cratetxt[p.getName()] = ft.EntityId;
        }

        public void hideCrate(BlockCoordinates b, Player p)
        {
            p.SendMessage("Closed"); //Debug

            var tileEvent = McpeBlockEvent.CreateObject();
            tileEvent.coordinates = b;
            tileEvent.case1 = 1;
            tileEvent.case2 = 0;
            p.Level.RelayBroadcast(tileEvent);


            if (cratetxt.ContainsKey(p.getName()))
            {
                McpeRemoveEntity mcpeRemoveEntity = McpeRemoveEntity.CreateObject();
                mcpeRemoveEntity.entityIdSelf = cratetxt[p.getName()];
                p.SendPacket(mcpeRemoveEntity);
                cratetxt.Remove(p.getName());
            }
        }

        public void addCrateKey(KeyData keyData)
        {
            if (keyData == null) return;
            CrateKeys[keyData.NBT_Key] = keyData;
        }

        public void rollCrate(BlockCoordinates b, Player player)
        {
            CrateObject co = isCrate(b);
            if (co != null)
            {
                List<Item> items = co.getPossibleItems();


                Dictionary<String, Object> data = new Dictionary<String, Object>();

                data.Add("PlayerName", player.getName());
                data.Add("slot", -1);
                data.Add("possible-items", items);
                data.Add("crate-name", co.CD.Name);
                data.Add("pos", b);

                showCrate(b, player);
                new CrateTickThread(player.getName(), player.Level, items, co.CD.Name, new Vector3(b.X, b.Y, b.Z));
//            Server.getInstance().getScheduler().scheduleTask(new RollTick(this, data));
            }
        }

        public void RemovePrimedPlayer(string s)
        {
            PrimedPlayer.Remove(s);
        }
    }
}