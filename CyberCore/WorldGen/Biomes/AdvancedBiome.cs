using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using CyberCore.Manager.FloatingText;
using CyberCore.Utils;
using log4net;
using log4net.Util.TypeConverters;
using MiNET.Blocks;
using MiNET.Items;
using MiNET.Utils;
using MiNET.Worlds;
using OpenAPI.World;

namespace CyberCore.WorldGen.Biomes
{
    public abstract class AdvancedBiome : ICloneable
    {
        public byte MCPEBiomeID
        {
            get;
            protected set;
        } = 255;
        protected int Waterlevel = 75+20;
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

        public int LocalId = -1;
        public string Name;
        public Random RNDM = new Random();
        public int Startheight = 80;

        public AdvancedBiome(string name, BiomeQualifications bq)
        {
            BiomeQualifications = bq;
            HeightNoise.SetGradientPerturbAmp(3);
            HeightNoise.SetFrequency(.24f);
            HeightNoise.SetNoiseType(FastNoise.NoiseType.CubicFractal);
            HeightNoise.SetFractalOctaves(2);
            HeightNoise.SetFractalLacunarity(.35f);
            HeightNoise.SetFractalGain(1);
            Name = name;
        }
private Block _Stone = new Stone();
private Random RDM = new Random();
        public void TryOreGeneraton(ChunkColumn cc, int x, int z, int yheight, Block b = null)
        {
            if (b == null) b = (Block) _Stone.Clone();
            int v = RDM.Next(100 + yheight);
            if (v <= 15) //15
            {
                // r = new Random(2 + x + z + yheight * 2);
                v = RDM.Next(500) / 10; //Max 50;
//Iron 30% 50*.3=15
                if (v < 15)
                    cc.SetBlock(x, yheight, z, new IronOre());
//Gold 15% = 7.5 = 7
                else if (v < 22)
                    cc.SetBlock(x, yheight, z, new GoldOre());
//Lapis 15% = 7.5 = 7
                else if (v < 29)
                    cc.SetBlock(x, yheight, z, new LapisOre());
//Redstone 15% = 7.5 = 7
                else if (v < 36)
                    cc.SetBlock(x, yheight, z, new RedstoneOre());
//Diamond 5% = 2.5 = 3 
                else if (v < 39)
                    cc.SetBlock(x, yheight, z, new DiamondOre());
//Emerald 2% = 1
                else if (v < 40)
                    cc.SetBlock(x, yheight, z, new EmeraldOre());
                else
                {
                    cc.SetBlock(x, yheight, z, b);
                }
                //TODO Add Chance Section for Unique Dirt Types... Try to use Noise map
            }
            else
            {
                cc.SetBlock(x, yheight, z, b);
            }
        }

        public int BorderType { get; set; } = 0;

        public abstract int GetSh(int x, int z, int cx, int cz);

        public bool Check(float[] rth)
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
                    r = GetSh(0, 15, c.X, c.Z);
                    break;
                case ChunkCorner.NorthEast:
                    r = GetSh(15, 15, c.X, c.Z);
                    break;
                case ChunkCorner.SouthWest:
                    r = GetSh(0, 0, c.X, c.Z);
                    break;
                case ChunkCorner.SouthEast:
                    r = GetSh(15, 0, c.X, c.Z);
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
                        r[x] = GetSh(x, 15, c.X, c.Z);
                    }

                    break;
                case ChunkSide.East:
                    for (int z = 0; z < 16; z++)
                    {
                        r[z] = GetSh(15, z, c.X, c.Z);
                    }

                    break;
                case ChunkSide.South:
                    for (int x = 0; x < 16; x++)
                    {
                        r[x] = GetSh(x, 0, c.X, c.Z);
                    }

