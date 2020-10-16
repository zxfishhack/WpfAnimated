using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;
using WpfAnimated.Decoding.Gif;
using WpfAnimated.Decoding.Png;

namespace WpfAnimated.Decoding
{
    internal abstract class AnimateFile
    {
        public IList<AnimateFrame> Frames { get; set; }
        public IList<AnimateExtension> Extensions { get; set; }
        public ushort RepeatCount { get; set; }
        public ReadOnlyCollection<BitmapFrame> BitmapFrames { get; set; }
        public BitmapFrame DefaultBitmapFrame { get; protected set; }
        public bool Rendered { get; protected set; }
        public bool IsAnimateFile => Frames.Count > 1;

        internal abstract ImageBehavior.Int32Size? GetFullSize();

        internal abstract int? TotalDuration();
    }

    internal class AnimateFileFactory
    {
        internal static AnimateFile ReadFile(Stream stream, bool metadataOnly)
        {
            var sig = new byte[4];
            if (stream.Read(sig, 0, 4) != 4)
            {
                return null;
            }

            stream.Position = 0;

            if (sig[0] == 'G' && sig[1] == 'I' && sig[2] == 'F')
            {
                return GifFile.ReadFile(stream, metadataOnly);
            }
            else if (sig[0] == 0x89 && sig[1] == 'P' && sig[2] == 'N' && sig[3] == 'G')
            {
                return PngFile.ReadFile(stream, metadataOnly);
            }

            return null;
        }
    }
}
