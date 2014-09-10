using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hyperstore.CodeAnalysis.Symbols
{
    internal interface IVisitableSymbol
    {
        void Accept(HyperstoreSymbolVisitor visitor);
    }

    internal sealed class HyperstoreSymbolWalker
    {
        private HyperstoreSymbolVisitor _visitor;

        public HyperstoreSymbolWalker(HyperstoreSymbolVisitor visitor)
        {
            _visitor = visitor;
        }

        public void Visit(DomainSymbol symbol)
        {
            ((IVisitableSymbol)symbol).Accept(_visitor);
        }
    }
}
