using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfAnimated.Decoding.Gif;

namespace WpfAnimated.Decoding.Gif
{
    class GifImageDescriptor : ImageDescriptor
    {
        internal static ImageDescriptor ReadImageDescriptor(Stream stream)
        {
            var descriptor = new GifImageDescriptor();
            descriptor.Read(stream);
            return descriptor;
        }

        private void Read(Stream stream)
        {
            byte[] bytes = new byte[9];
            stream.ReadAll(bytes, 0, bytes.Length);
            Left = BitConverter.ToUInt16(bytes, 0);
            Top = BitConverter.ToUInt16(bytes, 2);
            Width = BitConverter.ToUInt16(bytes, 4);
            Height = BitConverter.ToUInt16(bytes, 6);
            byte packedFields = bytes[8];
            HasLocalColorTable = (packedFields & 0x80) != 0;
            Interlace = (packedFields & 0x40) != 0;
            IsLocalColorTableSorted = (packedFields & 0x20) != 0;
            LocalColorTableSize = 1 << ((packedFields & 0x07) + 1);
        }
    }
}
