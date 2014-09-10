using Irony;
using Irony.Ast;

using Irony.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Hyperstore.Modeling.TextualLanguage
{
    public class ClassNode : TestRoslyn.Syntax.SyntaxNode, IEntitySyntaxNode
    {
        public bool HasClassInheritance { get; private set; }

        private ICollection<string> _derived;
        public IEnumerable<string> Derived { get { return _derived; } }
        public string Name { get; protected set; }
        public string DomainDefinitionName { get; protected set; }
        public string FullName { get; protected set; }
        public QualifiedNameNode Extends { get; private set; }

        private List<string> _implements;
        public IEnumerable<string> Implements { get { return _implements; } }

        private List<AttributeNode> _attributes;
        public IEnumerable<AttributeNode> Attributes { get { return _attributes; } }

        private List<ReferenceNode> _references;
        public IEnumerable<ReferenceNode> References { get { return _references; } }

        private List<GenerationAttributeNode> _generationAttributes;
        public IEnumerable<GenerationAttributeNode> GenerationAttributes { get { return _generationAttributes; } }
        private List<ConstraintNode> _constraints;
        public IEnumerable<ConstraintNode> Constraints { get { return _constraints; } }

        public bool CanGenerate { get { return true; } }

        public ClassNode()
        {
            _derived = new List<string>();
            _attributes = new List<AttributeNode>();
            _references = new List<ReferenceNode>();
        }

        public void AddDerived(string className)
        {
            _derived.Add(className);
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
            DomainDefinitionName = Name = treeNode.ChildNodes[3].FindTokenAndGetText();
            if (treeNode.ChildNodes[4].ChildNodes.Count > 0)
                Extends = treeNode.ChildNodes[4].ChildNodes[1].AstNode as QualifiedNameNode;

            if (treeNode.ChildNodes[5].ChildNodes.Count > 0)
            {
                foreach (var implements in treeNode.ChildNodes[5].ChildNodes[1].ChildNodes)
                {
                    if (Implements == null)
                        _implements = new List<string>();
                    _implements.Add(implements.ChildNodes[0].FindTokenAndGetText());
                }
            }

            var index = this is RelationshipNode ? 7 : 6;
            foreach (var child in treeNode.ChildNodes[index].ChildNodes)
            {
                var node = child.AstNode as AttributeNode;
                if (node != null)
                {
                    _attributes.Add(node);
                }
                else
                {
                    var reference = child.AstNode as ReferenceNode;
                    _references.Add(reference);
                }

                AddChild(null, child);
            }

            _constraints = new List<ConstraintNode>();
            var constraintsNodes = treeNode.ChildNodes[index + 1];
            if (constraintsNodes.ChildNodes.Count > 0)
            {
                foreach (var child in constraintsNodes.ChildNodes[0].ChildNodes)
                {
                    _constraints.Add(child.AstNode as ConstraintNode);
                }
            }
        }

        //public override void AcceptVisitor(IAstVisitor visitor)
        //{
        //    base.AcceptVisitor(visitor);

        //    var ctx = (SemanticAnalysisVisitor)visitor;
        //    var domain = ctx.Domain;

        //    DomainDefinitionName = String.Format("{0}Definition.{1}", domain.FullName, Name);
        //    FullName = !String.IsNullOrEmpty(domain.Namespace) ? String.Format("global::{0}.{1}", domain.Namespace, Name) : Name;

        //    foreach (var constraint in Constraints)
        //    {
        //        constraint.AcceptVisitor(visitor);
        //        if( constraint.Kind == ConstraintKind.Compute )
        //            ctx.Messages.Add(new LogMessage(ErrorLevel.Error, this.Span, "Incorrect constraint", null));
        //    }


        //    if (domain.IsPartial)
        //    {
        //        var partialClass = domain.PartialFor.Domain.Classes.FirstOrDefault(c => c.Name == Name);
        //        if (partialClass != null)
        //        {

        //            if (Extends != null && partialClass.Extends != null && Extends != partialClass.Extends)
        //                ctx.Messages.Add(new LogMessage(ErrorLevel.Error, this.Span, String.Format("Incorrect inheritance for {0} must be {1} or nothing", Name, partialClass.Extends), null));
        //            else if (Extends != null && partialClass.Extends == null)
        //            {
        //                IClassNode baseClass = null;
        //                if ((baseClass = domain.FindClass(Extends.Name)) == null && !domain.Externals.Any(c => c.Alias == Extends.Name))
        //                    ctx.Messages.Add(new LogMessage(ErrorLevel.Error, Extends.Span, "Unknow base type " + Extends, null));

        //                if (baseClass != null)
        //                {
        //                    baseClass.AddDerived(this.Name);
        //                    HasClassInheritance = true;
        //                }
        //            }
        //            var query = from a in partialClass.Attributes orderby a.Name group a by a.Name into g select g;
        //            foreach (var attName in (from a in query where a.Count() > 1 select a.Key))
        //            {
        //                ctx.Messages.Add(new LogMessage(ErrorLevel.Error, this.Span, "Duplicate attribute name " + attName, null));
        //            }

        //            var query2 = from a in partialClass.References orderby a.Name group a by a.Name into g select g;
        //            foreach (var attName in (from a in query2 where a.Count() > 1 select a.Key))
        //            {
        //                ctx.Messages.Add(new LogMessage(ErrorLevel.Error, this.Span, "Duplicate attribute name " + attName, null));
        //            }
        //            return;
        //        }
        //    }

        //    var duplicate = domain.Classes.FirstOrDefault(c => c != this && c.Name == Name);
        //    if (duplicate != null)
        //        ctx.Messages.Add(new LogMessage(ErrorLevel.Error, this.Span, "Duplicate type name " + Name, null));

        //    if (Extends != null)
        //    {
        //        IClassNode baseClass = null;
        //        if ((baseClass = domain.FindClass(Extends.Name)) == null && !domain.Externals.Any(c => c.Alias == Extends.Name))
        //            ctx.Messages.Add(new LogMessage(ErrorLevel.Error, Extends.Span, "Unknow base type " + Extends, null));

        //        if (baseClass != null)
        //        {
        //            baseClass.AddDerived(this.Name);
        //            HasClassInheritance = true;
        //        }
        //    }

        //    if (Implements != null)
        //    {
        //        foreach (var implements in Implements)
        //        {
        //            if (implements == Name)
        //                ctx.Messages.Add(new LogMessage(ErrorLevel.Error, this.Span, "Circular base class dependency " + implements, null));

        //            if (!domain.Externals.Any(c => c.Alias == implements))
        //                ctx.Messages.Add(new LogMessage(ErrorLevel.Error, this.Span, "The base class must be declared first for " + implements + " and other extension must be declared as external.", null));
        //        }
        //    }
        //}

        //public bool IsA(DomainNode domain, string clazz)
        //{
        //    if (clazz == Name)
        //        return true;

        //    if (Extends != null)
        //    {
        //        var ext = domain.FindClass(Extends.Name);
        //        return ext.IsA(domain, clazz);
        //    }
        //    return false;
        //}

        //public virtual string SuperTypeName
        //{
        //    get { return "ModelEntity"; }
        //}

        //public IClassNode GetSuperClass(DomainNode domain)
        //{
        //    if (Extends != null)
        //        return domain.FindClass(Extends.Name);

        //    return null;

        //}

    }
}
