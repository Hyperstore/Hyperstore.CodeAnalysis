using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hyperstore.CodeAnalysis.Syntax
{
    public sealed class DefaultValueSyntax : SyntaxNode
    {
        public QualifiedNameSyntax QualifiedIdentifier { get; private set; }

        public SyntaxToken Kind { get; private set; }

        public SyntaxToken Code { get; private set; }

        protected override void InitCore(Irony.Ast.AstContext context, Irony.Parsing.ParseTreeNode treeNode)
        {
            base.InitCore(context, treeNode);

            if (treeNode.ChildNodes.Count > 0)
            {
                Kind = new SyntaxToken(treeNode.ChildNodes[0].Token);
                AddChild(Kind);
            }

            if (treeNode.ChildNodes.Count > 1)
            {
                var child = treeNode.ChildNodes[1];
                Code = new SyntaxToken(child.Token);
                AddChild(Code);
            }

            if (treeNode.ChildNodes.Count > 2)
            {
                QualifiedIdentifier = treeNode.ChildNodes[2].AstNode as QualifiedNameSyntax;
                AddChild(QualifiedIdentifier);
            }
        }
    }
}
