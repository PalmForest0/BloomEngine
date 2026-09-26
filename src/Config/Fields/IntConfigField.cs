using System.Globalization;
using System.Text;
using BloomEngine.Config.Fields.Base;
using BloomEngine.UI;
using Il2CppReloaded.Input;
using MelonLoader;
using UnityEngine;

namespace BloomEngine.Config.Fields;

/// <summary>
/// A config field which displays and processes an <see cref="int"/> value using a numeric textbox.
/// To create an <see cref="IntConfigField"/>, use <see cref="ConfigService.CreateInt(string, string, int)"/>
/// </summary>
public sealed class IntConfigField : TypedConfigField<int, IntConfigField>
{
    /// <summary>
    /// The UI textbox which corresponds to this config field in the config panel.
    /// </summary>
    public ReloadedInputField Textbox { get; private set; } = null!;

    internal IntConfigField(string name, string description, int defaultValue) : base(name, description, defaultValue) { }

    /// <inheritdoc/>
    protected internal override GameObject CreateInputObject(RectTransform parent, string name)
    {
        Textbox = UIHelper.CreateTextField(name, parent, ValueType.Name, onTextChanged: _ => HandleInputChanged());
        return Textbox.gameObject;
    }

    /// <inheritdoc/>
    protected internal override void ApplyInput() => Value = (int)ValidateNumericInput(Textbox.text, typeof(int));

    /// <inheritdoc/>
    protected override void SetDisplayedValue(int value) => Textbox.SetTextWithoutNotify(value.ToString(CultureInfo.InvariantCulture));

    /// <inheritdoc/>
    internal override void HandleInputChanged()
    {
        // Perform basic sanitization on live input change
        Textbox.SetTextWithoutNotify(SanitizeNumericInput(Textbox.text));
        base.HandleInputChanged();
    }

    /// <summary>
    /// Performs basic sanitization of numeric input strings to be used live for config fields. Does not perform clamping, parsing or type conversion.
    /// </summary>
    private static string SanitizeNumericInput(string input) => new([.. input.Where(c => char.IsDigit(c) || c == '-' || c == '+' || c == '.')]);

    /// <summary>
    /// Performs full validation of an input string for a numeric type, including sanitization, parsing and clamping to the type's min/max values.
    /// </summary>
    private static object ValidateNumericInput(string input, Type type)
    {
        if (!IsNumericType(type))
        {
            Melon<BloomEngineMod>.Logger.Error($"{type} is not a numeric type. This should never occur.");
            return input;
        }

        input = FormatNumericString(input);

        if (string.IsNullOrWhiteSpace(input) || input == "-" || input == "+" || input == ".")
            return Convert.ChangeType(0, type, CultureInfo.InvariantCulture);

        var minField = type.GetField("MinValue");
        var maxField = type.GetField("MaxValue");

        double min = Convert.ToDouble(minField?.GetValue(null) ?? 0, CultureInfo.InvariantCulture);
        double max = Convert.ToDouble(maxField?.GetValue(null) ?? byte.MaxValue, CultureInfo.InvariantCulture);
        double val = double.Parse(input, CultureInfo.InvariantCulture);

        return Convert.ChangeType(Math.Clamp(val, min, max), type, CultureInfo.InvariantCulture);
    }

    /// <summary>
    /// Ensures that a given string is a valid numeric representation for the specified numeric type and can be parsed.
    /// </summary>
    private static string FormatNumericString(string input)
    {
        var sb = new StringBuilder();
        bool hasDecimal = false;
        bool hasSign = false;

        for (int i = 0; i < input.Length; i++)
        {
            char c = input[i];

            // Allow + or - sign at te start if the type is signed
            if (c is '-' or '+' && i == 0 && !hasSign)
            {
                sb.Append(c);
                hasSign = true;
            }
            // Decimal point if one doesn't already exist and the type allows it
            else if (c == '.' && !hasDecimal)
            {
                sb.Append('.');
                hasDecimal = true;
            }
            // Finally allow all digit characters
            else if (char.IsDigit(c))
            {
                sb.Append(c);
            }
        }

        return sb.ToString();
    }

    /// <summary>
    /// Determines whether the specified type is one of the most common numeric types.
    /// </summary>
    /// <param name="type">The type to check.</param>
    /// <returns>True if the provided type is numeric.</returns>
    private static bool IsNumericType(Type type) => Type.GetTypeCode(type) switch
    {
        TypeCode.Byte => true,
        TypeCode.SByte => true,
        TypeCode.UInt16 => true,
        TypeCode.UInt32 => true,
        TypeCode.UInt64 => true,
        TypeCode.Int16 => true,
        TypeCode.Int32 => true,
        TypeCode.Int64 => true,
        TypeCode.Decimal => true,
        TypeCode.Double => true,
        TypeCode.Single => true,
        _ => false
    };
}