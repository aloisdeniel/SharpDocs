using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpDocs.Documentation
{
    public class Type : TypeParameter
    {
        public IEnumerable<TypeParameter> Parents { get; set; }

        public IEnumerable<Method> Methods { get; set; }

        public IEnumerable<Event> Events { get; set; }

        public IEnumerable<Property> Properties { get; set; }
    }
}
