using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Irony.Parsing
{
    public static class ParsingEnumExtensions
    {

        public static bool IsSet(this TermFlags flags, TermFlags flag)
        {
            return (flags & flag) != 0;
        }
        public static bool IsSet(this LanguageFlags flags, LanguageFlags flag)
        {
            return (flags & flag) != 0;
        }
        public static bool IsSet(this ParseOptions options, ParseOptions option)
        {
            return (options & option) != 0;
        }
        public static bool IsSet(this TermListOptions options, TermListOptions option)
        {
            return (options & option) != 0;
        }
        public static bool IsSet(this ProductionFlags flags, ProductionFlags flag)
        {
            return (flags & flag) != 0;
        }
    }//class


    public static class QueueExtensions
    {
        public static IEnumerable<T> DequeueAll<T>(this Queue<T> queue, Func<T,bool> filter=null)
        {
            List<T> list = null;
            while (queue != null && queue.Count > 0 && (filter == null || filter(queue.Peek())))
            {
                if (list == null) list = new List<T>();
                list.Add(queue.Dequeue());
            }
            return list;
        }
    }
}
