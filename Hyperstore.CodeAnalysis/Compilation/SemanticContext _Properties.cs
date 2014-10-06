using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hyperstore.CodeAnalysis.Symbols;

namespace Hyperstore.CodeAnalysis.Compilation
{
    partial class SemanticContext : HyperstoreSymbolVisitor
    {       
        public void VisitPropertySymbol(PropertySymbol property)
        {
            if (property.PropertyType == null)
            {
                AddDiagnostic(property.PropertyTypeReference.SyntaxTokenOrNode, "Unknow type {0} for property {1}. Must be a primitive types (string,int, double, bool, datetime, timespan, single..) or an external type", property.PropertyTypeReference.Name, property.Name);
            }

            var rel = property.PropertyType as IRelationshipSymbol;
            if (rel != null)
            {
                if (property.DefaultValue != null)
                    AddDiagnostic(property.NameToken, "Only where expression is allowed on a relationship reference for property {0}.", property.Name);

                if (rel.Definition.Cardinality == RelationshipCardinality.OneToOne && property.WhereClause != null)
                    AddDiagnostic(property.NameToken, "Where clause is only allowed on a multiple cardinality for property {0}.", property.Name);

                if (property.Constraints.Count() > 0)
                    AddDiagnostic(property.NameToken, "Constraints are not allowed on a reference for property {0}.", property.Name);

            }
            else
            {
                if (property.WhereClause != null || property.SelectClause != null)
                    AddDiagnostic(property.NameToken, "Only default value expression is allowed on an attribute for property {0}.", property.Name);
            }


            var parent = property.Parent as ElementSymbol;
            CheckConstraints(property.Constraints);
        }

        public void VisitCommandPropertySymbol(CommandPropertySymbol property)
        {
            if (property.PropertyType == null)
            {
                AddDiagnostic(property.NameToken, "Unknow type {0} for property {1}. Must be a primitive types (string,int, double, bool, datetime, timespan, single..) or an external type", property.PropertyTypeReference.Name, property.Name);
            }
        }
    }
}
