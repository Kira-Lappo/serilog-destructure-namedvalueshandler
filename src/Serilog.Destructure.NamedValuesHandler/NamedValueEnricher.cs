using System.Linq;
using Serilog.Core;
using Serilog.Events;

namespace Serilog.Destructure.NamedValuesHandler
{
    internal class NamedValueEnricher : ILogEventEnricher
    {
        private readonly NamedValuesHandler _namedValuesHandler;
        private readonly OmitHandler        _omitHandler;

        public NamedValueEnricher(NamedValuesHandler namedValuesHandler, OmitHandler omitHandler)
        {
            _namedValuesHandler = namedValuesHandler;
            _omitHandler        = omitHandler;
        }

        // Unfortunately we can't detect original value and type of property here - it is an after deconstructing stage
        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            OmitProperties(logEvent);
            HandleNamedValues(logEvent, propertyFactory);
        }

        private void OmitProperties(LogEvent logEvent)
        {
            var properties = logEvent.Properties;
            var propertyToIgnore = properties
                .Where(p => _omitHandler.IsOmitted(new NamedValue(p.Key)));

            foreach (var (key, _) in propertyToIgnore)
            {
                logEvent.RemovePropertyIfPresent(key);
            }
        }

        private void HandleNamedValues(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            var properties = logEvent.Properties;
            foreach (var (key, _) in properties)
            {
                var (isHandled, newValue) = _namedValuesHandler.HandleNamedValue(new NamedValue(key));
                if (isHandled)
                {
                    var newProperty = propertyFactory.CreateProperty(key, newValue, destructureObjects: true);
                    logEvent.AddOrUpdateProperty(newProperty);
                }
            }
        }
    }
}
