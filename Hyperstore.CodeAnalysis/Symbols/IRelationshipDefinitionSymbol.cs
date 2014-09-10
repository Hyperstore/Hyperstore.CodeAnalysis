using System;
namespace Hyperstore.CodeAnalysis.Symbols
{
    public interface IRelationshipDefinitionSymbol : ISymbol
    {
        RelationshipCardinality Cardinality { get; }
        IElementSymbol End { get; }
        bool IsEmbedded { get; }
        IElementSymbol Source { get; }

        string SourceProperty { get; }

        string EndProperty { get; }
    }
}
