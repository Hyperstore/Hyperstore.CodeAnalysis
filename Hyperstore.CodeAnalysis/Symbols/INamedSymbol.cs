using System;
namespace Hyperstore.CodeAnalysis.Symbols
{
    public interface INamedSymbol : ISymbol
    {
        System.Collections.Generic.IEnumerable<IAttributeSymbol> Attributes { get; }
    }
}
