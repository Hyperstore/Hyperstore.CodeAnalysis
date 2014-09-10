using Hyperstore.CodeAnalysis.Syntax;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Hyperstore.CodeAnalysis.Compilation
{
    public class FileModelResolver : ISemanticModelResolver 
    {
        protected HyperstoreCompilation Compilation { get; private set; }

        public virtual void Initialize(HyperstoreCompilation compilation)
        {
            Compilation = compilation;
        }

        public virtual SemanticModel ResolveSemanticModel(string normalizedPath)
        {
            if( !File.Exists( normalizedPath))
                return null;

            try
            {
                var content = File.ReadAllText(normalizedPath);
                var tree = HyperstoreSyntaxTree.ParseText(content);
                return new SemanticModel(Compilation, tree);
            }
            catch
            {
                return null;
            }
        }
    }
}
