﻿using System;
using CyberCore.WorldGen;
using CyberCore.WorldGen.Biomes;
using MiNET.Blocks;
using MiNET.Worlds;

namespace CyberCore.WorldGen.Biomes
{
    public class TropicalSeasonalForest : AdvancedBiome
    {
        public TropicalSeasonalForest() : base("TropicalSeasonalForest", new BiomeQualifications(0.5f, 1.25f, 0f,
            0.5f,
            0.5f, 1.5f
            , 30))
        {
            LocalId = 15;
        }

        public override int GetSh(int x, int z, int cx, int cz)
        {
            return BiomeQualifications.Baseheight +
                   (int) GetNoise(cx * 16 + x, cz * 16 + z, /*rth[2] / */.035f,
                       BiomeQualifications.Heightvariation);
        }

        //     public override void PopulateChunk(CyberExperimentalWorldProvider CyberExperimentalWorldProvider,
        //         ChunkColumn c, float[] rth, int[,] ints)
        //     {
        //         for (var x = 0; x < 16; x++)
        //         for (var z = 0; z < 16; z++)
        //         {
        //             var sh = GetSH(x, z, c.X, c.Z);
        //         
        //         for (var y = 0; y < 255; y++)
        //         {
        //             if (y == 0)
        //             {
        //                 c.SetBlock(x, y, z, new Bedrock());
        //                 continue;
        //             }
        //
        //             if (y <= sh)
        //             {
        //                 c.SetBlock(x, y, z, new Stone());
        //                 continue;
        //             }
        //
        //             c.SetBlock(x, y, z, new RedstoneOre());
        //             c.SetHeight(x, z, (short) y);
        //             break;
        //         }
        //     }
        // }

        public override void GenerateVerticalColumn(int yheight, int maxheight, int x, int z, ChunkColumn cc, bool setair, bool objectcopy)
        {
            if (yheight < maxheight-1)
            {
                cc.SetBlock(x, yheight, z, new Stone());
            }
            else if(yheight < maxheight)
                cc.SetBlock(x, yheight, z, new RedstoneOre());
            else if(setair)
                cc.SetBlock(x,yheight,z,new Air());
        }
    }
}