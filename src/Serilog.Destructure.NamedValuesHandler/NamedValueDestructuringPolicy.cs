using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Serilog.Core;
using Serilog.Events;

namespace Serilog.Destructure.NamedValuesHandler
{
    public class NamedValueDestructuringPolicy : IDestructuringPolicy
    {
        private readonly Dictionary<string, Func<object, Type, object>> _namedValueHandlers = new();
        private readonly List<Func<string, object, Type, bool>>         _omitHandlers       = new();

        public bool TryDestructure(object value, ILogEventPropertyValueFactory propertyValueFactory, out LogEventPropertyValue result)
        {
            switch (value)
            {
                case null:
                case DateTime: // Todo [2021/09/10 KL] Need to figure it our a better
                    result = null;
                    return false;

                case IDictionary dictionary:
                    return TryDestructureDictionary(dictionary, propertyValueFactory, out result);

                case IEnumerable: // Todo [2021/09/10 KL] Ignoring any other enumerable for a while
                    result = null;
                    return false;

                default:
                    return TryDestructureObject(value, propertyValueFactory, out result);
            }
        }

        private bool TryDestructureObject(
            object objectValue,
            ILogEventPropertyValueFactory propertyValueFactory,
            out LogEventPropertyValue result
        )
        {
            var type = objectValue.GetType();
            var propertyInfos = type.GetProperties();
            if (propertyInfos.Length <= 0)
            {
                result = null;
                return false;
            }

            var namedValues = propertyInfos
                .Select(
                    pi =>
                    {
                        var name = pi.Name;
                        var value = pi.GetValue(objectValue);
                        var valueType = pi.PropertyType;
                        return (name, value, valueType);
                    });

            var logEventProperties = DestructureNamedValues(namedValues, propertyValueFactory)
                .Select(_ => new LogEventProperty(_.name, _.logEventValue));

            result = new StructureValue(logEventProperties);
            return true;
        }

        private bool TryDestructureDictionary(
            IDictionary dictionary,
            ILogEventPropertyValueFactory propertyValueFactory,
            out LogEventPropertyValue result
        )
        {
            var namedValues = dictionary.Keys
                .Cast<object>()
                .Select(
                    k =>
                    {
                        var name = k.ToString();
                        var value = dictionary[k];
                        var valueType = value.GetType();
                        return (name, value, valueType);
                    });

            var logEventProperties = DestructureNamedValues(namedValues, propertyValueFactory)
                .Select(_ => new KeyValuePair<ScalarValue, LogEventPropertyValue>(new ScalarValue(_.name), _.logEventValue));

            result = new DictionaryValue(logEventProperties);
            return true;
        }

        private IEnumerable<(string name, LogEventPropertyValue logEventValue)> DestructureNamedValues(
            IEnumerable<(string, object, Type)> namedValues,
            ILogEventPropertyValueFactory propertyValueFactory
        )
        {
            return namedValues
                .Where(nv => !IsOmitted(nv))
                .Select(
                    nv =>
                    {
                        var (name, value, valueType) = nv;
                        var handledValue = HandleNamedValue(name, value, valueType);
                        var logEventProperty = CreateEventPropertyValue(handledValue, propertyValueFactory);
                        return (name, logEventProperty);
                    });
        }

        private bool IsOmitted((string name, object value, Type valueType) _)
        {
            return _omitHandlers.Any(h => h.Invoke(_.name, _.value, _.valueType));
        }

        private static LogEventPropertyValue CreateEventPropertyValue(
            object value,
            ILogEventPropertyValueFactory propertyValueFactory
        )
        {
            return value == null
                ? new ScalarValue(value: null)
                : propertyValueFactory.CreatePropertyValue(value, destructureObjects: true);
        }

        private object HandleNamedValue(string name, object value, Type valueType)
        {
            name = UnifyName(name);
            if (_namedValueHandlers.TryGetValue(name, out var handler))
            {
                return handler.Invoke(value, valueType);
            }

            return value;
        }

        private static string UnifyName(string name)
        {
            return name.ToLower();
        }

        public class NamedValuePolicyBuilder
        {
            private readonly NamedValueDestructuringPolicy _policy = new();

            public NamedValuePolicyBuilder WithNamedValueHandler(string name, Func<object, Type, object> handler)
            {
                name = UnifyName(name);
                var handlers = _policy._namedValueHandlers;
                if (handlers.ContainsKey(name))
                {
                    handlers[name] = handler;
                }
                else
                {
                    handlers.Add(name, handler);
                }

                return this;
            }

            public NamedValuePolicyBuilder WithOmitHandler(Func<string, object, Type, bool> omitHandler)
            {
                _policy._omitHandlers.Add(omitHandler);
                return this;
            }

            public NamedValueDestructuringPolicy Build()
            {
                return _policy;
            }
        }
    }
}
