using HandlebarsDotNet;
using SharpDocs.Documentation;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml.Linq;

namespace SharpDocs
{
    using Formatters;
    using Parsers;

    public class SharpDocs
    {

        #region Tree

        private Assembly CreateNode(Parsers.Entities.Documentation doc, System.Reflection.Assembly assembly)
        {
            var descriptionAttribute = assembly
                                      .GetCustomAttributes(typeof(System.Reflection.AssemblyDescriptionAttribute), false)
                                      .OfType<System.Reflection.AssemblyDescriptionAttribute>()
                                      .FirstOrDefault();

            var node = new Assembly()
            {
                Name = assembly.GetName().Name,
                Description = doc.Assembly,
                Enums = assembly.ExportedTypes.Where((t) => t.IsEnum).Select((t) => CreateEnumNode(doc, t)).OrderBy((t) => t.Name).ToList(),
                Structs = assembly.ExportedTypes.Where((t) => !t.IsEnum && t.IsValueType).Select((t) => CreateNode(doc, t, "struct")).OrderBy((t) => t.Name).ToList(),
                Interfaces = assembly.ExportedTypes.Where((t) => t.IsInterface).Select((t) => CreateNode(doc, t, "interface")).OrderBy((t) => t.Name).ToList(),
                Classes = assembly.ExportedTypes.Where((t) => IsClass(t) && !t.IsAbstract).Select((t) => CreateNode(doc, t, "class")).OrderBy((t) => t.Name).ToList(),
                AbstractClasses = assembly.ExportedTypes.Where((t) => IsClass(t) && t.IsAbstract).Select((t) => CreateNode(doc, t, "abstract class")).OrderBy((t) => t.Name).ToList(),
                Attributes = assembly.ExportedTypes.Where((t) => IsAttribute(t)).Select((t) => CreateNode(doc, t, "attribute")).OrderBy((t) => t.Name).ToList(),
            };

            return node;
        }

        private static bool IsClass(System.Type t)
        {
            return t.IsClass && !t.IsValueType && !IsDelegate(t) && !IsAttribute(t);
        }

        private static bool IsAttribute(System.Type type)
        {
            return typeof(System.Attribute).IsAssignableFrom(type);
        }

        private static bool IsDelegate(System.Type type)
        {
            return typeof(System.Delegate).IsAssignableFrom(type);
        }

        private Parameter CreateNode(Parsers.Entities.Documentation doc, System.Reflection.ParameterInfo info)
        {
            return new Parameter()
            {
                Name = info.Name,
                Description = doc.FindParameter(info),
                Type = CreateName(info.ParameterType),
                IsOut = info.IsOut,
                IsOptional = info.IsOptional,
            };
        }

        private Property CreateNode(Parsers.Entities.Documentation doc, System.Reflection.PropertyInfo info)
        {
            return new Property()
            {
                Name = info.Name,
                Description = doc.FindProperty(info),
                IsAbstract = (info.CanWrite && info.SetMethod.IsAbstract) || (info.CanRead && info.GetMethod.IsAbstract),
                IsVirtual = (info.CanWrite && info.SetMethod.IsVirtual) || (info.CanRead && info.GetMethod.IsVirtual),
                Type = CreateName(info.PropertyType),
                CanRead = info.CanRead,
                CanWrite = info.CanWrite,
            };
        }

        private Method CreateNode(Parsers.Entities.Documentation doc, System.Reflection.MethodInfo info)
        {
            var node = new Method()
            {
                Name = CreateName(info),
                Description = doc.FindMethod(info),
                IsAbstract = info.IsAbstract,
                IsVirtual = info.IsVirtual,
                Type = CreateName(info.ReturnType),
                Arguments = info.GetParameters().Select((p) => this.CreateNode(doc, p)).ToList()
            };

            return node;
        }

        private Method CreateNode(Parsers.Entities.Documentation doc, System.Reflection.ConstructorInfo info)
        {
            var name = info.Name.Replace(".ctor", "#ctor");
            var node = new Method()
            {
                Name = name,
                Description = doc.FindConstructor(info),
                Arguments = info.GetParameters().Select((p) => this.CreateNode(doc, p)).ToList()
            };

            return node;
        }

