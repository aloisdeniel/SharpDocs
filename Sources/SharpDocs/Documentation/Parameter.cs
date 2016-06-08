using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpDocs.Documentation
{
    public class Parameter : TypedNode
    {
        public bool IsOptional { get; internal set; }
        public bool IsOut { get; set; }
    }
}
