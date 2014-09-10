using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hyperstore.CodeAnalysis.Compilation;
using Hyperstore.CodeAnalysis.Syntax;

namespace Hyperstore.CodeAnalysis.Symbols
{
    public class LazyRef<T> where T : ITypeSymbol
    {
        private HyperstoreCompilation _compilation;
        private readonly string _name;
        private T _value;
        private DomainSymbol _containingDomain;

        public TokenOrNode SyntaxTokenOrNode { get; private set; }

        public bool IsResolved { get; private set; }

        public string Name { get { return _name; } }

        internal LazyRef(T value, string name)
        {
            _name = name;
            _value = value;
        }

        internal LazyRef(SyntaxToken token, HyperstoreCompilation compilation, DomainSymbol domain)
        {
            _compilation = compilation;
            _name = token.Text;
            SyntaxTokenOrNode = new TokenOrNode(token);
            _containingDomain = domain;
        }

        internal LazyRef(string name, HyperstoreCompilation compilation, DomainSymbol domain)
        {
            _compilation = compilation;
            _name = name;
            _containingDomain = domain;
        }

        internal LazyRef(QualifiedNameSyntax qn, HyperstoreCompilation compilation, DomainSymbol domain)
        {
            _compilation = compilation;
            _name = qn.FullName;
            SyntaxTokenOrNode = new TokenOrNode(qn);
            _containingDomain = domain;
        }

        public T Value
        {
            get
            {
                if (!IsResolved)
                {
                    if (_name != null && _value == null)
                        _value = (T)_compilation.FindTypeSymbol(_containingDomain, _name);
                    if (_value != null || _name == null)
                    {
                        _compilation = null;
                        _containingDomain = null;
                        IsResolved = true;
                    }
                }
                return _value;
            }
        }
    }
}
