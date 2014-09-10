using Hyperstore.CodeAnalysis.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hyperstore.CodeAnalysis.Symbols
{
    internal sealed class AttributeSymbol : Symbol, Hyperstore.CodeAnalysis.Symbols.IAttributeSymbol
    {
        public IEnumerable<string> Arguments { get; set; }

        internal AttributeSymbol(Hyperstore.CodeAnalysis.Syntax.SyntaxNode node, Symbol parent, SyntaxToken name)
            : base(node, parent, name)
        {
        }

        protected override void AcceptCore(HyperstoreSymbolVisitor visitor)
        {
            base.AcceptCore(visitor);
            visitor.VisitAttributeSymbol(this);
        }
    }
}
