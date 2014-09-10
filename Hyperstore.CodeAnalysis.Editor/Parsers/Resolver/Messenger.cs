using Hyperstore.CodeAnalysis.Compilation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hyperstore.CodeAnalysis.Editor.Resolver
{
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
    
                if (toRemove != null)
                {
                    _receivers.Remove(toRemove);
                }
            }
        }
    
    
        public void Register(VSHyperstoreResolver receiver)
        {
            lock (_receivers)
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
