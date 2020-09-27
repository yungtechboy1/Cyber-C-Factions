﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using CyberCore.Utils;
using CyberCore.WorldGen.Biomes;
using fNbt;
using log4net;
using MiNET;
using MiNET.BlockEntities;
using MiNET.Blocks;
using MiNET.Items;
using MiNET.Net;
using MiNET.Utils;
using MiNET.Worlds;
using OpenAPI.World;

namespace CyberCore.WorldGen
{
    public class CyberExperimentalWorldProvider : IWorldProvider
    {
        private bool _isInitialized = false;
        private object _initializeSync = new object();

        public void Initialize()
        {
            if (_isInitialized) return; // Quick exit

            lock (_initializeSync)
            {
                if (_isInitialized) return;

                BasePath = BasePath ?? Config.GetProperty("PCWorldFolder", "World").Trim();

                NbtFile file = new NbtFile();
                var levelFileName = Path.Combine(BasePath, "level.dat");
                if (File.Exists(levelFileName))
                {
                    file.LoadFromFile(levelFileName);
                    NbtTag dataTag = file.RootTag["Data"];
                    LevelInfo = new LevelInfo(dataTag);
                }
                else
                {
                    Log.Warn($"No level.dat found at {levelFileName}. Creating empty.");
                    LevelInfo = new LevelInfo();
                }

                switch (Dimension)
                {
                    case Dimension.Overworld:
                        break;
                    case Dimension.Nether:
                        BasePath = Path.Combine(BasePath, @"DIM-1");
                        break;
                    case Dimension.TheEnd:
                        BasePath = Path.Combine(BasePath, @"DIM1");
                        break;
                }

                // MissingChunkProvider?.Initialize();

                _isInitialized = true;
            }
        }

        public bool IsCaching { get; private set; } = true;

        public long GetTime()
        {
            return LevelInfo.Time;
        }

        public long GetDayTime()
        {
            return LevelInfo.DayTime;
        }

        public string GetName()
        {
            return LevelInfo.LevelName;
        }

        public Vector3 GetSpawnPoint()
        {
            var spawnPoint = new Vector3(LevelInfo.SpawnX, LevelInfo.SpawnY + 2 /* + WaterOffsetY*/, LevelInfo.SpawnZ);
            if (Dimension == Dimension.TheEnd)
            {
                spawnPoint = new Vector3(100, 49, 0);
            }
            else if (Dimension == Dimension.Nether)
            {
                spawnPoint = new Vector3(0, 80, 0);
            }

            if (spawnPoint.Y > 256) spawnPoint.Y = 255;

            return spawnPoint;
        }

        private static readonly Random getrandom = new Random();
        private static readonly object syncLock = new object();

        private static readonly OpenSimplexNoise OpenNoise = new OpenSimplexNoise("a-seed".GetHashCode());

        private static readonly ILog Log = LogManager.GetLogger(typeof(CyberExperimentalWorldProvider));

        private static readonly Regex _regex =
            new Regex(@"^((\{""extra"":\[)?)(\{""text"":"".*?""})(],)?(""text"":"".*?""})?$");

        public static readonly Dictionary<int, Tuple<int, Func<int, byte, byte>>> Convert;
        private readonly int Seed;

        public ConcurrentDictionary<ChunkCoordinates, ChunkColumn> _chunkCache =
            new ConcurrentDictionary<ChunkCoordinates, ChunkColumn>();

        private HighPrecisionTimer _tickerHighPrecisionTimer;

        public void Run(object o)
        {
            // Console.WriteLine("RE-ADDER RAN==============22222222===========================");
            // Console.WriteLine($"RE-ADDER RAN==============22222222==========================={_isInitialized} |||| {Level != null}");
            if (Level != null && _isInitialized)
            {
                Log.Info("RE-ADDER RAN=========================================");
                List<String> Completed = new List<string>();
                foreach (var v in new Dictionary<String, List<Block>>(BlocksToBeAddedDuringChunkGeneration))
                {
                    var chunkkey = v.Key;
                    ChunkCoordinates cc = new ChunkCoordinates(int.Parse(chunkkey.Split("|")[0]),
                        int.Parse(chunkkey.Split("|")[1]));
                    var c = GenerateChunkColumn(cc, true);
                    if (c != null)
                    {
                        foreach (var block in BlocksToBeAddedDuringChunkGeneration[v.Key])
                        {
                            var ccc = block.Coordinates;
                            int ax = (ccc.X % 16);
                            if (ax < 0) ax += 16;
                            int az = (ccc.Z % 16);
                            if (az < 0) az += 16;
                            var bb = c.GetBlockObject(ax, ccc.Y, az);
                            var bid = bb.Id;
                            if (bid != new Wood().Id &&
                                bid != new Log().Id || bb.IsTransparent)
                                c.SetBlock(ax, ccc.Y, az, block);
                            // Log.Info($"=================================SETTING BLOCK AT {c} || {ax} {az} || {block.Id}");
                        }

                        // Completed.Add(v.Key);
                        BlocksToBeAddedDuringChunkGeneration[chunkkey].Clear();
                    }
                }

                // foreach (var comp in Completed)
                // {
                //     BlocksToBeAddedDuringChunkGeneration[comp].Clear();
                // }
            }
        }

        private float dirtBaseHeight = 3;
        private float dirtNoise = 0.004f;
        private float dirtNoiseHeight = 10;

        private float stoneBaseHeight = 50;
        private float stoneBaseNoise = 0.05f;
        private float stoneBaseNoiseHeight = 20;
        private float stoneMinHeight = 20;
        private float stoneMountainFrequency = 0.008f;

        private float stoneMountainHeight = 48;
        private int waterLevel = 25;

        public CyberExperimentalWorldProvider(int seed, string levelDirectory)
        {
            Seed = seed;
            instance = this;
            BasePath = levelDirectory.Trim();
            Log.Info("This Experimental World Generator Was Created by @Yungtechboy1");
            _tickerHighPrecisionTimer = new HighPrecisionTimer(50 * 10, Run, false, false);
        }

        public string BasePath { get; set; }
        public OpenLevel Level { get; set; }

        public LevelInfo LevelInfo { get; private set; }

        public Dimension Dimension { get; set; }


        public Queue<Block> LightSources { get; set; } = new Queue<Block>();
        public bool ReadSkyLight { get; set; } = true;

        public bool ReadBlockLight { get; set; } = true;


        public float[] getChunkRTH(ChunkColumn chunk)
        {
            return getChunkRTH(chunk.X, chunk.Z);
        }

        public float[] getChunkRTH(int x, int z)
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
            //CALCULATE HEIGHT
            // var heightnoise = new FastNoise(123123 + 2);
            // heightnoise.SetNoiseType(FastNoise.NoiseType.SimplexFractal);
            // heightnoise.SetFrequency(.015f);
            // heightnoise.SetFractalType(FastNoise.FractalType.FBM);
            // heightnoise.SetFractalOctaves(1);
            // heightnoise.SetFractalLacunarity(.25f);
            // heightnoise.SetFractalGain(1);


            var rain = rainnoise.GetNoise(x, z) + 1;
            var temp = tempnoise.GetNoise(x, z) + 1;
            var height = GetNoise(x, z, 0.015f, 2);
            return new[] {rain, temp, height};
        }


