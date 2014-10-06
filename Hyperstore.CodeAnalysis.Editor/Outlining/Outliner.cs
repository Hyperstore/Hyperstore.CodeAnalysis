using Hyperstore.CodeAnalysis.Editor.Classifiers;
using Hyperstore.CodeAnalysis.Editor.Parser;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace Hyperstore.CodeAnalysis.Editor.Outlining
{
    internal sealed class OutliningTagger : ITagger<IOutliningRegionTag>
    {
        private ITextBuffer _buffer;
        private readonly HyperstoreTokenizer _backgroundParser;
        private readonly Dispatcher _dispatcher;
        private IEnumerable<RegionInfo> _regions;

        internal OutliningTagger(ITextBuffer buffer)
        {
            _buffer = buffer;
            _dispatcher = Dispatcher.CurrentDispatcher;
            _backgroundParser = buffer.Properties.GetOrCreateSingletonProperty<HyperstoreTokenizer>(()=> new HyperstoreTokenizer() );
            _backgroundParser.EnsuresReady(buffer.CurrentSnapshot);
            _regions = _backgroundParser.Regions.ToList();
            _buffer.Changed += OnBufferChanged;
        }

        void OnBufferChanged(object sender, TextContentChangedEventArgs e)
        {
            _backgroundParser.EnsuresReady(e.After);

            ITextSnapshot snapshot = e.After;
            var newRegions = _backgroundParser.Regions.ToList();
            NormalizedSnapshotSpanCollection oldSectionSpans = new NormalizedSnapshotSpanCollection(_regions.Select(s => s.Span.GetSpan(snapshot)));
            NormalizedSnapshotSpanCollection newSectionSpans = new NormalizedSnapshotSpanCollection(newRegions.Select(s => s.Span.GetSpan(snapshot)));
            NormalizedSnapshotSpanCollection difference = SymmetricDifference(oldSectionSpans, newSectionSpans);

            Action updateAction = () =>
            {
                try
                {
                    _regions = newRegions;
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

        public IEnumerable<ITagSpan<IOutliningRegionTag>> GetTags(Microsoft.VisualStudio.Text.NormalizedSnapshotSpanCollection spans)
        {
            if (spans.Count == 0)
                yield break;

            if (_regions == null || _regions.Count() == 0)
                yield break;

            ITextSnapshot snapshot = spans[0].Snapshot;
            foreach (var section in _regions)
            {
                var sectionSpan = section.Span.GetSpan(snapshot);
                if (spans.IntersectsWith(new NormalizedSnapshotSpanCollection(sectionSpan)))
                {
                    string firstLine = sectionSpan.Start.GetContainingLine().GetText().TrimStart(' ', '\t');
                    string collapsedHintText;
                    if (sectionSpan.Length > 250)
                        collapsedHintText = snapshot.GetText(sectionSpan.Start, 247) + "...";
                    else
                        collapsedHintText = sectionSpan.GetText();
                    var tag = new OutliningRegionTag(firstLine, collapsedHintText);
                    yield return new TagSpan<IOutliningRegionTag>(sectionSpan, tag);
                }
            }
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
