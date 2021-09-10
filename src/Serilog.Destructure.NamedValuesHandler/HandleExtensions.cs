using System;

namespace Serilog.Destructure.NamedValuesHandler
{
    public static class HandleExtensions
    {
        public static NamedValueDestructuringPolicy.NamedValuePolicyBuilder HandleNamedValue(
            this NamedValueDestructuringPolicy.NamedValuePolicyBuilder namedValuePolicyBuilder,
            string name,
            Func<object, Type, object> handler
        )
        {
            return namedValuePolicyBuilder.WithNamedValueHandler(
                name,
                (value, declaringType) =>
                    handler != null
                        ? handler.Invoke(value, declaringType)
                        : value);
        }

        public static NamedValueDestructuringPolicy.NamedValuePolicyBuilder HandleNamedValue<TValue>(
            this NamedValueDestructuringPolicy.NamedValuePolicyBuilder namedValuePolicyBuilder,
            string name,
            Func<TValue, TValue> handler
        )
        {
            return namedValuePolicyBuilder.HandleNamedValue(
                name,
                (value, declaringType) =>
                    handler != null
                    && (typeof(TValue) == declaringType
                        || typeof(TValue) == value.GetType())
                        ? handler.Invoke((TValue)value)
                        : value);
        }
    }
}
