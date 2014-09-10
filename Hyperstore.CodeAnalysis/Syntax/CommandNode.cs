using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Irony.Ast;
using Irony.Parsing;
using Irony;

namespace Hyperstore.Modeling.TextualLanguage
{
    public class CommandNode : TestRoslyn.Syntax.SyntaxNode, ICommandSyntaxNode
    {
        private List<CommandAttributeNode> _attributes;
        public IEnumerable<CommandAttributeNode> Attributes { get { return _attributes; } }

        private List<GenerationAttributeNode> _generationAttributes;
        public IEnumerable<GenerationAttributeNode> GenerationAttributes { get { return _generationAttributes; } }
        public string Name { get; protected set; }
        public string FullName { get; protected set; }

        public CommandNode()
        {
            _attributes = new List<CommandAttributeNode>();
        }

        protected override void InitCore(AstContext context, ParseTreeNode treeNode)
        {
            base.InitCore(context, treeNode);

            _generationAttributes = new List<GenerationAttributeNode>();
            foreach (var ga in treeNode.ChildNodes[0].ChildNodes)
            {
                AddChild("GenerationAttribute", ga);
                _generationAttributes.Add(ga.AstNode as GenerationAttributeNode);
            }

            //   CSharpCode = treeNode.ChildNodes[2].TestRoslyn.Syntax.SyntaxNode as CSharpCodeNode;
            //   AddChild("Modifier", treeNode.ChildNodes[2]);
            Name = treeNode.ChildNodes[3].FindTokenAndGetText();

            var attributes = treeNode.ChildNodes[4];
            if (attributes.ChildNodes.Count > 0)
            {
                foreach (var child in attributes.ChildNodes[0].ChildNodes)
                {
                    var node = child.AstNode as CommandAttributeNode;
                    if (node != null)
                    {
                        _attributes.Add(node);
                    }

                    AddChild("Attribute", child);
                }
            }
        }

        //public override void AcceptVisitor(IAstVisitor visitor)
        //{
        //    base.AcceptVisitor(visitor);

        //    var ctx = (SemanticAnalysisVisitor)visitor;
        //    var domain = ctx.Domain;

        //    FullName = !String.IsNullOrEmpty(domain.Namespace) ? String.Format("global::{0}.{1}", domain.Namespace, Name) : Name;


        //    var query = from a in Attributes orderby a.Name group a by a.Name into g select g;
        //    foreach (var attName in (from a in query where a.Count() > 1 select a.Key))
        //    {
        //        ctx.Messages.Add(new LogMessage(ErrorLevel.Error, this.Span, "Duplicate attribute name " + attName, null));
        //    }

        //    var duplicate = domain.Commands.FirstOrDefault(c => c != this && c.Name == Name);
        //    if (duplicate != null)
        //        ctx.Messages.Add(new LogMessage(ErrorLevel.Error, this.Span, "Duplicate type name " + Name, null));
        //    else
        //        domain.Commands.Add(this);
        //}
    }
}
