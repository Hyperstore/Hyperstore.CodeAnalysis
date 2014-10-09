using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hyperstore.CodeAnalysis.Symbols;
using Hyperstore.CodeAnalysis.Syntax;

namespace Hyperstore.CodeAnalysis.Compilation
{
    public class HyperstoreCompilation : IDisposable
    {
        private List<Diagnostic> _diagnostics;

        public HyperstoreCompilationOptions Options { get; private set; }
        internal DomainManager DomainManager { get; private set; }
        private DomainMerger _merger;

        #region Diagnostics
        public IEnumerable<Diagnostic> GetDiagnostics()
        {
            ProcessSemanticRules();
            return DomainManager.SyntaxDiagnostics.Union(_diagnostics);
        }

        public bool HasSyntaxErrors
        {
            get
            {
                return DomainManager.HasSyntaxErrors;
            }
        }

        public bool HasErrors
        {
            get
            {
                return HasSyntaxErrors || HasCompilationErrors;
            }
        }

        private bool HasCompilationErrors
        {
            get
            {
                ProcessSemanticRules();
                return _diagnostics.Any(t => t.Severity == DiagnosticSeverity.Error);// TODO a optimiser
            }
        }


        internal void AddDiagnostic(Diagnostic diagnostic)
        {
            _diagnostics.Add(diagnostic);
        }

        internal void AddDiagnostic(Location location, string message, params string[] args)
        {
            _diagnostics.Add(Diagnostic.Create(
                                    args.Length == 0 ? message : String.Format(message, args),
                                    DiagnosticSeverity.Error,
                                    location
                ));
        }
        #endregion

        private HyperstoreCompilation(IEnumerable<HyperstoreSyntaxTree> syntaxTrees, ISemanticModelResolver resolver, HyperstoreCompilationOptions options)
        {
            Options = options;
            DomainManager = new DomainManager(this, resolver);

            Clear();

            if (syntaxTrees != null)
            {
                foreach (var syntaxTree in syntaxTrees)
                {
                    DomainManager.AddSyntaxTree(syntaxTree);
                }
            }
        }


        public static HyperstoreCompilation Create(IEnumerable<HyperstoreSyntaxTree> syntaxTrees, ISemanticModelResolver resolver = null, HyperstoreCompilationOptions options = HyperstoreCompilationOptions.Compilation)
        {
            return new HyperstoreCompilation(syntaxTrees, resolver, options);
        }

        public SemanticModel GetSemanticModel(HyperstoreSyntaxTree syntaxTree)
        {
            ProcessSemanticRules();
            return GetSemanticModel(syntaxTree.SourceFilePath);
        }

        public IEnumerable<SemanticModel> SemanticModels
        {
            get { return DomainManager.SemanticModels; }
        }

        public SemanticModel GetSemanticModel(string filePath)
        {
            return DomainManager.GetSemanticModel(filePath);
        }

        public void AddSyntaxTree(HyperstoreSyntaxTree syntaxTree)
        {
            if (syntaxTree == null)
                return;

            DomainManager.AddSyntaxTree(syntaxTree);
            Clear();
        }

        private void Clear()
        {
            _merger = null;
            _diagnostics = new List<Diagnostic>();
        }

        public void RenameSyntaxTree(string oldFilePath, string newFilePath)
        {
            DomainManager.RenameSyntaxTree(oldFilePath, newFilePath);
        }

        public void RemoveSyntaxTree(HyperstoreSyntaxTree syntaxTree)
        {
            if (syntaxTree == null)
                return;
            RemoveSyntaxTree(syntaxTree.SourceFilePath);
        }

        public void RemoveSyntaxTree(string filePath)
        {
            DomainManager.RemoveSyntaxTree(filePath);
            Clear();
        }

        public string Generate(Hyperstore.CodeAnalysis.Generation.IGenerator generator = null)
        {
            if (HasErrors)
                return null;

            var ctx = new Hyperstore.CodeAnalysis.Generation.HyperstoreGeneratorContext(this);

            if (generator == null)
            {
                generator = new Hyperstore.CodeAnalysis.Generation.CSharpGenerator();
            }

            generator.StartGenerate(ctx);
            foreach (var domain in _merger.Domains)
            {
                ctx.NewDomain();
                generator.GenerateCode(domain);
            }
            generator.EndGenerate();

            return ctx.ToString();
        }

        public void ProcessSemanticRules()
        {
            if (_merger != null || HasSyntaxErrors)
                return;

            _merger = null;
            DomainManager.Build();
            _merger = new DomainMerger(this);

            if (!HasCompilationErrors && Options == HyperstoreCompilationOptions.Compilation)
            {
                _merger.MergeDomains();
                if (!HasCompilationErrors && _merger.Domains.Count() > 0)
                {
                    var visitor = new SemanticContext(this);
                    var walker = new HyperstoreSymbolWalker(visitor);
                    foreach (var domain in _merger.Domains)
                    {
                        visitor.MergedDomain = domain as MergedDomain;
                        walker.Visit(domain);
                    }
                }
            }
        }

        internal ITypeSymbol FindTypeSymbol(IDomainSymbol currentDomain, string name)
        {
            if (_merger != null)
            {
                var domain = _merger.Domains.FirstOrDefault(d => String.Compare(currentDomain.QualifiedName, d.QualifiedName, StringComparison.Ordinal) == 0);
                if (domain != null)
                    currentDomain = domain;
            }

            return DomainManager.FindTypeSymbol(currentDomain, name);
        }

        public IDomainSymbol ResolveDomain(IDomainSymbol domain, string uri)
        {
            foreach (var loc in domain.Locations)
            {
                var s = ResolveDomain(loc.SyntaxTree, uri);
                if (s != null)
                    return s;
            }
            return null;
        }

        public IDomainSymbol ResolveDomain(HyperstoreSyntaxTree syntaxTree, string uri)
        {
            if (syntaxTree == null || uri == null)
                return null;
            var model = DomainManager.FindDomain(syntaxTree, uri);
            return model != null ? model.Domain : null;
        }

        public void Dispose()
        {
            Clear();
            DomainManager.Dispose();
        }

        public IEnumerable<IExternSymbol> GetPrimitives()
        {
            return DomainManager.GetPrimitives();
        }
    }
}