                    break;
                case ChunkSide.West:
                    for (int z = 0; z < 16; z++)
                    {
                        r[z] = GetSh(0, z, c.X, c.Z);
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


        public async Task<ChunkColumn> prePopulate(CyberExperimentalWorldProvider CyberExperimentalWorldProvider,
            ChunkColumn chunk,
            float[] rth)
        {
            var t = new Stopwatch();
            t.Start();

            var a = GenerateUseabelHeightMap(CyberExperimentalWorldProvider, chunk);
            PopulateChunk(CyberExperimentalWorldProvider, chunk, rth, a);

            if (BorderChunk)
            {
                chunk.SetBlock(7, 150, 7, new EmeraldBlock());
            }

            PostPopulate(CyberExperimentalWorldProvider, chunk, rth, a);

            t.Stop();


            if (t.ElapsedMilliseconds > 100)
            {
                Log.Debug($"Chunk Population of X:{chunk.X} Z:{chunk.Z} took {t.Elapsed}");
            }

            return chunk;
        }

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
                   if(MCPEBiomeID != 255) c.SetBiome(x,z,MCPEBiomeID);

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
            ChunkColumn c)
        {
            var m = GenerateChunkHeightMap(c, CyberExperimentalWorldProvider);

            if (BorderChunk)
            {
                SmoothingMap sm = HandleGeneration(m, new ChunkCoordinates(c.X, c.Z),
                    CyberExperimentalWorldProvider);
                // SaveViaCSV($"/MapTesting/MAPCHUNK NNNNN PRE SMOOTH EXPAND {c.X} {c.Z}.csv",IntArrayToString(sm.Map));
                // sm.AddBorderValues(CyberExperimentalWorldProvider);
                // SaveViaCSV($"/MapTesting/MAPCHUNK NNNNN POST BORDER EXPAND {c.X} {c.Z}.csv",IntArrayToString(sm.Map));
                // sm.SquareSmooth(3);
                // sm.StripSmooth(1);//4
                sm.SmoothMapV4();
                // SaveViaCSV($"/MapTesting/MAPCHUNK NNNNN POST SMOOTH EXPAND {c.X} {c.Z}.csv",IntArrayToString(sm.Map));
                // sm.StripSmooth(3);
                // SaveViaCSV($"/MapTesting/MAPCHUNK NNNNN POST SMOOTH EXPAND2 {c.X} {c.Z}.csv",IntArrayToString(sm.Map));
                m = sm.SetChunks(CyberExperimentalWorldProvider);
                // m[7, 7] = 100;
            }


            for (int x = 0; x < 16; x++)
            {
                for (int z = 0; z < 16; z++)
                {
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

        public async Task<ChunkColumn> GenerateTree(ChunkColumn chunk, float minrain = .5f, int treerandom = 3)
        {
            // Console.WriteLine($"TRYINGGGG FORRR TEEEEEEEEEEEEEEEEEEEEEEEEEEEE {cx} {cz}|| {x} {z}");
            var c = chunk;
            var ffy = 0;

            var cx = chunk.X;
            var cz = chunk.Z;
            var rx = new Random().Next(0, 15);
            var rz = new Random().Next(0, 15);
            var x = cx * 16 + rx;
            var z = cz * 16 + rz;
            int fy = chunk.GetHeight(rx, rz);
            var max = 20;
            var rain = BiomeManager.GetRainNoiseBlock(x, z) * 1.25f;
            if (rain > minrain)
            {
                // ccc++;
                var runamt = (int) Math.Ceiling(rain * 10f);
                // Console.WriteLine($"TRY AMOUNT IS {runamt}<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<");
                for (var ttry = 0; ttry < runamt; ttry++)
                    //1 In 16 Chance
                    if (new Random().Next(0, treerandom) == 0)
                    {
                        //RESET VALUES
                        rx = new Random().Next(0, 15);
                        rz = new Random().Next(0, 15);
                        x = cx * 16 + rx;
                        z = cz * 16 + rz;
                        fy = chunk.GetHeight(rx, rz);
                        //ACTUALLY RUN NOW 
                        var w = RNDM.Next(3, 5);
                        var h = RNDM.Next(6, 14);
                        var v = h - w;
                        var vv = 0;
                        ffy = fy + h;
                        for (var hh = 1; hh < h; hh++)
                        {
                            c.SetBlock(rx, fy + hh, rz, new Wood
                            {
                                WoodType = "birch"
                            });
                            //Bottom Half Leaves
                            if (hh > v)
                            {
                                vv++;
                                var ww = vv;
                                //Vertically Covers The Leaves
                                for (var teir = 1; teir <= ww; teir++)
                                    //
                                for (var teirn = 1; teirn <= teir; teirn++)
                                for (var xx = -teirn; xx <= teirn; xx++)
                                for (var zz = -teirn; zz <= teirn; zz++)
                                {
                                    if (xx == 0 && zz == 0) continue;

                                    if (rx + xx >= 0 && rx + xx < 16 && rz + zz >= 0 && rz + zz < 16)
                                        // Log.Error($"PUTTIN LEAVES AT {rx+xx} , {fy+ hh} , {rz+zz} || REAL {x+xx} , {fy+ hh} , {z+zz} || {cx} {cz}");
                                        c.SetBlock(rx + xx, fy + hh, rz + zz, new Leaves
                                        {
                                            OldLeafType = "jungle"
                                        });
                                    else
                                        // Log.Error($"PUTTIN LATEEEEEEEEEEEEE LEAVES AT {x+xx} , {fy+ hh} , {z+zz} || {cx} {cz} || {(x+xx >> 4)} {z+zz >> 4} || X&Z {xx} {zz} || {(x+xx)%16}  {(z+zz)%16} ");

                                        CyberExperimentalWorldProvider
                                            .AddBlockToBeAddedDuringChunkGeneration(
                                                new ChunkCoordinates((x + xx) >> 4, (z + zz) >> 4), new Leaves
                                                {
                                                    OldLeafType = "jungle",
                                                    Coordinates = new BlockCoordinates(x + xx, fy + hh, z + zz)
                                                });
                                }
                            }
                        }

                        // Top Leaves
                        for (var vvv = vv; vvv > 0; vvv--)
                        {
                            for (var teir = vvv; teir > 0; teir--)
                            for (var teirn = 1; teirn <= teir; teirn++)
                            for (var xx = -teirn; xx <= teirn; xx++)
                            for (var zz = -teirn; zz <= teirn; zz++)
                            {
                                // if(xx == 0 && zz == 0)continue;

                                if (rx + xx >= 0 && rx + xx < 16 && rz + zz >= 0 && rz + zz < 16)
                                    c.SetBlock(rx + xx, ffy, rz + zz, new Leaves
                                    {
                                        OldLeafType = "jungle"
                                    });

                                else
                                    CyberExperimentalWorldProvider
                                        .AddBlockToBeAddedDuringChunkGeneration(
                                            new ChunkCoordinates((x + xx) >> 4, (z + zz) >> 4), new Leaves
                                            {
                                                OldLeafType = "jungle",
                                                Coordinates = new BlockCoordinates(x + xx, ffy, z + zz)
                                            });
                            }

                            ffy++;
                        }

                        for (var teir = 0; teir <= v; teir++)
                        {
                        }
                    }
            }

            return chunk;
        }


        public SmoothingMap HandleGeneration(int[,] ints, ChunkCoordinates c,
            CyberExperimentalWorldProvider cyberExperimentalWorldProvider)
        {
            SmoothingMap sm = new SmoothingMap(c, ints);
            //Console.WriteLine($"YAAAAAA: >>>>");
            foreach (var VARIABLE in BorderChunkDirections)
            {
                //Console.WriteLine("TTTTTT: " + VARIABLE);
            }


            BorderChunkDirection d = BorderChunkDirection.North;
            CheckAndAddChunkDirection(d, c, sm, cyberExperimentalWorldProvider);
            d = BorderChunkDirection.East;
            CheckAndAddChunkDirection(d, c, sm, cyberExperimentalWorldProvider);
            d = BorderChunkDirection.South;
            CheckAndAddChunkDirection(d, c, sm, cyberExperimentalWorldProvider);
            d = BorderChunkDirection.West;
            CheckAndAddChunkDirection(d, c, sm, cyberExperimentalWorldProvider);
            d = BorderChunkDirection.NE;
            CheckAndAddChunkDirection(d, c, sm, cyberExperimentalWorldProvider);
            d = BorderChunkDirection.SE;
            CheckAndAddChunkDirection(d, c, sm, cyberExperimentalWorldProvider);
            d = BorderChunkDirection.SW;
            CheckAndAddChunkDirection(d, c, sm, cyberExperimentalWorldProvider);
            d = BorderChunkDirection.NW;
            CheckAndAddChunkDirection(d, c, sm, cyberExperimentalWorldProvider);

            return sm;
        }

        private void CheckAndAddChunkDirection(BorderChunkDirection b, ChunkCoordinates cordz, SmoothingMap sm,
            CyberExperimentalWorldProvider cyberExperimentalWorldProvider)
        {
            var sischunkcords = new ChunkCoordinates(cordz.X + b.GetX(), cordz.Z + b.GetZ());
            var sischunkbiome = BiomeManager.GetBiome(sischunkcords);
            // if (sischunkbiome.LocalId == 10) return;
            sm.AddChunk(cordz + new ChunkCoordinates(b.GetX(), b.GetZ()),
                sischunkbiome.GenerateChunkHeightMap(sischunkcords, cyberExperimentalWorldProvider));
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
            ChunkColumn nc, float[] rth, int[,] mm, bool xtra = true)
        {
            // if (xtra)
            // {
            //     SmoothingMap sm = HandleGeneration(mm, new ChunkCoordinates(nc.X, nc.Z),
            //         cyberExperimentalWorldProvider);
            //     // sm = new SmoothingMap(new ChunkCoordinates(nc.X, nc.Z), mm);
            //     // sm.StripSmooth(4);
            //     sm.SmoothMapV4();
            //     sm.SetChunks(cyberExperimentalWorldProvider,false);
            //     mm = sm.GetChunk(sm.getCenterCords());
            //     ;
            // }

            PopulateChunk(cyberExperimentalWorldProvider, nc, rth, mm);
            nc.SetBlock(7, 140, 7, new RedstoneBlock());
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


        public static float GetNoise(int x, int z, float scale, int mmax)
        {
            var heightnoise = new FastNoise(123123 + 2);
            heightnoise.SetNoiseType(FastNoise.NoiseType.SimplexFractal);
            heightnoise.SetFrequency(scale);
            heightnoise.SetFractalType(FastNoise.FractalType.FBM);
            heightnoise.SetFractalOctaves(2);
            heightnoise.SetFractalLacunarity(2);
            heightnoise.SetFractalGain(.5f);
            return (heightnoise.GetNoise(x, z) + .75f) * (mmax / 2f);
            // return (float) ((OpenNoise.Evaluate(x * scale, z * scale) + 1f) * (max / 2f));
        }

        public static float GetNoiseCubic(int x, int z, float scale, int mmax)
        {
            var heightnoise = new FastNoise(123123 + 2);
            heightnoise.SetNoiseType(FastNoise.NoiseType.CubicFractal);
            heightnoise.SetFrequency(scale);
            heightnoise.SetFractalType(FastNoise.FractalType.FBM);
            heightnoise.SetFractalOctaves(1);
            heightnoise.SetFractalLacunarity(2);
            heightnoise.SetFractalGain(.5f);
            return (heightnoise.GetNoise(x, z) + .75f) * (mmax / 2f);
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

        public virtual AdvancedBiome DoubleCheckCords(ChunkCoordinates chunk)
        {
            return this;
        }
    }
}