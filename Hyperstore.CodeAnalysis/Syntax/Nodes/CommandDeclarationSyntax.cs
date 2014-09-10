using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hyperstore.CodeAnalysis.Syntax
{
    [System.Diagnostics.DebuggerDisplay("{Name}")]
    public sealed class CommandDeclarationSyntax : SyntaxNode 
    {
        public IEnumerable<AttributeSyntax> Attributes { get; private set; }
        public SyntaxToken Name { get; private set; }
    }
}
