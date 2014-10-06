using Hyperstore.CodeAnalysis.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hyperstore.CodeAnalysis.Symbols
{
    internal abstract class NamedSymbol : Hyperstore.CodeAnalysis.Symbols.Symbol, Hyperstore.CodeAnalysis.Symbols.INamedSymbol
    {
        public List<AttributeSymbol> Attributes
        {
            get;
            protected set;
        }

        public bool Skip
        {
            get { return HasAttribute("Ignore"); }
        }

        internal NamedSymbol(Hyperstore.CodeAnalysis.Syntax.SyntaxNode node, Hyperstore.CodeAnalysis.Symbols.Symbol parent, SyntaxToken name)
            : base(node, parent, name)
        {
            Attributes = new List<AttributeSymbol>();
        }

        protected override void AcceptCore(Symbols.HyperstoreSymbolVisitor visitor)
        {
            base.AcceptCore(visitor);
            Attributes.ForEach(m => ((Hyperstore.CodeAnalysis.Symbols.IVisitableSymbol)m).Accept(visitor));
        }


        IEnumerable<IAttributeSymbol> INamedSymbol.Attributes
        {
            get { return this.Attributes; }
        }

        public bool HasAttribute(string name)
        {
            return Attributes.Any(a => String.Compare(name, a.Name, StringComparison.OrdinalIgnoreCase) == 0);
        }
    }
}
