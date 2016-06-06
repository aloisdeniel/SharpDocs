using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpDocs.Documentation
{
    public class Assembly : Node
    {
        public IEnumerable<Type> Types { get; set; }
    }
}
