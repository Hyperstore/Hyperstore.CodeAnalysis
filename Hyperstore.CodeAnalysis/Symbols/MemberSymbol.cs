using Hyperstore.CodeAnalysis.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hyperstore.CodeAnalysis.Symbols
{
    internal abstract class MemberSymbol : NamedSymbol, IMemberSymbol
    {
        internal MemberSymbol(Hyperstore.CodeAnalysis.Syntax.SyntaxNode node, Hyperstore.CodeAnalysis.Symbols.Symbol parent, SyntaxToken name)
            : base(node, parent, name)
        {
        }

    }
}
