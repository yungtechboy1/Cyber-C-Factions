using System;
using System.Threading.Tasks;
using MiNET.Blocks;
using MiNET.Worlds;

namespace CyberCore.WorldGen.Biomes.Biomes
{
    public class DesertHills : AdvancedBiome
    {
        public DesertHills() : base("DesertHills", new BiomeQualifications(0, .75f, 1.5f, 2, 1f, 2f , 40))
        {
        }

        public override int GetSh(int x, int z, int cx, int cz)
        {
            var heightnoise = new FastNoise(123123 + 2);
            heightnoise.SetNoiseType(FastNoise.NoiseType.CubicFractal);
            heightnoise.SetFrequency(0.035f);
            heightnoise.SetFractalType(FastNoise.FractalType.FBM);
            heightnoise.SetFractalOctaves(1);
            heightnoise.SetFractalLacunarity(2);
            heightnoise.SetFractalGain(.5f);
            heightnoise.SetGradientPerturbAmp(20);
            float xx = cx * 16 + x;
            float zz = cz * 16 + z;
            heightnoise.GradientPerturbFractal(ref xx,ref zz);
           //IDK If it works but it works XD
            
            return BiomeQualifications.Baseheight +
                   (int) ((heightnoise.GetNoise(xx,  zz) + .75f) * (BiomeQualifications.Heightvariation / 2f));
        }

        public override void GenerateVerticalColumn(int yheight, int maxheight, int x, int z, ChunkColumn cc,
            bool setair)
        {
            if (yheight == 0)
            {
                cc.SetBlock(x, yheight, z, new Bedrock());
            }
            else if (yheight < maxheight * 0.5f - 1)
                cc.SetBlock(x, yheight, z, new Stone());
            else if (yheight < maxheight * .75f - 1)
                cc.SetBlock(x, yheight, z, new Sandstone());
            else if (yheight < maxheight)
                cc.SetBlock(x, yheight, z, new Sand());
            else if (yheight == maxheight)
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