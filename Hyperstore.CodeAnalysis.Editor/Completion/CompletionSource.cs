using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.TextManager.Interop;
using Hyperstore.CodeAnalysis.Editor.Parsers;
using System.Threading.Tasks;
using Hyperstore.CodeAnalysis.Editor.Parser;
using Hyperstore.CodeAnalysis.Compilation;
using Hyperstore.CodeAnalysis.Symbols;

namespace Hyperstore.CodeAnalysis.Editor.Completion
{
    /// <summary> 
    /// Implementation of <see cref="ICompletionSource"/>. Provides the completion sets for the editor.  
    /// </summary> 
    internal class CompletionSource : ICompletionSource
    {
        internal static string CompletionSetName = "IronPython Completion";

        private ITextBuffer textBuffer;
        private IGlyphService glyphService;
        private IDomainSymbol _domain;
        private HyperstoreCompilation _compilation;
        private readonly HyperstoreTokenizer _tokenizer;

        internal CompletionSource(ITextBuffer textBuffer, IGlyphService glyphService)
        {
            this.textBuffer = textBuffer;
            this.glyphService = glyphService;
            var backgroundParser = textBuffer.Properties.GetOrCreateSingletonProperty(() => new SemanticBackgroundParser(textBuffer, TaskScheduler.Current));
            _domain = backgroundParser.LastValidDomain;
            _compilation = backgroundParser.Compilation;
            _tokenizer = textBuffer.Properties.GetOrCreateSingletonProperty<HyperstoreTokenizer>(() => new HyperstoreTokenizer());
        }

