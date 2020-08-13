using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using CyberCore.Manager.FloatingText;
using CyberCore.Utils;
using log4net;
using log4net.Util.TypeConverters;
using MiNET.Blocks;
using MiNET.Utils;
using MiNET.Worlds;
using OpenAPI.World;

namespace CyberCore.WorldGen.Biomes
{
    public class BiomeQualifications
    {
        public int baseheight = 80;

        public int heightvariation;
        public float startheight; //0-2
        public float startrain; //0 - 1
        public float starttemp; //0 - 2
        public float stopheight;
        public float stoprain;
        public float stoptemp;

        // public int baseheight = 20;
        public bool waterbiome;


        public BiomeQualifications(float startrain, float stoprain, float starttemp, float stoptemp, float startheight,
            float stopheight, int heightvariation, bool waterbiome = false)
        {
            this.startrain = startrain;
            this.stoprain = stoprain;
            this.starttemp = starttemp;
            this.stoptemp = stoptemp;
            this.startheight = startheight;
            this.stopheight = stopheight;
            this.waterbiome = waterbiome;
            this.heightvariation = heightvariation;
        }


        public bool check(float[] rth)
        {
            var rain = rth[0];
            var temp = rth[1];
            var height = rth[2];
            return startrain <= rain && stoprain >= rain && starttemp <= temp && stoptemp >= temp &&
                   startheight <= height && stopheight >= height;
        }

        public bool check(float rain, float temp, float height)
        {
            return startrain <= rain && stoprain >= rain && starttemp <= temp && stoptemp >= temp &&
                   startheight <= height && stopheight >= height;
        }
    }

    public abstract class AdvancedBiome : ICloneable
    {
        public int waterlevel = 75;
        private static readonly ILog Log = LogManager.GetLogger(typeof(AdvancedBiome));

        private static readonly OpenSimplexNoise OpenNoise = new OpenSimplexNoise("a-seed".GetHashCode());
        public BiomeQualifications BiomeQualifications;

        /// <summary>
        /// </summary>
        public AdvancedBiome BorderBiome;

        /// <summary>
        /// </summary>
        public enum BorderChunkDirection
        {
            None,
            North,
            South,
            East,
            West,
            NW,
            SW,
            NE,
            SE
        }

        public List<BorderChunkDirection> BorderChunkDirections = new List<BorderChunkDirection>();

        public bool BorderChunk = false;
        // public List<ChunkCoordinates> GenerateandSmooth = new List<ChunkCoordinates>();

        public FastNoise HeightNoise = new FastNoise(121212);

        public int LocalID = -1;
        public string name;
        public Random RNDM = new Random();
        public int startheight = 80;

        public AdvancedBiome(string name, BiomeQualifications bq)
        {
            BiomeQualifications = bq;
            HeightNoise.SetGradientPerturbAmp(3);
            HeightNoise.SetFrequency(.24f);
            HeightNoise.SetNoiseType(FastNoise.NoiseType.CubicFractal);
            HeightNoise.SetFractalOctaves(2);
            HeightNoise.SetFractalLacunarity(.35f);
            HeightNoise.SetFractalGain(1);
            this.name = name;
        }

        public int BorderType { get; set; } = 0;

        public abstract int GetSH(int x, int z, int cx, int cz);

        public bool check(float[] rth)
        {
            return BiomeQualifications.check(rth);
        }

        public virtual int[,] GenerateChunkHeightMap(ChunkColumn c, CyberExperimentalWorldProvider cc)
        {
            return GenerateChunkHeightMap(new ChunkCoordinates(c.X, c.Z), cc);
        }

        public enum ChunkCorner
        {
            NA,
            NorthWest,
            NorthEast,
            SouthWest,
            SouthEast
        }

        public enum ChunkSide
        {
            NA,

            // NorthWest,
            North,

            // NorthEast,
            West,
            East,

            // SouthWest,
            South,
            // SouthEast
        }


        public virtual int CornerGenerateChunkHeightMap(ChunkCorner side, ChunkCoordinates c,
            CyberExperimentalWorldProvider cyberExperimentalWorldProvider)
        {
            // Console.WriteLine("AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA");
            int r = -1;
            var cc = cyberExperimentalWorldProvider.GenerateChunkColumn(c, true);
            if (cc != null)
            {
                // Console.WriteLine($"CHUNK AT {c} WAS GENERATED");
                switch (side)
                {
                    case ChunkCorner.NorthWest:
                        r = cc.GetHeight(0, 15);
                        break;
                    case ChunkCorner.NorthEast:
                        r = cc.GetHeight(15, 15);
                        break;
                    case ChunkCorner.SouthWest:
                        r = cc.GetHeight(0, 0);
                        break;
                    case ChunkCorner.SouthEast:
                        r = cc.GetHeight(15, 0);
                        break;
                }

                return r;
            }

            switch (side)
            {
                case ChunkCorner.NorthWest:
                    r = GetSH(0, 15, c.X, c.Z);
                    break;
                case ChunkCorner.NorthEast:
                    r = GetSH(15, 15, c.X, c.Z);
                    break;
                case ChunkCorner.SouthWest:
                    r = GetSH(0, 0, c.X, c.Z);
                    break;
                case ChunkCorner.SouthEast:
                    r = GetSH(15, 0, c.X, c.Z);
                    break;
            }

            return r;
        }

        /// <summary>
        /// Relative to THIS CHUNK GET A int[] of Side of Chunk
        /// </summary>
        /// <param name="side"></param>
        /// <param name="c"></param>
        /// <param name="cyberExperimentalWorldProvider"></param>
        /// <returns></returns>
        public virtual int[] SideGenerateChunkHeightMap(ChunkSide side, ChunkCoordinates c,
            CyberExperimentalWorldProvider cyberExperimentalWorldProvider)
        {
            int[] r = new int[16];
            var cc = cyberExperimentalWorldProvider.GenerateChunkColumn(c, true);
            if (cc != null)
            {
                // Console.WriteLine($"CHUNK AT {c} WAS GENERATED");
                switch (side)
                {
                    case ChunkSide.North:
                        for (int x = 0; x < 16; x++)
                        {
                            r[x] = cc.GetHeight(x, 15);
                        }

                        break;
                    case ChunkSide.East:
                        for (int x = 0; x < 16; x++)
                        {
                            r[x] = cc.GetHeight(15, x);
                        }

                        break;
                    case ChunkSide.South:
                        for (int x = 0; x < 16; x++)
                        {
                            r[x] = cc.GetHeight(x, 0);
                        }

                        break;
                    case ChunkSide.West:

                        for (int x = 0; x < 16; x++)
                        {
                            r[x] = cc.GetHeight(0, x);
                        }

                        break;
                }

                return r;
            }


            switch (side)
            {
                case ChunkSide.North:
                    for (int x = 0; x < 16; x++)
                    {
                        r[x] = GetSH(x, 15, c.X, c.Z);
                    }

                    break;
                case ChunkSide.East:
                    for (int z = 0; z < 16; z++)
                    {
                        r[z] = GetSH(15, z, c.X, c.Z);
                    }

                    break;
                case ChunkSide.South:
                    for (int x = 0; x < 16; x++)
                    {
                        r[x] = GetSH(x, 0, c.X, c.Z);
                    }

                    break;
                case ChunkSide.West:
                    for (int z = 0; z < 16; z++)
                    {
                        r[z] = GetSH(0, z, c.X, c.Z);
                    }

                    break;
            }

            return r;
        }

        public virtual int[,] GenerateChunkHeightMap(ChunkCoordinates c,
            CyberExperimentalWorldProvider cyberExperimentalWorldProvider)
        {
            // var cc = cyberExperimentalWorldProvider.GenerateChunkColumn(c, true);
            var r = new int[16, 16];

            for (int z = 0; z < 16; z++)
            for (int x = 0; x < 16; x++)
            {
                // Console.WriteLine($"{c.X} *16 + {x} = {c.X * 16 + x} || {c.Z}*16 + {z} = {c.Z * 16 + z}");
                r[x, z] = cyberExperimentalWorldProvider.getBlockHeight(c.X * 16 + x, c.Z * 16 + z);
            }


            return r;
        }

        // public async Task<ChunkColumn> preSmooth(CyberExperimentalWorldProvider CyberExperimentalWorldProvider,
        //     ChunkColumn chunk,
        //     float[] rth)
        // {
        //     var t = new Stopwatch();
        //     t.Start();
        //     SmoothChunk(CyberExperimentalWorldProvider, chunk, rth);
        //     t.Stop();
        //
        //     if (t.ElapsedMilliseconds > 100) Log.Info($"CHUNK SMOOTHING OF {chunk.X} {chunk.Z} TOOK {t.Elapsed}");
        //     return chunk;
        // }

        public virtual int[,] GenerateExtendedChunkHeightMap(int[,] ints, ChunkColumn chunk,
            CyberExperimentalWorldProvider c)
        {
            return GenerateExtendedChunkHeightMap(ints, new ChunkCoordinates(chunk.X, chunk.Z), c);
        }

        public class ListMap2d
        {
            List<List<int>> L = new List<List<int>>();

            public ListMap2d()
            {
            }

            public void set(int x, int z, int v)
            {
                var a = L[x];
                if (a == null)
                {
                    Console.WriteLine("AAAAAAAAAAAAAAAASSSSSSSSSSSSSSSSSSSSDDDDDDDDDDDDD");
                    L[x] = new List<int>();
                }

                a[z] = v;
            }

            public int get(int x, int z)
            {
                return L[x][z];
            }
        }

