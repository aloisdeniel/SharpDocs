using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpDocs.Parsers.Entities
{
    public class Member
    {
        public string Name { get; set; }

        public Content Summary { get; set; }

        public IEnumerable<Parameter> Parameters { get; set; }

        public IEnumerable<Parameter> TypeParameters { get; set; }

        public IEnumerable<Exception> Exceptions { get; internal set; }

        public Content Returns { get; set; }

        public Content Example { get; internal set; }

        public Content Remarks { get; internal set; }
        public IEnumerable<SeeAlso> SeeAlso { get; internal set; }
    }
}
