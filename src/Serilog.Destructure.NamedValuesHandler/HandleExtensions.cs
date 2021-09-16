using System;

namespace Serilog.Destructure.NamedValuesHandler
{
    public static class HandleExtensions
    {
        public static NamedValueHandlersBuilder Handle<TValue>(
            this NamedValueHandlersBuilder namedValueHandlersBuilder,
            string valueName,
            Func<TValue, object> handler
        )
        {
            return namedValueHandlersBuilder.Handle<TValue>(
                valueName,
                value => (true, handler.Invoke(value)));
        }

        public static NamedValueHandlersBuilder Handle(
            this NamedValueHandlersBuilder namedValueHandlersBuilder,
            string valueName,
            Func<object, Type, object> handler
        )
        {
            return namedValueHandlersBuilder.Handle(
                valueName,
                (value, valueType) => (true, handler(value, valueType)));
        }

        public static NamedValueHandlersBuilder Handle(
            this NamedValueHandlersBuilder namedValueHandlersBuilder,
            string valueName,
            Func<object, Type, (bool, object)> handler
        )
        {
            return namedValueHandlersBuilder.Handle(
                namedValue =>
                {
                    var (name, value, valueType) = namedValue;
                    if (!string.Equals(valueName, name, StringComparison.InvariantCultureIgnoreCase))
                    {
                        return (false, value);
                    }

                    return handler(value, valueType);
                });
        }

        public static NamedValueHandlersBuilder Handle<TValue>(
            this NamedValueHandlersBuilder namedValueHandlersBuilder,
            string valueName,
            Func<TValue, (bool, object)> handler
        )
        {
            return namedValueHandlersBuilder.Handle(
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

        public static NamedValueHandlersBuilder Handle<TValue>(
            this NamedValueHandlersBuilder namedValueHandlersBuilder,
            Func<string, TValue, object> handler
        )
        {
            return namedValueHandlersBuilder.Handle<TValue>((name, value) => (true, handler(name, value)));
        }

        public static NamedValueHandlersBuilder Handle<TValue>(
            this NamedValueHandlersBuilder namedValueHandlersBuilder,
            Func<string, TValue, (bool IsHandled, object Value)> handler
        )
        {
            return namedValueHandlersBuilder.Handle(
                namedValue =>
                {
                    var (name, value, valueType) = namedValue;
                    if (handler == null
                        || typeof(TValue) != valueType
                        && typeof(TValue) != value.GetType())
                    {
                        return (false, value);
                    }

                    return handler(name, (TValue)value);
                });
        }

        public static NamedValueHandlersBuilder Handle(
            this NamedValueHandlersBuilder namedValueHandlersBuilder,
            Func<NamedValue, object> handler
        )
        {
            return namedValueHandlersBuilder.Handle(namedValue => (true, handler(namedValue)));
        }
    }
}
