using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hyperstore.CodeAnalysis.Editor.Completion
{
    /// <summary> 
    /// Initializes the completion controller when a text view is created 
    /// </summary> 
    [Export(typeof(IVsTextViewCreationListener))]
    [ContentType(ContentTypeAndFileExtensionDefinition.ContentTypeName)]
    [TextViewRole(PredefinedTextViewRoles.Editable)]
    internal class CompletionCommandFilterProvider : IVsTextViewCreationListener
    {
        [Import]
        IVsEditorAdaptersFactoryService EditorAdaptersFactory { get; set; }

        void IVsTextViewCreationListener.VsTextViewCreated(Microsoft.VisualStudio.TextManager.Interop.IVsTextView textViewAdapter)
        {
            // Get wpf text view of the adapter  
            var view = this.EditorAdaptersFactory.GetWpfTextView(textViewAdapter);

            // Initialize the completion controller 
            var completionController = view.Properties.GetProperty<CompletionController>(typeof(CompletionController));
            if (completionController != null)
                completionController.Initialize(textViewAdapter);
        }
    } 
}
