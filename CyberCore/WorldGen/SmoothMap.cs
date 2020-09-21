using System;
using System.Collections.Generic;
using CyberCore.Utils;
using CyberCore.WorldGen.Biomes;
using MiNET.Utils;
using MiNET.Worlds;

namespace CyberCore.WorldGen
{
    public class SmoothingMap
    {
        public int[,] Map = new int[16 * 3 + 2, 16 * 3 + 2];

        public ChunkCoordinates getCenterCords()
        {
            return ZeroCords + new ChunkCoordinates(1, 1);
        }

        public ChunkCoordinates ZeroCords;
        // public ChunkCoordinates CenterCords;

        public SmoothingMap(ChunkCoordinates centerchunkcords, int[,] data = null)
        {
            ZeroCords = centerchunkcords + new ChunkCoordinates(-1, -1);
            if (data != null)
            {
                AddChunk(centerchunkcords, data);
            }
        }

        public int GetMapData(int x, int z)
        {
            return Map[x + 1, z + 1];
        }

        public void AddMapData(int x, int z, int v)
        {
            Map[x + 1, z + 1] = v;
        }

        public int[,] GetChunk(ChunkCoordinates c)
        {
            int[,] data = new int[16, 16];
            int xo = 0;
            int zo = 0;
            int cdx = c.X - ZeroCords.X;
            int cdz = c.Z - ZeroCords.Z;
            xo = cdx * 16;
            zo = cdz * 16;
            for (int z = 0; z < 16; z++)
            {
                for (int x = 0; x < 16; x++)
                {
                    //TODO FIX
                    // Console.WriteLine($"DATA {x} {z} = ");
                    data[x, z] = GetMapData(xo + x, zo + z);
                }
            }

            return data;
        }

        public void AddChunk(ChunkCoordinates c, int[,] data)
        {
            int xo = 0;
            int zo = 0;
            int cdx = c.X - ZeroCords.X;
            int cdz = c.Z - ZeroCords.Z;
            xo = cdx * 16;
            zo = cdz * 16;
            for (int z = zo; z < zo + 16; z++)
            {
                for (int x = xo; x < xo + 16; x++)
                {
                    AddMapData(x, z, data[x - xo, z - zo]);
                    // Map[x, z] = data[x - xo, z - zo];
                }
            }
        }

        public void AddBorderValues(CyberExperimentalWorldProvider ccc)
        {
            int cx = ZeroCords.X * 16 + 1;
            int cz = ZeroCords.Z * 16 + 1;
            for (int z = 1; z < Map.GetLength(1) - 1; z++)
            for (int x = 1; x < Map.GetLength(0) - 1; x++)
            {
                int v = Map[x, z];
                if (v == 0) continue;
                int nvl = Map[x - 1, z];
                int nvr = Map[x + 1, z];
                int nvt = Map[x, z + 1];
                int nvb = Map[x, z - 1];

                if (nvl == 0)
                    Map[x - 1, z] = ccc.getBlockHeight(cx + x - 1, cz + z);
                if (nvr == 0)
                    Map[x + 1, z] = ccc.getBlockHeight(cx + x + 1, cz + z);
                if (nvt == 0)
                    Map[x, z + 1] = ccc.getBlockHeight(cx + x, cz + z + 1);
                if (nvb == 0)
                    Map[x, z - 1] = ccc.getBlockHeight(cx + x, cz + z - 1);
            }
        }

        public void SquareSmooth(int w = 2, bool cel = true)
        {
            for (int z = 0; z < Map.GetLength(1); z++)
            for (int x = 0; x < Map.GetLength(0); x++)
            {
                //  if ( x == 0 || z==0|| (Map[x, z] == 0 || Map[x+1, z] == 0 || Map[x-1, z] == 0 || Map[x, z+1] == 0 || Map[x, z-1] == 0 ||
                //    Map[x+1, z+1] == 0 || Map[x+1, z-1] == 0 || Map[x-1, z+1] == 0 || Map[x-1, z-1] == 0))
                if (Map[x, z] == 0) continue;
                bool zb = false;
                int ah = 0;
                int ac = 0;
                for (int zz = w * -1; zz <= w; zz++)
                {
                    for (int xx = w * -1; xx <= w; xx++)
                    {
                        int tx = x + xx;
                        int tz = z + zz;
                        // if (0 > z + i || f22.GetLength(1) <= z + i) continue; 
                        if (0 > tx || 0 > tz || Map.GetLength(0) <= tx || Map.GetLength(1) <= tz || Map[tx, tz] == 0)
                        {
                            // zb = true;
                            // break;
                            continue;
                        }

                        ac++;
                        ah += Map[tx, tz];
                    }

                    if (zb) break;
                }

                if (zb) continue;

                float alpha = .35f;
                if (cel)
                {
                    int vv = (int) Math.Ceiling(ah / (double) ac);
                    // int vvv = Interpolate(vv, ints[x, z], alpha);
                    int vvv = vv; //Interpolate(vv, ints[x, z], alpha);
                    // Console.WriteLine($"INTERPOLATION VALUE FROM {vv} TO {vvv} WITH #{ints[x, z]} AND A {alpha}");
                    Map[x, z] = vvv;
                }
                else
                    Map[x, z] = ah / ac;
            }
        }


