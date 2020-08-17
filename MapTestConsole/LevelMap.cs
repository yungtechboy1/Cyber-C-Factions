using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using CyberCore.Manager.ClassFactory;
using CyberCore.WorldGen;
using CyberCore.WorldGen.Biomes;
using log4net.Core;
using MiNET.Blocks;
using MiNET.Utils;
using MiNET.Worlds;

namespace MapTestConsole
{
    public class LevelMap
    {
        public int X = 5;
        public int Z = 5;

        // public List<LevelMapChunk> data = new List<LevelMapChunk>();
        public enum LevelType
        {
            ChunkColumn,
            IntMap
        }

        public LevelType TType;
        public ChunkColumn[,] MapData;
        public int[,] intMapData;
        private int Size = 5;
        private int Offset = 5;
        private CyberExperimentalWorldProvider C;

        public LevelMap(CyberExperimentalWorldProvider cyberExperimentalWorldProvider, int size = 5, int offset = 0)
        {
            Size = size;
            Offset = offset;
            C = cyberExperimentalWorldProvider;
            new BiomeManager();
        }

        public void GenerateTestChunkMaps(String name = null)
        {
            TType = LevelType.ChunkColumn;
            var s = new Stopwatch();
            var ss = new Stopwatch();
            s.Restart();
            ss.Restart();
            X = Z = Size;
            MapData = new ChunkColumn[X, Z];
            for (int z = 0; z < MapData.GetLength(1); z++)
            {
                ss.Restart();
                for (int x = 0; x < MapData.GetLength(0); x++)
                {
                    // if (x + Offset == 60 && z + Offset == 55)
                    if (true)
                    {
                        Console.WriteLine($"ABOUYT TO START GENERATION FOR {x + Offset} {z + Offset}");
                        SingleChunkFiles(new ChunkCoordinates(x + Offset, Offset + z), C, name);
                        // MapData[x, z] = a;
                    }
                }

                ss.Stop();
                // Console.WriteLine($"TOTAL CHUNK FOR Z: {z} GENERATION TOOK " + ss.Elapsed + " FOR " +
                //                   MapData.GetLength(0) + " CHUNKS");
            }

            s.Stop();
            Console.WriteLine("TOTAL CHUNK GENERATION TOOK " + s.Elapsed +
                              $" FOR {MapData.GetLength(1) * MapData.GetLength(0)}");
        }

