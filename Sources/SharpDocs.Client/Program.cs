using SharpDocs.Generators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpDocs.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            const string folder = @"..\..\..\SharpDocs.Samples\bin\Debug\";
            const string name = @"SharpDocs.Samples";
            var md = SharpDocs.Generate<Html>($"{folder}\\{name}.dll", @"<html><body>{{{{DOC}}}}</body></body>");
        }
    }
}
