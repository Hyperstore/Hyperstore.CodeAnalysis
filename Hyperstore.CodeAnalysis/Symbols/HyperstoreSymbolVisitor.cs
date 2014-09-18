using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Hyperstore.CodeAnalysis.Symbols
{
    public abstract class HyperstoreSymbolVisitor
    {

        public virtual void DefaultSymbol(ISymbol symbol)
        {

        }

        public virtual void VisitValueObjectSymbol(IValueObjectSymbol symbol)
        {
            DefaultSymbol(symbol);
        }

        public virtual void VisitRelationshipSymbol(IRelationshipSymbol symbol)
        {
            DefaultSymbol(symbol);
        }

        public virtual void VisitPropertyReferenceSymbol(IPropertyReferenceSymbol symbol)
        {
            DefaultSymbol(symbol);
        }

        public virtual void VisitPropertySymbol(IPropertySymbol symbol)
        {
            DefaultSymbol(symbol);
        }

        public virtual void VisitOppositeReferenceSymbol(IOppositeReferenceSymbol symbol)
        {
            DefaultSymbol(symbol);
        }

        public virtual void VisitExternSymbol(IExternSymbol symbol)
        {
            DefaultSymbol(symbol);
        }

        public virtual void VisitEnumSymbol(IEnumSymbol symbol)
        {
            DefaultSymbol(symbol);
        }

        public virtual void VisitEntitySymbol(IEntitySymbol symbol)
        {
            DefaultSymbol(symbol);
        }

        public virtual void VisitConstraintSymbol(IConstraintSymbol symbol)
        {
            DefaultSymbol(symbol);
        }

        public virtual void VisitCommandSymbol(ICommandSymbol symbol)
        {
            DefaultSymbol(symbol);
        }

        public virtual void VisitAttributeSymbol(IAttributeSymbol symbol)
        {
            DefaultSymbol(symbol);
        }

        public virtual void VisitDomainSymbol(IDomainSymbol symbol)
        {
            DefaultSymbol(symbol);
        }

        public virtual void VisitCSharpCode(ICSharpCodeSymbol symbol)
        {
            DefaultSymbol(symbol);
        }

        public virtual void VisitRelationshipDefinition(IRelationshipDefinitionSymbol symbol)
        {
            DefaultSymbol(symbol);
        }

        public virtual void VisitUsingSymbol(IUsingSymbol symbol)
        {
            DefaultSymbol(symbol);
        }

        public virtual void VisitCommandPropertySymbol(ICommandPropertySymbol symbol)
        {
            DefaultSymbol(symbol);
        }
    }
}
