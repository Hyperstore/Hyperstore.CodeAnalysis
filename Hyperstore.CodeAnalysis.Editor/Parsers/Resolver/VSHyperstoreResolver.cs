using Hyperstore.CodeAnalysis.Compilation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hyperstore.CodeAnalysis.Editor.Resolver
{
    class VSHyperstoreResolver : FileModelResolver, IDisposable
    {
        public VSHyperstoreResolver()
        {
            Messenger.Default.Register(this);
        }

        public override SemanticModel ResolveSemanticModel(string normalizedPath)
        {
            var model = FindModel(normalizedPath);
            if (model != null)
                return model;

            return base.ResolveSemanticModel(normalizedPath);
        }

        private SemanticModel FindModel(string path)
        {
            var msg = new VSHyperstoreResolverMessage { FilePath = path };
            Messenger.Default.Send(msg);
            return msg.Model;
        }

        public void OnMessageReceived(VSHyperstoreResolverMessage msg)
        {
            if (msg.Model == null)
                msg.Model = Compilation.GetSemanticModel(msg.FilePath);
        }

        public void Dispose()
        {
            Messenger.Default.Unregister(this);
        }
    }

}
