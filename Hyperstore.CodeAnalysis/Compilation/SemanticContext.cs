using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hyperstore.CodeAnalysis.Symbols;

namespace Hyperstore.CodeAnalysis.Compilation
{
    partial class SemanticContext : HyperstoreSymbolVisitor
    {
        private readonly HyperstoreCompilation _compilation;

        internal MergedDomain MergedDomain
        {
            get;
            set;
        }

        public SemanticContext(HyperstoreCompilation compilation)
        {
            _compilation = compilation;
        }

        private HyperstoreCompilation Compilation
        {
            get { return _compilation; }
        }

        private void AddDiagnostic(Diagnostic diagnostic)
        {
            Compilation.AddDiagnostic(diagnostic);
        }

        private void AddDiagnostic(Hyperstore.CodeAnalysis.Syntax.TokenOrNode tokenOrNode, string msg, params string[] args)
        {
            if (tokenOrNode.IsNode)
                AddDiagnostic(tokenOrNode.AsNode(), msg, args);
            else
                AddDiagnostic(tokenOrNode.AsToken(), msg, args);
        }

        private void AddDiagnostic(Symbol symbol, string msg, params string[] args)
        {
            foreach(var loc in symbol.Locations)
                Compilation.AddDiagnostic(loc, msg, args);
        }

        private void AddDiagnostic(Syntax.SyntaxNode node, string msg, params string[] args)
        {
            Compilation.AddDiagnostic(node.Location, msg, args);
        }

        private void AddDiagnostic(Syntax.SyntaxToken token, string msg, params string[] args)
        {
            Compilation.AddDiagnostic(token.Location, msg, args);
        }

        private void CheckConstraints(IEnumerable<ConstraintSymbol> constraints)
        {
            var cx = 0;
            foreach (var constraint in constraints)
            {
                if (String.IsNullOrWhiteSpace(constraint.Condition.Code))
                {
                    AddDiagnostic(constraint, "Constraint or compute code can not be null");
                }

                switch (constraint.Kind)
                {
                    case ConstraintKind.Check:
                    case ConstraintKind.Validate:
                        if (String.IsNullOrWhiteSpace(constraint.Message))
                            AddDiagnostic(constraint, "Constraint message can not be empty");
                        break;
                    case ConstraintKind.Compute:
                        cx++;
                        if (cx > 1)
                            AddDiagnostic(constraint, "Duplicate compute statement");
                        break;
                    default:
                        break;
                }
            }
        }

        public void VisitCSharpCode(CSharpCodeSymbol code)
        {
            base.VisitCSharpCode(code);

            if (code == null || code.Code == null)
                return;

            //    var tree = CSharp.CSharpSyntaxTree.ParseText(code.Script + ";", CSharp.CSharpParseOptions.Default.WithKind(SourceCodeKind.Script));
            //    foreach (var diag in tree.GetDiagnostics())
            //    {
            //        AddDiagnostic(
            //            Diagnostic.Create(diag.Id, diag.Category, diag.GetMessage(), diag.Severity, diag.IsEnabledByDefault, diag.WarningLevel, diag.IsWarningAsError,
            //            GetLocation(diag)
            //            ));

            //    }
            //}

            //internal Location GetLocation(Diagnostic diag)
            //{
            //    var span = SyntaxTokenOrNode.Span;
            //    var startLine = diag.Location.GetLineSpan().StartLinePosition.Line;
            //    var endLine = diag.Location.GetLineSpan().EndLinePosition.Line;
            //    return Location.Create(
            //        SyntaxTokenOrNode.SyntaxTree.SourceFilePath,
            //        new TextSpan(span.Location.Position + diag.Location.SourceSpan.Start, diag.Location.SourceSpan.Length),
            //        new LinePositionSpan(
            //            new LinePosition(span.Location.Line + startLine, span.Location.Column + diag.Location.SourceSpan.Start),
            //            new LinePosition(span.Location.Line + endLine, span.Location.Column + span.Length))
            //);
        }

        public void VisitUsingSymbol(UsingSymbol uses)
        {
            var referencedDomain = Compilation.ResolveDomain(uses.Domain, uses.DomainUri.Text);
            if (referencedDomain == null)
            {
                Compilation.AddDiagnostic(uses.DomainUri.Location, "Unable to found referenced domain {0}", uses.DomainUri.Text);
            }
            else if (referencedDomain.QualifiedName == MergedDomain.QualifiedName)
            {
                Compilation.AddDiagnostic(uses.DomainUri.Location, "A domain cannot reference itself {0}", uses.DomainUri.Text);
            }
        }

        public void VisitAttributeSymbol(AttributeSymbol attr)
        {
            var name = attr.Name.ToLower();

            if (name == "observable" || name == "dynamic" || name == "ignore")
            {
                if (attr.Arguments.Count() != 0)
                    AddDiagnostic(attr, "Attribute must have no argument");
            }
            else if (name == "modifier" || name == "attribute")
            {
                if (attr.Arguments.Count() != 1 || String.IsNullOrEmpty(attr.Arguments.First()))
                    AddDiagnostic(attr, "Attribute must have one argument");
            }
            else if (name == "index")
            {
                if (attr.Arguments.Count() < 1 || attr.Arguments.Count() > 2)
                    AddDiagnostic(attr, "Attribute index arguments error : (\"unique\", [\"IndexName\"])");
                else
                {
                    try
                    {
                        bool.Parse(attr.Arguments.First());
                    }
                    catch
                    {
                        AddDiagnostic(attr, "Attribute index arguments error : (\"unique\", [\"IndexName\"]) : unique must be true or false");
                    }
                }
            }
            //else
            //    AddDiagnostic (attr, "Invalid attribute name {0} for {1}", attr.Name, attr.Parent.Name);
        }

        #region override
        public override void VisitRelationshipSymbol(IRelationshipSymbol symbol)
        {
            VisitRelationshipSymbol(symbol as RelationshipSymbol);
        }

        public override void VisitPropertyReferenceSymbol(IPropertyReferenceSymbol symbol)
        {
            VisitPropertyReferenceSymbol(symbol as PropertyReferenceSymbol);
        }

        public override void VisitPropertySymbol(IPropertySymbol symbol)
        {
            VisitPropertySymbol(symbol as PropertySymbol);
        }

        public override void VisitOppositeReferenceSymbol(IOppositeReferenceSymbol symbol)
        {
            VisitOppositeReferenceSymbol(symbol as OppositeReferenceSymbol);
        }


        public override void VisitEntitySymbol(IEntitySymbol symbol)
        {
            VisitEntitySymbol(symbol as EntitySymbol);
        }

        public override void VisitAttributeSymbol(IAttributeSymbol symbol)
        {
            VisitAttributeSymbol(symbol as AttributeSymbol);
        }

        public override void VisitCSharpCode(ICSharpCodeSymbol symbol)
        {
            VisitCSharpCode(symbol as CSharpCodeSymbol);
        }


        public override void VisitUsingSymbol(IUsingSymbol symbol)
        {
            VisitUsingSymbol(symbol as UsingSymbol);
        }

        public override void VisitCommandPropertySymbol(ICommandPropertySymbol symbol)
        {
            VisitCommandPropertySymbol(symbol as CommandPropertySymbol);
        }

        public override void VisitValueObjectSymbol(IValueObjectSymbol symbol)
        {
            VisitValueObjectSymbol(symbol as ValueObjectSymbol);
        }
        #endregion
    }
}
