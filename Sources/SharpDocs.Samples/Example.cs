using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpDocs.Samples
{
    public class Example<T, T2, TExample> : IExample where T : IExample
    {
        /// <summary>
        /// The main constructor.
        /// </summary>
        /// <param name="name">Name of the example.</param>
        public Example(string name)
        {
            this.Name = name;
        }

        public int Index { get; set; }

        public string Name { get; set; }

        /// <summary>
        /// An other property.
        /// </summary>
        public Example<IExample,int,string> Other { get; set; }

        /// <summary>
        /// When the object is created.
        /// </summary>
        public event EventHandler<EventArgs> Created;
    }
}
