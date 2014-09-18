using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hyperstore.CodeAnalysis.Syntax
{
    public sealed class CommandMemberDeclarationSyntax : MemberDeclarationSyntax 
    {
        public SyntaxToken PropertyType { get; private set; }

        protected override void InitCore(Irony.Ast.AstContext context, Irony.Parsing.ParseTreeNode treeNode)
        {
            base.InitCore(context, treeNode);

            Attributes = treeNode.ChildNodes[0].AstNode as ListSyntax<AttributeSyntax>;
            AddChild(Attributes);

            Name = new SyntaxToken(treeNode.ChildNodes[1].Token);
            AddChild(Name);

            PropertyType = new SyntaxToken(treeNode.ChildNodes[2].ChildNodes[0].Token);
            AddChild(PropertyType);
        }
    }
}
