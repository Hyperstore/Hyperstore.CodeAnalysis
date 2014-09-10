using System;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Text.Editor;
using System.Windows.Input;
using System.Collections.Generic;
using Microsoft.VisualStudio.Text;
using System.Windows.Media;
using System.Windows.Controls;

namespace Hyperstore.CodeAnalysis.Editor.AutoComplete
{
    class AutoCompleteCommandFilter : IOleCommandTarget
    {
        private IWpfTextView m_textView;
        internal IOleCommandTarget m_nextTarget;
        private IAdornmentLayer m_adornmentLayer;
        private bool requiresHandling = false;
        internal bool m_added;

        public AutoCompleteCommandFilter(IWpfTextView textView)
        {
            m_textView = textView;
            m_adornmentLayer = m_textView.GetAdornmentLayer("HyperstoreAutoCompleteLayer");
        }

        int IOleCommandTarget.QueryStatus(ref Guid pguidCmdGroup, uint cCmds, OLECMD[] prgCmds, IntPtr pCmdText)
        {
            return m_nextTarget.QueryStatus(ref pguidCmdGroup, cCmds, prgCmds, pCmdText);
        }

        int IOleCommandTarget.Exec(ref Guid pguidCmdGroup, uint nCmdID, uint nCmdexecopt, IntPtr pvaIn, IntPtr pvaOut)
        {
            var result = m_nextTarget.Exec(ref pguidCmdGroup, nCmdID, nCmdexecopt, pvaIn, pvaOut);

            if (result == VSConstants.S_OK)
            {
                char typedChar = char.MinValue;

                if (pguidCmdGroup == VSConstants.VSStd2K && nCmdID == (uint)VSConstants.VSStd2KCmdID.TYPECHAR)
                {
                    typedChar = (char)(ushort)Marshal.GetObjectForNativeVariant(pvaIn);
                    string textToInsert = null;

                    if (typedChar.Equals('{'))
                        textToInsert = "\r\n\t}";
                    if (typedChar.Equals('"'))
                        textToInsert = "\"";

                    if (textToInsert != null)
                    {
                        CaretPosition curPosition = m_textView.Caret.Position;

                        var curTrackPoint = m_textView.TextSnapshot.CreateTrackingPoint(curPosition.BufferPosition.Position, Microsoft.VisualStudio.Text.PointTrackingMode.Negative);

                        ITextEdit edit = m_textView.TextBuffer.CreateEdit();

                        edit.Insert(curTrackPoint.GetPosition(m_textView.TextSnapshot), textToInsert);
                        edit.Apply();
                        edit.Dispose();
                        requiresHandling = true;
                        m_textView.Caret.MoveToPreviousCaretPosition();
                    }
                }
            }

            return result;
        }
    }
}
