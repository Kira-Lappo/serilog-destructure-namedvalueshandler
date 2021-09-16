using System;
using System.Collections.Generic;
using System.Linq;
using Serilog.Debugging;

namespace Serilog.Destructure.NamedValuesHandler
{
    internal class NamedValuesHandler
    {
        private readonly List<Func<NamedValue, (bool IsHandled, object Value)>> _valueHandlers = new();

        public void AddHandler(Func<NamedValue, (bool IsHandled, object Value)> handler)
        {
            _valueHandlers.Add(handler);
        }

        public (bool IsHandled, object Value) HandleNamedValue(NamedValue namedValue)
        {
            var handleResult = _valueHandlers
                .Select(h => HandleNamedValue(h, namedValue))
                .FirstOrDefault(r => r.IsHandled);

            return handleResult;
        }

        private static (bool IsHandled, object Value) HandleNamedValue(
            Func<NamedValue, (bool IsHandled, object Value)> handler,
            NamedValue namedValue
        )
        {
            try
            {
                return handler.Invoke(namedValue);
            }
            catch (Exception e)
            {
                SelfLog.WriteLine($"Error at handling value, the value is not modified. Name: {namedValue.Name} Type: {namedValue.ValueType}. Exception: {e}");
                return default;
            }
        }
    }
}
