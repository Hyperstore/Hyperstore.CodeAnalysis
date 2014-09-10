using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Hyperstore.CodeAnalysis.Symbols
{
    internal abstract class HyperstoreSymbolVisitor
    {

        //public void Visit(Symbol symbol)
        //{
        //    var visitable = symbol as IVisitableSymbol;
        //    if (visitable == null)
        //        return;

        //    if (symbol is DomainSymbol)
        //        VisitDomainSymbol(((DomainSymbol)symbol));
        //    else if (symbol is AttributeSymbol)
        //        VisitAttributeSymbol(((AttributeSymbol)symbol));
        //    else if (symbol is CommandSymbol)
        //        VisitCommandSymbol(((CommandSymbol)symbol));
        //    else if (symbol is ConstraintSymbol)
        //        VisitConstraintSymbol(((ConstraintSymbol)symbol));
        //    else if (symbol is EntitySymbol)
        //        VisitEntitySymbol(((EntitySymbol)symbol));
        //    else if (symbol is EnumSymbol)
        //        VisitEnumSymbol(((EnumSymbol)symbol));
        //    else if (symbol is ExternSymbol)
        //        VisitExternSymbol(((ExternSymbol)symbol));
        //    else if (symbol is OppositeReferenceSymbol)
        //        VisitOppositeReferenceSymbol(((OppositeReferenceSymbol)symbol));
        //    else if (symbol is PropertySymbol)
        //        VisitPropertySymbol(((PropertySymbol)symbol));
        //    else if (symbol is ReferenceSymbol)
        //        VisitReferenceSymbol(((ReferenceSymbol)symbol));
        //    else if (symbol is RelationshipSymbol)
        //        VisitRelationshipSymbol(((RelationshipSymbol)symbol));
        //}


        public virtual void VisitRelationshipSymbol(RelationshipSymbol symbol)
        {

        }

        public virtual void VisitPropertyReferenceSymbol(PropertyReferenceSymbol symbol)
        {

        }

        public virtual void VisitPropertySymbol(PropertySymbol symbol)
        {

        }

        public virtual void VisitOppositeReferenceSymbol(OppositeReferenceSymbol symbol)
        {

        }

        public virtual void VisitExternSymbol(ExternSymbol symbol)
        {

        }

        public virtual void VisitEnumSymbol(EnumSymbol symbol)
        {

        }

        public virtual void VisitEntitySymbol(EntitySymbol symbol)
        {

        }

        public virtual void VisitConstraintSymbol(ConstraintSymbol symbol)
        {

        }

        public virtual void VisitCommandSymbol(CommandSymbol symbol)
        {

        }

        public virtual void VisitAttributeSymbol(AttributeSymbol symbol)
        {

        }

        public virtual void VisitDomainSymbol(DomainSymbol symbol)
        {

        }

        public virtual void VisitCSharpCode(CSharpCodeSymbol cSharpCodeSymbol)
        {
        }

        public virtual void VisitRelationshipDefinition(RelationshipDefinitionSymbol definition)
        {
        }

        public virtual void VisitUsingSymbol(UsingSymbol usingSymbol)
        {
        }
    }
}
