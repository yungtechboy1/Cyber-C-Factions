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
        }

        public override int GetSh(int x, int z, int cx, int cz)
        {
            return BiomeQualifications.Baseheight +15+
                   (int) GetNoise(cx * 16 + x, cz * 16 + z, /*rth[2] / */.035f,
                       BiomeQualifications.Heightvariation);
        }

        public override void GenerateVerticalColumn(int yheight, int maxheight, int x, int z, ChunkColumn cc, bool setair)
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
            // Console.WriteLine($"TRYINGGGG FORRR TEEEEEEEEEEEEEEEEEEEEEEEEEEEE {cx} {cz}|| {x} {z}");
            var c = chunk;
            var ffy = 0;

            var cx = chunk.X;
            var cz = chunk.Z;
            var rx = new Random().Next(0, 15);
            var rz = new Random().Next(0, 15);
            var x = cx * 16 + rx;
            var z = cz * 16 + rz;
            int fy = chunk.GetHeight(rx, rz);
            var max = 20;
            var rain = BiomeManager.GetRainNoiseBlock(x, z) * 1.25f;
            if (rain > 1)
            {
                // ccc++;
                var runamt = (int) Math.Ceiling(rain * 10f);
                // Console.WriteLine($"TRY AMOUNT IS {runamt}<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<");
                for (var ttry = 0; ttry < runamt; ttry++)
                    //1 In 16 Chance
                    if (new Random().Next(0, 16) == 0)
                    {
                        //RESET VALUES
                        rx = new Random().Next(0, 15);
                        rz = new Random().Next(0, 15);
                        x = cx * 16 + rx;
                        z = cz * 16 + rz;
                        fy = chunk.GetHeight(rx, rz);
                        //ACTUALLY RUN NOW 
                        var w = RNDM.Next(3, 5);
                        var h = RNDM.Next(6, 14);
                        var v = h - w;
                        var vv = 0;
                        ffy = fy + h;
                        for (var hh = 1; hh < h; hh++)
                        {
                            c.SetBlock(rx, fy + hh, rz, new Wood
                            {
                                WoodType = "birch"
                            });
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
                                        // Log.Error($"PUTTIN LEAVES AT {rx+xx} , {fy+ hh} , {rz+zz} || REAL {x+xx} , {fy+ hh} , {z+zz} || {cx} {cz}");
                                        c.SetBlock(rx + xx, fy + hh, rz + zz, new Leaves
                                        {
                                            OldLeafType = "jungle"
                                        });
                                    else
                                        // Log.Error($"PUTTIN LATEEEEEEEEEEEEE LEAVES AT {x+xx} , {fy+ hh} , {z+zz} || {cx} {cz} || {(x+xx >> 4)} {z+zz >> 4} || X&Z {xx} {zz} || {(x+xx)%16}  {(z+zz)%16} ");

                                        CyberExperimentalWorldProvider
                                            .AddBlockToBeAddedDuringChunkGeneration(
                                                new ChunkCoordinates((x + xx) >> 4, (z + zz) >> 4), new Leaves
                                                {
                                                    OldLeafType = "jungle",
                                                    Coordinates = new BlockCoordinates(x + xx, fy + hh, z + zz)
                                                });
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
                                    c.SetBlock(rx + xx, ffy, rz + zz, new Leaves
                                    {
                                        OldLeafType = "jungle"
                                    });

                                else
                                    CyberExperimentalWorldProvider
                                        .AddBlockToBeAddedDuringChunkGeneration(
                                            new ChunkCoordinates((x + xx) >> 4, (z + zz) >> 4), new Leaves
                                            {
                                                OldLeafType = "jungle",
                                                Coordinates = new BlockCoordinates(x + xx, ffy, z + zz)
                                            });
                            }

                            ffy++;
                        }

                        for (var teir = 0; teir <= v; teir++)
                        {
                        }
                    }
            }

            return chunk;
        }

        
        
    }
}