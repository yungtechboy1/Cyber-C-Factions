using System;
using System.Collections.Generic;
using System.Numerics;
using MiNET.Blocks;
using MiNET.Utils;
using MiNET.Worlds;

namespace CyberCore.Custom
{
    public class CustomCyberGenerator : IWorldGenerator
  {
    public string Seed { get; set; }

    public List<Block> BlockLayers { get; set; }

    public Dimension Dimension { get; set; }

    public CustomCyberGenerator(Dimension dimension)
    {
      this.Dimension = dimension;
      switch (dimension)
      {
        case Dimension.Overworld:
          this.Seed = Config.GetProperty("superflat.overworld", "3;minecraft:bedrock,2*minecraft:dirt,minecraft:grass;1;village");
          break;
        case Dimension.Nether:
          this.Seed = Config.GetProperty("superflat.nether", "3;minecraft:bedrock,2*minecraft:netherrack,3*minecraft:lava,2*minecraft:netherrack,20*minecraft:air,minecraft:bedrock;1;village");
          break;
        case Dimension.TheEnd:
          this.Seed = Config.GetProperty("superflat.theend", "3;40*minecraft:air,minecraft:bedrock,7*minecraft:endstone;1;village");
          break;
      }
    }

    public void Initialize()
    {
      this.BlockLayers = SuperflatGenerator.ParseSeed(this.Seed);
    }

    public ChunkColumn GenerateChunkColumn(ChunkCoordinates chunkCoordinates)
    {
      ChunkColumn chunk = new ChunkColumn(true);
      chunk.X = chunkCoordinates.X;
      chunk.Z = chunkCoordinates.Z;
      this.PopulateChunk(chunk);
      Random random = new Random(chunk.X * 397 ^ chunk.Z);
      if (random.NextDouble() > 0.99)
        this.GenerateLake(random, chunk, this.Dimension == Dimension.Overworld ? (Block) new Water() : (this.Dimension == Dimension.Nether ? (Block) new Lava() : (Block) new Air()));
      else if (random.NextDouble() > 0.97)
        this.GenerateGlowStone(random, chunk);
      return chunk;
    }

    private void GenerateGlowStone(Random random, ChunkColumn chunk)
    {
      if (this.Dimension != Dimension.Nether || this.FindGroundLevel() < 0)
        return;
      Vector2 vector2_1 = new Vector2(7f, 8f);
      for (int bx = 0; bx < 16; ++bx)
      {
        for (int bz = 0; bz < 16; ++bz)
        {
          Vector2 vector2_2 = new Vector2((float) bx, (float) bz);
          if (random.Next((int) Vector2.DistanceSquared(vector2_1, vector2_2)) < 1)
          {
            chunk.SetBlock(bx, this.BlockLayers.Count - 2, bz, (Block) new Glowstone());
            if (random.NextDouble() > 0.85)
            {
              chunk.SetBlock(bx, this.BlockLayers.Count - 3, bz, (Block) new Glowstone());
              if (random.NextDouble() > 0.5)
                chunk.SetBlock(bx, this.BlockLayers.Count - 4, bz, (Block) new Glowstone());
            }
          }
        }
      }
    }

    private void GenerateLake(Random random, ChunkColumn chunk, Block block)
    {
      int groundLevel = this.FindGroundLevel();
      if (groundLevel < 0)
        return;
      Vector2 vector2_1 = new Vector2(7f, 8f);
      for (int bx = 0; bx < 16; ++bx)
      {
        for (int bz = 0; bz < 16; ++bz)
        {
          Vector2 vector2_2 = new Vector2((float) bx, (float) bz);
          if (random.Next((int) Vector2.DistanceSquared(vector2_1, vector2_2)) < 4)
          {
            if (this.Dimension == Dimension.Overworld)
              chunk.SetBlock(bx, groundLevel, bz, block);
            else if (this.Dimension == Dimension.Nether)
            {
              chunk.SetBlock(bx, groundLevel, bz, block);
              if (random.Next(30) == 0)
              {
                for (int by = groundLevel; by < this.BlockLayers.Count - 1; ++by)
                  chunk.SetBlock(bx, by, bz, block);
              }
            }
            else if (this.Dimension == Dimension.TheEnd)
            {
              for (int by = 0; by < this.BlockLayers.Count; ++by)
                chunk.SetBlock(bx, by, bz, (Block) new Air());
            }
          }
          else if (this.Dimension == Dimension.TheEnd && random.Next((int) Vector2.DistanceSquared(vector2_1, vector2_2)) < 15)
            chunk.SetBlock(bx, groundLevel, bz, (Block) new Air());
        }
      }
    }

    private int FindGroundLevel()
    {
      int num = 0;
      bool flag = false;
      foreach (Block blockLayer in this.BlockLayers)
      {
        if (flag && blockLayer is Air)
          return num - 1;
        if (blockLayer.IsSolid)
          flag = true;
        ++num;
      }
      return !flag ? -1 : num - 1;
    }

    public void PopulateChunk(ChunkColumn chunk)
    {
      List<Block> blockLayers = this.BlockLayers;
      for (int bx = 0; bx < 16; ++bx)
      {
        for (int bz = 0; bz < 16; ++bz)
        {
          int by1 = 0;
          foreach (Block block in blockLayers)
          {
            chunk.SetBlock(bx, by1, bz, block);
            ++by1;
          }
          chunk.SetHeight(bx, bz, (short) by1);
          for (int by2 = by1 + this.Dimension == Dimension.Overworld ? 1 : 0; by2 >= 0; --by2)
            chunk.SetSkyLight(bx, by2, bz, (byte) 0);
          chunk.SetBiome(bx, bz, (byte) 1);
        }
      }
    }

    public static List<Block> ParseSeed(string inputSeed)
    {
      if (string.IsNullOrEmpty(inputSeed))
        return new List<Block>();
      List<Block> blockList = new List<Block>();
      foreach (string str in inputSeed.Split(';', StringSplitOptions.None)[1].Split(',', StringSplitOptions.None))
      {
        string[] strArray1 = str.Replace("minecraft:", "").Split('*', StringSplitOptions.None);
        string[] strArray2 = strArray1[0].Split(':', StringSplitOptions.None);
        int num = 1;
        if (strArray1.Length > 1)
        {
          num = int.Parse(strArray1[0]);
          strArray2 = strArray1[1].Split(':', StringSplitOptions.None);
        }
        if (strArray2.Length != 0)
        {
          byte result1;
          Block block = !byte.TryParse(strArray2[0], out result1) ? BlockFactory.GetBlockByName(strArray2[0]) : BlockFactory.GetBlockById((int) result1);
          byte result2;
          if (strArray2.Length > 1 && byte.TryParse(strArray2[1], out result2))
            block.Metadata = result2;
          if (block != null)
          {
            for (int index = 0; index < num; ++index)
              blockList.Add(block);
          }
          else
            throw new Exception("Expected block, but didn't fine one for pattern " + str + ", " + string.Join("^", strArray2) + " ");
        }
      }
      return blockList;
    }
  }
}
