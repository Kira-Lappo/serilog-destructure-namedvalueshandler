using System;
using System.Linq;

namespace Serilog.Destructure.NamedValuesHandler
{
    public static class OmitExtensions
    {
        public static NamedValueDestructuringPolicy.NamedValuePolicyBuilder Omit(
            this NamedValueDestructuringPolicy.NamedValuePolicyBuilder builder,
            params string[] names
        )
        {
            return builder.WithOmitHandler((name, _, _) =>
                names.Any(n => string.Equals(n, name, StringComparison.InvariantCultureIgnoreCase)));
        }

        public static NamedValueDestructuringPolicy.NamedValuePolicyBuilder OmitNamespace(
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

        public static NamedValueDestructuringPolicy.NamedValuePolicyBuilder OmitType(
            this NamedValueDestructuringPolicy.NamedValuePolicyBuilder builder,
            params Type[] types
        )
        {
            return builder.WithOmitHandler((_, _, valueType) => types.Contains(valueType));
        }
    }
}