        public virtual int[,] GenerateExtendedChunkHeightMap(int[,] ints, ChunkCoordinates chunk,
            CyberExperimentalWorldProvider c)
        {
            // List<List<int>> l = new List<List<int>>();
            var l = new ListMap2d();


            int[,] r = new int[18, 18];
            var cw = BiomeManager.GetBiome2(new ChunkCoordinates(chunk.X - 1, chunk.Z));
            var ce = BiomeManager.GetBiome2(new ChunkCoordinates(chunk.X + 1, chunk.Z));
            var cs = BiomeManager.GetBiome2(new ChunkCoordinates(chunk.X, chunk.Z - 1));
            var cn = BiomeManager.GetBiome2(new ChunkCoordinates(chunk.X, chunk.Z + 1));
            var ccse = BiomeManager.GetBiome2(new ChunkCoordinates(chunk.X + 1, chunk.Z - 1));
            var ccne = BiomeManager.GetBiome2(new ChunkCoordinates(chunk.X + 1, chunk.Z + 1));
            var ccsw = BiomeManager.GetBiome2(new ChunkCoordinates(chunk.X - 1, chunk.Z - 1));
            var ccnw = BiomeManager.GetBiome2(new ChunkCoordinates(chunk.X - 1, chunk.Z + 1));
            // int[] west =
            //     SideGenerateChunkHeightMap(ChunkSide.West.Opposite(), new ChunkCoordinates(chunk.X - 1, chunk.Z));
            int[] west =
                cw.SideGenerateChunkHeightMap(ChunkSide.West.Opposite(), new ChunkCoordinates(chunk.X - 1, chunk.Z), c);
            int[] north =
                cn.SideGenerateChunkHeightMap(ChunkSide.North.Opposite(), new ChunkCoordinates(chunk.X, chunk.Z + 1),
                    c);
            int[] east =
                ce.SideGenerateChunkHeightMap(ChunkSide.East.Opposite(), new ChunkCoordinates(chunk.X + 1, chunk.Z), c);
            int[] south =
                cs.SideGenerateChunkHeightMap(ChunkSide.South.Opposite(), new ChunkCoordinates(chunk.X, chunk.Z - 1),
                    c);

            // int sx = 16;
            // int sz = 16;
            // if (BorderChunkDirections.Contains(BorderChunkDirection.North))
            //     sx += 1;
            // if(BorderChunkDirections.Contains(BorderChunkDirection.South))
            //     sx += 1;
            // if (BorderChunkDirections.Contains(BorderChunkDirection.East))
            //     sz += 1;
            // if(BorderChunkDirections.Contains(BorderChunkDirection.West))
            //     sz += 1;
            // r = new int[sx,sz];
            // if (BorderChunkDirections.Contains(BorderChunkDirection.SW))
            // {
            //     r[0,0] = ccsw.CornerGenerateChunkHeightMap(ChunkCorner.SouthWest.Opposite(),
            //         new ChunkCoordinates(chunk.X - 1, chunk.Z - 1), c);
            // }
            // if (BorderChunkDirections.Contains(BorderChunkDirection.South))
            // {
            //     for (int x = 0; x < 16; x++)
            //     {
            //         r[x, 0] = south[x]; 
            //     }
            // }
            // if (BorderChunkDirections.Contains(BorderChunkDirection.SW))
            // {
            //     r[0,0] = ccsw.CornerGenerateChunkHeightMap(ChunkCorner.SouthWest.Opposite(),
            //         new ChunkCoordinates(chunk.X - 1, chunk.Z - 1), c);
            // }
            // if (BorderChunkDirections.Contains(BorderChunkDirection.SW))
            // {
            //     r[0,0] = ccsw.CornerGenerateChunkHeightMap(ChunkCorner.SouthWest.Opposite(),
            //         new ChunkCoordinates(chunk.X - 1, chunk.Z - 1), c);
            // }
            //
            //

            Console.WriteLine("THE BORDERCHUNKDIRECTIONS ARE!!!!!!!!!!!!!!\n" + BorderChunkDirections.Count);
            foreach (var b in BorderChunkDirections)
            {
                Console.WriteLine("===>>>>" + b);
            }

            for (int x = 0; x <= 17; x++)
            for (int z = 0; z <= 17; z++)
            {
                if (x == 0)
                {
                    if (z == 0)
                    {
                        //SouthWest
                        // if (BorderChunkDirections.Contains(BorderChunkDirection.SW))
                        r[x, z] = ccsw.CornerGenerateChunkHeightMap(ChunkCorner.SouthWest.Opposite(),
                            new ChunkCoordinates(chunk.X - 1, chunk.Z - 1), c);
                        // else
                        //     r[x, z] = -1;
                    }
                    else if (z == 17)
                    {
                        //NorthWest

                        // if (BorderChunkDirections.Contains(BorderChunkDirection.NW))
                        r[x, z] = ccnw.CornerGenerateChunkHeightMap(ChunkCorner.NorthWest.Opposite(),
                            new ChunkCoordinates(chunk.X + 1, chunk.Z - 1), c);
                        // else
                        //     r[x, z] = -1;
                    }
                    else
                    {
                        // if (BorderChunkDirections.Contains(BorderChunkDirection.West))
                        r[x, z] = west[z - 1];
                        // else
                        //     r[x, z] = -1;
                        // r[x, z] = 80;
                    }
                }
                //LEFT SIDE FILLED/\/\
                else if (x == 17)
                {
                    //EAST North South
                    if (z == 0)
                    {
                        //SouthEast
                        // if (BorderChunkDirections.Contains(BorderChunkDirection.SE))
                        r[x, z] = ccse.CornerGenerateChunkHeightMap(ChunkCorner.SouthEast.Opposite(),
                            new ChunkCoordinates(chunk.X - 1, chunk.Z + 1), c);
                        // else
                        //     r[x, z] = -1;
                    }
                    else if (z == 17)
                    {
                        //NorthEast

                        // if (BorderChunkDirections.Contains(BorderChunkDirection.NE))
                        r[x, z] = ccne.CornerGenerateChunkHeightMap(ChunkCorner.NorthEast.Opposite(),
                            new ChunkCoordinates(chunk.X + 1, chunk.Z + 1), c);
                        // else
                        //     r[x, z] = -1;
                    }
                    else
                    {
                        // if (BorderChunkDirections.Contains(BorderChunkDirection.East))
                        r[x, z] = east[z - 1];
                        // else
                        //     r[x, z] = -1;
                    }
                }
                //RIGHT SIDE FILLED /\/\
                //CORNERS FILLED/\/\
                else
                {
                    //X,Z -> X,Z
                    //1,0 -> 16,16
                    /*
                     * 
                     */
                    if (z == 0)
                    {
                        //South
                        // if (BorderChunkDirections.Contains(BorderChunkDirection.South))
                        r[x, z] = south[x - 1];
                        // else
                        //     r[x, z] = -1;
                    }
                    else if (z == 17)
                    {
                        //East
                        // if (BorderChunkDirections.Contains(BorderChunkDirection.North))
                        r[x, z] = north[x - 1];
                        // else
                        //     r[x, z] = -1;
                    }
                    else
                    {
                        r[x, z] = ints[x - 1, z - 1];
                    }
                }
            }

            return r;
        }

        public async Task<ChunkColumn> prePopulate(CyberExperimentalWorldProvider CyberExperimentalWorldProvider,
            ChunkColumn chunk,
            float[] rth)
        {
            var t = new Stopwatch();
            t.Start();
            // OpenServer.FastThreadPool.QueueUserWorkItem(() => { PopulateChunk(openExperimentalWorldProvider,chunk, rth); });
            //CHECK IF BORDER CHUNK AND CHANGE SETTINGS
            // if (BorderChunk)
            // {
            //     var bro = BorderBiome;
            //     // Console.WriteLine($"OLD VALUE WAS {bro.BiomeQualifications.heightvariation} New Val is \\/\\/\\/\\/ || "+BiomeQualifications.heightvariation);
            //     var h = (bro.BiomeQualifications.heightvariation + BiomeQualifications.heightvariation) / 2;
            //     // Console.WriteLine("+++++++++++++++++++++++++++++++++++++ "+h);
            //     BiomeQualifications.heightvariation = h;
            //     // Console.WriteLine($"THE CHUNK AT {chunk.X} {chunk.Z} IS A BORDER CHUNK WITH VAL {bro} |||");
            // }
            // if (GenerateandSmooth.Count == 0)
            // {
            var a = GenerateUseabelHeightMap(CyberExperimentalWorldProvider, chunk, true);
            PopulateChunk(CyberExperimentalWorldProvider, chunk, rth, a);
            PostPopulate(CyberExperimentalWorldProvider, chunk, rth, a);
            // }
            // else
            // {
            //     GenerateWBorderChunks(chunk);
            // }

            t.Stop();
            // int minWorker, minIOC,maxworker,maxIOC;
            // ThreadPool.GetMinThreads(out minWorker, out minIOC);
            // ThreadPool.GetMaxThreads(out maxworker, out maxIOC);
            // if(minWorker != 20  && !ThreadPool.SetMinThreads(20,20))Console.WriteLine("EEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEE");

            // SmoothChunk(openExperimentalWorldProvider,chunk,rth);

            if (t.ElapsedMilliseconds > 100)
            {
                Log.Debug($"Chunk Population of X:{chunk.X} Z:{chunk.Z} took {t.Elapsed}");
            }

            return chunk;
        }

        // public void GenerateWBorderChunks(ChunkColumn chunk)
        // {
        //     bool n = false;
        //     bool e = false;
        //     bool s = false;
        //     bool w = false;
        //     bool nw = false;
        //     bool ne = false;
        //     bool sw = false;
        //     bool se = false;
        //     foreach (var gsc in GenerateandSmooth)
        //     {
        //         int dx = gsc.X - chunk.X;
        //         int dz = gsc.Z - chunk.Z;
        //         if (dx == 1)
        //         {
        //             n = true;
        //         }
        //         else if (dx == -1)
        //         {
        //             s = true;
        //         }
        //
        //         if (dz == 1)
        //         {
        //             e = true;
        //         }
        //         else if (dz == -1)
        //         {
        //             w = true;
        //         }
        //
        //         if (dx == 1 && dz == 1)
        //         {
        //             ne = true;
        //         }
        //
        //         if (dx == 1 && dz == -1)
        //         {
        //             se = true;
        //         }
        //
        //         if (dx == -1 && dz == 1)
        //         {
        //             nw = true;
        //         }
        //
        //         if (dx == -1 && dz == -1)
        //         {
        //             sw = true;
        //         }
        //     }
        //
        //     //Calculate Size of Chunks needed to generated and Smoothed
        //     int sz = 0;
        //     if (n) sz++;
        //     if (e) sz++;
        //     if (s) sz++;
        //     if (w) sz++;
        //     if (nw) sz++;
        //     if (sw) sz++;
        //     if (se) sz++;
        //     if (ne) sz++;
        //
        //     int top = 0;
        //     int left = 0;
        //     int right = 0;
        //     int bottom = 0;
        //
        //     if (n) top += 16;
        //     if (w) left += 16;
        //     if (e) right += 16;
        //     if (s) bottom += 16;
        //
        //     int xs = 16 + top + bottom;
        //     int zs = 16 + left + right;
        //
        //     int[,] map = new int[xs, zs];
        // }

        /// <summary>
        ///     Populate Chunk from Biome
        /// </summary>
        /// <param name="CyberExperimentalWorldProvider"></param>
        /// <param name="c"></param>
        /// <param name="rth"></param>
        /// <param name="ints"></param>
        protected /*Task*/ void PopulateChunk(CyberExperimentalWorldProvider CyberExperimentalWorldProvider,
            ChunkColumn c,
            float[] rth, int[,] ints)
        {
            for (int x = 0; x < 16; x++)
            {
                for (int z = 0; z < 16; z++)
                {
                    c.SetBlock(x, 0, z, new Bedrock());
                    for (int y = 1; y < 255; y++)
                    {
                        GenerateVerticalColumn(y, ints[x, z], x, z, c);
                    }

                    c.SetHeight(x, z, (short) ints[x, z]);
                }
            }
        }

