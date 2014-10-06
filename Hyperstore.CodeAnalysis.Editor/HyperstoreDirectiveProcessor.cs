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
            return new string[0];
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
                location
            };
        }

        public override bool IsDirectiveSupported(string directiveName)
        {
            return string.Compare(directiveName, "Domain", StringComparison.OrdinalIgnoreCase) == 0;
        }

        public override void ProcessDirective(string directiveName, IDictionary<string, string> arguments)
        {
            var filePath = GetFilePath(arguments);
            if (filePath == null)
                return;

            _codeBuffer.AppendFormat("protected global::Hyperstore.CodeAnalysis.Symbols.IDomainSymbol Domain {{get {{return global::Hyperstore.CodeAnalysis.T4.HyperstoreProcessorHelper.LoadDomainFromFile(@\"{0}\");}} }}", filePath);
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
        public static IDomainSymbol LoadDomainFromFile(string filePath)
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
            var compilation = HyperstoreCompilation.Create(new HyperstoreSyntaxTree[] { tree }, new VSHyperstoreResolver());

            if (compilation.HasErrors)
            {
                return null;
            }

            var model = compilation.GetSemanticModel(tree);
            return model != null ? model.Domain : null;
        }
    }
}
