using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading;
using CyberCore.Manager.FloatingText;
using CyberCore.Utils;
using MiNET;
using MiNET.Blocks;
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
        private string CrateName;
        private long eid = -1l;
        private long eid2 = -1l;
        private readonly List<Item> ItemList;

        public bool KeepAlive = true;

        public Level Level;

        //    public static CyberCoreMain CCM;
        private readonly string Player;

        //    private HashMap<String, Long> eids = new HashMap<>();
        private readonly Vector3 Position;

        public CrateTickThread(string player, Level ll, List<Item> itemList, string crateName, Vector3 position)
        {
            Level = ll;
            Player = player;
            ItemList = itemList;
            CrateName = crateName;
            Position = position;

            run();
        }

        public void CTstop()
        {
            KeepAlive = false;
        }

        public void run()
        {
            ItemEntity itemEntity = null;
            var lifecurrentticks = 0;
            var ff = false;
            var slot = -1;
            long lasttick = -1;
            var t = -1;
            var waittocurrenttick = -1;
            var nr = new Random();
//       Console.WriteLine("11111111111111111111");
            while (KeepAlive)
            {
                lifecurrentticks++;
                var currenttick = CyberUtils.getTick();
                Console.WriteLine("W|" + lifecurrentticks + "||" + currenttick);
                if (currenttick > lasttick)
                {
                    Console.WriteLine("1|" + currenttick);
                    lasttick = currenttick;

                    if (ItemList == null || ItemList.Count == 0)
                    {
                        CyberCoreMain.Log.Info("CRATE currenttick ERROR!!!! AT currenttick NUMBER " + currenttick +
                                               " | No Items Sent!");
                        CTstop();
                        return;
                    }

                    Console.WriteLine("2||");
                    if (slot == -1) slot = nr.Next(0, ItemList.Count - 1);
                    slot += nr.Next(0, ItemList.Count);
                    Console.WriteLine("3|||");
                    while (slot >= ItemList.Count) slot -= nr.Next(1, ItemList.Count - 1);

                    if (slot < 0) slot = Math.Abs(slot);
                    //TODO Cleanup

                    Console.WriteLine("4||||" + ItemList.Count + "|" + slot);


                    //Check to see if Item Exists
                    if (eid == -1) eid = generateEID();
                    //roll slots and Add 1 to it
//                while (slot >= ItemList.Count) {
//                    slot -= ItemList.Count;
//                }

                    Console.WriteLine("5|||||");
                    //Send Items
                    var itm = ItemList[slot];
                    Player p = CyberCoreMain.GetInstance().getAPI().PlayerManager.getPlayer(Player);
                    if (itemEntity != null)
                    {
                        McpeRemoveEntity mcpeRemoveEntity = McpeRemoveEntity.CreateObject();
                        mcpeRemoveEntity.entityIdSelf = itemEntity.EntityId;
                        p.SendPacket(mcpeRemoveEntity);
                    }

                    itemEntity = new ItemEntity(Level, itm)
                    {
                        KnownPosition =
                        {
                            X = Position.X + 0.5f,
                            Y = Position.Y + 0.5f,
                            Z = Position.Z + 0.5f
                        },
                        Velocity = new Vector3(0, 0, 0),
                        PickupDelay = 100000000
                    };

                    itemEntity.SpawnEntity();

                    var i = itm.getName();
                    var ll = "";
                    //Text Above Item
//                l = String.join("\n",pk.item.getLore());
                    Console.WriteLine("8||||||||");
                    var first = true;
                    foreach (var s in itm.getLore())
                    {
                        if (first)
                        {
                            first = false;
                            ll += "§r§c|==§bItem Lore§r§c==\n\n";
                        }

                        ll += "§r§c|==>" + ChatColors.Green + s + "\n\n";
                    }

                    var f1 = CenterText.sendCenteredMessage("§r§c|---->§b" + i + "§c<----|\n\n" + ll, 246);
                    var f2 = CenterText.sendCenteredMessage(
                        ChatFormatting.Obfuscated + "§b|||||||||§r" + ChatColors.Red + "ROLLING Item" +
                        ChatFormatting.Obfuscated +
                        "§b|||||||||§r", 246);
                    var ft = new CustomFloatingTextParticle(
                        new Vector3((float) (Position.X + .5), Position.Y + 3, (float) (Position.Z + 1.5)), Level, f1,
                        f2);

                    Console.WriteLine("9|||||||||");
//        FloatingTextFactory.AddFloatingText(new CyberFloatingTextContainer(FTM, getServer().getLevelByName("world").getSafeSpawn().add(0, 5, 0), ChatColors.Green + "This is Spawn!"));
//                Long e1d = main.cratetxt.getLong(pn);
                    if (eid2 == -1) eid2 = generateEID();
                    ft.EntityId = eid2;
                    var packets = ft.encode();

                    Console.WriteLine("10|||||||||");
                    Console.WriteLine("11||||||||||");
                    Console.WriteLine("12|||||||||||");
                    if (p != null)
                    {
                        Level.MakeSound(new ClickSound(Position));
                        foreach (var packet in packets) p.SendPacket(packet);
                    }

                    Console.WriteLine("13||||||||||||");
                    //l.addParticle(new RedstoneParticle(pos.add(.5,1,.5),2));
                    //This one
                    //l.addParticle(new DestroyBlockParticle(pos.add(.5,1,.5), Block.get(new NukkitRandom(currenttick).Next(0,100))));
                    //Or this one
                    //@TODO allow Config to choose break particle!
                    var aaa = new Web();
                    aaa.Coordinates =
                        new BlockCoordinates(new Vector3((float) (.5 + Position.X), 1 + Position.Y,
                            (float) (.5 + Position.Z)));

                    var aa = new DestroyBlockParticle(Level, aaa);
                    aa.Spawn();

                    Console.WriteLine("14|||||||||||||");

                    if (!ff)
                    {
                        //Schedule Next
                        var k = getDelayFromcurrenttick(lifecurrentticks);
                        Console.WriteLine("15A|||||||||||||" + k + "|" + currenttick + " |||\\| " + lifecurrentticks);
                        if (lifecurrentticks >= MAX) ff = true;

                        currenttick += k;
                        Console.WriteLine("16A|||||||||||||" + currenttick);
                        lasttick = currenttick + k;
                    }
                    else
                    {
                        Console.WriteLine("15B|||||||||||||");
                        //Last currenttick for Roll... Send Item!
                        //l.addParticle(new HugeExplodeParticle(pos));
                        var e1 = new LegacyParticle(ParticleType.HugeExplode, Level)
                        {
                            Position = new Vector3((float) (.5 + Position.X), 1 + Position.Y, (float) (.5 + Position.Z))
                        };
                        var e2 = new LegacyParticle(ParticleType.HugeExplode, Level)
                        {
                            Position = new Vector3((float) (.5 + Position.X), 1 + Position.Y,
                                (float) (1.5 + Position.Z))
                        };
                        var e3 = new LegacyParticle(ParticleType.HugeExplode, Level)
                        {
                            Position = new Vector3((float) (1.5 + Position.X), 1 + Position.Y,
                                (float) (1.5 + Position.Z))
                        };
                        var e4 = new LegacyParticle(ParticleType.HugeExplode, Level)
                        {
                            Position = new Vector3((float) (1.5 + Position.X), 1 + Position.Y,
                                (float) (.5 + Position.Z))
                        };
                        var e5 = new LegacyParticle(ParticleType.HugeExplode, Level)
                        {
                            Position = Position
                        };
                        e1.Spawn();
                        e2.Spawn();
                        e3.Spawn();
                        e4.Spawn();
                        e5.Spawn();
                        Console.WriteLine("16B|||||||||||||");
                        
                        if (p == null)
                        {
                            CyberCoreMain.Log.Error("Error Could not find player to give crate reward to!!!");
                        }
                        else
                        {
                            p.Inventory.AddItem(itm, true);
                            p.SendMessage("Item added!!!!");
                        }
                        
                        var zz = 5;
                        var zzz = 0;
                        
                        for (var z = 0; z < 1000; z++)
                        {
                            var e = new LegacyParticle(ParticleType.HugeExplode, Level);
                            e.Position = new Vector3((float) (.5 + Position.X), 1 + Position.Y,
                                (float) (.5 + Position.Z));
                            Thread.Sleep(250+zz+(zzz*30));
                            if (zz < 100)
                            {
                                zz += 5;
                            }
                            else
                            {
                                if (zzz > 10) break;
                                zzz++;
                            }

                            e.Spawn();
                        }


                        Console.WriteLine("17B|||||||||||||");
//                    Server.getInstance().getScheduler().scheduleDelayedTask(new SendItem(main, data), 20 * 5);
                        //Send items
                        //i
                       

                        var pk = new McpeRemoveEntity();
                        pk.entityIdSelf = eid2;
                        var pk2 = new McpeRemoveEntity();
                        pk2.entityIdSelf = eid;
                        p.SendPacket(pk);
                        p.SendPacket(pk2);

                        Console.WriteLine("18B|||||||||||||");
                        // Thread.Sleep(1000*60);
                        CTstop();
                        return;
                    }
                }
                else
                {
                    Thread.Sleep(80); //4 currentticks*2//200
                }
            }
        }


        public long generateEID()
        {
            return 1095216660480L + CyberUtils.LongRandom(0L, 2147483647L);
        }

        public int getDelayFromcurrenttick(long t)
        {
            var f = 0;
            if (t < MAX / 16)
                f = 2;
            else if (t < 2 * MAX / 16)
                f = 2;
            else if (t < 3 * MAX / 16)
                f = 2;
            else if (t < 4 * MAX / 16)
                f = 3;
            else if (t < 5 * MAX / 16)
                f = 4;
            else if (t < 6 * MAX / 16)
                f = 4;
            else if (t < 7 * MAX / 16)
                f = 4;
            else if (t < 8 * MAX / 16)
                f = 5;
            else if (t < 30 * MAX / 64)
                //9/16 > 18/32 > 36/64
                f = 6;
            else if (t < 35 * MAX / 64)
                f = 8;
            else if (t < 45 * MAX / 64)
                //12/16 >
                f = 13;
            else if (t < 55 * MAX / 64)
                //13/16 > 56/
                f = 17;
            else if (t < 60 * MAX / 64)
                f = 18;
            else if (t < 62 * MAX / 64)
                //15/16 > 30/32 > 45/48 > 60/64
                f = 19;
            else
                f = 20;

            //        f += 3;
            return f;
        }
    }
}