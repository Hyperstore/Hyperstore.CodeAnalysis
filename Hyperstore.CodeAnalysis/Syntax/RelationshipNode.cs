using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Irony;
using Irony.Ast;

using Irony.Parsing;

namespace Hyperstore.Modeling.TextualLanguage
{
    public class RelationshipNode : ClassNode, IRelationshipNode
    {
        public string End
        {
            get { return Definition.End;  }
        }

        public string Start
        {
            get { return Definition.Start; }
        }

        public string EndPropertyName
        {
            get;
            set;
        }

        public string StartPropertyName
        {
            get;
            set;
        }

        public RelationshipCardinality Cardinality
        {
            get { return Definition.Cardinality; }
            set { Definition.Cardinality = value; }
        }

        public bool IsEmbedded
        {
            get { return Definition.IsEmbedded; }
            set { Definition.IsEmbedded = value; }
        }

        public SourceSpan StartLocation
        {
            get { return Definition.StartLocation; }
        }
        public SourceSpan EndLocation
        {
            get { return Definition.EndLocation; }
        }

        public IRelationshipSyntaxNode Definition { get; private set; }

        public RelationshipNode(string name) : this()
        {
            DomainDefinitionName = Name = name;
        }

        public RelationshipNode() 
        {
        }

        //public override string SuperTypeName
        //{
        //    get { return String.Format("ModelRelationship"); }
        //}

        protected override void InitCore(AstContext context, ParseTreeNode treeNode)
        {
            base.InitCore(context, treeNode);
            Definition = treeNode.ChildNodes[6].AstNode as RelationshipDefinitionNode;
        }

        //public override void AcceptVisitor(IAstVisitor visitor)
        //{
        //    var ctx = (SemanticAnalysisVisitor)visitor;
        //    var domain = ctx.Domain;
        //    var superClass = GetSuperClass(domain);
        //    if (superClass != null && !(superClass is RelationshipNode))
        //        ctx.Messages.Add(new LogMessage(ErrorLevel.Error, this.Span, "Superclass must be a relationship : " + superClass, null));

        //    var virt = domain.Classes.FirstOrDefault(r => r is VirtualRelationshipNode && r.Name == this.Name);
        //    if (virt != null)
        //    {
        //        domain.Classes.Remove(virt);
        //    }

        //    base.AcceptVisitor(visitor);

        //    if (ctx.Domain.FindClass(Definition.Start) == null)
        //    {
        //        ctx.Messages.Add(new LogMessage(ErrorLevel.Error, Definition.StartLocation, "Unknow reference type " + Definition.Start, null));
        //    }
         
        //    if( ctx.Domain.FindClass(Definition.End) == null)
        //        ctx.Messages.Add(new LogMessage(ErrorLevel.Error, Definition.EndLocation, "Unknow reference type " + Definition.End, null));
        //}
    }

    public class VirtualRelationshipNode : IVirtualRelationship 
    {
        public VirtualRelationshipNode(string name)
        {
            DomainDefinitionName = Name = name;
        }

        public bool CanGenerate { get { return false; } }

        public string FullName
        {
            get { return Name; }
            
        }

        public string Name
        {
            get;
            set;
        }

        public string DomainDefinitionName
        {
            get;
            set;
        }

        public string End
        {
            get;
            set;
        }

        public string StartPropertyName
        {
            get;
            set;
        }

        public string EndPropertyName
        {
            get;
            set;
        }

        public RelationshipCardinality Cardinality
        {
            get;
            set;
        }

        public bool IsEmbedded
        {
            get;
            set;
        }

        public IEnumerable<AttributeNode> Attributes
        {
            get { yield break; }
        }

        public string Modifier
        {
            get { return null; }
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

        public IEnumerable<ReferenceNode> References
        {
            get { yield break; }
        }

        public bool IsA(DomainNode domain, string clazz)
        {
            return Name == clazz;
        }

        public IEnumerable<ConstraintNode> Constraints
        {
            get { yield break; }
        }


        public IEnumerable<GenerationAttributeNode> GenerationAttributes
        {
            get { yield break; }
        }

        public IEnumerable<string> Implements
        {
            get { yield break; }
        }

        public QualifiedNameNode Extends
        {
            get { return null; }
        }

        //public string SuperTypeName
        //{
        //    get { throw new NotImplementedException(); }
        //}

        //public IClassNode GetSuperClass(DomainNode domainNode)
        //{
        //    return null;
        //}

        public IEnumerable<string> Derived
        {
            get { return Enumerable.Empty<string>().ToList(); }
        }

        public bool HasClassInheritance { get { return false; } }


        public void AddDerived(string className)
        {
            throw new NotImplementedException();
        }

        public RelationshipCardinality? SourceCardinality
        {
            get;
            set;
        }

        public bool IsSourceEmbedded
        {
            get;
            set;
        }

        public RelationshipCardinality? OppositeCardinality
        {
            get;
            set;
        }

        public bool IsOppositeEmbedded
        {
            get;
            set;
        }
    }
}
