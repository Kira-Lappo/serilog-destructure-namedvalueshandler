using System;
using Serilog.Configuration;

namespace Serilog.Destructure.NamedValuesHandler
{
    public static class SerilogExtensions
    {
        public static LoggerConfiguration HandleValues(
            this LoggerDestructuringConfiguration configuration,
            Action<NamedValuePolicyBuilder> policyConfiguration
        )
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            if (policyConfiguration == null)
            {
                throw new ArgumentNullException(nameof(policyConfiguration));
            }

            var policyBuilder = new NamedValuePolicyBuilder();
            policyConfiguration.Invoke(policyBuilder);
            var policy = policyBuilder.Build();

            return configuration.With(policy);
        }
    }
}
