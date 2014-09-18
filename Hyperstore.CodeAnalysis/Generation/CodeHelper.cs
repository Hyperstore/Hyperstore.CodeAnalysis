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
        public static string AsDefinitionVariable(this ITypeSymbol element)
        {
            if (element is IExternSymbol)
            {
                return ((IExternSymbol)element).FullName;
            }
            var domain = element.Parent as IDomainSymbol;
            return String.Format("global::{0}Definition.{1}", domain.QualifiedName, element.Name);
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
