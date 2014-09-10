using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hyperstore.CodeAnalysis.Syntax
{
    public class ExternalDeclarationSyntax : SyntaxNode 
    {
        public SyntaxToken Kind { get; private set; }
        public SyntaxToken QualifiedName { get; private set; }

        public SyntaxToken Alias { get; private set; }

        protected override void InitCore(Irony.Ast.AstContext context, Irony.Parsing.ParseTreeNode treeNode)
        {
            base.InitCore(context, treeNode);
            if( treeNode.ChildNodes[1].ChildNodes.Count > 0)
            {
                Kind = new SyntaxToken(treeNode.ChildNodes[1].ChildNodes[0].Token);
                AddChild(Kind);
            }

            QualifiedName = new SyntaxToken( treeNode.ChildNodes[2].Token);
            AddChild(QualifiedName);
            if (treeNode.ChildNodes[3].ChildNodes.Count > 0)
            {
                Alias = new SyntaxToken(treeNode.ChildNodes[3].ChildNodes[1].Token);
                AddChild(Alias);
            }
        }
    }
}
