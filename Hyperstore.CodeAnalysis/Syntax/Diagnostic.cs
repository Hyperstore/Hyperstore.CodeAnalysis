using Hyperstore.CodeAnalysis.Syntax;
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
        public HyperstoreSyntaxTree SyntaxTree { get; internal set; }
        public TextSpan SourceSpan { get; private set; }

        public Location(HyperstoreSyntaxTree syntaxTree, TextSpan sourceSpan)
        {
            SyntaxTree = syntaxTree;
            this.SourceSpan = sourceSpan;
        }
    }

    public struct TextSpan
    {
        public static TextSpan Empty = new TextSpan(0, 0);

        private readonly int _start;
        private readonly int _length;
        private readonly int _line;
        private readonly int _column;

        public int End { get { return _start + _length; } }

        public int Start { get { return _start; } }

        public int Length { get { return _length; } }

        public int Line { get { return _line; } }

        public int Column { get { return _column; } }

        public TextSpan(int position, int length, int line = 0, int column = 0)
        {
            _start = position;
            _line = line + 1;
            _column = column + 1;
            _length = length;
        }

        internal TextSpan(SourceSpan sourceSpan)
            : this(sourceSpan.Location, sourceSpan.Length)
        {
        }

        internal TextSpan(SourceLocation location, int length)
            : this(location.Position, length, location.Line, location.Column)
        {
        }

        public override string ToString()
        {
            return String.Format("({1},{2})", Line, Column);
        }
    }

    public class Diagnostic
    {
        public string Message { get; private set; }

        public DiagnosticSeverity Severity { get; private set; }

        public Location Location { get; private set; }

        private Diagnostic(string message, DiagnosticSeverity severity, Location location = null)
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
        
        public static Diagnostic Create(string message, DiagnosticSeverity severity, Location loc=null)
        {
            return new Diagnostic(message, severity, loc);
        }

    }
}
