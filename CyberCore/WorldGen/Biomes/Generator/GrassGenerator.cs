using System;
using MiNET.Blocks;
using MiNET.Utils;
using MiNET.Worlds;

namespace CyberCore.WorldGen.Biomes
{
    public class GrassGenerator : StructureGenerartor
    {
        public Random RNDM = new Random();
        public int GrassRandom = 3;
        public int GrassGreaterThan = 0;
        public float RainMultiplier = 1;


        public GrassGenerator(ChunkColumn c) : base(c)
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
            var runamt = (int) (Math.Ceiling(rain * 10f)*RainMultiplier);
            for (var ttry = 0; ttry < runamt; ttry++)
            {
                int vv = RNDM.Next(0, GrassRandom)+1;
                //1 In TreeRandom Chance
                // Console.WriteLine($"OK 0-{GrassRandom} <= {GrassGreaterThan} == {vv}");
                if (vv <= GrassGreaterThan)
                {
                    //RESET VALUES
                    rx = RNDM.Next(0, 15);
                    rz = RNDM.Next(0, 15);
                    int fy = Chunk.GetHeight(rx, rz);
                    if (!c.GetBlockObject(rx, fy, rz).IsTransparent)
                    {
                        fy++;
                        if (!c.GetBlockObject(rx, fy, rz).IsTransparent)
                        {
                            fy++;
                            if (!c.GetBlockObject(rx, fy, rz).IsTransparent)
                            {
                                Console.WriteLine("AHHH THIS WAS SUPPOSeD TO BE AIR BUT WAS NOT3!!!!!!2");
                            }
                        }

                        continue;
                    }

                    // x = cx * 16 + rx;
                    // z = cz * 16 + rz;


                    //ACTUALLY RUN NOW 
                    c.SetBlock(rx, fy, rz, new Tallgrass());
                }
            }


            return Chunk;
        }
    }
}