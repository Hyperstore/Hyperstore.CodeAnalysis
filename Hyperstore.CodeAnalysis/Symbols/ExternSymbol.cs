using Hyperstore.CodeAnalysis.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hyperstore.CodeAnalysis.Symbols
{
    public enum ExternalKind
    {
        Normal,
        Interface,
        Enum,
        Primitive
    }

    internal sealed class ExternSymbol : TypeSymbol, Hyperstore.CodeAnalysis.Symbols.IExternSymbol
    {
        internal ExternSymbol(string alias, string fullName)
            : base(null, null, null)
        {
            Name = alias;
            FullName = fullName;
            Kind = ExternalKind.Primitive;
        }

        internal ExternSymbol(Hyperstore.CodeAnalysis.Syntax.SyntaxNode node, Hyperstore.CodeAnalysis.Symbols.Symbol parent, SyntaxToken name)
            : base(node, parent, name)
        {
        }


        public ExternalKind Kind { get; set; }
        public string FullName { get; set; }

        public string Alias { get { return Name; } }

        protected override void AcceptCore(Symbols.HyperstoreSymbolVisitor visitor)
        {
            base.AcceptCore(visitor);
            visitor.VisitExternSymbol(this);
        }
    }
}
