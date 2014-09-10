using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hyperstore.CodeAnalysis.Compilation;
using Hyperstore.CodeAnalysis.Syntax;

namespace Hyperstore.CodeAnalysis.Symbols
{
    internal sealed class VirtualRelationshipSymbol : RelationshipSymbol, IVirtualRelationshipSymbol
    {
        private string _name;

        internal VirtualRelationshipSymbol(Hyperstore.CodeAnalysis.Syntax.SyntaxNode node, Hyperstore.CodeAnalysis.Symbols.Symbol parent, RelationshipDefinitionSymbol definition, string name)
            : base(node, parent, null)
        {
            Definition = definition;
            definition.Parent = this;
            _name = name;            
        }

        public override string Name
        {
            get { return _name; }
        }
    }

    internal class RelationshipSymbol : Hyperstore.CodeAnalysis.Symbols.ElementSymbol, Hyperstore.CodeAnalysis.Symbols.IRelationshipSymbol
    {
        public RelationshipDefinitionSymbol Definition { get; internal set; }

        internal RelationshipSymbol(Hyperstore.CodeAnalysis.Syntax.SyntaxNode node, Hyperstore.CodeAnalysis.Symbols.Symbol parent, SyntaxToken name)
            : base(node, parent, name)
        {
        }

        protected override void AcceptCore(Symbols.HyperstoreSymbolVisitor visitor)
        {
            base.AcceptCore(visitor);
            ((Hyperstore.CodeAnalysis.Symbols.IVisitableSymbol)Definition).Accept(visitor);
            visitor.VisitRelationshipSymbol(this);
        }

        IRelationshipDefinitionSymbol IRelationshipSymbol.Definition
        {
            get { return this.Definition; }
        }
    }
}
