using Hyperstore.CodeAnalysis.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hyperstore.CodeAnalysis.Symbols
{
    internal sealed class UsingSymbol : Symbol, Hyperstore.CodeAnalysis.Symbols.IUsingSymbol
    {
        public SyntaxToken DomainUri { get; private set; }

        public string Path { get { return DomainUri.Text; } }

        internal UsingSymbol(Hyperstore.CodeAnalysis.Syntax.SyntaxNode node, Symbol parent, SyntaxToken alias, SyntaxToken uri)
            : base(node, parent, alias)
        {
            DomainUri = uri;
        }

        protected override void AcceptCore(HyperstoreSymbolVisitor visitor)
        {
            base.AcceptCore(visitor);
            visitor.VisitUsingSymbol(this);
        }

        string IUsingSymbol.DomainUri
        {
            get { return this.DomainUri != null ? this.DomainUri.Text : null; }
        }
    }
}