        public ChunkColumn GenerateChunkColumn(ChunkCoordinates chunkCoordinates, bool cacheOnly = false)
        {
            return OpenPreGenerateChunkColumn(chunkCoordinates, true, cacheOnly);
        }


        public ChunkColumn OpenPreGenerateChunkColumn(ChunkCoordinates chunkCoordinates, bool smooth = true,
            bool cacheOnly = false)
        {
            ChunkColumn v = OpenGenerateChunkColumn(chunkCoordinates, smooth, cacheOnly).Result;
            if (v == null) return null;
            _chunkCache[chunkCoordinates] = v;
            return v;
        }

        public int getBlockHeight(int x, int z)
        {
            int cx = x >> 4;
            int cz = z >> 4;
            int tx = x % 16;
            int tz = z % 16;
            if (tx < 0) tx += 16;
            if (tz < 0) tz += 16;
            // Console.WriteLine($" GETTING BLOCK HEIGHT CHUNKCORDZ: {cx} {cz} || {tx} {tz} || FROM {x} {z}");
            AdvancedBiome tb = BiomeManager.GetBiome(new ChunkCoordinates(cx, cz));
            var c = GenerateChunkColumn(new ChunkCoordinates(cx, cz), true);
            if (c != null)
            {
                // Console.WriteLine($"{tx} ||| {tz}");
                // Console.WriteLine($"DA CHUNK AT {cx} {cz} WAS ALREADY GENERATED AND AT {x} {z} >> {tx} {tz} >> HAS {c.GetHeight(tx, tz)}^^^ ");
                return c.GetHeight(tx, tz);
            }

            return tb.GetSh(tx, tz, cx, cz);
        }

        public static void AddBlocksToBeAddedDuringChunkGeneration(ChunkCoordinates chunkCoordinates,
            List<Block> blocks)
        {
            var c = chunkCoordinates.X + "|" + chunkCoordinates.Z;
            if (!BlocksToBeAddedDuringChunkGeneration.ContainsKey(c))
                BlocksToBeAddedDuringChunkGeneration[c] = new List<Block>();

            foreach (var block in blocks)
            {
                CyberExperimentalWorldProvider
                    .BlocksToBeAddedDuringChunkGeneration[c].Add(block);
            }
        }

        public static void AddBlocksToBeAddedDuringChunkGeneration(String c, List<Block> blocks)
        {
            if (!BlocksToBeAddedDuringChunkGeneration.ContainsKey(c))
                BlocksToBeAddedDuringChunkGeneration[c] = new List<Block>();

            foreach (var block in blocks)
            {
                CyberExperimentalWorldProvider
                    .BlocksToBeAddedDuringChunkGeneration[c].Add(block);
            }
        }

        /// <summary>
        /// Key is X|Z
        /// </summary>
        /// <param name="c"></param>
        /// <param name="block"></param>
        public static void AddBlockToBeAddedDuringChunkGeneration(String c, Block block)
        {
            if (!BlocksToBeAddedDuringChunkGeneration.ContainsKey(c))
                BlocksToBeAddedDuringChunkGeneration[c] = new List<Block>();

            CyberExperimentalWorldProvider
                .BlocksToBeAddedDuringChunkGeneration[c].Add(block);
        }

        public static void AddBlockToBeAddedDuringChunkGeneration(ChunkCoordinates chunkCoordinates, Block block)
        {
            var c = chunkCoordinates.X + "|" + chunkCoordinates.Z;
            if (!BlocksToBeAddedDuringChunkGeneration.ContainsKey(c))
                BlocksToBeAddedDuringChunkGeneration[c] = new List<Block>();

            CyberExperimentalWorldProvider
                .BlocksToBeAddedDuringChunkGeneration[c].Add(block);
        }

        /// <summary>
        /// 
        /// </summary>
        public static Dictionary<String, List<Block>> BlocksToBeAddedDuringChunkGeneration =
            new Dictionary<String, List<Block>>();

        public async Task<ChunkColumn> OpenGenerateChunkColumn(ChunkCoordinates chunkCoordinates, bool smooth = true,
            bool cacheOnly = false)
        {
            var s = new Stopwatch();
            s.Restart();
            ChunkColumn cachedChunk;
            if (_chunkCache.TryGetValue(chunkCoordinates, out cachedChunk)) return cachedChunk;

            ChunkColumn chunk;
            if (cacheOnly) return null;
            chunk = await PreGetChunk(chunkCoordinates, BasePath);
            if (chunk != null)
            {
                _chunkCache[chunkCoordinates] = chunk;
                return chunk;
            }


            chunk = new ChunkColumn();
            chunk.X = chunkCoordinates.X;
            chunk.Z = chunkCoordinates.Z;
            var rth = BiomeManager.getChunkRTH(chunkCoordinates);

            // Console.WriteLine("STARTING POPULATIOaN");
            var b = BiomeManager.GetBiome(chunkCoordinates);
            chunk = PopulateChunk(this, chunk, rth, b).Result;

            // if (smooth)
            // {
            //     chunk = SmoothChunk(this, chunk, rth, b).Result;
            // }

            // else if(b.BorderChunk)
            // {
            //     Console.WriteLine($"{chunkCoordinates} WAS NOT SMOOTH BUT WAS BORDER CHUNK");
            // }
            chunk = PreGenerateSurfaceItems(this, chunk, null).Result;
            // StackTrace stackTrace = new StackTrace(); 
// Get calling method name
            // Console.WriteLine(stackTrace.GetFrame(1).GetMethod().ReflectedType.Namespace+"."+stackTrace.GetFrame(1).GetMethod().ReflectedType.Name+"."+stackTrace.GetFrame(1).GetMethod().Name);
            // Console.WriteLine(stackTrace.GetFrame(2).GetMethod().ReflectedType.Namespace+"."+stackTrace.GetFrame(2).GetMethod().ReflectedType.Name+"."+stackTrace.GetFrame(2).GetMethod().Name);
            // Console.WriteLine(stackTrace.GetFrame(3).GetMethod().ReflectedType.Namespace+"."+stackTrace.GetFrame(3).GetMethod().ReflectedType.Name+"."+stackTrace.GetFrame(3).GetMethod().Name);
            // Console.WriteLine(stackTrace.GetFrame(4).GetMethod().ReflectedType.Namespace+"."+stackTrace.GetFrame(4).GetMethod().ReflectedType.Name+"."+stackTrace.GetFrame(4).GetMethod().Name);
            // Console.WriteLine(stackTrace.GetFrame(5).GetMethod().ReflectedType.Namespace+"."+stackTrace.GetFrame(5).GetMethod().ReflectedType.Name+"."+stackTrace.GetFrame(5).GetMethod().Name);
            // Console.WriteLine(stackTrace.GetFrame(6).GetMethod().ReflectedType.Namespace+"."+stackTrace.GetFrame(6).GetMethod().ReflectedType.Name+"."+stackTrace.GetFrame(6).GetMethod().Name);
            // Console.WriteLine(stackTrace.GetFrame(7).GetMethod().ReflectedType.Namespace+"."+stackTrace.GetFrame(7).GetMethod().ReflectedType.Name+"."+stackTrace.GetFrame(7).GetMethod().Name);
            // Console.WriteLine(stackTrace.GetFrame(8).GetMethod().ReflectedType.Namespace+"."+stackTrace.GetFrame(8).GetMethod().ReflectedType.Name+"."+stackTrace.GetFrame(8).GetMethod().Name);
            // Console.WriteLine(stackTrace.GetFrame(9).GetMethod().ReflectedType.Namespace+"."+stackTrace.GetFrame(9).GetMethod().ReflectedType.Name+"."+stackTrace.GetFrame(9).GetMethod().Name);
            // Console.WriteLine(stackTrace.GetFrame(10).GetMethod().ReflectedType.Namespace+"."+stackTrace.GetFrame(10).GetMethod().ReflectedType.Name+"."+stackTrace.GetFrame(10).GetMethod().Name);
            // Console.WriteLine(stackTrace.GetFrame(11).GetMethod().ReflectedType.Namespace+"."+stackTrace.GetFrame(11).GetMethod().ReflectedType.Name+"."+stackTrace.GetFrame(11).GetMethod().Name);
            // Console.WriteLine(stackTrace.GetFrame(12).GetMethod().ReflectedType.Namespace+"."+stackTrace.GetFrame(12).GetMethod().ReflectedType.Name+"."+stackTrace.GetFrame(12).GetMethod().Name);
            Console.WriteLine("GENERATING CHUNK TOOK "+s.Elapsed);
            return chunk;
        }