        private readonly System.Reflection.BindingFlags DeclaredFlags = System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.DeclaredOnly;

        private Event CreateNode(Parsers.Entities.Documentation doc, System.Reflection.EventInfo info)
        {
            var node = new Event()
            {
                Name = info.Name,
                Description = doc.FindEvent(info),
                Type = CreateName(info.EventHandlerType),
            };

            return node;
        }

        private Enum CreateEnumNode(Parsers.Entities.Documentation doc, System.Type type)
        {
            return new Enum()
            {
                Name = CreateName(type),
                Description = doc.FindType(type),
                Values = System.Enum.GetNames(type),
            };
        }

        private Type CreateNode(Parsers.Entities.Documentation doc, System.Type type, string category)
        {
            var node = new Type()
            {
                Name = CreateName(type),
                Category = category,
                Description = doc.FindType(type),
                IsAbstract = type.IsAbstract,
                Delegates = type.GetNestedTypes().Where((t) => typeof(System.Delegate).IsAssignableFrom(t)).Select((t) => this.CreateParameterNode(doc, t)).OrderBy((n) => n.Name).ToList(),
                ParentClass = type.BaseType != null && type.BaseType != typeof(System.ValueType) && type.BaseType != typeof(object) ? this.CreateParameterNode(doc, type.BaseType) : null,
                ParentInterfaces = type.GetInterfaces().Select((t) => this.CreateParameterNode(doc,t)).ToList(),
                Methods = type.GetMethods(DeclaredFlags).Where((m) => !m.IsSpecialName && !typeof(Task).IsAssignableFrom(m.ReturnType)).Select((m) => this.CreateNode(doc,m)).OrderBy((m) => m.Name).ToList(),
                AsyncMethods = type.GetMethods(DeclaredFlags).Where((m) => !m.IsSpecialName && typeof(Task).IsAssignableFrom(m.ReturnType)).Select((m) => this.CreateNode(doc, m)).OrderBy((m) => m.Name).ToList(),
                Constructors = type.GetConstructors().Select((m) => this.CreateNode(doc,m)).ToList(),
                Events = type.GetEvents(DeclaredFlags).Select((e) => this.CreateNode(doc,e)).OrderBy((m) => m.Name).ToList(),
                Properties = type.GetProperties().Where((p) => !IsCommand(p)).Select((p) => this.CreateNode(doc,p)).OrderBy((m) => m.Name).ToList(),
                Commands = type.GetProperties().Where((p) => IsCommand(p)).Select((p) => this.CreateNode(doc, p)).OrderBy((m) => m.Name).ToList(),
            };
            
            return node;
        }

        private static bool IsCommand(System.Reflection.PropertyInfo p)
        {
            return typeof(ICommand).IsAssignableFrom(p.PropertyType);
        }

        private TypeParameter CreateParameterNode(Parsers.Entities.Documentation doc, System.Type type)
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
            
            if (type.IsByRef)
            {
                name = name.TrimEnd('&');
            }
            
            if (type.IsNested && type.DeclaringType != null)
            {
                name = $"{CreateName(type.DeclaringType)}.{name}";
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

        private Assembly CreateTree(Parsers.Entities.Documentation doc, System.Reflection.Assembly assembly)
        {
            var root = CreateNode(doc,assembly);
            return root;
        }

        #endregion



        public string Generate(string dllFile, string template, string ext)
        {
            var assembly = System.Reflection.Assembly.LoadFrom(dllFile);
            
            var xmlDocFile = dllFile.Replace(".dll", ".xml");
    
            var doc = new MsDocParser().Parse(xmlDocFile);

            var tree = CreateTree(doc,assembly);

            var content = new HtmlFormatter().Render(tree);

            var outputFile = dllFile.Replace(".dll", ext);
            
            File.WriteAllText(outputFile, content);

            return content;
        }
    }
}
