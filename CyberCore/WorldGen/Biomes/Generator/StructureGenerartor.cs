using System.Text;
using MiNET.Blocks;
using MiNET.Worlds;

namespace CyberCore.WorldGen.Biomes
{
    public class StructureGenerartor
    {
        public ChunkColumn Chunk;
        

        public StructureGenerartor(ChunkColumn c)
        {
            Chunk = c;
        }

        public virtual ChunkColumn run()
        {
            return Chunk;
        }
    }
}