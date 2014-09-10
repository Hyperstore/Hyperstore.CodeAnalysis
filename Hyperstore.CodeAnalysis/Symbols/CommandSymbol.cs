using Hyperstore.CodeAnalysis.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hyperstore.CodeAnalysis.Symbols
{
    internal sealed class CommandSymbol : TypeSymbol, ICommandSymbol
    {
        internal CommandSymbol(Hyperstore.CodeAnalysis.Syntax.SyntaxNode node, Symbol parent, SyntaxToken name)
            : base(node, parent, name)
        {
        }


        protected override void AcceptCore(HyperstoreSymbolVisitor visitor)
        {
            base.AcceptCore(visitor);
            visitor.VisitCommandSymbol(this);
        }
    }
}
