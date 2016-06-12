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
    using System.Collections.Generic;
    using System.Text.RegularExpressions;
    public class SharpDocs
    {
        public SharpDocs() : this(new HtmlFormatter())
        {
        }

        public SharpDocs(IFormatter formatter)
        {
            this.Formatter = formatter;
        }

        public IFormatter Formatter { get; private set; }

        #region Tree

        private readonly System.Reflection.BindingFlags DeclaredFlags = System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.DeclaredOnly;


        private Assembly CreateNode(Parsers.Entities.Documentation doc, System.Reflection.Assembly assembly)
        {
            var descriptionAttribute = assembly
                                      .GetCustomAttributes(typeof(System.Reflection.AssemblyDescriptionAttribute), false)
                                      .OfType<System.Reflection.AssemblyDescriptionAttribute>()
                                      .FirstOrDefault();

            var node = new Assembly()
            {
                Name = assembly.GetName().Name,
                Description = doc?.Assembly,
                Enums = assembly.ExportedTypes.Where((t) => t.IsEnum).Select((t) => CreateEnumNode(doc, t)).OrderBy((t) => t.Name).ToList(),
                Structs = assembly.ExportedTypes.Where((t) => !t.IsEnum && t.IsValueType).Select((t) => CreateNode(doc, t, "struct")).OrderBy((t) => t.Name).ToList(),
                Interfaces = assembly.ExportedTypes.Where((t) => t.IsInterface).Select((t) => CreateNode(doc, t, "interface")).OrderBy((t) => t.Name).ToList(),
                Classes = assembly.ExportedTypes.Where((t) => IsClass(t) && !IsStaticClass(t) && !t.IsAbstract).Select((t) => CreateNode(doc, t, "class")).OrderBy((t) => t.Name).ToList(),
                StaticClasses = assembly.ExportedTypes.Where((t) => IsClass(t) && IsStaticClass(t)).Select((t) => CreateNode(doc, t, "static class")).OrderBy((t) => t.Name).ToList(),
                AbstractClasses = assembly.ExportedTypes.Where((t) => IsClass(t) && !IsStaticClass(t) && t.IsAbstract).Select((t) => CreateNode(doc, t, "abstract class")).OrderBy((t) => t.Name).ToList(),
                Attributes = assembly.ExportedTypes.Where((t) => IsAttribute(t)).Select((t) => CreateNode(doc, t, "attribute")).OrderBy((t) => t.Name).ToList(),
            };

            return node;
        }

        private static bool IsClass(System.Type t)
        {
            return t.IsClass && !t.IsValueType && !IsDelegate(t) && !IsAttribute(t);
        }


        private static bool IsStaticClass(System.Type t)
        {
            return t.IsSealed && t.IsAbstract;
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
                Description = doc?.FindParameter(info),
                Type = CreateName(info.ParameterType),
                IsOut = info.IsOut,
                IsRef = !info.IsOut && info.ParameterType.IsByRef,
                IsOptional = info.IsOptional,
            };
        }

        private Property CreateNode(Parsers.Entities.Documentation doc, System.Reflection.PropertyInfo info)
        {
            return new Property()
            {
                Name = info.Name,
                IsStatic = (info.SetMethod != null && info.SetMethod.IsStatic) || (info.GetMethod != null && info.GetMethod.IsStatic),
                Description = doc?.FindProperty(info),
                IsAbstract = (!info.DeclaringType.IsInterface) && ((info.CanWrite && info.SetMethod.IsAbstract) || (info.CanRead && info.GetMethod.IsAbstract)),
                IsVirtual = (info.CanWrite && info.SetMethod.IsVirtual) || (info.CanRead && info.GetMethod.IsVirtual),
                Type = CreateName(info.PropertyType),
                CanRead = info.CanRead,
                CanWrite = info.CanWrite,
            };
        }

        private Method CreateNode(Parsers.Entities.Documentation doc, System.Reflection.MethodInfo info)
        {
            var name = CreateName(info);

            if (specialMethods.ContainsKey(name))
            {
                name = specialMethods[name];
            }

            var node = new Method()
            {
                Name = name,
                IsStatic = info.IsStatic,
                Description = doc?.FindMethod(info),
                IsAbstract = !info.DeclaringType.IsInterface && info.IsAbstract,
                IsVirtual = info.IsVirtual,
                IsExtension = info.IsDefined(typeof(System.Runtime.CompilerServices.ExtensionAttribute), false),
                Type = CreateName(info.ReturnType),
                Arguments = info.GetParameters().Select((p) => this.CreateNode(doc, p)).ToList()
            };

            return node;
        }

        private readonly Dictionary<string, string> specialMethods = new Dictionary<string, string>
        {
            { "op_Addition", "+" },
            { "op_Subtraction", "-" },
            { "op_Multiply", "*" },
            { "op_Division", "/" },
            { "op_Equality", "==" },
            { "op_Inequality", "!=" },
            { "op_LessThan", "<" },
            { "op_GreaterThan", ">" },
            { "op_LessThanOrEqual", "<=" },
            { "op_GreaterThanOrEqual", ">=" },
            { "op_Increment", "++" },
            { "op_Decrement", "--" },
            { "op_False", "false" },
            { "op_True", "true" },
            { "op_LogicalNot", "!" },
            { "op_OnesComplement", "~" },
            { "op_UnaryNegation", "-" },
            { "op_UnaryPlus", "+" },
            { "op_Explicit", "explicit cast" },
        };

        private Method CreateNode(Parsers.Entities.Documentation doc, System.Reflection.ConstructorInfo info)
        {
            var name = info.Name.Replace(".ctor", this.CreateName(info.DeclaringType));
            
            var node = new Method()
            {
                Name = name,
                Description = doc?.FindConstructor(info),
                Arguments = info.GetParameters().Select((p) => this.CreateNode(doc, p)).ToList()
            };

            return node;
        }

        private Event CreateNode(Parsers.Entities.Documentation doc, System.Reflection.EventInfo info)
        {
            var node = new Event()
            {
                Name = info.Name,
                Description = doc?.FindEvent(info),
                Type = CreateName(info.EventHandlerType),
            };

            return node;
        }

        private Field CreateNode(Parsers.Entities.Documentation doc, System.Reflection.FieldInfo info)
        {
            if(info.IsLiteral)
            {

                return new Constant()
                {
                    Name = info.Name,
                    Value = info.GetValue(null)?.ToString(),
                    Description = doc?.FindField(info),
                    Type = CreateName(info.FieldType),
                };
            }

            return new Field()
            {
                Name = info.Name,
                IsStatic = info.IsStatic,
                IsReadOnly = info.IsInitOnly,
                Description = doc?.FindField(info),
                Type = CreateName(info.FieldType),
            };
        }

        private Enum CreateEnumNode(Parsers.Entities.Documentation doc, System.Type type)
        {
            return new Enum()
            {
                Name = CreateName(type),
                Namespace = type.Namespace,
                Description = doc?.FindType(type),
                Values = System.Enum.GetNames(type),
            };
        }

        private Type CreateNode(Parsers.Entities.Documentation doc, System.Type type, string category)
        {
            var properties = type.GetProperties().Where((p) => !IsCommand(p)).Select((p) => this.CreateNode(doc, p)).OrderBy((m) => m.Name).ToList();
            var methods = type.GetMethods(DeclaredFlags).Where((m) => !m.IsSpecialName && !typeof(Task).IsAssignableFrom(m.ReturnType)).Select((m) => this.CreateNode(doc, m)).OrderBy((m) => m.Name).ToList();
            var fields = type.GetFields().Where((f) => !f.IsLiteral).Select((f) => this.CreateNode(doc, f)).ToList();

            var node = new Type()
            {
                Name = CreateName(type),
                Namespace = type.Namespace,
                Category = category,
                Description = doc?.FindType(type),
                IsAbstract = type.IsAbstract,
                Fields = fields.Where((m) => !m.IsStatic),
                StaticFields = fields.Where((m) => m.IsStatic),
                Constants = type.GetFields().Where((f) => f.IsLiteral).Select((f) => this.CreateNode(doc, f)).ToList(),
                Delegates = type.GetNestedTypes().Where((t) => typeof(System.Delegate).IsAssignableFrom(t)).Select((t) => {
                    var typeNode = this.CreateNode(doc, t, "delegate");
                    var method = t.GetMethod("Invoke");
                    var methodNode = this.CreateNode(doc, method);
                    methodNode.Name = typeNode.Name;
                    methodNode.Description = typeNode.Description;
                    return methodNode;
                }).OrderBy((n) => n.Name).ToList(),
                ParentClass = type.BaseType != null && type.BaseType != typeof(System.ValueType) && type.BaseType != typeof(object) ? this.CreateParameterNode(doc, type.BaseType) : null,
                ParentInterfaces = type.GetInterfaces().Where((i) => !typeof(System.Runtime.InteropServices._Attribute).IsAssignableFrom(i)).Select((t) => this.CreateParameterNode(doc, t)).ToList(),
                Methods = methods.Where((m) => !m.IsStatic),
                StaticMethods = methods.Where((m) => m.IsStatic),
                Operators = type.GetMethods().Where((m) => m.IsSpecialName && m.Name.StartsWith("op_")).Select((m) => this.CreateNode(doc, m)).OrderBy((m) => m.Name).ToList(),
                AsyncMethods = type.GetMethods(DeclaredFlags).Where((m) => !m.IsSpecialName && typeof(Task).IsAssignableFrom(m.ReturnType)).Select((m) => this.CreateNode(doc, m)).OrderBy((m) => m.Name).ToList(),
                Constructors = type.GetConstructors().Select((m) => this.CreateNode(doc,m)).ToList(),
                Events = type.GetEvents(DeclaredFlags).Select((e) => this.CreateNode(doc,e)).OrderBy((m) => m.Name).ToList(),
                Properties = properties.Where((m) => !m.IsStatic),
                StaticProperties = properties.Where((m) => m.IsStatic),
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
            
            if (type.IsNested && !type.IsGenericParameter && type.DeclaringType != null)
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
        
        public string Generate(string dllFile, string xmlDocFile = null)
        {
            var assembly = System.Reflection.Assembly.LoadFrom(dllFile);
            
            if(string.IsNullOrEmpty(xmlDocFile))
            {
                xmlDocFile = dllFile.Replace(".dll", ".xml");
            }

            Parsers.Entities.Documentation doc = null;
            
            if(File.Exists(xmlDocFile))
            {
                doc = new MsDocParser().Parse(xmlDocFile);
            }
            else
            {
                System.Console.WriteLine("WARNING: no documentation file found, verify that you've setup the XML documentation generation in project properties.");
            }
            var tree = CreateTree(doc,assembly);

            var content = this.Formatter.Render(tree);

            var outputFile = Regex.Replace(xmlDocFile, "\\.xml", this.Formatter.Extension, RegexOptions.IgnoreCase);
            
            File.WriteAllText(outputFile, content);

            return content;
        }
    }
}
