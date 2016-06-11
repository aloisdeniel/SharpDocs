using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpDocs.Parsers.Entities
{
    public class Documentation
    {
        public Member Assembly { get; set; }

        public IEnumerable<Member> Members { get; set; }
    }
}
