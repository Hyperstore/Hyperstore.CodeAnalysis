using Hyperstore.CodeAnalysis.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hyperstore.CodeAnalysis.Symbols
{
    internal sealed class DomainSymbol : NamedSymbol, Hyperstore.CodeAnalysis.Symbols.IDomainSymbol
    {
        private readonly List<UsingSymbol> _usings = new List<UsingSymbol>();
        public string QualifiedName { get; private set; }

        public override DomainSymbol Domain
        {
            get
            {
                return this;
            }
        }
        public string Namespace
        {
            get;
            set;
        }

        public IEnumerable<ElementSymbol> Elements { get { return Members.Values.OfType<ElementSymbol>(); } }

        public IEnumerable<ExternSymbol> Externals { get { return Members.Values.OfType<ExternSymbol>(); } }

        public IEnumerable<EnumSymbol> Enums { get { return Members.Values.OfType<EnumSymbol>(); } }

        public IEnumerable<CommandSymbol> Commands { get { return Members.Values.OfType<CommandSymbol>(); } }

        public IEnumerable<ValueObjectSymbol> ValueObjects { get { return Members.Values.OfType<ValueObjectSymbol>(); } }

        public List<UsingSymbol> Usings { get { return _usings; } }

        internal Dictionary<string, TypeSymbol> Members { get; set; }

        internal DomainSymbol() : base( null, null, null)
        {
            Name = "Primitives";
        }

        internal DomainSymbol(Hyperstore.CodeAnalysis.Syntax.SyntaxNode node, QualifiedNameSyntax qname)
            : base(node, null, qname.NameToken)
        {
            Members = new Dictionary<string, TypeSymbol>();
            QualifiedName = qname.FullName;
            Namespace = String.Empty;
            var name = qname.FullName;
            Name = name;
            int pos = name.LastIndexOf('.');
            if (pos > 0)
            {
                Name = name.Substring(pos + 1);
                Namespace = name.Substring(0, pos);
            }
        }

        protected override void AcceptCore(HyperstoreSymbolVisitor visitor)
        {
            base.AcceptCore(visitor);

            var list = new List<ITypeSymbol>(Members.Values);
            foreach (IVisitableSymbol elem in list)
                elem.Accept(visitor);
            Usings.ForEach(m => ((Hyperstore.CodeAnalysis.Symbols.IVisitableSymbol)m).Accept(visitor));
            visitor.VisitDomainSymbol(this);
        }

        public bool IsObservable { get { return Attributes.Any(a => String.Compare(a.Name, "observable", StringComparison.OrdinalIgnoreCase) == 0); } }

        public bool IsDynamic { get { return Attributes.Any(a => String.Compare(a.Name, "dynamic", StringComparison.OrdinalIgnoreCase) == 0); } }

        internal void AddUsing(string alias, UsingSymbol domain)
        {
            _usings.Add(domain);
        }

        IEnumerable<ICommandSymbol> IDomainSymbol.Commands
        {
            get { return this.Commands; }
        }

        IEnumerable<IElementSymbol> IDomainSymbol.Elements
        {
            get { return this.Elements; }
        }

        IEnumerable<IEnumSymbol> IDomainSymbol.Enums
        {
            get { return this.Enums; }
        }

        IEnumerable<IExternSymbol> IDomainSymbol.Externals
        {
            get { return this.Externals; }
        }

        IEnumerable<IValueObjectSymbol> IDomainSymbol.ValueObjects
        {
            get { return this.ValueObjects; }
        }

        IEnumerable<IUsingSymbol> IDomainSymbol.Usings
        {
            get { return this.Usings; }
        }

        public SyntaxToken ExtendedDomainUri { get; set; }

        public bool IsPartial { get { return ExtendedDomainUri != null; } }
    }
}