        public void SingleChunkFiles(ChunkCoordinates c, CyberExperimentalWorldProvider CyberExperimentalWorldProvider,
            String name = null)
        {
            int[,] m;
            int[,] f2;
            int[,] f3;
            int[,] f4;
            int[,] f5;
            int[,] f6;
            var b = BiomeManager.GetBiome(c);

            List<int[,]> l = new List<int[,]>();
            var f1 = b.GenerateChunkHeightMap(c, CyberExperimentalWorldProvider);
            l.Add(f1);
            if (b.BorderChunkDirections.Count > 0)
            {
                ChunkCoordinates sischunkcords;
                AdvancedBiome sischunkbiome;
                if (b.BorderChunkDirections.Contains(AdvancedBiome.BorderChunkDirection.South))
                {
                    //South Sis Chunk
                    sischunkcords = new ChunkCoordinates(c.X, c.Z - 1);
                    sischunkbiome = BiomeManager.GetBiome(sischunkcords);
                    if (sischunkbiome.BorderChunkDirections.Contains(AdvancedBiome.BorderChunkDirection.North))
                    {
                        //Generate A 16 X 16*2 Chunk map and Populate Sister Chunk
                        f2 = b.GenerateExtendedChunkHeightMap(AdvancedBiome.BorderChunkDirection.North, f1,
                            sischunkbiome.GenerateChunkHeightMap(sischunkcords, CyberExperimentalWorldProvider),
                            C);
                        f3 = b.LerpXZ2X(f2);
                        f4 = b.FinalCropTo16(f3, AdvancedBiome.BorderChunkDirection.North);
                        //FINISH SISTER CHUNK4
                        var mm = b.FinalCropTo16(f3, AdvancedBiome.BorderChunkDirection.South);
                        f5 = (int[,]) mm.Clone();
                        var nc = new ChunkColumn();
                        nc.X = sischunkcords.X;
                        nc.Z = sischunkcords.Z;
                        sischunkbiome.GenerateChunkFromSmoothOrder(C, nc,
                            BiomeManager.getChunkRTH(sischunkcords), mm);
                        C._chunkCache[sischunkcords] = nc;
                        l.Add(f2);
                        l.Add(f3);
                        l.Add(f4);
                        l.Add(f4);
                    }
                }
                else if (b.BorderChunkDirections.Contains(AdvancedBiome.BorderChunkDirection.North))
                {
                    //North Sis Chunk
                    sischunkcords = new ChunkCoordinates(c.X, c.Z + 1);
                    sischunkbiome = BiomeManager.GetBiome(sischunkcords);
                    if (sischunkbiome.BorderChunkDirections.Contains(AdvancedBiome.BorderChunkDirection.South))
                    {
                        //Generate A 16 X 16*2 Chunk map and Populate Sister Chunk
                        f2 = b.GenerateExtendedChunkHeightMap(AdvancedBiome.BorderChunkDirection.South, f1,
                            sischunkbiome.GenerateChunkHeightMap(sischunkcords, CyberExperimentalWorldProvider),
                            C);
                        f3 = b.LerpXZ2X(f2);
                        f4 = b.FinalCropTo16(f3, AdvancedBiome.BorderChunkDirection.South);
                        //FINISH SISTER CHUNK
                        var mm = b.FinalCropTo16(f3, AdvancedBiome.BorderChunkDirection.North);
                        f5 = mm;
                        var nc = new ChunkColumn();
                        nc.X = sischunkcords.X;
                        nc.Z = sischunkcords.Z;
                        sischunkbiome.GenerateChunkFromSmoothOrder(C, nc,
                            BiomeManager.getChunkRTH(sischunkcords), mm);
                        C._chunkCache[sischunkcords] = nc;
                        l.Add(f2);
                        l.Add(f3);
                        l.Add(f4);
                        l.Add(f4);
                    }
                }
            }

            // var f2 = b.GenerateExtendedChunkHeightMap(f1, c, C);
            // var f22 = b.CropToSmoothChunks(f2, c, C); 
            // var f3 = b.SmoothMapV2(f22);
            // var f4 = b.SmoothMapV3(f3);
            // var f33 = b.LerpX(f2);
            // var f44 = b.LerpZ(f33);
            // var f55 = b.LerpX(f44);
            // var f66 = b.LerpZ(f55);
            // var f77 = b.FinalCropTo16(f66);
            if (name != null && name.Length != 0)
            {
                name += "/";
            }
            else
            {
                name = "";
            }

            // l.Add(f1);
            // l.Add(f2);
            // // l.Add(f22);
            // l.Add(f33);
            // l.Add(f44);
            // l.Add(f55);
            // l.Add(f66);
            // l.Add(f77);
            Console.WriteLine("ABOUT TO SAVE " + c);
            SaveViaCSV($"/MapTesting/{name}chunk{c.X} {c.Z}-F1.csv",
                IntArrayToString(JoinIntMaps(l)) + "\n," + b.BorderChunkDirections.Count);
            // SaveViaCSV($"/MapTesting/{name}chunk{c.X} {c.Z}-F2.csv", IntArrayToString(f2));
            // SaveViaCSV($"/MapTesting/{name}chunk{c.X} {c.Z}-F3.csv", IntArrayToString(f3));
            // SaveViaCSV($"/MapTesting/{name}chunk{c.X} {c.Z}-F4.csv", IntArrayToString(f4));
        }

        public int[,] JoinIntMaps(List<int[,]> l)
        {
            int[,] r = new int[0, 0];
            if (l.Count == 0) return r;
            int s = (l.Count * 18) + (l.Count - 1);
            r = new int[40, 200];
            int zz = 0;
            foreach (var ll in l)
            {
                int za = ll.GetLength(1);
                int xa = ll.GetLength(0);
                for (int z = 0; z < za; z++)
                {
                    for (int x = 0; x < xa; x++)
                    {
                        r[x, z + zz] = ll[x, z];
                    }
                }

                zz += za + 1;
            }


            return r;
        }

