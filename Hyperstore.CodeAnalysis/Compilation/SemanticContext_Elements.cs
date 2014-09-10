using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hyperstore.CodeAnalysis.Symbols;

namespace Hyperstore.CodeAnalysis.Compilation
{
    partial class SemanticContext : HyperstoreSymbolVisitor
    {
        public override void VisitEntitySymbol(EntitySymbol symbol)
        {

            var element = symbol as ElementSymbol;
            CheckConstraints(element.Constraints);

            CheckExtends(element);

            CheckImplements(element);
        }

        public override void VisitRelationshipSymbol(RelationshipSymbol symbol)
        {
            var element = symbol as ElementSymbol;
            CheckConstraints(element.Constraints);

            CheckExtends(element);

            CheckImplements(element);
        }

        private void CheckImplements(ElementSymbol element)
        {
            var set = new HashSet<string>();
            foreach (var r in element.ImplementReferences)
            {
                if (r.Value == null)
                    AddDiagnostic(r.SyntaxTokenOrNode, "Invalid implementation type {0} for {1}", r.Name, element.Name);
                else if (!set.Add(r.Value.QualifiedName))
                {
                    AddDiagnostic(r.SyntaxTokenOrNode, "Duplicate implementation type {0} for {1}", r.Name, element.Name);
                }
                else
                {
                    var ext = r.Value as IExternSymbol;
                    if( ext == null || ext.Kind != ExternalKind.Interface)
                        AddDiagnostic(r.SyntaxTokenOrNode, "Incorrect type {0} for {1}. Must be an interface declared by an extern interface statement.", r.Name, element.Name);
                }
            }
        }

        private void CheckExtends(ElementSymbol element)
        {
            TypeSymbol extendsReference = null;
            foreach (var extends in element.ExtendsReferences)
            {
                var r = extends.Value;
                if (r == null)
                    AddDiagnostic(extends.SyntaxTokenOrNode, "Invalid extends type {0} for {1}", extends.Name, element.Name);
                else if (r == element)
                    AddDiagnostic(extends.SyntaxTokenOrNode, "Circular base class dependency {0} for {1}", extends.Name, element.Name);
                else
                {
                    if (extendsReference != null && extendsReference.QualifiedName != r.QualifiedName)
                    {
                        AddDiagnostic(extends.SyntaxTokenOrNode, "Multiple extends definition founded for {0}", element.Name);
                        break;
                    }
                    extendsReference = r;
                }
            }

            if (element.SuperType != null)
            {
                element.SuperType.AddDerived(element);
                element.HasClassInheritance = true;
            }
        }
    }
}
