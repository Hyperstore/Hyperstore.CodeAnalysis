using Hyperstore.CodeAnalysis.Symbols;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hyperstore.CodeAnalysis.Compilation
{
    internal sealed class MergedDomain : Symbol, IDomainSymbol
    {
        private HashSet<DomainSymbol> _domains = new HashSet<DomainSymbol>();

        public IEnumerable<IElementSymbol> Elements { get { return Members.Values.OfType<ElementSymbol>(); } }

        public IEnumerable<IExternSymbol> Externals { get { return Members.Values.OfType<ExternSymbol>(); } }

        public IEnumerable<IEnumSymbol> Enums { get { return Members.Values.OfType<EnumSymbol>(); } }

        public IEnumerable<ICommandSymbol> Commands { get { return Members.Values.OfType<CommandSymbol>(); } }

        public IEnumerable<IValueObjectSymbol> ValueObjects { get { return Members.Values.OfType<ValueObjectSymbol>(); } }

        internal Dictionary<string, TypeSymbol> Members { get; set; }

        public MergedDomain()
        {
            Members = new Dictionary<string, TypeSymbol>();
        }

        public bool AddDomain(DomainSymbol domain)
        {
            if (!_domains.Any(d => d.Locations.First().SyntaxTree.SourceFilePath == domain.Locations.First().SyntaxTree.SourceFilePath))
            {
                _domains.Add(domain);
                return true;
            }
            return false;
        }

        protected override void AcceptCore(HyperstoreSymbolVisitor visitor)
        {
            foreach (IVisitableSymbol domain in _domains)
            {
                domain.Accept(visitor);
            }
        }

        public bool IsDynamic
        {
            get { return _domains.Any(a => a.IsDynamic); }
        }

        public bool IsObservable
        {
            get { return _domains.Any(a => a.IsObservable); }
        }

        public string Namespace
        {
            get { return _domains.First().Namespace; }
        }

        public string QualifiedName
        {
            get { return _domains.First().QualifiedName; }
        }

        public IEnumerable<IUsingSymbol> Usings
        {
            get { return _domains.SelectMany(d => d.Usings); }
        }

        public string ExtendedDomainPath
        {
            get { return null; }
        }

        public IEnumerable<IAttributeSymbol> Attributes
        {
            get { return _domains.SelectMany(d => d.Attributes); }
        }

        public bool HasAttribute(string name)
        {
            return _domains.Any(a => a.HasAttribute(name));

        }

        public bool Skip
        {
            get { return _domains.Any(a => a.Skip); }
        }

        public override string Name
        {
            get { return _domains.First().Name; }
        }

        public override IEnumerable<Location> Locations
        {
            get { return _domains.SelectMany(d => d.Locations); }
        }

        bool IDomainSymbol.TryGetMember(string name, out ITypeSymbol symbol)
        {
            TypeSymbol s;
            if (Members.TryGetValue(name, out s))
            {
                symbol = s;
                return true;
            }

            symbol = null;
            return false;
        }
    }
}
