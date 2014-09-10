using System;
namespace Hyperstore.CodeAnalysis.Symbols
{
    public interface IElementSymbol : ITypeSymbol
    {
        System.Collections.Generic.IEnumerable<IConstraintSymbol> Constraints { get; }
        System.Collections.Generic.IEnumerable<IElementSymbol> DerivedElements { get; }
        bool HasClassInheritance { get; }
        System.Collections.Generic.IEnumerable<ITypeSymbol> Implements { get; }
        bool IsA(IElementSymbol symbol);
        bool IsPartial { get; }
        System.Collections.Generic.IEnumerable<IOppositeReferenceSymbol> OppositeReferences { get; }
        System.Collections.Generic.IEnumerable<IPropertySymbol> Properties { get; }
        System.Collections.Generic.IEnumerable<IPropertyReferenceSymbol> References { get; }
        ITypeSymbol SuperType { get; }
    }
}
