using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.TextManager.Interop;
using Microsoft.VisualStudio.Utilities;
using System;
using System.ComponentModel.Composition;
using Hyperstore.CodeAnalysis.Editor.AutoComplete;

namespace Hyperstore.CodeAnalysis.Editor.AutoComplete
{
    [Export(typeof(IVsTextViewCreationListener))]
    [ContentType(Hyperstore.CodeAnalysis.Editor.ContentTypeAndFileExtensionDefinition.ContentTypeName)]
    [TextViewRole(PredefinedTextViewRoles.Editable)]
    internal class MultiEditFilterProvider : IVsTextViewCreationListener
    {
        [Export(typeof(AdornmentLayerDefinition))]
        [Name("HyperstoreAutoCompleteLayer")]
        [TextViewRole(PredefinedTextViewRoles.Editable)]
        internal AdornmentLayerDefinition m_multieditAdornmentLayer = null;

        [Import(typeof(IVsEditorAdaptersFactoryService))]
        internal IVsEditorAdaptersFactoryService editorFactory = null;

        public void VsTextViewCreated(IVsTextView textViewAdapter)
        {
            IWpfTextView textView = editorFactory.GetWpfTextView(textViewAdapter);
            if (textView == null)
                return;

            AddCommandFilter(textViewAdapter, new AutoCompleteCommandFilter(textView));
        }

        void AddCommandFilter(IVsTextView viewAdapter, AutoCompleteCommandFilter commandFilter)
        {
            if (commandFilter.m_added == false)
            {
                //get the view adapter from the editor factory
                IOleCommandTarget next;
                int hr = viewAdapter.AddCommandFilter(commandFilter, out next);

                if (hr == VSConstants.S_OK)
                {
                    commandFilter.m_added = true;
                    //you'll need the next target for Exec and QueryStatus
                    if (next != null)
                        commandFilter.m_nextTarget = next;
                }
            }
        }

    }

}
