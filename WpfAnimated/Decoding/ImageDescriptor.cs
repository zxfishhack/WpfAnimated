using System;
using System.IO;

namespace WpfAnimated.Decoding
{
    internal class ImageDescriptor
    {
        public int Left { get; set; }
        public int Top { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public bool HasLocalColorTable { get; set; }
        public bool Interlace { get; set; }
        public bool IsLocalColorTableSorted { get; set; }
        public int LocalColorTableSize { get; set; }
    }
}
