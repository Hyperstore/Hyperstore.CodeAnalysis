using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hyperstore.CodeAnalysis.Symbols;
using Hyperstore.CodeAnalysis.Syntax;

namespace Hyperstore.CodeAnalysis.Compilation
{
    class ModelBuilder : Hyperstore.CodeAnalysis.Compilation.IModelBuilder
    {
        internal DomainSymbol Domain { get; private set; }

        IDomainSymbol IModelBuilder.Domain { get { return this.Domain; } }

        public HyperstoreSyntaxTree SyntaxTree { get; private set; }

        public HyperstoreCompilation Compilation { get; private set; }

        public ModelBuilder(HyperstoreCompilation compilation)
        {
            Compilation = compilation;
        }

        public void Build(HyperstoreSyntaxTree syntaxTree)
        {
            syntaxTree.Built = true;
            SyntaxTree = syntaxTree;
            var domainNode = syntaxTree.Root;
            if (domainNode == null)
                return;

            var name = domainNode.QualifiedName.FullName;
            Domain = new DomainSymbol(domainNode, domainNode.QualifiedName);
            if (String.IsNullOrEmpty(name))
                Compilation.AddDiagnostic(domainNode.QualifiedName.Location, "Domain name is required");

            if (domainNode.Extends != null)
            {
                var extension = domainNode.Extends.Text;
                if (String.IsNullOrEmpty(extension))
                {
                    Compilation.AddDiagnostic(domainNode.Extends.Location, "A domain path is required.");
                }
                else
                {
                    try
                    {
                        var extendedDomain = Compilation.DomainManager.FindDomain(syntaxTree, extension);
                        if (extendedDomain == null)
                        {
                            Compilation.AddDiagnostic(domainNode.Extends.Location, "Unable to found domain to extends {0}", extension);
                        }
                        else if (extendedDomain.Domain.QualifiedName != name)
                        {
                            Compilation.AddDiagnostic(domainNode.Extends.Location, "Extension must have the same name than the extended domain {0}", name);
                        }
                    }
                    catch (Exception)
                    {
                        Compilation.AddDiagnostic(domainNode.Extends.Location, "Invalid domain path {0}", extension);
                    }
                }

                Domain.ExtendedDomainUri = domainNode.Extends;
            }

            BuildAttributes(Domain, Domain.Attributes, domainNode.Attributes);

            BuildUsesStatement(syntaxTree, domainNode.Uses);
            BuildExternals(domainNode.Externals);
            BuildValueObjects(domainNode.Elements.OfType<Syntax.ValueObjectDeclarationSyntax>());
            BuildEnums(domainNode.Elements.OfType<Syntax.EnumDeclarationSyntax>());
            BuildEntities(domainNode.Elements.OfType<Syntax.EntityDeclarationSyntax>());
            BuildRelationships(domainNode.Elements.OfType<Syntax.RelationshipDeclarationSyntax>());
            BuildCommands(domainNode.Elements.OfType<Syntax.CommandDeclarationSyntax>());
        }


        private void BuildAttributes(Symbol parent, List<AttributeSymbol> attributes, Syntax.ListSyntax<Syntax.AttributeSyntax> nodes)
        {
            foreach (var node in nodes)
            {
                var name = node.Name.Text;
                if (attributes.Any(a => String.CompareOrdinal(a.Name, name) == 0))
                    Compilation.AddDiagnostic(node.Name.Location, "Duplicate attribute {0}", name);
                else
                {
                    attributes.Add(new AttributeSymbol(node, parent, node.Name)
                                        {
                                            Arguments = node.Arguments.Select(p => p.Text)
                                        });
                }
            }
        }

