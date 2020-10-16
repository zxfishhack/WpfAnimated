using System.IO;

namespace LibAPNG
{
    public class IENDChunk : Chunk
    {
        public IENDChunk(Stream ms)
            : base(ms)
        {
        }

        public IENDChunk(Chunk chunk)
            : base(chunk)
        {
        }
    }
}