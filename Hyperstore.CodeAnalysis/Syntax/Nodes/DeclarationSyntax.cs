using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hyperstore.CodeAnalysis.Syntax
{
    public abstract class DeclarationSyntax : SyntaxNode
    {
        public ListSyntax<AttributeSyntax> Attributes { get; protected set; }
        public SyntaxToken Name { get; protected set; }
    }
}
