using Irony.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hyperstore.CodeAnalysis.Syntax
{
    public enum SyntaxWalkerDepth
    {
        Node,
        Token,
        Trivia
    }

    /// <summary>
    /// Walks the syntax tree, allowing subclasses to operate on all nodes, token and trivia.  The
    /// walker will perform a depth first walk of the tree.
    /// </summary>
    public class HyperstoreSyntaxWalker
    {
        /// <summary>
        /// True if this walker will descend into structured trivia.
        /// </summary>
        protected readonly SyntaxWalkerDepth Depth;

        private readonly HyperstoreSyntaxVisitor _visitor;

        /// <summary>
        /// Creates a new walker instance.
        /// </summary>
        /// <param name="depth">specify how much this walker will descent into
        /// trivia.</param>
        public HyperstoreSyntaxWalker(HyperstoreSyntaxVisitor visitor, SyntaxWalkerDepth depth = SyntaxWalkerDepth.Node)
        {
            this.Depth = depth;
            _visitor = visitor;
        }

        /// <summary>
        /// Called when the walker visits a node.  This method may be overridden if subclasses want
        /// to handle the node.  Overrides should call back into this base method if they want the
        /// children of this node to be visited.
        /// </summary>
        /// <param name="node">The current node that the walker is visiting.</param>
        public virtual void Visit(Hyperstore.CodeAnalysis.Syntax.SyntaxNode node)
        {
            _visitor.Visit(node);

            foreach (var child in node.ChildNodesAndTokens)
            {
                if (child.IsNode)
                {
                    if (this.Depth >= SyntaxWalkerDepth.Node)
                    {
                        Visit(child.AsNode());
                    }
                }
                else if (child.IsToken)
                {
                    if (this.Depth >= SyntaxWalkerDepth.Token)
                    {
                        VisitToken(child.AsToken());
                    }
                }
            }
        }

        /// <summary>
        /// Called when the walker visits a token.  This method may be overridden if subclasses want
        /// to handle the token.  Overrides should call back into this base method if they want the 
        /// trivia of this token to be visited.
        /// </summary>
        /// <param name="token">The current token that the walker is visiting.</param>
        protected virtual void VisitToken(Hyperstore.CodeAnalysis.Syntax.SyntaxToken token)
        {
            _visitor.VisitSyntaxToken(token);

            if (this.Depth >= SyntaxWalkerDepth.Trivia)
            {
                this.VisitLeadingTrivia(token);
                this.VisitTrailingTrivia(token);
            }
        }

        private void VisitLeadingTrivia(Hyperstore.CodeAnalysis.Syntax.SyntaxToken token)
        {
            foreach (var trivia in token.LeadingTrivia)
            {
                VisitTrivia(trivia);
            }
        }

        private void VisitTrailingTrivia(Hyperstore.CodeAnalysis.Syntax.SyntaxToken token)
        {
            foreach (var trivia in token.TrailingTrivia)
            {
                VisitTrivia(trivia);
            }
        }

        /// <summary>
        /// Called when the walker visits a trivia syntax.  This method may be overridden if
        /// subclasses want to handle the token.  Overrides should call back into this base method if
        /// they want the children of this trivia syntax to be visited.
        /// </summary>
        /// <param name="trivia">The current trivia syntax that the walker is visiting.</param>
        protected virtual void VisitTrivia(Trivia trivia)
        {

        }
    }
}