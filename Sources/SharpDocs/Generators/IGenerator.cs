using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SharpDocs.Generators
{
    public interface IGenerator
    {
        string Extension { get; }
        
        string Generate(Assembly assembly);
    }
}
