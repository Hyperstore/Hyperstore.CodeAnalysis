using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hyperstore.CodeAnalysis.Compilation;
using Hyperstore.CodeAnalysis.Syntax;

namespace Hyperstore.CodeAnalysis.Symbols
{
    internal sealed class ValueObjectSymbol : TypeSymbol, Hyperstore.CodeAnalysis.Symbols.IValueObjectSymbol
    {

        public List<ConstraintSymbol> Constraints { get; private set; }

        internal LazyRef<TypeSymbol> TypeReference { get; private set; }

        public TypeSymbol Type { get { return TypeReference.Value; } }


        internal ValueObjectSymbol(HyperstoreCompilation compilation, Hyperstore.CodeAnalysis.Syntax.SyntaxNode node, Symbol parent, SyntaxToken name, SyntaxToken valueType)
            : base(node, parent, name)
        {
            Constraints = new List<ConstraintSymbol>();
            TypeReference = new LazyRef<TypeSymbol>(valueType, compilation, parent.Domain);

        }

        public bool IsPartial { get; internal set; }

        protected override void AcceptCore(HyperstoreSymbolVisitor visitor)
        {
            base.AcceptCore(visitor);
            Constraints.ForEach(c => ((Hyperstore.CodeAnalysis.Symbols.IVisitableSymbol)c).Accept(visitor));
            visitor.VisitValueObjectSymbol(this);
        }

        IEnumerable<IConstraintSymbol> IValueObjectSymbol.Constraints
        {
            get { return this.Constraints; }
        }

        ITypeSymbol IValueObjectSymbol.Type
        {
            get { return this.Type; }
        }
    }
}
