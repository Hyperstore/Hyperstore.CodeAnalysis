using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hyperstore.CodeAnalysis.Compilation;
using Hyperstore.CodeAnalysis.Syntax;

namespace Hyperstore.CodeAnalysis.Symbols
{
    internal abstract class ElementSymbol : TypeSymbol, Hyperstore.CodeAnalysis.Symbols.IElementSymbol
    {
        private HashSet<ElementSymbol> _derived = new HashSet<ElementSymbol>();

        public IEnumerable<ElementSymbol> DerivedElements { get { return _derived; } }

        public List<ConstraintSymbol> Constraints { get; private set; }

        public IEnumerable<PropertySymbol> Properties { get { return Members.OfType<PropertySymbol>(); } }
        public IEnumerable<PropertyReferenceSymbol> References { get { return Members.OfType<PropertyReferenceSymbol>(); } }
        public IEnumerable<OppositeReferenceSymbol> OppositeReferences { get { return Members.OfType<OppositeReferenceSymbol>(); } }

        internal List<MemberSymbol> Members
        {
            get;
            private set;
        }

        private readonly List<LazyRef<TypeSymbol>> _extends;

        internal List<LazyRef<TypeSymbol>> ExtendsReferences { get { return _extends; } }
        internal IEnumerable<TypeSymbol> Extends { get { return ExtendsReferences.Where(i => i.Value != null).Select(i => i.Value); } }

        public TypeSymbol SuperType { get { return Extends.FirstOrDefault(); } }

        private readonly List<LazyRef<TypeSymbol>> _implements;

        internal List<LazyRef<TypeSymbol>> ImplementReferences { get { return _implements; } }
        public IEnumerable<TypeSymbol> Implements { get { return ImplementReferences.Where(i => i.Value != null).Select(i => i.Value); } }

        internal ElementSymbol(Hyperstore.CodeAnalysis.Syntax.SyntaxNode node, Symbol parent, SyntaxToken name)
            : base(node, parent, name)
        {
            Members = new List<MemberSymbol>();
            Constraints = new List<ConstraintSymbol>();
            _implements = new List<LazyRef<TypeSymbol>>();
            _extends = new List<LazyRef<TypeSymbol>>();
        }

        public bool IsPartial { get; internal set; }

        protected override void AcceptCore(HyperstoreSymbolVisitor visitor)
        {
            base.AcceptCore(visitor);
            Constraints.ForEach(c => ((Hyperstore.CodeAnalysis.Symbols.IVisitableSymbol)c).Accept(visitor));
            Members.ForEach(m => ((Hyperstore.CodeAnalysis.Symbols.IVisitableSymbol)m).Accept(visitor));
        }

        internal void Bind(HyperstoreCompilation compilation, QualifiedNameSyntax extends, IEnumerable<QualifiedNameSyntax> implements)
        {
            if (extends != null)
                _extends.Add(new LazyRef<TypeSymbol>(extends, compilation, Domain));
            _implements.AddRange(implements.Select(i => new LazyRef<TypeSymbol>(i, compilation, Domain)));
        }

        public bool HasClassInheritance { get; internal set; }

        internal override void AddDerived(ElementSymbol elem)
        {
            _derived.Add(elem);
        }

        public bool IsA(IElementSymbol symbol)
        {
            if (this == symbol)
                return true;

            var super = SuperType as IElementSymbol;
            if (super != null)
                return super.IsA(symbol);

            return false;
        }

        IEnumerable<IConstraintSymbol> IElementSymbol.Constraints
        {
            get { return this.Constraints; }
        }

        IEnumerable<IElementSymbol> IElementSymbol.DerivedElements
        {
            get { return this.DerivedElements; }
        }

        IEnumerable<ITypeSymbol> IElementSymbol.Implements
        {
            get { return this.Implements; }
        }

        IEnumerable<IOppositeReferenceSymbol> IElementSymbol.OppositeReferences
        {
            get { return this.OppositeReferences; }
        }

        IEnumerable<IPropertySymbol> IElementSymbol.Properties
        {
            get { return this.Properties; }
        }

        IEnumerable<IPropertyReferenceSymbol> IElementSymbol.References
        {
            get { return this.References; }
        }

        ITypeSymbol IElementSymbol.SuperType
        {
            get { return this.SuperType; }
        }
    }
}
