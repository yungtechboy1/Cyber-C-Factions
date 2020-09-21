using System;
using System.Collections.Generic;
using MiNET.Blocks;
using MiNET.Items;
using MiNET.Worlds;

namespace CyberCore.WorldGen.Biomes
{
    public class FlowerGenerator : StructureGenerartor
    {
        public Random RNDM = new Random();
        public int FlowerRandom = 3;
        public int FlowerGreaterThan = 0;

        public Dictionary<Block, int> FlowerChances = new Dictionary<Block, int>()
        {
            {new RedFlower(), 10}
        };


        public FlowerGenerator(ChunkColumn c) : base(c)
        {
        }

        public Block getFlowerFromChances()
        {
            foreach (KeyValuePair<Block, int> a in FlowerChances)
            {
                Block b = a.Key;
                int v = a.Value;
                int vv = RNDM.Next(0, v)+1;
                if (vv == v)
                    return b;
                // else
                // {
                //     Console.WriteLine($" CHANCESSSSS {0}-{v} || {v} == {vv}");
                // }
            }

            return null;
        }

        public override ChunkColumn run()
        {
            // Console.WriteLine($"TRYINGGGG FORRR TEEEEEEEEEEEEEEEEEEEEEEEEEEEE {cx} {cz}|| {x} {z}");
            var c = Chunk;

            var cx = Chunk.X;
            var cz = Chunk.Z;
            var rx = RNDM.Next(0, 15);
            var rz = RNDM.Next(0, 15);
            var x = cx * 16 + rx;
            var z = cz * 16 + rz;
            var rain = BiomeManager.GetRainNoiseBlock(x, z) * 1.25f;
            var runamt = (int) Math.Ceiling(rain * 10f);
            for (var ttry = 0; ttry < runamt; ttry++)
            {
                //1 In TreeRandom Chance
                if (RNDM.Next(0, FlowerRandom)+1 <= FlowerGreaterThan)
                {
                    //RESET VALUES
                    rx = RNDM.Next(0, 15);
                    rz = RNDM.Next(0, 15);
                    int fy = Chunk.GetHeight(rx, rz);
                    if (c.GetBlockObject(rx, fy, rz).Id != 0)
                    {
                        fy++;
                        if (c.GetBlockObject(rx, fy, rz).Id != 0)
                        {
                            Console.WriteLine("AHHH THIS WAS SUPPOSeD TO BE AIR BUT WAS NOT!!!!!!2");
                            continue;
                        }

                        continue;
                    }

                    // x = cx * 16 + rx;
                    // z = cz * 16 + rz;

                    var b = getFlowerFromChances();
                    if (b == null)
                    {
                        continue;
                    }
                    //ACTUALLY RUN NOW 
                    c.SetBlock(rx, fy, rz, b);
                }
            }


            return Chunk;
        }
    }
}