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
            return xdoc.Descendants("member").Where((n) => n.Attribute("name")?.Value == fullname).ToList().FirstOrDefault();
        }

        private string FindSummary(XDocument xdoc, string fullname)
        {
            var doc = FindDoc(xdoc, fullname);
            return doc?.Descendants("summary").ToList().FirstOrDefault()?.Value.Trim();
        }
        private string FindParam(XDocument xdoc, string fullname, string paramName, string paramType)
        {
            var doc = FindDoc(xdoc, fullname);
            return doc?.Descendants("param").Where((n) => n.Attribute("name")?.Value == paramName).ToList().FirstOrDefault()?.Value.Trim();
        }

        private Assembly CreateNode(XDocument doc, System.Reflection.Assembly assembly)
        {
            var descriptionAttribute = assembly
                                      .GetCustomAttributes(typeof(System.Reflection.AssemblyDescriptionAttribute), false)
                                      .OfType<System.Reflection.AssemblyDescriptionAttribute>()
                                      .FirstOrDefault();

            var node = new Assembly()
            {
                Name = assembly.GetName().Name,
                Description = descriptionAttribute?.Description,
                Types = assembly.ExportedTypes.Where((t) => !typeof(System.Attribute).IsAssignableFrom(t)).Select((t) => CreateNode(doc, t)).ToList(),
                Attributes = assembly.ExportedTypes.Where((t) => typeof(System.Attribute).IsAssignableFrom(t)).Select((t) => CreateNode(doc, t)).ToList(),
            };

            return node;
        }

        private Parameter CreateNode(XDocument xdoc, string parentFullname, System.Reflection.ParameterInfo info)
        {
            var doc = FindParam(xdoc, parentFullname, info.Name, "param");

            return new Parameter()
            {
                Name = info.Name,
                Description = doc,
                Type = CreateName(info.ParameterType),
                IsOut = info.IsOut,
                IsOptional = info.IsOptional,
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
                var args = string.Join(",", info.GetParameters().Select((p) => {
                    var name = p.ParameterType.FullName;
                    if(p.IsOut)
                    {
                        name += "@";
                    }
                    return name;
                }).ToList());
                fullname += $"({args})";
            }

            var doc = FindSummary(xdoc, fullname);

            var node = new Method()
            {
                Name = CreateName(info),
                Description = doc,
                Type = CreateName(info.ReturnType),
                Arguments = info.GetParameters().Select((p) => this.CreateNode(xdoc,fullname, p)).ToList()
            };

            return node;
        }

        private Method CreateNode(XDocument xdoc, System.Reflection.ConstructorInfo info)
        {
            var name = info.Name.Replace(".ctor", "#ctor");
            var fullname = $"M:{info.DeclaringType.FullName}.{name}";

            if (info.GetParameters().Any())
            {
                var args = string.Join(",", info.GetParameters().Select((p) => p.ParameterType.FullName).ToList());
                fullname += $"({args})";
            }

            var doc = FindSummary(xdoc, fullname);

            var node = new Method()
            {
                Name = name,
                Description = doc,
                Arguments = info.GetParameters().Select((p) => this.CreateNode(xdoc, fullname, p)).ToList()
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
                Methods = type.GetMethods(DeclaredFlags).Where((m) => !m.IsSpecialName && !typeof(Task).IsAssignableFrom(m.ReturnType)).Select((m) => this.CreateNode(xdoc,m)).ToList(),
                AsyncMethods = type.GetMethods(DeclaredFlags).Where((m) => !m.IsSpecialName && typeof(Task).IsAssignableFrom(m.ReturnType)).Select((m) => this.CreateNode(xdoc, m)).ToList(),
                Constructors = type.GetConstructors().Select((m) => this.CreateNode(xdoc,m)).ToList(),
                Events = type.GetEvents(DeclaredFlags).Select((e) => this.CreateNode(xdoc,e)).ToList(),
                Properties = type.GetProperties().Select((p) => this.CreateNode(xdoc,p)).ToList(),
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

        public string Generate(string dllFile)
        {
            var template = this.LoadTemplate("Default.html");
            return Generate(dllFile, template, ".html");
        }

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

            File.WriteAllText(outputFile, content);

            return content;
        }
    }
}
