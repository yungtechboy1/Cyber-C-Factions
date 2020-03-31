using System;
using System.Collections.Generic;
using System.IO;
using CyberCore.CustomEnums;
using CyberCore.Utils;
using MiNET;
using MiNET.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using static CyberCore.Manager.ClassFactory.ClassType;

namespace CyberCore.Manager.ClassFactory
{
    public class ClassFactory
    {
        public static List<PowerEnum> DEFAULTPOWERS = new List<PowerEnum>
        {
            PowerEnum.MineLife
        };

        public static Dictionary<PowerEnum, int> BUYPOWERS = new Dictionary<PowerEnum, int>
        {
            {PowerEnum.FireBox, 10} //XP Cost
        };

        private CyberCoreMain CCM;


        public Dictionary<string, object> MMOSave = new Dictionary<string, object>();

        public Dictionary<string, object> PlayerLearnedPowers = new Dictionary<string, object>();
//    private Dictionary<String, BaseClass> ClassList = new Dictionary<>();

        public ClassFactory(CyberCoreMain main)
        {
            CCM = main;
            // MMOSave = new Config(new File(CCM.getDataFolder(), "MMOSave.yml"), Config.YAML);
            // PlayerLearnedPowers = new Config(new File(CCM.getDataFolder(), "PlayerLearnedPowers.yml"), Config.YAML);
//        CCM.getServer().getScheduler().scheduleDelayedRepeatingTask(new LumberJackTreeCheckerTask(main), 20 * 60, 20 * 60);//Every Min
        }


        //Not Even use in Original!
//         private void loadMMOSave()
//         {
//             
//             JObject o1 = JObject.Parse(File.ReadAllText(CCM.getAPI().PluginManager.));
//             
//             // read JSON directly from a file
//             string pluginDirectory = Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath);
//             using (StreamReader file = File.OpenText(Path.Combine(pluginDirectory,"MMOSave.json")))
//             using (JsonTextReader reader = new JsonTextReader(file))
//             {
//                 JObject o2 = (JObject)JToken.ReadFrom(reader);
//             }
//             try
//             {
//                 GsonBuilder builder = new GsonBuilder();
//                 Gson gson = builder.create();
// //            Type listOfMyClassObject = new TypeToken<List<MyClass>>() {}.getType();
//                 string content = readFileAsString(new File(CCM.getDataFolder().toString(), "MMOSave.yml").toString());
// //            this.config = new ConfigSection(gson.fromJson(content, new TypeToken<LinkedDictionary<String, Object>>() {
// //            }.getType()));
//             }
//             catch (Exception e)
//             {
//             }
//         }

        //    handelEvent(event, cp);
        public PowerEnum[] getRegisteredPowers(CorePlayer p)
        {
            List<PowerEnum> pe = new List<PowerEnum>();
            if (!PlayerLearnedPowers.ContainsKey(p.getName().ToLower())) return new PowerEnum[0];
            Dictionary<string, object> c = (Dictionary<String, Object>) PlayerLearnedPowers[p.getName().ToLower()];
            var a = new PowerEnum();
            foreach (Object v in c.Values)
            {
                if (v is int)
                {
                    pe.Add(PowerEnum.fromint((int) v, a));
                }
                else
                {
                    Console.WriteLine("EEEEEEEEQweqweqwe qwe qwe qweqwqe qweqweqqqqqqqqq!");
                }
            }

            return pe.ToArray();
        }

        public void registerPowerToPlayer(CorePlayer p, PowerEnum e)
        {
            Dictionary<String, Object> c = new Dictionary<String, Object>();
            if (PlayerLearnedPowers.ContainsKey(p.getName().ToLower()))
            {
                c = (Dictionary<string, object>) PlayerLearnedPowers[p.getName().ToLower()];
            }

            c[e.Name] = e.ID;
            PlayerLearnedPowers[p.getName().ToLower()] = c;
        }

        public void leaveClass(CorePlayer p)
        {
            p.getPlayerClass().onLeaveClass();
            MMOSave.Remove(p.getName().ToLower());
            p.SetPlayerClass(null);
            p.SendMessage(ChatColors.Green + "You left your class!");
        }

        public BaseClass GetClass(CorePlayer p)
        {
            return GetClass(p, false);
        }

        public BaseClass GetClass(CorePlayer p, bool force)
        {
            if (p == null)
            {
                CyberCoreMain.Log.Error("Error! Tring to get class from NULL");
                return null;
            }

            if (p.getPlayerClass() != null && !force)
            {
                return p.getPlayerClass();
            }

            Dictionary<string, object> o = (Dictionary<string, object>) MMOSave[p.getName().ToLower()];
            if (o != null)
            {
                int a = (int) (o["TYPE"]);
                BaseClass data = null; //new BaseClass(CCM, p, (ConfigSection) o);
                switch (ClassTypeExtender.fromInt(a))
                {
                    case Unknown:
                        break;
                    case Class_Miner_TNT_Specialist:
                        // data = new TNTSpecialist(CCM, p, o);
                        break;
                    case Class_Miner_MineLife:
                        // data = new MineLifeClass(CCM, p, o);
                        break;
                    case Class_Magic_Enchanter:
                        break;
                    case Class_Rouge_Theif:
                        // data = new Theif(CCM, p, o);
                        break;
                    case Class_Offense_Knight:
                        // data = new Knight(CCM, p, o);
                        break;
                    case Class_Offense_Mercenary:
                        // data = new Mercenary(CCM, p, o);
                        break;
                    case Class_Offense_Holy_Knight:
                        // data = new HolyKnight(CCM, p, o);
                        break;
                    case Class_Offense_Dark_Knight:
                        // data = new DarkKnight(CCM, p, o);
                        break;
                    case Class_Offense_DragonSlayer:
                        // data = new DragonSlayer(CCM, p, o);
                        break;
                    case Class_Offense_Assassin:
                        break;
                    case Class_Offense_Raider:
                        break;
                    case Class_Magic_Sorcerer:
                        // data = new Sorcerer(CCM, p, o);
                        break;
                    case Class_Priest:
                        // data = new Priest(CCM, p, o);
                        break;
                }

                if (data == null)
                {
                    Console.WriteLine("ERROROROROR NONEEE WEREEEEEE DDDDDDDDDDD" + a);
                    return null;
                }

                Console.WriteLine(p.getName() + "'s CLASS WAS LOADEDEDDDD NONEEE WEREEEEEE DDDDDDDDDDD");
                data.onCreate();
                p.SetPlayerClass(data);
                return data;
            }

            return null;
        }

