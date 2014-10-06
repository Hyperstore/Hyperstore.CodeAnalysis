using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Hyperstore.CodeAnalysis.Compilation;

namespace Hyperstore.CodeAnalysis.MsBuild
{
    public class HyperstoreGeneratorTask : Task
    {
        [Required]
        public ITaskItem OutputPath { get; set; }

        [Required]
        public ITaskItem Config { get; set; }

        [Required]
        public ITaskItem[] DomainFiles { get; set; }

        [Output]
        public ITaskItem[] GeneratedCodeFiles { get; set; }

        public override bool Execute()
        {
            Log.LogMessageFromText(String.Format("Run domain model generator from {0}", this.GetType().Assembly.CodeBase), MessageImportance.Normal);

            if (DomainFiles.Count() == 0)
            {
                Log.LogWarning("No domain files to compile. Ensures 'Build Action' are set to 'None'.");
                return true;
            }

            try
            {
                var compiler = new HyperstoreCompiler(OutputPath.ItemSpec);
                if (!compiler.Run(DomainFiles.Select(s => s.ItemSpec).ToArray()))
                {
                    foreach (var diag in compiler.Diagnostics)
                    {
                        switch (diag.Severity)
                        {
                            case DiagnosticSeverity.Error:
                                Log.LogError("Hyperstore", null, null, diag.Location.SourceFile, diag.Location.Line, diag.Location.Column, diag.Location.Line, diag.Location.Column + diag.Location.Length, diag.Message);
                                break;
                            case DiagnosticSeverity.Warning:
                                Log.LogWarning("Hyperstore", null, null, diag.Location.SourceFile, diag.Location.Line, diag.Location.Column, diag.Location.Line, diag.Location.Column + diag.Location.Length, diag.Message);
                                break;
                            default:
                                break;
                        }
                    }
                    GeneratedCodeFiles = new ITaskItem[0];
                }
                else
                {
                    GeneratedCodeFiles = new TaskItem[] { new TaskItem(compiler.OutputFilePath) };
                    Log.LogMessageFromText(String.Format("File generated : {0}.", compiler.OutputFilePath), MessageImportance.Normal);
                }
            }
            catch (Exception ex)
            {
                Log.LogErrorFromException(ex);
            }
            return true;
        }
    }
}
