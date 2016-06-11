using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpDocs.Documentation
{
    public class Node
    {
        public static int lastId = 0;

        public Node()
        {
            this.Id = $"{lastId++}";
        }
            
        public string Id { get; protected set; }

        public string Name { get; set; }

        public Parsers.Entities.Member Description { get; set; }
    }
}
