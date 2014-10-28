using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hyperstore.CodeAnalysis.Symbols;
using Hyperstore.CodeAnalysis.Syntax;
using Irony.Parsing;

namespace Hyperstore.CodeAnalysis.Compilation
{
    public class SemanticModel
    {
        public HyperstoreSyntaxTree SyntaxTree { get; private set; }

        internal IModelBuilder Model { get; private set; }

        public IDomainSymbol Domain { get { return Model != null ? Model.Domain : null; } }

        public HyperstoreCompilation Compilation { get; private set; }

        public SemanticModel(HyperstoreCompilation compilation, HyperstoreSyntaxTree syntaxTree)
        {
            SyntaxTree = syntaxTree;
            Compilation = compilation;
        }

        internal void BuildModel()
        {
             if (Model == null)
            {
                var model = new ModelBuilder(Compilation);
                model.Build(SyntaxTree);
                Model = model;
            }
        }

        public void Visit(HyperstoreSymbolVisitor visitor)
        {
            if (Domain == null)
                return;
            var walker = new HyperstoreSymbolWalker(visitor);
            walker.Visit(Domain);
        }

        public IEnumerable<Diagnostic> GetDiagnostics()
        {
            return Compilation.GetDiagnostics().Where(d => d.Location.SyntaxTree == SyntaxTree);
        }
    }

    internal class DomainManager : IDisposable
    {
        private readonly ISemanticModelResolver _resolver;

        private Dictionary<string, SemanticModel> _models = new Dictionary<string, SemanticModel>(StringComparer.OrdinalIgnoreCase);

        internal IEnumerable<ModelBuilder> Models { get { return _models.Values.Select(e => (ModelBuilder)e.Model); } }

        private DomainSymbol PrimitivesDomain { get; set; }
        private readonly HyperstoreCompilation _compilation;

        public bool HasSyntaxErrors
        {
            get
            {
                return _models.Values.Any(st => st.SyntaxTree.HasErrors);
            }
        }

        public IEnumerable<Diagnostic> SyntaxDiagnostics
        {
            get
            {
                return _models.Values.SelectMany(st => st.SyntaxTree.GetDiagnostics());
            }
        }

        public DomainManager(HyperstoreCompilation compilation, ISemanticModelResolver resolver)
        {
            _compilation = compilation;
            _resolver = resolver ?? new FileModelResolver();
            _resolver.Initialize(compilation);
            RegisterPrimitiveDomain();
        }
       
        internal void AddSyntaxTree(HyperstoreSyntaxTree syntaxTree)
        {
            var filePath = syntaxTree.SourceFilePath ?? syntaxTree.GetHashCode().ToString();
            _models[syntaxTree.SourceFilePath] = new SemanticModel(_compilation, syntaxTree);
        }

        internal void RenameSyntaxTree(string oldFilePath, string newFilePath)
        {
            if (oldFilePath == null || newFilePath == null)
                throw new ArgumentNullException("filepath");

            var model = _models[oldFilePath];
            if (model != null)
            {
                _models.Remove(oldFilePath);
                model.SyntaxTree.SourceFilePath = newFilePath;
                _models[newFilePath] = model;
            }
        }

        internal void Build()
        {
            if (HasSyntaxErrors)
                return;

            foreach (var model in _models.Values)
            {
                model.BuildModel();
            }
        }


        internal void RemoveSyntaxTree(string path)
        {
            _models.Remove(path);
        }

