using System;
namespace Hyperstore.CodeAnalysis.Generation
{
    public interface IGenerator
    {
        void EndGenerate();
        void GenerateCode(Hyperstore.CodeAnalysis.Symbols.IDomainSymbol domain);
        void StartGenerate(Hyperstore.CodeAnalysis.Generation.HyperstoreGeneratorContext ctx);
    }
}
