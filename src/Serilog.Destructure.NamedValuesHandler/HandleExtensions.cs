using System;

namespace Serilog.Destructure.NamedValuesHandler
{
    public static class HandleExtensions
    {
        public static NamedValueDestructuringPolicy.NamedValuePolicyBuilder HandleNamedValue<TValue>(
            this NamedValueDestructuringPolicy.NamedValuePolicyBuilder namedValuePolicyBuilder,
            string valueName,
            Func<TValue, object> handler
        )
        {
            return namedValuePolicyBuilder.HandleNamedValue<TValue>(
                valueName,
                value => (true, handler.Invoke(value)));
        }

        public static NamedValueDestructuringPolicy.NamedValuePolicyBuilder HandleNamedValue(
            this NamedValueDestructuringPolicy.NamedValuePolicyBuilder namedValuePolicyBuilder,
            string valueName,
            Func<object, Type, (bool, object)> handler
        )
        {
            return namedValuePolicyBuilder.HandleNamedValue(
                (name, value, valueType) =>
                {
                    if (!string.Equals(valueName, name, StringComparison.InvariantCultureIgnoreCase))
                    {
                        return (false, value);
                    }

                    return handler(value, valueType);
                });
        }

        public static NamedValueDestructuringPolicy.NamedValuePolicyBuilder HandleNamedValue<TValue>(
            this NamedValueDestructuringPolicy.NamedValuePolicyBuilder namedValuePolicyBuilder,
            string valueName,
            Func<TValue, (bool, object)> handler
        )
        {
            return namedValuePolicyBuilder.HandleNamedValue(
                valueName,
                (value, valueType) =>
                {
                    if (handler == null
                        || typeof(TValue) != valueType
                        && typeof(TValue) != value.GetType())
                    {
                        return (false, value);
                    }

                    return handler((TValue)value);
                });
        }

        public static NamedValueDestructuringPolicy.NamedValuePolicyBuilder HandleNamedValue<TValue>(
            this NamedValueDestructuringPolicy.NamedValuePolicyBuilder namedValuePolicyBuilder,
            Func<string, TValue, object> handler
        )
        {
            return namedValuePolicyBuilder.HandleNamedValue<TValue>(
                (name, value) => (true, handler(name, value)));
        }

        public static NamedValueDestructuringPolicy.NamedValuePolicyBuilder HandleNamedValue<TValue>(
            this NamedValueDestructuringPolicy.NamedValuePolicyBuilder namedValuePolicyBuilder,
            Func<string, TValue, (bool IsHandled, object Value)> handler
        )
        {
            return namedValuePolicyBuilder.HandleNamedValue(
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

        public static NamedValueDestructuringPolicy.NamedValuePolicyBuilder HandleNamedValue(
            this NamedValueDestructuringPolicy.NamedValuePolicyBuilder namedValuePolicyBuilder,
            Func<string, object, Type, object> handler
        )
        {
            return namedValuePolicyBuilder.HandleNamedValue(
                (name, value, valueType) => (true, handler(name, value, valueType)));
        }
    }
}
