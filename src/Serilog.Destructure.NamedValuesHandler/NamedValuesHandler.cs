using System;
using System.Collections.Generic;
using System.Linq;
using Serilog.Debugging;

namespace Serilog.Destructure.NamedValuesHandler
{
    internal class NamedValuesHandler
    {
        private readonly List<Func<string, object, Type, (bool IsHandled, object value)>> _valueHandlers = new();

        public void AddHandler(Func<string, object, Type, (bool IsHandled, object value)> handler)
        {
            _valueHandlers.Add(handler);
        }

        public (bool isHandled, object value) HandleNamedValue((string, object, Type) namedValue)
        {
            var handleResult = _valueHandlers
                .Select(h => HandleNamedValue(h, namedValue))
                .FirstOrDefault(r => r.IsHandled);

            return handleResult;
        }

        private static (bool IsHandled, object value) HandleNamedValue(
            Func<string, object, Type, (bool IsHandled, object value)> handler,
            (string, object, Type) namedValue
        )
        {
            var (name, value, valueType) = namedValue;
            try
            {
                return handler.Invoke(name, value, valueType);
            }
            catch (Exception e)
            {
                SelfLog.WriteLine($"Error at handling value, the value is not modified. Name: {name} Type: {valueType}. Exception: {e}");
                return default;
            }
        }
    }
}
