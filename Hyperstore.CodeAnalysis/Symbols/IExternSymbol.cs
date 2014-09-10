using System;
namespace Hyperstore.CodeAnalysis.Symbols
{
    public interface IExternSymbol : ITypeSymbol
    {
        string Alias { get; }
        string FullName { get;  }
        ExternalKind Kind { get;  }
    }
}
