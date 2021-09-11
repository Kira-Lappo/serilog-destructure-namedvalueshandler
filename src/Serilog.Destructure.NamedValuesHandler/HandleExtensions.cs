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
                (value, valueType) =>
                    handler != null
                        ? handler.Invoke(value, valueType)
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
                (value, valueType) =>
                    handler != null
                    && (typeof(TValue) == valueType
                        || typeof(TValue) == value.GetType())
                        ? handler.Invoke((TValue)value)
                        : value);
        }
    }
}
