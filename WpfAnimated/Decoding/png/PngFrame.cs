using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfAnimated.Decoding.Png
{
    internal class PngFrame : AnimateFrame
    {
        public PngFrame()
        {
            Descriptor = new ImageDescriptor();
            Extensions = new List<AnimateExtension>(1);
        }
    }
}
