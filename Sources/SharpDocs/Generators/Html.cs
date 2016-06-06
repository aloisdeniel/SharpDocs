using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SharpDocs.Generators
{
    public class Html : IGenerator
    {
        public Html()
        {
            this.markdown = new Markdown();
        }

        private readonly Markdown markdown;
        
        public string Extension
        {
            get { return ".html"; }
        }

        public string Generate(Assembly assembly)
        {
            var md = this.markdown.Generate(assembly);
            var result = CommonMark.CommonMarkConverter.Convert(md);
            return result;
        }
    }
}
