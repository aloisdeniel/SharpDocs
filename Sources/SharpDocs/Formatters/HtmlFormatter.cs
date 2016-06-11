using HandlebarsDotNet;
using SharpDocs.Documentation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpDocs.Formatters
{
    public class HtmlFormatter
    {
        private void Generate(TextWriter writer, Parsers.Entities.Content content)
        {
            foreach (var item in content)
            {
                if (item is Parsers.Entities.Content.Text)
                {
                    writer.WriteSafeString($"{(item as Parsers.Entities.Content.Text).Value}");
                }
                else if (item is Parsers.Entities.Content.See)
                {
                    writer.WriteSafeString($"<b>{(item as Parsers.Entities.Content.See).Reference}</b>");
                }
                else if (item is Parsers.Entities.Content.Code)
                {
                    writer.WriteSafeString($"<pre><code>{(item as Parsers.Entities.Content.Code).Value}</code></pre>");
                }
                else if (item is Parsers.Entities.Content.InlineCode)
                {
                    writer.WriteSafeString($"<code>{(item as Parsers.Entities.Content.Code).Value}</code>");
                }
                else if (item is Parsers.Entities.Content.List)
                {
                    var list = item as Parsers.Entities.Content.List;

                    var listtype = "ul";
                    if (list.Type == "number")
                    {
                        listtype = "ol";
                    }

                    if (list.Header != null)
                    {
                        writer.WriteSafeString($"<h3>{list.Header.Value}</h3>\n");
                    }

                    if (list.Items != null)
                    {
                        writer.WriteSafeString($"<{listtype}>");

                        foreach (var listitem in list.Items)
                        {
                            writer.WriteSafeString($"<li>");
                            Generate(writer, listitem.Value);
                            writer.WriteSafeString($"</li>");
                        }

                        writer.WriteSafeString($"</{listtype}>");
                    }
                }
            }
        }
        
        private string LoadTemplate(string name)
        {
            var assembly = System.Reflection.Assembly.GetExecutingAssembly();
            var resourceName = $"SharpDocs.Templates.{name}";

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }

  
        public string Render(Assembly tree, string template = "Default.html")
        {

            Handlebars.RegisterHelper("msdoc", (writer, context, parameters) => {
                var member = context as Parsers.Entities.Member;

                if (member != null)
                {
                    if (member.TypeParameters.Any())
                    {
                        writer.WriteSafeString("<ul class='typeparams'>");
                        foreach (var p in member.TypeParameters)
                        {
                            writer.WriteSafeString($"<li><b>{p.Name}</b>");
                            Generate(writer, p.Summary);
                            writer.WriteSafeString("</li>");
                        }
                        writer.WriteSafeString("</ul>");
                    }

                    if (member.Parameters.Any())
                    {
                        writer.WriteSafeString("<ul class='args'>");
                        foreach (var p in member.Parameters)
                        {
                            writer.WriteSafeString($"<li><b>{p.Name}</b>");
                            Generate(writer, p.Summary);
                            writer.WriteSafeString("</li>");
                        }
                        writer.WriteSafeString("</ul>");
                    }

                    Generate(writer, member.Summary);
                }

            });

            var render = Handlebars.Compile(LoadTemplate(template));
            return render(tree);
        }
    }
}
