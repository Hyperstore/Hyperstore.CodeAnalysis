using Hyperstore.CodeAnalysis.Syntax;
using Irony.Ast;
using Irony.Parsing;

namespace Hyperstore.CodeAnalysis.DomainLanguage
{

    [Language("Hyperstore", "0.1", "Hyperstore")]
    public class HyperstoreGrammar : Irony.Parsing.Grammar
    {
        public HyperstoreGrammar()
            : base(true)
        {
            CommentTerminal SingleLineComment = new CommentTerminal("SingleLineComment", "//", "\r", "\n", "\u2085", "\u2028", "\u2029");
            CommentTerminal DelimitedComment = new CommentTerminal("DelimitedComment", "/*", "*/");
            NonGrammarTerminals.Add(SingleLineComment);
            NonGrammarTerminals.Add(DelimitedComment);

            var identifier = TerminalFactory.CreateCSharpIdentifier("Identifier");
            identifier.SetFlag(TermFlags.NoAstNode, true);

            var generic_identifier = TerminalFactory.CreateCSharpIdentifier("GIdentifier");
            generic_identifier.SetFlag(TermFlags.NoAstNode, true);
            generic_identifier.AllChars += "<>[],.";

            var stringLiteral = new StringLiteral("string", "\"", StringOptions.AllowsDoubledQuote);
            stringLiteral.SetFlag(TermFlags.NoAstNode, true);

            var lBr = ToTerm("{");
            var rBr = ToTerm("}");

            var qualified_identifier = new NonTerminal("QualifiedIdentifier", typeof(QualifiedNameSyntax));
            KeyTerm dot = ToTerm(".", "dot");

            #region
            //Nonterminals
            var domain = new NonTerminal("Domain", typeof(DomainSyntax));
            var declarations = new NonTerminal("Declarations", typeof(ListSyntax<SyntaxNode>));
            var externalDeclarations = new NonTerminal("ExternalDeclarations", typeof(ListSyntax<ExternalDeclarationSyntax>));
            var externalDeclaration = new NonTerminal("ExternalDeclaration", typeof(ExternalDeclarationSyntax));
            var elementDeclarations = new NonTerminal("ElementDeclarations", typeof(ListSyntax<DeclarationSyntax>));
            var classDeclaration = new NonTerminal("EntityDeclarationSyntax", typeof(EntityDeclarationSyntax));
            var commandDeclaration = new NonTerminal("CommandDeclarationSyntax", typeof(CommandDeclarationSyntax));
            var memberDeclarations = new NonTerminal("MemberDeclarations", typeof(ListSyntax<MemberDeclarationSyntax>));
            var commandMemberDeclarationList = new NonTerminal("CommandMemberDeclarations", typeof(ListSyntax<CommandMemberDeclarationSyntax>));
            var relationshipDeclaration = new NonTerminal("RelationshipDeclaration", typeof(RelationshipDeclarationSyntax));
            var attributeDeclaration = new NonTerminal("Attributes", typeof(PropertySyntax));
            var memberDeclaration = new NonTerminal("MemberDeclaration");
            var commandMemberDeclaration = new NonTerminal("CommandMemberDeclaration", typeof(CommandMemberDeclarationSyntax));
            var referenceDeclaration = new NonTerminal("Reference", typeof(ReferenceDeclarationSyntax));
            var oppositeReferenceDeclaration = new NonTerminal("OppositeReference", typeof(OppositeReferenceSyntax));
            var multiplicity_opt = new NonTerminal("Multiplicity");
            var partialDefinition_opt = new NonTerminal("PartialDefinition");
            var extends_opt = new NonTerminal("Extends");
            var implements_opt = new NonTerminal("Implements");
            var relationshipType = new NonTerminal("RelationshipType");
            var relationshipReference = new NonTerminal("RelationshipReference");
            var elementDeclaration = new NonTerminal("ElementDeclaration");
            var defaultvalue_opt = new NonTerminal("Defaultvalue_opt", typeof(DefaultValueSyntax));
            var csharpDefaultValueExpression = new CSharpCodeBlock("C# default value expression");
            var relationshipDefinition = new NonTerminal("RelationshipDefinition", typeof(RelationshipDefinitionSyntax));
            var qualifiedIdentifiers = new NonTerminal("Identifiers", typeof(SeparatedListSyntax<QualifiedNameSyntax>));
            var externalAttribut_opt = new NonTerminal("ExternalAttribut_opt");
            var usesDeclarationList = new NonTerminal("UsesDeclarations", typeof(ListSyntax<UsesDeclarationSyntax>));
            var usesDeclaration = new NonTerminal("UsesDeclaration", typeof(UsesDeclarationSyntax));
            var valueObjectDeclaration = new NonTerminal("ValueObjectDeclaration", typeof(ValueObjectDeclarationSyntax));

            var generationAttributeList = new NonTerminal("Attributes", typeof(ListSyntax<AttributeSyntax>));
            var generationAttribute = new NonTerminal("Attribute", typeof(AttributeSyntax));
            var attributeParameters_opt = new NonTerminal("AttributeParameters_opt");
            var generationAttributeParams = new NonTerminal("AttributeArguments", typeof(SeparatedListSyntaxToken));

            var partial_opt = new NonTerminal("Partial");
            var alias_opt = new NonTerminal("Alias_opt");
            var whereClause = new CSharpCodeBlock("Where clause");
            var enumDeclaration = new NonTerminal("EnumDeclaration", typeof(EnumDeclarationSyntax));
            var enumValueList = new NonTerminal("EnumValues", typeof(SeparatedListSyntaxToken));
            var selectClause = new CSharpCodeBlock("Select clause");
            var oppositeType = new NonTerminal("OppositeType");
            var message_opt = new NonTerminal("Message_opt");
            var errorLevel_opt = new NonTerminal("ErrorLevel_opt");

            // 'implicit'? constraint '(' string ')' ':' CSHARP
            var constraintDeclaration = new NonTerminal("ConstraintDeclaration", typeof(ConstraintDeclarationSyntax));
            var constraintDeclarations = new NonTerminal("ConstraintDeclarations", typeof(ListSyntax<ConstraintDeclarationSyntax>));
            var constraintDeclaration_opt = new NonTerminal("ConstraintDeclaration_opt");
            var constraintVerb = new NonTerminal("ConstraintVerb");
            var constraintExpression = new CSharpCodeBlock("ConstraintExpression");

            var configurationFilter_opt = new NonTerminal("ConfigurationFilter_opt");
            #endregion

            //Domain
            domain.Rule =
                generationAttributeList + ToTerm("domain", "domain") + qualified_identifier + partialDefinition_opt + "{" + declarations + "}";

            partialDefinition_opt.Rule =
                Empty
                | ToTerm("extends") + stringLiteral;

            qualified_identifier.Rule =
                MakePlusRule(qualified_identifier, dot, identifier);

            // Attributes
            generationAttributeList.Rule =
                MakeStarRule(generationAttributeList, generationAttribute);

            generationAttribute.Rule =
                ToTerm("[") + identifier + attributeParameters_opt + "]";

            attributeParameters_opt.Rule =
                Empty
                | ToTerm("(") + generationAttributeParams + ToTerm(")");

            generationAttributeParams.Rule =
                MakeStarRule(generationAttributeParams, ToTerm(","), stringLiteral);

            // Body
            declarations.Rule =
                usesDeclarationList + externalDeclarations + elementDeclarations;

            // Uses
            usesDeclarationList.Rule =
                MakeStarRule(usesDeclarationList, usesDeclaration);

            usesDeclaration.Rule =
                ToTerm("use", "use") + stringLiteral + "as" + identifier + ";";

            // Extern
            externalDeclarations.Rule =
              MakeStarRule(externalDeclarations, externalDeclaration);

            externalDeclaration.Rule =
              ToTerm("extern", "extern") + externalAttribut_opt + generic_identifier + alias_opt + ";";

            alias_opt.Rule =
                Empty
                | ToTerm("as") + identifier;

            externalAttribut_opt.Rule =
                Empty
                | ToTerm("interface", "interface")
                | ToTerm("enum", "enum");

            // Classes
            elementDeclarations.Rule =
              MakeStarRule(elementDeclarations, elementDeclaration);

            elementDeclaration.Rule =
                classDeclaration
                | enumDeclaration
                | commandDeclaration
                | relationshipDeclaration
                | valueObjectDeclaration;

            valueObjectDeclaration.Rule =
                generationAttributeList + ToTerm("def") + partial_opt + ToTerm("valueObject", "valueObject") + identifier + ToTerm(":") + identifier + lBr
                + constraintDeclaration_opt + rBr;

            classDeclaration.Rule =
                generationAttributeList + ToTerm("def") + partial_opt + ToTerm("entity", "entity") + identifier + extends_opt + implements_opt + lBr + memberDeclarations
                + constraintDeclaration_opt + rBr;

            partial_opt.Rule =
                Empty
                | ToTerm("partial");

            extends_opt.Rule =
              Empty
              | ToTerm("extends") + qualified_identifier;

            implements_opt.Rule =
              Empty
              | ToTerm("implements") + qualifiedIdentifiers;

            qualifiedIdentifiers.Rule =
                MakePlusRule(qualifiedIdentifiers, ToTerm(","), qualified_identifier);

            enumDeclaration.Rule =
                generationAttributeList + ToTerm("def") + ToTerm("enum", "enum") + identifier + lBr + enumValueList + rBr;

            enumValueList.Rule = MakeStarRule(enumValueList, ToTerm(","), identifier);

            commandDeclaration.Rule =
                generationAttributeList + ToTerm("def") + ToTerm("command", "command") + identifier + lBr + commandMemberDeclarationList + rBr;

            commandMemberDeclarationList.Rule =
                MakeStarRule(commandMemberDeclarationList, commandMemberDeclaration);

            commandMemberDeclaration.Rule =
                 generationAttributeList + identifier + ":" + qualified_identifier + ";";

            memberDeclarations.Rule =
              MakeStarRule(memberDeclarations, memberDeclaration);

            memberDeclaration.Rule =
              attributeDeclaration
              | referenceDeclaration + ";"
              | oppositeReferenceDeclaration + ";";

            // Relations
            relationshipDeclaration.Rule =
              generationAttributeList + ToTerm("def") + partial_opt + ToTerm("relationship", "relationship") + identifier + extends_opt + implements_opt
              + "(" + relationshipDefinition + ")"
              + lBr + memberDeclarations
              + constraintDeclaration_opt + rBr;

            constraintDeclaration_opt.Rule =
                Empty
                | ToTerm("constraints") + ToTerm(":") + constraintDeclarations + ";";

            // Propriétés
            attributeDeclaration.Rule =
                 generationAttributeList + identifier + ":" + qualified_identifier + defaultvalue_opt + constraintDeclarations + ";";

            defaultvalue_opt.Rule =
                Empty
                | ToTerm("=") + ToTerm("{") + csharpDefaultValueExpression + ToTerm("}")
                | ToTerm("select") + ToTerm("{") + selectClause + ToTerm("}") + qualified_identifier
                | ToTerm("where") + ToTerm("{") + whereClause + ToTerm("}");

            // Contraintes
            constraintDeclarations.Rule =
                MakeStarRule(constraintDeclarations, constraintDeclaration);

            constraintDeclaration.Rule =
                constraintVerb + errorLevel_opt + message_opt + ToTerm("{") + constraintExpression + ToTerm("}");

            errorLevel_opt.Rule =
                Empty
                | ToTerm("error")
                | ToTerm("warning");

            message_opt.Rule =
                Empty
                | stringLiteral;

            constraintVerb.Rule =
                ToTerm("compute")
                | ToTerm("check")
                | ToTerm("validate");

            // Références attribut
            referenceDeclaration.Rule =
              generationAttributeList + relationshipDefinition + relationshipReference;

            relationshipDefinition.Rule =
              identifier + multiplicity_opt + relationshipType + qualified_identifier + multiplicity_opt;

            oppositeReferenceDeclaration.Rule =
              generationAttributeList + identifier + multiplicity_opt + oppositeType + qualified_identifier + multiplicity_opt + relationshipReference;

            oppositeType.Rule =
                ToTerm("<-", "OppositeReference")
                | ToTerm("<=", "EmbeddedOppositeReference");

            relationshipType.Rule =
                  ToTerm("=>", "Embedded")
                  | ToTerm("->", "Reference");

            multiplicity_opt.Rule =
              Empty
              | "*";

            relationshipReference.Rule =
              Empty
              | ":" + qualified_identifier;

            //Set grammar root
            this.Root = domain;
            MarkPunctuation("{", "}", ":", ";", "(", ")", ",", "[", "]");
            this.RegisterBracePair("{", "}");
            this.MarkTransient(memberDeclaration, elementDeclaration);
            RegisterKeywords("use", "partial", "where", "error", "valueObject", "constraints", "warning", "entity", "extern", "*", "domain", "relationship", "command", "interface", "=>", "->", "<-", "<=", "def", "enum", "extends", "implements", "as", "constraint", "check", "validate", "compute");

            LanguageFlags = LanguageFlags.CreateAst | Irony.Parsing.LanguageFlags.NewLineBeforeEOF;
        }

