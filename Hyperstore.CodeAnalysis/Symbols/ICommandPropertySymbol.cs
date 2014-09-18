using System;
namespace Hyperstore.CodeAnalysis.Symbols
{
    public interface ICommandPropertySymbol : INamedSymbol
    {
        ITypeSymbol PropertyType { get; }

        bool IsEntity { get; }
    }
}
