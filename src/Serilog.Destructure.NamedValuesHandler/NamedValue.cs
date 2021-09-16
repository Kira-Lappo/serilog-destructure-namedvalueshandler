using System;
using System.Diagnostics.CodeAnalysis;

namespace Serilog.Destructure.NamedValuesHandler
{
    public readonly struct NamedValue
    {
        public readonly string Name;

        [AllowNull] public readonly object Value;

        [AllowNull] public readonly Type ValueType;

        public NamedValue(string name, object value = null, Type valueType = null)
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
