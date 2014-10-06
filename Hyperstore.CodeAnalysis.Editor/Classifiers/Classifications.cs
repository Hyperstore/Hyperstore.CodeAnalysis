using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Hyperstore.CodeAnalysis.Editor.Classifiers
{
    internal sealed class ClassificationDefinitions
    {
        internal const string CSharpCode = "Hyperstore.CSharpCode";
        internal const string Attribute = "Hyperstore.Attribute";

        [Export]
        [Name(ClassificationDefinitions.CSharpCode)]
        internal static ClassificationTypeDefinition HyperstoreCSharpCodeDefinition;

        [Export]
        [Name(ClassificationDefinitions.Attribute)]
        internal static ClassificationTypeDefinition HyperstoreAttributeDefinition;

    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = ClassificationDefinitions.CSharpCode)]
    [Name(ClassificationDefinitions.CSharpCode)]
    [UserVisible(true)]
    [Order(After = Priority.Default, Before = Priority.High)]
    internal sealed class CSharpFormat : ClassificationFormatDefinition
    {

        public CSharpFormat()
        {
            this.DisplayName = ClassificationDefinitions.CSharpCode;
            this.IsItalic = true;
            this.ForegroundColor = Colors.Teal;
        }
    }


    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = ClassificationDefinitions.Attribute)]
    [Name(ClassificationDefinitions.Attribute)]
    [UserVisible(true)]
    [Order(After = Priority.Default, Before = Priority.High)]
    internal sealed class AttributeFormat : ClassificationFormatDefinition
    {

        public AttributeFormat()
        {
            this.DisplayName = ClassificationDefinitions.Attribute;
            this.ForegroundColor = Colors.Cyan;
        }
    }
}
