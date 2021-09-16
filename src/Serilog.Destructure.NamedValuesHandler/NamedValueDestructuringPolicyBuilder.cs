using System;

namespace Serilog.Destructure.NamedValuesHandler
{
    public class NamedValueDestructuringPolicyBuilder
    {
        private readonly OmitHandler _omitHandler = new();
        private readonly NamedValuesHandler _namedValuesHandler = new();

        public NamedValueDestructuringPolicyBuilder Handle(Func<string, object, Type, (bool IsHandled, object value)> handler)
        {
            _namedValuesHandler.AddHandler(handler);
            return this;
        }

        public NamedValueDestructuringPolicyBuilder Omit(Func<string, object, Type, bool> handler)
        {
            _omitHandler.AddHandler(handler);
            return this;
        }

        internal NamedValueDestructuringPolicy Build()
        {
            return new(_namedValuesHandler, _omitHandler);
        }
    }
}
