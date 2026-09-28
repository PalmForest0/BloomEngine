using System.Globalization;
using System.Text;

namespace BloomEngine.Helpers;

public static class StringHelper
{
    /// <summary>
    /// Common numeric types that have their own TypeCode.
    /// </summary>
    private static readonly TypeCode[] NumericTypes =
    [
        TypeCode.Byte,
        TypeCode.SByte,
        TypeCode.UInt16,
        TypeCode.UInt32,
        TypeCode.UInt64,
        TypeCode.Int16,
        TypeCode.Int32,
        TypeCode.Int64,
        TypeCode.Decimal,
        TypeCode.Double,
        TypeCode.Single
    ];
    
    /// <summary>
    /// Performs basic sanitization of numeric input strings intended to be used for live input fields. Does not perform clamping, parsing or type conversion.
    /// </summary>
    /// <param name="input">The input string to perform sanitization on.</param>
    /// <returns>The input string, excluding any characters that are not a digit, <c>+</c>, <c>0</c> or <c>.</c>.</returns>
    public static string SanitizeNumericInput(string input) => new(input.Where(c => char.IsDigit(c) || c == '-' || c == '+' || c == '.').ToArray());

    /// <summary>
    /// Performs full validation of an input string for a given numeric type, including sanitization, parsing and clamping to the type's range.
    /// </summary>
    /// <param name="input">The input string to perform the full validation process on.</param>
    /// <param name="type">The numeric type that the input should be parsed to.</param>
    /// <returns>The parsed number as an object of the provided numeric type, or 0 in the case of failure.</returns>
    public static object ValidateNumericInput(string input, Type type)
    {
        if (!NumericTypes.Contains(Type.GetTypeCode(type)))
            throw new ArgumentException($"{type.FullName} is not supported numeric type.");

        input = FormatNumericString(input);

        if(!double.TryParse(input, NumberStyles.Float, CultureInfo.InvariantCulture, out double value))
            return Convert.ChangeType(0, type, CultureInfo.InvariantCulture);

        // Handle values below the minimum
        var minField = type.GetField("MinValue")!;
        double min = Convert.ToDouble(minField.GetValue(null), CultureInfo.InvariantCulture);
        if (value <= min) return minField.GetValue(null)!;
        
        // Handle values above the maximum
        var maxField = type.GetField("MaxValue")!;
        double max = Convert.ToDouble(maxField.GetValue(null), CultureInfo.InvariantCulture);
        if (value >= max) return maxField.GetValue(null)!;

        return Convert.ChangeType(value, type, CultureInfo.InvariantCulture);
    }

    /// <summary>
    /// Sanitizes a string for numeric parsing by keeping only digits, at most one decimal point, and an optional sign.
    /// </summary>
    /// <param name="input">Input string that needs to be formatted for parsing.</param>
    /// <returns>An output string which should only contain valid characters in correct positions, but is not guaranteed to be parsable.</returns>
    private static string FormatNumericString(string input)
    {
        var sb = new StringBuilder(input.Length);
        bool hasDecimal = false;

        for (int i = 0; i < input.Length; i++)
        {
            char c = input[i];

            switch (c)
            {
                // A sign is only valid as the first character
                case '+' or '-' when i == 0:    
                    sb.Append(c);
                    break;
                // Only the first decimal point is kept
                case '.' when !hasDecimal:      
                    sb.Append(c);
                    hasDecimal = true;
                    break;
                // Only ASCII digits are accepted
                case >= '0' and <= '9':         
                    sb.Append(c);
                    break;
            }
        }

        return sb.ToString();
    }

    /// <summary>
    /// Converts a string that would show up in code to a readable display string.
    /// </summary>
    /// <param name="input">Input string that should be processed.</param>
    /// <returns>A readable display string.</returns>
    public static string StringToReadable(string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        var sb = new StringBuilder();
        sb.Append(char.ToUpper(input[0], CultureInfo.InvariantCulture));

        for (int i = 1; i < input.Length; i++)
        {
            char current = input[i];
            char prev = input[i - 1];
            bool lastCharIsSpace = sb.Length > 0 && sb[^1] == ' ';

            if (current is '_' or '-')
            {
                if (!lastCharIsSpace)
                    sb.Append(' ');
                continue;
            }

            if (!lastCharIsSpace)
            {
                // Splits words from lowercase to uppercase
                if (char.IsUpper(current) && !char.IsUpper(prev))
                    sb.Append(' ');
                // Splits words between two uppercase
                else if (char.IsUpper(current) && char.IsUpper(prev) && i + 1 < input.Length && char.IsLower(input[i + 1]))
                    sb.Append(' ');
                // Splits digits before
                else if (char.IsDigit(current) && !char.IsDigit(prev))
                    sb.Append(' ');
                // Splits digits after
                else if (!char.IsDigit(current) && char.IsDigit(prev))
                    sb.Append(' ');
            }

            sb.Append(current);
        }

        return sb.ToString().TrimEnd();
    }
}