using Hyperstore.CodeAnalysis.Compilation;
using Hyperstore.CodeAnalysis.Editor.Resolver;
using Hyperstore.CodeAnalysis.Syntax;
using Microsoft.VisualStudio.Text;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Hyperstore.CodeAnalysis.Editor.Parser
{
    class DiagnosticInfo
    {
        public ITrackingSpan Span { get; set; }
        public Diagnostic Diagnostic { get; set; }
    }
}
