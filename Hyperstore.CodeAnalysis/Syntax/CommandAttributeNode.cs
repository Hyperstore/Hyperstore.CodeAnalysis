using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Irony;
using Irony.Ast;

using Irony.Parsing;

namespace Hyperstore.Modeling.TextualLanguage
{
    public class CommandAttributeNode : TestRoslyn.Syntax.SyntaxNode
    {
        public bool IsEntity { get; private set; }
        public string Name { get; private set; }
        public string Type { get; private set; }
        public List<GenerationAttributeNode> GenerationAttributes { get; private set; }

        private List<ConstraintNode> _constraints;
        public IEnumerable<ConstraintNode> Constraints { get { return _constraints; } }

        protected override void InitCore(AstContext context, ParseTreeNode treeNode)
        {
            base.InitCore(context, treeNode);

            GenerationAttributes = new List<GenerationAttributeNode>();
            foreach (var ga in treeNode.ChildNodes[0].ChildNodes)
            {
                AddChild("GenerationAttribute", ga);
                GenerationAttributes.Add(ga.AstNode as GenerationAttributeNode);
            }

            Name = treeNode.ChildNodes[1].FindTokenAndGetText();
            var qn = treeNode.ChildNodes[2].AstNode as QualifiedNameNode;
            if (qn != null)
                Type = qn.ToString();

            _constraints = new List<ConstraintNode>();
            foreach (var child in treeNode.ChildNodes[3].ChildNodes)
            {
                _constraints.Add(child.AstNode as ConstraintNode);
            }
        }

        //public override void AcceptVisitor(IAstVisitor visitor)
        //{
        //    base.AcceptVisitor(visitor);

        //    var ctx = (SemanticAnalysisVisitor)visitor;
        //    var domain = ctx.Domain;

        //    IsEntity = ctx.Domain.FindClass(Type) != null;
        //    if (!IsEntity)
        //    {
        //        if (domain.FindExternalType(Type) == null)
        //            ctx.Messages.Add(new LogMessage(ErrorLevel.Error, this.Span, "Unknow type. Must be an entity, a primitive type (string,int, double, bool, datetime, timespan, single..) or an external types " + Type, null));
        //    }

        //    foreach (var constraint in Constraints)
        //    {
        //        constraint.AcceptVisitor(visitor);
        //        if (constraint.Kind != ConstraintKind.Check)
        //        {
        //                ctx.Messages.Add(new LogMessage(ErrorLevel.Error, constraint.Span, "Only check statement are allowed.", null));
        //        }
        //    }
        //}
    }
}
