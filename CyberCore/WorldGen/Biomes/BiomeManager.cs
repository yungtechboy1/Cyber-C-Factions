﻿﻿using System;
using System.Collections.Generic;
 using CyberCore.WorldGen.Biomes;
 using CyberCore.WorldGen.Populator;
 using MiNET.Utils;
using MiNET.Worlds;


 namespace CyberCore.WorldGen
{
    public class BiomeManager
    {
        public static List<AdvancedBiome> Biomes = new List<AdvancedBiome>();

        private static int N;
        private static readonly Dictionary<int, AdvancedBiome> BiomeDict = new Dictionary<int, AdvancedBiome>();

        public BiomeManager()
        {
            // AddBiome(new MainBiome());
            AddBiome(new ForestBiome());
            AddBiome(new SnowyIcyChunk());
            AddBiome(new Desert());
            AddBiome(new Mountains());
            AddBiome(new Plains());
            AddBiome(new HighPlains());
            AddBiome(new WaterBiome());
            AddBiome(new BeachBiome());
            AddBiome(new SnowForest());
            AddBiome(new SnowTundra());
            AddBiome(new SnowyIcyChunk());
            AddBiome(new TropicalRainForest());
            AddBiome(new TropicalSeasonalForest());
        }

        public static void AddBiome(AdvancedBiome biome)
        {
            biome.BorderChunk = false;
            Biomes.Add(biome);
            biome.LocalID = N;
            BiomeDict[N] = biome;
            N++;
        }

        // public static AdvancedBiome GetBiomeLocalID(string name)
        // {
        //     foreach (var ab in Biomes)
        //         if (ab.LocalID == name)
        //             return ab;
        //
        //     return new SnowyIcyChunk();
        // }

        public static AdvancedBiome GetBiome(string name)
        {
            foreach (var ab in Biomes)
                if (ab.name == name)
                {
                    CyberCoreMain.Log.Info($"GETTING BIOME BY NAME {name} returned {ab.name}");
                    return ab;
                }
            CyberCoreMain.Log.Info($"GETTING BIOME BY NAME {name} returned WWWWWWWWWWWWWWWWWWWWWWWWWWWWWW");

            return new WaterBiome();
        }
        
        
         public static float[] getChunkRTH(ChunkCoordinates chunk)
        {
            //CALCULATE RAIN
            var rainnoise = new FastNoise(123123);
            rainnoise.SetNoiseType(FastNoise.NoiseType.SimplexFractal);
            rainnoise.SetFrequency(.007f); //.015
            rainnoise.SetFractalType(FastNoise.FractalType.FBM);
            rainnoise.SetFractalOctaves(1);
            rainnoise.SetFractalLacunarity(.25f);
            rainnoise.SetFractalGain(1);
            //CALCULATE TEMP
            var tempnoise = new FastNoise(123123 + 1);
            tempnoise.SetNoiseType(FastNoise.NoiseType.SimplexFractal);
            tempnoise.SetFrequency(.004f); //.015f
            tempnoise.SetFractalType(FastNoise.FractalType.FBM);
            tempnoise.SetFractalOctaves(1);
            tempnoise.SetFractalLacunarity(.25f);
            tempnoise.SetFractalGain(1);
            
            float rain = rainnoise.GetNoise(chunk.X, chunk.Z) + 1;
            float temp = tempnoise.GetNoise(chunk.X, chunk.Z) + 1;
            float height = GetChunkHeightNoise(chunk.X, chunk.Z, 0.015f,2);;
            return new []{rain, temp, height};
        }

         private static readonly OpenSimplexNoise OpenNoise = new OpenSimplexNoise("a-seed".GetHashCode());


