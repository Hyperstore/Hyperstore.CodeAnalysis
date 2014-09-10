using Irony.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hyperstore.CodeAnalysis
{
    public enum DiagnosticSeverity
    {
        Error,
        Warning,
        Info
    }

    public class Location
    {
        public string SourceFile { get; private set; }
        public int Position { get; private set; }

        public int Length { get; private set; }

        public int Line { get; private set; }
        public int Column { get; private set; }

        public Location(int position, int length, int line=0, int column=0, string sourceFile=null)
        {
            Position = position;
            Line = line+1;
            Column = column+1;
            SourceFile = sourceFile;
            Length = length;
        }

        public override string ToString()
        {
            return String.Format("{0} ({1},{2})", SourceFile, Line, Column);
        }
    }

    public class Diagnostic
    {
        public string Message { get; private set; }

        public DiagnosticSeverity Severity { get; private set; }

        public Location Location { get; private set; }

        private Diagnostic(string message, DiagnosticSeverity severity, Location location=null)
        {
            Message = message;
            Location = location;
            Severity = severity;
        }

        public override string ToString()
        {
            if (Location == null)
                return String.Format("[{0}] - {1}", Severity, Message);

            return String.Format("{1} : [{0}] - {2}", Severity, Location, Message);
        }

        public static Diagnostic Create(string message, DiagnosticSeverity severity)
        {
            return new Diagnostic(message, severity);
        }

        public static Diagnostic Create(string message, DiagnosticSeverity severity, SourceSpan span, string path)
        {
            var loc = new Location(span.Location.Position, span.Length, span.Location.Line, span.Location.Column, path); 
            return new Diagnostic(message, severity, loc);
        }

    }
}