        public void SaveClassToFile(CorePlayer p)
        {
            BaseClass bc = p.getPlayerClass();
            if (bc != null)
            {
                MMOSave[p.getName().ToLower()] = p.getPlayerClass().export();
                Console.WriteLine("SAVEEE");
            }
            else
            {
                Console.WriteLine(p.getName() + " HASS NUNN CLASS???");
            }

            //TODO
            // MMOSave.save();
        }

//    @EventHandler
//    public void OnEvent(BlockBreakEvent event) {
//        handelEvent(event, (CorePlayer) event.getPlayer());
//    }
//
//    @EventHandler
//    public void OnEvent(BlockPlaceEvent event) {
//        handelEvent(event, (CorePlayer) event.getPlayer());
//    }
//
//    @EventHandler
//    public void OnEvent(PlayerInteractEvent event) {
//        handelEvent(event, (CorePlayer) event.getPlayer());
//    }

//    @EventHandler
//    public void OnEvent(BlockPlaceEvent event) {
//        CorePlayer cp = (CorePlayer) ((BlockPlaceEvent) event).getPlayer();
//        handelEvent(event, cp);
//    }
//
//    @EventHandler
//    public void OnEvent(BlockBreakEvent event) {
//        CorePlayer cp = (CorePlayer) ((BlockBreakEvent) event).getPlayer();
//        handelEvent(event, cp);
//    }
//
//    @EventHandler
//    public void OnEvent(EntityDamageEvent event) {
//        if(event.getEntity() instanceof Player) {
//            CorePlayer cp = (CorePlayer) (event).getEntity();
//            handelEvent(event, cp);
////        } else {
////            handelEvent(event, null);
//        }
//    }

//    @EventHandler
//    public void OnEvent(EntityDamageByEntityEvent event) {
//        CorePlayer cp = (CorePlayer) ((BlockPlaceEvent) event).getPlayer();
//        handelEvent(event, cp);
//    }


//    public void OnEvent(Event event) {
//        CorePlayer cp = null;
//        if (event instanceof 1) {
//            if (event instanceof BlockPlaceEvent) {
//            } else if (event instanceof BlockBreakEvent) {
//                cp = (CorePlayer) ((BlockBreakEvent) event).getPlayer();
//            }
//        } else if (event instanceof PlayerEvent) {
//            cp = (CorePlayer) ((PlayerEvent) event).getPlayer();
//        } else if (event instanceof EntityEvent) {
//            cp = (CorePlayer) ((EntityEvent) event).getEntity();
//        }
//
//        if (cp == null) Console.WriteLine("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
//        if (cp == null) Console.WriteLine("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
//        if (cp == null) Console.WriteLine("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
//        if (cp == null) Console.WriteLine("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
//        if (cp == null) Console.WriteLine("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
//        if (cp == null) Console.WriteLine("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
//        if (cp == null) Console.WriteLine("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
//
//    }

/*
    public void OnEvent(Event event) {
        if (event instanceof BlockBreakEvent || event instanceof BlockPlaceEvent || event instanceof PlayerInteractEvent || event instanceof EntityRegainHealthEvent) {
            if (event instanceof BlockBreakEvent) {
                handelEvent(event, ((BlockBreakEvent) event).getPlayer());
            } else if (event instanceof BlockPlaceEvent) {
                handelEvent(event, ((BlockPlaceEvent) event).getPlayer());
            } else if (event instanceof PlayerInteractEvent) {
                handelEvent(event, ((PlayerInteractEvent) event).getPlayer());
            } else if (event instanceof EntityRegainHealthEvent && ((EntityRegainHealthEvent) event).getEntity() instanceof Player) {
                handelEvent(event, (Player) ((EntityRegainHealthEvent) event).getEntity());
            }
        }
    }*/

//    public void HandelEvent(Event event, CorePlayer p) {
//        BaseClass bc = GetClass(p);
//        if (bc == null) return;
//        bc.HandelEvent(event);
//    }

        public void Saveall()
        {
            CyberCoreMain.Log.Info("SAving All Classes!");
            foreach (Player p in CCM.getAPI().PlayerManager.GetPlayers())
            {
                CorePlayer cp = p as CorePlayer;
                if (cp?.getPlayerClass() == null) continue;
                save(cp, false);
            }

            CyberCoreMain.Log.Info("SAving File!");
            //TODO
            // MMOSave.save();
        }

        public void save(CorePlayer cp, bool savefile = true)
        {
            MMOSave[cp.getName().ToLower()] = cp.getPlayerClass().export();
            //TODO
            // if (savefile) MMOSave.save();
        }
    }
}