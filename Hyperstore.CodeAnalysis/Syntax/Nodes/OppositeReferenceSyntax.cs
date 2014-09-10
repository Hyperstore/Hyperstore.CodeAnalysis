using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hyperstore.CodeAnalysis.Syntax
{
    public sealed class OppositeReferenceSyntax : MemberDeclarationSyntax
    {
        public SyntaxToken SourceMultiplicity { get; private set; }
        public SyntaxToken TargetMultiplicity { get; private set; }
        public SyntaxToken Kind { get; private set; }
        public QualifiedNameSyntax TargetType { get; private set; }
        public QualifiedNameSyntax RelationshipName { get; private set; }

        protected override void InitCore(Irony.Ast.AstContext context, Irony.Parsing.ParseTreeNode treeNode)
        {
            base.InitCore(context, treeNode);

            Attributes = treeNode.ChildNodes[0].AstNode as ListSyntax<AttributeSyntax>;
            AddChild(Attributes);

            Name = new SyntaxToken(treeNode.ChildNodes[1].Token);
            AddChild(Name);

            if (treeNode.ChildNodes[2].ChildNodes.Count > 0)
            {
                SourceMultiplicity = new SyntaxToken(treeNode.ChildNodes[2].ChildNodes[0].Token);
                AddChild(SourceMultiplicity);
            }

            Kind = new SyntaxToken(treeNode.ChildNodes[3].ChildNodes[0].Token);
            AddChild(Kind);

            TargetType = treeNode.ChildNodes[4].AstNode as QualifiedNameSyntax;
            AddChild(TargetType);

            if (treeNode.ChildNodes[5].ChildNodes.Count > 0)
            {
                TargetMultiplicity = new SyntaxToken(treeNode.ChildNodes[5].ChildNodes[0].Token);
                AddChild(TargetMultiplicity);
            }

            if (treeNode.ChildNodes[6].ChildNodes.Count > 0)
            {
                RelationshipName = treeNode.ChildNodes[6].ChildNodes[0].AstNode as QualifiedNameSyntax;
                AddChild(RelationshipName);
            }
        }
    }
}
