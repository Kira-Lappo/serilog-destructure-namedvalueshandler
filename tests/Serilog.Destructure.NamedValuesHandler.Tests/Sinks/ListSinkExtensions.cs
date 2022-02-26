using Serilog.Configuration;
using Serilog.Core;
using Serilog.Events;

namespace Serilog.Destructure.NamedValuesHandler.Tests.Sinks
{
    public static class ListSinkExtensions
    {
        public static LoggerConfiguration Memory(
            this LoggerSinkConfiguration sinkConfiguration,
            out ILogEventsProvider logEventsProviderProvider,
            LogEventLevel restrictedToMinimumLevel = LevelAlias.Minimum,
            LoggingLevelSwitch levelSwitch = null)
        {
            var sink = new MemorySink();
            logEventsProviderProvider = sink;
            return sinkConfiguration.Sink(sink, restrictedToMinimumLevel, levelSwitch);
        }
    }
}