        public static string IntArrayToString(int[,] d)
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

        public static void SaveViaCSV(string datCsv, string text)
        {
            var s = new Stopwatch();
            s.Restart();
            System.IO.File.WriteAllText(@datCsv, text);
            s.Stop();
            Console.WriteLine("FILE WRITE TIME WAS " + s.Elapsed);
        }

        public virtual void PostPopulate(CyberExperimentalWorldProvider cyber, ChunkColumn c, float[] rth, int[,] ints)
        {
        }

        public virtual int[,] GenerateUseabelHeightMap(CyberExperimentalWorldProvider CyberExperimentalWorldProvider,
            ChunkColumn c, bool smooth = true)
        {
            var m = GenerateChunkHeightMap(c, CyberExperimentalWorldProvider);
            // if (smooth && BorderChunk)
            // {BIOMEMANAGER:
            //     m = SmoothMapV3(m);
            //     m = SmoothMapV4(m);
            // }
            // if (BorderChunk)
            // SaveViaCSV($"/MapTesting/MAPCHUNK ORIGINAL {c.X} {c.Z}.csv", IntArrayToString(m));

            if (BorderChunkDirections.Count > 0)
            {
                Console.WriteLine(
                    $"ABOUT TO GENERATEUSEABLEHEIGHTMAP {c.X} {c.Z} =>> HAS BCD::{BorderChunkDirections.Count}");
                foreach (var bb in BorderChunkDirections)
                {
                    Console.WriteLine("????????????????????????" + bb);
                }

                if (true)
                {
                    ChunkCoordinates sischunkcords;
                    AdvancedBiome sischunkbiome;


                    // HandleSmoothing(m,new ChunkCoordinates(c.X,c.Z),)
                    if ( /*c.X == 55 &&*/ c.Z == 57)
                    {
                        if (true)
                        {
                            SmoothingMap sm = HandleGeneration(m, new ChunkCoordinates(c.X, c.Z),
                                BorderChunkDirections,
                                CyberExperimentalWorldProvider);
                            SaveViaCSV($"/MapTesting/MAPCHUNK NNNNN PRE SMOOTH EXPAND {c.X} {c.Z}.csv",IntArrayToString(sm.Map));
                            // sm.AddBorderValues(CyberExperimentalWorldProvider);
                            SaveViaCSV($"/MapTesting/MAPCHUNK NNNNN POST BORDER EXPAND {c.X} {c.Z}.csv",IntArrayToString(sm.Map));
                            // sm.SquareSmooth(3);
                            sm.StripSmooth(4);
                            SaveViaCSV($"/MapTesting/MAPCHUNK NNNNN POST SMOOTH EXPAND {c.X} {c.Z}.csv",IntArrayToString(sm.Map));
                            // sm.StripSmooth(3);
                            // SaveViaCSV($"/MapTesting/MAPCHUNK NNNNN POST SMOOTH EXPAND2 {c.X} {c.Z}.csv",IntArrayToString(sm.Map));
                            m = sm.SetChunks(CyberExperimentalWorldProvider, BCD);
                            m[7, 7] = 100;
                        }
                    }
                    // // else
                    // if (BorderChunkDirections.Contains(BorderChunkDirection.North))
                    // {
                    //     //North Sis Chunk
                    //     sischunkcords = new ChunkCoordinates(c.X, c.Z + 1);
                    //     sischunkbiome = BiomeManager.GetBiome(sischunkcords);
                    //     if (sischunkbiome.BorderChunkDirections.Contains(BorderChunkDirection.South))
                    //     {
                    //         //Generate A 16 X 16*2 Chunk map and Populate Sister Chunk
                    //         m = GenerateExtendedChunkHeightMap(BorderChunkDirection.South, m,
                    //             sischunkbiome.GenerateChunkHeightMap(sischunkcords, CyberExperimentalWorldProvider),
                    //             CyberExperimentalWorldProvider);
                    //         // SaveViaCSV($"/MapTesting/MAPCHUNK PRE SMOOTH PRE EXPAND {c.X} {c.Z}.csv", IntArrayToString(m));
                    //         m = GenerateExtendedChunkHeightMapBorder(m, BorderChunkDirection.North, c,
                    //             CyberExperimentalWorldProvider);
                    //         SaveViaCSV($"/MapTesting/MAPCHUNK NNNNN PRE SMOOTH EXPAND {c.X} {c.Z}.csv",
                    //             IntArrayToString(m));
                    //         var mmm = SquareSmooth(m, 3);
                    //         // SaveViaCSV($"/MapTesting/MAPCHUNK NNNNN222 POST SMOOTH {c.X} {c.Z}.csv",
                    //         //     IntArrayToString(mmm));
                    //         SaveViaCSV($"/MapTesting/MAPCHUNK NNNNN POST SMOOTH {c.X} {c.Z}.csv",
                    //             IntArrayToString(mmm));
                    //         mmm = ShrinkFromExpand(mmm);
                    //         SaveViaCSV($"/MapTesting/MAPCHUNK NNNNN POST SMOOTH SHRINK {c.X} {c.Z}.csv",
                    //             IntArrayToString(mmm));
                    //         // Console.WriteLine($"AHHHH PLZ WORK {mmm.GetLength(0)} {mmm.GetLength(1)}");
                    //         m = FinalCropTo16(mmm, BorderChunkDirection.North);
                    //         m[10, 10] = 110;
                    //         SaveViaCSV($"/MapTesting/MAPCHUNK NNNNN {c.X} {c.Z}.csv", IntArrayToString(m));
                    //         // Console.WriteLine($"SMOOTHIGN SOUTH >> {c.X} {c.Z}");
                    //         //FINISH SISTER CHUNK
                    //         var mm = FinalCropTo16(mmm, BorderChunkDirection.South);
                    //         mm[10, 10] = 115;
                    //         var nc = new ChunkColumn();
                    //         nc.X = sischunkcords.X;
                    //         nc.Z = sischunkcords.Z;
                    //         sischunkbiome.GenerateChunkFromSmoothOrder(CyberExperimentalWorldProvider, nc,
                    //             BiomeManager.getChunkRTH(sischunkcords), mm);
                    //         SaveViaCSV($"/MapTesting/MAPCHUNK NNNNN {c.X} {c.Z} GENERATED {nc.X} {nc.Z}.csv",
                    //             IntArrayToString(mm));
                    //         CyberExperimentalWorldProvider._chunkCache[sischunkcords] = nc;
                    //         SaveViaCSV($"/MapTesting/MAPCHUNK NNNNN {c.X} {c.Z} GENERATED FOUND {nc.X} {nc.Z}.csv",
                    //             IntArrayToString(ChunkToIntMap(
                    //                 CyberExperimentalWorldProvider.GenerateChunkColumn(sischunkcords, true))));
                    //         Console.WriteLine($"#{c.X}:{c.Z} SMOOTH North TO {sischunkcords}");
                    //     }
                    // }
                    //
                    // // else
                    // if (BorderChunkDirections.Contains(BorderChunkDirection.East))
                    // {
                    //     //East Sis Chunk
                    //     sischunkcords = new ChunkCoordinates(c.X + 1, c.Z);
                    //     sischunkbiome = BiomeManager.GetBiome(sischunkcords);
                    //     if (sischunkbiome.BorderChunkDirections.Contains(BorderChunkDirection.West))
                    //     {
                    //         //Generate A 16 X 16*2 Chunk map and Populate Sister Chunk
                    //         m = GenerateExtendedChunkHeightMap(BorderChunkDirection.West, m,
                    //             sischunkbiome.GenerateChunkHeightMap(sischunkcords, CyberExperimentalWorldProvider),
                    //             CyberExperimentalWorldProvider);
                    //         // SaveViaCSV($"/MapTesting/MAPCHUNK PRE SMOOTH PRE EXPAND {c.X} {c.Z}.csv", IntArrayToString(m));
                    //         m = GenerateExtendedChunkHeightMapBorder(m, BorderChunkDirection.East, c,
                    //             CyberExperimentalWorldProvider, 3);
                    //         SaveViaCSV($"/MapTesting/MAPCHUNK EEEEEEE PRE SMOOTH EXPAND {c.X} {c.Z}.csv",
                    //             IntArrayToString(m));
                    //         var mmm = SquareSmooth(m);
                    //         SaveViaCSV($"/MapTesting/MAPCHUNK EEEEEEE POST SMOOTH {c.X} {c.Z}.csv",
                    //             IntArrayToString(mmm));
                    //         mmm = ShrinkFromExpand(mmm, 3);
                    //         SaveViaCSV($"/MapTesting/MAPCHUNK EEEEEEE POST SMOOTH SHRINK {c.X} {c.Z}.csv",
                    //             IntArrayToString(mmm));
                    //         // Console.WriteLine($"AHHHH PLZ WORK {mmm.GetLength(0)} {mmm.GetLength(1)}");
                    //         m = FinalCropTo16(mmm, BorderChunkDirection.East);
                    //         m[10, 10] = 110;
                    //         SaveViaCSV($"/MapTesting/MAPCHUNK EEEEEEE {c.X} {c.Z}.csv", IntArrayToString(m));
                    //         // Console.WriteLine($"SMOOTHIGN SOUTH >> {c.X} {c.Z}");
                    //         //FINISH SISTER CHUNK
                    //         var mm = FinalCropTo16(mmm, BorderChunkDirection.West);
                    //         mm[10, 10] = 115;
                    //         var nc = new ChunkColumn();
                    //         nc.X = sischunkcords.X;
                    //         nc.Z = sischunkcords.Z;
                    //         sischunkbiome.GenerateChunkFromSmoothOrder(CyberExperimentalWorldProvider, nc,
                    //             BiomeManager.getChunkRTH(sischunkcords), mm);
                    //         SaveViaCSV($"/MapTesting/MAPCHUNK EEEEEEE {c.X} {c.Z} GENERATED {nc.X} {nc.Z}.csv",
                    //             IntArrayToString(mm));
                    //         CyberExperimentalWorldProvider._chunkCache[sischunkcords] = nc;
                    //         Console.WriteLine($"#{c.X}:{c.Z} SMOOTH EAST TO {sischunkcords}");
                    //     }
                    // }

                    // }

                    // if (BCD.Contains(BorderChunkDirection.North)) m[7, 8] = 101;
                    // if (BCD.Contains(BorderChunkDirection.East)) m[8, 7] = 101;
                    // if (BCD.Contains(BorderChunkDirection.South)) m[7, 6] = 101;
                    // if (BCD.Contains(BorderChunkDirection.West)) m[6, 7] = 101;
                    // if (BCD.Contains(BorderChunkDirection.NE)) m[8, 8] = 101;
                    // if (BCD.Contains(BorderChunkDirection.SE)) m[8, 6] = 101;
                    // if (BCD.Contains(BorderChunkDirection.SW)) m[6, 6] = 101;
                    // if (BCD.Contains(BorderChunkDirection.NW)) m[6, 8] = 101;
                }
            }


            for (int x = 0; x < 16; x++)
            {
                for (int z = 0; z < 16; z++)
                {
                    // for (int y = 0; y < 255; y++)
                    // {
                    //     
                    // }
                    c.SetHeight(x, z, (short) m[x, z]);
                }
            }

            return m;
        }

