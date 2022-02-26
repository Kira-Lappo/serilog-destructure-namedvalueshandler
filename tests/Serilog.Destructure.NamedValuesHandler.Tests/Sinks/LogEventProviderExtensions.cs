using System;
using System.Collections.Generic;
using System.Linq;
using Serilog.Events;

namespace Serilog.Destructure.NamedValuesHandler.Tests.Sinks
{
    public static class LogEventProviderExtensions
    {
        public static List<LogEventPropertyValue> SelectAllPropertyValues(this IEnumerable<LogEvent> logEvents, string propertyName)
        {
            var logEvent = logEvents
                .SelectMany(e => e.Properties)
                .Where(p => string.Equals(p.Key, propertyName, StringComparison.OrdinalIgnoreCase))
                .Select(p => p.Value)
                .ToList();

            return logEvent;
        }
    }
}
