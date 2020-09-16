using System;
using System.Threading.Tasks;
using MiNET.Blocks;
using MiNET.Worlds;

namespace CyberCore.WorldGen.Biomes.Biomes
{
    public class DesertLake : AdvancedBiome
    {
        public DesertLake() : base("DesertLake", new BiomeQualifications(.75f, .1f, 1.5f, 2, .5f, 1.25f, 15))
        {
            BiomeQualifications.Baseheight = 87;
            LocalId = 6;
        }

        public override int GetSh(int x, int z, int cx, int cz)
        {
            return BiomeQualifications.Baseheight +
                   (int) GetNoise(cx * 16 + x, cz * 16 + z, /*rth[2] / */.035f / 4,
                       BiomeQualifications.Heightvariation);
        }

        public override void GenerateVerticalColumn(int yheight, int maxheight, int x, int z, ChunkColumn cc,
            bool setair, bool objectcopy)
        {
            if (yheight == 0)
                cc.SetBlock(x, yheight, z, new Bedrock());
            else if (yheight < maxheight * .75f - 1)
                cc.SetBlock(x, yheight, z, new Sandstone());
            else if (yheight <= maxheight)
                cc.SetBlock(x, yheight, z, new Sand());
            else if (yheight == maxheight)
                cc.SetBlock(x, yheight, z, new Sand());
            else if(yheight <= Waterlevel)
                cc.SetBlock(x,yheight,z,new Water());
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
                if (chunk.GetBlockId(x, h, z) == new Water().Id) return chunk;
                if (r.Next(600) == 180)
                {
                    int hh = (int) Math.Ceiling(r.Next(9) / 3f)+1;
                    for (int i = 0; i < hh; i++)
                    {
                        chunk.SetBlock(x, h + i, z, new Cactus());
                    }
                }
                else if (r.Next(250) == 195)
                {
                    chunk.SetBlock(x, h, z, new Deadbush());
                }
            }

            return chunk;
        }
        
    }
}