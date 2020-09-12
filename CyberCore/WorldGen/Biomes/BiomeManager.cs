using System;
using System.Collections.Generic;
using CyberCore.WorldGen.Biomes;
using CyberCore.WorldGen.Biomes.Biomes;
using CyberCore.WorldGen.Populator;
using log4net.Util.TypeConverters;
using MiNET.Utils;
using MiNET.Worlds;
using Org.BouncyCastle.Asn1.Smime;


namespace CyberCore.WorldGen
{
    public class BiomeManager
    {
        public static List<AdvancedBiome> Biomes = new List<AdvancedBiome>();

        private static int N = 0;
        private static readonly Dictionary<int, AdvancedBiome> BiomeDict = new Dictionary<int, AdvancedBiome>();
        private static AdvancedBiome TEST = new Plains();

        public BiomeManager()
        {
            AddBiome(TEST);
            // // AddBiome(new MainBiome());
            // AddBiome(new RainForestBiome());
            // AddBiome(new ForestBiome());
            // AddBiome(new SnowyIcyChunk());
            // AddBiome(new Desert());
            // AddBiome(new DesertHills());
            // AddBiome(new DesertLake());
            // AddBiome(new Mountains());//7
            // AddBiome(new Plains());
            // AddBiome(new HighPlains());
            // AddBiome(new WaterBiome());
            // // AddBiome(new BeachBiome());
            // AddBiome(new SnowForest());
            // AddBiome(new SnowTundra());
            // AddBiome(new TropicalRainForest());
            // AddBiome(new TropicalSeasonalForest());
        }

