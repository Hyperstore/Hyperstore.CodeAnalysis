using Microsoft.VisualStudio.Text;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Hyperstore.CodeAnalysis.Editor.Parser
{
    enum TokenKind
    {
        Keyword,
        Attribute,
        Comment,
        CSharpCode,
        Normal,
        Separator,
        EOF,
        String
    }
}
