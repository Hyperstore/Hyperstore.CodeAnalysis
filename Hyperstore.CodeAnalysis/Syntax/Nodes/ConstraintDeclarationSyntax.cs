using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hyperstore.CodeAnalysis.Syntax
{
    public sealed class ConstraintDeclarationSyntax : SyntaxNode 
    {
        public SyntaxToken Verb { get; private set; }

        public SyntaxToken Condition { get; private set; }

        public SyntaxToken Message { get; private set; }

        public SyntaxToken ErrorLevel { get; private set; }


        protected override void InitCore(Irony.Ast.AstContext context, Irony.Parsing.ParseTreeNode treeNode)
        {
            base.InitCore(context, treeNode);

            Verb = new SyntaxToken(treeNode.ChildNodes[0].ChildNodes[0].Token);
            AddChild(Verb);

            if (treeNode.ChildNodes[1].ChildNodes.Count > 0)
            {
                ErrorLevel = new SyntaxToken(treeNode.ChildNodes[1].ChildNodes[0].Token);
                AddChild(ErrorLevel);
            }

            if (treeNode.ChildNodes[2].ChildNodes.Count > 0)
            {
                Message = new SyntaxToken(treeNode.ChildNodes[2].ChildNodes[0].Token);
                AddChild(Message);
            }

            Condition = new SyntaxToken(treeNode.ChildNodes[3].Token);
            AddChild(Condition);
        }

    }
}
