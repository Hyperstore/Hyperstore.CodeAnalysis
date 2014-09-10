using System;
namespace Hyperstore.CodeAnalysis.Symbols
{
    public interface IUsingSymbol : ISymbol
    {
        string DomainUri { get; }
        string Path { get; }
    }
}
