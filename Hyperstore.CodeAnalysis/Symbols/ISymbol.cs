using System;
namespace Hyperstore.CodeAnalysis.Symbols
{
    public interface ISymbol
    {
        string Name { get; }
        ISymbol Parent { get;  }
        Hyperstore.CodeAnalysis.Syntax.TokenOrNode SyntaxTokenOrNode { get;}
        IDomainSymbol Domain { get; }
    }
}
