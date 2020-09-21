using System;
using MiNET.Blocks;
using MiNET.Utils;
using MiNET.Worlds;

namespace CyberCore.WorldGen.Biomes
{
    public class TreeGenerator : StructureGenerartor
    {
        public Block LeavesItem = new Leaves
        {
            OldLeafType = "jungle"
        };

        public int MaxTreeHeight = 14;
        public int MaxTreeWidth = 5;
        public float MinRain = .5f;
        public int MinTreeHeight = 6;
        public int MinTreeWidth = 3;
        public Random RNDM = new Random();
        public int TreeRandom = 3;

        public Block WoodItem = new Wood
        {
            WoodType = "birch"
        };

        public TreeGenerator(ChunkColumn c) : base(c)
        {
        }

        public override ChunkColumn run()
        {
            // Console.WriteLine($"TRYINGGGG FORRR TEEEEEEEEEEEEEEEEEEEEEEEEEEEE {cx} {cz}|| {x} {z}");
            var c = Chunk;

            var cx = Chunk.X;
            var cz = Chunk.Z;
            var rx = new Random().Next(0, 15);
            var rz = new Random().Next(0, 15);
            var x = cx * 16 + rx;
            var z = cz * 16 + rz;
            var rain = BiomeManager.GetRainNoiseBlock(x, z) * 1.25f;
            if (rain > MinRain)
            {
                var runamt = (int) Math.Ceiling(rain * 10f);
                for (var ttry = 0; ttry < runamt; ttry++)
                    //1 In TreeRandom Chance
                    if (new Random().Next(0, TreeRandom) == 0)
                    {
                        //RESET VALUES
                        rx = new Random().Next(0, 15);
                        rz = new Random().Next(0, 15);
                        x = cx * 16 + rx;
                        z = cz * 16 + rz;
                        int bottomy = Chunk.GetHeight(rx, rz);
                        //ACTUALLY RUN NOW 
                        var w = RNDM.Next(MinTreeWidth, MaxTreeWidth);
                        var h = RNDM.Next(MinTreeHeight, MaxTreeHeight);
                        var startleavesheight = h - w;
                        var currentyleavesheight = 0;
                        var ffy = bottomy + h;
                        for (var treey = 0; treey < h; treey++)
                        {
                            c.SetBlock(rx, bottomy + treey, rz, (Block) WoodItem.Clone());
                            //Bottom Half Leaves
                            if (treey > startleavesheight)
                            {
                                currentyleavesheight++;
                                //Vertically Covers The Leaves
                                for (var teir = 1; teir <= currentyleavesheight; teir++) //1-5
                                    //Fill Teir with leaves
                                    // for (var teirfill = 1; teirfill <= teir; teirfill++)
                                for (var xx = -teir; xx <= teir; xx++)
                                for (var zz = -teir; zz <= teir; zz++)
                                {
                                    if (xx == 0 && zz == 0) continue; //Skip Center

                                    if (rx + xx >= 0 && rx + xx < 16 && rz + zz >= 0 && rz + zz < 16)
                                    {
                                        c.SetBlock(rx + xx, bottomy + treey, rz + zz, (Block) LeavesItem.Clone());
                                    }
                                    else
                                    {
                                        //TODO ADD LOCKING METHOD SO IF TARGET CHUNK HAS BLOCKS THAT NEED TO PLACED THEN IT WILL NOT BE SENT UNTIL BLOCKS HAVE BEEN SET
                                        Block a = (Block) LeavesItem.Clone();
                                        a.Coordinates = new BlockCoordinates(rx + xx, bottomy + treey, rz + zz);
                                        CyberExperimentalWorldProvider
                                            .AddBlockToBeAddedDuringChunkGeneration(
                                                new ChunkCoordinates((x + xx) >> 4, (z + zz) >> 4),
                                                (Block) a);
                                    }
                                }
                            }
                        }

                        // Top Leaves
                        for (var topleavesheight = currentyleavesheight; topleavesheight > 0; topleavesheight--) //5
                        {
                            for (var teirlevel = topleavesheight; teirlevel > 0; teirlevel--) //5-0
                                // for (var teirn = 1; teirn <= teirlevel; teirn++)
                            for (var xx = -teirlevel; xx <= teirlevel; xx++)
                            for (var zz = -teirlevel; zz <= teirlevel; zz++)
                                // if(xx == 0 && zz == 0)continue;

                                if (rx + xx >= 0 && rx + xx < 16 && rz + zz >= 0 && rz + zz < 16)
                                {
                                    c.SetBlock(rx + xx, ffy, rz + zz, (Block) LeavesItem.Clone());
                                    // if (xx == Math.Abs(teirlevel) && zz == Math.Abs(teirlevel))
                                    if (xx == teirlevel || xx == -teirlevel || zz == teirlevel|| zz == -teirlevel || topleavesheight ==1)
                                        if(BiomeManager.GetRainNoiseBlock((x+xx), (z+zz))/2 <= .42f)
                                            c.SetBlock(rx + xx, ffy+1, rz + zz, new SnowLayer());
                                }
                                else
                                {
                                    Block a = (Block) LeavesItem.Clone();
                                    a.Coordinates = new BlockCoordinates(x + xx, ffy, z + zz);
                                    CyberExperimentalWorldProvider
                                        .AddBlockToBeAddedDuringChunkGeneration(
                                            new ChunkCoordinates((x + xx) >> 4, (z + zz) >> 4),
                                            (Block) a);
                                    // if ((xx == teirlevel || xx == -teirlevel) && ( zz == teirlevel|| zz == -teirlevel))
                                    if (xx == teirlevel || xx == -teirlevel || zz == teirlevel|| zz == -teirlevel|| topleavesheight ==1)
                                    {
                                        var sl = new SnowLayer();
                                        sl.Coordinates = new BlockCoordinates(x + xx, ffy + 1, z + zz);
                                        CyberExperimentalWorldProvider
                                            .AddBlockToBeAddedDuringChunkGeneration(
                                                new ChunkCoordinates((x + xx) >> 4, (z + zz) >> 4),
                                                sl);
                                    }
                                }

                            ffy++;
                        }
                    }
            }

            return Chunk;
        }
    }
}