using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hyperstore.CodeAnalysis.Symbols;

namespace Hyperstore.CodeAnalysis.Compilation
{
    partial class SemanticContext : HyperstoreSymbolVisitor
    {
        public void VisitEntitySymbol(EntitySymbol symbol)
        {

            var element = symbol as ElementSymbol;
            CheckConstraints(element.Constraints);

            CheckExtends(element);

            CheckImplements(element);
        }

        public void VisitValueObjectSymbol(ValueObjectSymbol element)
        {
            CheckConstraints(element.Constraints);

            var type = element.TypeReference.Value as IExternSymbol;
            bool error = true;
            if (type != null && type.Kind == ExternalKind.Primitive)
            {
                switch (type.Name)
                {
                    case "string":
                    case "int":
                    case "bool":
                    case "char":
                    case "decimal":
                    case "double":
                    case "float":
                    case "Guid":
                    case "Int16":
                    case "Int32":
                    case "Int64":
                    case "UInt16":
                    case "UInt32":
                    case "UInt64":
                    case "DateTime":
                    case "TimeSpan":
                        error = false;
                        break;
                }
            }

            if (error)
                AddDiagnostic(element.TypeReference.SyntaxTokenOrNode, "Invalid primitive type {0} for {1}. Only primitive types are allowed.", element.TypeReference.Name, element.Name);
        }

        public void VisitRelationshipSymbol(RelationshipSymbol symbol)
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
                    if (ext == null || ext.Kind != ExternalKind.Interface)
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
                if (!element.HasAttribute("IgnoreGeneration")) // Else element will be not generated
                    element.HasGeneratedClassInheritance = true;
            }
        }
    }
}
