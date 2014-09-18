using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hyperstore.CodeAnalysis.Syntax
{
    public sealed class ValueObjectDeclarationSyntax : DeclarationSyntax
    {
        public ListSyntax<ConstraintDeclarationSyntax> Constraints { get; private set; }

        public SyntaxToken Type { get; private set; }

        public SyntaxToken Partial { get; private set; }

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


            Type = new SyntaxToken(treeNode.ChildNodes[5].Token);
            AddChild(Type);

            if (treeNode.ChildNodes[6].ChildNodes.Count > 1)
            {
                Constraints = treeNode.ChildNodes[6].ChildNodes[1].AstNode as ListSyntax<ConstraintDeclarationSyntax>;
                AddChild(Constraints);
            }
            else
                Constraints = ListSyntax<ConstraintDeclarationSyntax>.Empty;
        }
    }
}
