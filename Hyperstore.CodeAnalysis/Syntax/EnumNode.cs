using Irony;

using System;
using System.Collections.Generic;
using System.Linq;

namespace Hyperstore.Modeling.TextualLanguage
{
    public class EnumNode : TestRoslyn.Syntax.SyntaxNode, IEnumSyntaxNode
    {
        private List<string> _values = new List<string>();
        public List<GenerationAttributeNode> GenerationAttributes { get; private set; }
        public List<string> Values { get { return _values; } }
        public bool CanGenerate { get { return true; } }

        public bool IsPrimitive { get { return false; } }

        protected override void InitCore(Irony.Ast.AstContext context, Irony.Parsing.ParseTreeNode treeNode)
        {
            base.InitCore(context, treeNode);

            GenerationAttributes = new List<GenerationAttributeNode>();
            foreach (var ga in treeNode.ChildNodes[0].ChildNodes)
            {
                AddChild("GenerationAttribute", ga);
                GenerationAttributes.Add(ga.AstNode as GenerationAttributeNode);
            }

            Alias = FullName = treeNode.ChildNodes[3].FindTokenAndGetText();

            foreach (var child in treeNode.ChildNodes[4].ChildNodes)
            {
                if (child.IsPunctuationOrEmptyTransient())
                    continue;
                _values.Add(child.FindTokenAndGetText());
            }
        }

        //public override void AcceptVisitor(IAstVisitor visitor)
        //{
        //    base.AcceptVisitor(visitor);

        //    var ctx = (SemanticAnalysisVisitor)visitor;

        //    var duplicate = ctx.Domain.Externals.FirstOrDefault(c => c != this && c.Alias == Alias);
        //    if (duplicate != null)
        //        ctx.Messages.Add(new LogMessage(ErrorLevel.Error, this.Span, "Duplicate external name " + Alias, null));

        //    var q = from v in _values
        //            group v by v into g
        //            where g.Count() > 1
        //            select g.Key;

        //    foreach (var name in q)
        //    {
        //        ctx.Messages.Add(new LogMessage(ErrorLevel.Error, this.Span, String.Format("Duplicate value name {0} for enum {1}", name, Alias), null));
        //    }
        //}

        public string Alias
        {
            get;
            private set;
        }

        public string FullName
        {
            get;
            private set;
        }

        public ExternalKind Kind
        {
            get { return ExternalKind.Enum; }
        }
    }
}
