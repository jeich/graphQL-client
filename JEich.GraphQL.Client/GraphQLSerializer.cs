using JEich.GraphQL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace JEich.GraphQL
{
    public class GraphQLSerializer
    {
        private static IDictionary<string, IEnumerable<PropertyInfo>> _assemblyCache;

        static GraphQLSerializer()
        {
            _assemblyCache = new Dictionary<string, IEnumerable<PropertyInfo>>();
        }

        public static string SerializeRequestObjects(params RequestObject[] objs)
        {
            var sb = new StringBuilder();
            sb.Append('{');
            sb.Append(Environment.NewLine);
            foreach (var obj in objs)
            {
                sb.Append(obj.Name);
                SerializeAlias(obj, sb);
                SerializeObjectStart(obj.Object, sb);
                sb.Append(" {");
                sb.Append(Environment.NewLine);
                SerializeObject(obj.Object, sb);
                sb.Append(Environment.NewLine);
                sb.Append('}');
                sb.Append(Environment.NewLine);
            }
            sb.Append('}');
            return sb.ToString();
        }

        public static string SerializeRequestObject(RequestObject obj)
        {
            var sb = new StringBuilder();
            sb.Append('{');
            sb.Append(Environment.NewLine);
            sb.Append(obj.Name);
            SerializeAlias(obj, sb);
            SerializeObjectStart(obj.Object, sb);
            sb.Append(" {");
            sb.Append(Environment.NewLine);
            SerializeObject(obj.Object, sb);
            sb.Append(Environment.NewLine);
            sb.Append('}');
            sb.Append(Environment.NewLine);
            sb.Append('}');
            return sb.ToString();
        }

        private static void SerializeObject(object obj, StringBuilder sb)
        {
            var properties = GetPropertyInfo(obj);
            foreach (var p in properties)
            {
                if (!IsNonCustomType(p.PropertyType.GetTypeInfo()))
                {
                    var unwrapped = p.PropertyType.IsArray
                        ? p.PropertyType.GetElementType()
                        : p.PropertyType;
                    sb.Append(p.Name.ToLower());
                    SerializeObjectStart(p.GetValue(obj) ?? Activator.CreateInstance(unwrapped), sb);
                    sb.Append(" {");
                    sb.Append(Environment.NewLine);
                    //TODO: Make a type based duplicate method to avoid this waste of resources
                    SerializeObject(obj == null ? Activator.CreateInstance(unwrapped) : p.GetValue(obj) ?? Activator.CreateInstance(unwrapped), sb);
                    sb.Append(Environment.NewLine);
                    sb.Append("}");
                }
                else
                {
                    object value = p.GetValue(obj);
                    //Exclude arguments
                    if (value == null)
                    {
                        sb.Append(p.Name.ToLower());
                        if (p != properties.Last())
                            sb.Append(Environment.NewLine);
                    }
                }
            }
        }

        private static void SerializeObjectStart(object obj, StringBuilder sb)
        {
            var props = GetPropertyInfo(obj);
            var specifiedProps = props.Where(x => x.GetValue(obj) != null);
            if (specifiedProps.Any())
            {
                sb.Append("(");
                //TODO: Fix stringification of primitives
                sb.Append(string.Join(", ", specifiedProps.Select(x => $"{x.Name.ToLower()}: \"{x.GetValue(obj)}\"")));
                sb.Append(")");
            }
        }

        private static bool IsNonCustomType(TypeInfo t)
        {
            return t.IsPrimitive
                || t.IsEnum
                || t.Equals(typeof(string))
                || t.Equals(typeof(decimal));
        }

        private static IEnumerable<PropertyInfo> GetPropertyInfo(object obj)
        {
            Type t = obj.GetType();
            if (_assemblyCache.ContainsKey(t.FullName))
                return _assemblyCache[t.FullName];
            else
                return (_assemblyCache[t.FullName] = t.GetTypeInfo().DeclaredProperties);
        }

        private static void SerializeAlias(RequestObject obj, StringBuilder sb)
        {
            if (obj is AliasedObject)
            {
                sb.Append(": ");
                sb.Append(obj.Object.GetType().Name.ToLower());
            }
        }
    }
}
