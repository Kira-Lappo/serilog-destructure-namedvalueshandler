using System;
using System.Linq;

namespace Serilog.Destructure.NamedValuesHandler
{
    public static class OmitExtensions
    {
        public static NamedValuePolicyBuilder Omit(
            this NamedValuePolicyBuilder builder,
            params string[] names
        )
        {
            return builder.Omit(
                (name, _, _) =>
                    names.Any(n => string.Equals(n, name, StringComparison.InvariantCultureIgnoreCase)));
        }

        public static NamedValuePolicyBuilder OmitNamespace(
            this NamedValuePolicyBuilder builder,
            params string[] namespaces
        )
        {
            return builder.Omit(
                (_, _, valueType) =>
                {
                    var @namespace = valueType.Namespace;
                    return @namespace != default && namespaces.Any(n => @namespace.StartsWith(n));
                });
        }

        public static NamedValuePolicyBuilder OmitType(
            this NamedValuePolicyBuilder builder,
            params Type[] types
        )
        {
            return builder.Omit((_, _, valueType) => types.Contains(valueType));
        }
    }
}