        public void RegisterKeywords(params string[] keywords)
        {
            foreach (var keyword in keywords)
            {
                var term = ToTerm(keyword);
                // term.SetOption(TermOptions.IsKeyword, true);
                // term.SetOption(TermOptions.IsReservedWord, true);

                this.MarkReservedWords(keyword);
                term.EditorInfo = new TokenEditorInfo(TokenType.Keyword, TokenColor.Keyword, TokenTriggers.None);

            }
        }

        public override void BuildAst(LanguageData language, ParseTree parseTree)
        {
            var astContext = new AstContext(language);
            //  astContext.DefaultNodeType = typeof(DummyNode);
            var astBuilder = new AstBuilder(astContext);
            astBuilder.BuildAst(parseTree);
        }


        //class ExplorerLog : Hyperstore.DomainLanguage.ILog
        //{
        //    private Irony.LogMessageList _messages;

        //    public ExplorerLog(Irony.LogMessageList messages)
        //    {
        //        _messages = messages;
        //    }

        //    public void LogWarning(string message, params string[] args)
        //    {
        //        _messages.Add(new Irony.LogMessage(Irony.ErrorLevel.Warning, new SourceSpan(), String.Format(message, args), null));
        //    }

        //    public void LogMessage(string message, params string[] args)
        //    {
        //        //_messages.Add(new Irony.LogMessage(Irony.ErrorLevel.Info, new SourceSpan(), String.Format(message, args), null));
        //    }

