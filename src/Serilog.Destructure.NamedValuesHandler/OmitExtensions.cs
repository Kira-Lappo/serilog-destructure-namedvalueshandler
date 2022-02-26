using System;
using System.Linq;

namespace Serilog.Destructure.NamedValuesHandler
{
    public static class OmitExtensions
    {
        public static NamedValueHandlersBuilder Omit(
            this NamedValueHandlersBuilder builder,
            params string[] names
        )
        {
            return builder.Omit(
                namedValue =>
                    names.Any(n => string.Equals(n, namedValue.Name, StringComparison.InvariantCultureIgnoreCase)));
        }

        public static NamedValueHandlersBuilder OmitNamespace(
            this NamedValueHandlersBuilder builder,
            params string[] namespaces
        )
        {
            return builder.Omit(
                namedValue =>
                {
                    var @namespace = namedValue.ValueType?.Namespace;
                    return @namespace != default && namespaces.Any(n => @namespace.StartsWith(n));
                });
        }

        public static NamedValueHandlersBuilder OmitType(
            this NamedValueHandlersBuilder builder,
            params Type[] types
        )
        {
            return builder.Omit(namedValue => namedValue.ValueType != default && types.Contains(namedValue.ValueType));
        }
    }
}
