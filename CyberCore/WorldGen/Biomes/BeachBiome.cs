using System;
using CyberCore.WorldGen;
using CyberCore.WorldGen.Biomes;
using MiNET.Blocks;
using MiNET.Worlds;

namespace CyberCore.WorldGen.Biomes
{
    public class BeachBiome : AdvancedBiome
    {


        public BeachBiome() : base("Beach", new BiomeQualifications(0, 2, 1, 1.75f, 0.25f, 0.5f
            , 10))
        {
            BiomeQualifications.Baseheight = 83; //30
            Waterlevel = 75;
        }

        public override void GenerateVerticalColumn(int yheight, int maxheight, int x, int z, ChunkColumn cc,
            bool setair)
        {
            //MAX
            //Sand
            //
            int sand = maxheight - new Random().Next(6, 10);
            // if (bid == new Water().Id || bid == new FlowingWater().Id) return;
            if (yheight < sand)
            {
                cc.SetBlock(x, yheight, z, new Stone());
            }
            else if (yheight <= maxheight - 3)
            {
                var r = new Random().Next(0, 10);
                if (r > 3)
                    cc.SetBlock(x, yheight, z, new Gravel());
                else
                    cc.SetBlock(x, yheight, z, new Sand());
            }
            else if (yheight < maxheight)
            {
                var r = new Random().Next(0, 10);
                if (r > 3)
                    cc.SetBlock(x, yheight, z, new Clay());
                else
                    cc.SetBlock(x, yheight, z, new Sand());
            }
            else if (setair)
                cc.SetBlock(x, yheight, z, new Air());
        }

        //TODO ADD CLAY
        public override int GetSh(int x, int z, int cx, int cz)
        {
            return BiomeQualifications.Baseheight +
                   (int) GetNoise(cx * 16 + x, cz * 16 + z, /*rth[2] / */.035f,
                       BiomeQualifications.Heightvariation);
        }
    }
}