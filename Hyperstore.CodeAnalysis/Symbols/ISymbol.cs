using Hyperstore.CodeAnalysis.Syntax;
using System;
using System.Collections.Generic;
namespace Hyperstore.CodeAnalysis.Symbols
{
    public interface ISymbol
    {
        string Name { get; }
        ISymbol Parent { get; }

        /// <summary>
        /// Gets the locations where this symbol was originally defined. 
        /// Some symbols (for example, partial entities) may be defined in more than one
        /// location.
        /// </summary>
        IEnumerable<Location> Locations { get; }

        IDomainSymbol Domain { get; }
    }
}