        public void generateViaChunkColumn()
        {
            TType = LevelType.ChunkColumn;
            var s = new Stopwatch();
            var ss = new Stopwatch();
            s.Restart();
            ss.Restart();
            X = Z = Size;
            MapData = new ChunkColumn[X, Z];
            for (int z = 0; z < MapData.GetLength(1); z++)
            {
                ss.Restart();
                for (int x = 0; x < MapData.GetLength(0); x++)
                {
                    var a = C.OpenPreGenerateChunkColumn(new ChunkCoordinates(x + Offset, Offset + z));
                    MapData[x, z] = a;
                }

                ss.Stop();
                Console.WriteLine($"TOTAL CHUNK FOR Z: {z} GENERATION TOOK " + ss.Elapsed + " FOR " +
                                  MapData.GetLength(0) + " CHUNKS");
            }

            s.Stop();
            Console.WriteLine("TOTAL CHUNK GENERATION TOOK " + s.Elapsed +
                              $" FOR {MapData.GetLength(1) * MapData.GetLength(0)}");

            for (int z = 0; z < MapData.GetLength(1); z++)
            {
                ss.Restart();
                for (int x = 0; x < MapData.GetLength(0); x++)
                {
                    var a = C.OpenPreGenerateChunkColumn(new ChunkCoordinates(x + Offset, Offset + z),true,true);
                    MapData[x, z] = a;
                }
            }
        }

        public int[,] HeightDataToCSV()
        {
            var s = new Stopwatch();
            var ss = new Stopwatch();
            var sss = new Stopwatch();
            s.Restart();
            ss.Restart();
            sss.Restart();
            int maxx = X * 16;
            int maxz = Z * 16;
            var f = new int[maxx, maxz];
            for (int x = 0; x < MapData.GetLength(0); x++)
            {
                ss.Restart();
                for (int z = 0; z < MapData.GetLength(1); z++)
                {
                    sss.Restart();
                    var c = MapData[x, z];
                    for (int zz = 0; zz < 16; zz++)
                    {
                        for (int xx = 0; xx < 16; xx++)
                        {
                            int kx = x * 16 + xx;
                            int kz = z * 16 + zz;
                            f[kx, kz] = c.GetHeight(xx, zz);
                            // if (c.GetBlockId(xx, f[kx, kz], zz) == 9)
                            // {
                            //     
                            // }
                        }
                    }

                    sss.Stop();
                    Console.WriteLine($"GETTING HEIGHT DATA FOR CHUNK {x} {z} > TOOK {sss.Elapsed}");
                }

                ss.Stop();
                Console.WriteLine($"GETTING HEIGHT DATA FOR CHUNK COLUME {x} > TOOK {ss.Elapsed}");
            }

            s.Stop();
            Console.WriteLine($"TOTAL TIME FOR GETTING HEIGHT DATA FOR CHUNKZ > TOOK {s.Elapsed}");


            return f;
        }

        
        public static string IntArrayToString(float[,] d)
        {
            var ss = new Stopwatch();
            var sss = new Stopwatch();
            ss.Restart();
            sss.Restart();
            string s = "";
            //            for (int z = d.GetLength(1)-1; z >= 0; z--)
            for (int z = 0; z < d.GetLength(1); z++)
            {
                sss.Restart();
                for (int x = 0; x < d.GetLength(0); x++)
                {
                    // s += d[x, z] + ",";
                    s += String.Format("{0:0.00}", d[x, z]) + ",";
                }

                sss.Stop();
                Console.WriteLine($"TOOK {sss.Elapsed} TO CONVER COL {z}/{d.GetLength(1)}");

                s += "\n";
            }

            ss.Stop();
            Console.WriteLine("TIME THAT IT TOOK TO CONVERT TO STRING " + ss.Elapsed);
            return s;
        }
        public static string IntArrayToString(int[,] d)
        {
            var ss = new Stopwatch();
            var sss = new Stopwatch();
            ss.Restart();
            sss.Restart();
            StringBuilder s = new StringBuilder();
            //            for (int z = d.GetLength(1)-1; z >= 0; z--)
            for (int z = 0; z < d.GetLength(1); z++)
            {
                sss.Restart();
                for (int x = 0; x < d.GetLength(0); x++)
                {
                    s.Append(d[x, z] + ",");
                }

                sss.Stop();
                // Console.WriteLine($"TOOK {sss.Elapsed} TO CONVER COL {z}/{d.GetLength(1)}");

                s.Append("\n");
            }

            ss.Stop();
            // Console.WriteLine("TIME THAT IT TOOK TO CONVERT TO STRING " + ss.Elapsed);
            return s.ToString();
        }

        public static void SaveViaCSV(string datCsv, string text)
        {
            var s = new Stopwatch();
            s.Restart();
            System.IO.File.WriteAllText(@datCsv, text);
            s.Stop();
            Console.WriteLine("FILE WRITE TIME WAS " + s.Elapsed);
        }
    }
}