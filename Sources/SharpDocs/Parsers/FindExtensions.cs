

namespace SharpDocs.Parsers
{
    using System;
    using System.Linq;
    using Entities;
    using System.Reflection;

    public static class FindExtensions
    {
        private static Member Find(this Documentation doc, string fullname)
        {
            return doc.Members.FirstOrDefault((m) => m.Name == fullname);
        }
        
        public static Member FindEvent(this Documentation doc, EventInfo info)
        {
            var fullname = $"E:{info.DeclaringType.FullName}.{info.Name}";
            return doc.Find(fullname);
        }

        public static Member FindParameter(this Documentation doc, ParameterInfo info)
        {
            var methodIndo = info.Member as MethodInfo;
            var method = doc.FindMethod(methodIndo);
            return method?.Parameters.FirstOrDefault((p) => p.Name == info.Name);
        }

        public static Member FindType(this Documentation doc, Type type)
        {
            var fullname = $"T:{type.FullName}";
            return doc.Find(fullname);
        }

        public static Member FindConstructor(this Documentation doc, ConstructorInfo info)
        {
            var name = info.Name.Replace(".ctor", "#ctor");
            var fullname = $"M:{info.DeclaringType.FullName}.{name}";

            if (info.GetParameters().Any())
            {
                var args = string.Join(",", info.GetParameters().Select((p) => p.ParameterType.FullName).ToList());
                fullname += $"({args})";
            }

            return doc.Find(fullname);
        }

        public static Member FindMethod(this Documentation doc, MethodInfo info)
        {
            if (info == null)
                return null;

            var fullname = info.Name;

            var genericParams = info.GetGenericArguments();
            if (genericParams.Any())
            {
                fullname += $"``{genericParams.Count()}";
            }

            if (info.GetParameters().Any())
            {
                var args = string.Join(",", info.GetParameters().Select((p) => {
                    var name = p.ParameterType.FullName;

                    if (p.ParameterType.IsGenericParameter)
                    {
                        name = "``" + genericParams.ToList().FindIndex((t) => t == p.ParameterType);
                    }

                    if (p.IsOut)
                    {
                        name = name.TrimEnd('&') + "@";
                    }
                    return name;
                }).ToList());
                fullname += $"({args})";
            }

            fullname = $"M:{info.DeclaringType.FullName}.{fullname}";

            return doc.Find(fullname);
        }
        
        public static Member FindProperty(this Documentation doc, PropertyInfo info)
        {
            var fullname = $"P:{info.DeclaringType.FullName}.{info.Name}";
            return doc.Find(fullname);
        }
     }
}
