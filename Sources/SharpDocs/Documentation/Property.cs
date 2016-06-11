using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpDocs.Documentation
{
    public class Property : TypedNode
    {
        public bool IsVirtual { get; set; }

        public bool IsAbstract { get; set; }

        public bool CanRead { get; set; }

        public bool CanWrite { get; set; }
    }
}
