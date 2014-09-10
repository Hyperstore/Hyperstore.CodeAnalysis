using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Irony.Parsing
{
    public abstract class Trivia
    {
        public SourceSpan Span { get; private set; }
        public Trivia(SourceSpan span)
        {
            Span = span;
        }
    }

    public class WhitespaceTrivia : Trivia
    {
        public WhitespaceTrivia(SourceSpan span)
            : base(span)
        {
        }

        public override string ToString()
        {
            return new String(' ', Span.Length);
        }
    }

    public class EndOfLineTrivia : Trivia
    {
        public EndOfLineTrivia(SourceSpan span)
            : base(span)
        {
        }

        public override string ToString()
        {
            return "\r\n";
        }
    }

    public class CommentTrivia : Trivia
    {
        public Token Comment { get; private set; }

        public CommentTrivia(Token comment)
            : base(new SourceSpan(comment.Location, comment.Length))
        {
            Comment = comment;
        }

        public override string ToString()
        {
            return Comment.Text;
        }
    }

}
