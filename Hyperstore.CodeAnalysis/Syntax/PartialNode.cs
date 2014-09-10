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
    public class PartialNode : TestRoslyn.Syntax.SyntaxNode
    {
        public string FullName { get; private set; }
        public IDomainSyntaxNode Domain { get; private set; }
        private bool _hasPartial;
        private SourceSpan _loc;

        protected override void InitCore(AstContext context, ParseTreeNode treeNode)
        {
            base.InitCore(context, treeNode);
            if (treeNode.ChildNodes.Count > 2)
            {
                FullName = treeNode.ChildNodes[2].Token.Text.Trim('"');
                _loc = treeNode.ChildNodes[2].Span;
                _hasPartial = true;
            }
        }

        //public override void AcceptVisitor(IAstVisitor visitor)
        //{
        //    base.AcceptVisitor(visitor);

        //    if (!_hasPartial)
        //        return;

        //    var ctx = visitor as SemanticAnalysisVisitor;
        //    if (String.IsNullOrWhiteSpace(FullName))
        //        ctx.Messages.Add(new LogMessage(ErrorLevel.Error, this._loc, "Invalid domain reference name", null));
        //    else
        //    {
        //        try
        //        {
        //            Domain = ctx.FindDomain(FullName);
        //            if (Domain == null)
        //            {
        //                if (Domain == null)
        //                    ctx.Messages.Add(new LogMessage(ErrorLevel.Error, this._loc, "Unknow domain " + FullName, null));
        //                else if( String.Compare( Domain.FullName, ctx.Domain.FullName, StringComparison.OrdinalIgnoreCase ) != 0)
        //                    ctx.Messages.Add(new LogMessage(ErrorLevel.Error, this._loc, "Partial domain name must be equal to referenced domain." + FullName, null));
        //            }
        //            else
        //            {
        //                Domain.RegisterAsPartial(ctx.Domain);
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            ctx.Messages.Add(new LogMessage(ErrorLevel.Error, this._loc, ex.Message, null));
        //        }
        //    }
        //}

        public override string ToString()
        {
            return String.Format("Partial for {0}", FullName);
        }
    }
}
