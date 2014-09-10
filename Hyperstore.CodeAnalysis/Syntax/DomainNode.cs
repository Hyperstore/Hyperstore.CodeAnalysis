using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Irony;
using Irony.Ast;

using Irony.Parsing;

namespace Hyperstore.Modeling.TextualLanguage
{
    public enum ExtensionMode
    {
        None,
        ReadOnly,
        Updatable
    }

    public class DomainNode : TestRoslyn.Syntax.SyntaxNode, IDomainSyntaxNode
    {
        public List<IDomainSyntaxNode> Partials { get; private set; }
        public string Name { get; private set; }
        public string Namespace { get; private set; }
        public List<IExternalSyntaxNode> Externals { get; private set; }
        public List<IEntitySyntaxNode> Classes { get; private set; }
        public List<ICommandSyntaxNode> Commands { get; private set; }
        public bool IsDynamic { get; private set; }
        public bool IsObservable { get; private set; }
        public ExtensionMode ExtensionMode { get; private set; }
        public string FullName { get; private set; }
        private List<UsesNode> _uses = new List<UsesNode>();
        private IDomainSyntaxNode _primitives;
        public Dictionary<string,string> Generators { get; private set; }
        public List<GenerationAttributeNode> GenerationAttributes { get; private set; }

        public bool IsPartial { get { return PartialFor.Domain != null; } }

        public PartialNode PartialFor { get;private set;}

        public IEnumerable<UsesNode> Uses { get { return _uses; } }

        public DomainNode()
        {
            Externals = new List<IExternalSyntaxNode>();
            Classes = new List<IEntitySyntaxNode>();
            _primitives = new PrimitivesDomainNode();
            Partials = new List<IDomainSyntaxNode>();
            Commands = new List<ICommandSyntaxNode>();
        }

        protected override void InitCore(AstContext context, ParseTreeNode treeNode)
        {
            base.InitCore(context, treeNode);

            GenerationAttributes = new List<GenerationAttributeNode>();
            foreach (var ga in treeNode.ChildNodes[0].ChildNodes)
            {
                AddChild("GenerationAttribute", ga);
                GenerationAttributes.Add(ga.AstNode as GenerationAttributeNode);
            }

            FullName = (treeNode.ChildNodes[2].AstNode as QualifiedNameNode).ToString();
            Namespace = String.Empty;
            Name = FullName;
            int pos = FullName.LastIndexOf('.');
            if (pos > 0)
            {
                Name = FullName.Substring(pos + 1);
                Namespace = FullName.Substring(0, pos);
            }

            PartialFor = treeNode.ChildNodes[3].AstNode as PartialNode;
           AddChild("Partial", treeNode.ChildNodes[3]);

            GetConfiguration(treeNode);

            foreach (var child in treeNode.ChildNodes[4].ChildNodes[0].ChildNodes)
            {
                var node = child.AstNode as UsesNode;
                _uses.Add(node);
                AddChild("Uses", child);
            }

            foreach (var child in treeNode.ChildNodes[4].ChildNodes[1].ChildNodes)
            {
                var node = child.AstNode as ExternalNode;
                Externals.Add(node);
                AddChild("External", child);
            }

            foreach (var child in treeNode.ChildNodes[4].ChildNodes[2].ChildNodes)
            {
                var node = child.AstNode as IEntitySyntaxNode;
                if (node != null)
                    Classes.Add(node);
                else if (child.AstNode is EnumNode)
                    Externals.Add(((EnumNode)child.AstNode));
                else if( child.AstNode is CommandNode)
                {

                }

                AddChild("Class", child);
            }
        }

        private void GetConfiguration(ParseTreeNode treeNode)
        {
            foreach(var arg in GenerationAttributes)
            {
                var term = arg.Name;
                if (term == "observable")
                    IsObservable = true;
                if (term == "dynamic")
                    IsDynamic = true;
                if( term == "extension" )
                {
                    ExtensionMode = arg.Arguments.Count == 0 ? ExtensionMode.Updatable : ExtensionMode.ReadOnly;
                }
            }
        }

        public override string ToString()
        {
            return "Domain " + Name;
        }

