using System;
using MiNET.Blocks;
using MiNET.Worlds;

namespace CyberCore.WorldGen.Biomes
{
    public class TallGrassGenerator : StructureGenerartor
    {
        public Random RNDM = new Random();
        public int GrassRandom = 3;
        public int GrassGreaterThan = 0;


        public TallGrassGenerator(ChunkColumn c) : base(c)
        {
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
                //1 In TreeRandom Chance
                if (RNDM.Next(0, GrassRandom)+1 <= GrassGreaterThan)
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


                    //ACTUALLY RUN NOW 
                    c.SetBlock(rx, fy, rz, new Tallgrass());
                }


            return Chunk;
        }
    }
}