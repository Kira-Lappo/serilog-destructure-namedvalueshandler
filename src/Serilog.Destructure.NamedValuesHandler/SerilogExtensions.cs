using System;

namespace Serilog.Destructure.NamedValuesHandler
{
    public static class SerilogExtensions
    {
        public static LoggerConfiguration HandleValues(
            this LoggerConfiguration configuration,
            Action<NamedValueHandlersBuilder> destructureConfiguration
        )
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            var handlersBuilder = new NamedValueHandlersBuilder();
            destructureConfiguration?.Invoke(handlersBuilder);

            var policy = handlersBuilder.BuildDestructuringPolicy();
            configuration.Destructure.With(policy);

            var enricher = handlersBuilder.BuildEnricher();
            configuration.Enrich.With(enricher);

            return configuration;
        }
    }
}
