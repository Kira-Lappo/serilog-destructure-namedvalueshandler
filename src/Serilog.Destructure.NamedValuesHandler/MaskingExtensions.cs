﻿namespace Serilog.Destructure.NamedValuesHandler
{
    public static class MaskingExtensions
    {
        private const char DefaultMaskChar = '*';

        public static NamedValueDestructuringPolicy.NamedValuePolicyBuilder Mask(
            this NamedValueDestructuringPolicy.NamedValuePolicyBuilder namedValuePolicyBuilder,
            string name,
            int? visibleCharsAmount = null,
            char maskChar = DefaultMaskChar
        )
        {
            return namedValuePolicyBuilder.Handle(
                name,
                (value, type) => (value?.ToString()).Mask(visibleCharsAmount, maskChar));
        }

        internal static string Mask(this string value, int? visibleCharsAmount = null, char maskChar = DefaultMaskChar)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return value;
            }

            if (!visibleCharsAmount.HasValue)
            {
                return new string(maskChar, value.Length);
            }

            if (value.Length <= visibleCharsAmount)
            {
                return value;
            }

            return $"{new string(maskChar, value.Length - visibleCharsAmount.Value)}{value[^visibleCharsAmount.Value..]}";
        }
    }
}
