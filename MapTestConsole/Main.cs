using System;
using System.Collections.Generic;
using System.Diagnostics;
using CyberCore.WorldGen;
using MiNET.Utils;

namespace MapTestConsole
{
    public class Main
    {
        private CyberExperimentalWorldProvider c = new CyberExperimentalWorldProvider(123,"/TEST/");
        public LevelMap LM;
        public Main()
        {
            new BiomeManager();
            // LM = new LevelMap(c,11,50);
            // // LM.GenerateTestChunkMaps();
            // LM.generateViaChunkColumn();
            // LevelMap.SaveViaCSV("/MapTesting/dat2.csv",LevelMap.IntArrayToString(LM.HeightDataToCSV()));
            var s = new Stopwatch();
            s.Restart();
            var a = GenerateBiomeMap(250);
            LevelMap.SaveViaCSV("/MapTesting/BIOME.csv",LevelMap.IntArrayToString(a));
            Console.WriteLine($"THIS TOOK {s.Elapsed}");
            // var aa = GenerateBiomeHeightMap(250);
            // LevelMap.SaveViaCSV("/MapTesting/BIOMERRR.csv",LevelMap.IntArrayToString(aa[0]));
            // LevelMap.SaveViaCSV("/MapTesting/BIOMETTT.csv",LevelMap.IntArrayToString(aa[1]));
            // LevelMap.SaveViaCSV("/MapTesting/BIOMEHHH.csv",LevelMap.IntArrayToString(aa[2]));
        }

        public int[,] GenerateBiomeMap(int size)
        {
            int[,] m = new int[size,size];
            for (int z = 0; z < size; z++)
            for (int x = 0; x < size; x++)
            {
                m[x, z] = BiomeManager.GetBiome(new ChunkCoordinates(x, z)).LocalId;
            }

            return m;
        }
        
        public List<float[,]> GenerateBiomeHeightMap(int size)
        {
            float[,] r = new float[size,size];
            float[,] t = new float[size,size];
            float[,] h = new float[size,size];
            for (int z = 0; z < size; z++)
            for (int x = 0; x < size; x++)
            {
                var v = BiomeManager.getChunkRTH(new ChunkCoordinates(x, z));
                r[x, z] = v[0];
                t[x, z] = v[1];
                h[x, z] = v[2];
            }

            List<float[,]> a = new List<float[,]>();
            a.Add(r);
            a.Add(t);
            a.Add(h);
            return a;
        }
    }
}