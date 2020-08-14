using System;
using CyberCore.WorldGen;
using CyberCore.WorldGen.Biomes;
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
        }


        public override void GenerateVerticalColumn(int yheight, int maxheight, int x, int z, ChunkColumn cc,
            bool setair)
        {
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