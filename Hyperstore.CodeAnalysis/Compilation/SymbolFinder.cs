using Hyperstore.CodeAnalysis.Symbols;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hyperstore.CodeAnalysis.Compilation
{
    class SymbolBeforePositionFinder : HyperstoreSymbolVisitor
    {
        private HyperstoreSpan _span;
        public ISymbol Symbol { get; private set; }

        public SymbolBeforePositionFinder(HyperstoreSpan span)
        {
            this._span = span;
        }

        public override void DefaultSymbol(ISymbol symbol)
        {
            var symbolSpan = symbol.SyntaxTokenOrNode.Span;
            var end = symbolSpan.Location.Position + symbolSpan.Length;;
            if( end <= _span.Start )
            {
                if( Symbol == null)
                {
                    Symbol = symbol;
                    return;
                }

                if( end > Symbol.SyntaxTokenOrNode.Span.Location.Position + Symbol.SyntaxTokenOrNode.Span.Length)
                {
                    Symbol = symbol;
                }
            }
        }
    }
}
