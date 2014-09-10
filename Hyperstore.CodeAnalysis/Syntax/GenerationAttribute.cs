using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Irony;
using Irony.Ast;

using Irony.Parsing;

namespace Hyperstore.Modeling.TextualLanguage
{
    public class GenerationAttributeNode : TestRoslyn.Syntax.SyntaxNode
    {
        public string Name { get; private set; }
        public List<string> Arguments { get; private set; }

        protected override void InitCore( AstContext context, ParseTreeNode treeNode )
        {
            base.InitCore( context, treeNode );

            Name = treeNode.ChildNodes[0].FindTokenAndGetText();

            Arguments = new List<string>();
            if( treeNode.ChildNodes[1].ChildNodes.Count > 0 )
            {
                foreach( var child in treeNode.ChildNodes[1].ChildNodes[0].ChildNodes )
                {
                    if (child.IsPunctuationOrEmptyTransient())
                        continue;

                    if( child.Token != null )
                        Arguments.Add( child.Token.ValueString );
                }
            }
        }

        //public override void AcceptVisitor( IAstVisitor visitor )
        //{
        //    base.AcceptVisitor( visitor );

        //    var ctx =  ( SemanticAnalysisVisitor )visitor ;
        //    var domain = ctx.Domain;
        //    if (Name == null)
        //        ctx.Messages.Add(new LogMessage(ErrorLevel.Error, this.Span, "Invalid attribute name " + Name, null));
        //    else
        //    {
        //        Name = Name.ToLower();

        //        if (Name == "observable" || Name == "dynamic")
        //        {
        //            if (Arguments.Count != 0)
        //                ctx.Messages.Add(new LogMessage(ErrorLevel.Error, this.Span, "Attribute must have no argument", null));
        //        }
        //        else if (Name == "modifier" || Name == "attribute")
        //        {
        //            if (Arguments.Count != 1 || String.IsNullOrEmpty(Arguments[0]))
        //                ctx.Messages.Add(new LogMessage(ErrorLevel.Error, this.Span, "Attribute must have one argument", null));
        //        }
        //        //else if( Name == "extension" )
        //        //{
        //        //    if(  Arguments.Count > 1 || (Arguments.Count == 1 && Arguments[0].ToLower() != "readonly"))
        //        //        ctx.Messages.Add( new LogMessage( ErrorLevel.Error, this.Span, "Attribute extension arguments error. Must be 'readonly' or nothing.", null ) );
        //        //}
        //        else if( Name == "index" )
        //        {
        //            if( Arguments.Count < 1 || Arguments.Count > 2 )
        //                ctx.Messages.Add( new LogMessage( ErrorLevel.Error, this.Span, "Attribute index arguments error : (\"unique\", [\"IndexName\"])", null ) );
        //            else
        //            {
        //                try
        //                {
        //                    bool.Parse( Arguments[0] );
        //                }
        //                catch
        //                {
        //                    ctx.Messages.Add( new LogMessage( ErrorLevel.Error, this.Span, "Attribute index arguments error : (\"unique\", [\"IndexName\"]) : unique must be true or false", null ) );
        //                }
        //            }
        //        }
        //        else
        //            ctx.Messages.Add( new LogMessage( ErrorLevel.Error, this.Span, "Unknow attribute name", null ) );
        //    }
        //}
    }
}
