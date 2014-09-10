using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Irony;
using Irony.Ast;

using Irony.Parsing;

namespace Hyperstore.Modeling.TextualLanguage
{
    public enum ExternalKind
    {
        Normal,
        Interface,
        Enum
    }

    public class ExternalNode : TestRoslyn.Syntax.SyntaxNode, IExternalSyntaxNode
    {
        public string Alias { get; private set; }
        public string FullName { get; private set; }
        public ExternalKind Kind { get; private set; }
        public bool CanGenerate { get { return true; } }

        public bool IsPrimitive { get { return false; } }

        protected override void InitCore(AstContext context, ParseTreeNode treeNode)
        {
            base.InitCore(context, treeNode);
            FullName = treeNode.ChildNodes[2].FindTokenAndGetText();
            AddChild("QualifiedName", treeNode.ChildNodes[2]);
            
            switch (treeNode.ChildNodes[1].FindTokenAndGetText())
            {
                case "interface":
                    Kind = ExternalKind.Interface;
                    break;
                case "enum":
                    Kind = ExternalKind.Enum;
                    break;
                default:
                    Kind = ExternalKind.Normal;
                    break;
            }

            var alias = treeNode.ChildNodes[3];
            if (alias.ChildNodes.Count > 0)
            {
                Alias = alias.ChildNodes[1].FindTokenAndGetText();
                AddChild("Alias", alias.ChildNodes[1]);
            }
            else
                Alias = FullName;
        }

        //public override void AcceptVisitor(IAstVisitor visitor)
        //{
        //    base.AcceptVisitor(visitor);

        //    var ctx = (SemanticAnalysisVisitor)visitor;
        //    var domain = ctx.Domain;

        //    var ex = domain.Externals.FirstOrDefault(e => e.Alias == Alias);
        //    if (ex != null && ex != this)
        //        ctx.Messages.Add(new LogMessage(ErrorLevel.Error, this.Span, "Duplicate extern name " + Alias, null));

        //}

        public override string ToString()
        {
            return String.Format("External {0} = {1}", Alias, FullName);
        }
    }
}
