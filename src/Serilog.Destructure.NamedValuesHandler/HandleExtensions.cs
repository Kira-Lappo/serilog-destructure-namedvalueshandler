using System;

namespace Serilog.Destructure.NamedValuesHandler
{
    public static class HandleExtensions
    {
        public static NamedValueDestructuringPolicyBuilder Handle<TValue>(
            this NamedValueDestructuringPolicyBuilder namedValueDestructuringPolicyBuilder,
            string valueName,
            Func<TValue, object> handler
        )
        {
            return namedValueDestructuringPolicyBuilder.Handle<TValue>(
                valueName,
                value => (true, handler.Invoke(value)));
        }

        public static NamedValueDestructuringPolicyBuilder Handle(
            this NamedValueDestructuringPolicyBuilder namedValueDestructuringPolicyBuilder,
            string valueName,
            Func<object, Type, object> handler
        )
        {
            return namedValueDestructuringPolicyBuilder.Handle(
                valueName,
                (value, valueType) => (true, handler(value, valueType)));
        }

        public static NamedValueDestructuringPolicyBuilder Handle(
            this NamedValueDestructuringPolicyBuilder namedValueDestructuringPolicyBuilder,
            string valueName,
            Func<object, Type, (bool, object)> handler
        )
        {
            return namedValueDestructuringPolicyBuilder.Handle(
                (name, value, valueType) =>
                {
                    if (!string.Equals(valueName, name, StringComparison.InvariantCultureIgnoreCase))
                    {
                        return (false, value);
                    }

                    return handler(value, valueType);
                });
        }

        public static NamedValueDestructuringPolicyBuilder Handle<TValue>(
            this NamedValueDestructuringPolicyBuilder namedValueDestructuringPolicyBuilder,
            string valueName,
            Func<TValue, (bool, object)> handler
        )
        {
            return namedValueDestructuringPolicyBuilder.Handle(
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

        public static NamedValueDestructuringPolicyBuilder Handle<TValue>(
            this NamedValueDestructuringPolicyBuilder namedValueDestructuringPolicyBuilder,
            Func<string, TValue, object> handler
        )
        {
            return namedValueDestructuringPolicyBuilder.Handle<TValue>((name, value) => (true, handler(name, value)));
        }

        public static NamedValueDestructuringPolicyBuilder Handle<TValue>(
            this NamedValueDestructuringPolicyBuilder namedValueDestructuringPolicyBuilder,
            Func<string, TValue, (bool IsHandled, object Value)> handler
        )
        {
            return namedValueDestructuringPolicyBuilder.Handle(
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

        public static NamedValueDestructuringPolicyBuilder Handle(
            this NamedValueDestructuringPolicyBuilder namedValueDestructuringPolicyBuilder,
            Func<string, object, Type, object> handler
        )
        {
            return namedValueDestructuringPolicyBuilder.Handle((name, value, valueType) => (true, handler(name, value, valueType)));
        }
    }
}