        private void RegisterPrimitiveDomain()
        {
            var primitives = new List<TypeSymbol>();

            primitives.Add(new ExternSymbol("string", "string"));
            primitives.Add(new ExternSymbol("int", "int"));
            primitives.Add(new ExternSymbol("Guid", "System.Guid"));
            primitives.Add(new ExternSymbol("Int16", "System.Int16"));
            primitives.Add(new ExternSymbol("Int32", "System.Int32"));
            primitives.Add(new ExternSymbol("Int64", "System.Int64"));
            primitives.Add(new ExternSymbol("UInt16", "System.UInt16"));
            primitives.Add(new ExternSymbol("UInt32", "System.UInt32"));
            primitives.Add(new ExternSymbol("UInt64", "System.UInt64"));
            primitives.Add(new ExternSymbol("bool", "bool"));
            primitives.Add(new ExternSymbol("char", "char"));
            primitives.Add(new ExternSymbol("DateTime", "DateTime"));
            primitives.Add(new ExternSymbol("TimeSpan", "TimeSpan"));
            primitives.Add(new ExternSymbol("decimal", "decimal"));
            primitives.Add(new ExternSymbol("double", "double"));
            primitives.Add(new ExternSymbol("float", "float"));

            PrimitivesDomain = new DomainSymbol() { Namespace="Hyperstore.Modeling", Members = primitives.ToDictionary(p => p.Name, p => p) };


            PrimitivesDomain.Members.Add("ModelEntity", new PrimitiveEntitySymbol(PrimitivesDomain));
            PrimitivesDomain.Members.Add("ModelRelationship", new PrimitiveRelationshipSymbol(PrimitivesDomain));

        }

        internal ITypeSymbol FindPrimitive(string qualifiedName)
        {
            if (String.IsNullOrWhiteSpace(qualifiedName))
                return null;

            TypeSymbol symbol = null;
            PrimitivesDomain.Members.TryGetValue(qualifiedName, out symbol);
            return symbol;
        }

        internal IEnumerable<IExternSymbol> GetPrimitives()
        {
            return PrimitivesDomain.Externals;
        }

        public SemanticModel GetSemanticModel(string filePath)
        {
            SemanticModel entry;
            _models.TryGetValue(filePath, out entry);
            return entry;
        }

        public IEnumerable<SemanticModel> SemanticModels
        {
            get { return _models.Values; }
        }

        protected string NormalizeUri(HyperstoreSyntaxTree syntaxTree, string uri)
        {
            var filePath = syntaxTree.SourceFilePath;
            return filePath != null ? System.IO.Path.Combine(System.IO.Path.GetDirectoryName(filePath), uri) : uri;
        }

        public IModelBuilder FindDomain(HyperstoreSyntaxTree syntaxTree, string uri)
        {
            var normalizedPath = NormalizeUri(syntaxTree, uri);
            SemanticModel entry;
            if (_models.TryGetValue(normalizedPath, out entry) && entry.Model != null)
                return entry.Model;

            entry = _resolver.ResolveSemanticModel(normalizedPath);
            if (entry != null)
            {
                entry.BuildModel();
                return entry.Model;
            }
            return null;
        }

        internal ITypeSymbol FindTypeSymbol(IDomainSymbol currentDomain, string name)
        {
            if (String.IsNullOrWhiteSpace(name))
                return null;

            IDomainSymbol domain = null;
            var pos = name.LastIndexOf('.');
            if (pos > 0)
            {
                var alias = name.Substring(0, pos);
                name = name.Substring(pos + 1);
                if (String.CompareOrdinal(currentDomain.Namespace, alias) == 0)
                {
                    domain = currentDomain;
                }
                else
                {
                    var uses = currentDomain.Usings.FirstOrDefault(u => String.CompareOrdinal(u.Name, alias) == 0);
                    if (uses != null)
                    {
                        var model = FindDomain(currentDomain.Locations.First().SyntaxTree, uses.Path);
                        if (model != null)
                        {
                            domain = model.Domain;
                        }
                    }
                }
            }
            else
            {
                domain = currentDomain;
            }

            if (domain == null)
                return null;

            ITypeSymbol symbol;
            if (domain.TryGetMember(name, out symbol))
                return symbol;

            if (currentDomain.ExtendedDomainPath != null)
            {
                var model = FindDomain(currentDomain.Locations.First().SyntaxTree, currentDomain.ExtendedDomainPath);
                if( model != null && model.Domain != null)
                {
                    if (model.Domain.TryGetMember(name, out symbol))
                        return symbol;
                }
            }

            return FindPrimitive(name);
        }

        public void Dispose()
        {
            _models.Clear();
            if (_resolver is IDisposable)
                ((IDisposable)_resolver).Dispose();
        }


    }
}
