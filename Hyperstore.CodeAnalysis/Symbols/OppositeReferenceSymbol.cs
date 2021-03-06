﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hyperstore.CodeAnalysis.Compilation;
using Hyperstore.CodeAnalysis.Syntax;

namespace Hyperstore.CodeAnalysis.Symbols
{
    internal sealed class OppositeReferenceSymbol : ReferenceSymbol, IOppositeReferenceSymbol
    {
        internal OppositeReferenceSymbol(HyperstoreCompilation compilation, Hyperstore.CodeAnalysis.Syntax.SyntaxNode node, Hyperstore.CodeAnalysis.Symbols.Symbol parent, Hyperstore.CodeAnalysis.Syntax.QualifiedNameSyntax relationshipName, SyntaxToken name)
            : base(compilation, node, parent, relationshipName, name)
        {
        }

        protected override void AcceptCore(Symbols.HyperstoreSymbolVisitor visitor)
        {
            base.AcceptCore(visitor);
            visitor.VisitOppositeReferenceSymbol(this);
        }
    }
}
