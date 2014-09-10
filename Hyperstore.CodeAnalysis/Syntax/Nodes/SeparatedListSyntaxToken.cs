using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hyperstore.CodeAnalysis.Syntax
{
    public sealed class SeparatedListSyntaxToken : SyntaxNode, IEnumerable<SyntaxToken>
    {
        private List<SyntaxToken> _list = new List<SyntaxToken>();
        public SyntaxToken Separator { get; private set; }

        private static SeparatedListSyntaxToken _empty = new SeparatedListSyntaxToken();
        public static SeparatedListSyntaxToken Empty { get { return _empty; } }

        public override IEnumerable<SyntaxNode> ChildNodes
        {
            get
            {
                return Enumerable.Empty<SyntaxNode>();
            }
        }

        public override IEnumerable<TokenOrNode> ChildNodesAndTokens
        {
            get
            {
                return _list.Select(n => new TokenOrNode(n));
            }
        }

        protected override void InitCore(Irony.Ast.AstContext context, Irony.Parsing.ParseTreeNode treeNode)
        {
            base.InitCore(context, treeNode);

            for (int i = 0; i < treeNode.ChildNodes.Count; )
            {
                _list.Add(new SyntaxToken(treeNode.ChildNodes[i++].Token));
                if (Separator == null && i < treeNode.ChildNodes.Count)
                {
                    Separator = new SyntaxToken(treeNode.ChildNodes[i].Token);
                }
                i++;
            }
        }

        public IEnumerator<SyntaxToken> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _list.GetEnumerator();
        }
    }
}
