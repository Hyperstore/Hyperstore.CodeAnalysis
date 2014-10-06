using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hyperstore.CodeAnalysis.Symbols;
using Hyperstore.CodeAnalysis.Syntax;

namespace Hyperstore.CodeAnalysis.Symbols
{
    internal abstract class TypeSymbol : NamedSymbol, Hyperstore.CodeAnalysis.Symbols.ITypeSymbol
    {
        internal TypeSymbol(Hyperstore.CodeAnalysis.Syntax.SyntaxNode node, Hyperstore.CodeAnalysis.Symbols.Symbol parent, SyntaxToken name)
            : base(node, parent, name)
        {
        }

        internal virtual void AddDerived(ElementSymbol elem)
        { }

        public virtual string QualifiedName { get { return String.Format("{0}.{1}", Domain.Namespace, this.Name); } }
    }
}
