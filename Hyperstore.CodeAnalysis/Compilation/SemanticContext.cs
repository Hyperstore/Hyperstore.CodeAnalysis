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
            Compilation.AddDiagnostic(symbol.SyntaxTokenOrNode.AsNode(), msg, args);
        }

        private void AddDiagnostic(Syntax.SyntaxNode node, string msg, params string[] args)
        {
            Compilation.AddDiagnostic(node, msg, args);
        }

        private void AddDiagnostic(Syntax.SyntaxToken token, string msg, params string[] args)
        {
            Compilation.AddDiagnostic(token, msg, args);
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

        public override void VisitCSharpCode(CSharpCodeSymbol code)
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

        public override void VisitUsingSymbol(UsingSymbol uses)
        {
            var domain = uses.Domain;

            var model = Compilation.ResolveDomain(uses);
            if (model == null)
            {
                Compilation.AddDiagnostic(uses.DomainUri, "Unable to found referenced domain {0}", uses.DomainUri.Text);
            }

        }

        public override void VisitAttributeSymbol(AttributeSymbol attr)
        {
            var name = attr.Name.ToLower();

            if (name == "observable" || name == "dynamic")
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
            else
                AddDiagnostic(attr, "Invalid attribute name {0} for {1}", attr.Name, attr.Parent.Name);
        }

        //public TypeSymbol FindTypeSymbol(string qn)
        //{
        //    if (qn == null)
        //        return null;

        //    TypeSymbol symbol = null;
        //    var pos = qn.LastIndexOf('.');
        //    if (pos < 0)
        //    {
        //        if (Domain.Members.TryGetValue(qn, out symbol))
        //            return symbol;
        //        return Compilation.Resolver.FindPrimitive(qn);
        //    }

        //    if (Compilation.Options == HyperstoreCompilationOptions.Compilation)
        //    {
        //        foreach (var uses in Domain.Usings)
        //        {
        //            if (qn.StartsWith(uses.Name, StringComparison.Ordinal))
        //            {
        //                uses.Domain.Members.TryGetValue(qn.Substring(uses.Name.Length + 1), out symbol);
        //                return symbol;
        //            }
        //        }
        //    }
        //    return null;
        //}
    }
}
