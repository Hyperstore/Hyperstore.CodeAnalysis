using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hyperstore.CodeAnalysis.Syntax
{
    public abstract class MemberDeclarationSyntax : SyntaxNode
    {
        public SyntaxToken Name { get; protected set; }
        public ListSyntax<AttributeSyntax> Attributes { get; protected set; }
    }
}
