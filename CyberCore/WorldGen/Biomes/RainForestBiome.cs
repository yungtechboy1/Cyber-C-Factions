using System;
using System.Threading.Tasks;
using log4net;
using MiNET.Blocks;
using MiNET.Utils;
using MiNET.Worlds;

namespace CyberCore.WorldGen.Biomes
{
    public class RainForestBiome : AdvancedBiome
    {
        public static int ccc = 0;


        private static readonly ILog Log = LogManager.GetLogger(typeof(RainForestBiome));

        public RainForestBiome() : base("RainForestBiome", new BiomeQualifications(1.25f, 2f, 1f, 2f, 0.5f, 1.25f
            , 30))
        {
            LocalId = 1;
        }


        public override void GenerateVerticalColumn(int yheight, int maxheight, int x, int z, ChunkColumn cc,
            bool setair)
        {
            cc.SetBiome(x, z, 4);
            if (yheight == 0)
            {
                cc.SetBlock(x, yheight, z, new Bedrock());
            }
            else if (yheight <= maxheight - 5)
            {
                TryOreGeneraton(cc, x, z, yheight);
            }
            else if (yheight < maxheight - 1)
            {
                var r = RNDM.Next(0, 3);
                if (r == 0) cc.SetBlock(x, yheight, z, new Stone());
                if (r == 1) cc.SetBlock(x, yheight, z, new Dirt());
                if (r == 2) cc.SetBlock(x, yheight, z, new Dirt());
                if (r == 3) cc.SetBlock(x, yheight, z, new Stone());
            }
            else if (yheight == maxheight - 1)
            {
                cc.SetBlock(x, yheight, z, new Grass());
            }
            else if (setair)
                cc.SetBlock(x, yheight, z, new Air());
        }

        /// <summary>
        /// </summary>
        /// <param name="o"></param>
        /// <param name="chunk"></param>
        /// <param name="rth"></param>
        /// <param name="openExperimentalWorldProvider"></param>
        /// <returns></returns>
        public override ChunkColumn GenerateSurfaceItems(CyberWorldProvider o, ChunkColumn chunk,
            float[] rth)
        {
            var cc = new TreeGenerator(chunk)
            {
                TreeRandom = 10,
                MaxTreeHeight = 20,

                // MaxTreeWidth = 8
            };
            ((Leaves) cc.LeavesItem).OldLeafType = "jungle";
            ((Wood) cc.WoodItem).WoodType = "jungle";
            var c = cc.run();
            c = new GrassGenerator(c)
            {
                GrassRandom = 11,
                GrassGreaterThan = 8,
                RainMultiplier = 1.3f
                // TreeRandom = 60
            }.run();
            // c = new TallGrassGenerator(c)
            // {
            //     GrassRandom = 10,
            //     GrassGreaterThan = 6
            // }.run();
            var f = new FlowerGenerator(c)
            {
                FlowerRandom = 100,
                FlowerGreaterThan = 60
            };
            f.FlowerChances.Add(new RedFlower()
            {
                FlowerType = "tulip_pink"
            }, 10);
            f.FlowerChances.Add(new RedFlower()
            {
                FlowerType = "houstonia"
            }, 10);
            f.FlowerChances.Add(new RedFlower()
            {
                FlowerType = "lily_of_the_valley"
            }, 10);
            f.FlowerChances.Add(new RedFlower()
            {
                FlowerType = "allium"
            }, 10);
            f.FlowerChances.Add(new RedFlower()
            {
                FlowerType = "poppy"
            }, 10);
            f.FlowerChances.Add(new RedFlower()
            {
                FlowerType = "cornflower"
            }, 10);
            f.FlowerChances.Add(new RedFlower()
            {
                FlowerType = "tulip_orange"
            }, 10);
            f.FlowerChances.Add(new RedFlower()
            {
                FlowerType = "oxeye"
            }, 10);
            f.FlowerChances.Add(new RedFlower()
            {
                FlowerType = "orchid"
            }, 10);

            c = f.run();
            c = new DoublePlantGenerator(c)
            {
                FlowerRandom = 100,
                FlowerGreaterThan = 40
            }.run();
            return c;
        }

