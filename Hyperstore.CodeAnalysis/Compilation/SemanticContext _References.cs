using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hyperstore.CodeAnalysis.Symbols;

namespace Hyperstore.CodeAnalysis.Compilation
{
    partial class SemanticContext : HyperstoreSymbolVisitor
    {

        public void VisitPropertyReferenceSymbol(PropertyReferenceSymbol reference)
        {
            var parent = reference.Parent as ElementSymbol;
            var domain = parent.Domain;

            if (reference.Relationship != null)
            {
                if (!parent.IsA(reference.Relationship.Definition.Source))
                    AddDiagnostic(reference, "Relationship type mismatch. Cannot convert {0} to {1}", parent.Name, reference.Relationship.Name);
            }

            if (reference.Definition.End == null)
            {
                AddDiagnostic(reference.Definition.EndReference.SyntaxTokenOrNode, "Unknow reference name {0} for property {1}", reference.Definition.EndReference.Name, reference.Name);
            }

            if (reference.RelationshipReference == null)
            {
                if (reference.Definition.End != null)
                {
                    var relationshipName = String.Format("{0}{1}{2}", parent.Name, reference.Definition.IsEmbedded ? "Has" : "References", reference.Definition.End.Name);
                    var existing = Compilation.FindTypeSymbol(domain, relationshipName);
                    if (existing == null)
                    {
                        // Change the embedded state
                        var relationshipName2 = String.Format("{0}{1}{2}", parent.Name, !reference.Definition.IsEmbedded ? "Has" : "References", reference.Definition.End.Name);
                        var existing2 = Compilation.FindTypeSymbol(domain, relationshipName2);
                        if (existing2 != null)
                        {
                            existing = existing2;
                            relationshipName = relationshipName2;
                        }
                    }

                    reference.Bind(Compilation, relationshipName);

                    // Create a new virtual relationship 
                    if (existing == null)
                    {
                        var vrel = new VirtualRelationshipSymbol(null, domain, new RelationshipDefinitionSymbol(reference.SyntaxTokenOrNode.AsNode(), null, parent, reference.Definition.End, reference.Definition.Cardinality, reference.Definition.IsEmbedded), relationshipName);
                        domain.Members.Add(relationshipName, vrel);
                        vrel.Definition.PropertySource = reference.Name;
                    }
                    else
                    {
                        var relationship = existing as RelationshipSymbol;
                        if (relationship == null)
                        {
                            AddDiagnostic(reference.NameToken, "An existing element exists with the same name ({0}) but it's not a relationship.", relationshipName);
                        }
                        else
                        {
                            relationship.Definition.PropertySource = reference.Name;
                        }
                    }
                }
            }
            else if (reference.RelationshipReference.Value == null)
            {
                var vrel = new VirtualRelationshipSymbol(null, domain, new RelationshipDefinitionSymbol(reference.SyntaxTokenOrNode.AsNode(), null, parent, reference.Definition.End, reference.Definition.Cardinality, reference.Definition.IsEmbedded), reference.RelationshipReference.Name);
                domain.Members.Add(vrel.Name, vrel);
                vrel.Definition.PropertySource = reference.Name;
            }
            else
            {
                // Match definitions
                if (reference.Relationship == null)
                    AddDiagnostic(reference.RelationshipReference.SyntaxTokenOrNode, "Unknow relationship {0} for property {1}", reference.RelationshipReference.Name, reference.Name);
                else
                {
                    var def1 = reference.Relationship.Definition;
                    var def2 = reference.Definition;
                    if (!def1.Equals(def2))
                        AddDiagnostic(reference.NameToken, "Relationship cardinality mismatch with an existing relationship");
                    reference.Relationship.Definition.PropertySource = reference.Name;
                }
            }
        }

        public void VisitOppositeReferenceSymbol(OppositeReferenceSymbol reference)
        {
            var parent = reference.Parent as ElementSymbol;
            var domain = parent.Domain;

            if (reference.Definition.Source == null)
            {
                AddDiagnostic(reference.Definition.EndReference.SyntaxTokenOrNode, "Unknow reference name {0} for property {1}", reference.Definition.SourceReference.Name, reference.Name);
            }

            if (reference.RelationshipReference == null)
            {
                if (reference.Definition.Source != null)
                {
                    var relationshipName = String.Format("{0}{1}{2}", reference.Definition.Source.Name, reference.Definition.IsEmbedded ? "Has" : "References", parent.Name);
                    var existing = Compilation.FindTypeSymbol(domain, relationshipName);
                    if (existing == null)
                    {
                        // Change the embedded state
                        var relationshipName2 = String.Format("{0}{1}{2}", reference.Definition.Source.Name, !reference.Definition.IsEmbedded ? "Has" : "References", parent.Name);
                        var existing2 = Compilation.FindTypeSymbol(domain, relationshipName2);
                        if (existing2 != null)
                        {
                            existing = existing2;
                            relationshipName = relationshipName2;
                        }
                    }

                    reference.Bind(Compilation, relationshipName);

                    // Create a new virtual relationship
                    if (existing == null)
                    {
                        var vrel = new VirtualRelationshipSymbol(null, domain, new RelationshipDefinitionSymbol(reference.SyntaxTokenOrNode.AsNode(), null, reference.Definition.Source, parent, reference.Definition.Cardinality, reference.Definition.IsEmbedded),relationshipName);
                        domain.Members.Add(relationshipName, vrel);
                        vrel.Definition.PropertyEnd = reference.Name;
                    }
                    else
                    {
                        var relationship = existing as RelationshipSymbol;
                        if (relationship == null)
                        {
                            AddDiagnostic(reference.NameToken, "An existing element exists with the same name ({0}) but it's not a relationship.", relationshipName);
                        }
                        else
                        {
                            relationship.Definition.PropertyEnd = reference.Name;
                        }
                    }
                }
            }
            else if (reference.RelationshipReference.Value == null)
            {
                var vrel = new VirtualRelationshipSymbol(null, domain, new RelationshipDefinitionSymbol(reference.SyntaxTokenOrNode.AsNode(), null, reference.Definition.Source, parent, reference.Definition.Cardinality, reference.Definition.IsEmbedded), reference.RelationshipReference.Name);
                domain.Members.Add(vrel.Name, vrel);
                vrel.Definition.PropertyEnd = reference.Name;
            }
            else
            {
                // Match definitions
                if (reference.Relationship == null)
                    AddDiagnostic(reference.RelationshipReference.SyntaxTokenOrNode, "Unknow relationship {0} for property {1}", reference.RelationshipReference.Name, reference.Name);
                else
                {
                    var def1 = reference.Relationship.Definition;
                    var def2 = reference.Definition;
                    if (!def1.Equals(def2))
                        AddDiagnostic(reference.NameToken, "Relationship cardinality mismatch with an existing relationship");
                    reference.Relationship.Definition.PropertyEnd = reference.Name;
                }
            }

            if (reference.Relationship != null)
            {
                if (!parent.IsA(reference.Relationship.Definition.End))
                    AddDiagnostic(reference.NameToken, "Relationship type mismatch. Cannot convert {0} to {1}", parent.Name, reference.Relationship.Name);
            }

        }
    }
}
