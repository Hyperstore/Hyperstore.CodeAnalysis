// Copyright (c) Alain Metge.  All rights reserved.
// This file is part of Hyperstore.
// 
//  Licensed under the Apache License, Version 2.0 (the "License"); you
//  may not use this file except in compliance with the License. You may
// obtain a copy of the License at
// 
// http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or
// implied. See the License for the specific language governing permissions
// and limitations under the License.long with this program.  If not, see <http://www.gnu.org/licenses/>.
// 

#region Imports

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TextManager.Interop;
using Hyperstore.CodeAnalysis.Editor.Parser;
#endregion

namespace Hyperstore.CodeAnalysis.Editor
{
    internal class ErrorListWindow
    {
        private readonly Guid ProviderGuid = new Guid("{8F29FF6B-8DDA-4B4F-9E73-09EB75FA216C}");

        #region Fields

        private readonly string _languageName;
        private readonly IServiceProvider _serviceProvider;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="ErrorListWindow" /> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        public ErrorListWindow(IServiceProvider serviceProvider, string languageName)
        {
            this._serviceProvider = serviceProvider;
            this._languageName = languageName;
        }

        #endregion

        #region Properties

        private ErrorListProvider errorListProvider;

        /// <summary>
        ///     Gets the error list provider.
        /// </summary>
        /// <value>The error list provider.</value>
        protected ErrorListProvider ErrorListProvider
        {
            get
            {
                if (errorListProvider == null)
                {
                    errorListProvider = new ErrorListProvider(this._serviceProvider);
                    errorListProvider.ProviderName = _languageName;
                    errorListProvider.ProviderGuid = ProviderGuid;
                }

                return errorListProvider;
            }
        }

        #endregion

        #region Public Implementation

        /// <summary>
        ///     Gets the errors count.
        /// </summary>
        /// <value>The errors count.</value>
        public int ErrorsCount
        {
            get { return ErrorListProvider.Tasks.Count; }
        }

        /// <summary>
        ///     Shows the errors.
        /// </summary>
        public void Show()
        {
            this.ErrorListProvider.Show();
        }

        /// <summary>
        ///     Writes the error.
        /// </summary>
        /// <param name="message">The message.</param>
        public void WriteErrors(IEnumerable<DiagnosticInfo> errors)
        {
            ClearErrors();

            foreach (var msg in errors)
            {
                var diag = msg.Diagnostic;

                var errorTask = new ErrorTask();
                errorTask.CanDelete = false;
                errorTask.Category = TaskCategory.All;
                errorTask.Priority = TaskPriority.Normal;
                errorTask.ErrorCategory = TaskErrorCategory.Error;
                errorTask.Text = diag.Message;
                errorTask.Line = diag.Location.SourceSpan.Line - 1;
                errorTask.Column = diag.Location.SourceSpan.Column - 1;
                errorTask.Document = diag.Location.SyntaxTree.SourceFilePath;
                errorTask.Navigate += NavigateDocument;
                this.ErrorListProvider.Tasks.Add(errorTask);
            }
        }

        private void NavigateDocument(object sender, EventArgs e)
        {
            var task = sender as ErrorTask;
            if (task == null)
                throw new ArgumentException("sender");
            //use the helper class to handle the navigation
            OpenDocumentAndNavigateTo(task.Document, task.Line, task.Column);
        }

        /// <summary>
        ///     Clears the errors.
        /// </summary>
        public void ClearErrors()
        {
            this.ErrorListProvider.SuspendRefresh();
            this.ErrorListProvider.Tasks.Clear();
            this.ErrorListProvider.ResumeRefresh();
        }

        #endregion

        public static void OpenDocumentAndNavigateTo(string path, int line, int column)
        {
            var openDoc = Package.GetGlobalService(typeof(IVsUIShellOpenDocument)) as IVsUIShellOpenDocument;
            if (openDoc == null)
                return;

            IVsWindowFrame frame;
            Microsoft.VisualStudio.OLE.Interop.IServiceProvider sp;
            IVsUIHierarchy hier;
            uint itemid;
            var logicalView = VSConstants.LOGVIEWID_Code;

            if (ErrorHandler.Failed(openDoc.OpenDocumentViaProject(path, ref logicalView, out sp, out hier, out itemid, out frame)) || frame == null)
                return;
            object docData;
            frame.GetProperty((int)__VSFPROPID.VSFPROPID_DocData, out docData);

            // Get the VsTextBuffer  
            var buffer = docData as VsTextBuffer;
            if (buffer == null)
            {
                var bufferProvider = docData as IVsTextBufferProvider;
                if (bufferProvider != null)
                {
                    IVsTextLines lines;
                    ErrorHandler.ThrowOnFailure(bufferProvider.GetTextBuffer(out lines));
                    buffer = lines as VsTextBuffer;
                    Debug.Assert(buffer != null, "IVsTextLines does not implement IVsTextBuffer");
                    if (buffer == null)
                        return;
                }
            }
            // Finally, perform the navigation.  
            var mgr = Package.GetGlobalService(typeof(VsTextManagerClass)) as IVsTextManager;
            if (mgr == null)
                return;
            mgr.NavigateToLineAndColumn(buffer, ref logicalView, line, column, line, column);
        }
    }
}