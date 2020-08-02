using System;
using System.Collections.Generic;
using System.Diagnostics;
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
                    if (x + Offset == 53 && z + Offset == 59)
                    // if (true)
                    {
                        Console.WriteLine($"ABOUYT TO START GENERATION FOR {x + Offset} {z + Offset}");
                        SingleChunkFiles(new ChunkCoordinates(x + Offset, Offset + z), name);
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

        public void SingleChunkFiles(ChunkCoordinates c, String name = null)
        {
            var b = BiomeManager.GetBiome(c);
            var f1 = b.GenerateChunkHeightMap(c);
            var f2 = b.GenerateExtendedChunkHeightMap(f1, c, C);
            var f22 = b.CropToSmoothChunks(f2, c, C); 
            // var f3 = b.SmoothMapV2(f22);
            // var f4 = b.SmoothMapV3(f3);
            var f33 = b.LerpX(f22);
            var f44 = b.LerpZ(f33);
            var f55 = b.FinalCropTo16(f44);
            if (name != null && name.Length != 0)
            {
                name += "/";
            }
            else
            {
                name = "";
            }

            List<int[,]> l = new List<int[,]>();
            l.Add(f1);
            l.Add(f2);
            l.Add(f22);
            l.Add(f33);
            l.Add(f44);
            l.Add(f55);
            Console.WriteLine("ABOUT TO SAVE " + c);
            SaveViaCSV($"/MapTesting/{name}chunk{c.X} {c.Z}-F1.csv", IntArrayToString(JoinIntMaps(l)));
            // SaveViaCSV($"/MapTesting/{name}chunk{c.X} {c.Z}-F2.csv", IntArrayToString(f2));
            // SaveViaCSV($"/MapTesting/{name}chunk{c.X} {c.Z}-F3.csv", IntArrayToString(f3));
            // SaveViaCSV($"/MapTesting/{name}chunk{c.X} {c.Z}-F4.csv", IntArrayToString(f4));
        }

        public int[,] JoinIntMaps(List<int[,]> l)
        {
            int[,] r = new int[0, 0];
            if (l.Count == 0) return r;
            int s = (l.Count * 18) + (l.Count - 1);
            r = new int[18, s];
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

        public string IntArrayToString(int[,] d)
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
                    s += d[x, z] + ",";
                }

                sss.Stop();
                // Console.WriteLine($"TOOK {sss.Elapsed} TO CONVER COL {z}/{d.GetLength(1)}");

                s += "\n";
            }

            ss.Stop();
            // Console.WriteLine("TIME THAT IT TOOK TO CONVERT TO STRING " + ss.Elapsed);
            return s;
        }

        public void SaveViaCSV(string datCsv, string text)
        {
            var s = new Stopwatch();
            s.Restart();
            System.IO.File.WriteAllText(@datCsv, text);
            s.Stop();
            Console.WriteLine("FILE WRITE TIME WAS " + s.Elapsed);
        }
    }
}