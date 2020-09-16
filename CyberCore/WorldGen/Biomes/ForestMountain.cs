using System;
using System.Threading.Tasks;
using CyberCore.WorldGen;
using CyberCore.WorldGen.Biomes;
using MiNET.Blocks;
using MiNET.Utils;
using MiNET.Worlds;

namespace CyberCore.WorldGen.Biomes
{
    public class ForestMountain : AdvancedBiome
    {
        public ForestMountain() : base("ForestMountain", new BiomeQualifications(1, 2, .5f, 1.5f, .75f, 2, 50))
        {
            LocalId = 21;
        }

        public override int GetSh(int x, int z, int cx, int cz)
        {
            return BiomeQualifications.Baseheight +15+
                   (int) GetNoise(cx * 16 + x, cz * 16 + z, /*rth[2] / */.035f,
                       BiomeQualifications.Heightvariation);
        }

        public override void GenerateVerticalColumn(int yheight, int maxheight, int x, int z, ChunkColumn cc, bool setair, bool objectcopy)
        {
            if (yheight == 0)
            {
                cc.SetBlock(x, yheight, z, new Bedrock());
            }
            else if (yheight <= maxheight - 5)
            {
                cc.SetBlock(x, yheight, z, new Stone());
            }
            else if (yheight < maxheight)
            {
                var r = RNDM.Next(0, 3);
                if (r == 0) cc.SetBlock(x, yheight, z, new Stone());
                if (r == 1) cc.SetBlock(x, yheight, z, new Dirt());
                if (r == 2) cc.SetBlock(x, yheight, z, new Dirt());
                if (r == 3) cc.SetBlock(x, yheight, z, new Stone());
            }else if(yheight==maxheight)
                cc.SetBlock(x,yheight,z, new Grass());
            else if (setair)
                cc.SetBlock(x, yheight, z, new Air());
        }
        
         /// <summary>
        /// </summary>
        /// <param name="openExperimentalWorldProvider"></param>
        /// <param name="chunk"></param>
        /// <param name="rth"></param>
        /// <returns></returns>
        public override async Task<ChunkColumn> GenerateSurfaceItems(
            CyberExperimentalWorldProvider o, ChunkColumn chunk, float[] rth)
        {
            var tg = new TreeGenerator(chunk)
            {
                TreeRandom = 16,
                MaxTreeHeight = 10,
                MaxTreeWidth = 7
            };
            return tg.run();
        }

        
        
    }
}