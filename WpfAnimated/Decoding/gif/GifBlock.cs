using System.Collections.Generic;
using System.IO;

namespace WpfAnimated.Decoding.Gif
{
    internal abstract class GifBlock : AnimateFrame
    {
        internal static GifBlock ReadBlock(Stream stream, IEnumerable<GifExtension> controlExtensions, bool metadataOnly)
        {
            int blockId = stream.ReadByte();
            if (blockId < 0)
                throw GifHelpers.UnexpectedEndOfStreamException();
            switch (blockId)
            {
                case GifExtension.ExtensionIntroducer:
                    return GifExtension.ReadExtension(stream, controlExtensions, metadataOnly);
                case GifFrame.ImageSeparator:
                    return GifFrame.ReadFrame(stream, controlExtensions, metadataOnly);
                case GifTrailer.TrailerByte:
                    return GifTrailer.ReadTrailer();
                default:
                    throw GifHelpers.UnknownBlockTypeException(blockId);
            }
        }

        internal abstract GifBlockKind Kind { get; }
    }
}