        private void BuildConstraints(Symbol parent, List<ConstraintSymbol> constraints, Syntax.ListSyntax<Syntax.ConstraintDeclarationSyntax> nodes)
        {
            constraints.AddRange(nodes.Select(c =>
                {
                    var kind = ToConstraintKind(c.Verb.Text);
                    var symbol = new ConstraintSymbol(c, parent, c.Message);
                    symbol.Condition = new CSharpCodeSymbol(c.Condition, symbol, c.Condition.Text, kind == ConstraintKind.Compute ? CSharpCodeKind.Compute : CSharpCodeKind.Constraint);
                    symbol.Kind = kind;
                    if (kind == ConstraintKind.Compute && c.ErrorLevel != null)
                        Compilation.AddDiagnostic(c.ErrorLevel.Location, "Syntax error. Expected C# code block.");
                    if (kind != ConstraintKind.Compute)
                        symbol.AsError = c.ErrorLevel.Text == "error";
                    return symbol;
                }));
        }

        private void BuildRelationships(IEnumerable<Syntax.RelationshipDeclarationSyntax> relationships)
        {
            BuildElements<IRelationshipSymbol>(relationships, (node, nameToken) =>
            {
                var rel = node as Syntax.RelationshipDeclarationSyntax;
                var def = rel.Definition;

                var symbol = new RelationshipSymbol(node, Domain, nameToken);

                symbol.Definition = new RelationshipDefinitionSymbol(node, symbol, this.Compilation,
                                        def.Name, def.SourceMultiplicity != null && def.SourceMultiplicity.Text == "*",
                                        def.TargetType, def.TargetMultiplicity != null && def.TargetMultiplicity.Text == "*",
                                        def.Kind.Text == "=>");
                return symbol;
            });
        }

        private void BuildEntities(IEnumerable<Syntax.EntityDeclarationSyntax> entities)
        {
            BuildElements<IEntitySymbol>(entities, (node, nameToken) => new EntitySymbol(node, Domain, nameToken));
        }

        private void BuildCommands(IEnumerable<CommandDeclarationSyntax> commands)
        {
            foreach (var commandNode in commands)
            {
                var elementName = commandNode.Name.Text;

                TypeSymbol tElement;
                CommandSymbol command;
                if (Domain.Members.TryGetValue(elementName, out tElement))
                {
                    Compilation.AddDiagnostic(commandNode.Name.Location, "Duplicate element name {0}", elementName);
                    continue;
                }
                else
                {
                    command = new CommandSymbol(commandNode, Domain, commandNode.Name);
                    Domain.Members.Add(elementName, command);
                }

                BuildAttributes(command, command.Attributes, commandNode.Attributes);

                var set = new HashSet<string>();
                foreach (var member in commandNode.Properties)
                {
                    var memberName = member.Name.Text;
                    if (!set.Add(memberName) || command.Properties.Any(m => m.Name == memberName))
                    {
                        Compilation.AddDiagnostic(member.Name.Location, "Duplicate member {0} in element {1}", memberName, elementName);
                        continue;
                    }

                    var prop = member as Syntax.CommandMemberDeclarationSyntax;
                    if (prop != null)
                    {
                        var p = new CommandPropertySymbol(this.Compilation, member, command, prop.PropertyType, prop.Name);
                        BuildAttributes(p, p.Attributes, member.Attributes);
                        command.Properties.Add(p);
                    }
                }
            }
        }

        private void BuildValueObjects(IEnumerable<ValueObjectDeclarationSyntax> elements)
        {
            foreach (var node in elements)
            {
                var elementName = node.Name.Text;
                var isPartial = node.Partial != null;

                TypeSymbol tElement;
                ValueObjectSymbol element;
                if (Domain.Members.TryGetValue(elementName, out tElement))
                {
                    element = tElement as ValueObjectSymbol;
                    if (element == null || (!element.IsPartial && !isPartial))
                    {
                        Compilation.AddDiagnostic(node.Name.Location, "Duplicate element name {0}", elementName);
                        continue;
                    }
                }
                else
                {
                    element = new ValueObjectSymbol(Compilation, node, Domain, node.Name, node.Type);
                    Domain.Members.Add(elementName, element);
                }

                element.IsPartial |= isPartial;

                BuildAttributes(element, element.Attributes, node.Attributes);
                BuildConstraints(element, element.Constraints, node.Constraints);
            }
        }


