using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hyperstore.CodeAnalysis.Syntax
{
    [System.Diagnostics.DebuggerDisplay("{Name}")]
    public sealed class CommandDeclarationSyntax : DeclarationSyntax
    {
        public ListSyntax<CommandMemberDeclarationSyntax> Properties { get; protected set; }

        protected override void InitCore(Irony.Ast.AstContext context, Irony.Parsing.ParseTreeNode treeNode)
        {
            base.InitCore(context, treeNode);

            Attributes = treeNode.ChildNodes[0].AstNode as ListSyntax<AttributeSyntax>;
            AddChild(Attributes);
            Name = new SyntaxToken(treeNode.ChildNodes[3].Token);
            AddChild(Name);
            Properties = treeNode.ChildNodes[4].AstNode as ListSyntax<CommandMemberDeclarationSyntax>;
            AddChild(Properties);
        }
    }
}
