using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hyperstore.CodeAnalysis.Syntax;

namespace Hyperstore.CodeAnalysis.Symbols
{
    internal abstract class Symbol : IVisitableSymbol, Hyperstore.CodeAnalysis.Symbols.ISymbol
    {
        private SyntaxToken _nameToken;

        internal SyntaxToken NameToken { get { return _nameToken; } private set { _nameToken = value; } }

        public virtual string Name
        {
            get;
            protected set;
        }

        public TokenOrNode SyntaxTokenOrNode { get; set; }

        public Symbol Parent { get; set; }

        public virtual DomainSymbol Domain
        {
            get
            {
                var parent = this.Parent;
                while (parent != null && !(parent is DomainSymbol))
                {
                    parent = parent.Parent;
                }
                return (DomainSymbol)parent;
            }
        }

        internal Symbol(SyntaxToken token, Symbol parent, SyntaxToken name)
        {
            NameToken = name;
            Name = NameToken != null ? NameToken.Text : String.Empty;

            SyntaxTokenOrNode = new TokenOrNode(token);
            Parent = parent;
        }

        internal Symbol(SyntaxNode node, Symbol parent, SyntaxToken name)
        {
            NameToken = name;
            Name = NameToken != null ? NameToken.Text : String.Empty;

            SyntaxTokenOrNode = new TokenOrNode(node);
            Parent = parent;
        }

        void IVisitableSymbol.Accept(HyperstoreSymbolVisitor visitor)
        {
            AcceptCore(visitor);
        }

        protected virtual void AcceptCore(HyperstoreSymbolVisitor visitor)
        {

        }

        ISymbol ISymbol.Parent
        {
            get
            {
                return this.Parent;
            }
        }

        TokenOrNode ISymbol.SyntaxTokenOrNode
        {
            get
            {
                return this.SyntaxTokenOrNode;
            }
        }

        IDomainSymbol ISymbol.Domain
        {
            get
            {
                return this.Domain;
            }
        }
    }
}
