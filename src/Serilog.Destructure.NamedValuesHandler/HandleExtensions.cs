using System;

namespace Serilog.Destructure.NamedValuesHandler
{
    public static class HandleExtensions
    {
        public static NamedValuePolicyBuilder Handle<TValue>(
            this NamedValuePolicyBuilder namedValuePolicyBuilder,
            string valueName,
            Func<TValue, object> handler
        )
        {
            return namedValuePolicyBuilder.Handle<TValue>(
                valueName,
                value => (true, handler.Invoke(value)));
        }

        public static NamedValuePolicyBuilder Handle(
            this NamedValuePolicyBuilder namedValuePolicyBuilder,
            string valueName,
            Func<object, Type, object> handler
        )
        {
            return namedValuePolicyBuilder.Handle(
                valueName,
                (value, valueType) => (true, handler(value, valueType)));
        }

        public static NamedValuePolicyBuilder Handle(
            this NamedValuePolicyBuilder namedValuePolicyBuilder,
            string valueName,
            Func<object, Type, (bool, object)> handler
        )
        {
            return namedValuePolicyBuilder.Handle(
                (name, value, valueType) =>
                {
                    if (!string.Equals(valueName, name, StringComparison.InvariantCultureIgnoreCase))
                    {
                        return (false, value);
                    }

                    return handler(value, valueType);
                });
        }

        public static NamedValuePolicyBuilder Handle<TValue>(
            this NamedValuePolicyBuilder namedValuePolicyBuilder,
            string valueName,
            Func<TValue, (bool, object)> handler
        )
        {
            return namedValuePolicyBuilder.Handle(
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

        public static NamedValuePolicyBuilder Handle<TValue>(
            this NamedValuePolicyBuilder namedValuePolicyBuilder,
            Func<string, TValue, object> handler
        )
        {
            return namedValuePolicyBuilder.Handle<TValue>((name, value) => (true, handler(name, value)));
        }

        public static NamedValuePolicyBuilder Handle<TValue>(
            this NamedValuePolicyBuilder namedValuePolicyBuilder,
            Func<string, TValue, (bool IsHandled, object Value)> handler
        )
        {
            return namedValuePolicyBuilder.Handle(
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

        public static NamedValuePolicyBuilder Handle(
            this NamedValuePolicyBuilder namedValuePolicyBuilder,
            Func<string, object, Type, object> handler
        )
        {
            return namedValuePolicyBuilder.Handle((name, value, valueType) => (true, handler(name, value, valueType)));
        }
    }
}
