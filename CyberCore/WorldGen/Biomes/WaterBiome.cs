using System;
using CyberCore.WorldGen;
using CyberCore.WorldGen.Biomes;
using log4net.Util.TypeConverters;
using MiNET.Blocks;
using MiNET.Utils;
using MiNET.Worlds;

namespace CyberCore.WorldGen
{
    public class WaterBiome : AdvancedBiome
    {
        public WaterBiome() : base("Water", new BiomeQualifications(0, 2, 1, 1.75f, 0.25f, 0f
            , 30))
        {
            BiomeQualifications.Baseheight = 30;
            LocalId = 10;
        }

        public override AdvancedBiome DoubleCheckCords(ChunkCoordinates chunk)
        {
            // BorderChunk = false;
            for (int zz = -2; zz <= 2; zz++)
            for (int xx = -2; xx <= 2; xx++)
            {
                if (xx == 0 && zz == 0) continue;
                var cc = new ChunkCoordinates()
                {
                    X = chunk.X + xx,
                    Z = chunk.Z + zz
                };
                var tb = BiomeManager.GetBiome(cc,false);
                // Console.WriteLine($"SO WHILE CHECKING WATER {LocalId} I FOUND TB {tb.LocalId} AT {cc} VIA {chunk}");
                if (tb.LocalId != LocalId && tb.LocalId != 11)
                {
                    // Console.WriteLine($"Yeahhh this got changed!");
                    var b = new BeachBiome().CClone();
                    BiomeManager.DoAdvancedStuff(ref b,chunk);
                    return b;
                }
            }

            BorderChunk = false;
      

            return this;
        }

        public override int[,] GenerateChunkHeightMap(ChunkCoordinates c, CyberExperimentalWorldProvider cyberExperimentalWorldProvider)
        {
            var r = new int[16, 16];

            for (int z = 0; z < 16; z++)
            for (int x = 0; x < 16; x++)
            {
                // Console.WriteLine($"{c.X} *16 + {x} = {c.X * 16 + x} || {c.Z}*16 + {z} = {c.Z * 16 + z}");
                // r[x, z] = cyberExperimentalWorldProvider.getBlockHeight(c.X * 16 + x, c.Z * 16 + z);
                r[x, z] = Waterlevel;
            }
            

            return r;
        }

        public override void GenerateVerticalColumn(int yheight, int maxheight, int x, int z, ChunkColumn cc,
            bool setair)
        {
            maxheight =  CyberExperimentalWorldProvider.getInstance().getBlockHeight(cc.X * 16 + x, cc.Z * 16 + z);
            int sand = maxheight - 6;
            if (yheight < sand)
            {
                cc.SetBlock(x, yheight, z, new Stone());
            }
            // else if (cc.GetBlockId(rx, y, rz) == 0) break;
            else if (yheight <= maxheight - 1)
            {
                // if (x == 0 || z == map.GetLength(0) - 1 || z == 0 || z == map.GetLength(1) - 1)
                //     cc.SetBlock(rxx, y, rzz, new EmeraldBlock());
                /*else*/
                var r = new Random().Next(0, 10);
                if (r > 3)
                    cc.SetBlock(x, yheight, z, new Gravel());
                else if (r > 5)
                    cc.SetBlock(x, yheight, z, new Clay());
                else
                    cc.SetBlock(x, yheight, z, new Sand());
            }
            else if (yheight == maxheight)
                cc.SetBlock(x, yheight, z, new Sand());
            else if (yheight <= Waterlevel)
                cc.SetBlock(x, yheight, z, new Water());
            else if (setair)
                cc.SetBlock(x, yheight, z, new Air());
        }

        public override int GetSh(int x, int z, int cx, int cz)
        {
            return (int) (BiomeQualifications.Baseheight +
                          (int) (GetNoise(cx * 16 + x, cz * 16 + z, /*rth[2] / */.035f,
                              BiomeQualifications.Heightvariation)));
        }
    }
}