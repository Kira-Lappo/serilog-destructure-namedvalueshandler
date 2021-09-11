using System;

namespace Serilog.Destructure.NamedValuesHandler
{
    public static class HandleExtensions
    {
        public static NamedValueDestructuringPolicy.NamedValuePolicyBuilder HandleNamedValue<TValue>(
            this NamedValueDestructuringPolicy.NamedValuePolicyBuilder namedValuePolicyBuilder,
            string valueName,
            Func<TValue, TValue> handler
        )
        {
            return namedValuePolicyBuilder.HandleNamedValue<TValue>(
                valueName,
                value => (true, handler.Invoke(value)));
        }

        public static NamedValueDestructuringPolicy.NamedValuePolicyBuilder HandleNamedValue<TValue>(
            this NamedValueDestructuringPolicy.NamedValuePolicyBuilder namedValuePolicyBuilder,
            string valueName,
            Func<TValue, (bool IsHandled, TValue Value)> handler
        )
        {
            return namedValuePolicyBuilder.HandleNamedValue<TValue>(
                (name, value) =>
                {
                    if (handler == null
                        || !string.Equals(valueName, name, StringComparison.InvariantCultureIgnoreCase))
                    {
                        return (false, value);
                    }

                    return handler.Invoke(value);
                });
        }

        public static NamedValueDestructuringPolicy.NamedValuePolicyBuilder HandleNamedValue<TValue>(
            this NamedValueDestructuringPolicy.NamedValuePolicyBuilder namedValuePolicyBuilder,
            Func<string, TValue, TValue> handler
        )
        {
            return namedValuePolicyBuilder.HandleNamedValue<TValue>(
                (name, value) => (true, handler(name, value)));
        }

        public static NamedValueDestructuringPolicy.NamedValuePolicyBuilder HandleNamedValue<TValue>(
            this NamedValueDestructuringPolicy.NamedValuePolicyBuilder namedValuePolicyBuilder,
            Func<string, TValue, (bool IsHandled, TValue Value)> handler
        )
        {
            return namedValuePolicyBuilder.WithNamedValueHandler(
                (name, value, valueType) =>
                {
                    if (handler == null
                        || typeof(TValue) != valueType
                        && typeof(TValue) != value.GetType())
                    {
                        return (false, value);
                    }

                    return handler(name, (TValue)value);
                });
        }
    }
}
