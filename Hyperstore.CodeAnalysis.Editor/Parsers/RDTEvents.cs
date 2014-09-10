using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Text;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using IServiceProvider = Microsoft.VisualStudio.OLE.Interop.IServiceProvider;

namespace Hyperstore.CodeAnalysis.Editor.Parser
{
    internal class RDTEvents : IVsRunningDocTableEvents, IDisposable
    {
        public ErrorListWindow ErrorsWindow { get; private set; }
        private bool _disposed;
        private uint _docCookie;
        private string _filePath;

        protected RDTEvents(ITextBuffer buffer)
        {
            ErrorsWindow = new ErrorListWindow(new ServiceProvider(GlobalServiceProvider), "Hyperstore");
            var doc = buffer.Properties.GetProperty<ITextDocument>(typeof(ITextDocument));
            _filePath = doc.FilePath;
            InitializeRunningDocumentTable();
        }

        #region RDT events

        int IVsRunningDocTableEvents.OnAfterAttributeChange(uint docCookie, uint grfAttribs)
        {
            if (grfAttribs == 4 && docCookie == _docCookie)
            {
                var oldFilePath = _filePath;
                _filePath = GetFileNameFromCookie(docCookie);
                OnFileRenamed(oldFilePath, _filePath);
            }
            return VSConstants.S_OK;
        }

        protected virtual void OnFileRenamed(string oldFilePath, string newFilePath)
        {
        }

        int IVsRunningDocTableEvents.OnAfterDocumentWindowHide(uint docCookie, IVsWindowFrame pFrame)
        {
            return VSConstants.S_OK;
        }

        int IVsRunningDocTableEvents.OnAfterFirstDocumentLock(uint docCookie, uint dwRDTLockType, uint dwReadLocksRemaining, uint dwEditLocksRemaining)
        {
            return VSConstants.S_OK;
        }

        private string GetFileNameFromCookie(uint docCookie)
        {
            var rdt = GetService<IVsRunningDocumentTable, SVsRunningDocumentTable>(GlobalServiceProvider);
            // Retrieve document info
            uint pgrfRDTFlags;
            uint pdwReadLocks;
            uint pdwEditLocks;
            string pbstrMkDocument;
            IVsHierarchy ppHier;
            uint pitemid;
            IntPtr ppunkDocData;
            rdt.GetDocumentInfo(docCookie, out pgrfRDTFlags, out pdwReadLocks, out pdwEditLocks,
            out pbstrMkDocument, out ppHier, out pitemid, out ppunkDocData);
            return pbstrMkDocument;
        }

        int IVsRunningDocTableEvents.OnAfterSave(uint docCookie)
        {
            return VSConstants.S_OK;
        }

        int IVsRunningDocTableEvents.OnBeforeDocumentWindowShow(uint docCookie, int fFirstShow, IVsWindowFrame pFrame)
        {
            if (String.Compare(_filePath, GetFileNameFromCookie(docCookie), StringComparison.OrdinalIgnoreCase) == 0)
            {
                if (fFirstShow == 0)
                    OnEditorShow();
                _docCookie = docCookie;
            }
            return VSConstants.S_OK;
        }

        protected virtual void OnEditorShow()
        {
        }

        int IVsRunningDocTableEvents.OnBeforeLastDocumentUnlock(uint docCookie, uint dwRDTLockType, uint dwReadLocksRemaining, uint dwEditLocksRemaining)
        {

            if (dwReadLocksRemaining == 0 && dwEditLocksRemaining == 0)
            {
                ErrorsWindow.ClearErrors();
                if (String.Compare(_filePath, GetFileNameFromCookie(docCookie), StringComparison.OrdinalIgnoreCase) == 0)
                    OnEditorClosed();
            }
            return VSConstants.S_OK;
        }

        protected virtual void OnEditorClosed()
        {
        }

        #endregion

        #region dispose

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
            }

            if (this._runningDocumentTableCookie != 0)
            {
                this._runningDocumentTable.UnadviseRunningDocTableEvents(this._runningDocumentTableCookie);
                this._runningDocumentTableCookie = 0;
            }

            _disposed = true;
        }

        #endregion

        #region Services

        private static IServiceProvider globalServiceProvider;
        private static IServiceProvider GlobalServiceProvider
        {
            get
            {
                if (globalServiceProvider == null)
                    globalServiceProvider = (IServiceProvider)Package.GetGlobalService(typeof(IServiceProvider));

                return globalServiceProvider;
            }
        }

        private static TServiceInterface GetService<TServiceInterface, TService>(IServiceProvider serviceProvider)
            where TServiceInterface : class
            where TService : class
        {
            return (TServiceInterface)GetService(serviceProvider, typeof(TService).GUID, false);
        }

        private static object GetService(IServiceProvider serviceProvider, Guid guidService, bool unique)
        {
            var guidInterface = VSConstants.IID_IUnknown;
            var ptr = IntPtr.Zero;
            object service = null;

            if (serviceProvider.QueryService(ref guidService, ref guidInterface, out ptr) == 0 && ptr != IntPtr.Zero)
            {
                try
                {
                    if (unique)
                        service = Marshal.GetUniqueObjectForIUnknown(ptr);
                    else
                        service = Marshal.GetObjectForIUnknown(ptr);
                }
                finally
                {
                    Marshal.Release(ptr);
                }
            }

            return service;
        }

        #endregion

        #region Helpers - Initialize and Dispose IVsRunningDocumentTable

        private IVsRunningDocumentTable _runningDocumentTable;
        private uint _runningDocumentTableCookie;
        private IVsRunningDocumentTable RunningDocumentTable
        {
            get { return this._runningDocumentTable ?? (this._runningDocumentTable = GetService<IVsRunningDocumentTable, SVsRunningDocumentTable>(GlobalServiceProvider)); }
        }

        private void InitializeRunningDocumentTable()
        {
            if (RunningDocumentTable != null)
                RunningDocumentTable.AdviseRunningDocTableEvents(this, out this._runningDocumentTableCookie);
        }

        #endregion
    }
}
