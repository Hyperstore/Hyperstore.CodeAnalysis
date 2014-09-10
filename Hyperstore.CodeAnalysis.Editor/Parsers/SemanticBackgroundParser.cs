using Hyperstore.CodeAnalysis.Compilation;
using Hyperstore.CodeAnalysis.Editor.Resolver;
using Hyperstore.CodeAnalysis.Symbols;
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
    class SemanticBackgroundParser : BackgroundParser
    {
        public HyperstoreCompilation Compilation { get; private set; }
        private IDomainSymbol _lastValidDomain;

        public IDomainSymbol LastValidDomain { get { return _lastValidDomain; } set { _lastValidDomain = value; } }

        public SemanticBackgroundParser(ITextBuffer textBuffer, TaskScheduler taskScheduler)
            : base(textBuffer, taskScheduler)
        {
            ReparseDelay = TimeSpan.FromMilliseconds(300);
        }

        protected override void ReParseImpl()
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            var doc = TextBuffer.Properties.GetProperty<ITextDocument>(typeof(ITextDocument));

            //do
            //{
            //var    original = _compiler;
            var snapshot = TextBuffer.CurrentSnapshot;
            var tree = HyperstoreSyntaxTree.ParseText(snapshot.GetText(), doc.FilePath);
            if (Compilation == null)
                Compilation = HyperstoreCompilation.Create(new HyperstoreSyntaxTree[] { tree }, new VSHyperstoreResolver());
            else
                Compilation.AddSyntaxTree(tree);

            var diags = new List<DiagnosticInfo>();
            foreach (var diagnostic in Compilation.Diagnostics)
            {
                var diag = new DiagnosticInfo();
                diag.Span = snapshot.CreateTrackingSpan(new Span(diagnostic.Location.Position, diagnostic.Location.Length), SpanTrackingMode.EdgeExclusive);
                diag.Diagnostic = diagnostic;
                diags.Add(diag);
            }

            var model = Compilation.GetSemanticModel(tree);
            if (model != null && model.Domain != null)
                Interlocked.Exchange(ref _lastValidDomain, model.Domain);

            OnParseComplete(new HyperstoreParseResultEventArgs(diags, snapshot, stopwatch.Elapsed));
        }

        protected override void OnFileRenamed(string oldFilePath, string newFilePath)
        {
            if (Compilation == null)
                return;
            Compilation.RenameSyntaxTree(oldFilePath, newFilePath);
        }

        protected override void OnEditorClosed()
        {
            Dispose();
        }

        protected override void OnEditorShow()
        {
            MarkDirty(false);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            Compilation.Dispose();
        }
    }
}
