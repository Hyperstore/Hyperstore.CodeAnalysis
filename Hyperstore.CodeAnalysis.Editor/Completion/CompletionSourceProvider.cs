using System;
using System.Collections.Generic;
using System; 
using System.Collections.Generic; 
using System.Linq; 
using System.Text; 
using System.ComponentModel.Composition; 
using Microsoft.VisualStudio.Language.Intellisense; 
using Microsoft.VisualStudio.Text; 
using Microsoft.VisualStudio.Text.Classification; 
using Microsoft.VisualStudio.Utilities; 
using Microsoft.VisualStudio.OLE.Interop; 
using Microsoft.VisualStudio.Shell; 

namespace Hyperstore.CodeAnalysis.Editor.Completion
{
    [Export(typeof(ICompletionSourceProvider))]
    [ContentType(ContentTypeAndFileExtensionDefinition.ContentTypeName)] 
    [Name("Hyperstore Completion Source Provider")] 
    [Order(Before = "default")] 
    internal class CompletionSourceProvider : ICompletionSourceProvider 
    { 
        [Import] 
        private IGlyphService GlyphService { get; set; } 
 
        public ICompletionSource TryCreateCompletionSource(ITextBuffer textBuffer) 
        { 
            // Create the completion source 
            return new CompletionSource(textBuffer, GlyphService); 
        } 
    } 
}