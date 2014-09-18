using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hyperstore.CodeAnalysis
{
    public class CSharpCodeParser
    {
        private readonly string _buffer;
        private int _position;
        private const char Eof = Char.MaxValue;
        private bool _isMultiline;
        private int _blockLevel;

        public CSharpCodeParser(string text, int position)
        {
            _buffer = text;
            _position = position;
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>
        ///  Parse c sharp code.
        /// </summary>
        /// <returns>
        ///  A tuple : Item1 = length, Item2=multipline
        /// </returns>
        ///-------------------------------------------------------------------------------------------------
        public Tuple<int, bool> Parse()
        {
            var done = false;
            while (!done)
            {
                var ch = PeekChar();
                if (ch == Eof)
                    break;

                switch (ch)
                {
                    case ' ':
                    case '\t':
                    case '\f':
                    case '\v':
                    case '\u001A':
                    case '\n':
                    case '\r':
                        AdvanceChar();
                        break;
                    case '@':
                        AdvanceChar();
                        if (PeekChar() == '"')
                        {
                            ParseVerbatimString();
                        }
                        break;
                    case '\'':
                    case '\"':
                        ParseString();
                        break;
                    case '/':
                        if (PeekChar(1) == '/')
                        {
                            ParseSinglelineComment();
                        }
                        else if (PeekChar(1) == '*')
                        {
                            AdvanceChar(2);
                            ParseMultilineComment();
                        }
                        AdvanceChar();
                        break;
                    case '{':
                        _blockLevel++;
                        AdvanceChar();
                        break;
                    case '}':
                        if (_blockLevel == 0)
                        {
                            done = true;
                            break;
                        }
                        _blockLevel--;
                        AdvanceChar();
                        break;
                    default:
                        AdvanceChar();
                        break;
                }
            }

            return Tuple.Create(Math.Min(_position, _buffer.Length), _isMultiline);
        }

        private void ParseVerbatimString()
        {
            AdvanceChar();
            for (; ; )
            {
                var ch = PeekChar();
                if (ch == Eof)
                    break;
                
                if (ch == '"')
                {
                    AdvanceChar();
                    if (PeekChar() != '"')
                    {
                        break;
                    }
                    AdvanceChar();
                }
                AdvanceChar();
            }           
        }

        private void ParseMultilineComment()
        {
            for (; ; )
            {
                var ch = PeekChar();
                if (ch == Eof)
                    break;
                if (ch == '*' && PeekChar(1) == '/')
                {
                    AdvanceChar(2);
                    break;
                }
                AdvanceChar();
            }
        }

        private void ParseSinglelineComment()
        {
            Char ch;
            while (!IsNewLine(ch = PeekChar()) && ch != Eof)
            {
                AdvanceChar();
            }
        }

        private void ParseString()
        {
            var quotedCharacter = PeekChar();
            AdvanceChar();
            for (; ; )
            {
                var ch = PeekChar();
                if (ch == '\\')
                {
                    AdvanceChar();
                }
                else if (ch == Eof || IsNewLine(ch))
                {
                    break;
                }
                else if (ch == quotedCharacter)
                {
                    AdvanceChar();
                    break;
                } 

                AdvanceChar();
            }
        }

        public static bool IsNewLine(char ch)
        {
            // new-line-character:
            //   Carriage return character (U+000D)
            //   Line feed character (U+000A)
            //   Next line character (U+0085)
            //   Line separator character (U+2028)
            //   Paragraph separator character (U+2029)

            return ch == '\r'
                || ch == '\n'
                || ch == '\u0085'
                || ch == '\u2028'
                || ch == '\u2029';
        }

        private void AdvanceChar(int delta = 1)
        {
            _position += delta;
        }

        private char PeekChar(int delta = 0)
        {
            var ch = _position + delta < _buffer.Length ? _buffer[_position + delta] : Eof;
            _isMultiline |= IsNewLine(ch);
            return ch;
        }
    }
}
