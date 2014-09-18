using Hyperstore.CodeAnalysis.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hyperstore.CodeAnalysis.Symbols
{
    internal sealed class CommandSymbol : TypeSymbol, ICommandSymbol
    {
        internal List<CommandPropertySymbol> Properties
        {
            get;
            private set;
        }

        internal CommandSymbol(Hyperstore.CodeAnalysis.Syntax.SyntaxNode node, Symbol parent, SyntaxToken name)
            : base(node, parent, name)
        {
            Properties = new List<CommandPropertySymbol>();
        }


        protected override void AcceptCore(HyperstoreSymbolVisitor visitor)
        {
            base.AcceptCore(visitor);
            visitor.VisitCommandSymbol(this);
            Properties.ForEach(m => ((Hyperstore.CodeAnalysis.Symbols.IVisitableSymbol)m).Accept(visitor));
        }

        IEnumerable<ICommandPropertySymbol> ICommandSymbol.Properties
        {
            get { return this.Properties; }
        }
    }
}
