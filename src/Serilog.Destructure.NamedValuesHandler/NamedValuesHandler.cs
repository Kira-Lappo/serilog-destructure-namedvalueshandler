using System;
using System.Collections.Generic;
using System.Linq;
using Serilog.Debugging;

namespace Serilog.Destructure.NamedValuesHandler
{
    internal class NamedValuesHandler
    {
        private readonly List<Func<NamedValue, HandledValue>> _valueHandlers = new();

        public void AddHandler(Func<NamedValue, HandledValue> handler)
        {
            _valueHandlers.Add(handler);
        }

        public HandledValue HandleNamedValue(NamedValue namedValue)
        {
            var handleResult = _valueHandlers
                .Select(h => HandleNamedValue(h, namedValue))
                .FirstOrDefault(r => r.IsHandled);

            return handleResult;
        }

        private static HandledValue HandleNamedValue(
            Func<NamedValue, HandledValue> handler,
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
