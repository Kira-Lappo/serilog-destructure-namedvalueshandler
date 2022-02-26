using System;

namespace Serilog.Destructure.NamedValuesHandler
{
    public class NamedValueHandlersBuilder
    {
        private readonly OmitHandler        _omitHandler        = new();
        private readonly NamedValuesHandler _namedValuesHandler = new();

        public NamedValueHandlersBuilder Handle(Func<NamedValue, HandledValue> handler)
        {
            _namedValuesHandler.AddHandler(handler);
            return this;
        }

        public NamedValueHandlersBuilder Omit(Func<NamedValue, bool> handler)
        {
            _omitHandler.AddHandler(handler);
            return this;
        }

        internal NamedValueDestructuringPolicy BuildDestructuringPolicy()
        {
            return new NamedValueDestructuringPolicy(_namedValuesHandler, _omitHandler);
        }

        internal NamedValueEnricher BuildEnricher()
        {
            return new NamedValueEnricher(_namedValuesHandler, _omitHandler);
        }
    }
}
