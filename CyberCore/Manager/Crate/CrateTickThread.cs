using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading;
using CyberCore.Manager.FloatingText;
using CyberCore.Utils;
using MiNET;
using MiNET.Blocks;
using MiNET.Entities;
using MiNET.Entities.World;
using MiNET.Items;
using MiNET.Net;
using MiNET.Particles;
using MiNET.Sounds;
using MiNET.Utils;
using MiNET.Worlds;

namespace CyberCore.Manager.Crate
{
    public class CrateTickThread
    {
        public readonly int MAX = 800 / 3;

        //    public static CyberCoreMain CCM;
        private String player;
        private List<Item> items;
        long eid2 = -1l;
        long eid = -1l;
        private String name;

        public bool Alive = true;

        //    private HashMap<String, Long> eids = new HashMap<>();
        private Vector3 asVector3;

        public Level l;

        public CrateTickThread(String player, Level ll, List<Item> items, String name, Vector3 asVector3)
        {
            l = ll;
//        CCM = ccm;
            this.player = player;
            this.items = items;
            this.name = name;
            this.asVector3 = asVector3;

            run();
        }

        public void CTstop()
        {
            Alive = false;
        }

        public void run()
        {
            int life = 0;
            bool ff = false;
            int slot = -1;
            long lasttick = -1;
//        int lt = -1;
            int tick = 0;
            int t = -1;
            int waittotick = -1;
//       Console.WriteLine("11111111111111111111");
            while (Alive)
            {
                life++;
                long tt = CyberUtils.getTick();
//            if(lt +2 > t)System.out.println("W|"+life);
                Console.WriteLine("W|" + life + "||" + tt);
                if (tt > lasttick)
                {
//                lt = tt;
                    Console.WriteLine("1|" + tick);
//               Console.WriteLine("||||||||======");
                    lasttick = tick; //TODO BIGGGGGGGG LOOK HERERE FIX E!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!


                    String pn = player;
                    Vector3 pos = asVector3;
                    List<Item> pis = items;
                    if (pis == null || pis.Count == 0)
                    {
                        CyberCoreMain.Log.Info("CRATE TICK ERROR!!!! AT TICK NUMBER " + tick + " | No Items Sent!");
                        CTstop();
                        return;
                    }

                    Console.WriteLine("2||");
                    Random nr = new Random();
                    if (slot == -1) slot = nr.Next(0, pis.Count - 1);
                    slot += nr.Next(0, pis.Count);
                    Console.WriteLine("3|||");
                    while (slot >= pis.Count)
                    {
                        slot -= nr.Next(1, pis.Count - 1);
                    }

                    if (slot < 0)
                    {
                        slot = Math.Abs(slot);
                    }

                    Console.WriteLine("4||||" + pis.Count + "|" + slot);


                    //Check to see if Item Exists
                    if (eid == -1)
                    {
                        eid = generateEID();
                    }
                    //roll slots and Add 1 to it
//                while (slot >= pis.Count) {
//                    slot -= pis.Count;
//                }

                    Console.WriteLine("5|||||");
                    //Send Items
                    var itm = pis[slot];
                    var itemEntity = new ItemEntity(l, itm)
                    {
                        KnownPosition =
                        {
                            X = (float) asVector3.X + 0.5f,
                            Y = (float) asVector3.Y + 0.5f,
                            Z = (float) asVector3.Z + 0.5f
                        },
                        Velocity = new Vector3(0, 0, 0)
                    };

                    itemEntity.SpawnEntity();

                    String i = itm.getName();
                    String ll = "";
                    //Text Above Item
//                l = String.join("\n",pk.item.getLore());
                    Console.WriteLine("8||||||||");
                    bool first = true;
                    foreach (String s in itm.getLore())
                    {
                        if (first)
                        {
                            first = false;
                            ll += "§r§c|==§bItem Lore§r§c==\n\n";
                        }

                        ll += "§r§c|==>" + ChatColors.Green + s + "\n\n";
                    }

                    String f1 = CenterText.sendCenteredMessage("§r§c|---->§b" + i + "§c<----|\n\n" + ll, 246);
                    String f2 = CenterText.sendCenteredMessage(
                        ChatFormatting.Obfuscated + "§b|||||||||§r" + ChatColors.Red + "ROLLING Item" +
                        ChatFormatting.Obfuscated +
                        "§b|||||||||§r", 246);
                    CustomFloatingTextParticle ft = new CustomFloatingTextParticle(
                        new Vector3((float) (pos.X + .5), pos.Y + 3, (float) (pos.Z + 1.5)), l, f1, f2);

                    Console.WriteLine("9|||||||||");
//        FloatingTextFactory.AddFloatingText(new CyberFloatingTextContainer(FTM, getServer().getLevelByName("world").getSafeSpawn().add(0, 5, 0), ChatColors.Green + "This is Spawn!"));
//                Long e1d = main.cratetxt.getLong(pn);
                    if (eid2 == -1) eid2 = generateEID();
                    ft.EntityId = eid2;
                    List<Packet> packets = ft.encode();

                    Console.WriteLine("10|||||||||");
                    Console.WriteLine("11||||||||||");
                    Player p = CyberCoreMain.GetInstance().getAPI().PlayerManager.getPlayer(pn);
                    Console.WriteLine("12|||||||||||");
                    if (p != null)
                    {
                        l.MakeSound(new ClickSound(pos));
                        foreach (Packet packet in packets)
                        {
                            p.SendPacket(packet);
                        }
                    }

                    Console.WriteLine("13||||||||||||");
                    //l.addParticle(new RedstoneParticle(pos.add(.5,1,.5),2));
                    //This one
                    //l.addParticle(new DestroyBlockParticle(pos.add(.5,1,.5), Block.get(new NukkitRandom(tick).Next(0,100))));
                    //Or this one
                    //@TODO allow Config to choose break particle!
                    var aaa = new Web();
                    aaa.Coordinates =
                        new BlockCoordinates(new Vector3((float) (.5 + pos.X), 1 + pos.Y, (float) (.5 + pos.Z)));

                    var aa = new DestroyBlockParticle(l, aaa);
                    aa.Spawn();

                    Console.WriteLine("14|||||||||||||");

                    if (!ff)
                    {
                        //Schedule Next
                        int k = getDelayFromTick(tick);
                        Console.WriteLine("15A|||||||||||||" + k + "|" + tick);
                        if (tick >= MAX)
                        {
                            ff = true;
                        }

                        tick += k;
                        Console.WriteLine("16A|||||||||||||" + tick);
                        lasttick = tt + k;
                        continue;
                    }
                    else
                    {
                        Console.WriteLine("15B|||||||||||||");
                        //Last Tick for Roll... Send Item!
                        //l.addParticle(new HugeExplodeParticle(pos));
                        var e1 = new LegacyParticle(ParticleType.HugeExplode, l)
                        {
                            Position = new Vector3((float) (.5 + pos.X), 1 + pos.Y, (float) (.5 + pos.Z))
                        };
                        var e2 = new LegacyParticle(ParticleType.HugeExplode, l)
                        {
                            Position = new Vector3((float) (.5 + pos.X), 1 + pos.Y, (float) (1.5 + pos.Z))
                        };
                        var e3 = new LegacyParticle(ParticleType.HugeExplode, l)
                        {
                            Position = new Vector3((float) (1.5 + pos.X), 1 + pos.Y, (float) (1.5 + pos.Z))
                        };
                        var e4 = new LegacyParticle(ParticleType.HugeExplode, l)
                        {
                            Position = new Vector3((float) (1.5 + pos.X), 1 + pos.Y, (float) (.5 + pos.Z))
                        };
                        var e5 = new LegacyParticle(ParticleType.HugeExplode, l)
                        {
                            Position = pos
                        };
                        e1.Spawn();
                        e2.Spawn();
                        e3.Spawn();
                        e4.Spawn();
                        e5.Spawn();
                        Console.WriteLine("16B|||||||||||||");
                        int zz = 5;
                        int zzz = 0;
                        for (int z = 0; z < 100000; z++)
                        {
                            var e = new LegacyParticle(ParticleType.HugeExplode, l);
                            e.Position = new Vector3((float) (.5 + pos.X), 1 + pos.Y, (float) (.5 + pos.Z));
                            Thread.Sleep(zz * 50);
                            if (zz < 100)
                            {
                                zz += 5;
                            }
                            else
                            {
                                if (zzz > 4) break;
                                zzz++;
                            }

                            e.Spawn();
                        }


                        Console.WriteLine("17B|||||||||||||");
//                    Server.getInstance().getScheduler().scheduleDelayedTask(new SendItem(main, data), 20 * 5);
                        //Send items
                        //i
                        if (p == null)
                        {
                            CyberCoreMain.Log.Error("Error Could not find player to give crate reward to!!!");
                        }
                        else
                        {
                            p.Inventory.AddItem(itm, true);
                            p.SendMessage("Item added!!!!");
                        }

                        var pk = new McpeRemoveEntity();
                        pk.entityIdSelf = eid2;
                        var pk2 = new McpeRemoveEntity();
                        pk2.entityIdSelf = eid;
                        p.SendPacket(pk);
                        p.SendPacket(pk2);

                        Console.WriteLine("18B|||||||||||||");
                        CTstop();
                        return;
                    }
                }
                else
                {
                        Thread.Sleep(80); //4 Ticks*2//200
                    
                }
            }
        }


