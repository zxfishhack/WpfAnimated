using System.IO;

namespace LibAPNG
{
    public class OtherChunk : Chunk
    {

        public OtherChunk(Stream ms)
            : base(ms)
        {
        }

        public OtherChunk(Chunk chunk)
            : base(chunk)
        {
        }

        protected override void ParseData(Stream ms)
        {
        }
    }
}