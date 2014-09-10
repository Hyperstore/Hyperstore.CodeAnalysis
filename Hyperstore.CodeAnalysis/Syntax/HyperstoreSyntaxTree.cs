using Irony.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Hyperstore.CodeAnalysis;
using Hyperstore.CodeAnalysis.Syntax;

namespace Hyperstore.CodeAnalysis.Syntax
{
    public class HyperstoreSyntaxTree
    {
        private readonly List<Diagnostic> _diagnostics;

        public bool HasErrors { get; private set; }

        public string SourceFilePath { get; set; }

        public IEnumerable<Diagnostic> GetDiagnostics()
        {
            return _diagnostics;
        }

        private HyperstoreSyntaxTree(ParseTree parseTree, string path)
        {
            if (parseTree.Root != null)
                Root = parseTree.Root.AstNode as DomainSyntax;

            SourceFilePath = path ?? Guid.NewGuid().ToString("G");

            _diagnostics = new List<Diagnostic>();
            foreach (var m in parseTree.ParserMessages)
            {
                _diagnostics.Add(Diagnostic.Create(
                                    m.Message,
                                    m.Level == global::Irony.ErrorLevel.Error ? DiagnosticSeverity.Error : m.Level == global::Irony.ErrorLevel.Warning ? DiagnosticSeverity.Warning : DiagnosticSeverity.Info,
                                    m.SourceSpan,
                                    path
                                    )
                                 );
                HasErrors |= m.Level == Irony.ErrorLevel.Error;
            }
        }

        public DomainSyntax Root { get; private set; }

        [ThreadStatic]
        private static Parser _parser;

        private static void InitializeParser()
        {
            if (_parser == null)
            {
                var language = new LanguageData(new Hyperstore.CodeAnalysis.DomainLanguage.HyperstoreGrammar());
                _parser = new Parser(language);
            }
        }

        public static HyperstoreSyntaxTree ParseText(string text, string path = "",
            Encoding encoding = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (text == null)
            {
                throw new ArgumentNullException("text");
            }

            if (path == null)
            {
                throw new ArgumentNullException("path");
            }

            InitializeParser();
            var parseTree = _parser.Parse(text, path);
            var tree = new HyperstoreSyntaxTree(parseTree, path);

            if (tree.Root != null)
            {
                var visitor = new SyntaxTreeInitializer(tree);
                var walker = new HyperstoreSyntaxWalker(visitor, SyntaxWalkerDepth.Token);
                walker.Visit(tree.Root);
            }
            return tree;
        }


        class SyntaxTreeInitializer : HyperstoreSyntaxVisitor
        {
            HyperstoreSyntaxTree _syntaxTree;

            public SyntaxTreeInitializer(HyperstoreSyntaxTree tree)
            {
                _syntaxTree = tree;
            }

            public override void VisitSyntaxToken(Hyperstore.CodeAnalysis.Syntax.SyntaxToken token)
            {
                token.SyntaxTree = _syntaxTree;
                base.VisitSyntaxToken(token);
            }

            protected override void DefaultVisit(Hyperstore.CodeAnalysis.Syntax.SyntaxNode node)
            {
                base.DefaultVisit(node);
                node.SyntaxTree = _syntaxTree;
            }
        }


        public bool Built { get; internal set; }
    }
}
