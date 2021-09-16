using System;

namespace Serilog.Destructure.NamedValuesHandler
{
    public class NamedValueDestructuringPolicyBuilder
    {
        private readonly NamedValueDestructuringPolicy _policy = new();

        public NamedValueDestructuringPolicyBuilder Handle(Func<string, object, Type, (bool IsHandled, object value)> handler)
        {
            _policy.ValueHandlers.Add(handler);
            return this;
        }

        public NamedValueDestructuringPolicyBuilder Omit(Func<string, object, Type, bool> omitHandler)
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
