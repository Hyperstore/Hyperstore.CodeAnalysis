using System;
namespace Hyperstore.CodeAnalysis.Symbols
{
    public interface IReferenceSymbol : IMemberSymbol
    {
        IRelationshipDefinitionSymbol Definition { get; }
        IRelationshipSymbol Relationship { get; }
    }
}
