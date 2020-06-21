using System;
using System.Collections.Generic;
using System.Numerics;
using CyberCore.Manager.Crate.Data;
using CyberCore.Manager.Crate.Form;
using CyberCore.Manager.FloatingText;
using CyberCore.Manager.Forms;
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
            CrateChestLocations.Remove(k);
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
                if (actiontype == null) return CrateAction.Null;
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
        public Dictionary<Vector3, CrateObject> CrateChestLocations = new Dictionary<Vector3, CrateObject>();
        public Dictionary<String, KeyData> CrateKeys = new Dictionary<String, KeyData>();

        public Dictionary<String, long> eids = new Dictionary<String, long>();

        //    private Dictionary<String,Object> CrateLocations = new Dictionary<String,Object>();
        public Dictionary<String, long> cratetxt = new Dictionary<String, long>();
        public CyberCoreMain CCM;

        //    public Dictionary<String, FloatingTextParticle> cratetxt = new Dictionary<>();
        private Dictionary<String, CrateData> CrateData = new Dictionary<String, CrateData>();
        private CrateLocationDataManager CrateChestLocationsDataManager;
        private CrateDataManager CrateDataManager;
        private CrateKeyDataManager crateKeyDataManager;


        //    public final String CrateKeyNBTKey =
        public CrateMain(CyberCoreMain ccm)
        {
            CCM = ccm;
            Log.Info("Loaded Crates System");
//        CCM.getServer().getPluginManager().registerEvents(new CrateListener(CCM), CCM);
            CrateChestLocationsDataManager =
                new CrateLocationDataManager(ccm).load<CrateLocationDataManager>();
            crateKeyDataManager = new CrateKeyDataManager(ccm).load<CrateKeyDataManager>();
            CrateDataManager = new CrateDataManager(ccm).load<CrateDataManager>();
            if (CrateChestLocationsDataManager == null)
            {
                CrateChestLocationsDataManager = new CrateLocationDataManager(ccm);
                Log.Error("ERROR WITH CL");
            }

            if (crateKeyDataManager == null)
            {
                
                Log.Error("ERROR WITH CK");
                crateKeyDataManager = new CrateKeyDataManager(ccm);
            }

            if (CrateDataManager == null)
            {
                
                Log.Error("ERROR WITH CD");
                CrateDataManager = new CrateDataManager(ccm);
            }
//        c.save();
//        ck.save();
//        cc.save();

            init();
        }

        public void init()
        {
            Log.Error("Was LOG ||" + CrateDataManager.Data);
            Log.Error("Was LOG ||" + CrateDataManager.Data.Count);
            Log.Error("Was LOG ||" + "Loading Crate Data");
            //TODO CDD
            // Dictionary<String, Object> cdd = cd.Data;
            if (CrateDataManager.Data.Count == 0)
            {
                CrateData ccd = new CrateData("DEFAULT");
                CrateDataManager.Data[ccd.Key] = ccd;
                CrateDataManager.save();
                CrateData[ccd.Key] = ccd;
                Log.Info("Creating Default Crate");
            }
            else
            {
                Log.Error("Was LOG ||" + "CD SIZE +=====>>" + CrateDataManager.Data.Count);
                foreach (CrateData o in CrateDataManager.Data.Values)
                {
                    Log.Info("[CRATE] Crate " + o.Name + " | " + o.Key + " Has been loaded!");
                    CrateData[o.Key] = o;
                }
            }

            Log.Error("Was LOG ||" + "Loading Crate Locations");
            if (CrateChestLocationsDataManager.LocData.Count != 0)
            {
                try
                {
                    Log.Error("Was LOG ||" + "CL SIZE +=====>>" + CrateChestLocationsDataManager.LocData.Count);
                    foreach (CrateLocationData o in CrateChestLocationsDataManager.LocData.Values)
                    {
                        String nme = o.Key;
                        CrateData cda = CrateData[nme];
                        var po = new PlayerLocation(o.X, o.Y, o.Z);
                        var lvl = o.Level;
                        var l = CCM.getAPI().LevelManager.GetLevel(null, lvl);
                        if (l == null)
                        {
                            Log.Error("WHOAAAA!! Error Loading Crate Loaction LEVEL For " + nme);
                            continue;
                        }

                        if (l.GetBlock(po).Id != new Chest().Id)
                        {
                            Log.Error("CHEST AT "+po+" Was not able to be loaded because Block was not a chest any mroe!");
                            continue;
                        }
                        CrateObject co = new CrateObject(po, l, cda);
                        if (cda != null)
                        {
                            
                            CrateChestLocations[po.ToVector3()] = co;
                        }
                        else
                        {
                            Log.Error("Error Loading Chest Crate Location! " + nme);
                        }
                    }
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }
                CrateChestLocationsDataManager.LocData.Clear();
            }


            Log.Error("Was LOG ||" + "Loading Crate Keys");
            if (crateKeyDataManager.KeyData.Count != 0)
            {
                // Log.Error("Was LOG ||" + "CKC SIZE +=====>>" + CKC.Count);
                foreach (CrateKeyData o in crateKeyDataManager.KeyData.Values)
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
            Log.Error("WasDDDDDDDDDDDDDDDDDDD LOG ||" + "Loading Crate Keys");

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
            return CrateData;
        }

        public void save()
        {
            foreach (CrateData o in CrateData.Values)
            {
                CrateDataManager.Data[o.Key] =  o;
                Console.WriteLine("Saving crateDataManager"+o.Key);
            }


            foreach (CrateObject o in CrateChestLocations.Values)
            {
                CrateChestLocationsDataManager.LocData[o.Location.ToVector3().toCyberString()] = o.toLocConfig();
                Console.WriteLine("Saving crateLocationDataManager");
            }


            foreach (KeyData o in CrateKeys.Values)
            {
                crateKeyDataManager.KeyData[o.getKey_Name()] = o.toConfig();
                Console.WriteLine("Saving crateKeyDataManager");
            }

            Console.WriteLine("Savingggggggg");

            if (CrateDataManager.Data != null)
            {
                CrateDataManager.save();
                Console.WriteLine("Savingggggggg1|" + CrateDataManager.Data.Count + "||" + CrateData.Count);
            }

            if (CrateChestLocationsDataManager.LocData != null)
            {
                CrateChestLocationsDataManager.save();
                Console.WriteLine("Savingggggggg2");
                Console.WriteLine($"DATATAAAAA {CrateChestLocations.Count} || {CrateChestLocationsDataManager.LocData.Count}");
            }
            
            if (crateKeyDataManager.KeyData != null)
            {
                crateKeyDataManager.save();
                Console.WriteLine("Savingggggggg3");
            }
        }


        public void CreateCrate(Vector3 v, Level l, CrateData cd)
        {
            CrateChestLocations[v] = new CrateObject(new PlayerLocation(v) + new Vector3(.5f, .5f, .5f), l, cd);
        }

        public void reload()
        {
            CrateChestLocationsDataManager = new CrateLocationDataManager(CCM);
            crateKeyDataManager = new CrateKeyDataManager(CCM);
            CrateDataManager = new CrateDataManager(CCM);
        }


        public void addCrate(CorePlayer cp, Vector3 v)
        {
            cp.showFormWindow(new AdminCrateChooseCrateWindow(v, this));

//        CrateLocations.put(Vector3toKey(v),)
        }

        // public void createCrate(String name, Vector3 b, Level l)
        // {
        //     var a = new CrateData(name);
        //     var aa = new CrateLocationData();
        //     aa.Key = name;
        //     aa.Level = l.LevelName;
        //     aa.X = (int) b.X;
        //     aa.Y = (int) b.Y;
        //     aa.Z = (int) b.Z;
        //     aa.ItemNBTTag = "DefaultKey";
        //     crateLocationDataManager.LocData[name] = aa;
        //     crateLocationDataManager.save();
        //
        //     // Dictionary<String, Object> data = new Dictionary<>();
        //     // data.put("Item-NBT-Tag", "DefaultKey");
        //     // String save = b.getX() + ":" + b.getY() + ":" + b.getZ();
        //     // data.put("Loc", save);
        //     // data.put("Prizes", new List<String>());
        //     // crateLocationDataManager.set(name, data);
        //     // crateLocationDataManager.save();
        // }

        // public bool isKey(String cratename, Item i)
        // {
        //     CrateLocationData data = CrateChestLocationsDataManager.LocData[cratename];
        //     return i.getNamedTag().Contains(data.ItemNBTTag);
        // }

        public CrateObject isCrate(Block b)
        {
            return isCrate(b.Coordinates.Floor());
        }

        public CrateObject isCrate(Vector3 b)
        {
            if (CrateChestLocations.ContainsKey(b)) return CrateChestLocations[b];
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