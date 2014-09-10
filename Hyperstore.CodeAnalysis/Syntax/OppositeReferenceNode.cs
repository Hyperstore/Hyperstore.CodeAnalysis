using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Irony;
using Irony.Ast;

using Irony.Parsing;

namespace Hyperstore.Modeling.TextualLanguage
{
    public class OppositeReferenceNode : ReferenceNode
    {
        class RelationshipDefinition : IRelationshipSyntaxNode
        {
            public string End
            {
                get;
                set;
            }

            public bool IsEmbedded
            {
                get;
                set;
            }

            public bool Many
            {
                get { return false; }
            }

            public string Start
            {
                get;
                set;
            }
            public SourceSpan StartLocation
            {
                get;
                set;
            }
            public SourceSpan EndLocation
            {
                get;
                set;
            }

            public RelationshipCardinality Cardinality
            {
                get;
                set;
            }
        }

        //public override void AcceptVisitor(IAstVisitor visitor)
        //{
        //    this.Name = Definition.Start;
        //    base.AcceptVisitor(visitor);
        //}

        protected override void Initialize(ParseTreeNode treeNode)
        {
            var def = new RelationshipDefinition();
            def.Start = treeNode.ChildNodes[1].Token.ValueString;
            def.StartLocation = treeNode.ChildNodes[1].Span;

            def.IsEmbedded = treeNode.ChildNodes[3].ChildNodes[0].Term.Name == "EmbeddedOppositeReference";
            var qn = treeNode.ChildNodes[4].AstNode as QualifiedNameNode;
            if (qn != null)
            {
                def.End = qn.ToString();
            }
            def.EndLocation = treeNode.ChildNodes[4].Span;
            var endMany = treeNode.ChildNodes[5].ChildNodes.Count > 0;
            var startMany = treeNode.ChildNodes[2].ChildNodes.Count > 0;

            if (endMany)
            {
                if (startMany)
                    def.Cardinality = RelationshipCardinality.ManyToMany;
                else
                    def.Cardinality = RelationshipCardinality.ManyToOne;
            }
            else
            {
                if (startMany)
                    def.Cardinality = RelationshipCardinality.OneToMany;
                else
                    def.Cardinality = RelationshipCardinality.OneToOne;
            }


            Type = treeNode.ChildNodes[6].FindTokenAndGetText();

            Definition = def;
        }

        //protected override void CheckRelationship(SemanticAnalysisVisitor ctx)
        //{
        //    var rel = ctx.Domain.FindClass(Type) as IRelationshipNode;

        //    if (rel == null)
        //    {
        //        var vrel = new VirtualRelationshipNode(Type);
        //        ctx.Domain.Classes.Add(vrel);
        //        vrel.End = ((ClassNode)this.Parent).Name;
        //        //   vrel.EndLocation = ((ClassNode)this.Parent).Location;
        //        vrel.Start = this.Definition.End;
        //        vrel.StartLocation = Definition.EndLocation;
        //        vrel.OppositeCardinality = this.Definition.Cardinality;
        //        vrel.IsOppositeEmbedded = this.Definition.IsEmbedded;
        //        vrel.EndPropertyName = this.Name;
        //    }
        //    else
        //    {
        //        var vrel2 = rel as IVirtualRelationship;
        //        if (vrel2 != null)
        //        {
        //            if (vrel2.OppositeCardinality != null)
        //            {
        //                ctx.Messages.Add(new LogMessage(ErrorLevel.Error, this.Span, "Duplicate relationship " + Type, null));
        //            }
        //            else
        //            {
        //                vrel2.OppositeCardinality = this.Definition.Cardinality;
        //                vrel2.IsOppositeEmbedded = this.Definition.IsEmbedded;
        //                vrel2.EndPropertyName = this.Name;
        //            }
        //        }
        //        rel.EndPropertyName = this.Name;
        //    }

        //    // Ce controle ne pourra se faire que quand toutes les relations auront été traitées
        //    ctx.PendingActions.Add(() =>
        //    {
        //        CheckVRelationshipConsistence(ctx);
        //    });
        //}

        //private void CheckVRelationshipConsistence(SemanticAnalysisVisitor ctx)
        //{
        //    var rel = ctx.Domain.FindClass(Type) as IRelationshipNode;
        //    if (rel == null)
        //    {
        //        ctx.Messages.Add(new LogMessage(ErrorLevel.Error, this.Span, "Unknow relationship " + Type, null));
        //        return;
        //    }

        //    if (!((ClassNode)this.Parent).IsA(ctx.Domain, rel.End))
        //        ctx.Messages.Add(new LogMessage(ErrorLevel.Error, this.Span, String.Format("Relationship type mismatch. Cannot convert {0} to {1}", ((ClassNode)this.Parent).Name, rel.End), null));

        //    var vrel = rel as IVirtualRelationship;
        //    if (vrel != null)
        //    {
        //        if (vrel.SourceCardinality != null)
        //        {
        //            if (vrel.IsSourceEmbedded && Definition.IsEmbedded)
        //                ctx.Messages.Add(new LogMessage(ErrorLevel.Error, this.Span, String.Format("Embedded cannot be defined at the two side of relationship {0}.", Type), null));

        //            // La redéfinition des cardinalités est facultative dans une relation opposée
        //            if (this.Definition.Cardinality != RelationshipCardinality.OneToOne && vrel.SourceCardinality != this.Definition.Cardinality)
        //            {
        //                ctx.Messages.Add(new LogMessage(ErrorLevel.Error, this.Span, String.Format("Relationship cardinality mismatch."), null));
        //                return;
        //            }

        //            vrel.Cardinality = vrel.SourceCardinality.Value;
        //            if (vrel.IsSourceEmbedded)
        //                vrel.IsEmbedded = true;
        //        }
        //        else
        //        {
        //            vrel.IsEmbedded = Definition.IsEmbedded;
        //            vrel.Cardinality = Definition.Cardinality;
        //        }
        //    }
        //    else
        //    {

        //        if (Definition.Cardinality != rel.Cardinality)
        //            ctx.Messages.Add(new LogMessage(ErrorLevel.Error, this.Span, String.Format("Relationship cardinality mismatch."), null));
        //    }
        //}
    }
}
