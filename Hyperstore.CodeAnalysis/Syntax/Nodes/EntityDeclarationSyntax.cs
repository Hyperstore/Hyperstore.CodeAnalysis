using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hyperstore.CodeAnalysis.Syntax
{
    [System.Diagnostics.DebuggerDisplay("{Name}")]
    public sealed class EntityDeclarationSyntax : ElementDeclarationSyntax
    {
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
            Members = treeNode.ChildNodes[7].AstNode as ListSyntax<MemberDeclarationSyntax>;
            AddChild(Members);
            if (treeNode.ChildNodes[8].ChildNodes.Count > 0)
            {
                Constraints = treeNode.ChildNodes[8].ChildNodes[0].AstNode as ListSyntax<ConstraintDeclarationSyntax>;
                AddChild(Constraints);
            }
            else
                Constraints = ListSyntax<ConstraintDeclarationSyntax>.Empty;
        }
    }
}
