using System;
using System.Diagnostics.CodeAnalysis;

namespace Serilog.Destructure.NamedValuesHandler
{
    public readonly struct NamedValue
    {
        [MaybeNull] public readonly string Name;

        [MaybeNull] public readonly object Value;

        [MaybeNull] public readonly Type ValueType;

        public NamedValue(string name = null, object value = null, Type valueType = null)
        {
            Name      = name;
            Value     = value;
            ValueType = valueType;
        }

        public void Deconstruct(out string name, out object value, out Type valueType)
        {
            name      = Name;
            value     = Value;
            valueType = ValueType;
        }
    }
}
