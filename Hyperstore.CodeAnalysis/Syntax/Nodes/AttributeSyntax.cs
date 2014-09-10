using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hyperstore.CodeAnalysis.Syntax
{
    [System.Diagnostics.DebuggerDisplay("{Name}")]
    public sealed class AttributeSyntax : SyntaxNode
    {
        public SeparatedListSyntaxToken Arguments { get; private set; }

        public SyntaxToken Name { get; private set; }

        protected override void InitCore(Irony.Ast.AstContext context, Irony.Parsing.ParseTreeNode treeNode)
        {
            base.InitCore(context, treeNode);

            if (treeNode.ChildNodes.Count > 0)
            {
                var child = treeNode.ChildNodes[0];
                Name = new SyntaxToken(child.Token);
                AddChild(Name);
            }

            if (treeNode.ChildNodes[1].ChildNodes.Count > 0)
            {
                var child = treeNode.ChildNodes[1].ChildNodes[0];
                Arguments = child.AstNode as SeparatedListSyntaxToken;
                AddChild(Arguments);
            }
            else
            {
                Arguments = SeparatedListSyntaxToken.Empty;
            }
        }
    }
}
