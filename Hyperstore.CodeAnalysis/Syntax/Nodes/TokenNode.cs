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

        public HyperstoreSyntaxTree SyntaxTree
        {
            get { return Location != null ? Location.SyntaxTree : null; }
            internal set { Location.SyntaxTree = value; }
        }

        public Location Location { get; private set; }

        public TextSpan FullSpan
        {
            get
            {
                var startPos = Location.SourceSpan.Start;
                var length = Location.SourceSpan.Length;
                if (LeadingTrivia != null)
                {
                    var trivia = LeadingTrivia.FirstOrDefault();
                    if (trivia != null)
                    {
                        startPos = trivia.Span.Location.Position;
                        length += trivia.Span.Length;
                    }
                }

                if (TrailingTrivia != null)
                {
                    var trivia = TrailingTrivia.LastOrDefault();
                    if (trivia != null)
                    {
                        length += trivia.Span.Length;
                    }
                }

                return new TextSpan(startPos, length);
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
            Location = new Location(null, new TextSpan(token.Location, token.Length));
        }

        public override string ToString()
        {
            return Text;
        }
    }
}
