using Hyperstore.CodeAnalysis.Compilation;
using Hyperstore.CodeAnalysis.Syntax;
using Microsoft.VisualStudio.Text;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Hyperstore.CodeAnalysis.Editor.SyntaxHighlight
{
    class DiagnosticInfo
    {
        public ITrackingSpan Span { get; set; }
        public Diagnostic Diagnostic { get; set; }
    }

    class SemanticBackgroundParser : BackgroundParser
    {
        private HyperstoreCompilation _compilation;


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
            if (_compilation == null)
                _compilation = HyperstoreCompilation.Create(new HyperstoreSyntaxTree[] { tree }, new VSHyperstoreResolver());
            else
                _compilation.AddSyntaxTree(tree);

            var diags = new List<DiagnosticInfo>();
            foreach (var diagnostic in _compilation.Diagnostics)
            {
                var diag = new DiagnosticInfo();
                diag.Span = snapshot.CreateTrackingSpan(new Span(diagnostic.Location.Position, diagnostic.Location.Length), SpanTrackingMode.EdgeExclusive);
                diag.Diagnostic = diagnostic;
                diags.Add(diag);
            }

            OnParseComplete(new HyperstoreParseResultEventArgs(diags, snapshot, stopwatch.Elapsed));
        }

        protected override void OnFileRenamed(string oldFilePath, string newFilePath)
        {
            if (_compilation == null)
                return;
            _compilation.RenameSyntaxTree(oldFilePath, newFilePath);
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
            _compilation.Dispose();
        }
    }
}
