using Hyperstore.CodeAnalysis.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hyperstore.CodeAnalysis.Symbols
{
    public enum ConstraintKind
    {
        Check,
        Validate,
        Compute
    }

    internal sealed class ConstraintSymbol : Symbol, IConstraintSymbol
    {
        public override string Name { get { throw new NotImplementedException(); } }

        public CSharpCodeSymbol Condition { get; set; }

        internal SyntaxToken MessageToken { get; private set; }

        public string Message { get; set; }

        public ConstraintKind Kind { get; set; }

        public bool AsError { get; set; }

        internal ConstraintSymbol(Hyperstore.CodeAnalysis.Syntax.SyntaxNode node, Symbol parent, SyntaxToken message)
            : base(node, parent, null)
        {
            MessageToken = message;
            if (message != null)
                Message = message.Text;
        }

        protected override void AcceptCore(Symbols.HyperstoreSymbolVisitor visitor)
        {
            base.AcceptCore(visitor);
            ((Hyperstore.CodeAnalysis.Symbols.IVisitableSymbol)Condition).Accept(visitor);
            visitor.VisitConstraintSymbol(this);
        }


        ICSharpCodeSymbol IConstraintSymbol.Condition
        {
            get
            {
                return this.Condition;
            }
        }
    }
}
