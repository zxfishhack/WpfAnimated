using System.IO;

namespace LibAPNG
{
    public class IDATChunk : Chunk
    {
        public IDATChunk(Stream ms)
            : base(ms)
        {
        }

        public IDATChunk(Chunk chunk)
            : base(chunk)
        {
        }
    }
}