using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Irony;
using Irony.Ast;

using Irony.Parsing;

namespace Hyperstore.Modeling.TextualLanguage
{
    public class QualifiedNameNode : TestRoslyn.Syntax.SyntaxNode
    {
        public string Name { get; private set; }

        protected override void InitCore( AstContext context, ParseTreeNode treeNode )
        {
            base.InitCore( context, treeNode );
            var sb = new StringBuilder();
           // bool first = true;
            foreach (var node in treeNode.ChildNodes)
            {
                //if (!first)
                //    sb.Append(".");
                //first = false;
                sb.Append(node.Token.ValueString);
            }
            Name = sb.ToString();
        }


        public override string ToString()
        {
            return Name;
        }
    }
}
