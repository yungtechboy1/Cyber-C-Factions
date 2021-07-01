using System.Threading.Tasks;
using CyberCore.WorldGen;
using CyberCore.WorldGen.Biomes;
using MiNET.Blocks;
using MiNET.Worlds;

namespace CyberCore.WorldGen.Populator
{
    public class Plains : AdvancedBiome
    {
        public Plains() : base("Plains", new BiomeQualifications(0.5f, 1.5f, 0.5f, 1.75f, 0.5f, .75f, 5))
        {
            LocalId = 8;
        }

        public override int GetSh(int x, int z, int cx, int cz)
        {
            return (int) (BiomeQualifications.Baseheight +
                          (int) (GetNoise(cx * 16 + x, cz * 16 + z, /*rth[2] / */.020f,
                              BiomeQualifications.Heightvariation)));
        }

        public override void GenerateVerticalColumn(int yheight, int maxheight, int x, int z, ChunkColumn cc,
            bool setair, bool objectcopy)
        {
            if (yheight == 0)
            {
                cc.SetBlock(x, yheight, z, new Bedrock());
            }
            else if (yheight <= maxheight - 5)
            {
                // cc.SetBlock(x, yheight, z, new Stone());
                TryOreGeneraton(cc, x, z, yheight);
            }
            else if (yheight < maxheight)
            {
                // int r = (RNDM).Next(0, 3);
              /*  if (r == 0)*/ cc.SetBlock(x, yheight, z, new Grass());
            }
            else if (setair)
                cc.SetBlock(x, yheight, z, new Air());
        }

        public override async Task<ChunkColumn> GenerateSurfaceItems(CyberExperimentalWorldProvider CyberExperimentalWorldProvider, ChunkColumn chunk,
            float[] rth)
        {
            var c = new TreeGenerator(chunk)
            {
                TreeRandom = 60
            }.run();
            return new GrassGenerator(c)
            {
                // TreeRandom = 60
            }.run();
        }
    }
}