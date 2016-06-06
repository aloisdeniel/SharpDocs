using HandlebarsDotNet;
using SharpDocs.Documentation;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SharpDocs
{
    public class SharpDocs
    {

        #region Tree

        private XElement FindDoc(XDocument xdoc, string fullname)
        {
            return xdoc.Descendants("member").Where((n) => n.Attribute("name")?.Value == fullname).FirstOrDefault();
        }

        private string FindSummary(XDocument xdoc, string fullname)
        {
            var doc = FindDoc(xdoc, fullname);
            return doc?.Descendants("summary").FirstOrDefault()?.Value.Trim();
        }
        private string FindParam(XDocument xdoc, string fullname, string paramName, string paramType)
        {
            var doc = FindDoc(xdoc, fullname);
            return doc?.Descendants("param").Where((n) => n.Attribute("name")?.Value == paramName).FirstOrDefault()?.Value.Trim();
        }

        private Assembly CreateNode(XDocument doc, System.Reflection.Assembly assembly)
        {
            var node = new Assembly()
            {
                Name = assembly.GetName().Name
            };

            var descriptionAttribute = assembly
                                        .GetCustomAttributes(typeof(System.Reflection.AssemblyDescriptionAttribute), false)
                                        .OfType<System.Reflection.AssemblyDescriptionAttribute>()
                                        .FirstOrDefault();

            node.Description = descriptionAttribute.Description;
            node.Types = assembly.ExportedTypes.Select((t) => CreateNode(doc, t));
            return node;
        }

        private Parameter CreateNode(XDocument xdoc, string parentFullname, System.Reflection.ParameterInfo info)
        {
            var doc = FindParam(xdoc, parentFullname, info.Name, "param");

            return new Parameter()
            {
                Name = info.Name,
                Description = doc,
                Type = CreateName(info.ParameterType)
            };
        }

        private Property CreateNode(XDocument xdoc, System.Reflection.PropertyInfo info)
        {
            var doc = FindSummary(xdoc, $"P:{info.DeclaringType.FullName}.{info.Name}");

            return new Property()
            {
                Name = info.Name,
                Description = doc,
                Type = CreateName(info.PropertyType),
                CanRead = info.CanRead,
                CanWrite = info.CanWrite,
            };
        }

        private Method CreateNode(XDocument xdoc, System.Reflection.MethodInfo info)
        {
            var fullname = $"M:{info.DeclaringType.FullName}.{info.Name}";

            if (info.GetGenericArguments().Any())
            {
                fullname += $"``{info.GetGenericArguments().Count()}";
            }

            if (info.GetParameters().Any())
            {
                fullname += $"({ (string.Join(",", info.GetParameters().Select((p) => p.ParameterType.FullName)))})";
            }

            var doc = FindSummary(xdoc, fullname);

            var node = new Method()
            {
                Name = CreateName(info),
                Description = doc,
                Type = CreateName(info.ReturnType),
                Arguments = info.GetParameters().Select((p) => this.CreateNode(xdoc,fullname, p))
            };

            return node;
        }

        private readonly System.Reflection.BindingFlags DeclaredFlags = System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.DeclaredOnly;

        private Event CreateNode(XDocument xdoc, System.Reflection.EventInfo info)
        {
            var doc = FindSummary(xdoc, $"E:{info.DeclaringType.FullName}.{info.Name}");

            var node = new Event()
            {
                Name = info.Name,
                Description = doc,
                Type = CreateName(info.EventHandlerType),
            };

            return node;
        }

        private Type CreateNode(XDocument xdoc, System.Type type)
        {
            var doc = FindSummary(xdoc, $"T:{type.FullName}");

            var node = new Type()
            {
                Name = CreateName(type),
                Description = doc,
                Methods = type.GetMethods(DeclaredFlags).Where((m) => !m.IsSpecialName).Select((m) => this.CreateNode(xdoc,m)),
                Events = type.GetEvents(DeclaredFlags).Select((e) => this.CreateNode(xdoc,e)),
                Properties = type.GetProperties().Select((p) => this.CreateNode(xdoc,p)),
            };
            
            return node;
        }

        private TypeParameter CreateParameterNode(XDocument xdoc, System.Type type)
        {

            var node = new TypeParameter()
            {
                Name = CreateName(type)
            };

            return node;
        }


        private string CreateName(System.Type type)
        {
            var name = type.Name;
            var gs = type.GetGenericArguments();

            if (gs.Any())
            {
                name = name.Replace($"`{gs.Length}", "<" + string.Join(",", gs.Select((g) => g.Name)) + ">");
            }
            
            return name;
        }

        private string CreateName(System.Reflection.MethodInfo m)
        {
            var name = m.Name;
            var gs = m.GetGenericArguments();

            if (gs.Any())
            {
                name += "<" + string.Join(",", gs.Select((g) => g.Name)) + ">";
            }

            return name;
        }

        private Assembly CreateTree(XDocument doc, System.Reflection.Assembly assembly)
        {
            var root = CreateNode(doc,assembly);
            return root;
        }

        #endregion

        public string Generate(string dllFile, string template, string ext)
        {
            var assembly = System.Reflection.Assembly.LoadFrom(dllFile);
            
            var xmlDocFile = dllFile.Replace(".dll", ".xml");
            XDocument xml = null;

            if (File.Exists(xmlDocFile))
            {
                xml = XDocument.Load(xmlDocFile);
            }

            var tree = CreateTree(xml,assembly);

            var outputFile = dllFile.Replace(".dll", ext);

            var render = Handlebars.Compile(template);
            var content = render(tree);
            content = template.Replace("{{{{DOC}}}}", content);

            File.WriteAllText(outputFile, content);

            return content;
        }
    }
}
