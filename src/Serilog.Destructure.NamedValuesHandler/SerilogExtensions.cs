using System;
using Serilog.Configuration;

namespace Serilog.Destructure.NamedValuesHandler
{
    public static class SerilogExtensions
    {
        public static LoggerConfiguration HandleValues(
            this LoggerConfiguration configuration,
            Action<NamedValueDestructuringPolicyBuilder> destructureConfiguration
        )
        {
            configuration.Destructure.HandleValues(destructureConfiguration);
            return configuration;
        }

        internal static LoggerConfiguration HandleValues(
            this LoggerDestructuringConfiguration configuration,
            Action<NamedValueDestructuringPolicyBuilder> destructureConfiguration
        )
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            var policyBuilder = new NamedValueDestructuringPolicyBuilder();
            destructureConfiguration?.Invoke(policyBuilder);
            var policy = policyBuilder.Build();

            return configuration.With(policy);
        }
    }
}
