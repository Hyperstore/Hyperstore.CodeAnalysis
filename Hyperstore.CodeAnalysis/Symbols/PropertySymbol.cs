using Hyperstore.CodeAnalysis.Compilation;
using Hyperstore.CodeAnalysis.Syntax;
using System.Collections.Generic;
using System.Linq;

namespace Hyperstore.CodeAnalysis.Symbols
{
    internal sealed class PropertySymbol : MemberSymbol, Hyperstore.CodeAnalysis.Symbols.IPropertySymbol
    {
        internal LazyRef<TypeSymbol> PropertyTypeReference { get; private set; }
        public TypeSymbol PropertyType { get { return PropertyTypeReference.Value; } }


        public List<ConstraintSymbol> Constraints
        {
            get;
            private set;
        }

        public CSharpCodeSymbol WhereClause { get; internal set; }
        public CSharpCodeSymbol SelectClause { get; internal set; }
        public CSharpCodeSymbol DefaultValue { get; internal set; }

        internal PropertySymbol(HyperstoreCompilation compilation, Hyperstore.CodeAnalysis.Syntax.SyntaxNode node, Hyperstore.CodeAnalysis.Symbols.Symbol parent, Hyperstore.CodeAnalysis.Syntax.QualifiedNameSyntax propertyType, SyntaxToken name)
            : base(node, parent, name)
        {
            Constraints = new List<ConstraintSymbol>();
            PropertyTypeReference = new LazyRef<TypeSymbol>(propertyType, compilation, parent.Domain);
        }

        protected override void AcceptCore(Symbols.HyperstoreSymbolVisitor visitor)
        {
            base.AcceptCore(visitor);
            Constraints.ForEach(m => ((Hyperstore.CodeAnalysis.Symbols.IVisitableSymbol)m).Accept(visitor));

            if (WhereClause != null)
                ((Hyperstore.CodeAnalysis.Symbols.IVisitableSymbol)WhereClause).Accept(visitor);
            if (SelectClause != null)
                ((Hyperstore.CodeAnalysis.Symbols.IVisitableSymbol)SelectClause).Accept(visitor);
            if (DefaultValue != null)
                ((Hyperstore.CodeAnalysis.Symbols.IVisitableSymbol)DefaultValue).Accept(visitor);

            visitor.VisitPropertySymbol(this);
        }

        public bool IsCalculatedProperty
        {
            get
            {
                return Constraints.Any(c => c.Kind == ConstraintKind.Compute);
            }
        }

        public string CalculatedCode
        {
            get
            {
                var constraint = Constraints.FirstOrDefault(c => c.Kind == ConstraintKind.Compute);
                return constraint != null ? constraint.Condition.Code : null;
            }
        }

        IEnumerable<IConstraintSymbol> IPropertySymbol.Constraints
        {
            get { return this.Constraints; }
        }

        ICSharpCodeSymbol IPropertySymbol.DefaultValue
        {
            get { return this.DefaultValue; }
        }

        ITypeSymbol IPropertySymbol.PropertyType
        {
            get { return this.PropertyType; }
        }

        ICSharpCodeSymbol IPropertySymbol.SelectClause
        {
            get { return this.SelectClause; }
        }

        ICSharpCodeSymbol IPropertySymbol.WhereClause
        {
            get { return this.WhereClause; }
        }

    }
}
