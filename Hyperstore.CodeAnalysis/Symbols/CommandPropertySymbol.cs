using Hyperstore.CodeAnalysis.Compilation;
using Hyperstore.CodeAnalysis.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hyperstore.CodeAnalysis.Symbols
{
    internal class CommandPropertySymbol : NamedSymbol, Hyperstore.CodeAnalysis.Symbols.ICommandPropertySymbol
    {
        internal LazyRef<TypeSymbol> PropertyTypeReference { get; private set; }
        public TypeSymbol PropertyType { get { return PropertyTypeReference.Value; } }

        public bool IsEntity
        {
            get { var value = PropertyTypeReference.Value; return value != null && !(value is IExternSymbol) ; }
        }

        internal CommandPropertySymbol(HyperstoreCompilation compilation, Hyperstore.CodeAnalysis.Syntax.SyntaxNode node, Symbol parent, Hyperstore.CodeAnalysis.Syntax.SyntaxToken propertyType, SyntaxToken name)
            : base(node, parent, name)
        {
            PropertyTypeReference = new LazyRef<TypeSymbol>(propertyType, compilation, parent.Domain);
        }

        protected override void AcceptCore(HyperstoreSymbolVisitor visitor)
        {
            base.AcceptCore(visitor);
            visitor.VisitCommandPropertySymbol(this);
        }

        ITypeSymbol ICommandPropertySymbol.PropertyType
        {
            get { return this.PropertyType; }
        }
    }
}
