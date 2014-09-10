using Irony.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hyperstore.CodeAnalysis.Syntax
{
    public struct TokenOrNode
    {
        private SyntaxNode _node;
        private SyntaxToken _token;

        public bool IsNode { get { return _node != null; } }

        internal TokenOrNode(SyntaxToken token)
        {
            _token = token;
            _node = null;
        }

        internal TokenOrNode(SyntaxNode node) 
        {
            _token = null;
            _node = node;
        }

        public SyntaxNode AsNode()
        {
            return _node;
        }

        public bool IsToken { get { return _token != null; } }

        public SyntaxToken AsToken()
        {
            return _token;
        }

        public HyperstoreSyntaxTree SyntaxTree
        {
            get { return _token != null ? _token.SyntaxTree : _node.SyntaxTree; }
        }

        public SourceSpan Span
        {
            get { return _token != null ? _token.Span : _node.Span; }
        }
    }
}
