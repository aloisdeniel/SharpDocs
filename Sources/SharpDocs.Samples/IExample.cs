using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpDocs.Samples
{
    public interface IExample
    {
        /// <summary>
        /// Name of the example.
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Index of the example.
        /// </summary>
        int Index { get; set; }
    }
}
