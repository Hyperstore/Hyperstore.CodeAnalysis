using System;
namespace Hyperstore.CodeAnalysis.Symbols
{
    public interface IDomainSymbol : INamedSymbol
    {
        System.Collections.Generic.IEnumerable<ICommandSymbol> Commands { get; }
        System.Collections.Generic.IEnumerable<IElementSymbol> Elements { get; }
        System.Collections.Generic.IEnumerable<IEnumSymbol> Enums { get; }
        System.Collections.Generic.IEnumerable<IExternSymbol> Externals { get; }
        System.Collections.Generic.IEnumerable<IValueObjectSymbol> ValueObjects { get; }

        bool IsDynamic { get; }
        bool IsObservable { get; }
        string Namespace { get; }
        string QualifiedName { get; }
        System.Collections.Generic.IEnumerable<IUsingSymbol> Usings { get; }
    }
}
