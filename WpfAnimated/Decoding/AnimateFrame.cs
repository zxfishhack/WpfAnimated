using System.Collections.Generic;
using System.IO;
using System.Linq;
using WpfAnimated.Decoding.Gif;

namespace WpfAnimated.Decoding
{
    internal abstract class AnimateFrame
    {
        public ImageDescriptor Descriptor { get; protected set; }
        public IList<AnimateExtension> Extensions { get; protected set; }
    }
}
