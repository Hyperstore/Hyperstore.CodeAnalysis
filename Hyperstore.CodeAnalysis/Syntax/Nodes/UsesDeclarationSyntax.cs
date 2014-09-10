using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hyperstore.CodeAnalysis.Syntax
{
    public sealed class UsesDeclarationSyntax : SyntaxNode
    {
        public SyntaxToken Uri { get; private set; }

        public SyntaxToken Alias { get; private set; }

        protected override void InitCore(Irony.Ast.AstContext context, Irony.Parsing.ParseTreeNode treeNode)
        {
            base.InitCore(context, treeNode);

            Uri = new SyntaxToken(treeNode.ChildNodes[1].Token);
            AddChild(Uri);
            Alias = new SyntaxToken(treeNode.ChildNodes[3].Token);
            AddChild(Alias);
        }
    }
}
