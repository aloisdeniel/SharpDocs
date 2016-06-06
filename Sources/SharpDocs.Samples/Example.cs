using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpDocs.Samples
{
    /// <summary>
    /// An example type.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <typeparam name="TExample"></typeparam>
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

        /// <summary>
        /// Do things asynchronously.
        /// </summary>
        /// <param name="ms">The number of milliseconds</param>
        /// <returns>A task with int</returns>
        public async Task<int> DoThingsAsync(int ms)
        {
            await Task.Delay(ms);
            return 8;
        }

        /// <summary>
        /// Example of a function with no parameter.
        /// </summary>
        public void NoParam()
        {

        }

        /// <summary>
        /// Example of a generic function.
        /// </summary>
        /// <typeparam name="T">Type parameter</typeparam>
        public void TestGeneric<T>()
        {

        }
    }
}
