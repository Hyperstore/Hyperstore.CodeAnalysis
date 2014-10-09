using Microsoft.VisualStudio.Text;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Hyperstore.CodeAnalysis.Editor.Parser
{
    class HyperstoreLexer
    {
        private class RegionData
        {
            public int Start;
            public string HoverText;
        }

        private static readonly HashSet<string> Keywords;

        private readonly string _buffer;
        private int _position;
        private const char Eof = '\0';
        private char _currentChar;
        public int Line = 1;
        public int Linepos;
        private readonly ITextSnapshot _snapshot;
        private TokenInfo _lastToken;

        static HyperstoreLexer()
        {
            Keywords = new HashSet<string>
            {
                "domain", "extends", "extern", "interface", "error", "warning", "valueObject", "enum", "as", "def", "entity", "constraints", "relationship", "implements", "where", "compute", "select", "check", "validate", "command", "use", "partial"
            };
        }

        public HyperstoreLexer(ITextSnapshot snapshot, string text)
        {
            _snapshot = snapshot;
            _buffer = text;
            _lastToken = TokenInfo.EOF;
            NextChar();
        }

        private void NextChar()
        {
            if (_position < _buffer.Length)
            {
                if (_currentChar == '\n')
                {
                    Line++;
                    Linepos = 0;
                }
                else if (_currentChar == '\t')
                {
                    Linepos += 4;
                }
                else
                {
                    Linepos++;
                }
                _currentChar = _buffer[_position++];
            }
            else
            {
                _currentChar = Eof;
            }
        }

        private char PeekNextChar()
        {
            return _position < _buffer.Length ? _buffer[_position] : Eof;
        }

        public ParserResult Scan(bool processRegions = true)
        {
            var result = new ParserResult(_snapshot);

            RegionData region = null;
            bool skipToken = false;
            var token = NextTokenSafe();
            for (; ; )
            {
                if (token.Kind == TokenKind.EOF)
                    break;

                if (processRegions)
                {
                    try
                    {
                        var tokenSpan = token.Span.GetSpan(_snapshot);
                        if (region != null)
                        {

                            if (region.HoverText == null && token.Kind == TokenKind.Normal)
                            {
                                region.HoverText = token.Value;
                            }
                            else if (token.Kind == TokenKind.Separator && token.Value == "}")
                            {
                                // Create a region if this is not the last token (the domain end token)
                                // Since region are used for completion, we need to know if we are in a incomplete definition or not.
                                var previousToken = token;
                                skipToken = true;
                                token = NextToken(); // We need to read the next token

                                if (token.Kind == TokenKind.EOF) // Exit without creating the region
                                    break;

                                var r = CreateRegion(region, previousToken);
                                result.Regions.Add(r);

                                region = null;
                            }
                        }

                        if (token.Kind == TokenKind.Keyword && token.Value == "def")
                        {
                            region = new RegionData { Start = tokenSpan.Start.Position };
                        }

                        if (token.Kind == TokenKind.Comment && token.IsMultiline)
                        {
                            var startLine = _snapshot.GetLineFromPosition(tokenSpan.Start);
                            var endLine = _snapshot.GetLineFromPosition(tokenSpan.End);
                            var r = new RegionInfo { Span = CreateSpan(startLine.Start, endLine.End - startLine.Start), HoverText = null, Type = RegionType.Comment };
                            result.Regions.Add(r);
                        }
                    }
                    catch
                    {
                        processRegions = false;
                    }
                }

                result.Tokens.Add(token);
                if (!skipToken)
                    token = NextToken(region != null);
                skipToken = false;
            }

            return result;
        }

        private RegionInfo CreateRegion(RegionData region, TokenInfo token)
        {
            var tokenSpan = token.Span.GetSpan(_snapshot);
            var startLine = _snapshot.GetLineFromPosition(region.Start);
            var endPosition = tokenSpan.Start.Position + tokenSpan.Length + 1;
            if (endPosition > _snapshot.Length)
                endPosition = _snapshot.Length;

            var endLine = _snapshot.GetLineFromPosition(endPosition);
            var r = new RegionInfo { Span = CreateSpan(startLine.Start, endLine.End - startLine.Start), HoverText = region.HoverText, Type = RegionType.Definition };
            return r;
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
        private TokenInfo NextTokenSafe()
        {
            try
            {
                return NextToken();
            }
            catch (TypeLoadException)
            {
                System.Windows.MessageBox.Show("Unable to load Hyperstore compiler assembly. Ensure you are the last version installed of the Hyperstore Designer and the last version of the Hyperstore Domain Language Nuget package.");
            }
            return TokenInfo.EOF;
        }

        private TokenInfo NextToken(bool isInRegion = false)
        {
            SkipWhiteSpace();

            TokenInfo token;
            if (_currentChar == '{' && _lastToken.IsPreCSharpCodeToken(isInRegion))
            {
                var r = new CSharpCodeParser(_buffer, _position).Parse();
                token = new TokenInfo(TokenKind.CSharpCode, CreateSpan(_position, r.Item1 - _position), null, r.Item2);
                _position = r.Item1 + 1;
                NextChar(); // Skip final }
            }
            else if (_currentChar == Eof)
                token = TokenInfo.EOF;
            else if (_currentChar == '\"')
                token = ParseBlock(TokenKind.String, '\"', false);
            else if (_currentChar == '[')
                token = ParseBlock(TokenKind.Attribute, ']', true);
            else
                token = ParseToken();

            _lastToken = token;
            return token;
        }

        private TokenInfo ParseToken()
        {
            var oldChar = _currentChar;
            var startPosition = _position - 1;
            if (_currentChar == '/')
            {
                var ch = PeekNextChar();
                if (ch == '/')
                    return ParseBlock(TokenKind.Comment, Eof, false);
                if (ch == '*')
                    return ParseMultiComment();
            }

            if (IsSeparator(_currentChar))
            {
                var str = new String(_currentChar, 1);
                NextChar();
                return new TokenInfo(TokenKind.Separator, CreateSpan(startPosition, 1), str);
            }

            if (_currentChar == '=' || _currentChar == '-' || _currentChar == '<')
            {
                NextChar();
                if (oldChar == '=' && _currentChar == '>')
                {
                    NextChar();
                    return new TokenInfo(TokenKind.Keyword, CreateSpan(startPosition, _position - startPosition - 1), "=>");
                }
                if (oldChar == '-' && _currentChar == '>')
                {
                    NextChar();
                    return new TokenInfo(TokenKind.Keyword, CreateSpan(startPosition, _position - startPosition - 1), "->");
                }

                if (oldChar == '<' && _currentChar == '=')
                {
                    NextChar();
                    return new TokenInfo(TokenKind.Keyword, CreateSpan(startPosition, _position - startPosition - 1), "<=");
                }

                if (oldChar == '<' && _currentChar == '-')
                {
                    NextChar();
                    return new TokenInfo(TokenKind.Keyword, CreateSpan(startPosition, _position - startPosition - 1), "<-");
                }
                if (oldChar == '=')
                    return new TokenInfo(TokenKind.Keyword, CreateSpan(startPosition, _position - startPosition - 1), "=");
            }

            var val = new StringBuilder();
            while (_currentChar != Eof)
            {
                if (IsWhitespace(_currentChar) || IsSeparator(_currentChar))
                {
                    var str = val.ToString();
                    var isKeyword = Keywords.Contains(str);
                    return new TokenInfo(isKeyword ? TokenKind.Keyword : TokenKind.Normal, CreateSpan(startPosition, _position - startPosition - 1), str);
                }

                val.Append(_currentChar);
                NextChar();
            }

            return TokenInfo.EOF;
        }

        private ITrackingSpan CreateSpan(int start, int length)
        {
            return _snapshot.CreateTrackingSpan(new Span(start, length), SpanTrackingMode.EdgeExclusive);
        }

        private TokenInfo ParseMultiComment()
        {
            var startPosition = _position - 1;
            var multiline = false;
            NextChar();
            NextChar();
            while (_currentChar != Eof)
            {
                if (_currentChar == '*')
                {
                    NextChar();
                    if (_currentChar == '/')
                    {
                        NextChar();
                        return new TokenInfo(TokenKind.Comment, CreateSpan(startPosition, _position - startPosition), null, multiline);
                    }
                }
                else
                {
                    if (IsNewLine(_currentChar))
                        multiline |= true;
                    NextChar();
                }
            }

            return new TokenInfo(TokenKind.Comment, CreateSpan(startPosition, _position - startPosition), null, multiline);
        }

        private TokenInfo ParseBlock(TokenKind kind, char terminal, bool allowMultiline)
        {
            var startPosition = _position - 1;
            NextChar();
            var multiline = false;
            while (_currentChar != Eof)
            {
                if (_currentChar == terminal)
                {
                    NextChar();
                    if (_currentChar != terminal)
                        return new TokenInfo(kind, CreateSpan(startPosition, _position - startPosition - 1), null, multiline);
                }
                else if (IsNewLine(_currentChar) && !allowMultiline)
                {
                    return new TokenInfo(kind, CreateSpan(startPosition, _position - startPosition - 1), null, multiline);
                }
                else
                {
                    if (IsNewLine(_currentChar))
                        multiline |= true;
                    NextChar();
                }
            }

            return new TokenInfo(kind, CreateSpan(startPosition, _position - startPosition - 1), null, multiline);
        }

        private bool IsNewLine(char ch)
        {
            return ch == '\r' || ch == '\n';
        }

        private void SkipWhiteSpace()
        {

            while (true)
            {
                if (_currentChar == Eof || !IsWhitespace(_currentChar))
                    break;
                NextChar();
            }
        }

        private static bool IsSeparator(char ch)
        {
            return (ch == '{' || ch == '}' || ch == '(' || ch == ')' || ch == ':' || ch == ';' || ch == '[' || ch == ']');
        }

        private bool IsWhitespace(char ch)
        {
            switch (ch)
            {
                case ' ':
                case '\n':
                case '\t':
                case '\r':
                    return true;
                default:
                    return false;
            }
        }
    }
}
