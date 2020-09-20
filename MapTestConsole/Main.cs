using System;
using System.Collections.Generic;
using System.Diagnostics;
using CyberCore.WorldGen;
using log4net.Util.TypeConverters;
using MiNET.Utils;

namespace MapTestConsole
{
    public class Main
    {
        private CyberExperimentalWorldProvider c = new CyberExperimentalWorldProvider(123, "/TEST/");
        public LevelMap LM;

        public Main()
        {
            new BiomeManager();
            var cc = new ChunkCoordinates(8,-6);
            var b = BiomeManager.GetBiome(cc);
            var i = b.GenerateChunkHeightMap(cc, c);
            LevelMap.SaveViaCSV("/MapTesting/StripSmoothTestOOOOO.csv",LevelMap.IntArrayToString(i));
            // SmoothingMap sm = new SmoothingMap(cc,i);
           // var sm = b.HandleGeneration(i, cc, c);
           var sm = new SmoothingMap(cc);
            LevelMap.SaveViaCSV("/MapTesting/StripSmoothTestMASSB4.csv",LevelMap.IntArrayToString(sm.Map));
            // sm.StripSmooth(1);//4
            sm.SmoothMapV4();
            LevelMap.SaveViaCSV("/MapTesting/StripSmoothTestMASSAFTER.csv",LevelMap.IntArrayToString(sm.Map));
            // LM = new LevelMap(c,11,50);
            // // LM.GenerateTestChunkMaps();
            // LM.generateViaChunkColumn();
            LevelMap.SaveViaCSV("/MapTesting/StripSmoothTest.csv",LevelMap.IntArrayToString(sm.GetChunk(sm.getCenterCords())));
            // LevelMap.SaveViaCSV("/MapTesting/dat2.csv",LevelMap.IntArrayToString(LM.HeightDataToCSV()));
            var s = new Stopwatch();
            s.Restart();
            // var a = GenerateBiomeMap(250);
            // LevelMap.SaveViaCSV("/MapTesting/BIOME.csv", LevelMap.IntArrayToString(a));
            Console.WriteLine($"THIS TOOK {s.Elapsed}");
            // var aa = GenerateBiomeHeightMap(250);
            // LevelMap.SaveViaCSV("/MapTesting/BIOMERRR.csv",LevelMap.IntArrayToString(aa[0]));
            // LevelMap.SaveViaCSV("/MapTesting/BIOMETTT.csv",LevelMap.IntArrayToString(aa[1]));
            // LevelMap.SaveViaCSV("/MapTesting/BIOMEHHH.csv",LevelMap.IntArrayToString(aa[2]));
        }
   
        public int[,] GenerateBiomeMap(int size)
        {
            int[,] m = new int[size * 2+1, size * 2+1];
            int zz = 0;
            for (int z = -size; z < size; z++)
            {
                int xx = 0;
                for (int x = -size; x < size; x++)
                {
                    // Console.WriteLine(xx+" || "+zz);
                    m[xx, zz] = BiomeManager.GetBiome(new ChunkCoordinates(x, z)).LocalId;
                    xx++;
                }
                zz++;
                Console.WriteLine("ON ZZ "+zz+" || "+z);
            }

            return m;
        }

        public List<float[,]> GenerateBiomeHeightMap(int size)
        {
            float[,] r = new float[size, size];
            float[,] t = new float[size, size];
            float[,] h = new float[size, size];
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