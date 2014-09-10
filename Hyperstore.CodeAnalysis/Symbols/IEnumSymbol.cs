using System;
namespace Hyperstore.CodeAnalysis.Symbols
{
    public interface IEnumSymbol : ITypeSymbol
    {
        System.Collections.Generic.IEnumerable<string> Values { get; }
    }
}
