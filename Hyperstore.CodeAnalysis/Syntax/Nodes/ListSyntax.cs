using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hyperstore.CodeAnalysis.Syntax
{
    public sealed class ListSyntax<T> : SyntaxNode, IEnumerable<T> where T : SyntaxNode
    {
        private List<T> _list = new List<T>();

        private static ListSyntax<T> _empty = new ListSyntax<T>();
        public static ListSyntax<T> Empty { get { return _empty; } }

        public override IEnumerable<SyntaxNode> ChildNodes
        {
            get
            {
                return _list;
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
                _list.Add((T)treeNode.ChildNodes[i++].AstNode);
            }
        }

        public T this[int index]
        {
            get { return _list[index]; }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _list.GetEnumerator();
        }
    }
}
