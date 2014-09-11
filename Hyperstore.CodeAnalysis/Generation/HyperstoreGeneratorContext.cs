using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hyperstore.CodeAnalysis.Generation
{
    public enum GenerationScope
    {
        MetadataDefinitionBegin = 0,
        MetadataDefinitionBody,
        MetadataDefinitionEnd,
        NewScope,
        Begin,
        MetadataOnBeforeLoad
    }

    public class HyperstoreGeneratorContext
    {
        class GenerationContext : IDisposable
        {
            HyperstoreGeneratorContext _ctx;

            public GenerationContext(HyperstoreGeneratorContext ctx)
            {
                _ctx = ctx;
            }

            public void Dispose()
            {
                _ctx.Pop();
            }
        }

        private readonly StringBuilder _global;
        private TextWriter[] _writers;
        private Stack<TextWriter> _current;

        public HyperstoreGeneratorContext()
        {
            _writers = new TextWriter[6];
            _current = new Stack<TextWriter>();
            _global = new StringBuilder();
        }

        internal void Write(string msg, params object[] args)
        {
            _current.Peek().Write(msg, args);
        }

        internal void Write(int indent, string msg, params object[] args)
        {
            WriteIndent(indent);
            _current.Peek().Write(msg, args);
        }

        private void WriteIndent(int x)
        {
            if (x > 0)
                _current.Peek().Write(new string(' ', x * 4));
        }

        internal void WriteLine(int indent = 0, string msg = null, params object[] args)
        {
            if (msg == null)
            {
                _current.Peek().WriteLine();
                return;
            }

            WriteIndent(indent);
            _current.Peek().WriteLine(msg, args);
        }

        public IDisposable Push(GenerationScope scope)
        {
            var writer = _writers[(int)scope];
            if (writer == null)
            {
                writer = _writers[(int)scope] = new StringWriter(new StringBuilder());
            }
            _current.Push(writer);
            return new GenerationContext(this);
        }

        public void Pop()
        {
            _current.Pop();
        }

        public void NewDomain()
        {
            if (_writers != null)
            {
                _global.AppendLine(Flush());
            }
            _writers = new TextWriter[6];
            _current = new Stack<TextWriter>();
        }

        public override string ToString()
        {
            _global.AppendLine(Flush());
            return _global.ToString();
        }

        private string Flush()
        {
            StringBuilder sb = new StringBuilder();
            var writer = _writers[(int)GenerationScope.Begin];
            if (writer != null)
                sb.Append(writer);

            writer = _writers[(int)GenerationScope.MetadataDefinitionBegin];
            if (writer != null) sb.Append(writer);

            writer = _writers[(int)GenerationScope.MetadataDefinitionBody];
            if (writer != null)
                sb.Append(writer);

            writer = _writers[(int)GenerationScope.MetadataDefinitionEnd];
            if (writer != null)
                sb.Append(writer);

            writer = _writers[(int)GenerationScope.MetadataOnBeforeLoad];
            if (writer != null)
                sb.Append(writer);

            writer = _writers[(int)GenerationScope.NewScope];
            if (writer != null)
                sb.Append(writer);

            return sb.ToString();
        }
    }
}
