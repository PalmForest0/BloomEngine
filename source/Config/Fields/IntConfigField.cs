using System.Globalization;
using BloomEngine.Helpers;
using BloomEngine.UI;
using Il2CppReloaded.Input;
using UnityEngine;

namespace BloomEngine.Config.Fields;

/// <summary>
/// A config field which displays and processes an <see cref="int"/> value using a numeric textbox.
/// </summary>
public sealed class IntConfigField(string name, int defaultValue)
    : ConfigField<int, IntConfigField>(name, defaultValue)
{
    private ReloadedInputField textbox = null!;

    /// <inheritdoc/>
    protected override GameObject CreateInputObject(RectTransform parent, string name, Action<int> onInputChanged)
    {
        textbox = UIHelper.CreateTextbox(name, Value.ToString(CultureInfo.InvariantCulture), parent, onTextChanged: val =>
        {
            textbox.SetTextWithoutNotify(StringHelper.SanitizeNumericInput(val));
            onInputChanged.Invoke(GetInputValue());
        });
        
        return textbox.gameObject;
    }
    
    /// <inheritdoc/>
    protected override int GetInputValue() => (int)StringHelper.ValidateNumericInput(textbox.text, typeof(int));

    /// <inheritdoc/>
    protected override void SetInputValue(int inputValue) => textbox.SetTextWithoutNotify(inputValue.ToString(CultureInfo.InvariantCulture));
}