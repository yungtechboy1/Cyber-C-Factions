using System;
using LibNoise.Combiner;
using MiNET.Blocks;
using MiNET.Utils;
using MiNET.Worlds;

namespace CyberCore.WorldGen.Biomes
{
    public class TreeGenerator : StructureGenerartor
    {
        public Random RNDM = new Random();
        public float MinRain = .5f;
        public int TreeRandom = 3;
        public int MaxTreeWidth = 5;
        public int MinTreeWidth = 3;
        public int MinTreeHeight = 6;
        public int MaxTreeHeight = 14;

        private Block LeavesItem = new Leaves
        {
            OldLeafType = "jungle"
        };

        private Block WoodItem = new Wood()
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
                        int fy = Chunk.GetHeight(rx, rz);
                        //ACTUALLY RUN NOW 
                        var w = RNDM.Next(MinTreeWidth, MaxTreeWidth);
                        var h = RNDM.Next(MinTreeHeight, MaxTreeHeight);
                        var v = h - w;
                        var vv = 0;
                        var ffy = fy + h;
                        for (var hh = 0; hh < h; hh++)
                        {
                            c.SetBlock(rx, fy + hh, rz, (Block) WoodItem.Clone());
                            //Bottom Half Leaves
                            if (hh > v)
                            {
                                vv++;
                                var ww = vv;
                                //Vertically Covers The Leaves
                                for (var teir = 1; teir <= ww; teir++)
                                    //
                                for (var teirn = 1; teirn <= teir; teirn++)
                                for (var xx = -teirn; xx <= teirn; xx++)
                                for (var zz = -teirn; zz <= teirn; zz++)
                                {
                                    if (xx == 0 && zz == 0) continue;

                                    if (rx + xx >= 0 && rx + xx < 16 && rz + zz >= 0 && rz + zz < 16)
                                        c.SetBlock(rx + xx, fy + hh, rz + zz, (Block) LeavesItem.Clone());
                                    else
                                        //TODO ADD LOCKING METHOD SO IF TARGET CHUNK HAS BLOCKS THAT NEED TO PLACED THEN IT WILL NOT BE SENT UNTIL BLOCKS HAVE BEEN SET
                                        CyberExperimentalWorldProvider
                                            .AddBlockToBeAddedDuringChunkGeneration(
                                                new ChunkCoordinates((x + xx) >> 4, (z + zz) >> 4),
                                                (Block) LeavesItem.Clone());
                                }
                            }
                        }

                        // Top Leaves
                        for (var vvv = vv; vvv > 0; vvv--)
                        {
                            for (var teir = vvv; teir > 0; teir--)
                            for (var teirn = 1; teirn <= teir; teirn++)
                            for (var xx = -teirn; xx <= teirn; xx++)
                            for (var zz = -teirn; zz <= teirn; zz++)
                            {
                                // if(xx == 0 && zz == 0)continue;

                                if (rx + xx >= 0 && rx + xx < 16 && rz + zz >= 0 && rz + zz < 16)
                                    c.SetBlock(rx + xx, ffy, rz + zz, (Block) LeavesItem.Clone());

                                else
                                    CyberExperimentalWorldProvider
                                        .AddBlockToBeAddedDuringChunkGeneration(
                                            new ChunkCoordinates((x + xx) >> 4, (z + zz) >> 4),
                                            (Block) LeavesItem.Clone());
                            }

                            ffy++;
                        }
                    }
            }

            return Chunk;
        }
    }
}