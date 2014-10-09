using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hyperstore.CodeAnalysis.Symbols;
using Hyperstore.CodeAnalysis.Syntax;
using Hyperstore.CodeAnalysis.Compilation;

namespace Hyperstore.CodeAnalysis.Symbols
{
    public enum RelationshipCardinality
    {
        OneToOne = 0,
        ManyToOne = 1,
        OneToMany = 2,
        ManyToMany = 3,
    }

    internal sealed class RelationshipDefinitionSymbol : Symbol, IRelationshipDefinitionSymbol
    {
        public override string Name { get { throw new NotImplementedException(); } }

        internal LazyRef<ElementSymbol> SourceReference { get; private set; }
        internal LazyRef<ElementSymbol> EndReference { get; private set; }

        public ElementSymbol End { get { return EndReference.Value; } }

        public ElementSymbol Source { get { return SourceReference.Value; } }

        public RelationshipCardinality Cardinality { get; private set; }

        public bool IsEmbedded { get; private set; }

        internal string PropertySource { get; set; }

        internal string PropertyEnd { get; set; }

        internal RelationshipDefinitionSymbol(SyntaxNode node, Symbol parent, ElementSymbol source, ElementSymbol end, RelationshipCardinality cardinality, bool isEmbedded)
            : base(node, parent, null)
        {
            if (source != null)
                SourceReference = new LazyRef<ElementSymbol>(source, source.Name);
            if (end != null)
                EndReference = new LazyRef<ElementSymbol>(end, end.Name);
            IsEmbedded = isEmbedded;
            Cardinality = cardinality;
        }

        internal RelationshipDefinitionSymbol(SyntaxNode node, Symbol parent, HyperstoreCompilation compilation, QualifiedNameSyntax source, bool sourceMultiplicity, SyntaxToken end, bool endMultiplicity, bool isEmbedded)
            : base(node, parent, null)
        {
            RecalculateSpan(source, end);

            SourceReference = new LazyRef<ElementSymbol>(source, compilation, parent.Domain);
            EndReference = new LazyRef<ElementSymbol>(end, compilation, parent.Domain);
            IsEmbedded = isEmbedded;
            CalculateCardinality(sourceMultiplicity, endMultiplicity);
        }

        internal RelationshipDefinitionSymbol(SyntaxNode node, Symbol parent, HyperstoreCompilation compilation, SyntaxToken source, bool sourceMultiplicity, QualifiedNameSyntax end, bool endMultiplicity, bool isEmbedded)
            : base(node, parent, null)
        {
            RecalculateSpan(end, source);

            SourceReference = new LazyRef<ElementSymbol>(source, compilation, parent.Domain);
            EndReference = new LazyRef<ElementSymbol>(end, compilation, parent.Domain);
            IsEmbedded = isEmbedded;
            CalculateCardinality(sourceMultiplicity, endMultiplicity);
        }

        private void RecalculateSpan(QualifiedNameSyntax source, SyntaxToken end)
        {
            var beginPos = source.Location.SourceSpan.Start < end.Location.SourceSpan.Start ? source.Location.SourceSpan : end.Location.SourceSpan;
            var endPos = source.Location.SourceSpan.Start < end.Location.SourceSpan.Start ? end.Location.SourceSpan : source.Location.SourceSpan;
            ReplaceLocation( new Location(source.SyntaxTree, new TextSpan( beginPos.Start, endPos.End - beginPos.Start)));
        }

        private void CalculateCardinality(bool sourceMultiplicity, bool endMultiplicity)
        {
            Cardinality = RelationshipCardinality.OneToOne;
            if (sourceMultiplicity)
                Cardinality |= RelationshipCardinality.ManyToOne;
            if (endMultiplicity)
                Cardinality |= RelationshipCardinality.OneToMany;
        }

        protected override void AcceptCore(HyperstoreSymbolVisitor visitor)
        {
            base.AcceptCore(visitor);
            visitor.VisitRelationshipDefinition(this);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var def2 = obj as RelationshipDefinitionSymbol;
            if (def2 == null)
                return false;

            return Cardinality == def2.Cardinality && def2.End == End && Source == def2.Source && IsEmbedded == def2.IsEmbedded;
        }

        IElementSymbol IRelationshipDefinitionSymbol.End
        {
            get { return this.End; }
        }

        IElementSymbol IRelationshipDefinitionSymbol.Source
        {
            get { return this.Source; }
        }

        string IRelationshipDefinitionSymbol.EndProperty
        {
            get { return this.PropertyEnd; }
        }

        string IRelationshipDefinitionSymbol.SourceProperty
        {
            get { return this.PropertySource; }
        }
    }
}
