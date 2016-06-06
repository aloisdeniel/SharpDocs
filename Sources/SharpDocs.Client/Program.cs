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
            var md = new SharpDocs().Generate($"{folder}\\{name}.dll", @"<html><body><h1>{{ name }}</h1></body></body>",".html");
        }
    }
}