        public bool CanSmooth(int x, int z)
        {
            int v = Map[x, z];
            for (int zz = -1; zz <= 1; zz++)
            for (int xx = -1; xx <= 1; xx++)
            {
                // Console.WriteLine("||| " + xx + "||" + zz);
                int vv = Map[x + xx, z + zz];
                if (vv == 0) return false;
                // Console.WriteLine(vv + " ||| " + xx + " || " + zz+$" {x+xx+1} {z+zz+1} ||||| {x} {z}");
            }

            return true;
        }



        public void SmoothMapV4()
        {
            for (int z = 0; z < Map.GetLength(1); z++)
            {
                SmoothStripx(z);
            }

            for (int x = 1; x < Map.GetLength(0); x++)
            {
                SmoothStripz(x);
            }
        }
        
        private void SmoothStripx(int z)
        {
            for (int i = 1; i < Map.GetLength(0)-1 ; i++)
            {
                // if (i == 0 || i ==  Map.GetLength(0) - 1) continue;
                int lv = Map[i - 1,z];
                int nv = Map[i + 1,z];
                int v = Map[i,z];
                if(lv == 0 || nv == 0 || v == 0) continue;
                //DOWN OR UP
                int du = nv - lv;
                //UP
                if (du > 0)
                {
                    v = lv + 1;
                }
                else if (du < 0)
                {
                    v = lv - 1;
                }
                else v = lv;

                Map[i,z] = v;
            }
        }
        private void SmoothStripz(int x)
        {
            for (int i = 1; i < Map.GetLength(1)-1 ; i++)
            {
                // if (i == 0 || i ==  Map.GetLength(1) - 1) continue;
                int lv = Map[x,i - 1];
                int nv = Map[x,i + 1];
                int v = Map[x,i];
                if(lv == 0 || nv == 0 || v == 0) continue;
                //DOWN OR UP
                int du = nv - lv;
                //UP
                if (du > 0)
                {
                    v = lv + 1;
                }
                else if (du < 0)
                {
                    v = lv - 1;
                }
                else v = lv;

                Map[x,i] = v;
            }
        }

        private int[] Fillstripx(int z)
        {
            List<int> strip = new List<int>();
            for (int a = 0; a < Map.GetLength(0); a++)
            {
                strip.Add(Map[a, z]);
            }

            return strip.ToArray();
        }
        private int[] Fillstripz(int x)
        {
            List<int> strip = new List<int>();
            for (int a = 0; a < Map.GetLength(1); a++)
            {
                strip.Add(Map[x, a]);
            }

            return strip.ToArray();
        }

        
        
        



