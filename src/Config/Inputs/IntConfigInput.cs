using BloomEngine.Config.Inputs.Base;
using BloomEngine.Config.Services;
using BloomEngine.Helpers;
using Il2CppReloaded.Input;
using MelonLoader;
using System.Text;
using UnityEngine;

namespace BloomEngine.Config.Inputs;

/// <summary>
/// A config input type which contains UI implementation for handling <see cref="int"/> input.<br/>
/// To create an <see cref="IntConfigInput"/>, use <see cref="ConfigService.CreateInt(string, string, int, ConfigInputOptions{int})"/>
/// </summary>
public sealed class IntConfigInput : TypedConfigInput<int>
{
    /// <summary>
    /// The UI textbox which corresponds to this config input in the config panel.
    /// </summary>
    public ReloadedInputField Textbox { get; private set; }

    internal IntConfigInput(string name, string description, int defaultValue, ConfigInputOptions<int> options) : base(name, description, defaultValue, options) { }

    internal override GameObject CreateInputObject(RectTransform parent)
    {
        Textbox = UIHelper.CreateTextField(InputObjectName, parent, ValueType.Name, onTextChanged: _ => OnUIChanged());
        return Textbox.gameObject;
    }

    internal override void UpdateFromUI() => Value = (int)ValidateNumericInput(Textbox.text, typeof(int));

    /// <inheritdoc/>
    protected override void SetDisplayedValue(int value) => Textbox.SetTextWithoutNotify(value.ToString());

    internal override void OnUIChanged()
    {
        // Perform basic sanitization on live input change
        Textbox.SetTextWithoutNotify(SanitizeNumericInput(Textbox.text));
        base.OnUIChanged();
    }

    /// <summary>
    /// Performs basic sanitisation of numeric input strings to be used for live input fields. Does not perform clamping, parsing or type conversion.
    /// </summary>
    private static string SanitizeNumericInput(string input) => new string([.. input.Where(c => char.IsDigit(c) || c == '-' || c == '+' || c == '.')]);

    /// <summary>
    /// Performs full validation of a string input for a numeric type, including sanitisation, parsing and clamping to the type's min/max values.
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
            return Convert.ChangeType(0, type);

        var minField = type.GetField("MinValue");
        var maxField = type.GetField("MaxValue");

        double min = Convert.ToDouble(minField.GetValue(null));
        double max = Convert.ToDouble(maxField.GetValue(null));
        double val = double.Parse(input);

        return Convert.ChangeType(Math.Clamp(val, min, max), type);
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
            if ((c == '-' || c == '+') && i == 0 && !hasSign)
            {
                sb.Append(c);
                hasSign = true;
            }
            // Decimal point if one doesnt already exist and the type allows it
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