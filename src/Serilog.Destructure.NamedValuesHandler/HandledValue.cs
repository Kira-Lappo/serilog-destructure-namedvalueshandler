using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace Serilog.Destructure.NamedValuesHandler
{
    public readonly struct HandledValue
    {
        public readonly bool IsHandled;

        [MaybeNull] public readonly object Value;

        public HandledValue(bool isHandled, object value)
        {
            IsHandled = isHandled;
            Value = value;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void Deconstruct(out bool isHandled, out object value)
        {
            isHandled = IsHandled;
            value     = Value;
        }
    }
}
