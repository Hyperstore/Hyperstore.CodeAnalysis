using System;
namespace Hyperstore.CodeAnalysis.Compilation
{
    public interface IModelBuilder
    {
      //  HyperstoreCompilation Compilation { get; }
        Hyperstore.CodeAnalysis.Symbols.IDomainSymbol Domain { get; }
       // Hyperstore.CodeAnalysis.Syntax.HyperstoreSyntaxTree SyntaxTree { get; }
    }
}