        private IEnumerable<Declaration> BuildCompletionNodes(ITrackingPoint position, ITextSnapshot snapshot, char currentKey)
        {
            _tokenizer.EnsuresReady(snapshot);
            var regions = _tokenizer.Regions;
            var tokens = _tokenizer.Tokens;

            var declarations = new List<Declaration>();
            var span = new Span(position.GetPosition(snapshot), 1);

            var region = regions.FirstOrDefault(r => span.OverlapsWith(r.Span.GetSpan(snapshot)));
            var token = tokens.LastOrDefault(t => t.Span.GetSpan(snapshot).Start < span.Start);

            bool beingEditIdentifier = !Char.IsWhiteSpace(currentKey);
            if (beingEditIdentifier && token != null && token.Kind != TokenKind.CSharpCode)
            {
                token = tokens.GetPreviousToken(token);
            }

            if (region == null)
            {
                if (token == null)
                {
                    declarations.Add(new Declaration { Title = "domain", Type = DeclarationType.Keyword });
                    return declarations;
                }

                if (token.Kind != TokenKind.CSharpCode)
                {
                    switch (token.Value)
                    {
                        case "def":
                            declarations.Add(new Declaration { Title = "entity", Type = DeclarationType.Keyword });
                            declarations.Add(new Declaration { Title = "enum", Type = DeclarationType.Keyword });
                            declarations.Add(new Declaration { Title = "relationship", Type = DeclarationType.Keyword });
                            return declarations;

                        case "extern":
                            declarations.Add(new Declaration { Title = "interface", Type = DeclarationType.Keyword });
                            return declarations;

                        case "domain":
                        case "entity":
                        case "relationship":
                        case "enum":
                        case "uses":
                        case "as":
                        case "interface":
                        case "select":
                        case "where":
                        case "compute":
                            return declarations;
                    }


                    var definitionState = tokens.IsInDefinition(token);
                    if (definitionState == DefinitionState.None)
                    {
                        declarations.Add(new Declaration { Title = "def", Type = DeclarationType.Keyword });
                        declarations.Add(new Declaration { Title = "extern", Type = DeclarationType.Keyword });
                        declarations.Add(new Declaration { Title = "uses", Type = DeclarationType.Keyword });
                    }
                    else if (token.Kind != TokenKind.Keyword)
                    {
                        if (definitionState == DefinitionState.Extern || definitionState == DefinitionState.Uses)
                        {
                            declarations.Add(new Declaration { Title = "as", Type = DeclarationType.Keyword });
                        }
                        else if (definitionState != DefinitionState.Enum)
                        {
                            if ((definitionState & DefinitionState.WithExtends) != DefinitionState.WithExtends)
                                declarations.Add(new Declaration { Title = "extends", Type = DeclarationType.Keyword });
                            if ((definitionState & DefinitionState.WithImplements) != DefinitionState.WithImplements)
                                declarations.Add(new Declaration { Title = "implements", Type = DeclarationType.Keyword });
                        }
                    }
                    else if (token.Kind == TokenKind.Keyword)
                    {
                        if (token.Value == "implements")
                            BuildInterfaceDeclarations(declarations);
                        else
                            BuildTypeDeclarations(declarations, true);
                    }
                    return declarations;
                }


            }
            else
            {
                if (token != null && token.Kind != TokenKind.CSharpCode)
                {
                    if (token.Value == "check" || token.Value == "validate")
                    {
                        declarations.Add(new Declaration { Title = "error", Type = DeclarationType.Keyword });
                        declarations.Add(new Declaration { Title = "warning", Type = DeclarationType.Keyword });
                        return declarations;
                    }

                    var definitionState = tokens.IsInDefinition(token);
                    if (definitionState != DefinitionState.None)
                    {
                        if (token.Kind != TokenKind.Keyword)
                        {
                            if (definitionState == DefinitionState.Extern || definitionState == DefinitionState.Uses)
                            {
                                declarations.Add(new Declaration { Title = "as", Type = DeclarationType.Keyword });
                            }
                            else if (definitionState != DefinitionState.Enum)
                            {
                                if ((definitionState & DefinitionState.WithExtends) != DefinitionState.WithExtends)
                                    declarations.Add(new Declaration { Title = "extends", Type = DeclarationType.Keyword });
                                if ((definitionState & DefinitionState.WithImplements) != DefinitionState.WithImplements)
                                    declarations.Add(new Declaration { Title = "implements", Type = DeclarationType.Keyword });
                            }
                        }
                        else
                        {
                            if (token.Value == "implements")
                                BuildInterfaceDeclarations(declarations);
                            else
                                BuildTypeDeclarations(declarations, true);
                        }
                    }
                    else if (_domain != null)
                    {
                        // def xxxx {
                        //   Name : xxx -><-
                        if (token.Kind == TokenKind.Normal)
                        {
                            declarations.Add(new Declaration { Title = "=", Type = DeclarationType.Keyword });
                            declarations.Add(new Declaration { Title = "check", Type = DeclarationType.Keyword });
                            declarations.Add(new Declaration { Title = "validate", Type = DeclarationType.Keyword });
                        }
                        else
                        {
                            switch (token.Value)
                            {
                                case ":":
                                    var prv = tokens.GetPreviousToken(token);
                                    if (prv == null || prv.Kind == TokenKind.String)
                                        break;
                                    goto case "=>";
                                case "=>":
                                case "<-":
                                case "->":
                                case "<=":
                                    BuildTypeDeclarations(declarations, false);
                                    break;
                            }
                        }
                    }
                }
                else
                {
                    // def xxxx {
                    //   Name : xxx = 0 -><-
                    declarations.Add(new Declaration { Title = "check", Type = DeclarationType.Keyword });
                    declarations.Add(new Declaration { Title = "validate", Type = DeclarationType.Keyword });
                }
            }

            return declarations;
        }

        private void BuildInterfaceDeclarations(List<Declaration> declarations, IDomainSymbol domain = null, string alias = null)
        {
            foreach (var elem in (domain ?? _domain).Elements)
            {
                if (elem is IExternSymbol && ((IExternSymbol)elem).Kind == ExternalKind.Interface)
                {
                    declarations.Add(new Declaration { Title = alias == null ? elem.Name : String.Format("{0}.{1}", alias, elem.Name), Type = DeclarationType.Type });
                }
            }

            if (alias != null)
                return;

            foreach (var uses in (domain ?? _domain).Usings)
            {
                var domainUsed = _compilation.ResolveDomain(uses);
                if (domainUsed != null && domainUsed.Domain != null)
                    BuildInterfaceDeclarations(declarations, domainUsed.Domain, uses.Name);
            }
        }

