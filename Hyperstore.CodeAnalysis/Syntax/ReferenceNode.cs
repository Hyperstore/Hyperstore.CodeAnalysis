using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Irony;
using Irony.Ast;

using Irony.Parsing;

namespace Hyperstore.Modeling.TextualLanguage
{
    public class ReferenceNode : TestRoslyn.Syntax.SyntaxNode, IReferenceSyntaxNode
    {
        public string Type { get; protected set; }
        public IRelationshipSyntaxNode Definition { get; protected set; }
        public string Name { get; protected set; }
        //public CSharpCodeNode Modifier { get; protected set; }
        public List<GenerationAttributeNode> GenerationAttributes { get; private set; }

        protected override void InitCore(AstContext context, ParseTreeNode treeNode)
        {
            base.InitCore(context, treeNode);

            GenerationAttributes = new List<GenerationAttributeNode>();
            foreach (var ga in treeNode.ChildNodes[0].ChildNodes)
            {
                AddChild("GenerationAttribute", ga);
                GenerationAttributes.Add(ga.AstNode as GenerationAttributeNode);
            }

            Initialize(treeNode);
        }

        protected virtual void Initialize(ParseTreeNode treeNode)
        {
           // Modifier = treeNode.ChildNodes[1].TestRoslyn.Syntax.SyntaxNode as CSharpCodeNode;
            Definition = treeNode.ChildNodes[1].AstNode as RelationshipDefinitionNode;
            Name = Definition.Start;

            if( treeNode.ChildNodes[2].ChildNodes.Count > 0)
                Type = ((QualifiedNameNode)treeNode.ChildNodes[2].ChildNodes[0].AstNode).Name;
        }


        //public override void AcceptVisitor(IAstVisitor visitor)
        //{
        //    base.AcceptVisitor(visitor);

        //    var ctx = (SemanticAnalysisVisitor)visitor;
        //    var domain = ctx.Domain;

        //    var parent = this.Parent as IClassNode;
        //    var duplicate = parent.References.FirstOrDefault(a => a.Name == Name);
        //    if (duplicate != null && duplicate != this)
        //        ctx.Messages.Add(new LogMessage(ErrorLevel.Error, this.Span, "Duplicate attribute name " + Name, null));

        //    this.Definition.Start = ((ClassNode)this.Parent).Name;
        //    if (ctx.Domain.FindClass(Definition.End) == null)
        //    {
        //        ctx.Messages.Add(new LogMessage(ErrorLevel.Error, Definition.EndLocation, "Unknow reference name " + Definition.End, null));
        //    }
        //    if( Type == null )
        //    {
        //        var clazz = ctx.Domain.FindClass(Definition.End);
        //        if (clazz != null)
        //        {
        //            Type = String.Format("{0}{1}{2}", parent.Name, Definition.IsEmbedded ? "Has" : "References", clazz.Name);
        //        }
        //    }

        //    if( Type != null)
        //        CheckRelationship(ctx);
        //}

        //protected virtual void CheckRelationship(SemanticAnalysisVisitor ctx)
        //{
        //    var rel = ctx.Domain.FindClass(Type) as IRelationshipNode;
        //    bool createNewRelationship = rel == null;

        //    if(rel != null)
        //    {
        //        var vrel2 = rel as IVirtualRelationship;
        //        if (vrel2 != null)
        //        {
        //            if (vrel2.SourceCardinality != null)
        //            {
        //               // if (this.Definition.Cardinality != vrel2.SourceCardinality || this.Definition.IsEmbedded != vrel2.IsSourceEmbedded)
        //                {
        //                    createNewRelationship = true;
        //                    var cx = ctx.Domain.AllClasses.Where(c => c.Name.StartsWith(Type)).Count();
        //                    Type += cx.ToString();
        //                }
        //            }
        //            else
        //            {
        //                vrel2.SourceCardinality = this.Definition.Cardinality;
        //                vrel2.IsSourceEmbedded = this.Definition.IsEmbedded;
        //            }
        //        }
        //        rel.StartPropertyName = this.Name;
        //    }

        //    if (createNewRelationship)
        //    {
        //        var vrel = new VirtualRelationshipNode(Type);
        //        ctx.Domain.Classes.Add(vrel);
        //        vrel.Start = ((ClassNode)this.Parent).Name;
        //        vrel.StartPropertyName = this.Name;
        //        //   vrel.EndLocation = ((ClassNode)this.Parent).Location;
        //        vrel.End = this.Definition.End;
        //        vrel.EndLocation = Definition.EndLocation;
        //        vrel.SourceCardinality = vrel.Cardinality = this.Definition.Cardinality;
        //        vrel.IsSourceEmbedded = vrel.IsEmbedded = this.Definition.IsEmbedded;
        //    }
        //    else

        //    // Ce controle ne pourra se faire que quand toutes les relations auront été traitées
        //    ctx.PendingActions.Add(() =>
        //    {
        //        CheckRelationshipConsistence(ctx, this);
        //    });
        //}

        //private static void CheckRelationshipConsistence(SemanticAnalysisVisitor ctx, ReferenceNode self)
        //{
        //    var rel = ctx.Domain.FindClass(self.Type) as IRelationshipNode;
        //    if (rel == null)
        //    {
        //        ctx.Messages.Add(new LogMessage(ErrorLevel.Error, self.Span, "Unknow relationship " + self.Type, null));
        //        return;
        //    }

        //    if (!((ClassNode)self.Parent).IsA(ctx.Domain, rel.Start))
        //        ctx.Messages.Add(new LogMessage(ErrorLevel.Error, self.Span, String.Format("Relationship type mismatch. Cannot convert {0} to {1}", ((ClassNode)self.Parent).Name, rel.Start), null));

        //    var vrel = rel as IVirtualRelationship;
        //    if (vrel != null)
        //    {
        //        if (vrel.IsOppositeEmbedded && self.Definition.IsEmbedded)
        //            ctx.Messages.Add(new LogMessage(ErrorLevel.Error, self.Span, String.Format("Embedded cannot be defined at the two side of relationship {0}.", self.Type), null));

        //        if (vrel.OppositeCardinality != null)
        //        {
        //            if (vrel.OppositeCardinality != self.Definition.Cardinality)
        //            {
        //                ctx.Messages.Add(new LogMessage(ErrorLevel.Error, self.Span, String.Format("Relationship cardinality mismatch."), null));
        //                return;
        //            }

        //        }

        //        vrel.Cardinality = self.Definition.Cardinality;
        //    }
        //    else
        //    {
        //        if (self.Definition.Cardinality != rel.Cardinality)
        //            ctx.Messages.Add(new LogMessage(ErrorLevel.Error, self.Span, String.Format("Relationship cardinality mismatch."), null));
        //    }
        //}
    }
}
