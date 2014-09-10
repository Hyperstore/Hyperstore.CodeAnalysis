using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Irony;
using Irony.Parsing;

namespace Hyperstore.CodeAnalysis.DomainLanguage
{
    public class CSharpCodeBlock : Terminal
    {
        public CSharpCodeBlock(string name)
            : base(name)
        {
            base.SetFlag(TermFlags.IsLiteral);
        }

        public override Token TryMatch(ParsingContext context, ISourceStream source)
        {
            if (context.PreviousToken != null && context.PreviousToken.Text == "{")
                return TryMatchContentSimple(context, source);
            return null;
        }

        private Token TryMatchContentSimple(ParsingContext context, ISourceStream source)
        {
            var startPos = source.PreviewPosition;
            var stringComp = Grammar.CaseSensitive ? StringComparison.InvariantCulture : StringComparison.InvariantCultureIgnoreCase;

            var parser = new CSharpCodeParser(source.Text, source.PreviewPosition);
            var r = parser.Parse();

            source.PreviewPosition = r.Item1;
            if (source.PreviewPosition == source.Text.Length)
                return context.CreateErrorToken(Resources.ErrFreeTextNoEndTag, '}');
            return source.CreateToken(this.OutputTerminal, source.Text.Substring(startPos, source.PreviewPosition - startPos));
        }
    }
}
