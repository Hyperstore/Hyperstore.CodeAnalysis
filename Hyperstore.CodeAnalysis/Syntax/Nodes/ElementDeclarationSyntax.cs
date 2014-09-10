using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hyperstore.CodeAnalysis.Syntax
{
    public abstract class ElementDeclarationSyntax : DeclarationSyntax
    {
        public ListSyntax<MemberDeclarationSyntax> Members { get; protected set; }

        public SeparatedListSyntax<QualifiedNameSyntax> Implements { get; protected set; }
        public ListSyntax<ConstraintDeclarationSyntax> Constraints { get; protected set; }

        public QualifiedNameSyntax Extends { get; protected set; }

        public SyntaxToken Partial { get; protected set; }
    }
}
