using System;
using System.Data;
using CyberCore.WorldGen;
using CyberCore.WorldGen.Biomes;
using MiNET.Blocks;
using MiNET.Worlds;

namespace CyberCore.WorldGen.Biomes
{
    public class Mountains : AdvancedBiome
    {
        public Mountains() : base("Mountains", new BiomeQualifications(.25f, 1, .75f, 1.75f, 1.25f, 2, 40))
        {
            BiomeQualifications.Baseheight += 10;
            
            LocalId = 7;
        }

        public override int GetSh(int x, int z, int cx, int cz)
        {
            return BiomeQualifications.Baseheight +
                   (int) GetNoise(cx * 16 + x, cz * 16 + z, /*rth[2] / */.015f,
                       BiomeQualifications.Heightvariation);
        }

        public override void GenerateVerticalColumn(int yheight, int maxheight, int x, int z, ChunkColumn cc,
            bool setair)
        {
            if (yheight == 0)
            {
                cc.SetBlock(x, yheight, z, new Bedrock());
            }
            else if (yheight < maxheight)
            {
                TryOreGeneraton(cc,x,z,yheight);
            }
            else if (setair)
                cc.SetBlock(x, yheight, z, new Air());
        }

        // public override void PopulateChunk(CyberExperimentalWorldProvider CyberExperimentalWorldProvider, ChunkColumn c,
        //     float[] rth, int[,] ints)
        // {
        //     for (var x = 0; x < 16; x++)
        //     for (var z = 0; z < 16; z++)
        //     {
        //         var sh = GetSH(x,z,c.X, c.Z);
        //     for (var y = 0; y < 255; y++)
        //     {
        //         
        //         c.SetHeight(x, z, (short) y);
        //         break;
        //     }
        // }
        // }
    }
}