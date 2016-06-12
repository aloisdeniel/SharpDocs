using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpDocs.Documentation
{
    public class Assembly : Node
    {
        public IEnumerable<Type> Classes { get; set; }

        public IEnumerable<Type> Interfaces { get; set; }

        public IEnumerable<Enum> Enums { get; set; }

        public IEnumerable<Type> Structs { get; set; }

        public IEnumerable<Type> Attributes { get; set; }

        public IEnumerable<Type> AllTypes
        {
            get
            {
                var types = Interfaces.ToList();
                types.AddRange(AbstractClasses);
                types.AddRange(Classes);
                types.AddRange(StaticClasses);
                types.AddRange(Attributes);
                types.AddRange(Structs);
                return types;
            }
        }

        public List<Type> AbstractClasses { get; internal set; }
        public List<Type> StaticClasses { get; internal set; }
    }
}
