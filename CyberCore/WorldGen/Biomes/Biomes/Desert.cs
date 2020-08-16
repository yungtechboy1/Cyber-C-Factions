using System;
using CyberCore.WorldGen;
using CyberCore.WorldGen.Biomes;
using MiNET.Blocks;
using MiNET.Worlds;

namespace CyberCore.WorldGen.Biomes.Biomes
{
    public class Desert : AdvancedBiome
    {
        public Desert() : base("Desert", new BiomeQualifications(0, .75f, 1.5f, 2, 0.5f, 1.25f
            , 15))
        {
        }

        public override int GetSh(int x, int z, int cx, int cz)
        {
            return BiomeQualifications.Baseheight +
                   (int) GetNoise(cx * 16 + x, cz * 16 + z, /*rth[2] / */.035f,
                       BiomeQualifications.Heightvariation);
        }

        public override void GenerateVerticalColumn(int yheight, int maxheight, int x, int z, ChunkColumn cc,
            bool setair)
        {
            if (yheight == 0)
            {
                cc.SetBlock(x, yheight, z, new Bedrock());
            }
            else if (yheight < maxheight * 0.5f - 1)
                cc.SetBlock(x, yheight, z, new Stone());
            else if (yheight < maxheight * .75f - 1)
                cc.SetBlock(x, yheight, z, new Sandstone());
            else if (yheight < maxheight)
                cc.SetBlock(x, yheight, z, new Sand());
            else if (yheight == maxheight)
                cc.SetBlock(x, yheight, z, new Sand());
            else if (setair)
                cc.SetBlock(x, yheight, z, new Air());
        }
    }
}