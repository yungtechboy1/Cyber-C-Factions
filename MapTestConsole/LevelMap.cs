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
                    var a = C.OpenGenerateChunkColumn(new ChunkCoordinates(x+Offset, Offset+z));
                    MapData[x, z] = a.Result;
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
            for (int z = 0; z < d.GetLength(1); z++)
            {
                sss.Restart();
                for (int x = 0; x < d.GetLength(0); x++)
                {
                    s += d[x, z] + ",";
                }
                sss.Stop();
                Console.WriteLine($"TOOK {sss.Elapsed} TO CONVER COL {z}/{d.GetLength(1)}");

                s += "\n";
            }
            ss.Stop();
Console.WriteLine("TIME THAT IT TOOK TO CONVERT TO STRING "+ss.Elapsed);
            return s;
        }

        public void SaveViaCSV(string datCsv, string text)
        {
            var s = new Stopwatch();
            s.Restart();
            System.IO.File.WriteAllText(@datCsv, text);
            s.Stop();
            Console.WriteLine("FILE WRITE TIME WAS "+s.Elapsed);
        }
    }
}