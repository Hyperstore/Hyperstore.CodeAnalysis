using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Irony;


namespace Hyperstore.Modeling.TextualLanguage
{
    public enum ConstraintKind
    {
        Check,
        Validate,
        Compute
    }

    public class ConstraintNode : TestRoslyn.Syntax.SyntaxNode
    {
        public string Message { get; private set; }
        public string ConstraintCode { get; private set; }
        public ConstraintKind Kind { get; private set; }

        protected override void InitCore(Irony.Ast.AstContext context, Irony.Parsing.ParseTreeNode treeNode)
        {
            base.InitCore(context, treeNode);
            switch (treeNode.ChildNodes[0].FindTokenAndGetText())
            {
                case "check":
                    Kind = ConstraintKind.Check;
                    break;
                case "validate":
                    Kind = ConstraintKind.Validate;
                    break;
                case "compute":
                    Kind = ConstraintKind.Compute;
                    break;
            }

            Message = treeNode.ChildNodes[1].FindTokenAndGetText();
            AddChild("Message", treeNode.ChildNodes[1]);

            ConstraintCode = treeNode.ChildNodes[2].FindTokenAndGetText();
            AddChild("Code", treeNode.ChildNodes[2]);
        }

        //public override void AcceptVisitor(IAstVisitor visitor)
        //{
        //    base.AcceptVisitor(visitor);

        //    var ctx = (SemanticAnalysisVisitor)visitor;
        //    var domain = ctx.Domain;

        //    if (Kind != ConstraintKind.Compute && String.IsNullOrEmpty(Message))
        //        ctx.Messages.Add(new LogMessage(ErrorLevel.Error, this.Span, "Message can be empty", null));

        //    if( String.IsNullOrEmpty( ConstraintCode ) )
        //        ctx.Messages.Add( new LogMessage( ErrorLevel.Error, this.Span, "Code is required", null ) );
        //    else
        //        ConstraintCode = ConstraintCode.TrimEnd( ';' );
        //}
    }
}
