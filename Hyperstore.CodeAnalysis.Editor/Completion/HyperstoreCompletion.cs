using Microsoft.VisualStudio.Language.Intellisense;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hyperstore.CodeAnalysis.Editor.Completion
{
    internal class HyperstoreCompletion : Microsoft.VisualStudio.Language.Intellisense.Completion, IComparable
    {
        internal HyperstoreCompletion(Declaration declaration, IGlyphService glyphService)
            : base(declaration.Title)
        {
            this.InsertionText = declaration.InsertionText ?? declaration.Title;
            this.Description = declaration.Description;
            this.IconSource = glyphService.GetGlyph(GetGroupFromDeclaration(declaration), GetScopeFromDeclaration(declaration));
        }

        private StandardGlyphItem GetScopeFromDeclaration(Declaration declaration)
        {
            return StandardGlyphItem.GlyphItemPublic;
        }


        private StandardGlyphGroup GetGroupFromDeclaration(Declaration declaration)
        {
            switch (declaration.Type)
            {
                case DeclarationType.Type:
                    return StandardGlyphGroup.GlyphGroupClass;
                case DeclarationType.Keyword:
                    return StandardGlyphGroup.GlyphKeyword;
                case DeclarationType.Primitive:
                    return StandardGlyphGroup.GlyphGroupValueType;
                default:
                    return StandardGlyphGroup.GlyphGroupClass;
            }
        }

        public int CompareTo(object other)
        {
            var otherCompletion = other as HyperstoreCompletion;
            return this.DisplayText.CompareTo(otherCompletion.DisplayText);
        }
    }
}