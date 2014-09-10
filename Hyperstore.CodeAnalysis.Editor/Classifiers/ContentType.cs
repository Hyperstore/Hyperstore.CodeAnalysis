using Microsoft.VisualStudio.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hyperstore.CodeAnalysis.Editor
{
    internal sealed class ContentTypeAndFileExtensionDefinition
    {
        internal const string ContentTypeName = "Hyperstore";
        internal const string FileExtension = ".domain";

        [Export]
        [Name(ContentTypeName)]
        [BaseDefinition("code")]
        internal static ContentTypeDefinition TypeDefinition = null;

        [Export]
        [FileExtension(FileExtension)]
        [ContentType(ContentTypeName)]
        internal static FileExtensionToContentTypeDefinition FileExtensionDefinition = null;
    }
}
