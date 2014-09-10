using System;
namespace Hyperstore.CodeAnalysis.Symbols
{
    public interface IVirtualRelationshipSymbol : IRelationshipSymbol
    {
    }

    public interface IRelationshipSymbol : IElementSymbol
    {
        IRelationshipDefinitionSymbol Definition { get; }
    }
}