        private void BuildElements<T>(IEnumerable<Syntax.ElementDeclarationSyntax> elements, Func<Syntax.ElementDeclarationSyntax, SyntaxToken, ElementSymbol> elementFactory) where T : IElementSymbol
        {
            foreach (var node in elements)
            {
                var elementName = node.Name.Text;
                var isPartial = node.Partial != null;

                TypeSymbol tElement;
                ElementSymbol element;
                if (Domain.Members.TryGetValue(elementName, out tElement))
                {
                    element = tElement as ElementSymbol;
                    if (element == null || !(element is T) || (!element.IsPartial && !isPartial))
                    {
                        Compilation.AddDiagnostic(node.Name.Location, "Duplicate element name {0}", elementName);
                        continue;
                    }
                }
                else
                {
                    element = elementFactory(node, node.Name);
                    Domain.Members.Add(elementName, element);
                }

                element.IsPartial |= isPartial;

                element.Bind(this.Compilation, node.Extends, node.Implements);

                BuildAttributes(element, element.Attributes, node.Attributes);
                BuildConstraints(element, element.Constraints, node.Constraints);

                var set = new HashSet<string>();
                foreach (var member in node.Members)
                {
                    var memberName = member.Name.Text;
                    if (!set.Add(memberName) || element.Members.Any(m => m.Name == memberName))
                    {
                        Compilation.AddDiagnostic(member.Name.Location, "Duplicate member {0} in element {1}", memberName, elementName);
                        continue;
                    }

                    var prop = member as Syntax.PropertySyntax;
                    if (prop != null)
                    {
                        var p = new PropertySymbol(this.Compilation, member, element, prop.PropertyType, prop.Name);

                        p.WhereClause = prop.DefaultValue.Kind != null && prop.DefaultValue.Kind.Text == "where" ? new CSharpCodeSymbol(prop.DefaultValue, p, prop.DefaultValue.Code.Text, CSharpCodeKind.WhereClause) : null;
                        p.SelectClause = prop.DefaultValue.Kind != null && prop.DefaultValue.Kind.Text == "select" ? new CSharpCodeSymbol(prop.DefaultValue, p, prop.DefaultValue.Code.Text, CSharpCodeKind.SelectClause) : null;
                        p.DefaultValue = prop.DefaultValue.Kind != null && prop.DefaultValue.Kind.Text == "=" ? new CSharpCodeSymbol(prop.DefaultValue, p, prop.DefaultValue.Code.Text, CSharpCodeKind.DefaultValue) : null;

                        if (prop.DefaultValue.Kind != null && prop.DefaultValue.Kind.Text == "where" && p.WhereClause == null)
                            Compilation.AddDiagnostic(prop.Location, "Where clause can not be empty.");

                        if (prop.DefaultValue.Kind != null && prop.DefaultValue.Kind.Text == "select" && p.SelectClause == null)
                            Compilation.AddDiagnostic(prop.Location, "Select clause can not be empty.");

                        if (prop.DefaultValue.Kind != null && prop.DefaultValue.Kind.Text == "=" && p.DefaultValue == null)
                            Compilation.AddDiagnostic(prop.Location, "Default value can not be empty.");

                        BuildConstraints(p, p.Constraints, prop.Constraints);
                        BuildAttributes(p, p.Attributes, member.Attributes);
                        element.Members.Add(p);
                        continue;
                    }

                    var reference = member as Syntax.ReferenceDeclarationSyntax;
                    if (reference != null)
                    {
                        var propertyReference = new PropertyReferenceSymbol(this.Compilation, member, element, reference.RelationshipName, reference.Definition.Name);

                        propertyReference.Definition = new RelationshipDefinitionSymbol(member, propertyReference, this.Compilation,
                                                            node.Name,
                                                            reference.Definition.SourceMultiplicity != null && reference.Definition.SourceMultiplicity.Text == "*",
                                                            reference.Definition.TargetType,
                                                            reference.Definition.TargetMultiplicity != null && reference.Definition.TargetMultiplicity.Text == "*",
                                                            reference.Definition.Kind.Text == "=>");
                        BuildAttributes(propertyReference, propertyReference.Attributes, member.Attributes);
                        element.Members.Add(propertyReference);
                        continue;
                    }

                    var opposite = member as Syntax.OppositeReferenceSyntax;
                    if (opposite != null)
                    {
                        var propertyReference = new OppositeReferenceSymbol(this.Compilation, member, element, opposite.RelationshipName, opposite.Name);
                        propertyReference.Definition = new RelationshipDefinitionSymbol(member, propertyReference, this.Compilation,
                                                            opposite.TargetType,
                                                            opposite.TargetMultiplicity != null && opposite.TargetMultiplicity.Text == "*",
                                                            node.Name,
                                                            opposite.SourceMultiplicity != null && opposite.SourceMultiplicity.Text == "*",
                                                            opposite.Kind.Text == "<=");
                        BuildAttributes(propertyReference, propertyReference.Attributes, member.Attributes);
                        element.Members.Add(propertyReference);
                        continue;
                    }
                }
            }
        }

