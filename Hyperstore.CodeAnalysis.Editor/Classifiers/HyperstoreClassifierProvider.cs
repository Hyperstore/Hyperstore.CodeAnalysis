using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hyperstore.CodeAnalysis.Editor.Classifiers
{
    [Export(typeof(IClassifierProvider))]
    [ContentType(ContentTypeAndFileExtensionDefinition.ContentTypeName)]
    internal sealed class MyClassifierProvider : IClassifierProvider
    {
        [Import]
        internal IClassificationTypeRegistryService classificationTypeRegistry
        { get; set; }

        [Import]
        internal ITextDocumentFactoryService TextDocumentFactoryService = null;

        public IClassifier GetClassifier(ITextBuffer buffer)
        {
            return buffer.Properties.GetOrCreateSingletonProperty(() => new HyperstoreClassifier(buffer, classificationTypeRegistry, TextDocumentFactoryService));
        }
    }
}
