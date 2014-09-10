using System;
namespace Hyperstore.CodeAnalysis.Compilation
{
    public interface ISemanticModelResolver
    {
        SemanticModel ResolveSemanticModel(string normalizedPath);

        void Initialize(HyperstoreCompilation compilation);
    }
}