        private ConstraintKind ToConstraintKind(string verb)
        {
            switch (verb)
            {
                case "check":
                    return ConstraintKind.Check;
                case "validate":
                    return ConstraintKind.Validate;
                default:
                    return ConstraintKind.Compute; ;
            }
        }

        private void BuildEnums(IEnumerable<Syntax.EnumDeclarationSyntax> enums)
        {
            foreach (var node in enums)
            {
                var name = node.Name.Text;
                if (Domain.Members.ContainsKey(name))
                    Compilation.AddDiagnostic(node.Name.Location, "Duplicate element name {0}", name);
                else
                {
                    var symbol = new EnumSymbol(node, Domain, node.Name);
                    BuildAttributes(symbol, symbol.Attributes, node.Attributes);

                    Domain.Members.Add(name, symbol);

                    var set = new HashSet<string>();
                    foreach (var val in node.Values)
                    {
                        var txt = val.Text;
                        if (!set.Add(txt))
                            Compilation.AddDiagnostic(val.Location, "Duplicate value {0} in enum {1}", txt, name);
                    }
                    symbol.Values = new List<string>(set);
                }
            }
        }

        private void BuildExternals(Syntax.ListSyntax<Syntax.ExternalDeclarationSyntax> externals)
        {
            foreach (var external in externals)
            {
                var alias = external.Alias != null ? external.Alias.Text : external.QualifiedName.Text;
                var qn = external.QualifiedName.Text;

                ExternalKind kind = ExternalKind.Normal;
                if (external.Kind != null)
                {
                    switch (external.Kind.Text)
                    {
                        case "interface":
                            kind = ExternalKind.Interface;
                            break;
                        case "enum":
                            kind = ExternalKind.Enum;
                            break;
                        default:
                            kind = ExternalKind.Normal;
                            break;
                    }
                }
                if (Domain.Members.ContainsKey(alias))
                    Compilation.AddDiagnostic(external.Alias.Location, "Duplicate element name {0}", alias);
                else
                    Domain.Members.Add(alias, new ExternSymbol(external, Domain, external.Alias != null ? external.Alias : external.QualifiedName) { FullName = qn, Kind = kind });
            }
        }

        private void BuildUsesStatement(HyperstoreSyntaxTree syntaxTree, Syntax.ListSyntax<Syntax.UsesDeclarationSyntax> uses)
        {
            foreach (var stmt in uses)
            {
                var alias = stmt.Alias.Text;
                var uri = stmt.Uri.Text;
                if (Domain.Usings.Any(d => alias == d.Name))
                    Compilation.AddDiagnostic(stmt.Alias.Location, "Duplicate alias {0}", alias);
                else
                {
                    var symbol = new UsingSymbol(stmt, Domain, stmt.Alias, stmt.Uri);
                    Domain.AddUsing(alias, symbol);
                }
            }
        }

    }
}
