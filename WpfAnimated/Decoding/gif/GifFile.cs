using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfAnimated.Decoding.Gif;

namespace WpfAnimated.Decoding.Gif
{
    internal class GifFile : AnimateFile
    {
        public GifHeader Header { get; private set; }
        public GifColor[] GlobalColorTable { get; set; }
        public IList<GifExtension> GifExtensions { get; set; }

        private GifFile()
        {
        }

        internal static AnimateFile ReadFile(Stream stream, bool metadataOnly)
        {
            var file = new GifFile();
            file.Read(stream, metadataOnly);
            return file;
        }

        private void Read(Stream stream, bool metadataOnly)
        {
            Header = GifHeader.ReadHeader(stream);

            if (Header.LogicalScreenDescriptor.HasGlobalColorTable)
            {
                GlobalColorTable = GifHelpers.ReadColorTable(stream, Header.LogicalScreenDescriptor.GlobalColorTableSize);
            }
            ReadFrames(stream, metadataOnly);

            var netscapeExtension =
                            Extensions
                                .OfType<GifApplicationExtension>()
                                .FirstOrDefault(GifHelpers.IsNetscapeExtension);

            if (netscapeExtension != null)
                RepeatCount = GifHelpers.GetRepeatCount(netscapeExtension);
            else
                RepeatCount = 1;
        }

        private void ReadFrames(Stream stream, bool metadataOnly)
        {
            List<AnimateFrame> frames = new List<AnimateFrame>();
            List<GifExtension> controlExtensions = new List<GifExtension>();
            List<GifExtension> specialExtensions = new List<GifExtension>();
            while (true)
            {
                var block = GifBlock.ReadBlock(stream, controlExtensions, metadataOnly);

                if (block.Kind == GifBlockKind.GraphicRendering)
                    controlExtensions = new List<GifExtension>();

                if (block is GifFrame)
                {
                    frames.Add((AnimateFrame)block);
                }
                else if (block is GifExtension)
                {
                    var extension = (GifExtension)block;
                    switch (extension.Kind)
                    {
                        case GifBlockKind.Control:
                            controlExtensions.Add(extension);
                            break;
                        case GifBlockKind.SpecialPurpose:
                            specialExtensions.Add(extension);
                            break;
                    }
                }
                else if (block is GifTrailer)
                {
                    break;
                }
            }

            this.Frames = frames.AsReadOnly();
            GifExtension.convertExtension(Extensions, specialExtensions);
            this.GifExtensions = specialExtensions.AsReadOnly();
        }

        internal override ImageBehavior.Int32Size? GetFullSize()
        {
            var lsd = Header.LogicalScreenDescriptor;
            return new ImageBehavior.Int32Size(lsd.Width, lsd.Height);
        }

        internal override int? TotalDuration()
        {
            return null;
        }
    }
}
