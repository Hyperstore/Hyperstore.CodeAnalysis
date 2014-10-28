using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hyperstore.CodeAnalysis.Compilation;
using Hyperstore.CodeAnalysis.Symbols;
using Hyperstore.CodeAnalysis.Syntax;

namespace Hyperstore.CodeAnalysis.Symbols
{
    internal abstract class ReferenceSymbol : MemberSymbol, Hyperstore.CodeAnalysis.Symbols.IReferenceSymbol
    {
        internal LazyRef<RelationshipSymbol> RelationshipReference { get; private set; }
        public RelationshipSymbol Relationship { get { return RelationshipReference != null ? RelationshipReference.Value : null; } }

        public RelationshipDefinitionSymbol Definition { get; internal set; }

        internal ReferenceSymbol(HyperstoreCompilation compilation, Hyperstore.CodeAnalysis.Syntax.SyntaxNode node, Hyperstore.CodeAnalysis.Symbols.Symbol parent, Hyperstore.CodeAnalysis.Syntax.QualifiedNameSyntax relationshipName, SyntaxToken name)
            : base(node, parent, name)
        {
            if (relationshipName != null)
                RelationshipReference = new LazyRef<RelationshipSymbol>(relationshipName, compilation, parent.Domain);
        }

        protected override void AcceptCore(Symbols.HyperstoreSymbolVisitor visitor)
        {
            base.AcceptCore(visitor);
            ((Hyperstore.CodeAnalysis.Symbols.IVisitableSymbol)Definition).Accept(visitor);
        }

        internal void Bind(HyperstoreCompilation compilation, string relationshipName)
        {
            RelationshipReference = new LazyRef<RelationshipSymbol>(relationshipName, compilation, Domain);
        }

        IRelationshipDefinitionSymbol IReferenceSymbol.Definition
        {
            get { return this.Definition; }
        }

        IRelationshipSymbol IReferenceSymbol.Relationship
        {
            get { return this.Relationship; }
        }

        internal override bool TryMerge(MemberSymbol other)
        {
            var prop = other as ReferenceSymbol;
            if (other == null || !prop.Definition.IsEmbedded.Equals(this.Definition))
                return false;

            this.Attributes.AddRange(prop.Attributes);
            return true;
        }
    }
}