        private int[,] ChunkToIntMap(ChunkColumn tc)
        {
            int[,] r = new int[16, 16];
            for (int z = 0; z < 16; z++)
            for (int x = 0; x < 16; x++)
            {
                r[x, z] = tc.GetHeight(x, z);
            }

            return r;
        }

        public List<BorderChunkDirection> BCD = null;
        // private CyberExperimentalWorldProvider ccc;

        private SmoothingMap HandleGeneration(int[,] ints, ChunkCoordinates c,
            List<BorderChunkDirection> borderChunkDirections,
            CyberExperimentalWorldProvider cyberExperimentalWorldProvider)
        {
            // ccc = cyberExperimentalWorldProvider;
            ChunkCoordinates sischunkcords;
            AdvancedBiome sischunkbiome;
            SmoothingMap sm = new SmoothingMap(c, ints);
            Console.WriteLine($"YAAAAAA: >>>>");
            foreach (var VARIABLE in borderChunkDirections)
            {
                Console.WriteLine("TTTTTT: " + VARIABLE);
            }

            if (borderChunkDirections.Contains(BorderChunkDirection.NE))
            {
                if (!borderChunkDirections.Contains(BorderChunkDirection.North))borderChunkDirections.Add(BorderChunkDirection.North);
                if (!borderChunkDirections.Contains(BorderChunkDirection.East))borderChunkDirections.Add(BorderChunkDirection.East);
            }
            if (borderChunkDirections.Contains(BorderChunkDirection.SE))
            {
                if (!borderChunkDirections.Contains(BorderChunkDirection.South))borderChunkDirections.Add(BorderChunkDirection.South);
                if (!borderChunkDirections.Contains(BorderChunkDirection.East))borderChunkDirections.Add(BorderChunkDirection.East);
            }
            if (borderChunkDirections.Contains(BorderChunkDirection.NW))
            {
                if (!borderChunkDirections.Contains(BorderChunkDirection.North))borderChunkDirections.Add(BorderChunkDirection.North);
                if (!borderChunkDirections.Contains(BorderChunkDirection.East))borderChunkDirections.Add(BorderChunkDirection.East);
            }
            if (borderChunkDirections.Contains(BorderChunkDirection.SW))
            {
                if (!borderChunkDirections.Contains(BorderChunkDirection.South))borderChunkDirections.Add(BorderChunkDirection.South);
                if (!borderChunkDirections.Contains(BorderChunkDirection.West))borderChunkDirections.Add(BorderChunkDirection.West);
            }

            BorderChunkDirection d = BorderChunkDirection.North;
            CheckAndAddChunkDirection(d, borderChunkDirections, c, sm, cyberExperimentalWorldProvider);

            d = BorderChunkDirection.East;
            CheckAndAddChunkDirection(d, borderChunkDirections, c, sm, cyberExperimentalWorldProvider);

            d = BorderChunkDirection.South;
            CheckAndAddChunkDirection(d, borderChunkDirections, c, sm, cyberExperimentalWorldProvider);

            d = BorderChunkDirection.West;
            CheckAndAddChunkDirection(d, borderChunkDirections, c, sm, cyberExperimentalWorldProvider);

            d = BorderChunkDirection.NE;
            CheckAndAddChunkDirection(d, borderChunkDirections, c, sm, cyberExperimentalWorldProvider);
            d = BorderChunkDirection.SE;
            CheckAndAddChunkDirection(d, borderChunkDirections, c, sm, cyberExperimentalWorldProvider);
            d = BorderChunkDirection.SW;
            CheckAndAddChunkDirection(d, borderChunkDirections, c, sm, cyberExperimentalWorldProvider);
            d = BorderChunkDirection.NW;
            CheckAndAddChunkDirection(d, borderChunkDirections, c, sm, cyberExperimentalWorldProvider);
            BCD = borderChunkDirections;

            return sm;
        }

        private void CheckAndAddChunkDirection(BorderChunkDirection b,
            List<BorderChunkDirection> bcd, ChunkCoordinates cordz, SmoothingMap sm,
            CyberExperimentalWorldProvider cyberExperimentalWorldProvider)
        {
            if (bcd.Contains(b))
            {
                if (b == BorderChunkDirection.SE)
                {
                    Console.WriteLine(
                        $"SEEEE {b.GetX()} {b.GetZ()} || ZC:{sm.ZeroCords} || CC:{sm.getCenterCords()} || TC: {cordz.X + b.GetX()} {cordz.Z + b.GetZ()} ");
                }

                var sischunkcords = new ChunkCoordinates(cordz.X + b.GetX(), cordz.Z + b.GetZ());
                var sischunkbiome = BiomeManager.GetBiome(sischunkcords);
                // if (sischunkbiome.BorderChunkDirections.Contains(b.Opposite()))
                // {
                sm.AddChunk(cordz + new ChunkCoordinates(b.GetX(), b.GetZ()),
                    sischunkbiome.GenerateChunkHeightMap(sischunkcords, cyberExperimentalWorldProvider));
                // }
                // else
                //     bcd.Remove(b);
            }
        }

        private int[,] ShrinkFromExpand(int[,] mmm, int xtra = 1)
        {
            int[,] m = new int[mmm.GetLength(0) - 2 * xtra, mmm.GetLength(1) - 2 * xtra];
            for (int z = 0; z < mmm.GetLength(1) - 1; z++)
            for (int x = 0; x < mmm.GetLength(0) - 1; x++)
            {
                if (x <= xtra - 1 || z >= xtra - 1)
                {
                    continue;
                }
                else
                {
                    Console.WriteLine(
                        $"M {x - xtra} {z - xtra} |{xtra}||| mmm {x} {z} || {m.GetLength(0)} {m.GetLength(1)} || {mmm.GetLength(0)} {mmm.GetLength(1)}");
                    m[x - xtra, z - xtra] = mmm[x, z];
                }
            }

            return m;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ints"></param>
        /// <param name="direction">Addation chunkColumn is to THis Direction</param>
        /// <param name="chunkColumn"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        private int[,] GenerateExtendedChunkHeightMapBorder(int[,] ints, BorderChunkDirection direction,
            ChunkColumn chunkColumn, CyberExperimentalWorldProvider c, int xtra = 1)
        {
            var zaxis = false;
            if (direction == BorderChunkDirection.North || direction == BorderChunkDirection.South) zaxis = true;
            ChunkCoordinates cc = new ChunkCoordinates(chunkColumn.X, chunkColumn.Z);
            if (direction != BorderChunkDirection.North && direction != BorderChunkDirection.East)
            {
                if (direction == BorderChunkDirection.South) cc += new ChunkCoordinates(0, 1);
                if (direction == BorderChunkDirection.West) cc += new ChunkCoordinates(1, 0);
            }

            //Get Chunk in Top Left Corner or Closer to 0,0
            // int xtra = 3;
            int[,] m = zaxis
                ? new int[16 + 2 * xtra, (16 * 2) + 2 * xtra]
                : new int[(16 * 2) + 2 * xtra, 16 + 2 * xtra];
            int sx = cc.X * 16 - xtra;
            int sz = cc.Z * 16 - xtra;
            for (int z = 0; z < m.GetLength(1); z++)
            for (int x = 0; x < m.GetLength(0); x++)
            {
                if (x <= xtra - 1 || x >= m.GetLength(0) - xtra - 1 || z <= xtra - 1 || z >= m.GetLength(1) - xtra - 1)
                {
                    m[x, z] = c.getBlockHeight(sx + x, sz + z);
                }
                else
                {
                    Console.WriteLine(
                        $"M {x} {z} ||| ints {x - xtra} {z - xtra} || {m.GetLength(0)} {m.GetLength(1)} || {ints.GetLength(0)} {ints.GetLength(1)}");
                    m[x, z] = ints[x - xtra, z - xtra];
                }
            }

            return m;
        }

        /// <summary>
        /// 2x Square Smoothing donse not work as goon :(
        /// </summary>
        /// <param name="ints"></param>
        /// <returns></returns>
        private int[,] SquareSmooth(int[,] ints, int w = 2, bool cel = true)
        {
            for (int z = 0; z < ints.GetLength(1); z++)
            for (int x = 0; x < ints.GetLength(0); x++)
            {
                int ah = 0;
                int ac = 0;
                for (int zz = w * -1; zz <= w; zz++)
                for (int xx = w * -1; xx <= w; xx++)
                {
                    int tx = x + xx;
                    int tz = z + zz;
                    // if (0 > z + i || f22.GetLength(1) <= z + i) continue; 
                    if (0 > tx || 0 > tz || ints.GetLength(0) <= tx || ints.GetLength(1) <= tz) continue;
                    ac++;
                    ah += ints[tx, tz];
                }

                float alpha = .35f;
                if (cel)
                {
                    int vv = (int) Math.Ceiling(ah / (double) ac);
                    // int vvv = Interpolate(vv, ints[x, z], alpha);
                    int vvv = vv; //Interpolate(vv, ints[x, z], alpha);
                    // Console.WriteLine($"INTERPOLATION VALUE FROM {vv} TO {vvv} WITH #{ints[x, z]} AND A {alpha}");
                    ints[x, z] = vvv;
                }
                else
                    ints[x, z] = ah / ac;
            }

            return ints;
        }

        float Interpolate(float x0, float x1, float alpha)
        {
            return x0 * (1 - alpha) + alpha * x1;
        }

        int Interpolate(int x0, int x1, float alpha)
        {
            return (int) (x0 * (1 - alpha) + alpha * x1);
        }

        public void GenerateChunkFromSmoothOrder(CyberExperimentalWorldProvider cyberExperimentalWorldProvider,
            ChunkColumn nc, float[] rth, int[,] mm)
        {
            PopulateChunk(cyberExperimentalWorldProvider, nc, rth, mm);
            PostPopulate(cyberExperimentalWorldProvider, nc, rth, mm);
        }

