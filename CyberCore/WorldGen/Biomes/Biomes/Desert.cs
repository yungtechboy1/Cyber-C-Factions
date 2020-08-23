using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using CyberCore.WorldGen;
using CyberCore.WorldGen.Biomes;
using MiNET.Blocks;
using MiNET.Worlds;

namespace CyberCore.WorldGen.Biomes.Biomes
{
    public class Desert : AdvancedBiome
    {
        public Desert() : base("Desert", new BiomeQualifications(0, .75f, 1.5f, 2, 0.5f, 1.25f
            , 15))
        {
            LocalId = 4;
        }

        public override int GetSh(int x, int z, int cx, int cz)
        {
            return BiomeQualifications.Baseheight +
                   (int) GetNoise(cx * 16 + x, cz * 16 + z, /*rth[2] / */.035f / 4,
                       BiomeQualifications.Heightvariation);
        }

        public override void GenerateVerticalColumn(int yheight, int maxheight, int x, int z, ChunkColumn cc,
            bool setair)
        {
            if (yheight == 0)
                cc.SetBlock(x, yheight, z, new Bedrock());
            else if (yheight < maxheight * .75f - 1)
                cc.SetBlock(x, yheight, z, new Sandstone());
            else if (yheight <= maxheight)
                cc.SetBlock(x, yheight, z, new Sand());
            else if (setair)
                cc.SetBlock(x, yheight, z, new Air());
        }

        public override async Task<ChunkColumn> GenerateSurfaceItems(
            CyberExperimentalWorldProvider CyberExperimentalWorldProvider, ChunkColumn chunk,
            float[] rth)
        {
            Random r = new Random();
            for (int z = 0; z < 16; z++)
            for (int x = 0; x < 16; x++)
            {
                int h = chunk.GetHeight(x, z) + 1;
                if (r.Next(220) == 180)
                {
                    int hh = (int) Math.Ceiling(r.Next(9) / 3f) + 1;
                    for (int i = 0; i < hh; i++)
                    {
                        chunk.SetBlock(x, h + i, z, new Cactus());
                    }
                }
                else if (r.Next(200) == 195)
                {
                    chunk.SetBlock(x, h, z, new Deadbush());
                }
            }

            return chunk;
        }
    }
}