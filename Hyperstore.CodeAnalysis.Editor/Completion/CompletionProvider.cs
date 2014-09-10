using System;
using System.Collections.Generic;
using System; 
using System.Collections.Generic; 
using System.Linq; 
using System.Text; 
using System.ComponentModel; 
using System.ComponentModel.Composition; 
using Microsoft.VisualStudio.Language.Intellisense; 
using Microsoft.VisualStudio.Text; 
using Microsoft.VisualStudio.Text.Classification; 
using Microsoft.VisualStudio.Utilities; 
using Microsoft.VisualStudio.Text.Editor; 
using Microsoft.VisualStudio.Editor; 
using Microsoft.VisualStudio.Shell; 

namespace Hyperstore.CodeAnalysis.Editor.Completion
{
    [Export(typeof(IIntellisenseControllerProvider))] 
    [ContentType(ContentTypeAndFileExtensionDefinition.ContentTypeName)] 
    [Name("Hyperstore Completion Controller")] 
    [Order(Before = "Default Completion Controller")] 
    [TextViewRole(PredefinedTextViewRoles.Editable)] 
    internal class CompletionControllerProvider : IIntellisenseControllerProvider 
    { 
        [Import] 
        private ICompletionBroker CompletionBrokerMapService { get; set; } 

 
        public IIntellisenseController TryCreateIntellisenseController(ITextView textView, IList<ITextBuffer> subjectBuffers) 
        { 
            // Create the completion controller and add it to the view properties 
            var completionController = new CompletionController(subjectBuffers, textView, this.CompletionBrokerMapService); 
 
            textView.Properties.AddProperty(completionController.GetType(), completionController); 
 
            return completionController; 
        } 
    } 
}

