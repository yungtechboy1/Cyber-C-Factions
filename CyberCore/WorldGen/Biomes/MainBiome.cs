using System;
using System.Threading.Tasks;
using CyberCore.WorldGen;
using CyberCore.WorldGen.Biomes;
using MiNET.Blocks;
using MiNET.Worlds;

namespace CyberCore.WorldGen
{
    /// <summary>
    /// Main biome
    /// </summary>
    public class MainBiome : AdvancedBiome
    {
        public MainBiome() : base("MAIN", new BiomeQualifications(0, 2, 0, 2, 0, 2, 20))
        {
            Startheight = 90;
            BiomeQualifications.Baseheight = 83; //30
        }

        public override int GetSh(int x, int z, int cx, int cz)
        {
            return (int) GetNoise(cx * 16 + x, cz * 16 + z, 0.004f,
                10);
        }

        private void GenerateTree(ChunkColumn chunk, int x, int treebase, int z)
        {
            int treeheight = GetRandomNumber(4, 5);

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

            for (int i = 0; i <= treeheight; i++)
            {
                chunk.SetBlock(x, treebase + i, z, new Log());
            }
        }

        private static readonly Random getrandom = new Random();
        private static readonly object syncLock = new object();

        private static int GetRandomNumber(int min, int max)
        {
            lock (syncLock)
            {
                // synchronize
                return getrandom.Next(min, max);
            }
        }
    }
}