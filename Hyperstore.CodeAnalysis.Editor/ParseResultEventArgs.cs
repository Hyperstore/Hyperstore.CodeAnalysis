using Microsoft.VisualStudio.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hyperstore.CodeAnalysis.Editor
{
    public class ParseResultEventArgs : EventArgs
    {
        public ParseResultEventArgs(ITextSnapshot snapshot)
        {
            this.Snapshot = snapshot;
        }

        public ParseResultEventArgs(ITextSnapshot snapshot, TimeSpan elapsedTime)
        {
            this.Snapshot = snapshot;
            this.ElapsedTime = elapsedTime;
        }

        public ITextSnapshot Snapshot
        {
            get;
            private set;
        }
        public TimeSpan? ElapsedTime
        {
            get;
            private set;
        }
    }
}
