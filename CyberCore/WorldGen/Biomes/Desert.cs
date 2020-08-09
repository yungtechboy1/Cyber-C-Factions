using System;
using CyberCore.WorldGen;
using CyberCore.WorldGen.Biomes;
using MiNET.Blocks;
using MiNET.Worlds;

namespace CyberCore.WorldGen.Biomes
{
    public class Desert : AdvancedBiome
    {
        public Desert() : base("Desert", new BiomeQualifications(0, 2, 1.75f, 2, 0.5f, 1
            , 30))
        {
        }

        public override int GetSH(int x, int z, int cx, int cz)
        {
            return BiomeQualifications.baseheight +
                   (int) GetNoise(cx * 16 + x, cz * 16 + z, /*rth[2] / */.035f,
                       BiomeQualifications.heightvariation);
        }

        public override void GenerateVerticalColumn(int yheight, int maxheight, int x, int z, ChunkColumn cc,
            bool setair)
        {
            if (yheight == 0)
            {
                cc.SetBlock(x, yheight, z, new Bedrock());
            }
            else if (yheight < maxheight - 1)
            {
                cc.SetBlock(x, yheight, z, new Stone());
            }
            else if (yheight < maxheight)
                cc.SetBlock(x, yheight, z, new RedSandstoneStairs());
            else if (setair)
                cc.SetBlock(x, yheight, z, new RedSandstoneStairs());
        }
    }
}