using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Irony;
using Irony.Ast;

using Irony.Parsing;

namespace Hyperstore.CodeAnalysis.Syntax
{
    [System.Diagnostics.DebuggerDisplay("{Name}")]
    public class QualifiedNameSyntax : Hyperstore.CodeAnalysis.Syntax.SyntaxNode
    {
        public string FullName { get; private set; }

        public SyntaxToken NameToken { get; private set; }

        protected override void InitCore( AstContext context, ParseTreeNode treeNode )
        {
            base.InitCore( context, treeNode );
            var sb = new StringBuilder();
            foreach (var node in treeNode.ChildNodes)
            {
                sb.Append(node.Token.ValueString);
                NameToken = new SyntaxToken( node.Token );
            }
            FullName = sb.ToString();
        }


        public override string ToString()
        {
            return FullName;
        }
    }
}
