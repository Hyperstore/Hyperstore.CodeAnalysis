using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Hyperstore.Modeling.TextualLanguage
{
    class PrimitivesDomainNode : IDomainSyntaxNode
    {
        class Primitive : IExternalSyntaxNode
        {
            public Primitive(string alias, string fullName)
            {
                Alias = alias;
                FullName = fullName;
            }

            public bool IsPrimitive { get { return true; } }

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
                get { throw new NotImplementedException(); }
            }

            public bool CanGenerate { get { return false; } }
        }

        private List<IExternalSyntaxNode> _primitives;
        private List<IEntitySyntaxNode> _classes;
        public PrimitivesDomainNode()
        {
            _primitives = new List<IExternalSyntaxNode>();

            _primitives.Add(new Primitive("string", "string"));
            _primitives.Add(new Primitive("int", "int"));
            _primitives.Add(new Primitive("Guid", "System.Guid"));
            _primitives.Add(new Primitive("Int16", "System.Int16"));
            _primitives.Add(new Primitive("Int32", "System.Int32"));
            _primitives.Add(new Primitive("Int64", "System.Int64"));
            _primitives.Add(new Primitive("UInt16", "System.UInt16"));
            _primitives.Add(new Primitive("UInt32", "System.UInt32"));
            _primitives.Add(new Primitive("UInt64", "System.UInt64"));
            _primitives.Add(new Primitive("bool", "bool"));
            _primitives.Add(new Primitive("char", "char"));
            _primitives.Add(new Primitive("DateTime", "DateTime"));
            _primitives.Add(new Primitive("TimeSpan", "TimeSpan"));
            _primitives.Add(new Primitive("decimal", "decimal"));
            _primitives.Add(new Primitive("double", "double"));
            _primitives.Add(new Primitive("float", "float"));

            _classes = new List<IEntitySyntaxNode>();
            Classes.Add(new PrimitiveClass("ModelElement", "PrimitivesDomainModel.ModelElementMetaClass"));
            Classes.Add(new PrimitiveClass("ModelRelationship", "PrimitivesDomainModel.ModelRelationshipMetaClass"));
        }

        public List<IEntitySyntaxNode> Classes
        {
            get { return _classes; }
        }

        public ExtensionMode ExtensionMode
        {
            get { return ExtensionMode.None; }
        }

        public List<IExternalSyntaxNode> Externals
        {
            get { return _primitives; }
        }

        public string FullName
        {
            get { throw new NotImplementedException();}
        }

        public bool IsDynamic
        {
            get { return false; }
        }

        public bool IsObservable
        {
            get { return false; }
        }

        public string Namespace
        {
            get { return "xxx"; }
        }


        public void RegisterAsPartial(DomainNode domainNode)
        {
            throw new NotImplementedException();
        }

        public bool IsValid
        {
            get { return false; }
        }

        public bool IsPartial
        {
            get { return false; }
        }

        public IEnumerable<IEntitySyntaxNode> AllClasses
        {
            get { throw new NotImplementedException(); }
        }

        public IEnumerable<IExternalSyntaxNode> AllExternals
        {
            get { throw new NotImplementedException(); }
        }

        public string Name
        {
            get { throw new NotImplementedException(); }
        }

        public List<ICommandSyntaxNode> Commands
        {
            get { throw new NotImplementedException(); }
        }
    }
}
