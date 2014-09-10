using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hyperstore.CodeAnalysis.Syntax
{
    public sealed class RelationshipDeclarationSyntax : ElementDeclarationSyntax 
    {
        public RelationshipDefinitionSyntax Definition { get; private set; }

        protected override void InitCore(Irony.Ast.AstContext context, Irony.Parsing.ParseTreeNode treeNode)
        {
            base.InitCore(context, treeNode);

            Attributes = treeNode.ChildNodes[0].AstNode as ListSyntax<AttributeSyntax>;
            AddChild(Attributes);
            if (treeNode.ChildNodes[2].ChildNodes.Count > 0)
            {
                Partial = new SyntaxToken(treeNode.ChildNodes[2].ChildNodes[0].Token);
                AddChild(Partial);
            }
            Name = new SyntaxToken(treeNode.ChildNodes[4].Token);
            AddChild(Name);

            if (treeNode.ChildNodes[5].ChildNodes.Count > 0)
            {
                Extends = treeNode.ChildNodes[5].ChildNodes[1].AstNode as QualifiedNameSyntax;
                AddChild(Extends);
            }
            if (treeNode.ChildNodes[6].ChildNodes.Count > 0)
            {
                Implements = treeNode.ChildNodes[6].ChildNodes[1].AstNode as SeparatedListSyntax<QualifiedNameSyntax>;
                AddChild(Implements);
            }
            else
            {
                Implements = SeparatedListSyntax<QualifiedNameSyntax>.Empty;
            }

            Definition = treeNode.ChildNodes[7].AstNode as RelationshipDefinitionSyntax;
            AddChild(Definition);
            Members = treeNode.ChildNodes[8].AstNode as ListSyntax<MemberDeclarationSyntax>;
            AddChild(Members);
            if (treeNode.ChildNodes[9].ChildNodes.Count > 0)
            {
                Constraints = treeNode.ChildNodes[9].ChildNodes[0].AstNode as ListSyntax<ConstraintDeclarationSyntax>;
                AddChild(Constraints);
            }
            else
                Constraints = ListSyntax<ConstraintDeclarationSyntax>.Empty;
        }
    }
}
