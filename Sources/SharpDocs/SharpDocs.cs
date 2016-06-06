using SharpDocs.Generators;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SharpDocs
{
    public static class SharpDocs
    {
        public static string Generate<TGenerator>(string dllFile, string template = "{{{{DOC}}}}") where TGenerator : IGenerator
        {

            var assembly = Assembly.LoadFrom(dllFile);

            var generator = Activator.CreateInstance<TGenerator>();

            var xmlDocFile = dllFile.Replace(".dll", ".xml");
            var outputFile = dllFile.Replace(".dll", generator.Extension);
  
            var content = generator.Generate(assembly);
            content = template.Replace("{{{{DOC}}}}", content);

            File.WriteAllText(outputFile, content);

            return content;
        }
    }
}