        public static void CleanSignText(NbtCompound blockEntityTag, string tagName)
        {
            var text = blockEntityTag[tagName].StringValue;
            var replace = /*Regex.Unescape*/_regex.Replace(text, "$3");
            blockEntityTag[tagName] = new NbtString(tagName, replace);
        }

        public void SaveLevelInfo(LevelInfo level)
        {
            if (Dimension != Dimension.Overworld) return;

            var leveldat = Path.Combine(BasePath, "level.dat");

            if (!Directory.Exists(BasePath))
                Directory.CreateDirectory(BasePath);
            else if (File.Exists(leveldat))
                return; // What if this is changed? Need a dirty flag on this

            if (LevelInfo.SpawnY <= 0) LevelInfo.SpawnY = 256;

            var file = new NbtFile();
            NbtTag dataTag = new NbtCompound("Data");
            var rootTag = (NbtCompound) file.RootTag;
            rootTag.Add(dataTag);
            level.SaveToNbt(dataTag);
            file.SaveToFile(leveldat, NbtCompression.GZip);
        }

        public int SaveChunks()
        {
            int a = 0;
            // return 0;
            var count = 0;
            try
            {
                lock (_chunkCache)
                {
                    Console.WriteLine(a++);
                    SaveLevelInfo(new LevelInfo());

                    Console.WriteLine(a++);
                    var regions = new Dictionary<Tuple<int, int>, List<ChunkColumn>>();
                    foreach (var chunkColumn in _chunkCache.OrderBy(pair => pair.Key.X >> 5)
                        .ThenBy(pair => pair.Key.Z >> 5))
                    {
                        var regionKey = new Tuple<int, int>(chunkColumn.Key.X >> 5, chunkColumn.Key.Z >> 5);
                        if (!regions.ContainsKey(regionKey)) regions.Add(regionKey, new List<ChunkColumn>());

                        regions[regionKey].Add(chunkColumn.Value);
                    }

                    Console.WriteLine(a++);
                    var tasks = new List<Task>();
                    foreach (var region in regions.OrderBy(pair => pair.Key.Item1).ThenBy(pair => pair.Key.Item2))
                    {
                        var task = new Task(delegate
                        {
                            var chunks = region.Value;
                            foreach (ChunkColumn chunkColumn in chunks)
                                if (chunkColumn != null && chunkColumn.NeedSave)
                                {
                                    SaveChunk(chunkColumn, BasePath);
                                    count++;
                                }
                        });
                        task.Start();
                        tasks.Add(task);
                    }

                    Console.WriteLine(a++);
                    Task.WaitAll(tasks.ToArray());

                    Console.WriteLine(a++);
                    //foreach (var chunkColumn in _chunkCache.OrderBy(pair => pair.Key.X >> 5).ThenBy(pair => pair.Key.Z >> 5))
                    //{
                    //	if (chunkColumn.Value != null && chunkColumn.Value.NeedSave)
                    //	{
                    //		SaveChunk(chunkColumn.Value, BasePath);
                    //		count++;
                    //	}
                    //}
                }
            }
            catch (Exception e)
            {
                Log.Error("saving 0 chunks", e);
            }

            return count;
        }

        static object readwrite = new object();

        public static ConcurrentDictionary<String, bool> RW = new ConcurrentDictionary<string, bool>();

