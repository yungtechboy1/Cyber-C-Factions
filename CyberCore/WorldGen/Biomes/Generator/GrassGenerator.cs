using System;
using MiNET.Blocks;
using MiNET.Utils;
using MiNET.Worlds;

namespace CyberCore.WorldGen.Biomes
{
    public class GrassGenerator : StructureGenerartor
    {
        public Random RNDM = new Random();
        public bool DoubleGrass = true;
        public int DoubleGrassChance = 10;
        public int GrassRandom = 3;

        private Block LeavesItem = new Leaves
        {
            OldLeafType = "jungle"
        };

        private Block WoodItem = new Wood()
        {
            WoodType = "birch"
        };

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
            var runamt = (int) Math.Ceiling(rain * 10f);
            for (var ttry = 0; ttry < runamt; ttry++)
                //1 In TreeRandom Chance
                if (RNDM.Next(0, GrassRandom) == 0)
                {
                    //RESET VALUES
                    int fy = 0;
                    while (true)
                    {
                        rx = RNDM.Next(0, 15);
                        rz = RNDM.Next(0, 15);
                        fy = Chunk.GetHeight(rx, rz) ;
                        if (c.GetBlockObject(rx, fy, rz).Id == 0)
                        {
                            break;
                        }
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