        public override int GetSh(int x, int z, int cx, int cz)
        {
            return BiomeQualifications.Baseheight + 12 +
                   (int) GetNoise(cx * 16 + x, cz * 16 + z, /*rth[2] / */.035f,
                       BiomeQualifications.Heightvariation / 5);
        }

        // public override void PopulateChunk(CyberExperimentalWorldProvider o,
        //     ChunkColumn c, float[] rth, int[,] ints)
        // {
        //     // int sh =
        //
        //     for (var x = 0; x < 16; x++)
        //     for (var z = 0; z < 16; z++)
        //     {
        //         // float h = HeightNoise.GetNoise(c.X * 16 + x, c.Z * 16 + z)+1;
        //         // int sh= (int) Math.Floor(BiomeQualifications.baseheight + ((rth[2] )* BiomeQualifications.heightvariation))+(int)(HeightNoise.GetNoise(c.X * 16 + x, c.Z * 16 + z) * 10);
        //         // int sh= (int) Math.Floor(BiomeQualifications.baseheight + ((rth[2] )* BiomeQualifications.heightvariation))+(int)(GetNoise(c.X * 16 + x, c.Z * 16 + z,0.035f,10));
        //         // int sh = (int) (BiomeQualifications.baseheight +
        //         //                 (rth[2] * BiomeQualifications.heightvariation) +
        //         //                 (int) (GetNoise(c.X * 16 + x, c.Z * 16 + z, 0.035f, 5)));
        //         var sh = GetSH(x,z,c.X, c.Z);
        //         // (int) (GetNoise(c.X * 16 + x, c.Z * 16 + z, 0.035f, 5)); //10
        //         // Console.WriteLine("FORRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRR >>>>>>>>>>>>>>> " + sh + " |||| " + rth[2]);
        //
        //         // int sh = (int) (BiomeQualifications.baseheight +
        //         //                 (GetNoise(c.X * 16 + x, c.Z * 16 + z, 0.035f / 3,
        //         //                     BiomeQualifications.heightvariation)) +
        //         //                 (int) (GetNoise(c.X * 16 + x, c.Z * 16 + z, 0.035f, 5)));
        //
        //
        //         // int sh= (int) (BiomeQualifications.baseheight + GetNoise(c.X * 16 + x, c.Z * 16 + z,0.035f,(int)((rth[2] )* BiomeQualifications.heightvariation)+10));
        //         // int sh= (int) Math.Floor(BiomeQualifications.baseheight + ((rth[2])* BiomeQualifications.heightvariation));
        //         var fy = 0;
        //         for (var y = 0; y < 255; y++)
        //         {
        //             
        //
        //
        //             c.SetHeight(x, z, (short) y);
        //             fy = y;
        //             break;
        //         }
        //
        //         if (RNDM.Next(0, 64) < 20)
        //         {
        //             c.SetBlock(x, fy + 1, z, new Tallgrass());
        //             continue;
        //         }
        //
        //         if (RNDM.Next(0, 300) < 3)
        //         {
        //             if (RNDM.Next(0, 64) < 15)
        //             {
        //                 c.SetBlock(x, fy + 1, z, new DoublePlant());
        //                 c.SetBlock(x, fy + 2, z, new YellowFlower());
        //                 continue;
        //             }
        //
        //             c.SetBlock(x, fy + 1, z, new YellowFlower());
        //         }
        //
        //         if (RNDM.Next(0, 300) < 3)
        //         {
        //             if (RNDM.Next(0, 64) < 15)
        //             {
        //                 c.SetBlock(x, fy + 1, z, new DoublePlant());
        //                 c.SetBlock(x, fy + 2, z, new RedFlower());
        //                 continue;
        //             }
        //
        //             c.SetBlock(x, fy + 1, z, new RedFlower());
        //         }
        //
        //         if (RNDM.Next(0, 300) < 8)
        //         {
        //             if (RNDM.Next(0, 64) < 15)
        //             {
        //                 c.SetBlock(x, fy + 1, z, new DoublePlant());
        //                 c.SetBlock(x, fy + 2, z, new YellowFlower());
        //                 continue;
        //             }
        //
        //             c.SetBlock(x, fy + 1, z, new YellowFlower());
        //         }
        //     }
        // }
    }
}