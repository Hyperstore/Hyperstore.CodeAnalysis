using System;
namespace Hyperstore.CodeAnalysis.Symbols
{
    public interface IAttributeSymbol : ISymbol
    {
        System.Collections.Generic.IEnumerable<string> Arguments { get; }
    }
}
