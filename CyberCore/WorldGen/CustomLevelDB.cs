// using MiNET;
// using MiNET.Blocks;
// using MiNET.Entities;
// using MiNET.LevelDB;
// using MiNET.Utils;
// using MiNET.Worlds;
// using fNbt;
// using log4net;
// using System;
// using System.Buffers.Binary;
// using System.Collections.Concurrent;
// using System.Collections.Generic;
// using System.Diagnostics;
// using System.IO;
// using System.Linq;
// using System.Numerics;
// using System.Runtime.CompilerServices;
// using System.Threading;
// using System.Threading.Tasks;
//
// namespace CyberCore.WorldGen
// {
//     public class CustomLevelDb : IWorldProvider, ICachingWorldProvider, ICloneable
//     {
//         private static readonly ILog Log = LogManager.GetLogger(typeof(CustomLevelDb));
//
//         private ConcurrentDictionary<ChunkCoordinates, ChunkColumn> _chunkCache =
//             new ConcurrentDictionary<ChunkCoordinates, ChunkColumn>();
//
//         public Database Db { get; private set; }
//
//         public string BasePath { get; private set; }
//
//         public LevelInfoBedrock LevelInfo { get; private set; }
//
//         public bool IsCaching { get; } = true;
//
//         public bool Locked { get; set; }
//
//         public IWorldGenerator MissingChunkProvider { get; set; }
//
//         public Dimension Dimension { get; set; }
//
//         public CustomLevelDb(Database db = null) => this.Db = db;
//
//         public void Initialize()
//         {
//             if (this.BasePath == null)
//                 this.BasePath = Config.GetProperty("LevelDBWorldFolder", "World").Trim();
//             DirectoryInfo dbDirectory = new DirectoryInfo(Path.Combine(this.BasePath, "db"));
//             string path = Path.Combine(this.BasePath, "level.dat");
//             Log.Debug((object) ("Loading level.dat from " + path));
//             if (File.Exists(path))
//             {
//                 NbtFile nbtFile = new NbtFile()
//                 {
//                     BigEndian = false,
//                     UseVarInt = false
//                 };
//                 using (FileStream fileStream = File.OpenRead(path))
//                 {
//                     fileStream.Seek(8L, SeekOrigin.Begin);
//                     nbtFile.LoadFromStream((Stream) fileStream, NbtCompression.None);
//                     Log.Debug((object) string.Format("Level DAT\n{0}", (object) nbtFile.RootTag));
//                     this.LevelInfo = nbtFile.RootTag.Deserialize<LevelInfoBedrock>();
//                 }
//             }
//             else
//             {
//                 Log.Warn((object) ("No level.dat found at " + path + ". Creating empty."));
//                 this.LevelInfo = new LevelInfoBedrock();
//             }
//
//             if (this.Db == null)
//             {
//                 Database database = new Database(dbDirectory, true);
//                 database.Open();
//                 this.Db = database;
//                 dbDirectory.Refresh();
//                 AppDomain.CurrentDomain.ProcessExit += (EventHandler) ((sender, args) =>
//                 {
//                     this.SaveChunks();
//                     Log.Warn((object) "Closing LevelDB");
//                     this.Db.Close();
//                 });
//             }
//
//             this.MissingChunkProvider?.Initialize((IWorldProvider) this);
//         }
//
//         public ChunkColumn GenerateChunkColumn(
//             ChunkCoordinates chunkCoordinates,
//             bool cacheOnly = false)
//         {
//             if (this.Locked | cacheOnly)
//             {
//                 ChunkColumn chunkColumn;
//                 this._chunkCache.TryGetValue(chunkCoordinates, out chunkColumn);
//                 return chunkColumn;
//             }
//
//             ChunkColumn chunkColumn1;
//             if (this._chunkCache.TryGetValue(chunkCoordinates, out chunkColumn1))
//             {
//                 if (chunkColumn1 == null)
//                     this._chunkCache.TryRemove(chunkCoordinates, out chunkColumn1);
//                 if (chunkColumn1 != null)
//                     return chunkColumn1;
//             }
//
//             return this._chunkCache.GetOrAdd(chunkCoordinates,
//                 (Func<ChunkCoordinates, ChunkColumn>) (coordinates =>
//                     this.GetChunk(coordinates, this.MissingChunkProvider)));
//         }
//
//         public ChunkColumn GetChunk(ChunkCoordinates coordinates, IWorldGenerator generator)
//         {
//             Stopwatch stopwatch = Stopwatch.StartNew();
//             stopwatch.Stop();
//             byte[] first = Combine(BitConverter.GetBytes(coordinates.X),
//                 BitConverter.GetBytes(coordinates.Z));
//             if (this.Dimension == Dimension.Nether)
//                 first = Combine(first, BitConverter.GetBytes(1));
//             byte[] numArray1 = Combine(first, (byte) 118);
//             stopwatch.Start();
//             byte[] numArray2 = this.Db.Get((Span<byte>) numArray1);
//             stopwatch.Stop();
//             ChunkColumn chunk = (ChunkColumn) null;
//             if (numArray2 != null && ((IEnumerable<byte>) numArray2).First<byte>() >= (byte) 10)
//             {
//                 chunk = new ChunkColumn()
//                 {
//                     X = coordinates.X,
//                     Z = coordinates.Z
//                 };
//                 byte[] numArray3 = Combine(first, new byte[2]
//                 {
//                     (byte) 47,
//                     (byte) 0
//                 });
//                 for (byte index = 0; index < (byte) 16; ++index)
//                 {
//                     byte[] numArray4 = numArray3;
//                     numArray4[numArray4.Length - 1] = index;
//                     stopwatch.Start();
//                     byte[] numArray5 = this.Db.Get((Span<byte>) numArray3);
//                     stopwatch.Stop();
//                     if (numArray5 == null)
//                     {
//                         chunk[(int) index]?.PutPool();
//                         chunk[(int) index] = (SubChunk) null;
//                     }
//                     else
//                         this.ParseSection(chunk[(int) index], (ReadOnlyMemory<byte>) numArray5);
//                 }
//
//                 stopwatch.Start();
//                 byte[] array1 = this.Db.Get((Span<byte>) Combine(first, (byte) 45));
//                 stopwatch.Stop();
//                 if (array1 != null)
//                 {
//                     Span<byte> span = array1.AsSpan<byte>();
//                     span = span.Slice(0, 512);
//                     Buffer.BlockCopy((Array) span.ToArray(), 0, (Array) chunk.height, 0, 512);
//                     ChunkColumn chunkColumn = chunk;
//                     span = array1.AsSpan<byte>();
//                     span = span.Slice(512, 256);
//                     byte[] array2 = span.ToArray();
//                     chunkColumn.biomeId = array2;
//                 }
//
//                 stopwatch.Start();
//                 byte[] array3 = this.Db.Get((Span<byte>) Combine(first, (byte) 49));
//                 stopwatch.Stop();
//                 Log.Debug((object) string.Format("Read chunk from LevelDB {0}, {1} in {2} ms.",
//                     (object) coordinates.X, (object) coordinates.Z, (object) stopwatch.ElapsedMilliseconds));
//                 if (array3 != null)
//                 {
//                     Memory<byte> memory = array3.AsMemory<byte>();
//                     NbtFile nbtFile = new NbtFile()
//                     {
//                         BigEndian = false,
//                         UseVarInt = false
//                     };
//                     int start = 0;
//                     do
//                     {
//                         start += (int) nbtFile.LoadFromStream(
//                             (Stream) new MemoryStreamReader((ReadOnlyMemory<byte>) memory.Slice(start)),
//                             NbtCompression.None);
//                         NbtTag rootTag = nbtFile.RootTag;
//                         int intValue1 = rootTag["x"].IntValue;
//                         int intValue2 = rootTag["y"].IntValue;
//                         int intValue3 = rootTag["z"].IntValue;
//                         chunk.SetBlockEntity(new BlockCoordinates(intValue1, intValue2, intValue3),
//                             (NbtCompound) rootTag);
//                     } while (start < memory.Length);
//                 }
//             }
//
//             bool flag = false;
//             if (chunk == null)
//             {
//                 if (numArray2 != null)
//                     Log.Error((object) string.Format("Expected other version, but got version={0}",
//                         (object) ((IEnumerable<byte>) numArray2).First<byte>()));
//                 chunk = generator?.GenerateChunkColumn(coordinates);
//                 chunk?.RecalcHeight();
//                 flag = true;
//             }
//
//             if (chunk != null)
//             {
//                 if (this.Dimension == Dimension.Overworld && Config.GetProperty("CalculateLights", false))
//                 {
//                     SkyLightBlockAccess lightBlockAccess = new SkyLightBlockAccess((IWorldProvider) this, chunk);
//                     new SkyLightCalculations().RecalcSkyLight(chunk, (IBlockAccess) lightBlockAccess);
//                 }
//
//                 chunk.IsDirty = false;
//             }
//
//             Log.Debug((object) string.Format("Read chunk {0}, {1} in {2} ms. Was generated: {3}",
//                 (object) coordinates.X, (object) coordinates.Z, (object) stopwatch.ElapsedMilliseconds, (object) flag));
//             return chunk;
//         }
//
//         internal void ParseSection(SubChunk section, ReadOnlyMemory<byte> data)
//         {
//             MemoryStreamReader memoryStreamReader = new MemoryStreamReader(data);
//             int num1 = memoryStreamReader.ReadByte() == 8
//                 ? memoryStreamReader.ReadByte()
//                 : throw new Exception("Wrong chunk version");
//             for (int index1 = 0; index1 < num1; ++index1)
//             {
//                 bool flag = index1 == 0;
//                 byte num2 = (byte) memoryStreamReader.ReadByte();
//                 if (((uint) num2 & 1U) > 0U)
//                     throw new Exception("Can't use runtime for persistent storage.");
//                 int num3 = (int) num2 >> 1;
//                 int num4 = (int) Math.Floor(32.0 / (double) num3);
//                 int num5 = (int) Math.Ceiling(4096.0 / (double) num4);
//                 long position1 = memoryStreamReader.Position;
//                 memoryStreamReader.Position += (long) (num5 * 4);
//                 int num6 = memoryStreamReader.ReadInt32();
//                 List<int> intList = flag ? section.RuntimeIds : section.LoggedRuntimeIds;
//                 intList.Clear();
//                 for (int index2 = 0; index2 < num6; ++index2)
//                 {
//                     NbtFile nbtFile = new NbtFile()
//                     {
//                         BigEndian = false,
//                         UseVarInt = false
//                     };
//                     nbtFile.LoadFromStream((Stream) memoryStreamReader, NbtCompression.None);
//                     NbtCompound rootTag = (NbtCompound) nbtFile.RootTag;
//                     Block block = BlockFactory.GetBlockByName(rootTag["name"].StringValue);
//                     if (block != null && block.GetType() != typeof(Block) && !(block is Air))
//                     {
//                         List<IBlockState> states = ReadBlockState(rootTag);
//                         block.SetState(states);
//                     }
//                     else
//                         block = (Block) new Air();
//
//                     intList.Add(block.GetRuntimeId());
//                 }
//
//                 long position2 = memoryStreamReader.Position;
//                 memoryStreamReader.Position = position1;
//                 int num7 = 0;
//                 for (int index2 = 0; index2 < num5; ++index2)
//                 {
//                     uint num8 = memoryStreamReader.ReadUInt32();
//                     for (int index3 = 0; index3 < num4; ++index3)
//                     {
//                         if (num7 < 4096)
//                         {
//                             int num9 = (int) ((long) (num8 >> num7 % num4 * num3) & (long) ((1 << num3) - 1));
//                             int bx = num7 >> 8 & 15;
//                             int by = num7 & 15;
//                             int bz = num7 >> 4 & 15;
//                             if (num9 > intList.Count)
//                                 Log.Error((object) string.Format(
//                                     "Got wrong state={0} from word. bitsPerBlock={1}, blocksPerWord={2}, Word={3}",
//                                     (object) num9, (object) num3, (object) num4, (object) num8));
//                             if (flag)
//                                 section.SetBlockIndex(bx, by, bz, (short) num9);
//                             else
//                                 section.SetLoggedBlockIndex(bx, by, bz, (byte) num9);
//                             ++num7;
//                         }
//                     }
//                 }
//
//                 memoryStreamReader.Position = position2;
//             }
//         }
//
//         [MethodImpl(MethodImplOptions.AggressiveInlining)]
//         private static byte[] Combine(byte[] first, byte[] second)
//         {
//             byte[] numArray = new byte[first.Length + second.Length];
//             Buffer.BlockCopy((Array) first, 0, (Array) numArray, 0, first.Length);
//             Buffer.BlockCopy((Array) second, 0, (Array) numArray, first.Length, second.Length);
//             return numArray;
//         }
//
//         [MethodImpl(MethodImplOptions.AggressiveInlining)]
//         private static byte[] Combine(byte[] first, byte b)
//         {
//             byte[] numArray1 = new byte[first.Length + 1];
//             Buffer.BlockCopy((Array) first, 0, (Array) numArray1, 0, first.Length);
//             byte[] numArray2 = numArray1;
//             numArray2[numArray2.Length - 1] = b;
//             return numArray1;
//         }
//
//         public Vector3 GetSpawnPoint() => new Vector3((float) this.LevelInfo.SpawnX,
//             this.LevelInfo.SpawnY == (int) short.MaxValue ? 0.0f : (float) this.LevelInfo.SpawnY,
//             (float) this.LevelInfo.SpawnZ);
//
//         public string GetName() => this.LevelInfo.LevelName;
//
//         public long GetTime() => this.LevelInfo.Time;
//
//         public long GetDayTime() => this.LevelInfo.Time;
//
//         public int SaveChunks()
//         {
//             if (!Config.GetProperty("Save.Enabled", false))
//                 return 0;
//             int num = 0;
//             try
//             {
//                 lock (this._chunkCache)
//                 {
//                     if (this.Dimension == Dimension.Overworld)
//                         this.SaveLevelInfo(this.LevelInfo);
//                     foreach (ChunkColumn chunk in (IEnumerable<ChunkColumn>) this._chunkCache.Values)
//                     {
//                         if (chunk != null && chunk.NeedSave)
//                         {
//                             this.SaveChunk(chunk);
//                             ++num;
//                         }
//                     }
//                 }
//             }
//             catch (Exception ex)
//             {
//                 Log.Error((object) "saving chunks", ex);
//             }
//
//             return num;
//         }
//
//         private unsafe void SaveLevelInfo(LevelInfoBedrock levelInfo)
//         {
//             string path = Path.Combine(this.BasePath, "level.dat");
//             Log.Debug((object) ("Saving level.dat to " + path));
//             NbtTag nbtTag = levelInfo.Serialize<LevelInfoBedrock>();
//             NbtFile nbtFile = new NbtFile()
//             {
//                 BigEndian = false,
//                 UseVarInt = false
//             };
//             nbtFile.RootTag = nbtTag;
//             byte[] buffer = nbtFile.SaveToBuffer(NbtCompression.None);
//             using (FileStream fileStream = File.Create(path))
//             {
//                 // ISSUE: reference to a compiler-generated field
//                 fileStream.Write(new ReadOnlySpan<byte>(
//                     (void*) &\u003CPrivateImplementationDetails\u003E
//                         .DC765660B06EE03DD16FD7CA5B957E8C805161AC2C4AF28C5A100AB2AB432CA1, 4));
//                 fileStream.Write((ReadOnlySpan<byte>) BitConverter.GetBytes(buffer.Length));
//                 fileStream.Write((ReadOnlySpan<byte>) buffer);
//                 fileStream.Flush();
//             }
//         }
//
//         private void SaveChunk(ChunkColumn chunk)
//         {
//             byte[] first1 = Combine(BitConverter.GetBytes(chunk.X), BitConverter.GetBytes(chunk.Z));
//             if (this.Dimension == Dimension.Nether)
//                 first1 = Combine(first1, BitConverter.GetBytes(1));
//             byte[] numArray1 = Combine(first1, (byte) 118);
//             if (this.Db.Get((Span<byte>) numArray1) == null)
//                 this.Db.Put((Span<byte>) numArray1, (Span<byte>) new byte[1]
//                 {
//                     (byte) 13
//                 });
//             byte[] numArray2 = Combine(first1, new byte[2]
//             {
//                 (byte) 47,
//                 (byte) 0
//             });
//             for (byte index = 0; index < (byte) 16; ++index)
//             {
//                 byte[] numArray3 = numArray2;
//                 numArray3[numArray3.Length - 1] = index;
//                 byte[] sectionBytes = this.GetSectionBytes(chunk[(int) index]);
//                 this.Db.Put((Span<byte>) numArray2, (Span<byte>) sectionBytes);
//             }
//
//             byte[] first2 = new byte[512];
//             Buffer.BlockCopy((Array) chunk.height, 0, (Array) first2, 0, 512);
//             byte[] numArray4 = Combine(first2, chunk.biomeId);
//             this.Db.Put((Span<byte>) Combine(first1, (byte) 45), (Span<byte>) numArray4);
//             chunk.NeedSave = false;
//         }
//
//         private byte[] GetSectionBytes(SubChunk subChunk)
//         {
//             using (MemoryStream stream = new MemoryStream())
//             {
//                 this.Write(subChunk, stream);
//                 return stream.ToArray();
//             }
//         }
//
//         public void Write(SubChunk subChunk, MemoryStream stream)
//         {
//             long position1 = stream.Position;
//             stream.WriteByte((byte) 8);
//             long position2 = stream.Position;
//             int num = 0;
//             stream.WriteByte((byte) num);
//             if (this.WriteStore(stream, subChunk.Blocks, (byte[]) null, false, subChunk.RuntimeIds))
//             {
//                 ++num;
//                 if (this.WriteStore(stream, (short[]) null, subChunk.LoggedBlocks, false, subChunk.LoggedRuntimeIds))
//                     ++num;
//             }
//
//             stream.Position = position2;
//             stream.WriteByte((byte) num);
//         }
//
//         internal bool WriteStore(
//             MemoryStream stream,
//             short[] blocks,
//             byte[] loggedBlocks,
//             bool forceWrite,
//             List<int> palette)
//         {
//             if (palette.Count == 0)
//                 return false;
//             int num1 = (int) Math.Ceiling(Math.Log((double) palette.Count, 2.0));
//             switch (num1)
//             {
//                 case 0:
//                     if (!forceWrite && palette.Contains(0))
//                         return false;
//                     num1 = 1;
//                     goto case 1;
//                 case 1:
//                 case 2:
//                 case 3:
//                 case 4:
//                 case 5:
//                 case 6:
//                     stream.WriteByte((byte) (num1 << 1 | 0));
//                     int num2 = (int) Math.Floor(32.0 / (double) num1);
//                     int length = (int) Math.Ceiling(4096.0 / (double) num2);
//                     uint[] numArray1 = new uint[length];
//                     int index1 = 0;
//                     for (int index2 = 0; index2 < length; ++index2)
//                     {
//                         uint num3 = 0;
//                         for (int index3 = 0; index3 < num2; ++index3)
//                         {
//                             if (index1 < 4096)
//                             {
//                                 uint num4 = blocks == null ? (uint) loggedBlocks[index1] : (uint) blocks[index1];
//                                 num3 |= num4 << num1 * index3;
//                                 ++index1;
//                             }
//                         }
//
//                         numArray1[index2] = num3;
//                     }
//
//                     byte[] buffer1 = new byte[numArray1.Length * 4];
//                     Buffer.BlockCopy((Array) numArray1, 0, (Array) buffer1, 0, numArray1.Length * 4);
//                     stream.Write(buffer1, 0, buffer1.Length);
//                     byte[] numArray2 = new byte[4];
//                     BinaryPrimitives.WriteInt32LittleEndian((Span<byte>) numArray2, palette.Count);
//                     stream.Write((ReadOnlySpan<byte>) numArray2);
//                     foreach (int index2 in palette)
//                     {
//                         BlockStateContainer container = BlockFactory.BlockPalette[index2];
//                         NbtFile nbtFile = new NbtFile()
//                         {
//                             BigEndian = false,
//                             UseVarInt = false
//                         };
//                         nbtFile.RootTag = (NbtTag) WriteBlockState(container);
//                         byte[] buffer2 = nbtFile.SaveToBuffer(NbtCompression.None);
//                         stream.Write((ReadOnlySpan<byte>) buffer2);
//                     }
//
//                     return true;
//                 case 7:
//                 case 8:
//                     num1 = 8;
//                     goto case 1;
//                 default:
//                     if (num1 > 8)
//                     {
//                         num1 = 16;
//                         goto case 1;
//                     }
//                     else
//                         goto case 1;
//             }
//         }
//
//         public bool HaveNether() => true;
//
//         public bool HaveTheEnd() => false;
//
//         public ChunkColumn[] GetCachedChunks() => this._chunkCache.Values
//             .Where<ChunkColumn>((Func<ChunkColumn, bool>) (column => column != null)).ToArray<ChunkColumn>();
//
//         public void ClearCachedChunks() => this._chunkCache.Clear();
//
//         public int UnloadChunks(Player[] players, ChunkCoordinates spawn, double maxViewDistance)
//         {
//             int removed = 0;
//             lock (this._chunkCache)
//             {
//                 List<ChunkCoordinates> coords = new List<ChunkCoordinates>()
//                 {
//                     spawn
//                 };
//                 foreach (Entity player in players)
//                 {
//                     ChunkCoordinates chunkCoordinates = new ChunkCoordinates(player.KnownPosition);
//                     if (!coords.Contains(chunkCoordinates))
//                         coords.Add(chunkCoordinates);
//                 }
//
//                 Parallel.ForEach<KeyValuePair<ChunkCoordinates, ChunkColumn>>(
//                     (IEnumerable<KeyValuePair<ChunkCoordinates, ChunkColumn>>) this._chunkCache,
//                     (Action<KeyValuePair<ChunkCoordinates, ChunkColumn>>) (chunkColumn =>
//                     {
//                         if (coords.Exists((Predicate<ChunkCoordinates>) (c =>
//                             c.DistanceTo(chunkColumn.Key) < maxViewDistance)))
//                             return;
//                         ChunkColumn chunkColumn1;
//                         this._chunkCache.TryRemove(chunkColumn.Key, out chunkColumn1);
//                         if (chunkColumn1 != null)
//                         {
//                             foreach (SubChunk subChunk in chunkColumn1)
//                                 subChunk.PutPool();
//                         }
//
//                         Interlocked.Increment(ref removed);
//                     }));
//             }
//
//             return removed;
//         }
//
//         public object Clone() => throw new NotImplementedException();
//
//         private static List<IBlockState> ReadBlockState(NbtCompound tag)
//         {
//             List<IBlockState> blockStateList = new List<IBlockState>();
//             foreach (NbtTag nbtTag in (NbtCompound) tag["states"])
//             {
//                 IBlockState blockState1;
//                 switch (nbtTag.TagType)
//                 {
//                     case NbtTagType.Byte:
//                         blockState1 = (IBlockState) new BlockStateByte()
//                         {
//                             Name = nbtTag.Name,
//                             Value = nbtTag.ByteValue
//                         };
//                         break;
//                     case NbtTagType.Int:
//                         blockState1 = (IBlockState) new BlockStateInt()
//                         {
//                             Name = nbtTag.Name,
//                             Value = nbtTag.IntValue
//                         };
//                         break;
//                     case NbtTagType.String:
//                         blockState1 = (IBlockState) new BlockStateString()
//                         {
//                             Name = nbtTag.Name,
//                             Value = nbtTag.StringValue
//                         };
//                         break;
//                     default:
//                         throw new ArgumentOutOfRangeException();
//                 }
//
//                 IBlockState blockState2 = blockState1;
//                 blockStateList.Add(blockState2);
//             }
//
//             return blockStateList;
//         }
//
//         private static NbtCompound WriteBlockState(BlockStateContainer container)
//         {
//             NbtCompound nbtCompound1 = new NbtCompound("");
//             nbtCompound1.Add((NbtTag) new NbtString("name", container.Name));
//             NbtCompound nbtCompound2 = new NbtCompound("states");
//             foreach (IBlockState state in container.States)
//             {
//                 switch (state)
//                 {
//                     case BlockStateByte blockStateByte:
//                         nbtCompound2.Add((NbtTag) new NbtByte(blockStateByte.Name, blockStateByte.Value));
//                         continue;
//                     case BlockStateInt blockStateInt:
//                         nbtCompound2.Add((NbtTag) new NbtInt(blockStateInt.Name, blockStateInt.Value));
//                         continue;
//                     case BlockStateString blockStateString:
//                         nbtCompound2.Add((NbtTag) new NbtString(blockStateString.Name, blockStateString.Value));
//                         continue;
//                     default:
//                         continue;
//                 }
//             }
//
//             nbtCompound1.Add((NbtTag) nbtCompound2);
//             return nbtCompound1;
//         }
//     }
// }