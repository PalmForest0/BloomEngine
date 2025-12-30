using BloomEngine.ModMenu.Services;
using BloomEngine.Utilities;
using Il2CppReloaded.Input;
using MelonLoader;
using System.Text;
using UnityEngine;

namespace BloomEngine.Config.Inputs;

/// <summary>
/// Creates an <see cref="int"/> input field in the form of a numeric textbox in your mod's config menu and returns it.
/// To register your config with the mod menu, make all your input fields publicly accessible in a static config class
/// and call <see cref="ModMenuEntry.AddConfig(Type)"/>, passing in your config class as the type.
/// </summary>
/// <param name="name">The name of this input field, which will be displayed in the config menu.</param>
/// <param name="description">The description of this input field, which will be displayed in the config menu.</param>
/// <param name="defaultValue">This default value of this config input field.</param>
/// <param name="onValueChanged">An action to run when the value is updated in the config.</param>
/// <param name="onInputChanged">An action to run every time the input is changed in the config.</param>
/// <param name="transformValue">A transformer function that modifies the new value before it is updated.</param>
/// <param name="validateValue">A function to validate the new value before it is updated.</param>
public sealed class IntConfigInput(string name, string description, int defaultValue, Action<int> onValueChanged = null, Action onInputChanged = null, Func<int, int> transformValue = null, Func<int, bool> validateValue = null)
    : TypedConfigInput<int>(name, description, defaultValue, onValueChanged, onInputChanged, transformValue, validateValue)
{
    public ReloadedInputField Textbox { get; private set; }

    internal override GameObject CreateInputObject(RectTransform parent)
    {
        Textbox = UIHelper.CreateTextField(InputObjectName, parent, ValueType.Name, onTextChanged: _ => OnUIChanged());
        return Textbox.gameObject;
    }

    internal override void UpdateFromUI() => Value = (int)ValidateNumericInput(Textbox.text, typeof(int));
    internal override void RefreshUI() => Textbox.SetTextWithoutNotify(Value.ToString());
    internal override void OnUIChanged()
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