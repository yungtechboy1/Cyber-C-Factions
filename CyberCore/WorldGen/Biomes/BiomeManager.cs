using System;
using System.Collections.Generic;
using CyberCore.WorldGen.Biomes;
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

        private static int N;
        private static readonly Dictionary<int, AdvancedBiome> BiomeDict = new Dictionary<int, AdvancedBiome>();

        public BiomeManager()
        {
            // AddBiome(new MainBiome());
            AddBiome(new RainForestBiome());
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
            biome.LocalId = N;
            BiomeDict[N] = biome;
            Console.WriteLine($"BIOME ADD AT {N} WITH NAME {biome.Name}");
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

            float rain = rainnoise.GetNoise(chunk.X, chunk.Z) + 1;
            float temp = tempnoise.GetNoise(chunk.X, chunk.Z) + 1;
            float height = GetChunkHeightNoise(chunk.X, chunk.Z, 0.015f, 2);
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
            heightnoise.SetFractalOctaves(1);
            heightnoise.SetFractalLacunarity(2);
            heightnoise.SetFractalGain(.5f);
            return (heightnoise.GetNoise(x, z) + 1) * (max / 2f);
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

        public static void DoAdvancedStuff(AdvancedBiome biome, ChunkCoordinates chunk)
        {
                    Console.WriteLine($"BIOMEMANAGER 1: OK SO BIOME FOUND THAT MATCHES RTH NAMED {biome.Name} {biome.LocalId} @ {chunk.X} {chunk.Z}");
                    bool BC = false;
                    int bcc = 0;


                    bool n = false;
                    bool e = false;
                    bool s = false;
                    bool w = false;
                    bool nw = false;
                    bool ne = false;
                    bool sw = false;
                    bool se = false;

                    List<ChunkCoordinates> smoothing = new List<ChunkCoordinates>();
                    List<ChunkCoordinates> fsmoothing = new List<ChunkCoordinates>();
                    for (int zz = -1; zz <= 1; zz++)
                    for (int xx = -1; xx <= 1; xx++)
                    {
                        if (xx == 0 && zz == 0) continue;
                        var cc = new ChunkCoordinates()
                        {
                            X = chunk.X + xx,
                            Z = chunk.Z + zz
                        };
                        var tb = BiomeManager.GetBiome2(getChunkRTH(cc));
                        if (tb.LocalId != biome.LocalId)
                        {
                            bcc++;
                            // Console.WriteLine(
                            //     $"A BORDER CHUNK WAS FOUND! AT {cc.X} {cc.Z} AND {tb.name} {tb.LocalID} != {biome.LocalID} |||| {xx} {zz}");
                            // int dx = cc.X ;
                            // int dz = cc.Z;
                            if (zz == 1 && xx == 0)
                            {
                                n = true;
                                biome.BorderChunkDirections.Add(AdvancedBiome.BorderChunkDirection.North);
                            }
                            else if (zz == -1 && xx == 0)
                            {
                                s = true;
                                biome.BorderChunkDirections.Add(AdvancedBiome.BorderChunkDirection.South);
                            }

                            if (xx == 1 && zz == 0)
                            {
                                e = true;
                                biome.BorderChunkDirections.Add(AdvancedBiome.BorderChunkDirection.East);
                            }
                            else if (xx == -1 && zz == 0)
                            {
                                w = true;
                                biome.BorderChunkDirections.Add(AdvancedBiome.BorderChunkDirection.West);
                            }

                            if (xx == 1 && zz == 1)
                            {
                                ne = true;
                                biome.BorderChunkDirections.Add(AdvancedBiome.BorderChunkDirection.NE);
                            }

                            if (xx == 1 && zz == -1)
                            {
                                se = true;
                                biome.BorderChunkDirections.Add(AdvancedBiome.BorderChunkDirection.SE);
                            }

                            if (xx == -1 && zz == 1)
                            {
                                nw = true;
                                biome.BorderChunkDirections.Add(AdvancedBiome.BorderChunkDirection.NW);
                            }

                            if (xx == -1 && zz == -1)
                            {
                                sw = true;
                                biome.BorderChunkDirections.Add(AdvancedBiome.BorderChunkDirection.SW);
                            }

                            // Console.WriteLine("BIOMEMANAGER1.05: THE COUNT OF biome.BorderChunkDirections =>" +
                            //                   biome.BorderChunkDirections.Count);
                            // break;
                        }
                    }
                    
                    if (n)
                    {
                        if(!biome.BorderChunkDirections.Contains(AdvancedBiome.BorderChunkDirection.NE))biome.BorderChunkDirections.Add(AdvancedBiome.BorderChunkDirection.NE);
                        if(!biome.BorderChunkDirections.Contains(AdvancedBiome.BorderChunkDirection.NW))biome.BorderChunkDirections.Add(AdvancedBiome.BorderChunkDirection.NW);
                    }
                            
                            
                    if (s)
                    {
                        if(!biome.BorderChunkDirections.Contains(AdvancedBiome.BorderChunkDirection.SW))biome.BorderChunkDirections.Add(AdvancedBiome.BorderChunkDirection.SW);
                        if(!biome.BorderChunkDirections.Contains(AdvancedBiome.BorderChunkDirection.SE)) biome.BorderChunkDirections.Add(AdvancedBiome.BorderChunkDirection.SE);
                    }
                            
                            
                    if (w)
                    {
                        if(!biome.BorderChunkDirections.Contains(AdvancedBiome.BorderChunkDirection.SW))biome.BorderChunkDirections.Add(AdvancedBiome.BorderChunkDirection.SW);
                        if(!biome.BorderChunkDirections.Contains(AdvancedBiome.BorderChunkDirection.NW))biome.BorderChunkDirections.Add(AdvancedBiome.BorderChunkDirection.NW);
                    }
                            
                            
                    if (e)
                    {
                        if(!biome.BorderChunkDirections.Contains(AdvancedBiome.BorderChunkDirection.NE))biome.BorderChunkDirections.Add(AdvancedBiome.BorderChunkDirection.NE);
                        if(!biome.BorderChunkDirections.Contains(AdvancedBiome.BorderChunkDirection.SE))biome.BorderChunkDirections.Add(AdvancedBiome.BorderChunkDirection.SE);
                    }
                    
                    

                    //Calculate Size of Chunks needed to generated and Smoothed
                    // int sz = 0;
                    // if (n) sz++;
                    // if (e) sz++;
                    // if (s) sz++;
                    // if (w) sz++;
                    // if (nw) sz++;
                    // if (sw) sz++;
                    // if (se) sz++;
                    // if (ne) sz++;

                    int top = 0;
                    int left = 0;
                    int right = 0;
                    int bottom = 0;

                    if (n) top += 16;
                    if (w) left += 16;
                    if (e) right += 16;
                    if (s) bottom += 16;

                    int xs = 16 + top + bottom;
                    int zs = 16 + left + right;

                    // int[,] map = new int[xs, zs];

                    Console.WriteLine("BIOMEMANAGER2: THE BCC COUNT WAS >> " + bcc);
                    Console.WriteLine($"BIOMEMANAGER3: SIDES TOGGELD ${chunk.X}|{chunk.Z} N:{n} E:{e} S:{s} W:{w} NE:{ne} NW:{nw} SE:{se} SW:{sw}");
                    // if (bcc <= 3)
                    // {
                        biome.BorderChunk = true;
                    // }

                    // Console.WriteLine("BIOMEMANAGER4: THE COUNT OF biome.BorderChunkDirections =>" +
                    //                   biome.BorderChunkDirections.Count);

                    //Double Check Smoothing Directions
                    // if (biome.BorderChunkDirections.Contains(AdvancedBiome.BorderChunkDirection.NW) &&
                    //     biome.BorderChunkDirections.Contains(AdvancedBiome.BorderChunkDirection.North) &&
                    //     biome.BorderChunkDirections.Contains(AdvancedBiome.BorderChunkDirection.West))
                    // {
                    //     biome.BorderChunkDirections.Clear();
                    //     biome.BorderChunkDirections.Add(AdvancedBiome.BorderChunkDirection.NW);
                    //     biome.BorderChunkDirections.Add(AdvancedBiome.BorderChunkDirection.North);
                    //     biome.BorderChunkDirections.Add(AdvancedBiome.BorderChunkDirection.West);
                    // }
                    // else if (biome.BorderChunkDirections.Contains(AdvancedBiome.BorderChunkDirection.NE) &&
                    //          biome.BorderChunkDirections.Contains(AdvancedBiome.BorderChunkDirection.North) &&
                    //          biome.BorderChunkDirections.Contains(AdvancedBiome.BorderChunkDirection.East))
                    // {
                    //     biome.BorderChunkDirections.Clear();
                    //     biome.BorderChunkDirections.Add(AdvancedBiome.BorderChunkDirection.NE);
                    //     biome.BorderChunkDirections.Add(AdvancedBiome.BorderChunkDirection.North);
                    //     biome.BorderChunkDirections.Add(AdvancedBiome.BorderChunkDirection.East);
                    // }
                    // else if (biome.BorderChunkDirections.Contains(AdvancedBiome.BorderChunkDirection.SE) &&
                    //          biome.BorderChunkDirections.Contains(AdvancedBiome.BorderChunkDirection.South) &&
                    //          biome.BorderChunkDirections.Contains(AdvancedBiome.BorderChunkDirection.East))
                    // {
                    //     biome.BorderChunkDirections.Clear();
                    //     biome.BorderChunkDirections.Add(AdvancedBiome.BorderChunkDirection.SE);
                    //     biome.BorderChunkDirections.Add(AdvancedBiome.BorderChunkDirection.South);
                    //     biome.BorderChunkDirections.Add(AdvancedBiome.BorderChunkDirection.East);
                    // }
                    // else if (biome.BorderChunkDirections.Contains(AdvancedBiome.BorderChunkDirection.SW) &&
                    //          biome.BorderChunkDirections.Contains(AdvancedBiome.BorderChunkDirection.South) &&
                    //          biome.BorderChunkDirections.Contains(AdvancedBiome.BorderChunkDirection.West))
                    // {
                    //     biome.BorderChunkDirections.Clear();
                    //     biome.BorderChunkDirections.Add(AdvancedBiome.BorderChunkDirection.SW);
                    //     biome.BorderChunkDirections.Add(AdvancedBiome.BorderChunkDirection.South);
                    //     biome.BorderChunkDirections.Add(AdvancedBiome.BorderChunkDirection.West);
                    // }
                    // else
                    // {
                    //     Console.WriteLine("BIOMEMANAGER5: AYYYYYY THIS IS A NORTH EAST WST STH TYPE BOREDR SMOOTH");
                    //  
                    //     if (biome.BorderChunkDirections.Count == 1 && (
                    //             biome.BorderChunkDirections.Contains(AdvancedBiome.BorderChunkDirection.NE) ||
                    //             biome.BorderChunkDirections.Contains(AdvancedBiome.BorderChunkDirection.SW) ||
                    //             biome.BorderChunkDirections.Contains(AdvancedBiome.BorderChunkDirection.NW) ||
                    //             biome.BorderChunkDirections.Contains(AdvancedBiome.BorderChunkDirection.SE))
                    //     )
                    //     {
                    //         //Only Single Chunk was selected for Smoothing!
                    //     }
                    //     // bb.BorderChunkDirections.Clear();
                    // }

                    // Console.WriteLine("BIOMEMANAGER6: THE COUNT OF biome.BorderChunkDirections =>" +
                    //                   biome.BorderChunkDirections.Count);
                    // foreach (var b in biome.BorderChunkDirections)
                    // {
                    //     Console.WriteLine($"BIOMEMANAGER7: ======================>{b}");
                    // }
                    //
                    // //IF N and W are true
                    // if (biome.BorderChunkDirections.Contains(AdvancedBiome.BorderChunkDirection.North) && biome.BorderChunkDirections.Contains(AdvancedBiome.BorderChunkDirection.West))
                    // {
                    //                         
                    // }
                    //

                    //
                    // if (smoothing.Count > 0)
                    // {
                    //     foreach (var sc in smoothing)
                    //     {
                    //         var tc = c.GenerateChunkColumn(sc, true);
                    //         if(tc == null)fsmoothing.Add(sc);
                    //     }
                    //
                    //     foreach (var fc in fsmoothing)
                    //     {
                    //         biome.GenerateandSmooth.Add(fc);
                    //     }
                    // }
                    //
                    // CyberCoreMain.Log.Info($"GETTING BIOME BY RTH {rth} {rth[0]} {rth[1]} {rth[2]} returned {biome.name}");
                    
        }
        
        public static Dictionary<ChunkCoordinates,AdvancedBiome> BiomeCache = new Dictionary<ChunkCoordinates, AdvancedBiome>();
        
        public static AdvancedBiome GetBiome(ChunkCoordinates chunk)
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
                    if (bb.BorderChunkDirections.Count > 0)
                    {
                        Console.WriteLine("HUGE ERROR!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!"+bb.BorderChunkDirections.Count);
                        // bb.BorderChunkDirections.Clear();
                    }
                    AdvancedBiome biome = (AdvancedBiome) bb.CClone();
                    // Console.WriteLine($"AFTER CLONE {biome.BorderChunkDirections.Count} VS OLD {bb.BorderChunkDirections.Count}");
                    DoAdvancedStuff(biome,chunk);
                    Console.WriteLine($"BIOMEMANAGER9: GETTING BIOME BY RTH {rth} {rth[0]} {rth[1]} {rth[2]} returned {biome.Name}");
                    BiomeCache[chunk] = biome;
                    return biome;
                }

            // CyberCoreMain.Log.Info($"GETTING BIOME BY RTH {rth} {rth[0]} {rth[1]} {rth[2]} returned WATTTTTTTTTTTTTTTTTTTTTTTTTT");
            Console.WriteLine(
                $"BIOMEMANAGER10: GETTING BIOME BY RTH {rth} {rth[0]} {rth[1]} {rth[2]} returned WATTTTTTTTTTTTTTTTTTTTTTTTTT");
            // return new MainBiome();
            // return new WaterBiome();
            var bbb = new HighPlains().CClone();
            DoAdvancedStuff(bbb,chunk);
            BiomeCache[chunk] = bbb;
            return bbb;

        }

        public static AdvancedBiome GetBiome2(float[] rth)
        {
            foreach (var ab in Biomes)
                if (ab.Check(rth))
                {
                    var aa = ab.CClone();
                    return (AdvancedBiome) aa;
                }

            // return new MainBiome();
            // return new WaterBiome();
            return new HighPlains().CClone();
        }

        public static AdvancedBiome GetBiome2(ChunkCoordinates c)
        {
            var rth = getChunkRTH(new ChunkCoordinates()
            {
                X = c.X,
                Z = c.Z
            });

            foreach (var ab in Biomes)
                if (ab.Check(rth))
                {
                    var aa = ab.CClone();
                    return (AdvancedBiome) aa;
                }

            // return new MainBiome();
            // return new WaterBiome();
            return new HighPlains().CClone();
        }

        public static bool IsOnBorder(ChunkCoordinates chunkColumn, int localId, int size = 1)
        {
            for (int z = -size; z < size; z++)
            for (int x = -size; x < size; x++)
            {
                if (x == 0 && z == 0) continue;
                if (GetBiome(chunkColumn + new ChunkCoordinates(x, z)).LocalId != localId)
                {
                    return true;
                }
            }

            return false;
        }
    }
}