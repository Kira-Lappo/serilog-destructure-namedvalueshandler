using System.Collections.Generic;
using Serilog.Events;

namespace Serilog.Destructure.NamedValuesHandler.Tests.Sinks
{
    public interface ILogEventsProvider
    {
        List<LogEvent> GetLogEvents();
    }
}
