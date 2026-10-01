using BloomEngine.UI;
using Il2CppReloaded.Input;
using UnityEngine;

namespace BloomEngine.Config.Fields;

/// <summary>
/// A config field which displays and processes a <see cref="string"/> value using a textbox.
/// </summary>
public sealed class StringConfigField(string identifier, string displayName, string defaultValue)
    : ConfigField<string, StringConfigField>(identifier, displayName, defaultValue)
{
    private ReloadedInputField textbox = null!;

    /// <inheritdoc/>
    protected override GameObject CreateInputObject(RectTransform parent, string name, Action<StringConfigField> onInputChanged)
    {
        textbox = UIHelper.CreateTextbox(name, Value, parent, onTextChanged: _ => onInputChanged(this));
        return textbox.gameObject;
    }

    /// <inheritdoc/>
    protected override string GetInputValue() => textbox.text;

    /// <inheritdoc/>
    protected override void SetInputValue(string inputValue) => textbox.SetTextWithoutNotify(inputValue);
}