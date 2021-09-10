namespace Serilog.Destructure.NamedValuesHandler
{
    public static class MaskingExtensions
    {
        public static NamedValueDestructuringPolicy.NamedValuePolicyBuilder MaskStringValue(
            this NamedValueDestructuringPolicy.NamedValuePolicyBuilder namedValuePolicyBuilder,
            string name,
            int? visibleCharsAmount = null,
            char maskChar = '#'
        )
        {
            return namedValuePolicyBuilder.HandleNamedValue<string>(
                name,
                value =>
                    value?.MaskValue(visibleCharsAmount, maskChar));
        }

        public static string MaskValue(this string value, int? visibleCharsAmount = null, char maskChar = '#')
        {
            if (string.IsNullOrWhiteSpace(value)) return value;

            if (!visibleCharsAmount.HasValue) return new string(maskChar, value.Length);

            if (value.Length <= visibleCharsAmount) return value;

            return $"{new string(maskChar, value.Length - visibleCharsAmount.Value)}{value[^visibleCharsAmount.Value..]}";
        }
    }
}
