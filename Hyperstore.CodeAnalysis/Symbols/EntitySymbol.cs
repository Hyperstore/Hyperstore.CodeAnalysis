using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hyperstore.CodeAnalysis.Compilation;
using Hyperstore.CodeAnalysis.Syntax;

namespace Hyperstore.CodeAnalysis.Symbols
{
    internal class EntitySymbol : ElementSymbol, IEntitySymbol
    {
        internal EntitySymbol(Hyperstore.CodeAnalysis.Syntax.SyntaxNode node, Symbol parent, SyntaxToken name)
            : base(node, parent, name)
        {
        }

        protected override void AcceptCore(HyperstoreSymbolVisitor visitor)
        {
            base.AcceptCore(visitor);
            visitor.VisitEntitySymbol(this);
        }
    }
}
