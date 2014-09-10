using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Irony;
using Irony.Ast;

using Irony.Parsing;

namespace Hyperstore.Modeling.TextualLanguage
{
    public class UsesNode : TestRoslyn.Syntax.SyntaxNode
    {
        public string Alias { get; private set; }
        public string FullName { get; private set; }
        public IDomainSyntaxNode Domain { get; private set; }

        protected override void InitCore(AstContext context, ParseTreeNode treeNode)
        {
            base.InitCore(context, treeNode);
            FullName = treeNode.ChildNodes[1].Token.Text.Trim('"');
            Alias = treeNode.ChildNodes[3].FindTokenAndGetText();
            AddChild("Alias", treeNode.ChildNodes[3]);
        }

        //public override void AcceptVisitor(IAstVisitor visitor)
        //{
        //    base.AcceptVisitor(visitor);

        //    var ctx = visitor as SemanticAnalysisVisitor;
        //    if( String.IsNullOrWhiteSpace(FullName))
        //        ctx.Messages.Add(new LogMessage(ErrorLevel.Error, this.Span, "Invalid domain reference name", null));

        //    if( String.IsNullOrWhiteSpace(Alias))
        //        ctx.Messages.Add(new LogMessage(ErrorLevel.Error, this.Span, "Invalid alias name", null));

        //    if( ctx.Domain.Uses.Any(a=> a != this && String.Compare(Alias, a.Alias, StringComparison.OrdinalIgnoreCase) == 0) )
        //        ctx.Messages.Add( new LogMessage( ErrorLevel.Error, this.Span, "Duplicate alias name " + Alias, null ) );

        //    try
        //    {
        //        Domain = ctx.FindDomain(FullName);
        //        if (Domain == null)
        //        {
        //            if (Domain == null)
        //                ctx.Messages.Add(new LogMessage(ErrorLevel.Error, this.Span, "Unknow domain " + FullName, null));
        //        }
        //    }
        //    catch(Exception ex)
        //    {
        //        ctx.Messages.Add(new LogMessage(ErrorLevel.Error, this.Span, ex.Message, null));
        //    }
        //}

        public override string ToString()
        {
            return String.Format("Uses {1} as {0}", Alias, FullName);
        }
    }
}
