using System;
namespace Hyperstore.CodeAnalysis.Symbols
{
    public interface IPropertySymbol : IMemberSymbol
    {
        string CalculatedCode { get; }
        System.Collections.Generic.IEnumerable<IConstraintSymbol> Constraints { get; }
        ICSharpCodeSymbol DefaultValue { get; }
        bool IsCalculatedProperty { get; }
        ITypeSymbol PropertyType { get; }
        ICSharpCodeSymbol SelectClause { get; }
        ICSharpCodeSymbol WhereClause { get; }
    }
}
