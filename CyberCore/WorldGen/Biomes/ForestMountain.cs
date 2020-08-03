using System;
using CyberCore.WorldGen;
using CyberCore.WorldGen.Biomes;
using MiNET.Blocks;
using MiNET.Worlds;

namespace CyberCore.WorldGen.Biomes
{
    public class ForestMountain : AdvancedBiome
    {
        public ForestMountain() : base("ForestMountain", new BiomeQualifications(1, 2, .5f, 1.75f, 1.25f, 2, 30))
        {
        }

        public override int GetSH(int x, int z, int cx, int cz)
        {
            return BiomeQualifications.baseheight +
                   (int) GetNoise(cx * 16 + x, cz * 16 + z, /*rth[2] / */.035f,
                       BiomeQualifications.heightvariation);
        }

        public override void GenerateVerticalColumn(int yheight, int maxheight, int x, int z, ChunkColumn cc)
        {
            if (yheight == 0)
            {
                cc.SetBlock(x, yheight, z, new Bedrock());
            }
            else if (yheight <= maxheight)
            {
                cc.SetBlock(x, yheight, z, new Stone());
            }
            else
            {
                var w = new Wool();
                w.Color = "green";
                cc.SetBlock(x, yheight, z, w);
            }
        }
    }
}