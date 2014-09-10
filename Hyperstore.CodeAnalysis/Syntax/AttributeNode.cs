using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Irony;
using Irony.Ast;

using Irony.Parsing;

namespace Hyperstore.Modeling.TextualLanguage
{
    public class AttributeNode : TestRoslyn.Syntax.SyntaxNode, IAttributeSyntaxNode
    {
        public string Name { get; private set; }
        public string Type { get; private set; }
        public string DefaultValue { get; private set; }
        public List<GenerationAttributeNode> GenerationAttributes { get; private set; }

        public string Clause { get; private set; }
        public string ReturnType { get; private set; }

        public IEntitySyntaxNode ParentClassNode { get { return Parent as IEntitySyntaxNode; } }
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

            //            Modifier = treeNode.ChildNodes[1].TestRoslyn.Syntax.SyntaxNode as CSharpCodeNode;
            Name = treeNode.ChildNodes[1].FindTokenAndGetText();
            var qn = treeNode.ChildNodes[2].AstNode as QualifiedNameNode;
            if (qn != null)
                Type = qn.ToString();

            _constraints = new List<ConstraintNode>();
            foreach (var child in treeNode.ChildNodes[4].ChildNodes)
            {
                _constraints.Add(child.AstNode as ConstraintNode);
            }

            var defaultValueNode = treeNode.ChildNodes[3];
            if (defaultValueNode.ChildNodes.Count > 0)
            {
                Clause = defaultValueNode.ChildNodes[0].FindTokenAndGetText();
                DefaultValue = defaultValueNode.ChildNodes[1].Token.ValueString;
                if (defaultValueNode.ChildNodes.Count > 2)
                    ReturnType = defaultValueNode.ChildNodes[2].FindTokenAndGetText();
            }

        }

        //public override void AcceptVisitor(IAstVisitor visitor)
        //{
        //    base.AcceptVisitor(visitor);

        //    var ctx = (SemanticAnalysisVisitor)visitor;
        //    var domain = ctx.Domain;

        //    var rel = ctx.Domain.FindClass(Type) as IRelationshipNode;
        //    if (rel != null)
        //    {
        //        ctx.PendingActions.Add(() =>
        //            {
        //                if (Clause != "where" && Clause != "select")
        //                    ctx.Messages.Add(new LogMessage(ErrorLevel.Error, this.Span, "Only where expression is allowed on a relationship reference.", null));
        //                else if (DefaultValue == null)
        //                    ctx.Messages.Add(new LogMessage(ErrorLevel.Error, this.Span, Clause + " clause can not be empty.", null));

        //                if (rel.Cardinality == RelationshipCardinality.OneToOne && DefaultValue != null)
        //                    ctx.Messages.Add(new LogMessage(ErrorLevel.Error, this.Span, Clause + " clause is only valid on a multiple cardinality", null));
        //                if (Constraints.Count() > 0)
        //                    ctx.Messages.Add(new LogMessage(ErrorLevel.Error, this.Span, "Constraints are not allowed on a reference.", null));
        //            });
        //    }
        //    else
        //    {
        //        if (domain.FindExternalType(Type) == null)
        //            ctx.Messages.Add(new LogMessage(ErrorLevel.Error, this.Span, "Unknow type. Must be a primitive types (string,int, double, bool, datetime, timespan, single..) or an external types " + Type, null));
        //        else
        //        {
        //            if (Clause != null)
        //            {
        //                if (Clause != "=")
        //                    ctx.Messages.Add(new LogMessage(ErrorLevel.Error, this.Span, "Only default value expression is allowed on an attribute.", null));
        //                else if (DefaultValue == null)
        //                    ctx.Messages.Add(new LogMessage(ErrorLevel.Error, this.Span, "Default value can not be empty.", null));
        //            }
        //        }
        //    }


        //    var parent = this.Parent as IClassNode;
        //    var duplicate = parent.Attributes.Any(a => a.Name == Name && a != this) || parent.References.Any(r => r.Name == Name);
        //    if (duplicate)
        //        ctx.Messages.Add(new LogMessage(ErrorLevel.Error, this.Span, "Duplicate attribute name " + Name, null));

        //    if (ReturnType != null)
        //    {
        //        if (Clause == "select")
        //        {
        //            if (domain.FindExternalType(ReturnType) == null && domain.FindClass(ReturnType) == null)
        //                ctx.Messages.Add(new LogMessage(ErrorLevel.Error, this.Span, "Unknow return type " + Type, null));
        //        }
        //        else
        //            ctx.Messages.Add(new LogMessage(ErrorLevel.Error, this.Span, "Syntax error - This definition doesn't support return type", null));
        //    }

        //    var cx = 0;
        //    foreach (var constraint in Constraints)
        //    {
        //        constraint.AcceptVisitor(visitor);
        //        if (constraint.Kind == ConstraintKind.Compute)
        //        {
        //            cx++;
        //            if (cx > 1)
        //                ctx.Messages.Add(new LogMessage(ErrorLevel.Error, constraint.Span, "Duplicate compute statement", null));
        //        }
        //    }
        //}
    }
}
