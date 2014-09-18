using Hyperstore.CodeAnalysis.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hyperstore.CodeAnalysis.Symbols
{
    public enum CSharpCodeKind
    {
        Constraint,
        WhereClause,
        Compute,
        SelectClause,
        DefaultValue
    }

    internal sealed class CSharpCodeSymbol : Symbol, Hyperstore.CodeAnalysis.Symbols.ICSharpCodeSymbol
    {
        public override string Name { get { throw new NotImplementedException(); } }

        public CSharpCodeKind Kind { get; private set; }

        public string Code { get; private set; }

        public string Script
        {
            get
            {
                switch (Kind)
                {
                    case CSharpCodeKind.Constraint:
                        return String.Format("self => {{ {0} }}", Code);
                    case CSharpCodeKind.WhereClause:
                        return String.Format("item => {{ {0} }}", Code);
                    case CSharpCodeKind.Compute:
                        return String.Format("() => {{ {0} }}", Code);
                    case CSharpCodeKind.SelectClause:
                        return String.Format("item =>  {{ {0} }}", Code);
                    case CSharpCodeKind.DefaultValue:
                        return Code;
                }

                throw new NotImplementedException();
            }
        }

        internal CSharpCodeSymbol(SyntaxToken token, Symbol parent, string code, CSharpCodeKind kind)
            : base(token, parent, null)
        {
            Code = code;
            Kind = kind;
        }

        internal CSharpCodeSymbol(SyntaxNode node, Symbol parent, string code, CSharpCodeKind kind)
            : base(node, parent, null)
        {
            Code = code;
            Kind = kind;
        }

        protected override void AcceptCore(HyperstoreSymbolVisitor visitor)
        {
            base.AcceptCore(visitor);
            visitor.VisitCSharpCode(this);
        }
    }
}
