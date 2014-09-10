using Hyperstore.CodeAnalysis.Compilation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hyperstore.CodeAnalysis.Editor.SyntaxHighlight
{
    class VSHyperstoreResolverMessage
    {
        public string FilePath;
        public SemanticModel Model;
    }

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
            if( msg.Model == null)
                msg.Model = Compilation.GetSemanticModel(msg.FilePath);
        }

        public void Dispose()
        {
            Messenger.Default.Unregister(this);
        }
    }

    internal class Messenger
    {
        private readonly List<WeakReference> _receivers = new List<WeakReference>();
        private static Messenger _default = new Messenger();

        public static Messenger Default { get { return _default; } }

        public void Unregister(VSHyperstoreResolver receiver)
        {
            lock (_receivers)
            {
                WeakReference toRemove = null;
                foreach (var reference in _receivers)
                {
                    var target = reference.Target as VSHyperstoreResolver;
                    if (target == null || target == receiver)
                    {
                        toRemove = reference;
                        break;
                    }
                }

                if( toRemove != null)
                {
                    _receivers.Remove(toRemove);
                }
            }
        }


        public void Register(VSHyperstoreResolver receiver)
        {
            lock(_receivers)
                _receivers.Add(new WeakReference(receiver));
        }

        public void Send(VSHyperstoreResolverMessage message)
        {
            lock (_receivers)
            {
                var toRemove = new List<WeakReference>();
                foreach (var reference in _receivers)
                {
                    var target = reference.Target as VSHyperstoreResolver;
                    if (target != null)
                    {
                        target.OnMessageReceived(message);
                    }
                    else
                    {
                        toRemove.Add(reference);
                    }
                }

                foreach (var dead in toRemove)
                {
                    _receivers.Remove(dead);
                }
            }
        }
    }
}