         /// <summary>
         /// 
         /// </summary>
         /// <param name="x"></param>
         /// <param name="z"></param>
         /// <param name="scale"></param>
         /// <param name="max"></param>
         /// <returns></returns>
         public static float GetChunkHeightNoise(int x, int z, float scale, int max)
         {//CALCULATE HEIGHT
             var heightnoise = new FastNoise(123123 + 2);
             heightnoise.SetNoiseType(FastNoise.NoiseType.SimplexFractal);
             heightnoise.SetFrequency(scale);
             heightnoise.SetFractalType(FastNoise.FractalType.FBM);
             heightnoise.SetFractalOctaves(1);
             heightnoise.SetFractalLacunarity(2);
             heightnoise.SetFractalGain(.5f);
             return (heightnoise.GetNoise(x, z)+1 )*(max/2f);
             return (float)(OpenNoise.Evaluate(x * scale, z * scale) + 1f) * (max / 2f);
         }
           /// <summary>
           /// 
           /// </summary>
           /// <param name="x"></param>
           /// <param name="z"></param>
           /// <param name="max"></param>
           /// <returns></returns>
           public static float GetHeightNoiseBlock(int x, int z)
           {//CALCULATE HEIGHT
             
               var heightnoise = new FastNoise(123123 + 2);
               heightnoise.SetNoiseType(FastNoise.NoiseType.SimplexFractal);
               heightnoise.SetFrequency(.015f/16);
               heightnoise.SetFractalType(FastNoise.FractalType.FBM);
               heightnoise.SetFractalOctaves(1);
               heightnoise.SetFractalLacunarity(2);
               heightnoise.SetFractalGain(.5f);
               return (heightnoise.GetNoise(x, z)+1 );
                              
               // return (float)(OpenNoise.Evaluate(x * scale, z * scale) + 1f) * (max / 2f);
           }public static float GetRainNoiseBlock(int x, int z)
           {//CALCULATE RAIN
               var rainnoise = new FastNoise(123123);
               rainnoise.SetNoiseType(FastNoise.NoiseType.SimplexFractal);
               rainnoise.SetFrequency(.007f/16); //.015
               rainnoise.SetFractalType(FastNoise.FractalType.FBM);
               rainnoise.SetFractalOctaves(1);
               rainnoise.SetFractalLacunarity(.25f);
               rainnoise.SetFractalGain(1);
               return (rainnoise.GetNoise(x, z)+1 );
           }
           public static float GetTempNoiseBlock(int x, int z)
           {//CALCULATE TEMP
               var tempnoise = new FastNoise(123123 + 1);
               tempnoise.SetNoiseType(FastNoise.NoiseType.SimplexFractal);
               tempnoise.SetFrequency(.004f/16); //.015f
               tempnoise.SetFractalType(FastNoise.FractalType.FBM);
               tempnoise.SetFractalOctaves(1);
               tempnoise.SetFractalLacunarity(.25f);
               tempnoise.SetFractalGain(1);
               return (tempnoise.GetNoise(x, z)+1 );
           }
         
        //CHECKED 5/10 @ 5:23 And this works fine!
        public static AdvancedBiome GetBiome(ChunkColumn chunk)
        {
            return GetBiome(new ChunkCoordinates(chunk.X, chunk.Z));
        }

        public static AdvancedBiome GetBiome(ChunkCoordinates chunk)
        {
            var rth = getChunkRTH(new ChunkCoordinates()
                    {
                        X = chunk.X,
                        Z = chunk.Z
                    });
            foreach (var biome in Biomes)
                if (biome.check(rth))
                {
                    bool BC = false;
                    for (int zz = -1; zz <= 1; zz++)
                    for (int xx = -1; xx <= 1; xx++)
                    {
                        if (xx == 0 && zz == 0) continue;
                        var tb = BiomeManager.GetBiome2(getChunkRTH(new ChunkCoordinates()
                        {
                            X = chunk.X+xx,
                            Z = chunk.Z+zz
                        }));
                        if (tb.LocalID != biome.LocalID)
                        {
                            BC = true;
                            break;
                        }
                    }
                    
                    
                    biome.BorderChunk = BC;
                    // CyberCoreMain.Log.Info($"GETTING BIOME BY RTH {rth} {rth[0]} {rth[1]} {rth[2]} returned {biome.name}");
            
                    return biome;
                }

            CyberCoreMain.Log.Info($"GETTING BIOME BY RTH {rth} {rth[0]} {rth[1]} {rth[2]} returned WATTTTTTTTTTTTTTTTTTTTTTTTTT");
            // return new MainBiome();
            return new WaterBiome();
            // return new HighPlains();
        }

        public static AdvancedBiome GetBiome2(float[] rth)
        {
            foreach (var ab in Biomes)
                if (ab.check(rth))
                    return ab;

            // return new MainBiome();
            return new WaterBiome();
            // return new HighPlains();
        }

        public static bool IsOnBorder(ChunkCoordinates chunkColumn, int localId, int size = 1)
        {
            for (int z = -size; z < size; z++)
            for (int x = -size; x < size; x++)
            {
                if (x == 0 && z == 0) continue;
                if (GetBiome(chunkColumn + new ChunkCoordinates(x, z)).LocalID != localId)
                {
                    return true;
                }

            }

            return false;
        }
    }
}