        public void StripSmooth(int w = 2, bool cel = true)
        {
            for (int z = 0; z < Map.GetLength(1); z++)
            for (int x = 0; x < Map.GetLength(0); x++)
            {
                //  if ( x == 0 || z==0|| (Map[x, z] == 0 || Map[x+1, z] == 0 || Map[x-1, z] == 0 || Map[x, z+1] == 0 || Map[x, z-1] == 0 ||
                //    Map[x+1, z+1] == 0 || Map[x+1, z-1] == 0 || Map[x-1, z+1] == 0 || Map[x-1, z-1] == 0))
                if (Map[x, z] == 0) continue;
                if (!CanSmooth(x, z))
                {
                    continue;
                }

                bool zb = false;
                int ah = 0;
                int ac = 0;
                for (int zz = w * -1; zz <= w; zz++)
                {
                    int tx = x + zz;
                    int tz = z;
                    // if (0 > z + i || f22.GetLength(1) <= z + i) continue; 
                    if (0 > tx || 0 > tz || Map.GetLength(0) <= tx || Map.GetLength(1) <= tz || Map[tx, tz] == 0)
                    {
                        // zb = true;
                        // break;
                        continue;
                    }

                    ac++;
                    ah += Map[tx, tz];
                }

                float alpha = .35f;
                if (cel)
                {
                    int vv = (int) Math.Ceiling(ah / (double) ac);
                    // int vvv = Interpolate(vv, ints[x, z], alpha);
                    int vvv = vv; //Interpolate(vv, ints[x, z], alpha);
                    // Console.WriteLine($"INTERPOLATION VALUE FROM {vv} TO {vvv} WITH #{ints[x, z]} AND A {alpha}");
                    Map[x, z] = vvv;
                }
                else
                    Map[x, z] = ah / ac;
            }


            for (int z = 0; z < Map.GetLength(1); z++)
            for (int x = 0; x < Map.GetLength(0); x++)
            {
                //  if ( x == 0 || z==0|| (Map[x, z] == 0 || Map[x+1, z] == 0 || Map[x-1, z] == 0 || Map[x, z+1] == 0 || Map[x, z-1] == 0 ||
                //    Map[x+1, z+1] == 0 || Map[x+1, z-1] == 0 || Map[x-1, z+1] == 0 || Map[x-1, z-1] == 0))
                if (Map[x, z] == 0) continue;
                if (!CanSmooth(x, z))
                {
                    continue;
                }
                bool zb = false;
                int ah = 0;
                int ac = 0;
                for (int zz = w * -1; zz <= w; zz++)
                {
                    int tx = x;
                    int tz = z + zz;
                    // if (0 > z + i || f22.GetLength(1) <= z + i) continue; 
                    if (0 > tx || 0 > tz || Map.GetLength(0) <= tx || Map.GetLength(1) <= tz || Map[tx, tz] == 0)
                    {
                        // zb = true;
                        // break;
                        continue;
                    }

                    ac++;
                    ah += Map[tx, tz];
                }

                float alpha = .35f;
                if (cel)
                {
                    int vv = (int) Math.Ceiling(ah / (double) ac);
                    // int vvv = Interpolate(vv, ints[x, z], alpha);
                    int vvv = vv; //Interpolate(vv, ints[x, z], alpha);
                    // Console.WriteLine($"INTERPOLATION VALUE FROM {vv} TO {vvv} WITH #{ints[x, z]} AND A {alpha}");
                    Map[x, z] = vvv;
                }
                else
                    Map[x, z] = ah / ac;
            }
        }

        public int[,] SetChunks(CyberWorldProvider cyberExperimentalWorldProvider, bool xtra = true)
        {
            List<AdvancedBiome.BorderChunkDirection> bb = new List<AdvancedBiome.BorderChunkDirection>();
            bb.Add(AdvancedBiome.BorderChunkDirection.North);
            bb.Add(AdvancedBiome.BorderChunkDirection.East);
            bb.Add(AdvancedBiome.BorderChunkDirection.West);
            bb.Add(AdvancedBiome.BorderChunkDirection.South);
            bb.Add(AdvancedBiome.BorderChunkDirection.NE);
            bb.Add(AdvancedBiome.BorderChunkDirection.NW);
            bb.Add(AdvancedBiome.BorderChunkDirection.SW);
            bb.Add(AdvancedBiome.BorderChunkDirection.SE);
            foreach (var b in bb)
            {
                // Console.WriteLine(
                // $"SEEEE DD{b} {b.GetX()} {b.GetZ()} || ZC:{ZeroCords} || CC:{getCenterCords()} || TC: {b.GetX() + getCenterCords().X} {b.GetZ() + getCenterCords().Z} ");
                int[,] data = new int[16, 16];
                int xo = 0;
                int zo = 0;

                ChunkCoordinates tc = new ChunkCoordinates((b.GetX() + getCenterCords().X),
                    (b.GetZ() + getCenterCords().Z));

                data = GetChunk(tc);

                var sischunkcords = tc;
                var sischunkbiome = BiomeManager.GetBiome(sischunkcords);
                // if (sischunkbiome.LocalId == 10) continue;//Water Biome
                var nc = new ChunkColumn();
                nc.X = sischunkcords.X;
                nc.Z = sischunkcords.Z;
                sischunkbiome.GenerateChunkFromSmoothOrder(cyberExperimentalWorldProvider, nc,
                    BiomeManager.getChunkRTH(sischunkcords), data,xtra);
                //TODO
                cyberExperimentalWorldProvider.WP._chunkCache[sischunkcords] = nc;
                // return data;
            }

            return GetChunk(getCenterCords());
        }
    }
}