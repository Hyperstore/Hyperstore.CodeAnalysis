using Microsoft.VisualStudio.Text;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Hyperstore.CodeAnalysis.Editor.Parsers
{
    class RegionInfo
    {
        public ITrackingSpan Span { get; set; }
        public string HoverText { get; set; }

        public RegionType Type { get; set; }
    }
}