        private void BuildTypeDeclarations(List<Declaration> declarations, bool forExtends, IDomainSymbol domain = null, string alias = null)
        {
            foreach (var elem in (domain ?? _domain).Elements)
            {
                if (elem is IEntitySymbol || elem is IRelationshipSymbol || elem is IExternSymbol)
                {
                    if (forExtends && elem is IExternSymbol)
                    {
                        var ext = elem as IExternSymbol;
                        if (ext.Kind != ExternalKind.Normal)
                            continue;
                    }
                    declarations.Add(new Declaration { Title = alias == null ? elem.Name : String.Format("{0}.{1}", alias, elem.Name), Type = DeclarationType.Type });
                }
            }

            if (alias != null)
                return;

            foreach (var elem in _compilation.GetPrimitives())
            {
                declarations.Add(new Declaration { Title = elem.Name, Type = DeclarationType.Primitive });
            }

            foreach (var uses in (domain ?? _domain).Usings)
            {
                var domainUsed = _compilation.ResolveDomain(uses);
                if (domainUsed != null && domainUsed.Domain != null)
                    BuildTypeDeclarations(declarations, forExtends, domainUsed.Domain, uses.Name);
            }
        }

        #region ICompletionSource Members

        void ICompletionSource.AugmentCompletionSession(ICompletionSession session, IList<CompletionSet> completionSets)
        {
            var position = session.GetTriggerPoint(session.TextView.TextBuffer);
            //            int line = textBuffer.CurrentSnapshot.GetLineNumberFromPosition(position);
            //            int column = position - textBuffer.CurrentSnapshot.GetLineFromPosition(position).Start.Position;

            completionSets.Add(GetCompletions(BuildCompletionNodes(position, session.TextView.TextSnapshot, session.Properties.GetProperty<char>("currentKey")), session));
        }

        /// <summary> 
        /// Get a piece of text of a given line in a text buffer 
        /// </summary> 
        public string TextOfLine(ITextBuffer textBuffer, int lineNumber, int endColumn, out int start, bool skipReadOnly)
        {
            start = 0;
            var line = textBuffer.CurrentSnapshot.GetLineFromLineNumber(lineNumber);

            if (textBuffer.IsReadOnly(line.Extent.Span))
            {
                start = GetReadOnlyLength(textBuffer.CurrentSnapshot) - line.Start;
            }

            return line.GetText().Substring(start, endColumn - start);
        }

        private int GetReadOnlyLength(ITextSnapshot textSnapshot)
        {
            return textSnapshot.TextBuffer.GetReadOnlyExtents(new Span(0, textSnapshot.Length)).Max(region => region.End);
        }

        /// <summary> 
        /// Gets the declarations and snippet entries for the completion 
        /// </summary> 
        private CompletionSet GetCompletions(IEnumerable<Declaration> attributes, ICompletionSession session)
        {
            // Add IPy completion 
            var completions = new List<Microsoft.VisualStudio.Language.Intellisense.Completion>();
            completions.AddRange(attributes.Select(declaration => new HyperstoreCompletion(declaration, glyphService)));

            // we want the user to get a sorted list 
            completions.Sort();

            return
                new HyperstoreCompletionSet("HyperstoreCompletion",
                    "Hyperstore Completion",
                    CreateTrackingSpan(session.GetTriggerPoint(session.TextView.TextBuffer).GetPosition(textBuffer.CurrentSnapshot)),
                    completions,
                    null);
        }

        private ITrackingSpan CreateTrackingSpan(int position)
        {
            char[] separators = new[] { '\n', '\r', '\t', ' ', '.', ':', '{', '}', '-', ';', '=', '*' };

            string text = textBuffer.CurrentSnapshot.GetText();
            int last = text.Substring(position).IndexOfAny(separators);
            int first = text.Substring(0, position).LastIndexOfAny(separators) + 1;

            if (last == -1)
                last = text.Length - position;

            return textBuffer.CurrentSnapshot.CreateTrackingSpan(new Span(first, (last + position) - first), SpanTrackingMode.EdgeInclusive);
        }

        #endregion

        public void Dispose() { }
    }
}