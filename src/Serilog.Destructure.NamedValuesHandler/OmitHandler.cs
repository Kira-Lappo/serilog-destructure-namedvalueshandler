using System;
using System.Collections.Generic;
using System.Linq;
using Serilog.Debugging;

namespace Serilog.Destructure.NamedValuesHandler
{
    internal class OmitHandler
    {
        private readonly List<Func<NamedValue, bool>> _omitHandlers = new();

        public void AddHandler(Func<NamedValue, bool> handler)
        {
            _omitHandlers.Add(handler);
        }

        public bool IsOmitted(NamedValue namedValue)
        {
            return _omitHandlers.Any(h => IsOmitted(h, namedValue));
        }

        private static bool IsOmitted(Func<NamedValue, bool> handler, NamedValue namedValue)
        {
            try
            {
                return handler.Invoke(namedValue);
            }
            catch (Exception e)
            {
                SelfLog.WriteLine($"Error at omit check, the value is not omitted. Name: {namedValue.Name} Type: {namedValue.ValueType}. Exception: {e}");
                return false;
            }
        }
    }
}
