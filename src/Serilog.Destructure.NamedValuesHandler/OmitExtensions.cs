using System;
using System.Linq;

namespace Serilog.Destructure.NamedValuesHandler
{
    public static class OmitExtensions
    {
        public static NamedValueDestructuringPolicy.NamedValuePolicyBuilder OmitNames(
            this NamedValueDestructuringPolicy.NamedValuePolicyBuilder builder,
            params string[] names
        )
        {
            return builder.WithOmitHandler((name, _, _) => names.Contains(name));
        }

        public static NamedValueDestructuringPolicy.NamedValuePolicyBuilder OmitFromNamespace(
            this NamedValueDestructuringPolicy.NamedValuePolicyBuilder builder,
            params string[] namespaces
        )
        {
            return builder.WithOmitHandler(
                (_, _, valueType) =>
                {
                    var @namespace = valueType.Namespace;
                    return @namespace != default && namespaces.Any(n => @namespace.StartsWith(n));
                });
        }

        public static NamedValueDestructuringPolicy.NamedValuePolicyBuilder OmitOfType(
            this NamedValueDestructuringPolicy.NamedValuePolicyBuilder builder,
            params Type[] types
        )
        {
            return builder.WithOmitHandler((_, _, valueType) => types.Contains(valueType));
        }
    }
}
