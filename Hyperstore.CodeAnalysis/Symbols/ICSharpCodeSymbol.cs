using System;
namespace Hyperstore.CodeAnalysis.Symbols
{
    public interface ICSharpCodeSymbol : ISymbol
    {
        string Code { get; }
        CSharpCodeKind Kind { get; }
        string Script { get; }
    }
}