        //    public void LogError(string fileName, int lineNumber, int columnNumber, int endLineNumber, int endColumnNumber, string message)
        //    {
        //        _messages.Add(new Irony.LogMessage(Irony.ErrorLevel.Error, new SourceSpan(new SourceLocation(0, lineNumber, columnNumber), endColumnNumber - columnNumber), message, null));
        //    }

        //    public void LogWarning(string fileName, int lineNumber, int columnNumber, int endLineNumber, int endColumnNumber, string message)
        //    {
        //        _messages.Add(new Irony.LogMessage(Irony.ErrorLevel.Warning, new SourceSpan(new SourceLocation(0, lineNumber, columnNumber), endColumnNumber - columnNumber), message, null));
        //    }
        //}


        //public string RunSample(RunSampleArgs args)
        //{
        //    var language = new LanguageData(new HyperstoreGrammar());
        //    var parser = new Parser(language);

        //    var parseTree = args.ParsedSample;
        //    if (parseTree.ParserMessages.Count == 0)
        //    {
        //        string result = null;
        //        var compiler = new Hyperstore.DomainLanguage.HCompiler(new ExplorerLog(parseTree.ParserMessages), null, "DEBUG", false,
        //            fp =>
        //            {
        //                if (fp == "$")
        //                    return args.Sample;
        //                return File.ReadAllText(fp);
        //            },
        //            (filePath, content) =>
        //            {
        //                if (filePath == "$")
        //                    result = content;
        //            }
        //            );

        //        compiler.Compile(new List<string> { "$" });
        //        if (parseTree.ParserMessages.Count() == 0)
        //        {
        //            return result;
        //        }
        //        else
        //            return String.Join(Environment.NewLine, parseTree.ParserMessages.Select(m => String.Format("{0} ({1},{2})", m.Message, m.SourceSpan.Location.Line + 1, m.SourceSpan.Location.Column)));

        //    }
        //    return null;
        //}

    }
}
