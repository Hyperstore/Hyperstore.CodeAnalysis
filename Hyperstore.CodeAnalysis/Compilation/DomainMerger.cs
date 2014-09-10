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

        public IEnumerable<DomainSymbol> Domains { get; private set; }

        public DomainMerger(HyperstoreCompilation compilation)
        {
            _compilation = compilation;
        }

        public void MergeDomains()
        {
            if (Domains != null)
                return;

            List<DomainSymbol> mergedDomains = new List<DomainSymbol>();
            var domainByNames = _compilation.DomainManager.Models.Select(d => d.Domain).GroupBy(d => d.QualifiedName);
            foreach (var domains in domainByNames)
            {
                if (domains.Count() == 1)
                {
                    mergedDomains.Add(domains.First());
                    continue;
                }

                var mergedDomain = domains.FirstOrDefault(d => d.ExtendedDomainUri == null);
                mergedDomains.Add(mergedDomain);

                foreach (var domain in domains)
                {
                    if (domain == mergedDomain)
                        continue;

                    if (domain.ExtendedDomainUri != null)
                    {
                        if (_compilation.DomainManager.FindDomain(domain, domain.ExtendedDomainUri.Text) == null)
                        {
                            _compilation.AddDiagnostic(domain.ExtendedDomainUri, "Unable to found domain to extends at {0}", domain.ExtendedDomainUri.Text);
                            continue;
                        }
                    }

                    foreach (var uses in domain.Usings)
                    {
                        if (mergedDomain.Usings.Any(e => e.Name == uses.Name))
                        {
                            _compilation.AddDiagnostic(uses.NameToken, "A using declaration already exists with the same alias {0}", uses.Name);
                            continue;
                        }
                        mergedDomain.Usings.Add(uses);
                    }

                    foreach (var member in domain.Members.Values)
                    {
                        TypeSymbol element;
                        if (mergedDomain.Members.TryGetValue(member.Name, out element) && !domain.IsPartial)
                        {
                            _compilation.AddDiagnostic(member.NameToken, "Duplicate member {0}", member.Name);
                        }
                        else if (element is IExternSymbol)
                        {
                            mergedDomain.Members.Add(element.Name, element);
                        }
                        else
                        {
                            var mergedElement = element as ElementSymbol;
                            var elementToMerge = member as ElementSymbol;

                            if (mergedElement == null || elementToMerge == null || element.GetType() != member.GetType() || (!mergedElement.IsPartial && !elementToMerge.IsPartial))
                            {
                                _compilation.AddDiagnostic(member.NameToken, "A member with the same name {0} already exists. Uses partial if your want override an existing type", member.Name);
                                continue;
                            }
                            MergeElement(mergedElement, elementToMerge);
                        }
                    }
                }
            }
            Domains = mergedDomains;
        }

        private void MergeElement(ElementSymbol mergedElement, ElementSymbol memberToMerge)
        {
            mergedElement.Constraints.AddRange(memberToMerge.Constraints);

            // Implements
            mergedElement.ImplementReferences.AddRange(memberToMerge.ImplementReferences);
            mergedElement.ExtendsReferences.AddRange(memberToMerge.ExtendsReferences);

            // members
            foreach (var member in memberToMerge.Members)
            {
                if (mergedElement.Members.Any(m => m.Name == member.Name))
                {
                    _compilation.AddDiagnostic(member.NameToken, "Duplicate member name {0}", member.Name);
                    continue;
                }
                mergedElement.Members.Add(member);
            }
        }
    }
}
