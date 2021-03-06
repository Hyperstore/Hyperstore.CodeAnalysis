﻿using Hyperstore.CodeAnalysis.Compilation;
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
        private Lazy<HyperstoreCompilation> _compilation = new Lazy<HyperstoreCompilation>(() => HyperstoreCompilation.Create(null, null, new VSHyperstoreResolver()));
        public HyperstoreCompilation Compilation { get { return _compilation.Value; } }
        private IDomainSymbol _lastValidDomain;

        public IDomainSymbol LastValidDomain { get { return _lastValidDomain; } set { _lastValidDomain = value; } }

        public SemanticBackgroundParser(ITextBuffer textBuffer, TaskScheduler taskScheduler)
            : base(textBuffer, taskScheduler)
        {
            ReparseDelay = TimeSpan.FromMilliseconds(500);
        }

        protected override void ReParseImpl()
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            var doc = TextBuffer.Properties.GetProperty<ITextDocument>(typeof(ITextDocument));

            var snapshot = TextBuffer.CurrentSnapshot;
            var tree = HyperstoreSyntaxTree.ParseText(snapshot.GetText(), doc.FilePath);
            Compilation.AddSyntaxTree(tree);

            var model = Compilation.GetSemanticModel(tree);
            var diags = new List<DiagnosticInfo>();
            foreach (var diagnostic in (model != null ? model.GetDiagnostics() : tree.GetDiagnostics()))
            {
                var diag = new DiagnosticInfo();
                diag.Span = snapshot.CreateTrackingSpan(new Span(diagnostic.Location.SourceSpan.Start, diagnostic.Location.SourceSpan.Length), SpanTrackingMode.EdgeExclusive);
                diag.Diagnostic = diagnostic;
                diags.Add(diag);
            }

            var m = Compilation.GetMergedDomains().FirstOrDefault(d => d.Locations.Any(l => l.SyntaxTree == tree));
            if (m != null)
                Interlocked.Exchange(ref _lastValidDomain, m);

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
            if (Compilation != null)
                Compilation.Dispose();
        }
    }
}
