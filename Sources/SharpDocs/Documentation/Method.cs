using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpDocs.Documentation
{
    public class Method : TypedNode
    {
        public bool IsStatic { get; set; }

        public bool IsVirtual { get; set; }

        public bool IsAbstract { get; set; }

        public IEnumerable<TypeParameter> Generic { get; set; }

        public IEnumerable<Parameter> Arguments { get; set; }

        public bool IsExtension { get; set; }
    }
}