        public int[,] LerpXZ2X(int[,] ints, bool ns = false)
        {
            // Console.WriteLine($"A0 0 : {ints[0, 0]}");
            // Console.WriteLine($"A0 16 : {ints[0, 16]}");
            var mm = LerpX(ints, !ns);
            // Console.WriteLine($"A0 0 : {m[0, 0]}");
            // Console.WriteLine($"A0 16 : {m[0, 16]}");
            mm = LerpZ(mm, ns);
            // Console.WriteLine($"A0 0 : {mm[0, 0]}");
            // Console.WriteLine($"A0 16 : {mm[0, 16]}");

            mm = LerpX(mm, !ns);
            // Console.WriteLine($"A0 0 : {m[0, 0]}");
            // Console.WriteLine($"A0 16 : {m[0, 16]}");

            mm = LerpZ(mm, ns);
            // Console.WriteLine($"A0 0 : {m[0, 0]}");
            // Console.WriteLine($"A0 16 : {m[0, 16]}");
            return mm;
        }

        public int[] GetChunkSideHeight(BorderChunkDirection direction, ChunkCoordinates cc,
            CyberExperimentalWorldProvider c)
        {
            ChunkCoordinates tcc;
            int[] d = new int[0];
            if (direction == BorderChunkDirection.North)
            {
                tcc = cc + new ChunkCoordinates(0, 1);
                var tb = BiomeManager.GetBiome(tcc);
                d = tb.SideGenerateChunkHeightMap(ChunkSide.North, tcc, c);
            }
            else if (direction == BorderChunkDirection.East)
            {
                tcc = cc + new ChunkCoordinates(1, 0);
                var tb = BiomeManager.GetBiome(tcc);
                d = tb.SideGenerateChunkHeightMap(ChunkSide.East, tcc, c);
            }
            else if (direction == BorderChunkDirection.South)
            {
                tcc = cc + new ChunkCoordinates(0, -1);
                var tb = BiomeManager.GetBiome(tcc);
                d = tb.SideGenerateChunkHeightMap(ChunkSide.South, tcc, c);
            }
            else if (direction == BorderChunkDirection.West)
            {
                tcc = cc + new ChunkCoordinates(-1, 0);
                var tb = BiomeManager.GetBiome(tcc);
                d = tb.SideGenerateChunkHeightMap(ChunkSide.West, tcc, c);
            }
            else
            {
                throw new Exception("ERror! GETCHUNKSIDEHEIGHT RETURNED NOT N E W S");
            }

            return d;
        }

        public int[,] GenerateExtendedChunkHeightMap(BorderChunkDirection direction, int[,] ints,
            int[,] sischunkbiome, CyberExperimentalWorldProvider c)
        {
            bool ns = false;
            int[,] r;
            if (direction == BorderChunkDirection.North || direction == BorderChunkDirection.South)
            {
                r = new int[16, (16 * 2)];
                ns = true;
                for (int z = 0; z < r.GetLength(1); z++)
                for (int x = 0; x < r.GetLength(0); x++)
                {
                    if (z < 16)
                    {
                        r[x, z] = direction == BorderChunkDirection.South ? ints[x, z] : sischunkbiome[x, z];
                    }
                    else
                    {
                        r[x, z] = direction == BorderChunkDirection.South ? sischunkbiome[x, z - 16] : ints[x, z - 16];
                    }

                    // Console.WriteLine($"ZZAAZZ {x} {z} || " + r[x, z]);
                }
            }
            else
            {
                r = new int[(16 * 2), 16];
                for (int z = 0; z < r.GetLength(1); z++)
                for (int x = 0; x < r.GetLength(0); x++)
                {
                    if (x < 16)
                    {
                        r[x, z] = direction == BorderChunkDirection.West ? ints[x, z] : sischunkbiome[x, z];
                    }
                    else
                    {
                        r[x, z] = direction == BorderChunkDirection.West ? sischunkbiome[x - 16, z] : ints[x - 16, z];
                    }

                    // Console.WriteLine($"AAAAAAZZ" + r[x, z]);
                }
            }

            return r;
        }

