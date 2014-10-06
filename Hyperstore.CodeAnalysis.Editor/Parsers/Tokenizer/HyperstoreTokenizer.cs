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
    internal class HyperstoreTokenizer
    {
        private ParserResult _parserResult;
        private readonly object _sync = new object();

        public TokenList Tokens { get { var result = _parserResult; return _parserResult != null ? _parserResult.Tokens : TokenList.Empty; } }

        public IEnumerable<RegionInfo> Regions { get { var result = _parserResult; return _parserResult != null ? _parserResult.Regions : Enumerable.Empty<RegionInfo>(); } }

        public void EnsuresReady(ITextSnapshot snapshot)
        {
            if (_parserResult == null || _parserResult.Snapshot != snapshot)
            {
                lock (_sync)
                {
                    if (_parserResult == null || _parserResult.Snapshot != snapshot)
                    {
                        Debug.WriteLine("Reparse version " + snapshot.Version.ToString());

                        var lexer = new HyperstoreLexer(snapshot, snapshot.GetText());
                        Interlocked.Exchange(ref _parserResult, lexer.Scan());
                    }
                }
            }
        }
    }

}
