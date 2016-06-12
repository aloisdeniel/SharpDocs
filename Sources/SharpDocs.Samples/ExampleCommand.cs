using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SharpDocs.Samples
{
    public class ExampleCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        /// <summary>
        /// Indicates whether we can <c>Execute(null)</c> the command or note <see cref="ICommand"/>
        /// <para>
        /// Another paragraph with the same <see cref="ICommand"/> reference.
        /// </para>
        /// <list type="bullet">
        /// <item>
        /// <description>Item 1.</description>
        /// </item>
        /// <item>
        /// <description>Item 2.</description>
        /// </item>
        /// </list>
        /// <para>
        /// And a number list :
        /// </para>
        /// <list type="number">
        /// <item>
        /// <description>Item 1.</description>
        /// </item>
        /// <item>
        /// <description>Item 2.</description>
        /// </item>
        /// </list>
        /// <code>
        /// public bool CanExecute(object parameter)
        /// {
        ///    throw new NotImplementedException();
        /// }
        /// </code>
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public bool CanExecute(object parameter)
        {
            throw new NotImplementedException();
        }

        public void Execute(object parameter)
        {
            throw new NotImplementedException();
        }
    }
}
