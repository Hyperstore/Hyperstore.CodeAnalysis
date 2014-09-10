using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hyperstore.CodeAnalysis.Syntax;

namespace Hyperstore.CodeAnalysis.Syntax
{
    public abstract class HyperstoreSyntaxVisitor
    {
        public void Visit(SyntaxNode node)
        {
            if (node is QualifiedNameSyntax)
                VisitQualifiedNameSyntax((QualifiedNameSyntax)node);
            else if (node is PropertySyntax)
                VisitPropertySyntax((PropertySyntax)node);
            else if (node is ReferenceDeclarationSyntax)
                VisitReferenceDeclarationSyntax((ReferenceDeclarationSyntax)node);
            else if (node is OppositeReferenceSyntax)
                VisitOppositeReferenceSyntax((OppositeReferenceSyntax)node);
            else if (node is RelationshipDefinitionSyntax)
                VisitRelationshipDefinitionSyntax((RelationshipDefinitionSyntax)node);
            else if (node is EntityDeclarationSyntax)
                VisitEntityDeclarationSyntax((EntityDeclarationSyntax)node);
            else if (node is ConstraintDeclarationSyntax)
                VisitConstraintDeclarationSyntax((ConstraintDeclarationSyntax)node);
            else if (node is AttributeSyntax)
                VisitAttributeSyntax((AttributeSyntax)node);
            else if (node is CommandDeclarationSyntax)
                VisitCommandDeclarationSyntax((CommandDeclarationSyntax)node);
            else if (node is CommandMemberDeclarationSyntax)
                VisitCommandMemberDeclarationSyntax((CommandMemberDeclarationSyntax)node);
            else if (node is DefaultValueSyntax)
                VisitDefaultValueSyntax((DefaultValueSyntax)node);
            else if (node is EnumDeclarationSyntax)
                VisitEnumDeclarationSyntax((EnumDeclarationSyntax)node);
            else if (node is ExternalDeclarationSyntax)
                VisitExternalDeclarationSyntax((ExternalDeclarationSyntax)node);
            else if (node is RelationshipDeclarationSyntax)
                VisitRelationshipDeclarationSyntax((RelationshipDeclarationSyntax)node);
            else if (node is UsesDeclarationSyntax)
                VisitUsesDeclarationSyntax((UsesDeclarationSyntax)node);
            else if (node is DomainSyntax)
                VisitDomainSyntax((DomainSyntax)node);
        }

        private void VisitQualifiedNameSyntax(QualifiedNameSyntax node)
        {
            DefaultVisit(node);
        }

        protected virtual void VisitOppositeReferenceSyntax(OppositeReferenceSyntax node)
        {
            DefaultVisit(node);
        }

        protected virtual void DefaultVisit(SyntaxNode node)
        {

        }

        protected virtual void VisitUsesDeclarationSyntax(UsesDeclarationSyntax node)
        {
            DefaultVisit(node);
        }

        protected virtual void VisitRelationshipDefinitionSyntax(RelationshipDefinitionSyntax node)
        {
            DefaultVisit(node);
        }

        protected virtual void VisitRelationshipDeclarationSyntax(RelationshipDeclarationSyntax node)
        {
            DefaultVisit(node);
        }

        protected virtual void VisitReferenceDeclarationSyntax(ReferenceDeclarationSyntax node)
        {
            DefaultVisit(node);
        }

        protected virtual void VisitPropertySyntax(PropertySyntax node)
        {
            DefaultVisit(node);
        }

        protected virtual void VisitExternalDeclarationSyntax(ExternalDeclarationSyntax node)
        {
            DefaultVisit(node);
        }

        protected virtual void VisitEnumDeclarationSyntax(EnumDeclarationSyntax node)
        {
            DefaultVisit(node);
        }

        protected virtual void VisitEntityDeclarationSyntax(EntityDeclarationSyntax node)
        {
            DefaultVisit(node);
        }

        protected virtual void VisitDefaultValueSyntax(DefaultValueSyntax node)
        {
            DefaultVisit(node);
        }

        protected virtual void VisitConstraintDeclarationSyntax(ConstraintDeclarationSyntax node)
        {
            DefaultVisit(node);
        }

        protected virtual void VisitCommandMemberDeclarationSyntax(CommandMemberDeclarationSyntax node)
        {
            DefaultVisit(node);
        }

        protected virtual void VisitCommandDeclarationSyntax(CommandDeclarationSyntax node)
        {
            DefaultVisit(node);
        }

        protected virtual void VisitAttributeSyntax(AttributeSyntax node)
        {
            DefaultVisit(node);
        }

        public virtual void VisitSyntaxToken(SyntaxToken token)
        {
        }

        public virtual void VisitDomainSyntax(DomainSyntax node)
        {
            DefaultVisit(node);
        }
    }
}
