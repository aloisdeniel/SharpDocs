using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpDocs.Documentation
{
    public class Field : TypedNode
    {
        public bool IsStatic { get; set; }

        public bool IsReadOnly { get; set; }
    }
}
