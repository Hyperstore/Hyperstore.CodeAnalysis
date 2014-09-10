using System;
namespace Hyperstore.CodeAnalysis.Symbols
{
    public interface ITypeSymbol : INamedSymbol
    {
        string QualifiedName { get; }
    }
}
