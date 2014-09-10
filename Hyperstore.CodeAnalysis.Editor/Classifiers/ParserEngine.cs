using Microsoft.VisualStudio.Text;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Hyperstore.CodeAnalysis.Editor.Classifiers
{
    //class HyperstoreResultEventArgs : ParseResultEventArgs
    //{
    //    public IEnumerable<Section> Sections { get; private set; }

    //    public IEnumerable<Token> Tokens { get; private set; }

    //    public HyperstoreResultEventArgs(IEnumerable<Section> sections, IEnumerable<Token> tokens, ITextSnapshot snapshot, TimeSpan elapsedTime)
    //        : base(snapshot, elapsedTime)
    //    {
    //        Sections = sections;
    //        Tokens = tokens;
    //    }
    //}

    internal class ParserResult
    {
        public List<TokenInfo> Tokens;
        public List<RegionInfo> Regions;

        public ParserResult()
        {
            Tokens = new List<TokenInfo>();
            Regions = new List<RegionInfo>();
        }
    }

    internal class ParserEngine
    {
        private ParserResult _parserResult;

        public IEnumerable<TokenInfo> Tokens { get { var result = _parserResult; return _parserResult != null ? _parserResult.Tokens : Enumerable.Empty<TokenInfo>(); } }

        public IEnumerable<RegionInfo> Regions { get { var result = _parserResult; return _parserResult != null ? _parserResult.Regions : Enumerable.Empty<RegionInfo>(); } }

        public void EnsuresReady(ITextSnapshot snapshot)
        {
            if (_parserResult == null)
            {
                var parser = new HyperstoreParser(snapshot, snapshot.GetText());
                Interlocked.CompareExchange(ref _parserResult, parser.Parse(), null);
            }
        }

        internal void Reset()
        {
            Interlocked.Exchange(ref _parserResult, null);
        }
    }

    enum TokenKind
    {
        Keyword,
        Attribute,
        Comment,
        CSharpCode,
        Normal,
        Separator,
        EOF,
        String
    }

    class RegionInfo
    {
        public ITrackingSpan Span { get; set; }
        public string HoverText { get; set; }
    }

    internal struct TokenInfo
    {
        public static TokenInfo EOF = new TokenInfo(TokenKind.EOF);

        public ITrackingSpan Span { get; set; }
        public TokenKind Kind { get; private set; }
        public string Value { get; private set; }

        public bool IsMultiline { get; private set; }
        private TokenInfo(TokenKind kind)
            : this()
        {
            Kind = kind;
        }

        public TokenInfo(TokenKind kind, ITrackingSpan span, string value = null, bool isMultiline = false)
            : this()
        {
            Kind = kind;
            Span = span;
            Value = value;
            IsMultiline = isMultiline;
        }

        internal bool IsPreCSharpCodeToken(bool inConstraintStatement)
        {
            if (Value == null)
                return false;

            switch (Value)
            {
                case "compute":
                case "select":
                case "where":
                case "=":
                    return true;
                case ":":
                    return inConstraintStatement;
                default:
                    return false;
            }
        }
    }

    class HyperstoreParser
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
        private bool _inConstraintStatement;
        private readonly ITextSnapshot _snapshot;
        private TokenInfo _lastToken;

        static HyperstoreParser()
        {
            Keywords = new HashSet<string>
            {
                "domain", "extends", "extern", "interface", "enum", "as", "def", "entity", "relationship", "implements", "where", "compute", "select", "check", "validate", "command", "uses"
            };
        }

        public HyperstoreParser(ITextSnapshot snapshot, string text)
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
                if (_currentChar == '\r')
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

        public ParserResult Parse(bool processRegions = true)
        {
            var result = new ParserResult();

            RegionData region = null;
            for (; ; )
            {
                var token = NextToken();
                if (token.Kind == TokenKind.EOF)
                    break;

                if (processRegions)
                {
                    try
                    {
                        if (region != null)
                        {

                            if (region.HoverText == null && token.Kind == TokenKind.Normal)
                            {
                                region.HoverText = token.Value;
                            }
                            else if (token.Kind == TokenKind.Separator && token.Value == "}")
                            {
                                var tokenSpan = token.Span.GetSpan(_snapshot);
                                var startLine = _snapshot.GetLineFromPosition(region.Start);
                                var endLine = _snapshot.GetLineFromPosition(tokenSpan.Start + tokenSpan.Length + 1);
                                var r = new RegionInfo { Span = CreateSpan(startLine.Start, endLine.End - startLine.Start), HoverText = region.HoverText };
                                result.Regions.Add(r);
                                region = null;
                            }
                        }

                        if (token.Kind == TokenKind.Keyword && token.Value == "def")
                        {
                            region = new RegionData { Start = token.Span.GetSpan(_snapshot).Start.Position };
                        }

                        if (token.Kind == TokenKind.Comment && token.IsMultiline)
                        {
                            var tokenSpan = token.Span.GetSpan(_snapshot);
                            var startLine = _snapshot.GetLineFromPosition(tokenSpan.Start);
                            var endLine = _snapshot.GetLineFromPosition(tokenSpan.End);
                            var r = new RegionInfo { Span = CreateSpan(startLine.Start, endLine.End - startLine.Start), HoverText = null };
                            result.Regions.Add(r);
                        }
                    }
                    catch
                    {
                        processRegions = false;
                    }
                }

                if (token.Kind != TokenKind.Normal)
                    result.Tokens.Add(token);
            }
            return result;
        }

        public TokenInfo NextToken()
        {
            SkipWhiteSpace();

            TokenInfo token;
            if (_lastToken.IsPreCSharpCodeToken(_inConstraintStatement))
            {
                token = ParseCSharpCode(_lastToken.Value != "=");
                _inConstraintStatement = false;
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

            var val = new StringBuilder();
            while (_currentChar != Eof)
            {
                if (IsWhitespace(_currentChar) || IsSeparator(_currentChar))
                {
                    var str = val.ToString();
                    var isKeyword = Keywords.Contains(str);
                    _inConstraintStatement |= str == "check" || str == "validate";
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
                    if (_currentChar == '\r')
                        multiline |= true;
                    NextChar();
                }
            }

            return new TokenInfo(TokenKind.Comment, CreateSpan(startPosition, _position - startPosition), null, multiline);
        }

        private TokenInfo ParseCSharpCode(bool allowBlock)
        {
            var startPosition = _position - 1;
            bool inString = false;
            int blockLevel = 0;
            var multiline = false;
            while (_currentChar != Eof)
            {
                if (_currentChar == '\'' || _currentChar == '"')
                {
                    if (PeekNextChar() != _currentChar)
                        inString = !inString;
                }
                else if (!inString)
                {
                    if (_currentChar == '{')
                    {
                        if (!allowBlock)
                        {
                            NextChar();
                            return new TokenInfo(TokenKind.CSharpCode, CreateSpan(startPosition, _position - startPosition - 1), null, multiline);
                        }
                        else
                        {
                            blockLevel++;
                        }
                    }
                    else if (_currentChar == '}')
                    {
                        NextChar();
                        blockLevel--;
                        if (blockLevel <= 0)
                            return new TokenInfo(TokenKind.CSharpCode, CreateSpan(startPosition, _position - startPosition - 1), null, multiline);
                    }
                    else if (_currentChar == ';')
                    {
                        if (blockLevel == 0)
                        {
                            NextChar();
                            return new TokenInfo(TokenKind.CSharpCode, CreateSpan(startPosition, _position - startPosition - 1), null, multiline);
                        }
                    }
                }
                if (_currentChar == '\r')
                    multiline |= true;
                NextChar();
            }
            return new TokenInfo(TokenKind.CSharpCode, CreateSpan(startPosition, _position - startPosition - 1), null, multiline);
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
                else if (_currentChar == '\r' && !allowMultiline)
                {
                    return new TokenInfo(kind, CreateSpan(startPosition, _position - startPosition - 1), null, multiline);
                }
                else
                {
                    if (_currentChar == '\r')
                        multiline |= true;
                    NextChar();
                }
            }

            return new TokenInfo(kind, CreateSpan(startPosition, _position - startPosition - 1), null, multiline);
        }

        private void SkipWhiteSpace()
        {
            while (true)
            {
                while (true)
                {
                    if (_currentChar == Eof || !IsWhitespace(_currentChar))
                        break;
                    NextChar();
                }

                if (_currentChar == '#')
                {
                    do
                    {
                        NextChar();
                    }
                    while (_currentChar != '\r');
                }
                else break;
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
