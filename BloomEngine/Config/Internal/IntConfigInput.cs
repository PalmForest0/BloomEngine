using BloomEngine.Utilities;
using Il2CppReloaded.Input;
using MelonLoader;
using System.Text;
using UnityEngine;

namespace BloomEngine.Config.Internal;

public sealed class IntConfigInput(string name, int value, Action<int> onValueChanged, Action onInputChanged, Func<int, int> transformValue, Func<int, bool> validateValue) : BaseConfigInput<int>(name, value, onValueChanged, onInputChanged, transformValue, validateValue)
{
    public ReloadedInputField Textbox { get; private set; }

    public override void SetInputObject(GameObject inputObject)
    {
        base.SetInputObject(inputObject);
        Textbox = inputObject.GetComponent<ReloadedInputField>();
    }

    public override void UpdateFromUI() => Value = (int)ValidateNumericInput(Textbox.text, typeof(int));
    public override void RefreshUI() => Textbox.SetTextWithoutNotify(Value.ToString());
    public override void OnUIChanged()
    {
        Textbox.SetTextWithoutNotify(SanitiseNumericInput(Textbox.text));
        base.OnUIChanged();
    }

    /// <summary>
    /// Performs basic sanitisation of numeric input strings to be used for live input fields. Does not perform clamping, parsing or type conversion.
    /// </summary>
    private static string SanitiseNumericInput(string input) => new string(input.Where(c => char.IsDigit(c) || c == '-' || c == '+' || c == '.').ToArray());

    /// <summary>
    /// Performs full validation of a string input for a numeric type, including sanitisation, parsing and clamping to the type's min/max values.
    /// </summary>
    private static object ValidateNumericInput(string input, Type type)
    {
        if (!TypeHelper.IsNumericType(type))
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
}