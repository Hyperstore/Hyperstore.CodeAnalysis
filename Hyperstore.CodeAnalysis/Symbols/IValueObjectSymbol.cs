using System;
namespace Hyperstore.CodeAnalysis.Symbols
{
    public interface IValueObjectSymbol : ITypeSymbol
    {
        System.Collections.Generic.IEnumerable<IConstraintSymbol> Constraints { get; }
        bool IsPartial { get; }
        ITypeSymbol Type { get; }
    }
}
