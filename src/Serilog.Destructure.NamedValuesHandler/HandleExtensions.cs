using System;
using System.Diagnostics.CodeAnalysis;

namespace Serilog.Destructure.NamedValuesHandler
{
    public static class HandleExtensions
    {
        public static NamedValueHandlersBuilder Handle<TValue>(
            this NamedValueHandlersBuilder namedValueHandlersBuilder,
            [MaybeNull]string valueName,
            Func<TValue, object> handler
        )
        {
            return namedValueHandlersBuilder.Handle<TValue>(
                valueName,
                value => new HandledValue(isHandled: true, handler.Invoke(value)));
        }

        public static NamedValueHandlersBuilder Handle(
            this NamedValueHandlersBuilder namedValueHandlersBuilder,
            [MaybeNull]string valueName,
            Func<object, Type, object> handler
        )
        {
            return namedValueHandlersBuilder.Handle(
                valueName,
                (value, valueType) => new HandledValue(isHandled: true, handler(value, valueType)));
        }

        public static NamedValueHandlersBuilder Handle(
            this NamedValueHandlersBuilder namedValueHandlersBuilder,
            [MaybeNull]string valueName,
            Func<object, Type, HandledValue> handler
        )
        {
            return namedValueHandlersBuilder.Handle(
                namedValue =>
                {
                    var (name, value, valueType) = namedValue;
                    if (!string.Equals(valueName, name, StringComparison.InvariantCultureIgnoreCase))
                    {
                        return new HandledValue(isHandled: false, value);
                    }

                    return handler(value, valueType);
                });
        }

        public static NamedValueHandlersBuilder Handle<TValue>(
            this NamedValueHandlersBuilder namedValueHandlersBuilder,
            [MaybeNull]string valueName,
            Func<TValue, HandledValue> handler
        )
        {
            return namedValueHandlersBuilder.Handle(
                valueName,
                (value, valueType) =>
                {
                    if (handler == null
                        || typeof(TValue) != valueType
                        && typeof(TValue) != value?.GetType())
                    {
                        return new HandledValue(isHandled: false, value);
                    }

                    return handler((TValue)value);
                });
        }

        public static NamedValueHandlersBuilder Handle<TValue>(
            this NamedValueHandlersBuilder namedValueHandlersBuilder,
            Func<string, TValue, object> handler
        )
        {
            return namedValueHandlersBuilder.Handle<TValue>((name, value) => new HandledValue(isHandled: true, handler(name, value)));
        }

        public static NamedValueHandlersBuilder Handle<TValue>(
            this NamedValueHandlersBuilder namedValueHandlersBuilder,
            Func<string, TValue, HandledValue> handler
        )
        {
            return namedValueHandlersBuilder.Handle(
                namedValue =>
                {
                    var (name, value, valueType) = namedValue;
                    if (handler == null
                        || typeof(TValue) != valueType
                        && typeof(TValue) != value?.GetType())
                    {
                        return new HandledValue(isHandled: false, value);
                    }

                    return handler(name, (TValue)value);
                });
        }

        public static NamedValueHandlersBuilder Handle(
            this NamedValueHandlersBuilder namedValueHandlersBuilder,
            Func<NamedValue, object> handler
        )
        {
            return namedValueHandlersBuilder.Handle(namedValue => new HandledValue(isHandled: true, handler(namedValue)));
        }
    }
}
