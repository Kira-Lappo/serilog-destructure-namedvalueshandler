using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Serilog.Core;
using Serilog.Debugging;
using Serilog.Events;

namespace Serilog.Destructure.NamedValuesHandler
{
    internal class NamedValueDestructuringPolicy : IDestructuringPolicy
    {
        private const string RootValueName = "root-value-object";
        public readonly List<Func<string, object, Type, bool>>                           OmitHandlers  = new();
        public readonly List<Func<string, object, Type, (bool IsHandled, object value)>> ValueHandlers = new();

        public bool TryDestructure(object value, ILogEventPropertyValueFactory propertyValueFactory, out LogEventPropertyValue result)
        {
            if (TryDestructureRootValue(value, propertyValueFactory, out result))
            {
                return true;
            }

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

                case IEnumerable:
                    result = null;
                    return false;

                default:
                    return TryDestructureObject(value, propertyValueFactory, out result);
            }
        }

        private bool TryDestructureRootValue(object value, ILogEventPropertyValueFactory propertyValueFactory, out LogEventPropertyValue result)
        {
            var type = value?.GetType() ?? typeof(object);
            if (!IsOmitted((RootValueName, value, type)))
            {
                var (isHandled, newValue) = HandleNamedValue(RootValueName, value, type);
                if (isHandled)
                {
                    result = propertyValueFactory.CreatePropertyValue(newValue, destructureObjects: true);
                    return true;
                }
            }

            result = null;
            return false;
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

            result = new StructureValue(logEventProperties, type.Name);
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
                        var name = propertyValueFactory.CreatePropertyValue(k, destructureObjects: true)
                            .ToString()
                            .Trim(trimChar: '"');

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
                        var (isHandled, handledValue) = HandleNamedValue(name, value, valueType);
                        var newValue = isHandled
                            ? handledValue
                            : value;

                        var logEventProperty = CreateEventPropertyValue(newValue, propertyValueFactory);
                        return (name, logEventProperty);
                    });
        }

        private bool IsOmitted((string name, object value, Type valueType) _)
        {
            return OmitHandlers.Any(
                h =>
                {
                    try
                    {
                        return h.Invoke(_.name, _.value, _.valueType);
                    }
                    catch (Exception e)
                    {
                        SelfLog.WriteLine($"Error at omit check, the value is not omitted. Name: {_.name} Type: {_.valueType}. Exception: {e}");
                        return false;
                    }
                });
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

        private (bool isHandled, object value) HandleNamedValue(string name, object value, Type valueType)
        {
            var handleResult = ValueHandlers
                .Select(
                    h =>
                    {
                        try
                        {
                            return h.Invoke(name, value, valueType);
                        }
                        catch (Exception e)
                        {
                            SelfLog.WriteLine($"Error at handling value, the value is not modified. Name: {name} Type: {valueType}. Exception: {e}");
                            return default;
                        }
                    })
                .FirstOrDefault(r => r.IsHandled);

            return handleResult;
        }
    }
}