        public static void AddBiome(AdvancedBiome biome)
        {
            biome.BorderChunk = false;
            Biomes.Add(biome);
            BiomeDict[biome.LocalId] = biome;
            Console.WriteLine($"BIOME ADD AT {N} | {biome.LocalId} WITH NAME {biome.Name} {biome.BiomeQualifications}");
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

        // public static AdvancedBiome GetBiome(string name)
        // {
        //     foreach (var ab in Biomes)
        //         if (ab.name == name)
        //         {
        //             CyberCoreMain.Log.Info($"GETTING BIOME BY NAME {name} returned {ab.name}");
        //             return ab;
        //         }
        //
        //     CyberCoreMain.Log.Info($"GETTING BIOME BY NAME {name} returned WWWWWWWWWWWWWWWWWWWWWWWWWWWWWW");
        //
        //     return new WaterBiome();
        // }


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

            float rain = rainnoise.GetNoise(chunk.X, chunk.Z) + .75f;
            float temp = tempnoise.GetNoise(chunk.X, chunk.Z) + .75f;
            float height = GetChunkHeightNoise(chunk.X, chunk.Z, 0.008f, 2);
            ;
            return new[] {rain, temp, height};
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
        {
            //CALCULATE HEIGHT
            var heightnoise = new FastNoise(123123 + 2);
            heightnoise.SetNoiseType(FastNoise.NoiseType.SimplexFractal);
            heightnoise.SetFrequency(scale);
            heightnoise.SetFractalType(FastNoise.FractalType.FBM);
            heightnoise.SetFractalOctaves(2);
            heightnoise.SetFractalLacunarity(2);
            heightnoise.SetFractalGain(.5f);

            return (heightnoise.GetNoise(x, z) + .75f) * (max / 2f);
            return (float) (OpenNoise.Evaluate(x * scale, z * scale) + 1f) * (max / 2f);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="z"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static float GetHeightNoiseBlock(int x, int z)
        {
            //CALCULATE HEIGHT

            var heightnoise = new FastNoise(123123 + 2);
            heightnoise.SetNoiseType(FastNoise.NoiseType.SimplexFractal);
            heightnoise.SetFrequency(.015f / 16);
            heightnoise.SetFractalType(FastNoise.FractalType.FBM);
            heightnoise.SetFractalOctaves(1);
            heightnoise.SetFractalLacunarity(2);
            heightnoise.SetFractalGain(.5f);
            return (heightnoise.GetNoise(x, z) + 1);

            // return (float)(OpenNoise.Evaluate(x * scale, z * scale) + 1f) * (max / 2f);
        }

        public static float GetRainNoiseBlock(int x, int z)
        {
            //CALCULATE RAIN
            var rainnoise = new FastNoise(123123);
            rainnoise.SetNoiseType(FastNoise.NoiseType.SimplexFractal);
            rainnoise.SetFrequency(.007f / 16); //.015
            rainnoise.SetFractalType(FastNoise.FractalType.FBM);
            rainnoise.SetFractalOctaves(1);
            rainnoise.SetFractalLacunarity(.25f);
            rainnoise.SetFractalGain(1);
            return (rainnoise.GetNoise(x, z) + 1);
        }

        public static float GetTempNoiseBlock(int x, int z)
        {
            //CALCULATE TEMP
            var tempnoise = new FastNoise(123123 + 1);
            tempnoise.SetNoiseType(FastNoise.NoiseType.SimplexFractal);
            tempnoise.SetFrequency(.004f / 16); //.015f
            tempnoise.SetFractalType(FastNoise.FractalType.FBM);
            tempnoise.SetFractalOctaves(1);
            tempnoise.SetFractalLacunarity(.25f);
            tempnoise.SetFractalGain(1);
            return (tempnoise.GetNoise(x, z) + 1);
        }

        //CHECKED 5/10 @ 5:23 And this works fine!
        public static AdvancedBiome GetBiome(ChunkColumn chunk)
        {
            return GetBiome(new ChunkCoordinates(chunk.X, chunk.Z));
        }

        public static void DoAdvancedStuff(ref AdvancedBiome biome, ChunkCoordinates chunk)
        {
            // Console.WriteLine($"BIOMEMANAGER 1: OK SO BIOME FOUND THAT MATCHES RTH NAMED {biome.Name} {biome.LocalId} @ {chunk.X} {chunk.Z}");
            for (int zz = -1; zz <= 1; zz++)
            for (int xx = -1; xx <= 1; xx++)
            {
                if (xx == 0 && zz == 0) continue;
                var cc = new ChunkCoordinates()
                {
                    X = chunk.X + xx,
                    Z = chunk.Z + zz
                };
                var tb = GetBiome(cc, false);
                if (tb.LocalId != biome.LocalId)
                {
                    biome.BorderChunk = true;
                    break;
                }
            }

            biome = biome.DoubleCheckCords(chunk);
            // Console.WriteLine($"DONE WITH ADVANCED STUFFr");
        }

        public static Dictionary<ChunkCoordinates, AdvancedBiome> BiomeCache =
            new Dictionary<ChunkCoordinates, AdvancedBiome>();

        public static AdvancedBiome GetBiome(ChunkCoordinates chunk, bool doadvancedstuff = true)
        {
            if (BiomeCache.ContainsKey(chunk)) return BiomeCache[chunk];
            var rth = getChunkRTH(new ChunkCoordinates()
            {
                X = chunk.X,
                Z = chunk.Z
            });
            foreach (AdvancedBiome bb in Biomes)
                if (bb.Check(rth))
                {
                    AdvancedBiome biome = (AdvancedBiome) bb.CClone();
                    // Console.WriteLine($"AFTER CLONE {biome.BorderChunkDirections.Count} VS OLD {bb.BorderChunkDirections.Count}");
                    if (doadvancedstuff) DoAdvancedStuff(ref biome, chunk);
                    // Console.WriteLine($"BIOMEMANAGER9: GETTING BIOME BY RTH {rth} {rth[0]} {rth[1]} {rth[2]} returned {biome.Name}");
                    if (doadvancedstuff) BiomeCache[chunk] = biome;
                    return biome;
                }

            // CyberCoreMain.Log.Info($"GETTING BIOME BY RTH {rth} {rth[0]} {rth[1]} {rth[2]} returned WATTTTTTTTTTTTTTTTTTTTTTTTTT");
            // Console.WriteLine($"BIOMEMANAGER10: GETTING BIOME BY RTH {rth} {rth[0]} {rth[1]} {rth[2]} returned WATTTTTTTTTTTTTTTTTTTTTTTTTT");
            // return new MainBiome();
            // return new WaterBiome();
            // var bbb = new WaterBiome().CClone();
            var bbb = TEST.CClone();
            // var bbb = new DesertLake().CClone();
            // bbb.LocalId = 7;
            if (doadvancedstuff) DoAdvancedStuff(ref bbb, chunk);
            if (doadvancedstuff) BiomeCache[chunk] = bbb;
            return bbb;
        }
    }
}