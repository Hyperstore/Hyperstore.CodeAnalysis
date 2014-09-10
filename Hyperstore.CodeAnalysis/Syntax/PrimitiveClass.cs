using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hyperstore.Modeling.TextualLanguage
{
    class PrimitiveClass : IEntitySyntaxNode
    {
        public PrimitiveClass(string name, string displayName)
        {
            Name = name;
            DomainDefinitionName = displayName;
        }

        public bool CanGenerate { get { return false; } }

        public IEnumerable<AttributeNode> Attributes
        {
            get { throw new NotImplementedException(); }
        }

        public string Modifier
        {
            get { throw new NotImplementedException(); }
        }

        public string DomainDefinitionName
        {
            get;
            private set;
        }

        public string FullName
        {
            get { return Name; }
        }

        public string Name
        {
            get;
            private set;
        }

        public IEnumerable<ReferenceNode> References
        {
            get { throw new NotImplementedException(); }
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
            get { throw new NotImplementedException(); }
        }

        public IEnumerable<string> Implements
        {
            get { throw new NotImplementedException(); }
        }

        public QualifiedNameNode Extends
        {
            get { throw new NotImplementedException(); }
        }

        public string SuperTypeName
        {
            get { throw new NotImplementedException(); }
        }

        public IEntitySyntaxNode GetSuperClass(DomainNode domainNode)
        {
            throw new NotImplementedException();
        }

        public bool HasClassInheritance { get { return false; } }

        public IEnumerable<string> Derived
        {
            get { throw new NotImplementedException(); }
        }

        public void AddDerived(string className)
        {
            throw new NotImplementedException();
        }
    }
}
