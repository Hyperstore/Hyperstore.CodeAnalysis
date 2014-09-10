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
    enum DefinitionState
    {
        None,
        Simple = 1,
        WithExtends = 3,
        WithImplements = 5,
        Enum = 9,
        Extern = 17,
        Uses = 33,
        Domain=64
    }

    class TokenList : List<TokenInfo>
    {
        public TokenInfo GetPreviousToken(TokenInfo token, int nb=1)
        {
            var pos = IndexOf(token);
            pos -= nb;
            if (pos >= 0)
                return this[pos];
            return null;
        }

        public DefinitionState IsInDefinition(TokenInfo currentToken)
        {
            var state = DefinitionState.None;

            var prv = currentToken;
            while (prv != null && prv.Kind != TokenKind.Separator)
            {
                if (prv.Value == "def")
                {
                    if( state == DefinitionState.None)
                        state |= DefinitionState.Simple;
                    return state;
                }
                if (prv.Value == "domain")
                    return DefinitionState.Domain;
                if (prv.Value == "extends")
                    state |= DefinitionState.WithExtends;
                if (prv.Value == "implements")
                    state |= DefinitionState.WithImplements;
                if (prv.Value == "uses")
                    state |= DefinitionState.Uses;
                if (prv.Value == "extern")
                    state |= DefinitionState.Extern;
                if (prv.Value == "enum")
                    state |= DefinitionState.Enum;
                prv = GetPreviousToken(prv);
            }

            return state;
        }

        private static TokenList _empty = new TokenList();
        public static TokenList Empty { get { return _empty; } }
    }

    internal class ParserResult
    {
        public TokenList Tokens;
        public List<RegionInfo> Regions;
        public ITextSnapshot Snapshot { get; private set; }

        public ParserResult(ITextSnapshot snapshot)
        {
            Snapshot = snapshot;
            Tokens = new TokenList();
            Regions = new List<RegionInfo>();
        }

    }
}
