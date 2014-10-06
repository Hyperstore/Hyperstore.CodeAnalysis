using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hyperstore.CodeAnalysis.Symbols;

namespace Hyperstore.CodeAnalysis.Generation
{
    static class TypeSymbolExtensions
    {
        public static string AsDefinitionVariable(this ITypeSymbol element, IDomainSymbol currentDomain)
        {
            if (element is IExternSymbol)
            {
                return ((IExternSymbol)element).FullName;
            }

            if (element is IPrimitiveSymbol)
            {
                return String.Format("global::Hyperstore.Modeling.Metadata.PrimitivesSchema.{0}Schema", element.Name);
            }

            var domain = element.Parent as IDomainSymbol;
            if (String.Compare(domain.QualifiedName, currentDomain.QualifiedName, StringComparison.OrdinalIgnoreCase) == 0)
                return element.Name;

            return String.Format("schema.Store.GetSchema{2}(\"{0}.{1}\")", domain.QualifiedName, element.Name, element is IRelationshipSymbol ? "Relationship" : element is IEntitySymbol ? "Entity" : "Element");
        }

        public static string AsFullName(this ITypeSymbol element)
        {
            if (element is IExternSymbol)
            {
                return ((IExternSymbol)element).FullName;
            }

            if (element is IValueObjectSymbol)
            {
                return ((IValueObjectSymbol)element).Type.AsFullName();
            }

            var domain = element.Parent as IDomainSymbol;
            return String.Format("global::{0}.{1}", domain.Namespace, element.Name);
        }

        public static string ToCamelCase(this string txt)
        {
            return String.Format("{0}{1}", Char.ToLower(txt[0]), txt.Substring(1));
        }
    }
}
