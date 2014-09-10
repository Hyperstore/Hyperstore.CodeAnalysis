using Microsoft.VisualStudio.Text;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Hyperstore.CodeAnalysis.Editor.Parsers
{
    [DebuggerDisplay("{Value}")]
    internal class TokenInfo
    {
        public static TokenInfo EOF = new TokenInfo(TokenKind.EOF);

        public ITrackingSpan Span { get; set; }
        public TokenKind Kind { get; private set; }
        public string Value { get; private set; }

        public bool IsMultiline { get; private set; }
        private TokenInfo(TokenKind kind)
        {
            Kind = kind;
        }

        public TokenInfo(TokenKind kind, ITrackingSpan span, string value = null, bool isMultiline = false)
        {
            Kind = kind;
            Span = span;
            Value = value;
            IsMultiline = isMultiline;
        }

        internal bool IsPreCSharpCodeToken()
        {
            if (Value == null)
                return Kind == TokenKind.String;

            switch (Value)
            {
                case "compute":
                case "select":
                case "where":
                case "=":
                case "error":
                case "warning":
                    return true;
                default:
                    return false;
            }
        }
    }
}
