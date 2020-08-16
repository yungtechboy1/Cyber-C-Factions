using System;
using System.Collections.Generic;
using CyberCore.WorldGen;
using CyberCore.WorldGen.Biomes;
using LibNoise.Renderer;
using MiNET.Worlds;

namespace MapTestConsole
{
    public class LevelMapChunk
    {
        private int X;
        private int Z;

        public ChunkColumn Chunk;
        public AdvancedBiome Biome;
        public int[,] HeightMap;
        // public 
        
        public LevelMapChunk(int x, int z, CyberExperimentalWorldProvider cyberExperimentalWorldProvider)
        {
            X = x;
            Z = z;
            HeightMap =  new int[16,16];
            Chunk = new ChunkColumn();
            Chunk.X = x;
            Chunk.Z = z;
            float[] rth = cyberExperimentalWorldProvider.getChunkRTH(x,z);
            // Biome = BiomeManager.GetBiome2(rth);
            // Chunk = Biome.prePopulate(cyberExperimentalWorldProvider,Chunk,rth).Result;
            
        }
    }
}