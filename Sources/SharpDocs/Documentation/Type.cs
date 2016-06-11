using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpDocs.Documentation
{
    public class Type : TypeParameter
    {
        public string Category { get; set; }

        public bool IsAbstract { get; set; }

        public IEnumerable<TypeParameter> Generic { get; set; }

        public TypeParameter ParentClass { get; set; }

        public IEnumerable<TypeParameter> ParentInterfaces { get; set; }
        
        public IEnumerable<TypeParameter> Parents { get; set; }

        public IEnumerable<Method> Constructors { get; internal set; }

        public IEnumerable<Method> Methods { get; set; }

        public IEnumerable<Method> AsyncMethods { get; set; }

        public IEnumerable<Event> Events { get; set; }

        public IEnumerable<TypeParameter> Delegates { get; set; }

        public IEnumerable<Property> Properties { get; set; }

        public IEnumerable<Property> Commands { get; set; }
    }
}
