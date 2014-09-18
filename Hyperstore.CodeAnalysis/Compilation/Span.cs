using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hyperstore.CodeAnalysis
{
    public sealed class HyperstoreSpan
    {
        public int Start { get; private set; }

        public int Length { get; private set; }

        public HyperstoreSpan(int start, int length)
        {
            Start = start;
            Length = length;
        }
    }
}
