using Irony.Ast;
using Irony.Parsing;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hyperstore.CodeAnalysis.Syntax
{
    public abstract class SyntaxNode : IAstNodeInit
    {
        private readonly List<Hyperstore.CodeAnalysis.Syntax.SyntaxNode> _childNodes = new List<Hyperstore.CodeAnalysis.Syntax.SyntaxNode>();
        private readonly List<Hyperstore.CodeAnalysis.Syntax.TokenOrNode> _childNodesAndTokens = new List<Hyperstore.CodeAnalysis.Syntax.TokenOrNode>();

        protected SyntaxNode()
        {
        }

        public virtual IEnumerable<Hyperstore.CodeAnalysis.Syntax.SyntaxNode> ChildNodes { get { return _childNodes; } }

        public virtual IEnumerable<Hyperstore.CodeAnalysis.Syntax.TokenOrNode> ChildNodesAndTokens { get { return _childNodesAndTokens; } }

        #region IAstNodeInit Members
        void IAstNodeInit.Init(AstContext context, ParseTreeNode parseNode)
        {
            InitCore(context, parseNode);
        }

        public HyperstoreSyntaxTree SyntaxTree
        {
            get { return Location != null ? Location.SyntaxTree : null; }
            internal set { Location.SyntaxTree = value; }
        }

        protected virtual void InitCore(AstContext context, ParseTreeNode treeNode)
        {
            Location = new Location(null, new TextSpan(treeNode.Span));
            treeNode.AstNode = this;
        }

        #endregion

        public Location Location { get; private set; }


        public Hyperstore.CodeAnalysis.Syntax.SyntaxNode Parent { get; set; }

        public T FindParent<T>() where T : Hyperstore.CodeAnalysis.Syntax.SyntaxNode
        {
            var p = this.Parent;
            while (p != null && !(p is T))
            {
                p = p.Parent;
            }
            return (T)p;
        }

        protected void AddChild(SyntaxToken token)
        {
            if (token == null)
                return;
            _childNodesAndTokens.Add(new TokenOrNode(token));
        }
        protected Hyperstore.CodeAnalysis.Syntax.SyntaxNode AddChild(Hyperstore.CodeAnalysis.Syntax.SyntaxNode child)
        {
            if (child == null)
                return null;
            child.Parent = this;
            _childNodes.Add(child);
            _childNodesAndTokens.Add(new TokenOrNode(child));
            return child;
        }
    }
}
