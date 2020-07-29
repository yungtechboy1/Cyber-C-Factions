﻿using System;
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

        public override void PopulateChunk(CyberExperimentalWorldProvider CyberExperimentalWorldProvider, ChunkColumn c,
            float[] rth)
        {for (var x = 0; x < 16; x++)
            for (var z = 0; z < 16; z++){
                
                var sh = GetSH(x,z,c.X,c.Z);
            for (var y = 0; y < 255; y++)
            {
                if (y == 0)
                {
                    c.SetBlock(x, y, z, new Bedrock());
                    continue;
                }

                if (y <= sh)
                {
                    c.SetBlock(x, y, z, new Stone());
                    continue;
                }

                var w = new Wool();
                w.Color = "green";
                c.SetBlock(x, y, z, w);
                c.SetHeight(x, z, (short) y);
                break;
            }
        }
        }
    }

}