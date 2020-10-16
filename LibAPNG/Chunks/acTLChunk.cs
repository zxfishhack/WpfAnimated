using System.IO;

namespace LibAPNG
{
    public class acTLChunk : Chunk
    {
        public acTLChunk(Stream ms)
            : base(ms)
        {
        }

        public acTLChunk(Chunk chunk)
            : base(chunk)
        {
        }

        public uint NumFrames { get; private set; }

        public uint NumPlays { get; private set; }

        protected override void ParseData(Stream ms)
        {
            NumFrames = Helper.ConvertEndian(ms.ReadUInt32());
            NumPlays = Helper.ConvertEndian(ms.ReadUInt32());
        }
    }
}