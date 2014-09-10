using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Irony;
using Irony.Ast;

using Irony.Parsing;

namespace Hyperstore.Modeling.TextualLanguage
{
    [Flags]
    public enum RelationshipCardinality
    {
        OneToOne=0,
        ManyToOne = 1,
        OneToMany = 2,
        ManyToMany = 3,
    }

    public class RelationshipDefinitionNode : TestRoslyn.Syntax.SyntaxNode, IRelationshipSyntaxNode
    {
        public string Start { get; set; }
        public SourceSpan StartLocation { get; private set; }
        public bool IsEmbedded { get; set; }
        public RelationshipCardinality Cardinality { get; set; }

        public string End { get; protected set; }
        public SourceSpan EndLocation { get; private set; }

        private bool startMany;
        private bool endMany;

        protected override void InitCore(AstContext context, ParseTreeNode treeNode)
        {
            base.InitCore(context, treeNode);

            Start = treeNode.ChildNodes[0].Token.ValueString;
            StartLocation = treeNode.ChildNodes[0].Span;

            IsEmbedded = treeNode.ChildNodes[2].ChildNodes[0].Term.Name == "Embedded";
            var qn = treeNode.ChildNodes[3].AstNode as QualifiedNameNode;
            if (qn != null)
            {
                End = qn.ToString();
            }
            EndLocation = treeNode.ChildNodes[3].Span;
            endMany = treeNode.ChildNodes[4].ChildNodes.Count > 0;
            startMany = treeNode.ChildNodes[1].ChildNodes.Count > 0;

            if (endMany)
            {
                if (startMany)
                    Cardinality = RelationshipCardinality.ManyToMany;
                else
                    Cardinality = RelationshipCardinality.OneToMany;
            }
            else
            {
                if (startMany)
                    Cardinality = RelationshipCardinality.ManyToOne;
                else
                    Cardinality = RelationshipCardinality.OneToOne;
            }

        }

    }
}