        public static void SaveChunk(ChunkColumn chunk, string basePath)
        {
            // WARNING: This method does not consider growing size of the chunks. Needs refactoring to find
            // free sectors and clear up old ones. It works fine as long as no dynamic data is written
            // like block entity data (signs etc).

            var time = Stopwatch.StartNew();

            chunk.NeedSave = false;

            var coordinates = new ChunkCoordinates(chunk.X, chunk.Z);

            var width = 32;
            var depth = 32;

            var rx = coordinates.X >> 5;
            var rz = coordinates.Z >> 5;

            var filePath = Path.Combine(basePath,
                string.Format(@"region{2}r.{0}.{1}.mca", rx, rz, Path.DirectorySeparatorChar));

            Log.Debug($"Save chunk X={chunk.X}, Z={chunk.Z} to {filePath}");
            // lock (readwrite) CAUSE OF ALL MY ISSUES
            while (RW.ContainsKey(filePath))
            {
                Thread.Sleep(100);
            }

            RW[filePath] = true;

            if (!File.Exists(filePath))
            {
                // Make sure directory exist
                Directory.CreateDirectory(Path.Combine(basePath, "region"));

                // Create empty region file
                using (var regionFile = File.Open(filePath, FileMode.CreateNew))
                {
                    var buffer = new byte[8192];
                    regionFile.Write(buffer, 0, buffer.Length);
                }
            }

            var testTime = new Stopwatch();

            using (var regionFile = File.Open(filePath, FileMode.Open))
            {
                // Region files begin with an 8kiB header containing information about which chunks are present in the region file, 
                // when they were last updated, and where they can be found.
                var buffer = new byte[8192];
                regionFile.Read(buffer, 0, buffer.Length);

                var xi = coordinates.X % width;
                if (xi < 0) xi += 32;
                var zi = coordinates.Z % depth;
                if (zi < 0) zi += 32;
                var tableOffset = (xi + zi * width) * 4;

                regionFile.Seek(tableOffset, SeekOrigin.Begin);

                // Location information for a chunk consists of four bytes split into two fields: the first three bytes are a(big - endian) offset in 4KiB sectors 
                // from the start of the file, and a remaining byte which gives the length of the chunk(also in 4KiB sectors, rounded up).
                var offsetBuffer = new byte[4];
                regionFile.Read(offsetBuffer, 0, 3);
                Array.Reverse(offsetBuffer);
                var offset = BitConverter.ToInt32(offsetBuffer, 0) << 4;
                var sectorCount = (byte) regionFile.ReadByte();

                testTime.Restart(); // RESTART

                // Seriaize NBT to get lenght
                var nbt = CreateNbtFromChunkColumn(chunk);

                testTime.Stop();

                var nbtBuf = nbt.SaveToBuffer(NbtCompression.ZLib);
                var nbtLength = nbtBuf.Length;
                var nbtSectorCount = (byte) Math.Ceiling(nbtLength / 4096d);

                // Don't write yet, just use the lenght

                if (offset == 0 || sectorCount == 0 || nbtSectorCount > sectorCount)
                {
                    if (Log.IsDebugEnabled)
                        if (sectorCount != 0)
                            Log.Warn(
                                $"Creating new sectors for this chunk even tho it existed. Old sector count={sectorCount}, new sector count={nbtSectorCount} (lenght={nbtLength})");

                    regionFile.Seek(0, SeekOrigin.End);
                    offset = (int) ((int) regionFile.Position & 0xfffffff0);

                    regionFile.Seek(tableOffset, SeekOrigin.Begin);

                    var bytes = BitConverter.GetBytes(offset >> 4);
                    Array.Reverse(bytes);
                    regionFile.Write(bytes, 0, 3);
                    regionFile.WriteByte(nbtSectorCount);
                }

                var lenghtBytes = BitConverter.GetBytes(nbtLength + 1);
                Array.Reverse(lenghtBytes);

                regionFile.Seek(offset, SeekOrigin.Begin);
                regionFile.Write(lenghtBytes, 0, 4); // Lenght
                regionFile.WriteByte(0x02); // Compression mode zlib

                regionFile.Write(nbtBuf, 0, nbtBuf.Length);

                int reminder;
                Math.DivRem(nbtLength + 4, 4096, out reminder);

                var padding = new byte[4096 - reminder];
                if (padding.Length > 0) regionFile.Write(padding, 0, padding.Length);

                testTime.Stop(); // STOP

                Log.Warn(
                    $"Took {time.Elapsed} {time.ElapsedMilliseconds}ms to save. And {testTime.Elapsed}ms to generate bytes from NBT");
            }

            bool a = false;
            RW.TryRemove(filePath, out a);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sectionTag"></param>
        /// <param name="chunkColumn"></param>
        /// <param name="convertBid"></param>
        public void ReadSection(NbtTag sectionTag, ChunkColumn chunkColumn, bool convertBid = true)
        {
            int sectionIndex = sectionTag["Y"].ByteValue;
            var blocks = sectionTag["Blocks"].ByteArrayValue;
            var data = sectionTag["Data"].ByteArrayValue;
            var addTag = sectionTag["Add"];
            var adddata = new byte[2048];
            // sectionTag.Add(new NbtByteArray("RuntimeID", runtimeid));
            // sectionTag.Add(new NbtByteArray("RuntimeID2", runtimeid2));
            if (addTag != null) adddata = addTag.ByteArrayValue;
            var ridtag = sectionTag["RuntimeID"];
            var riddata = new byte[2048];
            if (ridtag != null) riddata = ridtag.ByteArrayValue;
            var ridtag2 = sectionTag["RuntimeID2"];
            var riddata2 = new byte[2048];
            if (ridtag2 != null) riddata2 = ridtag2.ByteArrayValue;
            var blockLight = sectionTag["BlockLight"].ByteArrayValue;
            var skyLight = sectionTag["SkyLight"].ByteArrayValue;

            var subChunk = chunkColumn[sectionIndex];
            int fail = 16 * 16 * 16;
            int fc = 0;
            for (var x = 0; x < 16; x++)
            for (var z = 0; z < 16; z++)
            for (var y = 0; y < 16; y++)
            {
                var yi = (sectionIndex << 4) + y;

                var anvilIndex = (y << 8) + (z << 4) + x;
                var blockId = blocks[anvilIndex] + (Nibble4(adddata, anvilIndex) << 8);

                // Anvil to PE friendly converstion

                int aablockId = blockId;
                // Func<int, byte, byte> dataConverter = (i, b) => b; // Default no-op converter
                // Log.Info($"LETS SEE IF WE NEED TO CONVERT {convertBid} && {Convert.ContainsKey(blockId)}  {blockId} ");
                // if (convertBid && Convert.ContainsKey(blockId))
                // {
                //     dataConverter = Convert[blockId].Item2;
                //     blockId = Convert[blockId].Item1;
                //     Log.Info($"Convertiing FROM {aablockId} TO  {dataConverter} || {blockId} ");
                // }
                //else
                //{
                //	if (BlockFactory.GetBlockById((byte)blockId).GetType() == typeof(Block))
                //	{
                //		Log.Warn($"No block implemented for block ID={blockId}, Meta={data}");
                //		//blockId = 57;
                //	}
                //}

                chunkColumn.IsAllAir &= blockId == 0;
                // if (blockId > 255)
                // {
                //     Log.Warn($"Failed mapping for block ID={blockId}, Meta={data}");
                //     blockId = 41;
                // }

                if (yi == 0 && (blockId == 8 || blockId == 9)) blockId = 7; // Bedrock under water

                var metadata = Nibble4(data, anvilIndex);
                int ablockId = blockId;
                byte ametadata = metadata;
                // metadata = dataConverter(blockId, metadata);
                var runtimeId = -1;

                if (ridtag != null && ridtag2 != null)
                {
                    var r1 = Nibble4(riddata, anvilIndex);
                    var r2 = Nibble4(riddata2, anvilIndex);
                    var r = r1 + (r2 << 8);
                    if (r < BlockFactory.BlockPalette.Count)
                    {
                        var bbb = BlockFactory.BlockPalette[r];
                        if (bbb.Id == blockId)
                        {
                            runtimeId = r;
                        }
                    }
                }

                if (runtimeId == -1) runtimeId = (int) BlockFactory.GetRuntimeId(blockId, metadata);

                if (runtimeId == -1)
                {
                    // if (blockId == 211)
                    // {
                    //     runtimeId = (int) BlockFactory.GetRuntimeId(new Log().Id, metadata);
                    // }
                    // else
                    runtimeId = (int) BlockFactory.GetRuntimeId(0, 0);

                    Console.WriteLine($"EEEEEEEEEEEEEEEE22222222223333333333333333222222EEEE {blockId} {metadata}");
                    // Console.WriteLine($"EEEEEEEEEEEEEEEE22222222223333sss```````````````333333333333222222EEEE {ablockId} {ametadata} ||  {aablockId}");
                    // fc++;
                }

                subChunk.SetBlockByRuntimeId(x, y, z, runtimeId);

                if (ReadBlockLight) subChunk.SetBlocklight(x, y, z, Nibble4(blockLight, anvilIndex));

                if (ReadSkyLight)
                    subChunk.SetSkylight(x, y, z, Nibble4(skyLight, anvilIndex));
                else
                    subChunk.SetSkylight(x, y, z, 0);

                if (blockId == 0) continue;

                if (convertBid && blockId == 3 && metadata == 2)
                {
                    // Dirt Podzol => (Podzol)
                    subChunk.SetBlock(x, y, z, new Podzol());
                    blockId = 243;
                }

                if (BlockFactory.LuminousBlocks[blockId] != 0)
                {
                    var block = BlockFactory.GetBlockById(subChunk.GetBlockId(x, y, z));
                    block.Coordinates = new BlockCoordinates(x + (chunkColumn.X << 4), yi, z + (chunkColumn.Z << 4));
                    subChunk.SetBlocklight(x, y, z, (byte) block.LightLevel);
                    lock (LightSources)
                    {
                        LightSources.Enqueue(block);
                    }
                }
            }
        }


        public static NbtFile CreateNbtFromChunkColumn(ChunkColumn chunk)
        {
            var nbt = new NbtFile();

            var levelTag = new NbtCompound("Level");
            var rootTag = (NbtCompound) nbt.RootTag;
            rootTag.Add(levelTag);

            levelTag.Add(new NbtByte("TerrainPopulated", 1));

            levelTag.Add(new NbtInt("xPos", chunk.X));
            levelTag.Add(new NbtInt("zPos", chunk.Z));
            levelTag.Add(new NbtLong("LastUpdate", 0));
            levelTag.Add(new NbtByteArray("Biomes", chunk.biomeId));

            var sectionsTag = new NbtList("Sections", NbtTagType.Compound);
            levelTag.Add(sectionsTag);

            for (var i = 0; i < 16; i++)
            {
                var subChunk = chunk[i];
                if (subChunk == null)
                {
                    Log.Debug($"Chunk was null? I value: {i} {chunk.X} {chunk.Z}");
                    continue;
                }
                else if (subChunk.IsAllAir())
                {
                    if (i == 0) Log.Debug($"All air bottom chunk? {subChunk.GetBlockId(0, 0, 0)}");
                    continue;
                }

                var sectionTag = new NbtCompound();
                sectionsTag.Add(sectionTag);
                sectionTag.Add(new NbtByte("Y", (byte) i));


                // Stopwatch ss = new Stopwatch();
                // ss.Restart();
                var blocks = new byte[4096];
                var add = new byte[2048];
                var data = new byte[2048];
                var blockLight = new byte[2048];
                var skyLight = new byte[2048];
                var runtimeid = new byte[2048];
                var runtimeid2 = new byte[2048];


                for (var x = 0; x < 16; x++)
                for (var z = 0; z < 16; z++)
                for (var y = 0; y < 16; y++)
                {
                    byte addd = 0;
                    var anvilIndex = y * 16 * 16 + z * 16 + x;
                    // var a = subChunk.GetBlockId()
                    var b = subChunk.GetBlockObject(x, y, z);
                    int bid = b.Id;
                    byte bmeta = b.Metadata;
                    if (bid > 255)
                    {
                        addd = (byte) (bid >> 8);
                        bid -= 256;
                    }

                    // if(blockId == 211 || blockId == 17)Console.WriteLine($"THE BLOCK ID OF THIS IS {blockId}");
                    // if(blockId == 211 || blockId == 17)Console.WriteLine($"THE BLOCK ID OF THIS IS {subChunk.GetBlockObject(x,y,z).GetRuntimeId()} || {subChunk.GetBlockObject(x,y,z).Name}");
                    // if(blockId == 211 || blockId == 17)Console.WriteLine($"THE BLOCK ID OF THIS IS {blockId} {subChunk.GetBlockObject(x,y,z).Id} {subChunk.GetBlockObject(x,y,z).Metadata}");


                    // if (blockId == 211 || blockId == 17)
                    //     Console.WriteLine(
                    //         $"THE BLOCK ID OF THIS IS {blockId} {subChunk.GetBlockObject(x, y, z).Id} {subChunk.GetBlockObject(x, y, z).Metadata}");
                    // if (blockId == 211 || blockId == 17)
                    //     Console.WriteLine(
                    //         $" >> {blockId} {b.Id} {b.Metadata} {b.GetState().Data} || {bid} {bmeta} {addd} |||| {b.Id}");
                    // if(blockId == 211 || blockId == 17)Console.WriteLine($" >> |||||||| {(bid >> 8)}");

                    blocks[anvilIndex] = (byte) bid;
                    // var brid = b.GetRuntimeId();
                    // SetNibble4(runtimeid, anvilIndex, (byte) (brid & 255));
                    // SetNibble4(runtimeid2, anvilIndex, (byte) (brid >> 8));
                    SetNibble4(data, anvilIndex, bmeta);
                    SetNibble4(add, anvilIndex, addd);
                    SetNibble4(blockLight, anvilIndex, subChunk.GetBlocklight(x, y, z));
                    SetNibble4(skyLight, anvilIndex, subChunk.GetSkylight(x, y, z));
                }

                // Console.WriteLine($" THOOK32 ::: {ss.Elapsed}");
                // ss.Restart();
                sectionTag.Add(new NbtByteArray("Blocks", blocks));
                sectionTag.Add(new NbtByteArray("Data", data));
                sectionTag.Add(new NbtByteArray("Add", add));
                sectionTag.Add(new NbtByteArray("BlockLight", blockLight));
                sectionTag.Add(new NbtByteArray("SkyLight", skyLight));
                sectionTag.Add(new NbtByteArray("RuntimeID", runtimeid));
                sectionTag.Add(new NbtByteArray("RuntimeID2", runtimeid2));
                // Console.WriteLine($" THOOK32 ::: {ss.Elapsed}");
                // ss.Stop();
            }

            var heights = new int[256];
            for (var h = 0; h < heights.Length; h++) heights[h] = chunk.height[h];
            levelTag.Add(new NbtIntArray("HeightMap", heights));

            // TODO: Save entities
            var entitiesTag = new NbtList("Entities", NbtTagType.Compound);
            levelTag.Add(entitiesTag);

            var blockEntitiesTag = new NbtList("TileEntities", NbtTagType.Compound);
            int n = 0;
            foreach (var blockEntityNbt in chunk.BlockEntities.Values)
            {
                // Console.WriteLine($"Saving BlockEntity {n} NAME:{blockEntityNbt.Names}");
                // if (blockEntityNbt.Contains("Items"))
                // {
                //     foreach (var v in (NbtList) blockEntityNbt["Items"])
                //     {
                //         if (((NbtShort) v["id"]).Value == 0)
                //         {
                //             Console.WriteLine($"{n}AIR|");
                //         }
                //         else
                //             Console.WriteLine($"#{n} ||||| {v["slot"]} = {v["id"]} | {v["Damage"]} | {v["Count"]}");
                //
                //         n++;
                //     }
                // }
                //
                // Console.WriteLine("+==================================+");

                var nbtClone = (NbtCompound) blockEntityNbt.Clone();
                nbtClone.Name = null;
                blockEntitiesTag.Add(nbtClone);
                n++;
            }

            //CHEST DEBUG
            // Console.WriteLine(chunk.BlockEntities.Count + " <<<<<<<<<<<<<<<< DA BE SIZE");

            levelTag.Add(blockEntitiesTag);

            levelTag.Add(new NbtList("TileTicks", NbtTagType.Compound));

            levelTag.Add(new NbtByte("MCPE BID", 1)); // Indicate that the chunks contain PE block ID's.

            return nbt;
        }


        /// <summary>Try to get chunk from File Task
        /// </summary>
        /// <param name="coordinates"></param>
        /// <param name="basePath"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<ChunkColumn> PreGetChunk(ChunkCoordinates coordinates, string basePath)
        {
            return await GetChunk(coordinates, basePath);
        }

        public async Task<ChunkColumn> GetChunk(ChunkCoordinates coordinates, string basePath)
        {
            try
            {
                var width = 32;
                var depth = 32;

                var rx = coordinates.X >> 5;
                var rz = coordinates.Z >> 5;

                var filePath = Path.Combine(basePath,
                    string.Format(@"region{2}r.{0}.{1}.mca", rx, rz, Path.DirectorySeparatorChar));

                while (RW.ContainsKey(filePath))
                {
                    Thread.Sleep(10);
                }

                RW[filePath] = true;

                if (!File.Exists(filePath))
                {
                    bool aaaaaaa = false;
                    RW.TryRemove(filePath, out aaaaaaa);
                    return null;
                }

                using (var regionFile = File.OpenRead(filePath))
                {
                    var buffer = new byte[8192];

                    regionFile.Read(buffer, 0, 8192);

                    var xi = coordinates.X % width;
                    if (xi < 0) xi += 32;
                    var zi = coordinates.Z % depth;
                    if (zi < 0) zi += 32;
                    var tableOffset = (xi + zi * width) * 4;

                    regionFile.Seek(tableOffset, SeekOrigin.Begin);

                    var offsetBuffer = new byte[4];
                    regionFile.Read(offsetBuffer, 0, 3);
                    Array.Reverse(offsetBuffer);
                    var offset = BitConverter.ToInt32(offsetBuffer, 0) << 4;

                    var bytes = BitConverter.GetBytes(offset >> 4);
                    Array.Reverse(bytes);
                    if (offset != 0 && offsetBuffer[0] != bytes[0] && offsetBuffer[1] != bytes[1] &&
                        offsetBuffer[2] != bytes[2])
                        throw new Exception(
                            $"Not the same buffer\n{Packet.HexDump(offsetBuffer)}\n{Packet.HexDump(bytes)}");

                    var length = regionFile.ReadByte();

                    if (offset == 0 || length == 0)
                    {
                        bool aaaaaa = false;
                        RW.TryRemove(filePath, out aaaaaa);
                        return null;
                    }

                    regionFile.Seek(offset, SeekOrigin.Begin);
                    var waste = new byte[4];
                    regionFile.Read(waste, 0, 4);
                    var compressionMode = regionFile.ReadByte();

                    if (compressionMode != 0x02)
                        throw new Exception(
                            $"CX={coordinates.X}, CZ={coordinates.Z}, NBT wrong compression. Expected 0x02, got 0x{compressionMode:X2}. " +
                            $"Offset={offset}, length={length}\n{Packet.HexDump(waste)}");

                    var nbt = new NbtFile();
                    nbt.LoadFromStream(regionFile, NbtCompression.ZLib);

                    var dataTag = (NbtCompound) nbt.RootTag["Level"];

                    var isPocketEdition = false;
                    if (dataTag.Contains("MCPE BID")) isPocketEdition = dataTag["MCPE BID"].ByteValue == 1;

                    var sections = dataTag["Sections"] as NbtList;

                    var chunk = new ChunkColumn
                    {
                        X = coordinates.X,
                        Z = coordinates.Z,
                        biomeId = dataTag["Biomes"].ByteArrayValue,
                        IsAllAir = true
                    };

                    if (chunk.biomeId.Length > 256) throw new Exception();

                    NbtTag heights = dataTag["HeightMap"] as NbtIntArray;
                    if (heights != null)
                    {
                        var intHeights = heights.IntArrayValue;
                        for (var i = 0; i < 256; i++) chunk.height[i] = (short) intHeights[i];
                    }

                    // This will turn into a full chunk column
                    foreach (NbtTag sectionTag in sections)
                        ReadSection(sectionTag, chunk, /*!isPocketEdition*/false);

                    var entities = dataTag["Entities"] as NbtList;

                    var blockEntities = dataTag["TileEntities"] as NbtList;
                    if (blockEntities != null)
                        foreach (var nbtTag in blockEntities)
                        {
                            var blockEntityTag = (NbtCompound) nbtTag.Clone();
                            var entityId = blockEntityTag["id"].StringValue;
                            var x = blockEntityTag["x"].IntValue;
                            var y = blockEntityTag["y"].IntValue;
                            var z = blockEntityTag["z"].IntValue;

                            if (entityId.StartsWith("minecraft:"))
                            {
                                var id = entityId.Split(':')[1];

                                entityId = id.First().ToString().ToUpper() + id.Substring(1);
                                if (entityId == "Flower_pot") entityId = "FlowerPot";
                                else if (entityId == "Shulker_box") entityId = "ShulkerBox";

                                blockEntityTag["id"] = new NbtString("id", entityId);
                            }

                            var blockEntity = BlockEntityFactory.GetBlockEntityById(entityId);

                            if (blockEntity != null)
                            {
                                blockEntityTag.Name = string.Empty;
                                blockEntity.Coordinates = new BlockCoordinates(x, y, z);

                                if (blockEntity is SignBlockEntity)
                                {
                                    if (Log.IsDebugEnabled)
                                        Log.Debug($"Loaded sign block entity\n{blockEntityTag}");
                                    // Remove the JSON stuff and get the text out of extra data.
                                    // TAG_String("Text2"): "{"extra":["10c a loaf!"],"text":""}"
                                    CleanSignText(blockEntityTag, "Text1");
                                    CleanSignText(blockEntityTag, "Text2");
                                    CleanSignText(blockEntityTag, "Text3");
                                    CleanSignText(blockEntityTag, "Text4");
                                }
                                else if (blockEntity is ChestBlockEntity || blockEntity is ShulkerBoxBlockEntity)
                                {
                                    Console.WriteLine("YEAHHHH CHESTBLOCK IS HERE");
                                    if (blockEntity is ShulkerBoxBlockEntity)
                                    {
                                        //var meta = chunk.GetMetadata(x & 0x0f, y, z & 0x0f);

                                        //blockEntityTag["facing"] = new NbtByte("facing", (byte) (meta >> 4));

                                        //chunk.SetBlock(x & 0x0f, y, z & 0x0f, 218,(byte) (meta - ((byte) (meta >> 4) << 4)));
                                    }

                                    var items = (NbtList) blockEntityTag["Items"];

                                    if (items != null)
                                    {
                                        Console.WriteLine($"YEAHHHH CHESTBLOCK Items is not Null > {items.Count}");
                                        for (byte i = 0; i < items.Count; i++)
                                        {
                                            var item = (NbtCompound) items[i];

                                            var itemName = item["id"].StringValue;
                                            // Console.WriteLine($"YEAHHHH CHESTBLOCK ITEM {i} IS {itemName}");
                                            if (itemName != "0")
                                            {
                                            }

                                            if (itemName.StartsWith("minecraft:"))
                                            {
                                                var id = itemName.Split(':')[1];

                                                itemName = id.First().ToString().ToUpper() + id.Substring(1);
                                                var itemId = ItemFactory.GetItemIdByName(itemName);

                                                Console.WriteLine($"YEAHHHH CHESTBLOCK ITEM NEW IS {itemId}");
                                                item.Remove("id");
                                                item.Add(new NbtShort("id", itemId));
                                            }
                                        }
                                    }
                                }
                                else if (blockEntity is BedBlockEntity)
                                {
                                    var color = blockEntityTag["color"];
                                    blockEntityTag.Remove("color");
                                    blockEntityTag.Add(color is NbtByte
                                        ? color
                                        : new NbtByte("color", (byte) color.IntValue));
                                }
                                else if (blockEntity is FlowerPotBlockEntity)
                                {
                                    var itemName = blockEntityTag["Item"].StringValue;
                                    if (itemName.StartsWith("minecraft:"))
                                    {
                                        var id = itemName.Split(':')[1];

                                        itemName = id.First().ToString().ToUpper() + id.Substring(1);
                                    }

                                    var itemId = ItemFactory.GetItemIdByName(itemName);
                                    blockEntityTag.Remove("Item");
                                    blockEntityTag.Add(new NbtShort("item", itemId));

                                    var data = blockEntityTag["Data"].IntValue;
                                    blockEntityTag.Remove("Data");
                                    blockEntityTag.Add(new NbtInt("mData", data));
                                }
                                else
                                {
                                    if (Log.IsDebugEnabled) Log.Debug($"Loaded block entity\n{blockEntityTag}");
                                    blockEntity.SetCompound(blockEntityTag);
                                    blockEntityTag = blockEntity.GetCompound();
                                }

                                chunk.SetBlockEntity(new BlockCoordinates(x, y, z), blockEntityTag);
                            }
                            else
                            {
                                if (Log.IsDebugEnabled) Log.Debug($"Loaded unknown block entity\n{blockEntityTag}");
                            }
                        }

                    //NbtList tileTicks = dataTag["TileTicks"] as NbtList;

                    // if (Dimension == Dimension.Overworld && Config.GetProperty("CalculateLights", false))
                    // {
                    //     chunk.RecalcHeight();
                    //
                    //     var blockAccess = new SkyLightBlockAccess(this, chunk);
                    //     new SkyLightCalculations().RecalcSkyLight(chunk, blockAccess);
                    //     //TODO: Block lights.
                    // }

                    chunk.IsDirty = false;
                    chunk.NeedSave = false;

                    bool aaaaa = false;
                    RW.TryRemove(filePath, out aaaaa);
                    return chunk;
                }
            }
            catch (Exception e)
            {
                Log.Error($"Error While Loading chunk {coordinates}", e);
                return null;
            }
        }

        public bool HaveNether()
        {
            return false;
        }

        public bool HaveTheEnd()
        {
            return false;
        }

        private static void SetNibble4(byte[] arr, int index, byte value)
        {
            value &= 0xF;
            var idx = index >> 1;
            arr[idx] &= (byte) (0xF << (((index + 1) & 1) * 4));
            arr[idx] |= (byte) (value << ((index & 1) * 4));
        }

        static CyberExperimentalWorldProvider
            ()
        {
            var air = new Mapper(0, (i, b) => 0);
            Convert = new Dictionary<int, Tuple<int, Func<int, byte, byte>>>
            {
                {36, new NoDataMapper(250)}, // minecraft:piston_extension		=> MovingBlock
                {43, new Mapper(43, (i, b) => (byte) (b == 6 ? 7 : b == 7 ? 6 : b))}, // Fence		=> Fence
                {
                    44, new Mapper(44, (i, b) => (byte) (b == 6 ? 7 : b == 7 ? 6 : b == 14 ? 15 : b == 15 ? 14 : b))
                }, // Fence		=> Fence
                {
                    77, new Mapper(77, delegate(int i, byte b) // stone_button
                    {
                        switch (b & 0x7f)
                        {
                            case 0:
                                return (byte) BlockFace.Down;
                            case 1:
                                return (byte) BlockFace.East;
                            case 2:
                                return (byte) BlockFace.West;
                            case 3:
                                return (byte) BlockFace.South;
                            case 4:
                                return (byte) BlockFace.North;
                            case 5:
                                return (byte) BlockFace.Up;
                        }

                        return 0;
                    })
                },
                {84, new NoDataMapper(25)}, // minecraft:jukebox		=> noteblock
                {85, new Mapper(85, (i, b) => 0)}, // Fence		=> Fence
                {95, new NoDataMapper(241)}, // minecraft:stained_glass	=> Stained Glass
                {
                    96, new Mapper(96, (i, b) => (byte) (((b & 0x04) << 1) | ((b & 0x08) >> 1) | (3 - (b & 0x03))))
                }, // Trapdoor Fix
                {125, new NoDataMapper(157)}, // minecraft:double_wooden_slab	=> minecraft:double_wooden_slab
                {126, new NoDataMapper(158)}, // minecraft:wooden_slab		=> minecraft:wooden_slab
                {
                    143, new Mapper(143, delegate(int i, byte b) // wooden_button
                    {
                        switch (b & 0x7f)
                        {
                            case 0:
                                return (byte) BlockFace.Down; // 0
                            case 1:
                                return (byte) BlockFace.East; // 5
                            case 2:
                                return (byte) BlockFace.West; // 4
                            case 3:
                                return (byte) BlockFace.South; // 3
                            case 4:
                                return (byte) BlockFace.North; // 2
                            case 5:
                                return (byte) BlockFace.Up; // 1
                        }

                        return 0;
                    })
                },
                {157, new NoDataMapper(126)}, // minecraft:activator_rail
                {158, new NoDataMapper(125)}, // minecraft:dropper
                {166, new NoDataMapper(95)}, // minecraft:barrier		=> (Invisible Bedrock)
                {
                    167,
                    new Mapper(167, (i, b) => (byte) (((b & 0x04) << 1) | ((b & 0x08) >> 1) | (3 - (b & 0x03))))
                }, //Fix iron_trapdoor
                {188, new Mapper(85, (i, b) => 1)}, // Spruce Fence		=> Fence
                {189, new Mapper(85, (i, b) => 2)}, // Birch Fence		=> Fence
                {190, new Mapper(85, (i, b) => 3)}, // Jungle Fence		=> Fence
                {191, new Mapper(85, (i, b) => 5)}, // Dark Oak Fence	=> Fence
                {192, new Mapper(85, (i, b) => 4)}, // Acacia Fence		=> Fence
                {198, new NoDataMapper(208)}, // minecraft:end_rod	=> EndRod
                {199, new NoDataMapper(240)}, // minecraft:chorus_plant
                {202, new Mapper(201, (i, b) => 2)}, // minecraft:purpur_pillar => PurpurBlock:2 (idk why)
                {204, new Mapper(181, (i, b) => 1)}, // minecraft:purpur_double_slab
                {205, new Mapper(182, (i, b) => 1)}, // minecraft:purpur_slab
                {207, new NoDataMapper(244)}, // minecraft:beetroot_block
                {208, new NoDataMapper(198)}, // minecraft:grass_path
                {210, new NoDataMapper(188)}, // repeating_command_block
                {211, new NoDataMapper(189)}, // minecraft:chain_command_block
                {212, new NoDataMapper(297)}, // Frosted Ice
                {218, new NoDataMapper(251)}, // minecraft:observer => Observer
                {219, new Mapper(218, (i, b) => (byte) (0 + (b << 4)))}, // => minecraft:white_shulker_box
                {220, new Mapper(218, (i, b) => (byte) (1 + (b << 4)))}, // => minecraft:orange_shulker_box
                {221, new Mapper(218, (i, b) => (byte) (2 + (b << 4)))}, // => minecraft:magenta_shulker_box
                {222, new Mapper(218, (i, b) => (byte) (3 + (b << 4)))}, // => minecraft:light_blue_shulker_box 
                {223, new Mapper(218, (i, b) => (byte) (4 + (b << 4)))}, // => minecraft:yellow_shulker_box 
                {224, new Mapper(218, (i, b) => (byte) (5 + (b << 4)))}, // => minecraft:lime_shulker_box 
                {225, new Mapper(218, (i, b) => (byte) (6 + (b << 4)))}, // => minecraft:pink_shulker_box 
                {226, new Mapper(218, (i, b) => (byte) (7 + (b << 4)))}, // => minecraft:gray_shulker_box 
                {227, new Mapper(218, (i, b) => (byte) (8 + (b << 4)))}, // => minecraft:light_gray_shulker_box 
                {228, new Mapper(218, (i, b) => (byte) (9 + (b << 4)))}, // => minecraft:cyan_shulker_box 
                {229, new Mapper(218, (i, b) => (byte) (10 + (b << 4)))}, // => minecraft:purple_shulker_box 
                {230, new Mapper(218, (i, b) => (byte) (11 + (b << 4)))}, // => minecraft:blue_shulker_box 
                {231, new Mapper(218, (i, b) => (byte) (12 + (b << 4)))}, // => minecraft:brown_shulker_box 
                {232, new Mapper(218, (i, b) => (byte) (13 + (b << 4)))}, // => minecraft:green_shulker_box 
                {233, new Mapper(218, (i, b) => (byte) (14 + (b << 4)))}, // => minecraft:red_shulker_box 
                {234, new Mapper(218, (i, b) => (byte) (15 + (b << 4)))}, // => minecraft:black_shulker_box 

                {235, new NoDataMapper(220)}, // => minecraft:white_glazed_terracotta
                {236, new NoDataMapper(221)}, // => minecraft:orange_glazed_terracotta
                {237, new NoDataMapper(222)}, // => minecraft:magenta_glazed_terracotta
                {238, new NoDataMapper(223)}, // => minecraft:light_blue_glazed_terracotta
                {239, new NoDataMapper(224)}, // => minecraft:yellow_glazed_terracotta
                {240, new NoDataMapper(225)}, // => minecraft:lime_glazed_terracotta
                {241, new NoDataMapper(226)}, // => minecraft:pink_glazed_terracotta
                {242, new NoDataMapper(227)}, // => minecraft:gray_glazed_terracotta
                {243, new NoDataMapper(228)}, // => minecraft:light_gray_glazed_terracotta
                {244, new NoDataMapper(229)}, // => minecraft:cyan_glazed_terracotta
                {245, new NoDataMapper(219)}, // => minecraft:purple_glazed_terracotta
                {246, new NoDataMapper(231)}, // => minecraft:blue_glazed_terracotta
                {247, new NoDataMapper(232)}, // => minecraft:brown_glazed_terracotta
                {248, new NoDataMapper(233)}, // => minecraft:green_glazed_terracotta
                {249, new NoDataMapper(234)}, // => minecraft:red_glazed_terracotta
                {250, new NoDataMapper(235)}, // => minecraft:black_glazed_terracotta

                {251, new NoDataMapper(236)}, // => minecraft:concrete
                {252, new NoDataMapper(237)} // => minecraft:concrete_powder
            };
        }

        private static byte Nibble4(byte[] arr, int index)
        {
            return (byte) ((arr[index >> 1] >> ((index & 1) * 4)) & 0xF);
        }

        // public async Task<ChunkColumn> SmoothChunk(CyberExperimentalWorldProvider cewp,
        //     ChunkColumn chunk,
        //     float[] rth, AdvancedBiome b)
        // {
        //     var a = await b.preSmooth(cewp, chunk, rth);
        //     return a;
        // }

        public async Task<ChunkColumn> PopulateChunk(CyberExperimentalWorldProvider cewp,
            ChunkColumn chunk,
            float[] rth, AdvancedBiome b)
        {
            // var b = new MainBiome();
            var a = await b.prePopulate(cewp, chunk, rth);
            return a;
            // b.PopulateChunk(chunk, rain, temp);

// Console.WriteLine($"GENERATORED YO BITCH >> {chunk.X} {chunk.Z}");
        }

        public async Task<ChunkColumn> PreGenerateSurfaceItems(
            CyberExperimentalWorldProvider cewp,
            ChunkColumn chunk,
            float[] rth)
        {
            var s = new Stopwatch();
            s.Start();
            var a = await GenerateSurfaceItems(cewp, chunk, rth);
            s.Stop();
            if (s.ElapsedMilliseconds > 100) Log.Info($"CHUNK ADDING SURFACE ITEMS TOOK {s.Elapsed}");
            return a;
            // b.PopulateChunk(chunk, rain, temp);

// Console.WriteLine($"GENERATORED YO BITCH >> {chunk.X} {chunk.Z}");
        }

        public async Task<ChunkColumn> GenerateSurfaceItems(CyberExperimentalWorldProvider cewp,
            ChunkColumn chunk,
            float[] rth)
        {
            var b = BiomeManager.GetBiome(chunk);
            // var b = new MainBiome();
            var a = await b.GenerateSurfaceItems(cewp, chunk, rth);
            return a;
            // b.PopulateChunk(chunk, rain, temp);

// Console.WriteLine($"GENERATORED YO BITCH >> {chunk.X} {chunk.Z}");
        }

        private void GenerateTree(ChunkColumn chunk, int x, int treebase, int z)
        {
            var treeheight = GetRandomNumber(4, 5);

            chunk.SetBlock(x, treebase + treeheight + 2, z, new Leaves()); //Top leave

            chunk.SetBlock(x, treebase + treeheight + 1, z + 1, new Leaves());
            chunk.SetBlock(x, treebase + treeheight + 1, z - 1, new Leaves());
            chunk.SetBlock(x + 1, treebase + treeheight + 1, z, new Leaves());
            chunk.SetBlock(x - 1, treebase + treeheight + 1, z, new Leaves());

            chunk.SetBlock(x, treebase + treeheight, z + 1, new Leaves());
            chunk.SetBlock(x, treebase + treeheight, z - 1, new Leaves());
            chunk.SetBlock(x + 1, treebase + treeheight, z, new Leaves());
            chunk.SetBlock(x - 1, treebase + treeheight, z, new Leaves());

            chunk.SetBlock(x + 1, treebase + treeheight, z + 1, new Leaves());
            chunk.SetBlock(x - 1, treebase + treeheight, z - 1, new Leaves());
            chunk.SetBlock(x + 1, treebase + treeheight, z - 1, new Leaves());
            chunk.SetBlock(x - 1, treebase + treeheight, z + 1, new Leaves());

            for (var i = 0; i <= treeheight; i++) chunk.SetBlock(x, treebase + i, z, new Log());
        }

        private static int GetRandomNumber(int min, int max)
        {
            lock (syncLock)
            {
                // synchronize
                return getrandom.Next(min, max);
            }
        }

        public static float GetNoise(int x, int z, float scale, int max)
        {
            return (float) ((OpenNoise.Evaluate(x * scale, z * scale) + 1f) * (max / 2f));
            // var heightnoise = new FastNoise(123123 + 2);
            // heightnoise.SetNoiseType(FastNoise.NoiseType.SimplexFractal);
            // heightnoise.SetFrequency(scale);
            // heightnoise.SetFractalType(FastNoise.FractalType.FBM);
            // heightnoise.SetFractalOctaves(1);
            // heightnoise.SetFractalLacunarity(2);
            // heightnoise.SetFractalGain(.5f);
            // return (heightnoise.GetNoise(x, z)+1 )*(max/2f);
            // return (float)(OpenNoise.Evaluate(x * scale, z * scale) + 1f) * (max / 2f);
        }

        public static CyberExperimentalWorldProvider instance;

        public static CyberExperimentalWorldProvider getInstance()
        {
            return instance;
        }
    }
}