        public long generateEID()
        {
            return 1095216660480L + CyberUtils.LongRandom(0L, 2147483647L);
        }

        public int getDelayFromTick(int t)
        {
            int f = 0;
            if (t < (MAX / 16))
            {
                f = 2;
            }
            else if (t < (2 * MAX / 16))
            {
                f = 3;
            }
            else if (t < (3 * MAX / 16))
            {
                f = 3;
            }
            else if (t < (4 * MAX / 16))
            {
                f = 4;
            }
            else if (t < (5 * MAX / 16))
            {
                f = 4;
            }
            else if (t < (6 * MAX / 16))
            {
                f = 5;
            }
            else if (t < (7 * MAX / 16))
            {
                f = 6;
            }
            else if (t < (8 * MAX / 16))
            {
                f = 7;
            }
            else if (t < (30 * MAX / 64))
            {
                //9/16 > 18/32 > 36/64
                f = 9;
            }
            else if (t < (35 * MAX / 64))
            {
                f = 12;
            }
            else if (t < (45 * MAX / 64))
            {
                //12/16 >
                f = 15;
            }
            else if (t < (55 * MAX / 64))
            {
                //13/16 > 56/
                f = 17;
            }
            else if (t < (60 * MAX / 64))
            {
                f = 18;
            }
            else if (t < (62 * MAX / 64))
            {
                //15/16 > 30/32 > 45/48 > 60/64
                f = 19;
            }
            else
            {
                f = 20;
            }

//        f += 3;
            return f;
        }
    }
}