using System;
using System.Threading.Tasks;
using CyberCore.WorldGen;
using CyberCore.WorldGen.Biomes;
using MiNET.Blocks;
using MiNET.Worlds;

namespace CyberCore.WorldGen.Biomes
{
    public class SnowForest : AdvancedBiome
    {
        public SnowForest() : base("SnowForest", new BiomeQualifications(.5f, 1.25f, 0, 0.5f, .75f, 1.25f, 30))
        {
            LocalId = 12;
            MCPEBiomeID=30;
        }

        public override int GetSh(int x, int z, int cx, int cz)
        {
            return BiomeQualifications.Baseheight +
                   (int) GetNoise(cx * 16 + x, cz * 16 + z, /*rth[2] / */.035f,
                       BiomeQualifications.Heightvariation);
        }

        public override async Task<ChunkColumn> GenerateSurfaceItems(
            CyberExperimentalWorldProvider o, ChunkColumn chunk, float[] rth)
        {
            var cc = new TreeGenerator(chunk)
            {
                TreeRandom = 10,
                MaxTreeHeight = 20,

                // MaxTreeWidth = 8
            };
            ((Leaves) cc.LeavesItem).OldLeafType = "jungle";
            ((Wood) cc.WoodItem).WoodType = "jungle";
            var c = cc.run();
            c = new GrassGenerator(c)
            {
                GrassRandom = 11,
                GrassGreaterThan = 8,
                RainMultiplier = 1.3f
                // TreeRandom = 60
            }.run();
            // c = new TallGrassGenerator(c)
            // {
            //     GrassRandom = 10,
            //     GrassGreaterThan = 6
            // }.run();
            var f = new FlowerGenerator(c)
            {
                FlowerRandom = 100,
                FlowerGreaterThan = 60
            };
            f.FlowerChances.Add(new RedFlower()
            {
                FlowerType = "tulip_pink"
            }, 10);
            f.FlowerChances.Add(new RedFlower()
            {
                FlowerType = "houstonia"
            }, 10);
            f.FlowerChances.Add(new RedFlower()
            {
                FlowerType = "lily_of_the_valley"
            }, 10);
            f.FlowerChances.Add(new RedFlower()
            {
                FlowerType = "allium"
            }, 10);
            f.FlowerChances.Add(new RedFlower()
            {
                FlowerType = "poppy"
            }, 10);
            f.FlowerChances.Add(new RedFlower()
            {
                FlowerType = "cornflower"
            }, 10);
            f.FlowerChances.Add(new RedFlower()
            {
                FlowerType = "tulip_orange"
            }, 10);
            f.FlowerChances.Add(new RedFlower()
            {
                FlowerType = "oxeye"
            }, 10);
            f.FlowerChances.Add(new RedFlower()
            {
                FlowerType = "orchid"
            }, 10);

            c = f.run();
            c = new DoublePlantGenerator(c)
            {
                FlowerRandom = 100,
                FlowerGreaterThan = 40
            }.run();
            return c;
        }
        
        public override void GenerateVerticalColumn(int yheight, int maxheight, int x, int z, ChunkColumn cc,
            bool setair, bool objectcopy)
        {
            if (yheight <= 1)
                cc.SetBlock(x, yheight, z, new Bedrock());
            if (yheight < maxheight-1)
            
                TryOreGeneraton(cc, x, z, yheight);
            
            if (yheight == maxheight-1)
            
                cc.SetBlock(x, yheight, z, new Grass());
            if (yheight == maxheight)
            
                cc.SetBlock(x, yheight, z, new SnowLayer());
            else if (setair)
                cc.SetBlock(x, yheight, z, new Air());
        }
    }
}