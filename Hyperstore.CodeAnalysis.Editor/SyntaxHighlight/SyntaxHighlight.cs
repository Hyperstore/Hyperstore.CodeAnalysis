using Hyperstore.CodeAnalysis.Editor.Classifiers;
using Hyperstore.CodeAnalysis.Editor.Parser;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Adornments;
using Microsoft.VisualStudio.Text.Tagging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace Hyperstore.CodeAnalysis.Editor.SyntaxHighlight
{
    internal sealed class SyntaxHighlightTagger : ITagger<IErrorTag>
    {
        private ITextBuffer _buffer;
        private readonly SemanticBackgroundParser _backgroundParser;
        private readonly Dispatcher _dispatcher;
        private IEnumerable<DiagnosticInfo> _diagnostics;

        internal SyntaxHighlightTagger(ITextBuffer buffer)
        {
            _buffer = buffer;
            _dispatcher = Dispatcher.CurrentDispatcher;
            _backgroundParser = buffer.Properties.GetOrCreateSingletonProperty(() => new SemanticBackgroundParser(buffer, TaskScheduler.Current));
            _backgroundParser.ParseComplete += OnParseCompelete;
            _backgroundParser.RequestParse(true);
        }

        void OnParseCompelete(object sender, ParseResultEventArgs e)
        {
            var args = e as HyperstoreParseResultEventArgs;
            ITextSnapshot snapshot = e.Snapshot;
            var newDiagnostics = args.Diagnostics.ToList();
            NormalizedSnapshotSpanCollection oldSectionSpans = _diagnostics != null ? new NormalizedSnapshotSpanCollection(_diagnostics.Select(s => s.Span.GetSpan(snapshot))) : new NormalizedSnapshotSpanCollection();
            NormalizedSnapshotSpanCollection newSectionSpans = new NormalizedSnapshotSpanCollection(newDiagnostics.Select(s => s.Span.GetSpan(snapshot)));
            NormalizedSnapshotSpanCollection difference = SymmetricDifference(oldSectionSpans, newSectionSpans);

            Action updateAction = () =>
            {
                try
                {
                    _diagnostics = newDiagnostics;
                    foreach (var span in difference)
                    {
                        var temp = TagsChanged;
                        if (temp != null)
                            temp(this, new SnapshotSpanEventArgs(span));
                    }
                }
                catch (Exception)
                {
                    //if (ErrorHandler.IsCriticalException(ex))
                    //    throw;
                }
            };
            _dispatcher.BeginInvoke(updateAction);
        }

        public IEnumerable<ITagSpan<IErrorTag>> GetTags(Microsoft.VisualStudio.Text.NormalizedSnapshotSpanCollection spans)
        {
            if (spans.Count == 0)
                yield break;

            if (_diagnostics == null || _diagnostics.Count() == 0)
            {
                _backgroundParser.ErrorsWindow.ClearErrors();
                yield break;
            }

            ITextSnapshot snapshot = spans[0].Snapshot;
            foreach (var diag in _diagnostics)
            {
                var sectionSpan = diag.Span.GetSpan(snapshot);
                if (spans.IntersectsWith(new NormalizedSnapshotSpanCollection(sectionSpan)))
                {
                    var tag = new ErrorTag(diag.Diagnostic.Severity == DiagnosticSeverity.Error ? PredefinedErrorTypeNames.SyntaxError : PredefinedErrorTypeNames.Warning, diag.Diagnostic.Message);
                    yield return new TagSpan<IErrorTag>(sectionSpan, tag);
                }
            }

            _backgroundParser.ErrorsWindow.WriteErrors(_diagnostics);
        }

        NormalizedSnapshotSpanCollection SymmetricDifference(NormalizedSnapshotSpanCollection first, NormalizedSnapshotSpanCollection second)
        {
            return NormalizedSnapshotSpanCollection.Union(
            NormalizedSnapshotSpanCollection.Difference(first, second),
            NormalizedSnapshotSpanCollection.Difference(second, first));
        }

        public event EventHandler<Microsoft.VisualStudio.Text.SnapshotSpanEventArgs> TagsChanged;
    }
}
