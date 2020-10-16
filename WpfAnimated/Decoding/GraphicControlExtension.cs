using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfAnimated.Decoding
{
    internal class GraphicControlExtension : AnimateExtension
    {
        public int Delay { get; set; }
        public int DisposalMethod { get; set; }
        public int BlendOption { get; set; }
    }
}
