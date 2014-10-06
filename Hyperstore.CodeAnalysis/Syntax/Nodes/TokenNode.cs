using Irony.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hyperstore.CodeAnalysis.Syntax
{
    [System.Diagnostics.DebuggerDisplay("{RawValue}")]
    public sealed class SyntaxToken
    {
        private Irony.Parsing.Token token;

        public HyperstoreSyntaxTree SyntaxTree { get; internal set; }

        public SourceSpan Span { get; private set; }

        public SourceSpan FullSpan
        {
            get
            {
                var startPos = Span;
                if (LeadingTrivia != null)
                {
                    var trivia = LeadingTrivia.FirstOrDefault();
                    if (trivia != null)
                        startPos = trivia.Span;
                }

                var endPos = Span;
                if (TrailingTrivia != null)
                {
                    var trivia = TrailingTrivia.LastOrDefault();
                    if (trivia != null)
                        endPos = trivia.Span;
                }

                return new SourceSpan(startPos.Location, endPos.Location.Position + endPos.Length - startPos.Location.Position);
            }
        }

        public string Text
        {
            get { return token.ValueString; }
        }

        public object RawValue
        {
            get { return token.Value; }
        }

        public IEnumerable<Trivia> LeadingTrivia
        {
            get { return token.LeadTrivias; }
        }

        public IEnumerable<Trivia> TrailingTrivia
        {
            get { return token.TrailTrivias; }
        }

        public SyntaxToken(Irony.Parsing.Token token)
        {
            this.token = token;
            Span = new SourceSpan(token.Location, token.Length);
        }

        public override string ToString()
        {
            return Text;
        }
    }
}
