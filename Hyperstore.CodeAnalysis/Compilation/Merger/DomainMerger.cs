using Hyperstore.CodeAnalysis.Symbols;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hyperstore.CodeAnalysis.Compilation
{
    class DomainMerger
    {
        private HyperstoreCompilation _compilation;

        public IEnumerable<IDomainSymbol> Domains { get; private set; }

        public DomainMerger(HyperstoreCompilation compilation)
        {
            _compilation = compilation;
        }

        public void MergeDomains()
        {
            if (Domains != null)
                return;

            List<IDomainSymbol> mergedDomains = new List<IDomainSymbol>();
            var modelByNames = GetDomainsRecursive(_compilation.DomainManager.Models)
                               .Select(d => d)
                               .GroupBy(d => d.Domain.QualifiedName);

            foreach (var models in modelByNames)
            {
                var mergedDomain = new MergedDomain();
                mergedDomains.Add(mergedDomain);

                foreach (var model in models)
                {
                    var domain = model.Domain;
                    MergeDomains(mergedDomain, domain);
                }
            }

            Domains = mergedDomains;
        }

        private IEnumerable<DomainSymbol> GetDomainsRecursive(IEnumerable<ModelBuilder> models)
        {
            var set = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var queue = new Queue<IModelBuilder>(models);

            while (queue.Count > 0)
            {
                var model = queue.Dequeue() as ModelBuilder;
                var sourceFile = model.SyntaxTree.SourceFilePath;
                if (set.Add(sourceFile))
                {
                    if (!_compilation.IsValidForCurrentConfiguration(model.Domain))
                        continue;

                    var domain = model.Domain;
                    yield return domain;

                    if (domain.ExtendedDomainPath != null)
                    {
                        queue.Enqueue(_compilation.DomainManager.FindDomain(model.SyntaxTree, domain.ExtendedDomainPath));
                    }
                }
            }
        }

        private void MergeDomains(MergedDomain mergedDomain, DomainSymbol domain)
        {
            if (!mergedDomain.AddDomain(domain))
                return;

            foreach (var member in domain.Members.Values)
            {
                TypeSymbol element;
                if (mergedDomain.Members.TryGetValue(member.Name, out element))
                {
                    if (element != null)
                    {
                        if (element is IExternSymbol || element is ICommandSymbol || element is IValueObjectSymbol || element is IEnumSymbol)
                        {
                            _compilation.AddDiagnostic(member.NameToken.Location, "Duplicate member {0}", member.Name);
                            continue;
                        }

                        var mergedElement = element as ElementSymbol;
                        var elementToMerge = member as ElementSymbol;

                        if (mergedElement == null || elementToMerge == null || element.GetType() != member.GetType())
                        {
                            _compilation.AddDiagnostic(member.NameToken.Location, "A member with the same name {0} already exists. Uses 'def partial' if your want override an existing type", member.Name);
                            continue;
                        }
                        MergeElement(mergedElement, elementToMerge);
                        continue;
                    }
                }

                mergedDomain.Members.Add(member.Name, member);
            }
        }

        private void MergeElement(ElementSymbol mergedElement, ElementSymbol memberToMerge)
        {
            mergedElement.Constraints.AddRange(memberToMerge.Constraints);
            mergedElement.Attributes.AddRange(memberToMerge.Attributes);

            // Implements
            mergedElement.ImplementReferences.AddRange(memberToMerge.ImplementReferences);
            mergedElement.ExtendsReferences.AddRange(memberToMerge.ExtendsReferences);

            // members
            foreach (var member in memberToMerge.Members)
            {
                MemberSymbol other = mergedElement.Members.FirstOrDefault(m => m.Name == member.Name);
                if (other != null)
                {
                    if (!member.TryMerge(other))
                    {
                        _compilation.AddDiagnostic(member.NameToken.Location, "All partial members must have the same definition. Member {0} definition is not correct.", member.Name);
                    }
                }
                else
                    mergedElement.Members.Add(member);
            }
        }
    }
}
