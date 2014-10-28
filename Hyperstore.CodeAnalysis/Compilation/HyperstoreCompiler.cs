using Hyperstore.CodeAnalysis.Syntax;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hyperstore.CodeAnalysis.Compilation
{
    public class HyperstoreCompiler
    {
        private readonly string _basePath;
        private readonly string _outputDirectory;
        private HyperstoreCompilation _compilation;

        private const string OutputFileName = "Domains.g.cs";

        public IEnumerable<Diagnostic> Diagnostics
        {
            get
            {
                return _compilation.GetDiagnostics();
            }
        }

        public HyperstoreCompiler(string outputDirectory, string basePath = null)
        {
            _basePath = basePath;
            _outputDirectory = outputDirectory;
        }

        public bool Run(string[] inputFiles)
        {
            if (inputFiles == null || inputFiles.Length == 0)
            {
                ClearOutputFile();
                return false;
            }

            try
            {
                var trees = new HyperstoreSyntaxTree[inputFiles.Count()];

                //if (trees.Length > 1)
                //{
                //    Parallel.For(0, trees.Length, ix =>
                //    {
                //        var inputFile = inputFiles[ix];

                //        string content;
                //        string normalizedPath;
                //        if (OpenFile(inputFile, out content, out normalizedPath))
                //        {
                //            trees[ix] = HyperstoreSyntaxTree.ParseText(content, normalizedPath);
                //        }
                //    });
                //}
                //else
                var i = 0;
                foreach (var inputFile in inputFiles)
                {
                    string content;
                    string normalizedPath;
                    if (OpenFile(inputFile, out content, out normalizedPath))
                    {
                        trees[i++] = HyperstoreSyntaxTree.ParseText(content, normalizedPath);
                    }
                }

                _compilation = HyperstoreCompilation.Create("C#", trees.Where(t => t != null));
                if (_compilation.HasErrors)
                {
                    ClearOutputFile();
                    return false;
                }

                var output = _compilation.Generate();
                WriteOutputFile(output);
                return true;
            }
            catch (Exception ex)
            {
                ClearOutputFile();
                throw ex;
            }
        }

        private void ClearOutputFile()
        {
            var tmp = MakeOutputFilePath();
            OutputFilePath = null;
            if (File.Exists(tmp))
                File.Delete(tmp);
        }

        private void WriteOutputFile(string output)
        {
            OutputFilePath = MakeOutputFilePath();
            Directory.CreateDirectory(Path.GetDirectoryName(OutputFilePath));
            File.WriteAllText(OutputFilePath, output);
            OutputFilePath = new FileInfo(OutputFilePath).FullName;
        }

        private string MakeOutputFilePath()
        {
            return Path.Combine(_outputDirectory, OutputFileName);
        }

        public bool OpenFile(string inputFile, out string content, out string normalizedPath)
        {
            content = null;
            normalizedPath = _basePath != null ? Path.Combine(_basePath, inputFile) : inputFile;
            if (!File.Exists(normalizedPath))
            {
                AddDiagnostic("File {0} not found.", normalizedPath);
                return false;
            }

            using (var stream = File.OpenRead(normalizedPath))
            {
                using (var reader = new StreamReader(stream))
                {
                    content = reader.ReadToEnd();
                    normalizedPath = stream.Name;
                }
            }
            return true;
        }

        private void AddDiagnostic(string msg, params string[] args)
        {
            var diag = Diagnostic.Create(
                                         args.Length == 0 ? msg : String.Format(msg, args),
                                         DiagnosticSeverity.Error);

        }

        public string OutputFilePath { get; private set; }
    }
}
