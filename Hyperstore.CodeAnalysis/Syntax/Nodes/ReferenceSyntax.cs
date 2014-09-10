using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hyperstore.CodeAnalysis.Syntax
{
    public sealed class ReferenceDeclarationSyntax : MemberDeclarationSyntax 
    {
        public RelationshipDefinitionSyntax Definition { get; private set; }
        public QualifiedNameSyntax RelationshipName { get; private set; }

        protected override void InitCore(Irony.Ast.AstContext context, Irony.Parsing.ParseTreeNode treeNode)
        {
            base.InitCore(context, treeNode);

            Attributes = treeNode.ChildNodes[0].AstNode as ListSyntax<AttributeSyntax>;
            AddChild(Attributes);

            Definition = treeNode.ChildNodes[1].AstNode as RelationshipDefinitionSyntax;
            Name = Definition.Name;
            AddChild(Name);
            AddChild(Definition);

            if (treeNode.ChildNodes[2].ChildNodes.Count > 0)
            {
                RelationshipName = treeNode.ChildNodes[2].ChildNodes[0].AstNode as QualifiedNameSyntax;
                AddChild(RelationshipName);
            }
        }
    }
}
