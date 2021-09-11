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
        private readonly List<Func<string, object, Type, (bool IsHandled, object value)>> _namedValueHandlers = new();
        private readonly List<Func<string, object, Type, bool>>                           _omitHandlers       = new();

        public bool TryDestructure(object value, ILogEventPropertyValueFactory propertyValueFactory, out LogEventPropertyValue result)
        {
            switch (value)
            {
                case null:
                case DateTime: // Todo [2021/09/10 KL] Need to figure it our a better way to ignore non-key-value structures
                case Guid:
                case Enum:
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
            var handleResult = _namedValueHandlers
                .Select(h => h.Invoke(name, value, valueType))
                .FirstOrDefault(r => r.IsHandled);

            return handleResult == default
                ? value
                : handleResult.value;
        }

        private static string UnifyName(string name)
        {
            return name.ToLower();
        }

        public class NamedValuePolicyBuilder
        {
            private readonly NamedValueDestructuringPolicy _policy = new();

            public NamedValuePolicyBuilder WithNamedValueHandler(Func<string, object, Type, (bool IsHandled, object value)> handler)
            {
                _policy._namedValueHandlers.Add(handler);
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
