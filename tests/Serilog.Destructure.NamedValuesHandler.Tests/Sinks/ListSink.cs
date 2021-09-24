using System.Collections.Generic;
using Serilog.Core;
using Serilog.Events;

namespace Serilog.Destructure.NamedValuesHandler.Tests.Sinks
{
    public class ListSink : ILogEventSink, ILogEventsProvider
    {
        private readonly List<LogEvent> _logEvents = new();

        public void Emit(LogEvent logEvent)
        {
            _logEvents.Add(logEvent);
        }

        public List<LogEvent> GetLogEvents()
        {
            return _logEvents;
        }
    }
}
