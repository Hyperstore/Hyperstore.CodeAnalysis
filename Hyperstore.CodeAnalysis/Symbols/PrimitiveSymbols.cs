using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hyperstore.CodeAnalysis.Symbols
{
    internal sealed class PrimitiveEntitySymbol : EntitySymbol, IPrimitiveSymbol
    {
        public PrimitiveEntitySymbol(Symbol domain)
            : base(null, domain, null)
        {
        }

        public override string Name
        {
            get
            {
                return "ModelEntity";
            }
            protected set
            {
            }
        }
    }

    internal sealed class PrimitiveRelationshipSymbol : RelationshipSymbol, IPrimitiveSymbol
    {
        public PrimitiveRelationshipSymbol(Symbol domain)
            : base(null, domain, null)
        {
        }

        public override string Name
        {
            get
            {
                return "ModelRelationship";
            }
            protected set
            {
            }
        }
    }
}
