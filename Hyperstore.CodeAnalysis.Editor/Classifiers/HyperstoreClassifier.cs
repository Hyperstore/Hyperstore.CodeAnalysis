using Hyperstore.CodeAnalysis.Editor.Parsers;
using Microsoft.VisualStudio.Language.StandardClassification;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace Hyperstore.CodeAnalysis.Editor.Classifiers
{
  
    ///-------------------------------------------------------------------------------------------------
    /// <summary>
    ///  Hyperstore classifier.
    /// </summary>
    /// <seealso cref="T:Microsoft.VisualStudio.Text.Classification.IClassifier"/>
    /// <remarks>
    ///   Hyperstore classification types :
    ///   - keyword    : domain extends extern interface enum as def entity relationship implements where compute select check validate command uses  
    ///   - attribute  : [...]  
    ///   - identifier : after def entity|relationship   
    ///   - comment  
    ///   - csharpcode
    /// </remarks>
    ///-------------------------------------------------------------------------------------------------
    internal sealed class HyperstoreClassifier : IClassifier
    {
        private ITextBuffer _buffer;
        private IClassificationTypeRegistryService _classificationTypeRegistry;
        private readonly HyperstoreTokenizer _parser;
        private readonly Dispatcher _dispatcher;

        internal HyperstoreClassifier(ITextBuffer buffer, IClassificationTypeRegistryService classificationTypeRegistry, ITextDocumentFactoryService textDocumentFactoryService)
        {
            _buffer = buffer;
            _classificationTypeRegistry = classificationTypeRegistry;
            _parser = buffer.Properties.GetOrCreateSingletonProperty<HyperstoreTokenizer>(() => new HyperstoreTokenizer()); _buffer.Changed += OnBufferChanged;
            _dispatcher = Dispatcher.CurrentDispatcher;
        }

        void OnBufferChanged(object sender, TextContentChangedEventArgs e)
        {
            Action updateAction = () =>
            {
                try
                {
                    foreach (var change in e.Changes)
                    {
                        var text = e.Before.GetLineFromPosition(change.OldPosition).GetTextIncludingLineBreak();
                        var lexer = new HyperstoreLexer(e.Before, text);
                        if (text.IndexOf("*/") >= 0 || lexer.Scan(false).Tokens.Any(t => t.IsMultiline && t.Kind != TokenKind.Normal))
                        {
                            ClassificationChanged(this, new ClassificationChangedEventArgs(new SnapshotSpan(_buffer.CurrentSnapshot, 0, _buffer.CurrentSnapshot.Length)));
                            return;
                        }

                        text = e.After.GetLineFromPosition(change.NewPosition).GetTextIncludingLineBreak();
                        lexer = new HyperstoreLexer(e.After, text);
                        if (text.IndexOf("*/") >= 0 || lexer.Scan(false).Tokens.Any(t => t.IsMultiline && t.Kind != TokenKind.Normal))
                        {
                            ClassificationChanged(this, new ClassificationChangedEventArgs(new SnapshotSpan(_buffer.CurrentSnapshot, 0, _buffer.CurrentSnapshot.Length)));
                            return;
                        }
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

        public event EventHandler<ClassificationChangedEventArgs> ClassificationChanged;

        public IList<ClassificationSpan> GetClassificationSpans(SnapshotSpan span)
        {
            _parser.EnsuresReady(span.Snapshot);

            Span simpleSpan = span.Span;
            List<ClassificationSpan> classifications = new List<ClassificationSpan>();
            var tokens = _parser.Tokens.Where(t=> t.Kind != TokenKind.Normal).ToList();
            if (tokens != null)
            {
                foreach (var token in tokens)
                {
                    var newSpan = token.Span.GetSpan(span.Snapshot);
                    if (newSpan.IntersectsWith(span))
                        classifications.Add(new ClassificationSpan(newSpan, GetClassificationsType(token.Kind)));
                }
            }
            return classifications;
        }

        private void OnClassificationChanged(SnapshotSpan newSpan)
        {
            var tmp = ClassificationChanged;
            if (tmp != null)
                tmp(this, new ClassificationChangedEventArgs(newSpan));
        }

        private IClassificationType GetClassificationsType(TokenKind kind)
        {
            switch (kind)
            {
                case TokenKind.Keyword:
                    return _classificationTypeRegistry.GetClassificationType(PredefinedClassificationTypeNames.Keyword);
                case TokenKind.Attribute:
                    return _classificationTypeRegistry.GetClassificationType("Hyperstore.Attribute");
                case TokenKind.Comment:
                    return _classificationTypeRegistry.GetClassificationType(PredefinedClassificationTypeNames.Comment);
                case TokenKind.CSharpCode:
                    return _classificationTypeRegistry.GetClassificationType("Hyperstore.CSharpCode");
                case TokenKind.Separator:
                    return _classificationTypeRegistry.GetClassificationType(PredefinedClassificationTypeNames.Operator);
                case TokenKind.String:
                    return _classificationTypeRegistry.GetClassificationType(PredefinedClassificationTypeNames.String);
                default:
                    return null;
            }
        }
    }


}