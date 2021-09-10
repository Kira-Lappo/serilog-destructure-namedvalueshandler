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

                default: return TryDestructureObject(value, propertyValueFactory, out result);
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
                        var declaringType = pi.DeclaringType;
                        return (name, value, declaringType);
                    });

            return TryDestructureNamedValues(namedValues, propertyValueFactory, out result);
        }

        private bool TryDestructureDictionary(
            IDictionary dictionary,
            ILogEventPropertyValueFactory propertyValueFactory,
            out LogEventPropertyValue result
        )
        {
            var namedValues = new List<(string name, object value, Type declaringType)>();
            foreach (var key in dictionary.Keys)
            {
                var name = key.ToString();
                var value = dictionary[key];
                var declaringType = value.GetType().DeclaringType;
                namedValues.Add((name, value, declaringType));
            }

            return TryDestructureNamedValues(namedValues, propertyValueFactory, out result);
        }

        private bool TryDestructureNamedValues(
            IEnumerable<(string, object, Type)> namedValues,
            ILogEventPropertyValueFactory propertyValueFactory,
            out LogEventPropertyValue result
        )
        {
            var eventProperties = namedValues
                .Where(nv => !IsOmitted(nv))
                .Select(
                    nv =>
                    {
                        var (name, value, declaringType) = nv;
                        var handledValue = HandleNamedValue(name, value, declaringType);

                        return CreateEventPropertyValue(name, handledValue, propertyValueFactory);
                    })
                .ToList();

            result = new StructureValue(eventProperties);
            return true;
        }

        private bool IsOmitted((string name, object value, Type declaringType) _)
        {
            return _omitHandlers.Any(h => h.Invoke(_.name, _.value, _.declaringType));
        }

        private static LogEventProperty CreateEventPropertyValue(
            string name,
            object value,
            ILogEventPropertyValueFactory propertyValueFactory
        )
        {
            var eventValue = value == null
                ? new ScalarValue(value: null)
                : propertyValueFactory.CreatePropertyValue(value, destructureObjects: true);

            return new LogEventProperty(name, eventValue);
        }

        private object HandleNamedValue(string name, object value, Type declaringType)
        {
            name = UnifyName(name);
            if (_namedValueHandlers.TryGetValue(name, out var handler)) handler.Invoke(value, declaringType);

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
                    handlers[name] = handler;
                else
                    handlers.Add(name, handler);

                return this;
            }

            public NamedValueDestructuringPolicy Build()
            {
                return _policy;
            }

            public NamedValuePolicyBuilder WithOmitHandler(Func<string, object, Type, bool> omitHandler)
            {
                _policy._omitHandlers.Add(omitHandler);
                return this;
            }
        }
    }
}
