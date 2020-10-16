using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using LibAPNG;

namespace WpfAnimated.Decoding.Png
{
    internal class PngFile : AnimateFile
    {
        private int _width = 0, _height = 0;
        private int _total = 0;
        internal override ImageBehavior.Int32Size? GetFullSize()
        {
            return new ImageBehavior.Int32Size(_width, _height);
        }

        private static int Align(int width, int size)
        {
            return (width + size - 1) / size* size;
        }

        internal static AnimateFile ReadFile(Stream stream, bool metadataOnly)
        {
            var file = new PngFile();
            file.Read(stream, metadataOnly);
            return file;
        }

        internal override int? TotalDuration()
        {
            if (_total == 0)
            {
                return null;
            }
            return _total;
        }

        private void Read(Stream stream, bool metadataOnly)
        {
            var apng = new APNG(stream);
            _width = apng.IHDRChunk.Width;
            _height = apng.IHDRChunk.Height;
            RepeatCount = (ushort)apng.acTLChunk.NumPlays;
            var collection = new List<BitmapFrame>();
            var frames = new List<AnimateFrame>();
            double total = 0;
            var fastGen = true;
            foreach (var pngFrame in apng.Frames)
            {
                double delay = 1000.0 * pngFrame.fcTLChunk.DelayNum / pngFrame.fcTLChunk.DelayDen;
                total += delay;
                var frame = new PngFrame();
                frame.Descriptor.Width = (int)pngFrame.fcTLChunk.Width;
                frame.Descriptor.Height = (int)pngFrame.fcTLChunk.Height;
                frame.Descriptor.Top = (int)pngFrame.fcTLChunk.YOffset;
                frame.Descriptor.Left = (int)pngFrame.fcTLChunk.XOffset;
                var gce = new GraphicControlExtension();
                gce.Delay = (int)delay;
                switch (pngFrame.fcTLChunk.DisposeOp)
                {
                    case DisposeOps.APNGDisposeOpBackground:
                        gce.DisposalMethod = (int)ImageBehavior.FrameDisposalMethod.RestoreBackground;
                        break;
                    case DisposeOps.APNGDisposeOpNone:
                        gce.DisposalMethod = (int)ImageBehavior.FrameDisposalMethod.None;
                        break;
                    case DisposeOps.APNGDisposeOpPrevious:
                        gce.DisposalMethod = (int)ImageBehavior.FrameDisposalMethod.RestorePrevious;
                        fastGen = false;
                        break;
                }

                switch (pngFrame.fcTLChunk.BlendOp)
                {
                    case BlendOps.APNGBlendOpSource:
                        gce.BlendOption = (int)ImageBehavior.BlendOption.BlendOpSource;
                        break;
                    case BlendOps.APNGBlendOpOver:
                        gce.BlendOption = (int)ImageBehavior.BlendOption.BlendOpOver;
                        fastGen = false;
                        break;
                }

                frame.Extensions.Add(gce);
                frames.Add(frame);
            }

            if (fastGen)
            {
                var stride = Align(apng.IHDRChunk.Width, 8) * 4;
                var bgdPxl = new byte[apng.IHDRChunk.Height * stride];
                var framePxl = new byte[apng.IHDRChunk.Height * stride];
                var inited = false;
                for (var i = 0; i < apng.Frames.Length; i++)
                {
                    var pngFrame = apng.Frames[i];
                    var bitmap = BitmapFrame.Create(pngFrame.GetStream(), BitmapCreateOptions.None, BitmapCacheOption.None);
                    Array.Copy(bgdPxl, framePxl, framePxl.Length);
                    bitmap.CopyPixels(framePxl, stride, (int)(pngFrame.fcTLChunk.YOffset * stride + pngFrame.fcTLChunk.XOffset * 4));
                    collection.Add(BitmapFrame.Create(BitmapSource.Create(apng.IHDRChunk.Width, apng.IHDRChunk.Height, 96d, 96d, PixelFormats.Bgra32, null, framePxl, stride)));

                    if (pngFrame.fcTLChunk.DisposeOp == DisposeOps.APNGDisposeOpNone)
                    {
                        var t = framePxl;
                        framePxl = bgdPxl;
                        bgdPxl = t;
                    }

                    frames[i].Descriptor.Width = apng.IHDRChunk.Width;
                    frames[i].Descriptor.Height = apng.IHDRChunk.Height;
                    frames[i].Descriptor.Top = 0;
                    frames[i].Descriptor.Left = 0;
                }

                Rendered = true;
            }
            else
            {
                foreach (var pngFrame in apng.Frames)
                {
                    var bitmap = BitmapFrame.Create(pngFrame.GetStream(), BitmapCreateOptions.None, BitmapCacheOption.None);
                    collection.Add(bitmap);
                }
            }

            _total = (int) total;

            Frames = frames;

            BitmapFrames = new ReadOnlyCollection<BitmapFrame>(collection);
        }
    }
}
