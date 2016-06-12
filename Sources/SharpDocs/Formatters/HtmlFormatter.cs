namespace SharpDocs.Formatters
{
    using HandlebarsDotNet;
    using Documentation;
    using System.IO;
    using System.Linq;

    public class HtmlFormatter : IFormatter
    {
        private readonly string template;

        public HtmlFormatter(string template = "Default.html")
        {
            this.template = template;
        }

        public string Extension
        {
            get { return ".html"; }
        }

        private void Generate(TextWriter writer, Parsers.Entities.Content content)
        {
            if (content != null)
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
                        writer.WriteSafeString($"</p><pre><code>{(item as Parsers.Entities.Content.Code).Value}</code></pre><p>");
                    }
                    else if (item is Parsers.Entities.Content.Paragraph)
                    {
                        writer.WriteSafeString($"</p><p>");
                        Generate(writer, (item as Parsers.Entities.Content.Paragraph).Content);
                        writer.WriteSafeString($"</p><p>");
                    }
                    else if (item is Parsers.Entities.Content.InlineCode)
                    {
                        writer.WriteSafeString($"<code>{(item as Parsers.Entities.Content.InlineCode).Value}</code>");
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
                            writer.WriteSafeString($"</p><{listtype}>");

                            foreach (var listitem in list.Items)
                            {
                                writer.WriteSafeString($"<li>");
                                Generate(writer, listitem.Value);
                                writer.WriteSafeString($"</li>");
                            }

                            writer.WriteSafeString($"</p></{listtype}>");
                        }
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
        
        public string Render(Assembly tree)
        {
            Handlebars.RegisterHelper("msdoc", (writer, context, parameters) =>
            {
                var member = context as Parsers.Entities.Member;

                if (member != null)
                {
                    if (member.Summary != null)
                    {
                        writer.WriteSafeString("<p>");
                        Generate(writer, member.Summary);
                        writer.WriteSafeString("</p>");
                    }

                    if (member.Parameters.Any())
                    {
                        writer.WriteSafeString("<h5>Parameters</h5>");
                        writer.WriteSafeString("<ul class='args'>");
                        foreach (var p in member.Parameters)
                        {
                            writer.WriteSafeString($"<li><b>{p.Name}</b>");
                            Generate(writer, p.Summary);
                            writer.WriteSafeString("</li>");
                        }
                        writer.WriteSafeString("</ul>");
                    }

                    if (member.TypeParameters.Any())
                    {
                        writer.WriteSafeString("<h5>Type parameters</h5>");
                        writer.WriteSafeString("<ul class='typeparams'>");
                        foreach (var p in member.TypeParameters)
                        {
                            writer.WriteSafeString($"<li><b>{p.Name}</b>");
                            Generate(writer, p.Summary);
                            writer.WriteSafeString("</li>");
                        }
                        writer.WriteSafeString("</ul>");
                    }

                    if (member.Returns != null)
                    {
                        writer.WriteSafeString("<h5>Returns</h5>");
                        writer.WriteSafeString("<p>");
                        Generate(writer, member.Returns);
                        writer.WriteSafeString("</p>");
                    }

                    if (member.Remarks != null)
                    {
                        writer.WriteSafeString("<h5>Remarks</h5>");
                        writer.WriteSafeString("<p>");
                        Generate(writer, member.Returns);
                        writer.WriteSafeString("</p>");
                    }

                    if (member.Example != null)
                    {
                        writer.WriteSafeString("<h5>Examples</h5>");
                        writer.WriteSafeString("<p>");
                        Generate(writer, member.Example);
                        writer.WriteSafeString("</p>");
                    }
                }

            });

            var render = Handlebars.Compile(LoadTemplate(template));
            return render(tree);
        }
    }
}
