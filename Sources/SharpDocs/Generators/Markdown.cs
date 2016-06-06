using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SharpDocs.Generators
{
    public class Markdown : IGenerator
    {
        public string Extension
        {
            get { return ".md"; }
        }

        private StringBuilder AppendAssembly(StringBuilder builder, Assembly assembly)
        {
            builder.AppendLine($"# {assembly.GetName().Name}").AppendLine();

            var descriptionAttribute = assembly
                                        .GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false)
                                        .OfType<AssemblyDescriptionAttribute>()
                                        .FirstOrDefault();

            if (descriptionAttribute != null)
                builder.AppendLine($"{descriptionAttribute.Description}").AppendLine();

            return builder;
        }
        private StringBuilder AppendType(StringBuilder builder, Type type)
        {
            var name = type.Name;
            var gs = type.GetGenericArguments();
            if (gs.Any())
            {
                name = name.Replace($"`{gs.Length}", "<" + string.Join(",", gs.Select((g) => g.Name)) + ">");
            }

            builder.Append(name);

            return builder;
        }

        private StringBuilder AppendProperty(StringBuilder builder, PropertyInfo info)
        {
            builder.Append($"### (Property) ");
            AppendType(builder,info.PropertyType);
            builder.Append($" {info.Name} ");
            builder.Append($"{(info.CanRead ? "[get]" : "")} ");
            builder.Append($"{(info.CanWrite ? "[set]" : "")} ");
            builder.AppendLine().AppendLine();

            return builder;
        }


        private StringBuilder AppendMethod(StringBuilder builder, MethodInfo info)
        {
            builder.Append($"### (Method) ");
            AppendType(builder, info.ReturnType);
            builder.Append($" {info.Name}(");

            var parameters = info.GetParameters();
            for (int i = 0; i < parameters.Length; i++)
            {
                var p = parameters[i];
                if (i > 0) builder.Append(", ");
                AppendType(builder, p.ParameterType);
                builder.Append($" {p.Name}");
            }

            builder.Append($")");

            builder.AppendLine().AppendLine();

            return builder;
        }

        private StringBuilder AppendEvent(StringBuilder builder, EventInfo info)
        {
            builder.Append($"### (Event) ");
            AppendType(builder, info.EventHandlerType);
            builder.Append($" {info.Name}");

            builder.AppendLine().AppendLine();

            return builder;
        }

        private StringBuilder AppendMethodParameter(StringBuilder builder, MethodInfo info)
        {
            builder.Append($"{info.ReturnType.Name} ");
            builder.Append($"{info.Name} ");
            builder.Append($"{info.Name} ");
            builder.AppendLine().AppendLine();

            return builder;
        }

        public string Generate(Assembly assembly)
        {
            var builder = new StringBuilder();

            AppendAssembly(builder, assembly);

            foreach (var type in assembly.ExportedTypes)
            {
                builder.Append($"## ");
                AppendType(builder, type);
                builder.AppendLine().AppendLine();

                var props = type.GetProperties();
                if (props.Any())
                {
                    foreach (var prop in props)
                    {
                        AppendProperty(builder, prop);
                    }
                }

                var methods = type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);

                if (methods.Any())
                {
                    foreach (var meth in methods.Where((m) => !m.IsSpecialName))
                    {
                        AppendMethod(builder, meth);
                    }
                }

                var events = type.GetEvents();

                if (events.Any())
                {
                    foreach (var ev in events)
                    {
                        AppendEvent(builder, ev);
                    }
                }

            }

            return builder.ToString();
        }
    }
}
