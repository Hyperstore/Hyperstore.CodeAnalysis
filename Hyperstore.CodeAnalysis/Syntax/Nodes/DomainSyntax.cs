using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hyperstore.CodeAnalysis.Syntax
{
    [System.Diagnostics.DebuggerDisplay("{QualifiedName}")]
    public sealed class DomainSyntax : SyntaxNode
    {
        public SyntaxToken Extends { get; private set; }
        public ListSyntax<AttributeSyntax> Attributes { get; private set; }

        public QualifiedNameSyntax QualifiedName { get; private set; }

        public ListSyntax<UsesDeclarationSyntax> Uses { get; private set; }

        public ListSyntax<ExternalDeclarationSyntax> Externals { get; private set; }

        public ListSyntax<DeclarationSyntax> Elements { get; private set; }

        protected override void InitCore(Irony.Ast.AstContext context, Irony.Parsing.ParseTreeNode treeNode)
        {
            base.InitCore(context, treeNode);

            Attributes = treeNode.ChildNodes[0].AstNode as ListSyntax<AttributeSyntax>;
            AddChild(Attributes);

            QualifiedName = treeNode.ChildNodes[2].AstNode as QualifiedNameSyntax;
            AddChild(QualifiedName);

            if (treeNode.ChildNodes[3].ChildNodes.Count > 1)
            {
                Extends = new SyntaxToken(treeNode.ChildNodes[3].ChildNodes[1].Token);
                AddChild(Extends);
            }

            var declarations = treeNode.ChildNodes[4].AstNode as ListSyntax<SyntaxNode>;
            Uses = declarations[0] as ListSyntax<UsesDeclarationSyntax>;
            AddChild(Uses);

            Externals = declarations[1] as ListSyntax<ExternalDeclarationSyntax>;
            AddChild(Externals);

            Elements = declarations[2] as ListSyntax<DeclarationSyntax>;
            AddChild(Elements);
        }
    }
}
