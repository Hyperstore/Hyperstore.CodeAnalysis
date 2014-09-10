using System;
namespace Hyperstore.CodeAnalysis.Symbols
{
    public interface IConstraintSymbol : ISymbol
    {
        ICSharpCodeSymbol Condition { get; }
        ConstraintKind Kind { get; }
        string Message { get; }

        bool AsError { get; }
    }
}
