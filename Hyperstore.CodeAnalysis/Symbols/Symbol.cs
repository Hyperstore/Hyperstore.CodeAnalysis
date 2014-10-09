using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hyperstore.CodeAnalysis.Syntax;

namespace Hyperstore.CodeAnalysis.Symbols
{
    [System.Diagnostics.DebuggerDisplay("{Name}")]
    internal abstract class Symbol : IVisitableSymbol, Hyperstore.CodeAnalysis.Symbols.ISymbol
    {
        private SyntaxToken _nameToken;
        private List<Location> _locations;

        internal SyntaxToken NameToken { get { return _nameToken; } private set { _nameToken = value; } }

        public virtual string Name
        {
            get;
            protected set;
        }

        protected void ReplaceLocation(Location location)
        {
            _locations[0] = location;
        }

        public virtual IEnumerable<Location> Locations { get { return _locations; } }

        public Symbol Parent { get; set; }

        public virtual DomainSymbol Domain
        {
            get
            {
                var parent = this.Parent;
                while (parent != null && !(parent is DomainSymbol))
                {
                    parent = parent.Parent;
                }
                return (DomainSymbol)parent;
            }
        }

        protected Symbol()
        {
        }

        internal Symbol(SyntaxToken token, Symbol parent, SyntaxToken name)
        {
            NameToken = name;
            Name = NameToken != null ? NameToken.Text : String.Empty;

            _locations = new List<Location>();
            if (token != null)
                _locations.Add(token.Location);
            Parent = parent;
        }

        internal Symbol(SyntaxNode node, Symbol parent, SyntaxToken name)
        {
            NameToken = name;
            Name = NameToken != null ? NameToken.Text : String.Empty;

            _locations = new List<Location>();
            if (node != null)
                _locations.Add(node.Location);
            Parent = parent;
        }

        void IVisitableSymbol.Accept(HyperstoreSymbolVisitor visitor)
        {
            AcceptCore(visitor);
        }

        protected virtual void AcceptCore(HyperstoreSymbolVisitor visitor)
        {

        }
        IEnumerable<Location> ISymbol.Locations
        {
            get
            {
                return this.Locations;
            }
        }

        ISymbol ISymbol.Parent
        {
            get
            {
                return this.Parent;
            }
        }

        IDomainSymbol ISymbol.Domain
        {
            get
            {
                return this.Domain;
            }
        }
    }
}