        public virtual void SetHeightMapToChunks(ChunkColumn[] ca, int[,] map)
        {
            int l = (int) Math.Sqrt(ca.Length);
            // Console.WriteLine($"{l} =================================================== 5? {ca.Length}");
            for (int x = 0; x < map.GetLength(0); x++)
            for (int z = 0; z < map.GetLength(1); z++)
            {
                int cnx = (int) Math.Floor(x / 16f);
                int cnz = (int) Math.Floor(z / 16f);
                int cn = cnx + (cnz * l);
                // if()
                // if (cn >= 6) cn--;
                // Console.WriteLine($"{x} {z} >>>>>>> {cn}");
                ChunkColumn cc = ca[cn];

                int rx = x % 16;
                int rz = z % 16;
                int h = map[x, z];
                // int rzz = (15 - rz);
                int rzz = (rz);
                // int rxx = ( 15-rx);
                int rxx = rx;
                // map[x, z] = cc.GetHeight(rx, rz);

                // Console.WriteLine($"{x} AND {z} GAVE ({cnx} AND {cnz}) {cn} AND CHUNK {ca[cn]}");

                // if (x == 0 || z == map.GetLength(0) - 1 || z == 0 || z == map.GetLength(1) - 1)
                // {
                // Console.WriteLine(
                //     $"{x} {z} > GAVE ({cnx} AND {cnz}) CN VAL {cn} ||  {rx} {rz} ({rzz}) >======== {h}");


                cc.SetHeight(rxx, rzz, (short) h);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="yheight"></param>
        /// <param name="maxheight"></param>
        /// <param name="x"></param>
        /// <param name="z"></param>
        /// <param name="cc"></param>
        public virtual void GenerateVerticalColumn(int yheight, int maxheight, int x, int z, ChunkColumn cc,
            bool setair = false)
        {
            // var subChunk = cc.GetSubChunk(yheight);
            // var v = subChunk.GetBlockId(rxx, yheight & 0xf, rzz);
            // Console.WriteLine(subChunk);
            // Console.WriteLine(v);
            // Console.WriteLine("++++++++++++++++++++++++++++++++");

// if(yheight < Sand){}

            if (yheight < maxheight)
            {
                cc.SetBlock(x, yheight, z, new Stone());
            }
            // else if (cc.GetBlockId(rx, y, rz) == 0) break;
            else
            {
                cc.SetBlock(x, yheight, z, new Air());
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public List<int> NotAllowedBlocks = new List<int>()
        {
            new Stone().Id, new Stonebrick().Id, new Sand().Id, new Grass().Id, new Dirt().Id, new Grass().Id,
            new Tallgrass().Id, new DoublePlant().Id, new RedFlower().Id, new ChorusFlower().Id, new YellowFlower().Id
        };

        public int[,] CreateMapFrom8Chunks(ChunkColumn[] ca)
        {
            int l = (int) Math.Sqrt(ca.Length);
            var map = new int[16 * l, 16 * l];
            for (int z = 0; z < map.GetLength(1); z++)
            for (int x = 0; x < map.GetLength(0); x++)
            {
                int rx = x % 16;
                int rz = /*15 -*/ z % 16;
                int cnx = (int) Math.Floor(x / 16f);
                int cnz = (int) Math.Floor(z / 16f);
                int cn = cnx + (cnz * l);
                // if()
                // if (cn >= 6) cn--;
                ChunkColumn cc;

                cc = ca[cn];
                // Console.WriteLine($"{x} AND {z} GAVE ({cnx} AND {cnz}) {cn} AND CHUNK {ca[cn]}");

                map[x, z] = cc.GetHeight(rx, rz);
                // for (int yy = 0; yy < 254; yy++)
                // {
                //     if (cc.GetBlockId(rx, yy, rz) == 0)
                //     {
                //
                //         if(map[x,z] != yy)Console.WriteLine($"ERROR {map[x, z]} WAS SET BUT IS {yy}");
                //         map[x, z] = yy;
                //         break;
                //     }
                // }
            }


            return map;
        }

        public class ChunkColumHeightMap
        {
            private int[] Map;

            public ChunkColumHeightMap(int xlenght, int zlenght)
            {
            }
        }

        public int[,] CreateMapFrom2Chunks(ChunkColumn c1, ChunkColumn c2, int pos)
        {
            if (pos == 0 || pos > 4) return null;

            var map = new int[0, 0];
            if (pos == 1)
                //TOP
                map = new int[16, 32];
            else if (pos == 2)
                //RIGHT
                map = new int[32, 16];
            else if (pos == 3)
                //Bottom
                map = new int[16, 32];
            else if (pos == 4)
                //LEFT
                map = new int[32, 16];

            for (var x = 0; x < map.GetLength(0); x++)
            for (var z = 0; z < map.GetLength(1); z++)
            {
                if (pos == 1)
                {
                    if (x < 16 && z < 16)
                        map[x, z] = c1.GetHeight(x, z);
                    else
                        map[x, z] = c2.GetHeight(x, z - 16);
                }
                else if (pos == 2)
                {
                    if (x < 16 && z < 16)
                        map[x, z] = c1.GetHeight(x, z);
                    else
                        map[x, z] = c2.GetHeight(x - 16, z);
                }
                else if (pos == 3)
                {
                    if (x < 16 && z < 16)
                        map[x, z] = c2.GetHeight(x, z);
                    else
                        map[x, z] = c1.GetHeight(x, z - 16);
                }
                else if (pos == 4)
                {
                    if (x < 16 && z < 16)
                        map[x, z] = c2.GetHeight(x, z);
                    else
                        map[x, z] = c1.GetHeight(x - 16, z);
                }

                // if (x == 0 || z == 0 || z == 16 || x == 16)
                // {
                //     map[x, z] = 157;
                // }
            }

            return map;
        }

        public int getNeighbor(int x, int z, int xo, int zo, int[,] map)
        {
            if (x + xo >= map.GetLength(0) || x + xo < 0) return -1;
            if (z + zo >= map.GetLength(1) || z + zo < 0) return -1;
            // Console.WriteLine($"{x+xo} || {z+zo}");
            return map[x + xo, z + zo];
        }

        List<String> Ran = new List<string>();

        public void SmoothToNeighbor(int x, int z, int xo, int zo, int max, int[,] map)
        {
            var ch = map[x, z];
            //Top
            if (Ran.Contains(x + xo + "||" + z + zo) || x == 0 || z == 0) return;
            var n = getNeighbor(x, z, xo, zo, map);
            if (n != -1)
            {
                // int d = Math.Abs(ch - n);

                // var d = new Random().Next(0, 3);
                if (n > ch)
                {
                    int vvv = Math.Abs(n - ch);


                    if (vvv < 2) vvv = 2;
                    int vv = new Random().Next(1, vvv);
                    // int v = Math.Max(2, vv);
                    var d = new Random().Next(0, vv);
                    /*if (d > 3)*/
                    d = new Random().Next(0, 2);
                    n = ch + d;
                }
                else
                {
                    int vvv = Math.Abs(n - ch) / 2;
                    if (vvv < 2) vvv = 2;
                    int vv = new Random().Next(1, vvv);
                    // int v = Math.Max(2, vv);
                    var d = new Random().Next(0, vv);
                    /*if (d > 3)*/
                    d = new Random().Next(0, 2);
                    n = ch - d;
                }

                map[x + xo, z + zo] = n;
            }
        }

        public int[,] SmoothMapV4Spiral(int[,] map)
        {
            int[,] newmap = new int[map.GetLength(0), map.GetLength(1)];
            int size = 18;
            int x = 0, z = 0;

            int boundary = size - 1;
            int sizeLeft = size - 1;
            int flag = 1;

            // Variable to determine the movement 
            // r = right, l = left, d = down, u = upper 
            char move = 'r';

            // Array for matrix 
            int[,] matrix = new int[size, size];

            for (int i = 1; i < size * size + 1; i++)
            {
                // Assign the value 
                matrix[x, z] = i;
                if (x == 0 || z == 0 || z == 17 || x == 17)
                {
                    matrix[x, z] = map[x, z];
                }
                else
                {
                }

                // switch-case to determine the next index 
                switch (move)
                {
                    // If right, go right 
                    case 'r':
                        z += 1;
                        break;

                    // if left, go left 
                    case 'l':
                        z -= 1;
                        break;

                    // if up, go up 
                    case 'u':
                        x -= 1;
                        break;

                    // if down, go down 
                    case 'd':
                        x += 1;
                        break;
                }

                // Check if the matrix 
                // has reached array boundary 
                if (i == boundary)
                {
                    // Add the left size for the next boundary 
                    boundary += sizeLeft;

                    // If 2 rotations has been made, 
                    // decrease the size left by 1 
                    if (flag != 2)
                    {
                        flag = 2;
                    }
                    else
                    {
                        flag = 1;
                        sizeLeft -= 1;
                    }

                    // switch-case to rotate the movement 
                    switch (move)
                    {
                        // if right, rotate to down 
                        case 'r':
                            move = 'd';
                            break;

                        // if down, rotate to left 
                        case 'd':
                            move = 'l';
                            break;

                        // if left, rotate to up 
                        case 'l':
                            move = 'u';
                            break;

                        // if up, rotate to right 
                        case 'u':
                            move = 'r';
                            break;
                    }
                }
            }

            // Print the matrix 
            for (x = 0; x < size; x++)
            {
                for (z = 0; z < size; z++)
                {
                    int n = matrix[x, z];
                    Console.Write((n < 10)
                        ? (n + " ")
                        : (n + " "));
                }

                Console.WriteLine();
            }

            return newmap;
        }

        public int[,] SmoothMapV3(int[,] map)
        {
            int[,] newmap = new int[map.GetLength(0), map.GetLength(1)];
            // int[,]  newmap = map;

            //SMooth BORDER
            // for (int x = 0; x < map.GetLength(0); x++)
            // {
            //     for (int z = 0; z < map.GetLength(1); z++)
            //     {
            //         if (x == 0 || x == map.GetLength(0) - 1 || z == 0 || z == map.GetLength(1) - 1)
            //         {
            //             if ((x == 0 || x == map.GetLength(0) - 1) && (z == 0 || z == map.GetLength(1) - 1))
            //             {
            //                 continue;
            //             }
            //
            //             int lv = -1;
            //             int nv = -1;
            //             if (z == 0)
            //             {
            //                 lv = map[x - 1, z];
            //                 nv = map[x + 1, z];
            //             }
            //             else if (z == map.GetLength(1) - 1)
            //             {
            //                 lv = map[x - 1, z];
            //                 nv = map[x + 1, z];
            //             }
            //             else if (x == 0)
            //             {
            //                 lv = map[x, z - 1];
            //                 nv = map[x, z + 1];
            //             }
            //             else if (x == map.GetLength(0) - 1)
            //             {
            //                 lv = map[x, z - 1];
            //                 nv = map[x, z + 1];
            //             }
            //
            //             int cv = map[x, z];
            //             int a = (lv + nv) / 2;
            //             int dv = (a - cv);
            //             if (dv > 1)
            //             {
            //                 if (lv > nv)
            //                     a = lv - 1;
            //                 else
            //                     a = lv + 1;
            //             }
            //             else if (dv < -1)
            //             {
            //                 if (lv < nv)
            //                     a = lv + 1;
            //                 else
            //                     a = lv - 1;
            //             }
            //
            //             int fv = a;
            //             // Console.WriteLine($"{x} {z} => {cv} ||| LV{lv} NV{nv} | A{a} | DV{dv} | {fv}");
            //             map[x, z] = fv;
            //             // if (x == 0)
            //             // {
            //             //     map[x, z] = (int) Lerp(map[0, 0], map[0, map.GetLength(1) - 1],
            //             //         (float) z / map.GetLength(1));
            //             // }
            //             // else if (x == map.GetLength(0) - 1)
            //             // {
            //             //     map[x, z] = (int) Lerp(map[map.GetLength(0) - 1, 0],
            //             //         map[map.GetLength(0) - 1, map.GetLength(1) - 1],
            //             //         (float) z / map.GetLength(1));
            //             // }
            //             // else if (z == 0)
            //             // {
            //             //     map[x, z] = (int) Lerp(map[0, 0], map[map.GetLength(0) - 1, 0],
            //             //         (float) x / map.GetLength(0));
            //             // }
            //             // else if (z == map.GetLength(0) - 1)
            //             // {
            //             //     map[x, z] = (int) Lerp(map[0,map.GetLength(1) - 1],
            //             //         map[map.GetLength(0) - 1, map.GetLength(1) - 1], 
            //             //         (float) x / map.GetLength(1));
            //             // }
            //
            //             // float nhx = Lerp(map[0, z], map[map.GetLength(0) - 1, z], (float) x / (map.GetLength(0) - 1));
            //             // float nhz = Lerp(map[x, 0], map[x, map.GetLength(1) - 1], (float) z / (map.GetLength(1) - 1));
            //             //
            //             // // map[x, z] =
            //             //     int v = map[x, z];
            //         }
            //     }
            // }

            for (int x = 0; x < map.GetLength(0); x++)
            {
                for (int z = 0; z < map.GetLength(1); z++)
                {
                    if (x == 0 || x == map.GetLength(0) - 1 || z == 0 || z == map.GetLength(1) - 1)
                    {
                        newmap[x, z] = map[x, z];
                        continue;
                    }

                    int cv = map[x, z];
                    int lvx = map[x - 1, z];
                    int lvz = map[x, z - 1];
                    int nvx = map[x + 1, z];
                    int nvz = map[x, z + 1];
                    int c1 = map[x - 1, z + 1];
                    int c2 = map[x + 1, z + 1];
                    int c3 = map[x - 1, z - 1];
                    int c4 = map[x - 1, z - 1];
                    int tvx = map[map.GetLength(0) - 1, z];
                    int tvz = map[x, map.GetLength(1) - 1];
                    int lndx = nvx - lvx;
                    int lndz = nvz - lvz;
                    int lnax = (nvx + lvx) / 2;
                    //Smooth Z
                    int lnaz = (nvz + lvz + lnax) / 3;
                    newmap[x, z] = lnaz;
                    // Console.WriteLine($" OK >> LAN> {lnax} {lnaz}");
                    // map[x, z] = lnax;
                    float lnaxm = lnax / (float) cv;
                    float lnazm = lnaz / (float) cv;
                    // int m =  
                    // int lna = (int) Math.Ceiling((lnax + lnaz + c1 + c2 + c3 + c4) / 6f);
                    // if (nvx > lna+1)
                    // {
                    //     lna = lvx + 1;
                    // }else if (nvz > lna + 1)
                    // {
                    //     lna = lvz + 1;
                    // }
                    // if (lvx + 1 < lna)
                    // { - cv;
                    //     lna = lvx + 1;
                    //     if(nvx < lna)
                    //         lna = nvx - 1;
                    // }
                    // if (lvz + 1 < lna)
                    // {
                    //     lna = lvz + 1;
                    //     if(nvz < lna)
                    //         lna = nvz - 1;
                    // }

                    //X AXIS
                    // float nhx = Lerp(map[0, z], map[map.GetLength(0) - 1, z], (float) x / (map.GetLength(0) - 1));
                    // //Z AXIS
                    // float nhz = Lerp(map[x, 0], map[x, map.GetLength(1) - 1], (float) z / (map.GetLength(1) - 1));
                    // newmap[x, z] = (int) Math.Floor((nhx + nhz) / 2f);
                    // newmap[x, z] = lna;

                    // Console.WriteLine($"LERPING CHHUNK FROM {map[x,0]} TO {map[x,map.GetLength(1) - 1]} WITH {(float)z / map.GetLength(1)} ====== {nhz}");
                    // float nhx = Lerp(map[0, z], map[map.GetLength(0) - 1, z], x / map.GetLength(0));
                    // float nhz = Lerp(map[x, 0], map[x, map.GetLength(1) - 1], z / map.GetLength(1));
                    // newmap[x, z] = (int) Math.Floor(((float)(x+z)/(map.GetLength(0)+map.GetLength(1)))*100 + 50);
                    // newmap[x, z] = (int) Math.Floor(( nhz) );
                    // newmap[x, z] = BiomeQualifications.baseheight + x+z;
                }
            }

            return newmap;
        }

        public int[,] SmoothMapV2(int[,] map)
        {
            int[,] newmap = new int[map.GetLength(0), map.GetLength(1)];
            // int[,]  newmap = map;
            for (int x = 0; x < map.GetLength(0); x++)
            {
                for (int z = 0; z < map.GetLength(1); z++)
                {
                    if (x == 0 || x == map.GetLength(0) || z == 0 || z == map.GetLength(1))
                    {
                        newmap[x, z] = map[x, z];
                        continue;
                    }

                    float nhx = Lerp(map[x, 0], map[x, map.GetLength(1) - 1], z / map.GetLength(1));
                    float nhz = Lerp(map[0, z], map[map.GetLength(0) - 1, z], x / map.GetLength(0));
                    // float nhx = Lerp(map[0, z], map[map.GetLength(0) - 1, z], x / map.GetLength(0));
                    // float nhz = Lerp(map[x, 0], map[x, map.GetLength(1) - 1], z / map.GetLength(1));
                    newmap[x, z] = (int) Math.Floor((nhx + nhz) / 2f);
                }
            }

            return newmap;
        }

        public float Lerp(int firstFloat, int secondFloat, float by)
        {
            return firstFloat * (1 - by) + secondFloat * by;
        }

        public static void printDisplayTable(int[,] table, string title = "Testing1")
        {
            var mx = table.GetLength(0);
            var mz = table.GetLength(1);

            var t2 = 58 - title.Length / 2;
            if (title.Length % 2 != 0) t2++;
            var t1 = 58 - title.Length / 2;
            var c1 = new string[t1];
            var c2 = new string[t2];
            c1.Fill("=", t1);
            c2.Fill("=", t2);
            var mastertitle = c1._tostring() + title + c2._tostring();
            Console.WriteLine(mastertitle);
            mastertitle = "";
            for (var z = 0; z < mz; z++)
            {
                var s = "";
                for (var x = 0; x < mx; x++)
                {
                    s += $"{table[x, z]},";
                    // if (x + 1 == mx) s += "||";
                }

                mastertitle += "\n" + s;
                Console.WriteLine(s);
            }

            Console.WriteLine("==========================================================");
            System.IO.File.WriteAllText($@"D:\MINET\{title}.csv", mastertitle);
        }


        public void FormatChunk(int x, int z, int xo, int zo, int dif, ChunkColumn chunk, ChunkColumn nc)
        {
            if (x < 16 && z < 16)
            {
                for (var y = 20; y < 255; y++)
                {
                    if (y > dif)
                    {
                        if (chunk.GetBlockId(x, y, z) == 0) break;
                        chunk.SetBlock(x, y, z, new Air());
                    }
                    else if (y == dif)
                    {
                        chunk.SetBlock(x, y, z, new StainedGlass()
                        {
                            Color = "orange"
                        });
                    }
                    else
                    {
                        // if (chunk.GetBlockId(x, y, z) == 0)
                        // {
                        chunk.SetBlock(x, y, z, new Stone());
                        // }
                    }
                }
            }
            else
            {
                for (var y = 20; y < 255; y++)
                {
                    if (y > dif)
                    {
                        if (nc.GetBlockId(x - xo, y, z - zo) == 0) break;
                        nc.SetBlock(x - xo, y, z - zo, new Air());
                    }
                    else if (y == dif)
                    {
                        nc.SetBlock(x - xo, y, z - zo, new StainedGlass()
                        {
                            Color = "orange"
                        });
                    }
                    else
                    {
                        // if (chunk.GetBlockId(x, y, z) == 0)
                        // {
                        nc.SetBlock(x - xo, y, z - zo, new Stone());
                        // }
                    }
                }
            }
        }


        public static int max = 0;

        // public virtual void SmoothChunk(CyberExperimentalWorldProvider o, ChunkColumn chunk, float[] rth)
        // {
        //     //Smooth Biome
        //
        //     if (BorderChunk)
        //     {
        //         // max++;
        //         chunk.SetBlock(8, 125, 8, new EmeraldBlock());
        //         AdvancedBiome n;
        //         var nc = new ChunkColumn();
        //         var pos = 0;
        //         int[,] h = null;
        //         var i = -1;
        //
        //
        //         ChunkColumn[] chunks = new ChunkColumn[25];
        //         int ab = 0;
        //         for (int zz = 2; zz >= -2; zz--)
        //         for (int xx = -2; xx <= 2; xx++)
        //         {
        //             int k = xx + 2 + ((zz + 2) * (int) Math.Sqrt(chunks.Length));
        //             // Console.WriteLine($"#{ab} || {xx} {zz} || {k}");
        //             if (xx == 0 && zz == 0) chunks[k] = chunk;
        //             else
        //                 chunks[k] = o.OpenPreGenerateChunkColumn(
        //                     new ChunkCoordinates {X = chunk.X + xx, Z = chunk.Z + zz},
        //                     false);
        //             ab++;
        //         }
        //
        //
        //         h = CreateMapFrom8Chunks(chunks);
        //         var nh = SmoothMapV3(h);
        //         nh = SmoothMapV4(nh);
        //
        //         // printDisplayTable(h, $"C{chunk.X}{chunk.Z}Pre");
        //         // printDisplayTable(nh, $"C{chunk.X}{chunk.Z}Post");
        //
        //         SetHeightMapToChunks(chunks, nh);
        //     }
        //
        //     // if (pos == 0) 
        //     // {
        //     //     Console.WriteLine("ERRRRRRRRRRRR NOOOOOOOOOOaaaaaaaa SMOOOOOOOOOOOOOOTHHHHHHHHH");
        //     // }
        //     // else
        //     // {
        //     //     // var nh = SmoothMapV2(h);
        //     //     // printDisplayTable(nh);
        //     //
        //     //     // chunk.SetBlock(8, 109, 8, new Netherrack());
        //     //     // chunk.SetBlock(8, 109 - pos, 8, new EmeraldBlock());
        //     //     Console.WriteLine($"ABOUT TO SMOOTH BUT POS={pos} NC={nc} ");
        //     // }
        //     //
        //     // if (pos != 0 && nc != null)
        //     // {
        //     //     chunk.SetBlock(8, 111, 8, new RedstoneBlock());
        //     //     Console.WriteLine($"SMOOTHING CHUNK {chunk.X} {chunk.Z}");
        //     //     var nh = SmoothMapV3(h);
        //     //
        //     //     printDisplayTable(nh,$"{chunk.X} {chunk.Z}");
        //     //     
        //     //
        //     //     // var xx = 0;
        //     //     // var zz = 0;
        //     //     Console.WriteLine($" X:{nh.GetLength(0)} ||| Z:{nh.GetLength(1)}");
        //     //     for (var z = 0; z < nh.GetLength(1); z++)
        //     //     {
        //     //         for (var x = 0; x < nh.GetLength(0); x++)
        //     //         {
        //     //             // Console.WriteLine($"STARTING ON {x} ::: {z}");
        //     //             // for (var z = sz; z < stopz; z++)
        //     //             // {
        //     //             //     for (var x = sx; x < stopx; x++)
        //     //             //     {
        //     //             var dif = nh[x, z];
        //     //             // dif = h[x, z];
        //     //
        //     //             // for (var y = 50; y < 255; y++)
        //     //             // {
        //     //             //     if (y >= dif)
        //     //             //         o.Level.SetBlock(new Air()
        //     //             //         {
        //     //             //             Coordinates = new BlockCoordinates(sx+x,y,sz+z)
        //     //             //         });
        //     //             //     if (o.Level.GetBlock(new PlayerLocation(x, y, z)).Id == 0) break;
        //     //             // }
        //     //
        //     //             if (pos == 1 || pos == 3)
        //     //             {
        //     //                 if (pos == 1)
        //     //                 {
        //     //                     chunk.SetBlock(8, 110, 8 + 1, new Furnace());
        //     //                     FormatChunk(x, z, 0, 16, dif, chunk, nc);
        //     //                 }
        //     //                 else if (pos == 3)
        //     //                 {
        //     //                     chunk.SetBlock(8, 110, 8 - 1, new Furnace());
        //     //                     FormatChunk(x, z, 0, 16, dif, nc, chunk);
        //     //                 }
        //     //             }
        //     //             else if (pos == 2 || pos == 4)
        //     //             {
        //     //                 // if(x == 0 || x == 16 )
        //     //                 if (pos == 2)
        //     //                 {
        //     //                     chunk.SetBlock(8 + 1, 110, 8, new Furnace());
        //     //                     FormatChunk(x, z, 16, 0, dif, chunk, nc);
        //     //                 }
        //     //                 else if (pos == 4)
        //     //                 {
        //     //                     chunk.SetBlock(8 - 1, 110, 8, new Furnace());
        //     //                     FormatChunk(x, z, 16, 0, dif, nc, chunk);
        //     //                 }
        //     //             }
        //     //
        //     //             // xx++;
        //     //         }
        //     //
        //     //         // xx = 0;
        //     //         // zz++;
        //     //     }
        //     // }
        // }


        public int[,] SmoothMapV4(int[,] map)
        {
            int[,] newmap = map;
            for (int z = 1; z < map.GetLength(1) - 2; z++)
            {
                var xstrip = SmoothStrip(Fillstripx(0, map.GetLength(0) - 1, z, map));
                int cnt = 0;
                for (int x = 1; x < map.GetLength(0) - 2; x++)
                {
                    map[x, z] = xstrip[cnt];
                    cnt++;
                }
            }

            for (int x = 1; x < map.GetLength(0) - 2; x++)
            {
                var zstrip = SmoothStrip(Fillstripz(0, map.GetLength(1) - 1, x, map));
                int cnt = 0;
                for (int z = 1; z < map.GetLength(1) - 2; z++)
                {
                    map[x, z] = zstrip[cnt];
                    cnt++;
                }
            }


            // for (int x = 1; x < map.GetLength(0) - 2; x++)
            // {
            //     for (int z = 1; z < map.GetLength(1) - 2; z++)
            //     {
            //         int cv = map[x, z];
            //         int lvz = map[x , z-1];
            //         int nvz = map[x , 1+z];
            //         int d = -lvz + nvz;
            //         int v1 = lvz - cv;
            //         int v2 = cv - nvz;
            //         Console.WriteLine($"{x} {z} ||>> {lvz} | {cv} | {nvz} ||| {v1} VS {v2} TO");
            //         if (v1 >= 1 || v2 >= 1 || v1 <= -1 || v2 <= -1)
            //             // if ((v1 >= 1 || v2 >= 1) && (v1 <= -1 || v2 <= -1))
            //         {
            //             //Between -1 & 1
            //             // newmap[x, z] = map[x, z];
            //             continue;
            //         }
            //
            //         newmap[x, z] = lvz + ((d >= 0 ? 1 : -1));
            //         //Z SMOOTH NOW
            //     }
            // }

            return newmap;
        }

        private int[] SmoothStrip(int[] fillstripz)
        {
            for (int i = 0; i < fillstripz.Length - 1; i++)
            {
                if (i == 0 || i == fillstripz.Length - 1) continue;
                int lv = fillstripz[i - 1];
                int nv = fillstripz[i + 1];
                int v = fillstripz[i];
                //DOWN OR UP
                int du = nv - lv;
                //UP
                if (du > 0)
                {
                    v = lv + 1;
                }
                else if (du < 0)
                {
                    v = lv - 1;
                }
                else v = lv;

                fillstripz[i] = v;
            }

            return fillstripz;
        }

        private int[] Fillstripx(int i, int getLength, int z, int[,] map)
        {
            List<int> strip = new List<int>();
            for (int a = i; a < getLength; a++)
            {
                strip.Add(map[a, z]);
            }

            return strip.ToArray();
        }

        private int[] Fillstripz(int i, int getLength, int x, int[,] map)
        {
            List<int> strip = new List<int>();
            for (int a = i; a < getLength; a++)
            {
                strip.Add(map[x, a]);
            }

            return strip.ToArray();
        }

        public static AdvancedBiome GetBiome(int biomeId)
        {
            return null;
        }


        public static float GetNoise(int x, int z, float scale, int max)
        {
            var heightnoise = new FastNoise(123123 + 2);
            heightnoise.SetNoiseType(FastNoise.NoiseType.SimplexFractal);
            heightnoise.SetFrequency(scale);
            heightnoise.SetFractalType(FastNoise.FractalType.FBM);
            heightnoise.SetFractalOctaves(1);
            heightnoise.SetFractalLacunarity(2);
            heightnoise.SetFractalGain(.5f);
            return (heightnoise.GetNoise(x, z) + 1) * (max / 2f);
            // return (float) ((OpenNoise.Evaluate(x * scale, z * scale) + 1f) * (max / 2f));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="CyberExperimentalWorldProvider"></param>
        /// <param name="chunk"></param>
        /// <param name="rth"></param>
        /// <returns></returns>
        public virtual async Task<ChunkColumn> GenerateSurfaceItems(
            CyberExperimentalWorldProvider CyberExperimentalWorldProvider,
            ChunkColumn chunk, float[] rth)
        {
            return chunk;
        }

        // public object CropToSmoothChunks(int[,] f2, ChunkCoordinates chunk, CyberExperimentalWorldProvider c)
        // {
        //     var l = new ListMap2d();
        //
        //     var cw = BiomeManager.GetBiome(new ChunkCoordinates(chunk.X - 1, chunk.Z));
        //     var ce = BiomeManager.GetBiome(new ChunkCoordinates(chunk.X + 1, chunk.Z));
        //     var cs = BiomeManager.GetBiome(new ChunkCoordinates(chunk.X, chunk.Z - 1));
        //     var cn = BiomeManager.GetBiome(new ChunkCoordinates(chunk.X, chunk.Z + 1));
        //     var ccnw = BiomeManager.GetBiome(new ChunkCoordinates(chunk.X + 1, chunk.Z - 1));
        //     var ccne = BiomeManager.GetBiome(new ChunkCoordinates(chunk.X + 1, chunk.Z + 1));
        //     var ccsw = BiomeManager.GetBiome(new ChunkCoordinates(chunk.X - 1, chunk.Z - 1));
        //     var ccse = BiomeManager.GetBiome(new ChunkCoordinates(chunk.X - 1, chunk.Z + 1));
        //     // int[] west =
        //     //     SideGenerateChunkHeightMap(ChunkSide.West.Opposite(), new ChunkCoordinates(chunk.X - 1, chunk.Z));
        //     int[] west =
        //         cw.SideGenerateChunkHeightMap(ChunkSide.West.Opposite(), new ChunkCoordinates(chunk.X - 1, chunk.Z), c);
        //     int[] north =
        //         cn.SideGenerateChunkHeightMap(ChunkSide.North.Opposite(), new ChunkCoordinates(chunk.X, chunk.Z + 1),
        //             c);
        //     int[] east =
        //         ce.SideGenerateChunkHeightMap(ChunkSide.East.Opposite(), new ChunkCoordinates(chunk.X + 1, chunk.Z), c);
        //     int[] south =
        //         cs.SideGenerateChunkHeightMap(ChunkSide.South.Opposite(), new ChunkCoordinates(chunk.X, chunk.Z - 1),
        //             c);
        //
        //     foreach (var b in BorderChunkDirections)
        //     {
        //         if (b == BorderChunkDirection.NW)
        //         {
        //             l.set(17, 0, ccnw.CornerGenerateChunkHeightMap(ChunkCorner.NorthWest.Opposite(),
        //                 new ChunkCoordinates(chunk.X + 1, chunk.Z - 1), c));
        //         }
        //
        //         if (b == BorderChunkDirection.NE)
        //         {
        //             l.set(0, 0, ccne.CornerGenerateChunkHeightMap(ChunkCorner.NorthEast.Opposite(),
        //                 new ChunkCoordinates(chunk.X + 1, chunk.Z + 1), c));
        //         }
        //
        //         if (b == BorderChunkDirection.SE)
        //         {
        //             l.set(0, 0, ccsw.CornerGenerateChunkHeightMap(ChunkCorner.SouthWest.Opposite(),
        //                 new ChunkCoordinates(chunk.X - 1, chunk.Z - 1), c));
        //         }
        //
        //         if (b == BorderChunkDirection.SW)
        //         {
        //             l.set(0, 0, ccsw.CornerGenerateChunkHeightMap(ChunkCorner.SouthWest.Opposite(),
        //                 new ChunkCoordinates(chunk.X - 1, chunk.Z - 1), c));
        //         }
        //     }
        // }
        /// <summary>
        /// DO NOT USE
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            return MemberwiseClone();
        }

        public AdvancedBiome CClone()
        {
            AdvancedBiome a = (AdvancedBiome) MemberwiseClone();
            a.BorderChunkDirections = new List<BorderChunkDirection>();
            a.BorderChunk = false;
            return a;
        }

        public int[,] CropToSmoothChunks(int[,] f2, ChunkCoordinates chunkCoordinates,
            CyberExperimentalWorldProvider cyberExperimentalWorldProvider)
        {
            // Console.WriteLine("STARTING TO CROP");
            bool n = false;
            bool e = false;
            bool s = false;
            bool w = false;
            bool nw = false;
            bool ne = false;
            bool sw = false;
            bool se = false;

            if (f2[f2.GetLength(0) - 1, 0] == -1) nw = true;
            if (f2[0, 0] == -1) sw = true;
            if (f2[0, f2.GetLength(1) - 1] == -1) se = true;
            if (f2[f2.GetLength(0) - 1, f2.GetLength(1) - 1] == -1) ne = true;

            for (int i = 1; i <= 16; i++)
            {
                bool ss = f2[i, 0] == -1;
                if (!ss && !s)
                {
                    // Console.WriteLine("SS"+f2[i, 0]);
                    s = true;
                }

                bool ww = f2[0, i] == -1;
                if (!ww && !w)
                {
                    Console.WriteLine("WW" + f2[0, i]);
                    w = true;
                }
                // else
                // {
                //     Console.WriteLine("WW WAS GOOD "+ww+"|||"+f2[0,i]);
                // }

                bool nn = f2[i, f2.GetLength(1) - 1] == -1;
                if (!nn && !n)
                {
                    // Console.WriteLine("NN"+f2[i, 0]);
                    n = true;
                }

                bool ee = f2[f2.GetLength(0) - 1, i] == -1;
                // Console.WriteLine("WWWW"+f2[i, 0]);
                if (!ee && !e)
                {
                    // Console.WriteLine("EE"+f2[i, 0]);
                    e = true;
                }
            }

            n = !n;
            e = !e;
            s = !s;
            w = !w;


            Console.WriteLine($"SIDES222222 TOGGELD N:{n} E:{e} S:{s} W:{w} NE:{ne} NW:{nw} SE:{se} SW:{sw}");
            Console.WriteLine($"YOOOO BUT THE BORDER CHUNK DIRECTSIONS>>:");
            foreach (var bcd in BorderChunkDirections)
            {
                Console.WriteLine($"############:{bcd}");
            }

            int rzt = 0;
            int rzb = 0;
            int rxr = 0;
            int rxl = 0;
            if (n && ne && nw)
            {
                rzt++;
            }

            if (ne && e && se)
            {
                rxr++;
            }

            if (s && se && sw)
            {
                rzb++;
            }

            if (w && sw && nw)
            {
                rxl++;
            }

            int fx = f2.GetLength(0) - rxl - rxr;
            int fz = f2.GetLength(1) - rzb - rzt;
            int[,] f1 = new int[fx, fz];
            for (int z = 0; z < fz; z++)
            for (int x = 0; x < fx; x++)
            {
                int xx = x + rxl;
                int zz = z + rzb;
                f1[x, z] = f2[xx, zz];
            }

            // Console.WriteLine("DONE TO CROP");
            return f1;
        }

        public int[,] LerpX(int[,] f22, bool ew = false)
        {
            int[,] r = new int[f22.GetLength(0), f22.GetLength(1)];
            Console.WriteLine($"LERPX {r.GetLength(0)} {r.GetLength(1)}");
            for (int z = 0; z < f22.GetLength(1); z++)
            {
                // Console.WriteLine($"starting LERP X ON Z:{z} STARTING X: {startx} AND {stopx}");
                for (int x = 0; x < f22.GetLength(0); x++)
                {
                    if (z == 0 && z == f22.GetLength(1) - 1)
                    {
                        r[x, z] = f22[x, z];
                    }
                    else
                    {
                        if ((x == 0 || x == f22.GetLength(0) - 1) && ew && false)
                        {
                            r[x, z] = f22[x, z];
                        }
                        else
                        {
                            int ah = 0;
                            int ac = 0;
                            for (int i = -3; i < 3; i++)
                            {
                                if (0 > x + i || f22.GetLength(0) <= x + i) continue;
                                ah += f22[x + i, z];
                                ac++;
                            }

                            r[x, z] = ah / ac;
                        }
                    }
                }
            }

            return r;
        }


        public int[,] LerpZ(int[,] f22, bool ns = false)
        {
            int[,] r = new int[f22.GetLength(0), f22.GetLength(1)];

            Console.WriteLine($"LERPZ {r.GetLength(0)} {r.GetLength(1)}");
            for (int x = 0; x < f22.GetLength(0); x++)
            {
                int startz = f22[x, 0];
                int stopz = f22[x, f22.GetLength(1) - 1];
                // Console.WriteLine($"starting LERP Z ON X:{x} STARTING X: {startz} AND {stopz}");

                for (int z = 0; z < f22.GetLength(1); z++)
                {
                    if (x == 0 || x == f22.GetLength(0) - 1)
                    {
                        r[x, z] = f22[x, z];
                    }
                    else
                    {
                        if ((z == 0 || z == f22.GetLength(1) - 1) && ns && false)
                        {
                            r[x, z] = f22[x, z];
                        }
                        else
                        {
                            int ah = 0;
                            int ac = 0;
                            for (int i = -3; i < 3; i++)
                            {
                                if (0 > z + i || f22.GetLength(1) <= z + i) continue;
                                ah += f22[x, z + i];
                                ac++;
                            }

                            r[x, z] = ah / ac;
                            // r[x, z] = ((int) Lerp(startz, stopz, (float) z / (f22.GetLength(1) - 1))+f22[x,z])/2;
                        }
                    }
                }
            }

            return r;
        }

        public int[,] FinalCropTo16(int[,] f2, BorderChunkDirection direction)
        {
            int[,] f1 = new int[16, 16];
            int xo = 0;
            int zo = 0;

            if (direction == BorderChunkDirection.South)
            {
                zo = 16;
            }
            else if (direction == BorderChunkDirection.West)
            {
                xo = 16;
            }

            for (int z = 0; z < 16; z++)
            for (int x = 0; x < 16; x++)
            {
                // Console.WriteLine($"CURRENT OFFSET {xo} {zo} || {direction}");
                // Console.WriteLine($"X:{x} Z:{z} ||| {xo + x} {zo + z} |A| {f2[xo + x, zo + z]} || {direction}");
                f1[x, z] = f2[xo + x, z + zo];
            }

            return f1;
        }
    }
}