using Hyperstore.CodeAnalysis.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hyperstore.CodeAnalysis.Symbols
{
    internal sealed class EnumSymbol : TypeSymbol, Hyperstore.CodeAnalysis.Symbols.IEnumSymbol
    {
        public List<string> Values { get; set; }

        internal EnumSymbol(Hyperstore.CodeAnalysis.Syntax.SyntaxNode node, Hyperstore.CodeAnalysis.Symbols.Symbol parent, SyntaxToken name)
            : base(node, parent, name)
        {
        }

        protected override void AcceptCore(Symbols.HyperstoreSymbolVisitor visitor)
        {
            base.AcceptCore(visitor);
            visitor.VisitEnumSymbol(this);
        }

        IEnumerable<string> IEnumSymbol.Values
        {
            get { return this.Values; }
        }
    }
}
