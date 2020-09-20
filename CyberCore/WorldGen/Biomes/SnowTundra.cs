using System;
using System.Threading.Tasks;
using CyberCore.WorldGen;
using CyberCore.WorldGen.Biomes;
using MiNET.Blocks;
using MiNET.Worlds;

namespace CyberCore.WorldGen.Biomes
{
    public class SnowTundra : AdvancedBiome
    {
        public SnowTundra() : base("SnowTundra", new BiomeQualifications(0, 2, 0, .5f, .5f, 1, 10))
        {
            LocalId = 13;
            MCPEBiomeID = 12;
        }

        public override int GetSh(int x, int z, int cx, int cz)
        {
            return BiomeQualifications.Baseheight +
                   (int) GetNoise(cx * 16 + x, cz * 16 + z, /*rth[2] / */.035f,
                       BiomeQualifications.Heightvariation);
        }


        public override void GenerateVerticalColumn(int yheight, int maxheight, int x, int z, ChunkColumn cc,
            bool setair)
        {
            if (yheight < maxheight-1)
            
                TryOreGeneraton(cc, x, z, yheight);
            
            if (yheight == maxheight-1)
            
                cc.SetBlock(x, yheight, z, new Grass());
            if (yheight == maxheight)
                if(BiomeManager.GetRainNoiseBlock((x+(16*cc.X)), (z+(16*cc.Z)))/2 <= .42f)cc.SetBlock(x, yheight, z, new SnowLayer());
            else if (setair)
                cc.SetBlock(x, yheight, z, new Air());
        }

        
        
        public override ChunkColumn GenerateSurfaceItems(CyberWorldProvider o, ChunkColumn chunk,
            float[] rth)
        {
            var cc = new TreeGenerator(chunk)
            {
                TreeRandom = 30,
                MaxTreeHeight = 20,

                // MaxTreeWidth = 8
            };
            ((Leaves) cc.LeavesItem).OldLeafType = "jungle";
            ((Wood) cc.WoodItem).WoodType = "jungle";
            var c = cc.run();
            c = new GrassGenerator(c)
            {
                GrassRandom = 12,
                GrassGreaterThan = 9,
                RainMultiplier = 1.3f
                // TreeRandom = 60
            }.run();
            
            return c;
        }
        
        
    }
}