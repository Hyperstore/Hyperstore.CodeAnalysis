using Hyperstore.CodeAnalysis.Compilation;
using Microsoft.VisualStudio.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hyperstore.CodeAnalysis.Editor.Parser
{
    class HyperstoreParseResultEventArgs : ParseResultEventArgs
    {
        public IEnumerable<DiagnosticInfo> Diagnostics { get; private set; }


        public HyperstoreParseResultEventArgs(IEnumerable<DiagnosticInfo> diagnostics, ITextSnapshot snapshot, TimeSpan elapsedTime)
            : base(snapshot, elapsedTime)
        {
            Diagnostics = diagnostics.ToList();
        }
    }
}