        //internal IExternalNode FindExternalType(string name)
        //{
        //    var ext = _primitives.Externals.FirstOrDefault(p => p.Alias == name);
        //    if( ext == null)
        //        ext = Externals.FirstOrDefault(e => e.Alias == name);
        //    if (ext == null && PartialFor.Domain != null)
        //        ext = PartialFor.Domain.Externals.FirstOrDefault(e => e.Alias == name);
        //    return ext;
        //}

        //internal IClassNode FindClass(string className)
        //{
        //    IDomainNode fromDomain;
        //    return FindClass(className, out fromDomain);
        //}

        //private IClassNode FindClass(string className, out IDomainNode fromDomain)
        //{
        //    var parts = className.Split('.');
        //    if (parts.Length == 1)
        //    {
        //        var classNode = this.AllClasses.FirstOrDefault(c => c.Name == className);
        //        if (classNode != null)
        //        {
        //            fromDomain = null;
        //            return classNode;
        //        }
        //        // Partials
        //        if (PartialFor.Domain != null)
        //        {
        //            classNode = PartialFor.Domain.Classes.FirstOrDefault(c => c.Name == className);
        //            if (classNode != null)
        //            {
        //                fromDomain = PartialFor.Domain;
        //                return classNode;
        //            }
        //        }
        //        // Primitives
        //        classNode = _primitives.Classes.FirstOrDefault(c => c.Name == className);
        //        if (classNode != null)
        //        {
        //            fromDomain = _primitives;
        //            return classNode;
        //        }
        //    }
        //    else
        //    {
        //        var use = _uses.FirstOrDefault(u => u.Alias == parts[0]);
        //        if (use != null && use.Domain != null)
        //        {
        //            fromDomain = use.Domain;
        //            var classNode = fromDomain.AllClasses.FirstOrDefault(c => c.Name == parts[1]);
        //            if (classNode != null)
        //                return classNode;
        //        }
        //    }
        //    fromDomain = null;
        //    return null;
        //}


        //internal string GetClassDeclaration(string className)
        //{
        //    IDomainNode fromDomain;
        //    var classNode = FindClass(className, out fromDomain);
        //    string name = className;
        //    if (classNode != null)
        //    {
        //        if (fromDomain == null)
        //            name = classNode.Name;
        //        else
        //            name = String.Format("{0}Definition.{1}", fromDomain.FullName, classNode.Name);
        //    }

        //    return name;
        //}

        //internal string GetClassFullName(string className)
        //{
        //    IDomainNode fromDomain;
        //    var classNode = FindClass(className, out fromDomain);
        //    string name = String.Empty;
        //    if (classNode == null)
        //    {
        //        var ext = FindExternalType(className);
        //        if (ext == null)
        //            return String.Empty;
        //        name = ext.FullName;
        //    }
        //    else
        //        name = classNode.FullName;

        //    return name;
        //}

        //public string GetTypeAsString(string type)
        //{
        //    string typeAsString = null;
        //    var clazz = FindClass(type);
        //    if (clazz != null)
        //        typeAsString = clazz.DomainDefinitionName;
        //    else
        //    {
        //        var ext = FindExternalType(type);
        //        if (ext != null)
        //            typeAsString = ext.FullName;
        //    }
        //    return typeAsString;
        //}

        //public void RegisterAsPartial(DomainNode domainNode)
        //{
        //    Partials.Add(domainNode);
        //}

        //public IEnumerable<IExternalNode> AllExternals
        //{
        //    get
        //    {
        //        return from d in Partials.Union(new List<IDomainNode>() { this })
        //               from c in d.Externals
        //               select c;
        //    }
        //}

        //public IEnumerable<IClassNode> AllClasses
        //{
        //    get
        //    {
        //        var classByNames = from d in Partials.Union(new List<IDomainNode>() { this })
        //                 from c in d.Classes
        //                 orderby c.Name
        //                 group c by c.Name into g
        //                 select g.ToList();

        //        foreach(var g in classByNames)
        //        {
        //            var clazz = g.First();
        //            if (clazz is IVirtualRelationship)
        //                yield return new AggregateVirtualRelationshipNode(g);
        //            else if (clazz is IRelationshipNode)
        //                yield return new AggregatedRelationshipNode(g);
        //            else
        //                yield return new AggregatedClassNode(g);
        //        }
        //    }
        //}

        public bool IsValid { get; set; }
    }
}
