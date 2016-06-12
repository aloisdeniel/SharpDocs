

namespace SharpDocs.Parsers
{
    using System;
    using System.Linq;
    using Entities;
    using System.Reflection;
    using System.Text.RegularExpressions;
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
            
            if(typeof(System.Delegate).IsAssignableFrom(type))
            {
                fullname = fullname.Replace("+D",".D");
            }

            return doc.Find(fullname);
        }

        public static Member FindConstructor(this Documentation doc, ConstructorInfo info)
        {
            var name = info.Name.Replace(".ctor", "#ctor");
            var fullname = $"M:{info.DeclaringType.FullName}.{name}";
            
            if (info.GetParameters().Any())
            {
                var args = FormatParameters(info.GetParameters(), new Type[0]);
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
                var args = FormatParameters(info.GetParameters(), genericParams);
                fullname += $"({args})";
            }

            fullname = $"M:{info.DeclaringType.FullName}.{fullname}";
            
            return doc.Find(fullname);
        }

        private static string FormatParameters(ParameterInfo[] info, Type[] genericParams)
        {
            return string.Join(",", info.Select((p) => {

                var name = FormatTypeName(p.ParameterType, genericParams);
                if(name.EndsWith("&"))
                {
                    name = name.TrimEnd('&') + "@"; // ref & out
                }
                return name;
            }).ToList());
        }

        private static string FormatTypeName(Type t, Type[] genericParams)
        {
            if (t.IsArray)
            {
                var baseType = t.GetElementType();
                var rank = t.GetArrayRank();
                var result = $"{FormatTypeName(baseType, genericParams)}[";
                
                // FIXME : should be [lowerboundsize:size]
                if(rank > 1)
                {
                    for (int i = 0; i < rank; i++)
                    {
                        if (i != 0) result += ",";
                        result += "0:";
                    }
                }

                return $"{result}]";
            }

            var name = t.FullName ?? t.Name;

            if (t.IsGenericParameter)
            {
                name = "``" + genericParams.ToList().FindIndex((g) => g == t);
            }

            return name;
        }
        
        public static Member FindProperty(this Documentation doc, PropertyInfo info)
        {
            var fullname = $"P:{info.DeclaringType.FullName}.{info.Name}";
            return doc.Find(fullname);
        }

        public static Member FindField(this Documentation doc, FieldInfo info)
        {
            var fullname = $"F:{info.DeclaringType.FullName}.{info.Name}";
            return doc.Find(fullname);
        }
    }
}
