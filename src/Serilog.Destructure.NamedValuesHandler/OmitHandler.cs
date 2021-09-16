using System;
using System.Collections.Generic;
using System.Linq;
using Serilog.Debugging;

namespace Serilog.Destructure.NamedValuesHandler
{
    public class OmitHandler
    {
        private readonly List<Func<string, object, Type, bool>> _omitHandlers = new();

        public void AddHandler(Func<string, object, Type, bool> handler)
        {
            _omitHandlers.Add(handler);
        }

        public bool IsOmitted((string name, object value, Type valueType) namedValue)
        {
            return _omitHandlers.Any(h => IsOmitted(h, namedValue));
        }

        private static bool IsOmitted(Func<string, object, Type, bool> handler, (string, object, Type) namedValue)
        {
            var (name, value, valueType) = namedValue;
            try
            {
                return handler.Invoke(name, value, valueType);
            }
            catch (Exception e)
            {
                SelfLog.WriteLine($"Error at omit check, the value is not omitted. Name: {name} Type: {valueType}. Exception: {e}");
                return false;
            }
        }
    }
}
