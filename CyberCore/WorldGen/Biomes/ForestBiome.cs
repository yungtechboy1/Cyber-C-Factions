using System;
using System.Threading.Tasks;
using CyberCore.WorldGen;
using CyberCore.WorldGen.Biomes;
using log4net;
using MiNET.Blocks;
using MiNET.Utils;
using MiNET.Worlds;

namespace CyberCore.WorldGen
{
    public class ForestBiome : AdvancedBiome
    {
        public static int ccc = 0;


        private static readonly ILog Log = LogManager.GetLogger(typeof(ForestBiome));

        public ForestBiome() : base("ForestBiome", new BiomeQualifications(.75f, 1.25f, 0.5f, 1.5f, 0.5f, 1.25f
            , 30))
        {
            LocalId = 2;
        }


        public override void GenerateVerticalColumn(int yheight, int maxheight, int x, int z, ChunkColumn cc,
            bool setair)
        {
            if (yheight == 0)
            {
                cc.SetBlock(x, yheight, z, new Bedrock());
            }
            else if (yheight <= maxheight - 5)
            {
                cc.SetBlock(x, yheight, z, new Stone());
            }
            else if (yheight < maxheight)
            {
                var r = RNDM.Next(0, 3);
                if (r == 0) cc.SetBlock(x, yheight, z, new Stone());
                if (r == 1) cc.SetBlock(x, yheight, z, new Dirt());
                if (r == 2) cc.SetBlock(x, yheight, z, new Dirt());
                if (r == 3) cc.SetBlock(x, yheight, z, new Stone());
            }else if(yheight==maxheight)
                cc.SetBlock(x,yheight,z, new Grass());
            else if (setair)
                cc.SetBlock(x, yheight, z, new Air());
        }


        
        
        /// <summary>
        /// </summary>
        /// <param name="openExperimentalWorldProvider"></param>
        /// <param name="chunk"></param>
        /// <param name="rth"></param>
        /// <returns></returns>
        public override async Task<ChunkColumn> GenerateSurfaceItems(
            CyberExperimentalWorldProvider o, ChunkColumn chunk, float[] rth)
        {
            var tg = new TreeGenerator(chunk)
            {
                // TreeRandom = 16,
                // MaxTreeHeight = 10,
                // MaxTreeWidth = 7
            };
            return tg.run();
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