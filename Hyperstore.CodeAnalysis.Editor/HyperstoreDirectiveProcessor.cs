using Hyperstore.CodeAnalysis.Compilation;
using Hyperstore.CodeAnalysis.Editor.Resolver;
using Hyperstore.CodeAnalysis.Symbols;
using Hyperstore.CodeAnalysis.Syntax;
using Microsoft.VisualStudio.TextTemplating;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hyperstore.CodeAnalysis.Editor;

namespace Hyperstore.CodeAnalysis.T4
{
    // <#@ Domain Processor="HyperstoreProcessor" [File="domain file"] [Target="JS,C#"]#>
    class HyperstoreProcessor : DirectiveProcessor
    {
        private StringBuilder _codeBuffer = new StringBuilder();
        private string _currentFileName;

        private CompilerErrorCollection _errorsValue;
        public new CompilerErrorCollection Errors
        {
            get { return _errorsValue; }
        }

        public override void FinishProcessingRun()
        {
           
        }

        public override string GetClassCodeForProcessingRun()
        {
            return _codeBuffer.ToString();
        }

        public override string[] GetImportsForProcessingRun()
        {
            return new string[] {"Hyperstore.CodeAnalysis.Symbols", "System", "System.Linq"};
        }

        public override string GetPostInitializationCodeForProcessingRun()
        {
            return String.Empty;
        }

        public override string GetPreInitializationCodeForProcessingRun()
        {
            return String.Empty;
        }

        public override string[] GetReferencesForProcessingRun()
        {
            var location = this.GetType().Assembly.Location;
            return new string[]
            {
                Path.Combine(Path.GetDirectoryName(location), "Hyperstore.CodeAnalysis.dll"),
                Path.Combine(Path.GetDirectoryName(location), "Hyperstore.CodeAnalysis.Irony.dll"),
                "System.Core",
                location
            };
        }

        public override bool IsDirectiveSupported(string directiveName)
        {
            return string.Compare(directiveName, "Domains", StringComparison.OrdinalIgnoreCase) == 0;
        }

        public override void ProcessDirective(string directiveName, IDictionary<string, string> arguments)
        {
            var filePath = GetFilePath(arguments);
            if (filePath == null)
                return;

            string config;
            if (arguments.TryGetValue("Target", out config))
                config = "\"" + config + "\"";
            else
                config= "null";

            _codeBuffer.AppendFormat("protected global::System.Collections.Generic.IEnumerable<global::Hyperstore.CodeAnalysis.Symbols.IDomainSymbol> Domains {{get {{return global::Hyperstore.CodeAnalysis.T4.HyperstoreProcessorHelper.LoadDomainsFromFile({0}, @\"{1}\");}} }}", config, filePath);
            _codeBuffer.AppendLine();
        }

        public override void Initialize(Microsoft.VisualStudio.TextTemplating.ITextTemplatingEngineHost host)
        {
            base.Initialize(host);
            _currentFileName = host.TemplateFile;
        }

        public override void StartProcessingRun(CodeDomProvider languageProvider, string templateContents, CompilerErrorCollection errors)
        {
            base.StartProcessingRun(languageProvider, templateContents, errors);
            _errorsValue = errors;
        }

        private string GetFilePath(IDictionary<string, string> arguments)
        {
            string filePath;
            if (arguments.TryGetValue("File", out filePath))
            {
                filePath = Path.Combine(Path.GetDirectoryName(_currentFileName), filePath);
            }
            else
            {
                filePath = Path.ChangeExtension(_currentFileName, ".domain");
            }

            if( !File.Exists(filePath))
            {
                _errorsValue.Add(new CompilerError(_currentFileName, 1, 1, "H0001", "Domain definition file not found. You can add a 'File=' argument or the T4 file must have the same name than the domain definition file."));
                return null;
            }
            return filePath;
        }
    }

    public class HyperstoreProcessorHelper 
    {
        public static IEnumerable<IDomainSymbol> LoadDomainsFromFile(string config, string filePath)
        {
            string content;
            try
            {
                content = File.ReadAllText(filePath);
            }
            catch (Exception)
            {
                return null;
            }

            var tree = HyperstoreSyntaxTree.ParseText(content, filePath);
            var compilation = HyperstoreCompilation.Create(config, new HyperstoreSyntaxTree[] { tree }, new VSHyperstoreResolver());

            if (compilation.HasErrors)
            {
                return null;
            }

            return compilation.GetMergedDomains();
        }
    }
}
