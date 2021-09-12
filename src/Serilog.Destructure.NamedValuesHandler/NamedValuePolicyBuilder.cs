using System;

namespace Serilog.Destructure.NamedValuesHandler
{
    public class NamedValuePolicyBuilder
    {
        private readonly NamedValueDestructuringPolicy _policy = new();

        public NamedValuePolicyBuilder Handle(Func<string, object, Type, (bool IsHandled, object value)> handler)
        {
            _policy.ValueHandlers.Add(handler);
            return this;
        }

        public NamedValuePolicyBuilder WithOmitHandler(Func<string, object, Type, bool> omitHandler)
        {
            _policy.OmitHandlers.Add(omitHandler);
            return this;
        }

        internal NamedValueDestructuringPolicy Build()
        {
            return _policy;
        }
    }
}
