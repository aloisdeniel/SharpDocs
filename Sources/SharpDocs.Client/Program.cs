using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpDocs.Client
{
    class Program
    {
        static int Main(string[] args)
        {
            if (args.Length == 0)
            {
                System.Console.WriteLine("An assembly name should be precised.");
                return 1;
            }

            //const string assembly = @"..\..\..\SharpDocs.Samples\bin\Debug\SharpDocs.Samples.dll";
            var assembly = args[0].Trim('"').Trim('\'');

            Console.WriteLine($"Generating documentation for {assembly}");

            var docs = new SharpDocs();
            docs.Generate(assembly);

            return 0;
        }
    }
}
